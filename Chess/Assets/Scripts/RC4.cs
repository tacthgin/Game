/// <summary>
/// RC4密码流生成器
/// </summary>
public class RC4
{
    private byte[] steam = new byte[256];
    private int x = 0;
    private int y = 0;

    /// <summary>
    /// 用空密钥初始化密码流生成器
    /// </summary>
    public void InitZero()
    {
        for(int i = 0; i < 256; i++)
        {
            steam[i] = (byte)i;
        }

        int j = 0;
        byte uc = 0;
        for(int i = 0; i < 256; i++)
        {
            j = (j + steam[i]) & 255;
            uc = steam[i];
            steam[i] = steam[j];
            steam[j] = uc;
        }
    }

    /// <summary>
    /// 生成密码流的下一个字节
    /// </summary>
    /// <returns></returns>
    public byte NextByte()
    {
        byte uc = 0;
        x = (x + 1) & 255;
        y = (y + steam[x]) & 255;
        uc = steam[x];
        steam[x] = steam[y];
        steam[y] = uc;
        return steam[(steam[x] + steam[y]) & 255];
    }
	
    /// <summary>
    /// 生成密码流的下四个字节
    /// </summary>
    /// <returns></returns>
    public uint NextLong()
    {
        uint uc0, uc1, uc2, uc3;
        uc0 = NextByte();
        uc1 = NextByte();
        uc2 = NextByte();
        uc3 = NextByte();
        return (uc0 + (uc1 << 8) + (uc2 << 16) + (uc3 << 24));
    }
}
