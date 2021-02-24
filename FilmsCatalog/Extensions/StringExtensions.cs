namespace FilmsCatalog.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string stringToCheck) 
            => string.IsNullOrEmpty(stringToCheck);
    }
}