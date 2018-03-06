using UnityEngine;
/// <summary>
/// 局面结构
/// </summary>
public class Situation
{
    /// <summary>
    /// 轮到谁走 0=红方，1=黑方
    /// </summary>
    private int sdPlayer = 0;
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

    public int Distance
    {
        set { distance = value; }
        get { return distance; }
    }

    /// <summary>
    /// 初始化棋盘
    /// </summary>
    /// <param name="startBoard">初始棋盘设置</param>
    public void Init(ChessLogic chessLogic, sbyte[] startBoard)
    {
        logic = chessLogic;

        sdPlayer = 0;
        redValue = 0;
        blackValue = 0;
        distance = 0;

        int pc = 0;
        for(int sq = 0; sq < 256; sq++)
        {
            pc = startBoard[sq];
            if(pc != 0)
            {
                AddPiece(sq, pc);
            }
        }
    }

    /// <summary>
    /// 交换走子方
    /// </summary>
    public void ChangeSide()
    {
        sdPlayer = 1 - sdPlayer;
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
        }
        else
        {
            blackValue += logic.piecePosValue[pc - 16, logic.SquareFilp(sq)];
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
        }
        else
        {
            blackValue -= logic.piecePosValue[pc - 16, logic.SquareFilp(sq)];
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
    public bool MakeMove(int mv, out int pcCaptured)
    {
        pcCaptured = MovePiece(mv);
        if (Checked())
        {
            UndoMovePiece(mv, pcCaptured);
            return false;
        }
        ChangeSide();
        ++distance;
        return true;
    }

    /// <summary>
    /// 撤销走一步棋
    /// </summary>
    /// <param name="mv"></param>
    /// <param name="pcCaptured"></param>
    public void UndoMakeMove(int mv, int pcCaptured)
    {
        --distance;
        ChangeSide();
        UndoMovePiece(mv, pcCaptured);
    }

    /// <summary>
    /// 生成所有走法
    /// </summary>
    /// <param name="mvs"></param>
    /// <returns></returns>
    public int GenerateMoves(out int[] mvs)
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
                            if ((pcDst & pcSelfSide) == 0)
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
                            if ((pcDst & pcSelfSide) == 0)
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
                            if ((pcDst & pcSelfSide) == 0)
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
                                if ((pcDst & pcSelfSide) == 0)
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
                                    tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                    ++genMoves;
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
                                    tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                    ++genMoves;
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
                                if (pcDst != 0 && (pcDst & pcOppSide) != 0)
                                {
                                    tempMvs[genMoves] = logic.Move(sqSrc, sqDst);
                                    ++genMoves;
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
                            if ((pcDst & pcSelfSide) == 0)
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
                                    if ((pcDst & pcSelfSide) == 0)
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
}
