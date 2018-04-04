/// <summary>
/// 走法排序结构
/// </summary>
public class SortInfo
{
    /// <summary>
    /// 置换表走法
    /// </summary>
    private int mvHash = 0;

    /// <summary>
    /// 两个杀手走法
    /// </summary>
    private int mvKiller1 = 0;
    private int mvKiller2 = 0;

    /// <summary>
    /// 当前阶段
    /// </summary>
    private int phase = 0;

    /// <summary>
    /// 当前采用第几个走法
    /// </summary>
    private int index = 0;

    /// <summary>
    /// 总共有几个走法
    /// </summary>
    private int genMoves = 0;

    /// <summary>
    /// 所有走法
    /// </summary>
    private int[] mvs = new int[ChessLogic.MAX_GENERATE_MOVES];

    /// <summary>
    /// 搜索实例
    /// </summary>
    private Search search;

    public void Init(int hash, Search s)
    {
        search = s;
        mvHash = hash;
        mvKiller1 = s.MvKillers[s.MySituation.Distance, 0];
        mvKiller2 = s.MvKillers[s.MySituation.Distance, 1];
        phase = ChessLogic.PHASE_HASH;
    }

    /// <summary>
    /// 得到下一个走法
    /// </summary>
    /// <returns></returns>
    public int Next()
    {
        int mv = 0;
        //phase表示着法启发的若干阶段，依次为
        switch(phase)
        {
            //0.置换表着法启发，完成后立即进入下一阶段；
            case ChessLogic.PHASE_HASH:
                {
                    phase = ChessLogic.PHASE_KILLER_1;
                    if (mvHash != 0)
                    {
                        return mvHash;
                    }
                    goto case ChessLogic.PHASE_KILLER_1;
                }
            //1.杀手着法启发(第一个杀手着法)，完成后立即进入下一个阶段；
            case ChessLogic.PHASE_KILLER_1:
                {
                    phase = ChessLogic.PHASE_KILLER_2;
                    if (mvKiller1 != mvHash && mvKiller1 != 0 && search.MySituation.LegalMove(mvKiller1))
                    {
                        return mvKiller1;
                    }
                    goto case ChessLogic.PHASE_KILLER_2;
                }
            //2.杀手着法启发(第二个杀手着法)，完成后立即进入下一个阶段；
            case ChessLogic.PHASE_KILLER_2:
                {
                    phase = ChessLogic.PHASE_GEN_MOVES;
                    if (mvKiller2 != mvHash && mvKiller2 != 0 && search.MySituation.LegalMove(mvKiller2))
                    {
                        return mvKiller2;
                    }
                    goto case ChessLogic.PHASE_GEN_MOVES;
                }
            //3.生成所有走法，完成后立即进入下一阶段；
            case ChessLogic.PHASE_GEN_MOVES:
                {
                    phase = ChessLogic.PHASE_REST;
                    genMoves = search.MySituation.GenerateMoves(out mvs);
                    System.Array.Sort(mvs, 0, genMoves, search.GetHistoryCompare());
                    index = 0;
                    goto case ChessLogic.PHASE_REST;
                }
            //4.对剩余的走法做历史表启发
            case ChessLogic.PHASE_REST:
                {
                    while (index < genMoves)
                    {
                        mv = mvs[index];
                        ++index;
                        if (mv != mvHash && mv != mvKiller1 && mv != mvKiller2)
                        {
                            return mv;
                        }
                    }
                }break;
            //没有走法，返回0
            default:
                return 0;
        }

        return 0;
    }
}
