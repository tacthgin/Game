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

    public void Init(int hash, Search s)
    {
        mvHash = hash;
        mvKiller1 = s.MvKillers[s.GetSituationDistance(), 0];
        mvKiller2 = s.MvKillers[s.GetSituationDistance(), 1];
        phase = ChessLogic.PHASE_HASH;
    }

    public void Next()
    {
        //phase表示着法启发的若干阶段，依次为
        switch(phase)
        {
            //0.置换表着法启发，完成后立即进入下一阶段；
            case ChessLogic.PHASE_HASH:
                {
                    phase = ChessLogic.PHASE_KILLER_1;
                }
                break;
        }
    }
}
