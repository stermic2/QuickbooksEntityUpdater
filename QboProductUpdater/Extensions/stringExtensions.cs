namespace QboProductUpdater.Extensions
{
    public static class stringExtensions
    {
        public static string AddSingleParenthesis(this string input)
        {
            return "'" + input + "'";
        }
    }
}