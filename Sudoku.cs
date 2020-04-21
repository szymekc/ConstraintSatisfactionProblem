using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CSP {
    public class Sudoku : Puzzle<int> {
        public Sudoku(int id, double difficulty, Board board, string solution) : base(id, difficulty, board, solution) {
            globalDomain = Enumerable.ToArray<int>(Enumerable.Range(1, board.size));
            constraints.Add(CheckUniqueHori);
            constraints.Add(CheckUniqueVert);
            constraints.Add(CheckUniqueSq);
            this.vec = new Variable<int>[board.size * board.size];
            for (int i = 0; i < board.size; i++) {
                for (int j = 0; j < board.size; j++) {
                    vec[i * board.size + j] = new Variable<int>((int)char.GetNumericValue(board.board[i, j].GetValueOrDefault()), new Domain(board.size), i, j);
                }
            }
        }
        public bool CheckUniqueHori(Variable<int> v) {
            if (v.value == 0) return true;
            for (int i = 0; i < board.size; i++) {
                if (vec[v.x * board.size + i].Equals(v)) {
                    return false;
                }
            }
            return true;
        }
        public bool CheckUniqueVert(Variable<int> v) {
            if (v.value == 0) return true;
            for (int i = 0; i < board.size; i++) {
                if (vec[i * board.size + v.y].Equals(v)) {
                    return false;
                }
            }
            return true;
        }
        public bool CheckUniqueSq(Variable<int> v) {
            if (v.value == 0) return true;
            var sqrt = (int)Math.Sqrt(board.size);
            var xfloor = v.x / sqrt * sqrt;
            var yfloor = v.y / sqrt * sqrt;
            for (int i = xfloor; i < xfloor + sqrt; i++) {
                for (int j = yfloor; j < yfloor + sqrt; j++) {
                    if (vec[i * board.size + j].Equals(v)) {
                        return false;
                    }
                }
            }
            return true;
        }
        public override void CalculateDomain(Variable<int> v) {
            for (int i = 0; i < board.size; i++) {
                v.domain.mask[i] = true;
            }
            foreach (var field in GetAllLinkedFields(v)) {
                if (field.value > 0) {
                    v.domain.mask[field.value - 1] = false;
                }
            }
        }
        public override List<Variable<int>> GetAllLinkedFields(Variable<int> v) {
            var variables = new List<Variable<int>>();
            var sqrt = (int)Math.Sqrt(board.size);
            var xfloor = v.x / sqrt * sqrt;
            var yfloor = v.y / sqrt * sqrt;
            for (int i = xfloor; i < xfloor + sqrt; i++) {
                for (int j = yfloor; j < yfloor + sqrt; j++) {
                    variables.Add(vec[i * board.size + j]);
                }
            }
            for (int i = 0; i < yfloor; i++) {
                variables.Add(vec[v.x * board.size + i]);
            }
            for (int i = yfloor + sqrt; i < board.size; i++) {
                variables.Add(vec[v.x * board.size + i]);
            }
            for (int i = 0; i < xfloor; i++) {
                variables.Add(vec[i * board.size + v.y]);
            }
            for (int i = xfloor + sqrt; i < board.size; i++) {
                variables.Add(vec[i * board.size + v.y]);
            }
            return variables;
        }
        public override void Print() {
            var temp = vec.OrderBy(x => x.x).ThenBy(x => x.y).ToList();
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    if (temp[i * 9 + j].value == 0) {
                        Console.Write("[   ]");
                    } else
                        Console.Write("[ {0} ]", temp[i * 9 + j].value);
                }
                Console.Write("\n\n");
            }
        }
    }
    public class HashComparer : IEqualityComparer<Variable<int>> {
        public bool Equals([AllowNull] Variable<int> x, [AllowNull] Variable<int> y) {
            if (x.x == y.x && x.y == y.y) {
                return true;
            }
            return false;
        }

        public int GetHashCode([DisallowNull] Variable<int> obj) {
            return HashCode.Combine(obj.x, obj.y);
        }
    }
}
