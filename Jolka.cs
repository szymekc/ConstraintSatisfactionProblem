using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
namespace CSP {
    public class Jolka : Puzzle<string> {
        public Jolka(int id, double difficulty, Board board, string solution, string[] globalDomain) : base(id, difficulty, board, solution) {
            this.globalDomain = globalDomain;
            constraints.Add(CheckIfLengthOk);
            constraints.Add(CheckIfWordAvailable);
            constraints.Add(CheckIfFieldsVacant);

            var varList = new List<Variable<string>>();
            string leftRightString = "";
            string topDownString = "";
            Queue<(int, int, bool)> posq = new Queue<(int, int, bool)>();
            char prevChar = '#';
            for (int i = 0; i < board.x; i++) {
                for (int j = 0; j < board.y; j++) {
                    if ((prevChar == '\n' || prevChar == '#') && board.board[i, j] == '_') {
                        posq.Enqueue((i, j, false));
                    }
                    leftRightString += board.board[i, j];
                    prevChar = (char)board.board[i, j];
                }
                leftRightString += "\n";
                prevChar = '\n';
            }
            for (int i = 0; i < board.y; i++) {
                for (int j = 0; j < board.x; j++) {
                    if ((prevChar == '\n' || prevChar == '#') && board.board[j, i] == '_') {
                        posq.Enqueue((j, i, true));
                    }
                    topDownString += board.board[j, i];
                    prevChar = (char)board.board[j, i];
                }
                topDownString += "\n";
                prevChar = '\n';
            }

            foreach (var v in leftRightString.Split(new char[] { '#', '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                var tup = posq.Dequeue();
                if (v.Length == 1) continue;
                varList.Add(new Variable<string>(v, new Domain(globalDomain.Length), tup.Item1, tup.Item2, tup.Item3));
            }
            foreach (var v in topDownString.Split(new char[] { '#', '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                var tup = posq.Dequeue();
                if (v.Length == 1) continue;
                varList.Add(new Variable<string>(v, new Domain(globalDomain.Length), tup.Item1, tup.Item2, tup.Item3));
            }
            this.vec = varList.ToArray();
        }

        public bool CheckIfWordAvailable(Variable<string> v) {
            foreach (var el in vec) {
                if (el.Equals(v)) {
                    return false;
                }
            }
            return true;
        }
        public bool CheckIfFieldsVacant(Variable<string> v) {
            var list = GetAllLinkedFields(v);
            foreach (var el in list) {
                if (v.value[el.Item3] != el.Item2) {
                    return false;
                }
            }
            return true;
        }
        public bool CheckIfLengthOk(Variable<string> v) {
            if (v.length != v.value.Length) {
                return false;
            }
            return true;
        }
        public override void CalculateDomain(Variable<string> v) {
            var prev = v.value;
            for (int i = 0; i < globalDomain.Length; i++) {
                v.value = globalDomain[i];
                v.domain.mask[i] = IsAcceptable(v);
            }
            v.value = prev;
            var linked = GetAllLinkedFields(v);
            if (linked.Count == 0) return;
            string matchingString = new string('?', v.length);
            var arr = matchingString.ToCharArray();
            foreach (var el in linked) {
                arr[el.Item3] = el.Item2;
            }
            matchingString = new string(arr);
            for (int i = 0; i < v.domain.mask.Length; i++) {
                v.domain.mask[i] = Regex.IsMatch(globalDomain[i], WildCardToRegular(matchingString));
            }
        }
        public override List<(Variable<string>, char, int)> GetAllLinkedFields(Variable<string> v) { //returns list of intersecting words, along with the characters
            var linkeds = new List<(Variable<string>, char, int)>();                                 //at intersections and their position in variable v
            var vFields = new List<(int, int)>();
            if (v.isVertical) {
                for (int i = 0; i < v.length; i++) {
                    vFields.Add((v.x + i, v.y));
                }
                foreach (var neighbour in vec) {
                    for (int i = 0; i < neighbour.length; i++) {
                        if (!neighbour.isVertical && vFields.Contains((neighbour.x, neighbour.y + i)) && neighbour.value != null) {
                            linkeds.Add((neighbour, neighbour.value[i], vFields.IndexOf((neighbour.x, neighbour.y + i))));
                        }
                    }
                }
            } else {
                for (int i = 0; i < v.length; i++) {
                    vFields.Add((v.x, v.y + i));
                }
                foreach (var neighbour in vec) {
                    for (int i = 0; i < neighbour.length; i++) {
                        if (neighbour.isVertical && vFields.Contains((neighbour.x + i, neighbour.y)) && neighbour.value != null) {
                            linkeds.Add((neighbour, neighbour.value[i], vFields.IndexOf((neighbour.x+i, neighbour.y))));
                        }
                    }
                }
            }
            return linkeds;
        }
        public override void Print() {
            char?[,] b = (char?[,])board.board.Clone();
            foreach(var v in vec) {
                for (int i = 0; i < v.value.Length; i++) {
                    if (v.isVertical) {
                        b[v.x + i, v.y] = v.value[i];
                    } else {
                        b[v.x, v.y + i] = v.value[i];
                    }
                }
            }
            for (int i = 0; i < board.x; i++) {
                for (int j = 0; j < board.y; j++) {
                    if (b[i, j] != null && b[i, j] != '#') {
                        Console.Write(b[i, j].ToString());
                    } else {
                        Console.Write("■");
                    }
                }
                Console.Write("\n");
            }
        }
        private static String WildCardToRegular(String value) {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }
    }
}
