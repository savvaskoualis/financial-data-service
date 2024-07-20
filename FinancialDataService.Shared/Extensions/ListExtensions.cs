namespace FinancialDataService.Shared.Extensions
{
    public static class ListExtensions
    {
        public static IEnumerable<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            int index = 0;
            while (index < source.Count)
            {
                yield return source.GetRange(index, Math.Min(chunkSize, source.Count - index));

                index += chunkSize;
            }
        }
    }
}