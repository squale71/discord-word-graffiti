using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Discord.WordGraffiti.Helpers
{
    public static class StringExtensions
    {
        public static HashSet<string> GetUniqueWordsFromString(this string value)
        {
            string[] msgWords = Regex.Replace(value, @"[^\w]", " ").Split(' '); // splits messages into an array of words - also strips out non-letter characters and replaces with a space.
            var uniqueWords = new HashSet<string>(msgWords.Select(x => x.ToLower())); //reduces list to unique words only

            return uniqueWords;
        }
    }
}
