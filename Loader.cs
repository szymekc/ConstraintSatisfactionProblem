using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.IO;

namespace CSP {
    public class Loader {
        public static List<Sudoku> LoadSudoku(string filename) {
            using (TextFieldParser parser = new TextFieldParser(filename)) {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                string[] fields = parser.ReadFields();
                List<Sudoku> puzzles = new List<Sudoku>();
                while (!parser.EndOfData) {
                    fields = parser.ReadFields();
                    puzzles.Add(new Sudoku(int.Parse(fields[0]), double.Parse(fields[1]), new Board(fields[2]), fields[3]));
                }
                return puzzles;
            }
        }
        public static List<Jolka> LoadJolka(string directory) {
            DirectoryInfo d = new DirectoryInfo(directory);
            var puzzles = new List<Jolka>();
            int i = 0;
            var words = LoadJolkaWords(directory);
            foreach (var file in d.GetFiles("puzzle*")) {
                puzzles.Add(new Jolka(i, (double)i, new Board(File.ReadAllText(file.FullName), true), "", words[i].Split("\n", System.StringSplitOptions.RemoveEmptyEntries)));
                i++;
            }
            return puzzles;
        }
        public static List<string> LoadJolkaWords(string directory) {
            DirectoryInfo d = new DirectoryInfo(directory);
            var words = new List<string>();
            int i = 0;
            foreach (var file in d.GetFiles("words*")) {
                words.Add(File.ReadAllText(file.FullName));
            }
            return words;
        }
    }
}
