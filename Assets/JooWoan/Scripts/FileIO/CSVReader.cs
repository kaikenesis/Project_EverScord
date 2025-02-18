using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EverScord.FileIO
{
    public class CSVReader
    {
        static string SPLIT = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static IDictionary<string, List<List<string>>> ReadAugmentSheet()
        {
            TextAsset data = ResourceManager.Instance.GetAsset<TextAsset>("ArmorAugmentSheet");

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
                    string cell = TrimCell(cells[j]);
                    entry.Add(cell);
                }

                dict[currentArmor].Add(entry);
            }

            return dict;
        }

        public static List<IDictionary<string, string>> ReadDataSheet(string assetID)
        {
            var list = new List<IDictionary<string, string>>();
            TextAsset data = ResourceManager.Instance.GetAsset<TextAsset>(assetID);

            var lines = Regex.Split(data.text, LINE_SPLIT);

            if (lines.Length <= 3)
                return list;
            
            var header = Regex.Split(lines[1], SPLIT);

            for (int i = 3; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SPLIT);

                if (values.Length < 2 || string.IsNullOrEmpty(values[1]))
                    continue;

                var entry = new Dictionary<string, string>();

                for (int j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    entry[header[j]] = TrimCell(value);
                }

                list.Add(entry);
            }

            return list;
        }

        private static string TrimCell(string value)
        {
            return value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
        }

        public static List<int> SplitCellInt(string cell, char split = ',')
        {
            List<int> list = new List<int>();
            string[] tags = cell.Split(',');

            for (int i = 0; i < tags.Length; i++)
            {
                int.TryParse(tags[i], out int value);
                list.Add(value);
            }

            return list;
        }
        public static List<float> SplitCellFloat(string cell, char split = ',')
        {
            List<float> list = new List<float>();
            string[] tags = cell.Split(',');

            for (int i = 0; i < tags.Length; i++)
            {
                float.TryParse(tags[i], out float value);
                list.Add(value);
            }

            return list;
        }
    }
}

