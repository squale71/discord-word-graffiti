using System.Collections.Generic;
using System.Linq;

namespace Discord.WordGraffiti.Helpers
{
    public static class WordHelper
    {
        /// <summary>
        /// Given a string and an integer, returns true if words meet maximum count and outputs enumerable of words, otherwise returns false.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="amount"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaximumWordsFromString(string value, int amount, out List<string> result)
        {
            result = value.GetUniqueWordsFromString().ToList();

            return result.Count() > amount;
        }

    }
}
