using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/*
	CSVReader by Dock. (24/8/11)
	http://starfruitgames.com
 
	usage: 
	CSVReader.SplitCsvGrid(textString)
*/
namespace VNEngine
{
    public static class CSVReader
    {
        // Generates a dictionary for our localized languages
        public static Dictionary<string, Dictionary<string, string>> Generate_Localized_Dictionary(TextAsset text_asset)
        {
            Dictionary<string, Dictionary<string, string>> language_dictionaries = new Dictionary<string, Dictionary<string, string>>();

            // Read in the Localized UI so we change the labels of our UI to the correct language
            string[,] split_csv = CSVReader.SplitCsvGrid(text_asset.text);

            // Put these values into multiple dictionaries based on their language
            // Each language gets its own dictionary
            // Then the specific dictionary is searched
            for (int x = 1; x < split_csv.GetUpperBound(0); x++)
            {
                // Create a dictionary for this language
                Dictionary<string, string> new_dictionary = new Dictionary<string, string>();

                if (string.IsNullOrEmpty(split_csv[x, 0]))
                    break;

                // Populate the entries
                for (int y = 1; y < split_csv.GetUpperBound(1); y++)
                {
                    string key = split_csv[0, y];
                    string value = split_csv[x, y];
                    if (!string.IsNullOrEmpty(key) || !string.IsNullOrEmpty(value))
                    {
                        new_dictionary.Add(key, value);
                    }
                }

                language_dictionaries.Add(split_csv[x, 0], new_dictionary);
            }

            return language_dictionaries;
        }


        // outputs the content of a 2D array, useful for checking the importer
        public static void DebugOutputGrid(string[,] grid)
        {
            string textOutput = "";
            for (int y = 0; y < grid.GetUpperBound(1); y++)
            {
                for (int x = 0; x < grid.GetUpperBound(0); x++)
                {

                    textOutput += grid[x, y];
                    textOutput += "|";
                }
                textOutput += "\n";
            }
            Debug.Log(textOutput);
        }


        // Splits a CSV file into a 2D string array
        private static string[,] SplitCsvGrid(string csvText)
        {
            string[] lines = csvText.Split("\n"[0]);

            // finds the max width of row
            int width = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                string[] row = SplitCsvLine(lines[i]);
                width = Mathf.Max(width, row.Length);
            }

            // creates new 2D string grid to output to
            string[,] outputGrid = new string[width + 1, lines.Length + 1];
            for (int y = 0; y < lines.Length; y++)
            {
                string[] row = SplitCsvLine(lines[y]);
                for (int x = 0; x < row.Length; x++)
                {
                    outputGrid[x, y] = row[x];

                    // This line was to replace "" with " in my output. 
                    // Include or edit it as you wish.
                    outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
                }
            }

            return outputGrid;
        }


        // Splits a CSV row 
        private static string[] SplitCsvLine(string line)
        {
            return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
            @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                    select m.Groups[1].Value).ToArray();
        }
    }
}