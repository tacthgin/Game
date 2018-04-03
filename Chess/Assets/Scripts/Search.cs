using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Search
{
    /// <summary>
    /// 最大搜索深度
    /// </summary>
    public const int LIMIT_DEPTH = 32;

    /// <summary>
    /// 置换表大小
    /// </summary>
    public const int HASH_SIZE = 1 << 20;

    /// <summary>
    /// 开局库大小
    /// </summary>
    public const int BOOK_SIZE = 16384;

    /// <summary>
    /// ALPHA节点的置换表项
    /// </summary>
    public const int HASH_ALPHA = 1;

    /// <summary>
    /// BETA节点的置换表项
    /// </summary>
    public const int HASH_BETA = 2;

    /// <summary>
    /// PV节点的置换表项
    /// </summary>
    public const int HASH_PV = 3;

    /// <summary>
    /// 生成吃子走法
    /// </summary>
    public const bool GEN_CAPTURE = true;

    /// <summary>
    /// 电脑走的棋
    /// </summary>
    private int mvResult;

    /// <summary>
    /// 历史表
    /// </summary>
    private int[] historyTable = new int[65536];

    /// <summary>
    /// MVV/LVA每种
    /// </summary>
    private byte[] ucMvvLva = new byte[24]
    {
        0, 0, 0, 0, 0, 0, 0, 0,
        5, 1, 1, 3, 4, 3, 2, 0,
        5, 1, 1, 3, 4, 3, 2, 0
    };

    /// <summary>
    /// 杀手走法表
    /// </summary>
    private int[,] mvKillers = new int[LIMIT_DEPTH, 2];

    /// <summary>
    /// 置换表项结构
    /// </summary>
    class HashItem
    {
        private int depth = 0;
        private int flag = 0;
        private int mvValue = 0;
        private int mv = 0;
        private uint lock0 = 0;
        private uint lock1 = 0;

        public int Depth
        {
            set { depth = value; }
            get { return depth; }
        }

        public int Flag
        {
            set { flag = value; }
            get { return flag; }
        }

        public int MvValue
        {
            set { mvValue = value; }
            get { return mvValue; }
        }

        public int Mv
        {
            set { mv = value; }
            get { return mv; }
        }

        public uint Lock0
        {
            set { lock0 = value; }
            get { return lock0; }
        }

        public uint Lock1
        {
            set { lock1 = value; }
            get { return lock1; }
        }
    };

    /// <summary>
    /// 置换表
    /// </summary>
    HashItem[] hashTable = new HashItem[HASH_SIZE];

    public class BookItem
    {
        public uint bookLock = 0;
        public ushort mv = 0;
        public ushort value = 0;
    };

    /// <summary>
    /// 开局库
    /// </summary>
    private BookItem[] bookTable;

    /// <summary>
    /// 开局库的实际大小
    /// </summary>
    private int bookSize = 0;

    /// <summary>
    /// 局面实例
    /// </summary>
    private Situation situation;

    private ChessLogic chessLogic;

    public int MvResult
    {
        set { }
        get { return mvResult; }
    }

    public int[,] MvKillers
    {
        set { }
        get { return mvKillers; }
    }

    public Situation MySituation
    {
        set { }
        get { return situation; }
    }

    public class HistoryCompare : IComparer<int>
    {
        private int[] historyTable;
        public HistoryCompare(int[] table)
        {
            historyTable = table;
        }

        public int Compare(int x, int y)
        {
            return historyTable[y] - historyTable[x];
        }
    }

    public HistoryCompare GetHistoryCompare()
    {
        return new HistoryCompare(historyTable);
    }

    public Search(ChessLogic c, Situation s)
    {
        situation = s;
        chessLogic = c;
    }

    /// <summary>
    /// 求MVV/LVA值
    /// </summary>
    /// <param name="mv"></param>
    /// <returns></returns>
    public int MvvLva(int mv)
    {
        return (ucMvvLva[situation.CurrentBoard[chessLogic.Dst(mv)]] << 3) - ucMvvLva[situation.CurrentBoard[chessLogic.Src(mv)]];
    }

    /// <summary>
    /// 按MVV/LVA值排序的比较函数
    /// </summary>
    /// <param name="mv1"></param>
    /// <param name="mv2"></param>
    /// <returns></returns>
    public int CompareMvvLva(int mv1, int mv2)
    {
        return MvvLva(mv2) - MvvLva(mv1);
    }

    public class MvvLvaCompare : IComparer<int>
    {
        private Search search;
        public MvvLvaCompare(Search s)
        {
            search = s;
        }

        public int Compare(int x, int y)
        {
            return search.CompareMvvLva(x, y);
        }
    }

    /// <summary>
    /// 开局库比较函数
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public int CompareBook(BookItem l, BookItem r)
    {
        return l.bookLock > r.bookLock ? 1 : (l.bookLock < r.bookLock ? -1 : 0);
    }

    public class BookCompare : IComparer<BookItem>
    {
        private Search search;
        public BookCompare(Search s)
        {
            search = s;
        }

        public int Compare(BookItem l, BookItem r)
        {
            return search.CompareBook(l, r);
        }
    }

    /// <summary>
    /// 加载开局库
    /// </summary>
    public void LoadBook()
    {
        String path = Application.dataPath + "/File/BOOK.DAT";
        FileStream f = File.Open(path, FileMode.Open);
        byte[] buffer = new byte[(int)f.Length];
        f.Read(buffer, 0, buffer.Length);
        f.Close();

        bookSize = buffer.Length / 8;
        bookTable = new BookItem[bookSize];
        int offset = 0;
        for (int i = 0; i < bookSize; i++)
        {
            BookItem item = new BookItem();
            item.bookLock = BitConverter.ToUInt32(buffer, offset);
            offset += 4;
            item.mv = BitConverter.ToUInt16(buffer, offset);
            offset += 2;
            item.value = BitConverter.ToUInt16(buffer, offset);
            offset += 2;
            bookTable[i] = item;
        }
    }

    /// <summary>
    /// 搜索开局库
    /// </summary>
    /// <returns></returns>
    public int SearchBook()
    {
        //搜索开局库的过程有以下几个步骤
        //1.没有开局库直接返回
        if(bookSize == 0)
        {
            return 0;
        }
        //2.搜索当前局面
        bool mirror = false;
        BookItem itemToSearch = new BookItem();
        itemToSearch.bookLock = situation.Zobr.Lock1;
        int index = Array.BinarySearch(bookTable, itemToSearch, new BookCompare(this));
        //3.如果没有找到，那么搜索当前局面的镜像局面
        if(index < 0)
        {
            mirror = true;
            Situation mirrorSituation = new Situation();
            mirrorSituation.Init(chessLogic);
            situation.Mirror(ref mirrorSituation);
            itemToSearch.bookLock = mirrorSituation.Zobr.Lock1;
            index = Array.BinarySearch(bookTable, itemToSearch, new BookCompare(this));
        }
        //4.如果镜像局面也没找到，就返回
        if(index < 0)
        {
            return 0;
        }
        //5.如果找到，则向前查第一个开局库项
        while(index >= 0 && itemToSearch.bookLock == bookTable[index].bookLock)
        {
            --index;
        }
        ++index;
        //6.把走法和分值写入到"mvs"和"values"数组中
        int value = 0;
        int bookMoves = 0;
        int[] mvs = new int[ChessLogic.MAX_GENERATE_MOVES];
        int[] values = new int[ChessLogic.MAX_GENERATE_MOVES];
        int mv = 0;
        while(index < bookSize && itemToSearch.bookLock == bookTable[index].bookLock)
        {
            mv = (mirror ? chessLogic.MirrorMove(bookTable[index].mv) : bookTable[index].mv);
            if(situation.LegalMove(mv))
            {
                mvs[bookMoves] = mv;
                values[bookMoves] = bookTable[index].value;
                value += values[bookMoves];
                ++bookMoves;
                if(bookMoves == ChessLogic.MAX_GENERATE_MOVES)
                {
                    break; //防止BOOK.DAT中含有异常数据
                }
            }
            ++index;
        }
        if(value == 0)
        {
            return 0; //防止BOOK.DAT中含有异常数据
        }
        //7.根据权重随机选择一个走法
        value = (new System.Random().Next()) % value;
        int valueIndex = 0;
        for(valueIndex = 0; valueIndex < bookMoves; valueIndex++)
        {
            value -= values[valueIndex];
            if(value < 0)
            {
                break;
            }
        }

        return mvs[valueIndex];
    }

    /// <summary>
    /// 初始化置换表
    /// </summary>
    private void InitHashTable()
    {
        //清空置换表
        Array.Clear(hashTable, 0, HASH_SIZE);
        for (int i = 0; i < HASH_SIZE; i++)
        {
            hashTable[i] = new HashItem();
        }
    }

    /// <summary>
    /// 提取置换表项
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <param name="depth"></param>
    /// <param name="mv"></param>
    /// <returns></returns>
    public int ProbeHash(int alpha, int beta, int depth, out int mv)
    {
        //杀棋标志：如果是杀棋，那么不需要满足深度条件
        bool mate = false;
        HashItem hsh = hashTable[situation.Zobr.Key & (HASH_SIZE - 1)];
        if(hsh.Lock0 != situation.Zobr.Lock0 || hsh.Lock1 != situation.Zobr.Lock1)
        {
            mv = 0;
            return -ChessLogic.MATE_VALUE;
        }

        mv = hsh.Mv;
        if(hsh.MvValue > ChessLogic.WIN_VALUE)
        {
            hsh.MvValue -= situation.Distance;
            mate = true;
        }else if(hsh.MvValue < -ChessLogic.WIN_VALUE)
        {
            hsh.MvValue += situation.Distance;
            mate = true;
        }

        if(hsh.Depth > depth || mate)
        {
            if(hsh.Flag == HASH_BETA)
            {
                return (hsh.MvValue >= beta ? hsh.MvValue : -ChessLogic.MATE_VALUE);
            }else if(hsh.Flag == HASH_ALPHA)
            {
                return (hsh.MvValue <= alpha ? hsh.MvValue : -ChessLogic.MATE_VALUE);
            }
            return hsh.MvValue;
        }

        return -ChessLogic.MATE_VALUE;
    }

    /// <summary>
    /// 保存置换表项
    /// </summary>
    /// <param name="flag"></param>
    /// <param name="value"></param>
    /// <param name="depth"></param>
    /// <param name="mv"></param>
    public void RecordHash(int flag, int value, int depth, int mv)
    {
        HashItem hsh = hashTable[situation.Zobr.Key & (HASH_SIZE - 1)];
        if(hsh.Depth > depth)
        {
            return;
        }

        hsh.Flag = flag;
        hsh.Depth = depth;
        if(value > ChessLogic.WIN_VALUE)
        {
            hsh.MvValue = value + situation.Distance;
        }else if(value < -ChessLogic.WIN_VALUE)
        {
            hsh.MvValue = value - situation.Distance;
        }else
        {
            hsh.MvValue = value;
        }

        hsh.Mv = mv;
        hsh.Lock0 = situation.Zobr.Lock0;
        hsh.Lock1 = situation.Zobr.Lock1;
        hashTable[situation.Zobr.Key & (HASH_SIZE - 1)] = hsh;
    }

    /// <summary>
    /// 对最佳走法的处理
    /// </summary>
    /// <param name="mv"></param>
    /// <param name="depth"></param>
    public void SetBestMove(int mv, int depth)
    {
        historyTable[mv] += depth * depth;
        if(mvKillers[situation.Distance, 0] != mv)
        {
            mvKillers[situation.Distance, 1] = mvKillers[situation.Distance, 0];
            mvKillers[situation.Distance, 0] = mv;
        }
    }

    /// <summary>
    /// 静态(Quiescence)搜索过程
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    public int SearchQuiesc(int alpha, int beta)
    {
        //静态搜索分为以下几个阶段
        //1.检查重复局面
        int value = situation.RepStatus();
        if(value != 0)
        {
            return situation.RepValue(value);
        }
        
        //2.到达极限深度就返回局面评价
        if(situation.Distance == LIMIT_DEPTH)
        {
            return situation.Evaluate();
        }

        //3.初始化最佳值
        int bestValue = -ChessLogic.MATE_VALUE; //这样可以知道，是否一个走法都没走过(杀棋)

        int[] mvs = new int[ChessLogic.MAX_GENERATE_MOVES];
        int genMoves = 0;
        if (situation.InCheck())
        {
            //4.如果被将军，则生成全部走法
            genMoves = situation.GenerateMoves(out mvs);
            Array.Sort(mvs, 0, genMoves, new HistoryCompare(historyTable));
        }else
        {
            //5.如果不被将军，先做局面评价
            value = situation.Evaluate();
            if(value > bestValue)
            {
                bestValue = value;
                if(value >= beta)
                {
                    return value;
                }

                if(value > alpha)
                {
                    alpha = value;
                }
            }

            //6.如果局面评价没有截断，再生成吃子走法
            genMoves = situation.GenerateMoves(out mvs, GEN_CAPTURE);
            Array.Sort(mvs, 0, genMoves, new MvvLvaCompare(this));
        }

        //7.逐一走这些走法，并进行递归
        for(int i = 0; i < genMoves; i++)
        {
            if(situation.MakeMove(mvs[i]))
            {
                value = -SearchQuiesc(-beta, -alpha);
                situation.UndoMakeMove();

                //8.进行Alpha-Beta大小判断和截断
                if(value > bestValue) //找到最佳值(但不能确定是Alpha、PV还是beta走法)
                {
                    bestValue = value; //bestValue是目前要返回的最佳值，可能超出Alpha-Beta边界
                    if(value >= beta) //找到一个Beta走法
                    {
                        return value; //Beta截断
                    }
                    if(value > alpha) //找到一个PV走法
                    {
                        alpha = value; //缩小Alpha-Beta边界
                    }
                }
            }
        }

        //9.所有走法都搜索完了，返回最佳值
        return bestValue == -ChessLogic.MATE_VALUE ? situation.Distance - ChessLogic.MATE_VALUE : bestValue;
    }

    /// <summary>
    /// 超出边界(Fail-Soft)的Alpha-Beta搜索过程
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <param name="depth"></param>
    /// <param name="noNull"></param>
    /// <returns></returns>
    public int SearchFull(int alpha, int beta, int depth, bool noNull = false)
    {
        //一个Alpha-Beta完全搜索分为以下几个阶段
        int value = 0;
        int mvHash = 0;
        if(situation.Distance > 0)
        {
            //1.到达水平线，则调用静态搜索(注意：由于空步裁剪，深度可能小于零)
            if (depth <= 0)
            {
                return SearchQuiesc(alpha, beta);
            }

            //1-1.检查重复局面(注意：不要再根节点检查，否则就没有走法了)
            value = situation.RepStatus();
            if(value != 0)
            {
                return situation.RepValue(value);
            }

            //1-2.到达极限深度就返回局面评价
            if(situation.Distance == LIMIT_DEPTH)
            {
                return situation.Evaluate();
            }

            //1-3.尝试置换表裁剪，并得到置换表走法
            value = ProbeHash(alpha, beta, depth, out mvHash);
            if(value > -ChessLogic.MATE_VALUE)
            {
                return value;
            }

            //1-5.尝试空步裁剪(根节点的Beta值是"MATE_VALUE"，所以不可能发生空步裁剪)
            if(!noNull && !situation.InCheck() && situation.NullOkey())
            {
                situation.NullMove();
                value = -SearchFull(-beta, 1 - beta, depth - ChessLogic.NULL_DEPTH - 1, true);
                situation.UndoNullMove();
                if(value > beta)
                {
                    return value;
                }
            }
        }else
        {
            mvHash = 0;
        }

        //2.初始化最佳值和最佳走法
        int bestValue = -ChessLogic.MATE_VALUE; //是否一个走法都没走过(杀棋)
        int mvBest = 0; //是否搜索到了Beta走法或pv走法，以便保存到历史表
        int hashFlag = HASH_ALPHA;

        //3.初始化走法排序结构
        SortInfo sort = new SortInfo();
        sort.Init(mvHash, this);

        //4.遍历走法
        int mv = 0;
        while((mv = sort.Next()) != 0)
        {
            if (situation.MakeMove(mv))
            {
                //将军延伸
                value = -SearchFull(-beta, -alpha, situation.InCheck() ? depth : depth - 1);
                situation.UndoMakeMove();

                //5.进行Alpha-Beta大小判断和截断
                if (value > bestValue) //找到最佳值(但不确定是Alpha、PV还是Beta走法)
                {
                    bestValue = value; //bestValue就是目前要返回的最佳值，可能超出Alpha-Beta边界

                    if (value >= beta) //找到一个Beta走法
                    {
                        mvBest = mv;
                        break;
                    }

                    if (value > alpha) //找到一个PV走法
                    {
                        mvBest = mv; //PV走法要保存到历史表
                        alpha = value;
                    }
                }
            }
        }

        //6.所有走法都搜索完了，把最佳走法(不能是Alpha走法)保存到历史表，返回最佳值
        if(bestValue == -ChessLogic.MATE_VALUE)
        {
            //如果是杀棋，就根据最佳走法保存到历史表
            return situation.Distance - ChessLogic.MATE_VALUE;
        }

        //记录到置换表
        RecordHash(hashFlag, bestValue, depth, mvBest);

        if (mvBest != 0)
        {
            //如果不是Alpha走法，就将最佳走法保存到历史表中
            SetBestMove(mvBest, depth);
            if (situation.Distance == 0)
            {
                //搜索根节点时，总是有一个最佳走法(因为全窗口搜索不会超出边界)，将这个走法保存下来
                mvResult = mvBest;
            }
        }

        return bestValue;
    }

    /// <summary>
    /// 迭代加深搜索过程
    /// </summary>
    public void SearchMain()
    {
        //清空历史表
        Array.Clear(historyTable, 0, 65536);
        //清空杀手走法表
        Array.Clear(mvKillers, 0, LIMIT_DEPTH);
        //初始化置换表
        InitHashTable();

        //初始化定时器
        DateTime srcTime = DateTime.UtcNow;
        //初始化步数
        situation.Distance = 0;

        mvResult = SearchBook();
        if(mvResult != 0)
        {
            situation.MakeMove(mvResult);
            if(situation.RepStatus(3) == 0)
            {
                situation.UndoMakeMove();
                return;
            }
            situation.UndoMakeMove();
        }

        //检查是否只有唯一走法
        int value = 0;
        int[] mvs = new int[ChessLogic.MAX_GENERATE_MOVES];
        int genMoves = situation.GenerateMoves(out mvs);
        for(int i = 0; i < genMoves; i++)
        {
            if(situation.MakeMove(mvs[i]))
            {
                situation.UndoMakeMove();
                mvResult = mvs[i];
                value++;
            }
        }
        if (value == 1) return;

        //迭代加深过程
        for(int i = 1; i <= LIMIT_DEPTH; i++)
        {
            value = SearchFull(-ChessLogic.MATE_VALUE, ChessLogic.MATE_VALUE, i);
            //搜索到杀棋，就停止搜索
            if (value > ChessLogic.WIN_VALUE || value < -ChessLogic.WIN_VALUE)
            {
                break;
            }

            //搜索大于一秒就停止搜索
            TimeSpan span = DateTime.UtcNow - srcTime;
            if (span.TotalMilliseconds > 500)
            {
                break;
            }
        }
    }
}
