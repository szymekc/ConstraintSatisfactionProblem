using System;
using System.Linq;

namespace CSP {
    public class Board {
        public char?[,] board;
        public int size;
        public int x;
        public int y;

        public Board(char?[,] board) {
            this.board = board;
        }
        public Board(string formula) {
            this.size = (int)Math.Sqrt(formula.Length);
            this.board = new char?[size, size];
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    char ch = formula[i * size + j];
                    if (ch == '.') board[i, j] = '0';
                    else board[i, j] = ch;
                }
            }
        }
        public Board(string formula, bool isJolka) {
            this.y = formula.Split("\n")[0].Length;
            this.x = formula.Count((a) => a.Equals('\n'));
            this.board = new char?[x, y];
            string[] strings = formula.Split("\n");
            for (int i = 0; i < x; i++) {
                for (int j = 0; j < y; j++) {
                    board[i, j] = strings[i][j];
                }
            }

        }
    }
}
