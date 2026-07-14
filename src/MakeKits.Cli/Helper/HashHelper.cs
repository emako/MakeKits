using System.Security.Cryptography;
using System.Text;

namespace MakeKits.Cli.Helper;

internal static class HashHelper
{
    public static string GetFileMd5(string filePath)
    {
        using FileStream stream = File.OpenRead(filePath);
        using MD5 md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(stream);

        StringBuilder sb = new(hash.Length * 2);
        foreach (byte b in hash)
        {
            sb.Append(b.ToString("X2"));
        }
        return sb.ToString();
    }
}
