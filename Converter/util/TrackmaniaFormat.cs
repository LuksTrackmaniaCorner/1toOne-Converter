using System;
using System.Collections.Generic;
using System.Text;

namespace Converter.util
{
    public static class TrackmaniaFormat
    {
        private readonly static ISet<char> _colourCharacters;

        static TrackmaniaFormat()
        {
            _colourCharacters = new HashSet<char>();
            for(char c = '0'; c <= '9'; c++)
            {
                _colourCharacters.Add(c);
            }

            for (char c = 'a'; c <= 'f'; c++)
            {
                _colourCharacters.Add(c);
            }

            for (char c = 'A'; c <= 'F'; c++)
            {
                _colourCharacters.Add(c);
            }
        }

        public static string RemoveTrackmaniaFormat(this string original)
        {
            var result = new StringBuilder(original.Length);
            var index = 0;
            var length = original.Length;

            while(!End())
            {
                var c = original[index];

                if (c == '$')
                    ParseSpecial();
                else
                    result.Append(c);

                index++;
            }

            return result.ToString();


            void ParseSpecial()
            {
                index++;
                var nextChar = original[index];

                if (End())
                {
                    return; // $ at end of string is not shown
                }
                 
                //Skip the colour code
                if (_colourCharacters.Contains(nextChar))
                {
                    //skip a second colour char, if any
                    index++;
                    nextChar = original[index];
                    if (End() || !_colourCharacters.Contains(nextChar))
                    {
                        index--;
                        return;
                    }


                    //skip a third colour char, if any
                    index++;
                    nextChar = original[index];
                    if (End() || !_colourCharacters.Contains(nextChar))
                    {
                        index--;
                        return;
                    }

                    return;
                }
                
                if (nextChar == '$')
                {
                    result.Append('$');
                    return;
                }

                // next char just gets removed
            }

            bool End()
            {
                return index >= length;
            }
        }
    }
}
