/// <summary>
/// Zobrist结构
/// </summary>
public class Zobrist
{
    private uint key = 0;
    private uint lock0 = 0;
    private uint lock1 = 0;

    public uint Key
    {
        set { }
        get { return key; }
    }

    public uint Lock0
    {
        set { }
        get { return lock0; }
    }

    public uint Lock1
    {
        set { }
        get { return lock1; }
    }

    /// <summary>
    /// 用密码流填充Zobrist
    /// </summary>
    /// <param name="rc4"></param>
    public void InitRC4(RC4 rc4)
    {
        key = rc4.NextLong();
        lock0 = rc4.NextLong();
        lock1 = rc4.NextLong();
    }

    /// <summary>
    /// 执行异或操作
    /// </summary>
    /// <param name="zobr"></param>
    public void Xor(ref Zobrist zobr)
    {
        key ^= zobr.key;
        lock0 ^= zobr.lock0;
        lock1 ^= zobr.lock1;
    }

    public void Xor(ref Zobrist zobr1, ref Zobrist zobr2)
    {
        key = zobr1.key ^ zobr2.key;
        lock0 = zobr1.lock0 ^ zobr2.lock0;
        lock1 = zobr1.lock1 ^ zobr2.lock1;
    }
}

/// <summary>
/// Zobrist表
/// </summary>
static public class ZobristTable
{
    static public Zobrist player = new Zobrist();
    static public Zobrist[,] table = new Zobrist[14, 256];

    static public void InitZobristTable()
    {
        RC4 rc4 = new RC4();
        rc4.InitZero();
        ZobristTable.player.InitRC4(rc4);
        for(int i = 0; i < 14; i++)
        {
            for(int j = 0; j < 256; j++)
            {
                Zobrist zobr = new Zobrist();
                zobr.InitRC4(rc4);
                ZobristTable.table[i, j] = zobr;
            }
        }
    }
}