namespace AdminLte.Services
{
    public static class CustomFilter
    {
        public static List<T> Filter<T>(this List<T> records, Func<T, bool> filter)
        {
            List<T> FilteredRecords = new List<T>();

            foreach (var record in records)
            {
                if (filter(record))
                {
                    FilteredRecords.Add(record);
                }
            }

            return FilteredRecords;
        }
    }
}
