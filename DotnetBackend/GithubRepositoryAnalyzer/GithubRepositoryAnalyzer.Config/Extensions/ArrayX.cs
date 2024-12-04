namespace GithubRepositoryAnalyzer.Config.Extensions;

public static class ArrayX
{
    public static byte[] MergeByteArrays(params byte[][] arrays)
    {
        byte[] destinationArray = new byte[((IEnumerable<byte[]>) arrays).Sum<byte[]>((Func<byte[], int>) (a => a.Length))];
        int destinationIndex = 0;
        foreach (byte[] array in arrays)
        {
            Array.Copy((Array) array, 0, (Array) destinationArray, destinationIndex, array.Length);
            destinationIndex += array.Length;
        }
        return destinationArray;
    }
}