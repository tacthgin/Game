
using UnityEngine;

public class ChessLogic
{
    /// <summary>
    /// 棋盘范围
    /// </summary>
    public const int ROW_TOP = 3;
    public const int ROW_BOTTOM = 12;
    public const int COLUMN_LEFT = 3;
    public const int COLUMN_RIGHT = 11;

    /// <summary>
    /// 棋子的编号
    /// </summary>
    public int PIECE_KING = 0;
    public int PIECE_ADVISOR = 1;
    public int PIECE_BISHOP = 2;
    public int PIECE_KNIGHT = 3;
    public int PIECE_ROOK = 4;
    public int PIECE_CANNON = 5;
    public int PIECE_PAWN = 6;

    /// <summary>
    /// 判断棋子是否在棋盘中
    /// </summary>
    private readonly sbyte[] chessInBoard = {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    /// <summary>
    /// 判断棋子是否在九宫
    /// </summary>
    private readonly sbyte[] chessInFort = {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    /// <summary>
    /// 判断步长是否符合特定走法，1=帅，2=士，3=相
    /// </summary>
    private readonly sbyte[] legalSpan = {
                             0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 2, 1, 2, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 2, 1, 2, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0
    };

    /// <summary>
    /// 根据步长判断马是否憋腿
    /// </summary>
    private readonly sbyte[] knightPin = {
            0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,-16,  0,-16,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0, -1,  0,  0,  0,  1,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0, -1,  0,  0,  0,  1,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0, 16,  0, 16,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0
    };

    /// <summary>
    /// 初始棋盘
    /// </summary>
    private readonly sbyte[] startupChessBoard= {
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0, 20, 19, 18, 17, 16, 17, 18, 19, 20,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0, 21,  0,  0,  0,  0,  0, 21,  0,  0,  0,  0,  0,
        0,  0,  0, 22,  0, 22,  0, 22,  0, 22,  0, 22,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0, 14,  0, 14,  0, 14,  0, 14,  0, 14,  0,  0,  0,  0,
        0,  0,  0,  0, 13,  0,  0,  0,  0,  0, 13,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0, 12, 11, 10,  9,  8,  9, 10, 11, 12,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
        0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0
    };

    /// <summary>
    /// 帅(将)的步长
    /// </summary>
    private readonly sbyte[] kingDelta = { -16, -1, 1, 16 };

    /// <summary>
    /// 士的步长
    /// </summary>
    private readonly sbyte[] advisorDelta = { -17, -15, 15, 17 };

    /// <summary>
    /// 马的步长，以帅的步长作为马腿
    /// </summary>
    private readonly sbyte[,] knightDelta = new sbyte[4, 2] { { -33, -31 }, { -18, 14 }, { -14, 18 }, { 31, 33 } };

    /// <summary>
    /// 马的步长，以士的步长作为马腿
    /// </summary>
    private readonly sbyte[,] knightCheckDelta = new sbyte[4, 2] { { -33, -18 }, { -31, -14 }, { 14, 31 }, { 18, 33 } };

    /// <summary>
    /// 判断棋子是否在棋盘中
    /// </summary>
    /// <param name="sq"></param>
    /// <returns></returns>
    public bool InBoard(int sq)
    {
        return chessInBoard[sq] != 0;
    }

    /// <summary>
    /// 判断棋子是否在九宫中
    /// </summary>
    /// <param name="sq"></param>
    /// <returns></returns>
    public bool InFort(int sq)
    {
        return chessInFort[sq] != 0;
    }

    /// <summary>
    /// 获得格子的横坐标
    /// </summary>
    /// <param name="sq"></param>
    /// <returns></returns>
    public int ColumnX(int sq)
    {
        return sq & 15;
    }

    /// <summary>
    /// 获得格子的纵坐标
    /// </summary>
    /// <param name="sq"></param>
    /// <returns></returns>
    public int RowY(int sq)
    {
        return sq >> 4;
    }

    /// <summary>
    /// 根据横坐标，纵坐标获得格子
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int CoordXY(int x, int y)
    {
        return x + (y << 4);
    }

    /// <summary>
    /// 走法是否符合帅的步长
    /// </summary>
    /// <param name="sqSrc"></param>
    /// <param name="sqDst"></param>
    /// <returns></returns>
    public bool KingSpan(int sqSrc, int sqDst)
    {
        return legalSpan[sqDst - sqSrc + 256] == 1;
    }

    /// <summary>
    /// 走法是否符合士的步长
    /// </summary>
    /// <param name="sqSrc"></param>
    /// <param name="sqDst"></param>
    /// <returns></returns>
    public bool AdvisorSpan(int sqSrc, int sqDst)
    {
        return legalSpan[sqDst - sqSrc + 256] == 2;
    }

    /// <summary>
    /// 走法是否符合象的步长
    /// </summary>
    /// <param name="sqSrc"></param>
    /// <param name="sqDst"></param>
    /// <returns></returns>
    public bool BishopSpan(int sqSrc, int sqDst)
    {
        return legalSpan[sqDst - sqSrc + 256] == 3;
    }

    /// <summary>
    /// 象眼的位置
    /// </summary>
    /// <param name="sqSrc"></param>
    /// <param name="sqDst"></param>
    /// <returns></returns>
    public int BishopPin(int sqSrc, int sqDst)
    {
        return (sqSrc + sqDst) >> 1;
    }

    /// <summary>
    /// 马腿的位置
    /// </summary>
    /// <param name="sqSrc"></param>
    /// <param name="sqDst"></param>
    /// <returns></returns>
    public int KnightPin(int sqSrc, int sqDst)
    {
        return sqSrc + knightPin[sqDst - sqSrc + 256];
    }

    /// <summary>
    /// 是否未过河
    /// </summary>
    /// <param name="sq"></param>
    /// <param name="sd"></param>
    /// <returns></returns>
    public bool HomeHalf(int sq, int sd)
    {
        return (sq & 0x80) != (sd << 7);
    }

    /// <summary>
    /// 是否已过河
    /// </summary>
    /// <param name="sq"></param>
    /// <param name="sd"></param>
    /// <returns></returns>
    public bool AwayHalf(int sq, int sd)
    {
        return (sq & 0x80) == (sd << 7);
    }

    /// <summary>
    /// 是否在河的同一边
    /// </summary>
    /// <param name="sqSrc"></param>
    /// <param name="sqDst"></param>
    /// <returns></returns>
    public bool SameHalf(int sqSrc, int sqDst)
    {
        return ((sqSrc ^ sqDst) & 0x80) == 0;
    }

    /// <summary>
    /// 是否在同一行(低位不同)
    /// </summary>
    /// <param name="sqSrc"></param>
    /// <param name="sqDst"></param>
    /// <returns></returns>
    public bool SameRow(int sqSrc, int sqDst)
    {
        return ((sqSrc ^ sqDst) & 0xf0) == 0;
    }

    /// <summary>
    /// 是否在同一列
    /// </summary>
    /// <param name="sqSrc"></param>
    /// <param name="sqDst"></param>
    /// <returns></returns>
    public bool SameColumn(int sqSrc, int sqDst)
    {
        return ((sqSrc ^ sqDst) & 0x0f) == 0;
    }

    /// <summary>
    /// 获得红黑标记(8是红，16是黑)
    /// </summary>
    /// <param name="sd"></param>
    public int SideTag(int sd)
    {
        return 8 + (sd << 3);
    }

    /// <summary>
    /// 获得对方红黑标记
    /// </summary>
    /// <param name="sd"></param>
    /// <returns></returns>
    public int oppSideTag(int sd)
    {
        return 16 - (sd << 3);
    }

    /// <summary>
    /// 获得走法起点
    /// </summary>
    /// <param name="mv"></param>
    /// <returns></returns>
    public int Src(int mv)
    {
        return mv & 255;
    }

    /// <summary>
    /// 获得走法终点
    /// </summary>
    /// <param name="mv"></param>
    /// <returns></returns>
    public int Dst(int mv)
    {
        return mv >> 8;
    }

    /// <summary>
    /// 根据走法起点和终点获得走法
    /// </summary>
    /// <param name="sqSrc">起点棋子编号</param>
    /// <param name="sqDst">终点棋子编号</param>
    /// <returns>走法</returns>
    public int Move(int sqSrc, int sqDst)
    {
        return sqSrc + sqDst * 256;
    }

    public bool Red(int pc)
    {
        return (pc & 8) != 0;
    }

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

        /// <summary>
        /// 初始化棋盘
        /// </summary>
        /// <param name="startBoard">初始棋盘设置</param>
        public void Init(ChessLogic chessLogic, byte[] startBoard)
        {
            sdPlayer = 0;
            startBoard.CopyTo(currentBoard, 0);
            logic = chessLogic;
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
            currentBoard[sq] = (byte)pc;
        }

        /// <summary>
        /// 棋盘上拿走一枚棋子
        /// </summary>
        /// <param name="sq">棋子编号</param>
        public void DeletePiece(int sq)
        {
            currentBoard[sq] = 0;
        }

        /// <summary>
        /// 搬一步棋的棋子
        /// </summary>
        /// <param name="mv"></param>
        public void MovePiece(int mv)
        {
            int sqSrc, sqDst, pc;
            sqSrc = logic.Src(mv);
            sqDst = logic.Dst(mv);
            pc = currentBoard[sqSrc];
            DeletePiece(sqSrc);
            AddPiece(sqDst, pc);
        }

        /// <summary>
        /// 走一步棋
        /// </summary>
        /// <param name="mv"></param>
        public void MakeMove(int mv)
        {
            MovePiece(mv);
            ChangeSide();
        }
    }

    /// <summary>
    /// 局面结构的实例
    /// </summary>
    private Situation situation = new Situation();
    public Situation MySituation
    {
        set { }
        get { return situation; }
    }
    /// <summary>
    /// 选中的棋子编号
    /// </summary>
    private int sqSelected = 0;

    /// <summary>
    /// 上一步棋
    /// </summary>
    private int mvLast = 0;

    /// <summary>
    /// 画选中的图像
    /// </summary>
    /// <param name="visible"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public delegate void DrawSelectHandle(bool visible, int x = 0, int y = 0);
    public DrawSelectHandle drawSelectHandle;

    /// <summary>
    /// 画移动棋子的图像
    /// </summary>
    /// <param name="srcPosition"></param>
    /// <param name="dstPosition"></param>
    public delegate void MovePieceHandle(Vector2 srcPosition, Vector2 dstPosition);
    public MovePieceHandle movePieceHandle;

    /// <summary>
    /// 初始化局面实例
    /// </summary>
    public void Init()
    {
        situation.Init(this, startupChessBoard);
    }

    public void ClickSquare(int x, int y)
    {
        int sq = CoordXY(x, y);
        int pc = situation.CurrentBoard[sq];

        //选中自己的棋子
        if((pc & SideTag(situation.SdPlayer)) != 0)
        {
            sqSelected = sq;
            drawSelectHandle(true, x, y);
        }else if(sqSelected != 0)
        {
            mvLast = Move(sqSelected, sq);
            situation.MakeMove(mvLast);
            drawSelectHandle(false);
            movePieceHandle(new Vector2(ColumnX(sqSelected), RowY(sqSelected)), new Vector2(x, y));
            sqSelected = 0;
        }
    }
}
