using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace EverScord.FileIO
{
    public class CSVReader
    {
        static string SPLIT = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static IDictionary<string, List<List<string>>> ReadAugmentSheet(string filePath)
        {
            TextAsset data = Resources.Load(filePath) as TextAsset;

            var lines = Regex.Split(data.text, LINE_SPLIT);
            var dict = new Dictionary<string, List<List<string>>>();

            if (lines.Length <= 3)
            {
                Debug.LogWarning("Invalid augment data sheet format.");
                return dict;
            }

            string currentArmor = "";

            for (int i = 3; i < lines.Length; i++)
            {
                var cells = Regex.Split(lines[i], SPLIT);

                if (cells.Length == 0 || (cells[0].Length > 0 && cells[0][0] == '@') || string.IsNullOrEmpty(cells[1]))
                    continue;

                if (!string.IsNullOrEmpty(cells[0]))
                {
                    currentArmor = cells[0];
                    dict[currentArmor] = new List<List<string>>();
                }

                List<string> entry = new List<string>();

                for (int j = 1; j < cells.Length; j++)
                {
                    string cell = cells[j].TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    entry.Add(cell);
                }

                dict[currentArmor].Add(entry);
            }

            return dict;
        }
    }
}

