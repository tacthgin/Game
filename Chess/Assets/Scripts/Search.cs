using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Search
{
    /// <summary>
    /// 最大搜索深度
    /// </summary>
    public const int LIMIT_DEPTH = 32;

    /// <summary>
    /// 搜索出胜负的分值界限，超出此值就说明已经搜索出杀棋了
    /// </summary>
    public const int WIN_VALUE = ChessLogic.MATE_VALUE - 100;

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
    /// 局面实例
    /// </summary>
    private Situation situation;

    private ChessLogic chessLogic;

    public int MvResult
    {
        set { }
        get { return mvResult; }
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
        return ucMvvLva[situation.CurrentBoard[chessLogic.Dst(mv)] << 3] - ucMvvLva[situation.CurrentBoard[chessLogic.Src(mv)]];
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

            //1-3.尝试空步裁剪(根节点的Beta值是"MATE_VALUE"，所以不可能发生空步裁剪)
            if(!noNull && !situation.InCheck() && situation.NullOkey())
            {
                situation.NullMove();
                value = -SearchFull(-beta, 1 - beta, depth - ChessLogic.NULL_DEPTH - 1, ChessLogic.NO_NULL);
            }

        }
        

        //初始化最佳值和最佳走法
        int bestValue = -ChessLogic.MATE_VALUE; //是否一个走法都没走过(杀棋)
        int mvBest = 0; //是否搜索到了Beta走法或pv走法，以便保存到历史表

        //生成全部走法，并根据历史表排序
        int[] mvs = new int[ChessLogic.MAX_GENERATE_MOVES];
        int genMoves = situation.GenerateMoves(out mvs);
        Array.Sort(mvs, 0, genMoves, new HistoryCompare(historyTable));

        //遍历走法
        int pcCaptured = 0;
        int value = 0;
        for(int i = 0; i < genMoves; i++)
        {
            if(situation.MakeMove(mvs[i]))
            {
                value = -SearchFull(-beta, -alpha, depth - 1);
                situation.UndoMakeMove();

                //进行Alpha-Beta大小判断和截断
                if(value > bestValue) //找到最佳值(但不确定是Alpha、PV还是Beta走法)
                {
                    bestValue = value; //bestValue就是目前要返回的最佳值，可能超出Alpha-Beta边界

                    if (value >= beta) //找到一个Beta走法
                    {
                        mvBest = mvs[i];
                        break;
                    }

                    if(value > alpha) //找到一个PV走法
                    {
                        mvBest = mvs[i]; //PV走法要保存到历史表
                        alpha = value;
                    }
                }
            }
        }

        //所有走法都搜索完了，把最佳走法(不能是Alpha走法)保存到历史表，返回最佳值
        if(bestValue == -ChessLogic.MATE_VALUE)
        {
            //如果是杀棋，就根据最佳走法保存到历史表
            return situation.Distance - ChessLogic.MATE_VALUE;
        }
        if (mvBest != 0)
        {
            //如果不是Alpha走法，就将最佳走法保存到历史表中
            historyTable[mvBest] += depth * depth;
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
        //初始化历史表
        Array.Clear(historyTable, 0, 65536);
        //初始化定时器
        DateTime srcTime = DateTime.UtcNow;
        //初始化步数
        situation.Distance = 0;

        //迭代加深过程
        int value = 0;
        for(int i = 1; i <= LIMIT_DEPTH; i++)
        {
            value = SearchFull(-ChessLogic.MATE_VALUE, ChessLogic.MATE_VALUE, i);
            //搜索到杀棋，就停止搜索
            if (value > WIN_VALUE || value < -WIN_VALUE)
            {
                break;
            }

            //搜索大于一秒就停止搜索
            if((DateTime.UtcNow - srcTime).TotalMilliseconds > 1000)
            {
                break;
            }
        }
    }
}
