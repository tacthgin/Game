
//历史走法信息
public class MoveInfo 
{
    private ushort mv = 0;
    private byte pcCaptured = 0;
    private byte check = 0;
    private uint key = 0;

    public ushort Mv
    {
        set { mv = value; }
        get { return mv; }
    }

    public byte PcCaptured
    {
        set { pcCaptured = value; }
        get { return pcCaptured; }
    }

    public byte Check
    {
        set { check = value; }
        get { return check; }
    }

    public uint Key
    {
        set { key = value; }
        get { return key; }
    }
}
