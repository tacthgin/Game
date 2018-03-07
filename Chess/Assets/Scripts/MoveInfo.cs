
//历史走法信息
public class MoveInfo 
{
    private ushort mv = 0;
    private byte pcCaptured = 0;
    private bool check = false;
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

    public bool Check
    {
        set { check = value; }
        get { return check; }
    }

    public uint Key
    {
        set { key = value; }
        get { return key; }
    }

    public void Set(ushort mv, byte captured, bool check, uint key)
    {
        this.mv = mv;
        pcCaptured = captured;
        this.check = check;
        this.key = key;
    }
}
