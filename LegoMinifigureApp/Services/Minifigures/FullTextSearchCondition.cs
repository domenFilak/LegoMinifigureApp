using System.Text;

namespace LegoMinifigureApp.Services.Minifigures
{
    //changes what user typed into sql server safe full text search condition
    public static class FullTextSearchCondition
    {
        public const int MinWordLength = 2;
        public const int MaxWords = 5;

        //"Darth vad!" => "Darth*" AND "vad*"
        //returns null if nothing usable left
        public static string? Build(string search)
        {
            //keep letters and digits only, everything else space so user input cant break full text syntax
            var cleaned = new StringBuilder(search.Length);

            foreach (var character in search)
            {
                if (char.IsLetterOrDigit(character))
                {
                    cleaned.Append(character);
                }
                else
                {
                    cleaned.Append(' ');
                }
            }

            var words = cleaned.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            //each word becomes a prefix term: "word*"
            var terms = new List<string>();

            foreach (var word in words)
            {
                if (word.Length < MinWordLength)
                {
                    continue;
                }

                terms.Add($"\"{word}*\"");

                if (terms.Count == MaxWords)
                {
                    break;
                }
            }

            if (terms.Count == 0)
            {
                return null;
            }

            //every word must match
            return string.Join(" AND ", terms);
        }
    }
}