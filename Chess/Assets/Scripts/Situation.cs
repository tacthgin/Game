/// <summary>
/// 局面结构
/// </summary>
public class Situation
{
    /// <summary>
    /// 最大历史走法数
    /// </summary>
    public const int MAX_MOVES = 256;

    /// <summary>
    /// 轮到谁走 0=红方，1=黑方
    /// </summary>
    private int sdPlayer = 0;

    /// <summary>
    /// 是否翻转棋盘
    /// </summary>
    private bool fliped = false;

    /// <summary>
    /// 棋盘
    /// </summary>
    private sbyte[] currentBoard = new sbyte[256];

    /// <summary>
    /// 红方子力价值
    /// </summary>
    private int redValue = 0;

    /// <summary>
    /// 黑方子力价值
    /// </summary>
    private int blackValue = 0;

    /// <summary>
    /// 距离根节点的步数
    /// </summary>
    private int distance = 0;

    /// <summary>
    /// 历史走法数
    /// </summary>
    private int moveNum = 0;

    /// <summary>
    /// 历史走法信息列表
    /// </summary>
    MoveInfo[] mvsList = new MoveInfo[MAX_MOVES];

    /// <summary>
    /// Zobrist
    /// </summary>
    Zobrist zobr = new Zobrist();

    private ChessLogic logic;

    public sbyte[] CurrentBoard
    {
        set { }
        get { return currentBoard; }
    }

    public int SdPlayer
    {
        set { }
        get { return sdPlayer; }
    }

    public bool Filped
    {
        set { }
        get { return fliped; }
    }

    public int Distance
    {
        set { distance = value; }
        get { return distance; }
    }

    public int MoveNum
    {
        set { }
        get { return moveNum; }
    }

    public Zobrist Zobr
    {
        set { }
        get { return zobr; }
    }

    /// <summary>
    /// 清空历史走法信息
    /// </summary>
    public void SetIrrev()
    {
        System.Array.Clear(mvsList, 0, MAX_MOVES);
        for (int i = 0; i < MAX_MOVES; i++)
        {
            mvsList[i] = new MoveInfo();
        }
        mvsList[0].Set(0, 0, Checked(), zobr.Key);
        moveNum = 1;
    }

    /// <summary>
    /// 初始化局面
    /// </summary>
    /// <param name="chessLogic"></param>
    public void Init(ChessLogic chessLogic)
    {
        logic = chessLogic;
    }

    /// <summary>
    /// 清空棋盘
    /// </summary>
    public void ClearBoard()
    {
        sdPlayer = 0;
        blackValue = 0;
        redValue = 0;
        distance = 0;
        System.Array.Clear(currentBoard, 0, 256);
        zobr.InitZero();
    }

    /// <summary>
    /// 初始化棋盘
    /// </summary>
    /// <param name="red"></param>
    public void Startup(bool red)
    {
        ClearBoard();
        fliped = !red;
        int pc = 0;
        for (int sq = 0; sq < 256; sq++)
        {
            pc = logic.startupChessBoard[sq];
            if (pc != 0)
            {
                AddPiece(sq, pc);
            }
        }
 
        SetIrrev();
    }

    /// <summary>
    /// 交换走子方
    /// </summary>
    public void ChangeSide()
    {
        sdPlayer = 1 - sdPlayer;
        zobr.Xor(ref ZobristTable.player);
    }

    /// <summary>
    /// 棋盘上放置一枚棋子
    /// </summary>
    /// <param name="sq">棋子编号</param>
    /// <param name="pc">棋子的值</param>
    public void AddPiece(int sq, int pc)
    {
        currentBoard[sq] = (sbyte)pc;
        //红方加分，黑方(piecePosValue取值要颠倒)减分
        if (pc < 16)
        {
            redValue += logic.piecePosValue[pc - 8, sq];
            zobr.Xor(ref ZobristTable.table[pc - 8, sq]);
        }
        else
        {
            blackValue += logic.piecePosValue[pc - 16, logic.SquareFilp(sq)];
            zobr.Xor(ref ZobristTable.table[pc - 9, sq]);
        }
    }

    /// <summary>
    /// 棋盘上拿走一枚棋子
    /// </summary>
    /// <param name="sq">棋子编号</param>
    /// <param name="pc"></param>
    public void DeletePiece(int sq, int pc)
    {
        currentBoard[sq] = 0;
        if (pc < 16)
        {
            redValue -= logic.piecePosValue[pc - 8, sq];
            zobr.Xor(ref ZobristTable.table[pc - 8, sq]);
        }
        else
        {
            blackValue -= logic.piecePosValue[pc - 16, logic.SquareFilp(sq)];
            zobr.Xor(ref ZobristTable.table[pc - 9, sq]);
        }
    }

    /// <summary>
    /// 局面评价函数
    /// </summary>
    /// <returns></returns>
    public int Evaluate()
    {
        return (sdPlayer == 0 ? redValue - blackValue : blackValue - redValue) + ChessLogic.ADVANCED_VALUE;
    }

    /// <summary>
    /// 是否被将军
    /// </summary>
    /// <returns></returns>
    public bool InCheck()
    {
        return mvsList[moveNum - 1].Check;
    }

    /// <summary>
    /// 上一步是否吃子
    /// </summary>
    /// <returns></returns>
    public bool Captured()
    {
        return mvsList[moveNum - 1].PcCaptured != 0;
    }

    /// <summary>
    /// 搬一步棋的棋子
    /// </summary>
    /// <param name="mv"></param>
    /// <returns></returns>
    public int MovePiece(int mv)
    {
        int sqSrc, sqDst;
        sqSrc = logic.Src(mv);
        sqDst = logic.Dst(mv);
        sbyte pcCaptured = currentBoard[sqDst];
        if (pcCaptured != 0)
        {
            DeletePiece(sqDst, pcCaptured);
        }
        sbyte pc = currentBoard[sqSrc];
        DeletePiece(sqSrc, pc);
        AddPiece(sqDst, pc);
        return pcCaptured;
    }

    /// <summary>
    /// 走一步棋
    /// </summary>
    /// <param name="mv"></param>
    /// <returns></returns>
    public bool MakeMove(int mv)
    {
        uint key = zobr.Key;
        int pcCaptured = MovePiece(mv);
        if (Checked())
        {
            UndoMovePiece(mv, pcCaptured);
            return false;
        }
        ChangeSide();
        mvsList[moveNum].Set((ushort)mv, (byte)pcCaptured, Checked(), key);
        ++moveNum;
        ++distance;
        return true;
    }

    /// <summary>
    /// 撤销走一步棋
    /// </summary>
    public void UndoMakeMove()
    {
        --distance;
        --moveNum;
        ChangeSide();
        UndoMovePiece(mvsList[moveNum].Mv, mvsList[moveNum].PcCaptured);
    }

    /// <summary>
    /// 走一步空步
    /// </summary>
    public void NullMove()
    {
        uint key = zobr.Key;
        ChangeSide();
        mvsList[moveNum].Set(0, 0, false, key);
        ++moveNum;
        ++distance;
    }

    /// <summary>
    /// 撤销空步
    /// </summary>
    public void UndoNullMove()
    {
        --distance;
        --moveNum;
        ChangeSide();
    }

    /// <summary>
    /// 生成所有走法
    /// </summary>
    /// <param name="mvs"></param>
    /// <param name="capture">true,只生成吃子走法</param>
    /// <returns></returns>
    public int GenerateMoves(out int[] mvs, bool capture = false)
    {
        int genMoves = 0;
        int pcSelfSide = logic.SideTag(sdPlayer);
        int pcOppSide = logic.oppSideTag(sdPlayer);

        int pcSrc = 0;
        int pcDst = 0;
        int sqDst = 0;
        int[] tempMvs = new int[ChessLogic.MAX_GENERATE_MOVES];
        for (int sqSrc = 0; sqSrc < currentBoard.Length; sqSrc++)
        {
            //找到一个本方棋子，再做以下判断
            pcSrc = currentBoard[sqSrc];
            if ((pcSrc & pcSelfSide) == 0)
            {
                continue;
            }

            //根据棋子确定走法
            switch (pcSrc - pcSelfSide)
            {
                case ChessLogic.PIECE_KING:
                    {
                        for (int i = 0; i < logic.kingDelta.Length; i++)
                        {
                            sqDst = sqSrc + logic.kingDelta[i];
                            if (!logic.InFort(sqDst))
                            {
                                continue;
                            }
                            pcDst = currentBoard[sqDst];
                            //走到不是有自己棋子的地方
                            if (capture ? (pcDst & pcOppSide) != 0 : (pcDst & pcSelfSide) == 0)
                            {
                                tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                ++genMoves;
                            }
                        }
                    }
                    break;
                case ChessLogic.PIECE_ADVISOR:
                    {
                        for (int i = 0; i < logic.advisorDelta.Length; i++)
                        {
                            sqDst = sqSrc + logic.advisorDelta[i];
                            if (!logic.InFort(sqDst))
                            {
                                continue;
                            }
                            pcDst = currentBoard[sqDst];
                            if (capture ? (pcDst & pcOppSide) != 0 : (pcDst & pcSelfSide) == 0)
                            {
                                tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                ++genMoves;
                            }
                        }
                    }
                    break;
                case ChessLogic.PIECE_BISHOP:
                    {
                        for (int i = 0; i < logic.advisorDelta.Length; i++)
                        {
                            sqDst = sqSrc + logic.advisorDelta[i];
                            //象眼位置
                            if (!(logic.InBoard(sqDst) && logic.HomeHalf(sqDst, sdPlayer) && currentBoard[sqDst] == 0))
                            {
                                continue;
                            }
                            //象的步长是士的2倍
                            sqDst += logic.advisorDelta[i];
                            pcDst = currentBoard[sqDst];
                            if (capture ? (pcDst & pcOppSide) != 0 : (pcDst & pcSelfSide) == 0)
                            {
                                tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                ++genMoves;
                            }
                        }
                    }
                    break;
                case ChessLogic.PIECE_KNIGHT:
                    {
                        for (int i = 0; i < logic.kingDelta.Length; i++)
                        {
                            sqDst = sqSrc + logic.kingDelta[i];
                            //马腿位置
                            if (currentBoard[sqDst] != 0)
                            {
                                continue;
                            }

                            for (int j = 0; j < 2; j++)
                            {
                                sqDst = sqSrc + logic.knightDelta[i, j];
                                if(!logic.InBoard(sqDst))
                                {
                                    continue;
                                }
                                pcDst = currentBoard[sqDst];
                                if (capture ? (pcDst & pcOppSide) != 0 : (pcDst & pcSelfSide) == 0)
                                {
                                    tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                    ++genMoves;
                                }
                            }
                        }
                    }
                    break;
                case ChessLogic.PIECE_ROOK:
                    {
                        int delta = 0;
                        for (int i = 0; i < logic.kingDelta.Length; i++)
                        {
                            delta = logic.kingDelta[i];
                            sqDst = sqSrc + delta;
                            while (logic.InBoard(sqDst))
                            {
                                pcDst = currentBoard[sqDst];
                                if (pcDst == 0)
                                {
                                    if(!capture)
                                    {
                                        tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                        ++genMoves;
                                    }
                                }
                                else 
                                {
                                    if ((pcDst & pcOppSide) != 0) //对方棋子位置
                                    { 
                                        tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                        ++genMoves;
                                    }
                                    break;
                                }
                                sqDst += delta;
                            }
                        }
                    }
                    break;
                case ChessLogic.PIECE_CANNON:
                    {
                        int delta = 0;
                        for (int i = 0; i < logic.kingDelta.Length; i++)
                        {
                            delta = logic.kingDelta[i];
                            sqDst = sqSrc + delta;
                            while (logic.InBoard(sqDst))
                            {
                                pcDst = currentBoard[sqDst];
                                if (pcDst == 0)
                                {
                                    if(!capture)
                                    {
                                        tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                        ++genMoves;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                                sqDst += delta;
                            }

                            sqDst += delta;
                            while (logic.InBoard(sqDst))
                            {
                                pcDst = currentBoard[sqDst];
                                if (pcDst != 0 )
                                {
                                    if ((pcDst & pcOppSide) != 0)
                                    {
                                        tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                        ++genMoves;
                                    }
                                    break;
                                }
                                sqDst += delta;
                            }
                        }
                    }
                    break;
                case ChessLogic.PIECE_PAWN:
                    {
                        sqDst = logic.SquareForward(sqSrc, sdPlayer);
                        if (logic.InBoard(sqDst))
                        {
                            pcDst = currentBoard[sqDst];
                            if (capture ? (pcDst & pcOppSide) != 0 : (pcDst & pcSelfSide) == 0)
                            {
                                tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                ++genMoves;
                            }
                        }
                        if (logic.AwayHalf(sqSrc, sdPlayer))
                        {
                            for (int delta = -1; delta <= 1; delta += 2)
                            {
                                sqDst = sqSrc + delta;
                                if (logic.InBoard(sqDst))
                                {
                                    pcDst = currentBoard[sqDst];
                                    if (capture ? (pcDst & pcOppSide) != 0 : (pcDst & pcSelfSide) == 0)
                                    {
                                        tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                        ++genMoves;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }
        mvs = tempMvs;
        return genMoves;
    }

    /// <summary>
    /// 撤销走一步棋
    /// </summary>
    /// <param name="mv"></param>
    /// <param name="pcCaptured"></param>
    public void UndoMovePiece(int mv, int pcCaptured)
    {
        int sqSrc, sqDst;
        sqSrc = logic.Src(mv);
        sqDst = logic.Dst(mv);
        sbyte pc = currentBoard[sqDst];
        DeletePiece(sqDst, pc);
        AddPiece(sqSrc, pc);
        if (pcCaptured != 0)
        {
            AddPiece(sqDst, pcCaptured);
        }
    }

    /// <summary>
    /// 判断走法是否合理
    /// </summary>
    /// <param name="mv"></param>
    /// <returns></returns>
    public bool LegalMove(int mv)
    {
        int sqSrc = logic.Src(mv);
        int pcSrc = currentBoard[sqSrc];
        int pcSelfSide = logic.SideTag(sdPlayer);
        //判断起始的格子是否是自己的棋子
        if ((pcSrc & pcSelfSide) == 0)
        {
            return false;
        }

        int sqDst = logic.Dst(mv);
        int pcDst = currentBoard[sqDst];
        //目标的格子是否自己的棋子
        if ((pcDst & pcSelfSide) != 0)
        {
            return false;
        }

        int sqPin = 0;
        int delta = 0;
        //根据棋子类型判断走法是否合理
        switch (pcSrc - pcSelfSide)
        {
            case ChessLogic.PIECE_KING:
                {
                    return logic.InFort(sqDst) && logic.KingSpan(sqSrc, sqDst);
                }
            case ChessLogic.PIECE_ADVISOR:
                {
                    return logic.InFort(sqDst) && logic.AdvisorSpan(sqSrc, sqDst);
                }
            case ChessLogic.PIECE_BISHOP:
                {
                    return logic.SameHalf(sqSrc, sqDst) && logic.BishopSpan(sqSrc, sqDst) && currentBoard[logic.BishopPin(sqSrc, sqDst)] == 0;
                }
            case ChessLogic.PIECE_KNIGHT:
                {
                    sqPin = logic.KnightPin(sqSrc, sqDst);
                    return sqPin != sqSrc && currentBoard[sqPin] == 0;
                }
            case ChessLogic.PIECE_ROOK:
            case ChessLogic.PIECE_CANNON:
                {
                    if (logic.SameRow(sqSrc, sqDst))
                    {
                        delta = sqDst < sqSrc ? -1 : 1;
                    }
                    else if (logic.SameColumn(sqSrc, sqDst))
                    {
                        delta = sqDst < sqSrc ? -16 : 16;
                    }
                    else
                    {
                        return false;
                    }

                    sqPin = sqSrc + delta;
                    while (sqPin != sqDst && currentBoard[sqPin] == 0)
                    {
                        sqPin += delta;
                    }
                    if (sqPin == sqDst)
                    {
                        return pcDst == 0 || pcSrc - pcSelfSide == ChessLogic.PIECE_ROOK;
                    }
                    else if (pcDst != 0 && pcSrc - pcSelfSide == ChessLogic.PIECE_CANNON)
                    {
                        sqPin += delta;
                        while (sqPin != sqDst && currentBoard[sqPin] == 0)
                        {
                            sqPin += delta;
                        }
                        return sqPin == sqDst;
                    }

                    return false;
                }
            case ChessLogic.PIECE_PAWN:
                {
                    if (logic.AwayHalf(sqSrc, sdPlayer) && (sqDst == sqSrc - 1 || sqDst == sqSrc + 1))
                    {
                        return true;
                    }
                    return sqDst == logic.SquareForward(sqSrc, sdPlayer);
                }
            default:
                return false;
        }
    }

    /// <summary>
    /// 判断是否被将军
    /// </summary>
    /// <returns></returns>
    public bool Checked()
    {
        int sqDst = 0;
        int pcDst = 0;
        int pcSelfSide = logic.SideTag(sdPlayer);
        int pcOppSide = logic.oppSideTag(sdPlayer);
        int delta = 0;
        for (int sqSrc = 0; sqSrc < currentBoard.Length; sqSrc++)
        {
            if (currentBoard[sqSrc] != pcSelfSide + ChessLogic.PIECE_KING)
            {
                continue;
            }
            //被对面的兵将军
            if (currentBoard[logic.SquareForward(sqSrc, sdPlayer)] == pcOppSide + ChessLogic.PIECE_PAWN)
            {
                return true;
            }

            for (delta = -1; delta <= 1; delta += 2)
            {
                if (currentBoard[sqSrc + delta] == pcOppSide + ChessLogic.PIECE_PAWN)
                {
                    return true;
                }
            }

            //被对面的马将军(以士的步长作为马腿)
            for (int i = 0; i < logic.advisorDelta.Length; i++)
            {
                if (currentBoard[sqSrc + logic.advisorDelta[i]] != 0)
                {
                    continue;
                }
                for (int j = 0; j < 2; j++)
                {
                    pcDst = currentBoard[sqSrc + logic.knightCheckDelta[i, j]];
                    if (pcDst == pcOppSide + ChessLogic.PIECE_KNIGHT)
                    {
                        return true;
                    }
                }
            }

            //被对面车或炮将军(包括将帅对脸)
            for (int i = 0; i < logic.kingDelta.Length; i++)
            {
                delta = logic.kingDelta[i];
                sqDst = sqSrc + delta;
                while (logic.InBoard(sqDst))
                {
                    pcDst = currentBoard[sqDst];
                    if (pcDst != 0)
                    {
                        if (pcDst == pcOppSide + ChessLogic.PIECE_ROOK || pcDst == pcOppSide + ChessLogic.PIECE_KING)
                        {
                            return true;
                        }
                        break;
                    }
                    sqDst += delta;
                }
                sqDst += delta;
                while (logic.InBoard(sqDst))
                {
                    pcDst = currentBoard[sqDst];
                    if (pcDst != 0)
                    {
                        if (pcDst == pcOppSide + ChessLogic.PIECE_CANNON)
                        {
                            return true;
                        }
                        break;
                    }
                    sqDst += delta;
                }
            }

            return false;
        }
        return false;
    }

    /// <summary>
    /// 判断是否被杀
    /// </summary>
    /// <returns></returns>
    public bool IsMate()
    {
        int[] mvs = new int[ChessLogic.MAX_GENERATE_MOVES];
        int genMoveNum = GenerateMoves(out mvs);
        int pcCaptured = 0;
        for (int i = 0; i < genMoveNum; i++)
        {
            pcCaptured = MovePiece(mvs[i]);
            if (!Checked())
            {
                UndoMovePiece(mvs[i], pcCaptured);
                return false;
            }
            else
            {
                UndoMovePiece(mvs[i], pcCaptured);
            }
        }

        return true;
    }

    /// <summary>
    /// 和棋分值
    /// </summary>
    /// <returns></returns>
    public int DrawValue()
    {
        return (distance & 1) == 0 ? -ChessLogic.DRAW_VALUE : ChessLogic.DRAW_VALUE;
    }

    /// <summary>
    /// 检测重复局面
    /// </summary>
    /// <param name="recur"></param>
    /// <returns></returns>
    public int RepStatus(int recur = 1)
    {
        bool selfSide = false;
        //本方长将
        bool perpCheck = true;
        //对方长将
        bool oppPerpCheck = true;
        int currentMoveNum = moveNum - 1;
        MoveInfo move = mvsList[currentMoveNum];
        while(move.Mv != 0 && move.PcCaptured == 0)
        {
            if(selfSide)
            {
                perpCheck = perpCheck && move.Check;
                if(move.Key == zobr.Key)
                {
                    recur--;
                    if(recur == 0)
                    {
                        return 1 + (perpCheck ? 2 : 0) + (oppPerpCheck ? 4 : 0);
                    }
                }
            }else
            {
                oppPerpCheck = oppPerpCheck && move.Check;
            }
            selfSide = !selfSide;
            move = mvsList[--currentMoveNum];
        }
        return 0;
    }

    /// <summary>
    /// 重复局面分值
    /// </summary>
    /// <param name="repStatus"></param>
    /// <returns></returns>
    public int RepValue(int repStatus)
    {
        int value = 0;
        value = ((repStatus & 2) == 0 ? 0 : distance - ChessLogic.MATE_VALUE) + ((repStatus & 4) == 0 ? 0 : ChessLogic.MATE_VALUE - distance);
        return value == 0 ? -ChessLogic.DRAW_VALUE : value;
    }

    /// <summary>
    /// 判断是否允许空步裁剪
    /// </summary>
    /// <returns></returns>
    public bool NullOkey()
    {
        return (sdPlayer == 0 ? redValue : blackValue) > ChessLogic.NULL_MARGIN;
    }

    public void Mirror(ref Situation mirrorSituation)
    {
        mirrorSituation.ClearBoard();

        int pc = 0;
        for(int sq = 0; sq < 256; sq++)
        {
            pc = currentBoard[sq];
            if(pc != 0)
            {
                mirrorSituation.AddPiece(logic.MirrorSqure(sq), pc);
            }
        }

        if(sdPlayer == 1)
        {
            mirrorSituation.ChangeSide();
        }
        mirrorSituation.SetIrrev();
    }
}
