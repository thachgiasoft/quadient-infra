using System.Linq;

namespace Infrastructure.Core.Security
{
    public class AntiHeaderManipulation
    {
        private static readonly char[] BlackList = { '\n', '\r', ':', '=' };
        public static char SafeChar = '_';
        public static string Encoded = " (Encoded)";

        public static string PreventHeaderManipulation(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            var clean = BlackList.Aggregate(input, (current, t) => current.Replace(t, SafeChar));
            if (!input.Equals(clean))
            {
                clean += Encoded;
            }
            return clean;
        }
    }
}
