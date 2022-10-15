using System.Collections.Generic;
using System.Security.Cryptography;

public static class Shuffle
{
    public static void RandomShuffle<T>(this IList<T> list)
    {
        var provider = new RNGCryptoServiceProvider();
        var n = list.Count;
        while (n > 1)
        {
            var box = new byte[1];
            do
            {
                provider.GetBytes(box);
            } while (!(box[0] < n * (byte.MaxValue / n)));

            var k = box[0] % n;
            n--;
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}