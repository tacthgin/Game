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
    public const int PIECE_KING = 0;
    public const int PIECE_ADVISOR = 1;
    public const int PIECE_BISHOP = 2;
    public const int PIECE_KNIGHT = 3;
    public const int PIECE_ROOK = 4;
    public const int PIECE_CANNON = 5;
    public const int PIECE_PAWN = 6;

    /// <summary>
    /// 走法排序阶段
    /// </summary>
    public const int PHASE_HASH = 0;
    public const int PHASE_KILLER_1 = 1;
    public const int PHASE_KILLER_2 = 2;
    public const int PHASE_GEN_MOVES = 3;
    public const int PHASE_REST = 4;

    /// <summary>
    /// 最大走法数
    /// </summary>
    public const int MAX_GENERATE_MOVES = 128;

    /// <summary>
    /// 最高分支，即将死的分支
    /// </summary>
    public const int MATE_VALUE = 10000;

    /// <summary>
    /// 长将判负的分值，低于该值将不写入置换表
    /// </summary>
    public const int BAN_VALUE = MATE_VALUE - 100;

    /// <summary>
    /// 搜索出胜负的分值界限，超出此值就说明已经搜索出杀棋了
    /// </summary>
    public const int WIN_VALUE = MATE_VALUE - 100;

    /// <summary>
    /// 和棋时返回的分数(取负值)
    /// </summary>
    public const int DRAW_VALUE = 20;     

    /// <summary>
    /// 先行权分值
    /// </summary>
    public const int ADVANCED_VALUE = 3;

    /// <summary>
    /// 空步裁剪的子力边界
    /// </summary>
    public const int NULL_MARGIN = 400;

    /// <summary>
    /// 空步裁剪的裁剪深度
    /// </summary>
    public const int NULL_DEPTH = 2;

    /// <summary>
    /// 开局库大小
    /// </summary>
    public const int BOOK_SIZE = 16384;

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
    /// 判断步长是否符合特定走法，1=帅，2=士，3=象
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
    public readonly sbyte[] startupChessBoard= {
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
    public readonly sbyte[] kingDelta = { -16, -1, 1, 16 };

    /// <summary>
    /// 士的步长
    /// </summary>
    public readonly sbyte[] advisorDelta = { -17, -15, 15, 17 };

    /// <summary>
    /// 马的步长，以帅的步长作为马腿
    /// </summary>
    public readonly sbyte[,] knightDelta = new sbyte[4, 2] { { -33, -31 }, { -18, 14 }, { -14, 18 }, { 31, 33 } };

    /// <summary>
    /// 马的步长，以士的步长作为马腿
    /// </summary>
    public readonly sbyte[,] knightCheckDelta = new sbyte[4, 2] { { -33, -18 }, { -31, -14 }, { 14, 31 }, { 18, 33 } };

    // 子力位置价值表
    public readonly byte[,] piecePosValue = new byte[7, 256]
    {
        { // 帅(将)
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
            0,  0,  0,  0,  0,  0,  1,  1,  1,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  2,  2,  2,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0, 11, 15, 11,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0
        }, 
        { // 仕(士)
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
            0,  0,  0,  0,  0,  0, 20,  0, 20,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0, 23,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0, 20,  0, 20,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0
        },
        { // 相(象)
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0, 20,  0,  0,  0, 20,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0, 18,  0,  0,  0, 23,  0,  0,  0, 18,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0, 20,  0,  0,  0, 20,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0
        }, 
        { // 马
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0, 90, 90, 90, 96, 90, 96, 90, 90, 90,  0,  0,  0,  0,
            0,  0,  0, 90, 96,103, 97, 94, 97,103, 96, 90,  0,  0,  0,  0,
            0,  0,  0, 92, 98, 99,103, 99,103, 99, 98, 92,  0,  0,  0,  0,
            0,  0,  0, 93,108,100,107,100,107,100,108, 93,  0,  0,  0,  0,
            0,  0,  0, 90,100, 99,103,104,103, 99,100, 90,  0,  0,  0,  0,
            0,  0,  0, 90, 98,101,102,103,102,101, 98, 90,  0,  0,  0,  0,
            0,  0,  0, 92, 94, 98, 95, 98, 95, 98, 94, 92,  0,  0,  0,  0,
            0,  0,  0, 93, 92, 94, 95, 92, 95, 94, 92, 93,  0,  0,  0,  0,
            0,  0,  0, 85, 90, 92, 93, 78, 93, 92, 90, 85,  0,  0,  0,  0,
            0,  0,  0, 88, 85, 90, 88, 90, 88, 90, 85, 88,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0
        }, 
        { // 车
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,206,208,207,213,214,213,207,208,206,  0,  0,  0,  0,
            0,  0,  0,206,212,209,216,233,216,209,212,206,  0,  0,  0,  0,
            0,  0,  0,206,208,207,214,216,214,207,208,206,  0,  0,  0,  0,
            0,  0,  0,206,213,213,216,216,216,213,213,206,  0,  0,  0,  0,
            0,  0,  0,208,211,211,214,215,214,211,211,208,  0,  0,  0,  0,
            0,  0,  0,208,212,212,214,215,214,212,212,208,  0,  0,  0,  0,
            0,  0,  0,204,209,204,212,214,212,204,209,204,  0,  0,  0,  0,
            0,  0,  0,198,208,204,212,212,212,204,208,198,  0,  0,  0,  0,
            0,  0,  0,200,208,206,212,200,212,206,208,200,  0,  0,  0,  0,
            0,  0,  0,194,206,204,212,200,212,204,206,194,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0
        }, 
        { // 炮
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,100,100, 96, 91, 90, 91, 96,100,100,  0,  0,  0,  0,
            0,  0,  0, 98, 98, 96, 92, 89, 92, 96, 98, 98,  0,  0,  0,  0,
            0,  0,  0, 97, 97, 96, 91, 92, 91, 96, 97, 97,  0,  0,  0,  0,
            0,  0,  0, 96, 99, 99, 98,100, 98, 99, 99, 96,  0,  0,  0,  0,
            0,  0,  0, 96, 96, 96, 96,100, 96, 96, 96, 96,  0,  0,  0,  0,
            0,  0,  0, 95, 96, 99, 96,100, 96, 99, 96, 95,  0,  0,  0,  0,
            0,  0,  0, 96, 96, 96, 96, 96, 96, 96, 96, 96,  0,  0,  0,  0,
            0,  0,  0, 97, 96,100, 99,101, 99,100, 96, 97,  0,  0,  0,  0,
            0,  0,  0, 96, 97, 98, 98, 98, 98, 98, 97, 96,  0,  0,  0,  0,
            0,  0,  0, 96, 96, 97, 99, 99, 99, 97, 96, 96,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0
        }, 
        { // 兵(卒)
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  9,  9,  9, 11, 13, 11,  9,  9,  9,  0,  0,  0,  0,
            0,  0,  0, 19, 24, 34, 42, 44, 42, 34, 24, 19,  0,  0,  0,  0,
            0,  0,  0, 19, 24, 32, 37, 37, 37, 32, 24, 19,  0,  0,  0,  0,
            0,  0,  0, 19, 23, 27, 29, 30, 29, 27, 23, 19,  0,  0,  0,  0,
            0,  0,  0, 14, 18, 20, 27, 29, 27, 20, 18, 14,  0,  0,  0,  0,
            0,  0,  0,  7,  0, 13,  0, 16,  0, 13,  0,  7,  0,  0,  0,  0,
            0,  0,  0,  7,  0,  7,  0, 15,  0,  7,  0,  7,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
            0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0
        }
    };


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
    /// 翻转格子
    /// </summary>
    /// <param name="sq"></param>
    /// <returns></returns>
    public int SquareFilp(int sq)
    {
        return 254 - sq;
    }

    /// <summary>
    /// 横坐标垂直镜像
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public int ColumnFilp(int x)
    {
        return 14 - x;
    }

    /// <summary>
    /// 纵坐标水平镜像
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public int RowFilp(int y)
    {
        return 15 - y;
    }

    /// <summary>
    /// 格子水平镜像
    /// </summary>
    /// <param name="sq"></param>
    /// <returns></returns>
    public int MirrorSqure(int sq)
    {
        return CoordXY(ColumnFilp(ColumnX(sq)), RowY(sq));
    }

    /// <summary>
    /// 棋子向前走一步
    /// </summary>
    /// <param name="sq"></param>
    /// <param name="sd"></param>
    /// <returns></returns>
    public int SquareForward(int sq, int sd)
    {
        return sq - 16 + (sd << 5);
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

    /// <summary>
    /// 走法水平镜像
    /// </summary>
    /// <param name="mv"></param>
    /// <returns></returns>
    public int MirrorMove(int mv)
    {
        return Move(MirrorSqure(Src(mv)), MirrorSqure(Dst(mv)));
    }

    public bool Red(int pc)
    {
        return (pc & 8) != 0;
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
    /// 搜索实例
    /// </summary>
    private Search search;

    /// <summary>
    /// 选中的棋子编号
    /// </summary>
    private int sqSelected = 0;

    /// <summary>
    /// 上一步棋
    /// </summary>
    private int mvLast = 0;

    /// <summary>
    /// 电脑下棋了
    /// </summary>
    private bool computer = false;

    /// <summary>
    /// 游戏结束
    /// </summary>
    private bool gameOver = false;

    public bool Computer
    {
        set { }
        get { return computer; }
    }
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
        ZobristTable.InitZobristTable();
        situation.Init(this);
        search = new Search(this, situation);
        search.LoadBook();
    }

    public void ClickSquare(int x, int y)
    {
        if(computer || gameOver)
        {
            return;
        }
        int sq = CoordXY(x, y);
        int pc = situation.CurrentBoard[sq];

        //选中自己的棋子
        if((pc & SideTag(situation.SdPlayer)) != 0)
        {
            sqSelected = sq;
            drawSelectHandle(true, x, y);
            SoundManager.MyInstance.PlayEffect(SoundManager.AUDIO_CLICK);
        }
        else if(sqSelected != 0)
        {
            int mv = Move(sqSelected, sq);
            if(situation.LegalMove(mv))
            {
                if(situation.MakeMove(mv))
                {
                    mvLast = mv;
                    drawSelectHandle(true, x, y);
                    movePieceHandle(new Vector2(ColumnX(sqSelected), RowY(sqSelected)), new Vector2(x, y));
                    sqSelected = 0;
                    //检测重复局面
                    int repValue = situation.RepStatus(3);
                    if (situation.IsMate())
                    {
                        //分出胜负
                        Debug.Log("you win");
                        gameOver = true;
                    }
                    else if (repValue > 0)
                    {
                        //repValue是对电脑来说的分值
                        repValue = situation.RepValue(repValue);
                        int soundId = 0;
                        string message = "";
                        if(repValue > WIN_VALUE)
                        {
                            soundId = SoundManager.AUDIO_LOSS;
                            message = "长打作负，请不要气馁！";
                        }else if(repValue < -WIN_VALUE)
                        {
                            soundId = SoundManager.AUDIO_WIN;
                            message = "电脑长打作负，祝贺你取得胜利！";
                        }else
                        {
                            soundId = SoundManager.AUDIO_DRAW;
                            message = "双方不变作和，辛苦了！";
                        }
                        SoundManager.MyInstance.PlayEffect(soundId);
                        Debug.Log(message);
                        gameOver = true;
                    }
                    else if(situation.MoveNum > 100)
                    {
                        SoundManager.MyInstance.PlayEffect(SoundManager.AUDIO_DRAW);
                        Debug.Log("超过自然限着作和，辛苦了！");
                        gameOver = true;
                    }
                    else
                    {
                        //将军或者吃子
                        int soundId = situation.Checked() ? SoundManager.AUDIO_ENEMY_CHECK : (pc != 0 ? SoundManager.AUDIO_CAPTURE : SoundManager.AUDIO_MOVE);
                        SoundManager.MyInstance.PlayEffect(soundId);
                        if(situation.Captured())
                        {
                            situation.SetIrrev();
                        }
                        //电脑走棋
                        computer = true;
                    }
                }else
                {
                    //被将军
                    SoundManager.MyInstance.PlayEffect(SoundManager.AUDIO_CHECK);
                }
            }else
            {
                //不合法的棋
                SoundManager.MyInstance.PlayEffect(SoundManager.AUDIO_ILLEGAL);
            }
        }
    }

    /// <summary>
    /// 电脑回应一步棋
    /// </summary>
    public void ResponseMove()
    {
        computer = false;
        //电脑走一步棋
        search.SearchMain();
        int pcCaptured = 0;
        situation.MakeMove(search.MvResult);
        mvLast = search.MvResult;
        //画电脑的棋
        int sqSrc = Src(mvLast);
        int sqDst = Dst(mvLast);
        drawSelectHandle(true, ColumnX(sqDst), RowY(sqDst));
        movePieceHandle(new Vector2(ColumnX(sqSrc), RowY(sqSrc)), new Vector2(ColumnX(sqDst), RowY(sqDst)));
        //检测重复局面
        int repValue = situation.RepStatus(3);
        if (situation.IsMate())
        {
            Debug.Log("player die");
            //你死了
            SoundManager.MyInstance.PlayEffect(SoundManager.AUDIO_LOSS);
            gameOver = true;
        }
        else if (repValue > 0)
        {
            //repValue是对电脑来说的分值
            repValue = situation.RepValue(repValue);
            int soundId = 0;
            string message = "";
            if (repValue > WIN_VALUE)
            {
                soundId = SoundManager.AUDIO_LOSS;
                message = "长打作负，请不要气馁！";
            }
            else if (repValue < -WIN_VALUE)
            {
                soundId = SoundManager.AUDIO_WIN;
                message = "电脑长打作负，祝贺你取得胜利！";
            }
            else
            {
                soundId = SoundManager.AUDIO_DRAW;
                message = "双方不变作和，辛苦了！";
            }
            SoundManager.MyInstance.PlayEffect(soundId);
            Debug.Log(message);
            gameOver = true;
        }
        else if (situation.MoveNum > 100)
        {
            SoundManager.MyInstance.PlayEffect(SoundManager.AUDIO_DRAW);
            Debug.Log("超过自然限着作和，辛苦了！");
            gameOver = true;
        }
        else
        {
            //被将军或者吃子
            int soundId = situation.Checked() ? SoundManager.AUDIO_ENEMY_CHECK : (pcCaptured != 0 ? SoundManager.AUDIO_CAPTURE : SoundManager.AUDIO_MOVE);
            SoundManager.MyInstance.PlayEffect(soundId);
        }
    }
}
