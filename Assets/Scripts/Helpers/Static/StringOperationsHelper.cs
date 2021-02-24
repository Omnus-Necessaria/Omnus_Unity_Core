using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Helpers
{
    public static class StringOperationsHelper
    {

        #region [Parse]
        
        public static readonly CultureInfo FloatParseCulture = new CultureInfo("ru");
        private static readonly CultureInfo FloatParseCultureEn = new CultureInfo("en");
        
        public static float ParseFloat(this string strToParse)
        {
            if (float.TryParse(strToParse, NumberStyles.Float, FloatParseCulture, out float result))
            {
                return result;
            }

            if (float.TryParse(strToParse, NumberStyles.Float, FloatParseCultureEn, out float resultEn))
            {
                return resultEn;
            }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError($"[StringOperationsHelper] Float parse failed! Input string = [{strToParse}]");
#endif
            return float.NaN;
        }
        
        public static List<string> GetCorrectSubstrings(string line, char separator = '\"')
        {
            var substrings        = line.Split(',').ToList();
            var correctSubstrings = new List<string>();
            var incorrectSubstrings = new List<string>();
            for (int i = 0; i < substrings.Count; ++i)
            {
                if (substrings[i].ToList().FindAll(s => s.Equals(separator)).Count % 2 == 1)
                {
                    int firstIndex = i;
                    try
                    {
                        do
                        {
                            substrings[i] = string.Concat(substrings[i], ',');
                            ++i;
                        } while (!substrings[i].Contains(separator));
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        incorrectSubstrings.Add(string.Concat(substrings.GetRange(firstIndex, i - firstIndex + 1)).Replace("\"", string.Empty));
                        continue;
                        //Debug.LogError($"[ParseHelper] String: [{line}] can not be handled.");
                        //return substrings;
                    }

                    correctSubstrings.Add(string.Concat(substrings.GetRange(firstIndex, i - firstIndex + 1)).Replace("\"", string.Empty));
                }
                else correctSubstrings.Add(substrings[i]);
            }

            if (incorrectSubstrings.Count > 0)
            {
                string message = $"Incorrect string during parsing [{incorrectSubstrings.Count}]:";
            
                foreach (var substring in incorrectSubstrings)
                {
                    message += substring + "/r/n";
                }
            
                Debug.LogError(message);
            }

            return correctSubstrings;
        }

        #endregion

        #region [TextField]

        public static void FitStringInField(Text textField)
        {
            textField.font.RequestCharactersInTexture(textField.text, textField.fontSize, textField.fontStyle);
            string text = textField.text;
            float textFieldWidth = textField.rectTransform.rect.width;
            
            int maxTextSize = 0;
            int index = 0;
            while (maxTextSize < textFieldWidth && index < text.Length)
            {
                textField.font.GetCharacterInfo(text[index], out var info, textField.fontSize, textField.fontStyle);
                maxTextSize += info.advance;
                index++;
            }
        
            textField.text = CutString(text, index);
        }

        #endregion

        #region [Strings]
        
        public static string CutString(string text, int maxTextSize)
        {
            if (text.Length <= maxTextSize) return text;
            return String.Concat(text.Substring(0, maxTextSize - 3).Trim(' '), "...");
        }

        public static string Remove(this string sourceString, string textToRemove)
        {
            return sourceString.Replace(textToRemove, "");
        }

        public static string[] RemoveQuotesAndSplit(this string sourceString)
        {
            return sourceString.RemoveQuotes().Split(',');
        }

        public static string RemoveQuotes(this string sourceString)
        {
            return sourceString.Remove("\"");
        }

        public static string RemoveSpaces(this string sourceString)
        {
            return sourceString.Remove(" ");
        }

        public static string[] HandleStringSplit(string inputString)
        {
            return inputString.TrimEnd(';').Split(';');
        }

        public static string TypeName(this object obj)
        {
            return $"[{obj.GetType().Name}]";
        }

        public static string AddSpaces(this string sourceString)
        {
            return Regex.Replace(sourceString, @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", " $0");
        }

        #endregion
    }
}
