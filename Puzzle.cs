using System;
using System.Collections.Generic;

namespace CSP {
    public abstract class Puzzle<T> {
        public int id;
        public double difficulty;
        public Board board;
        public string solution;
        public Variable<T>[] vec;
        public List<Func<Variable<T>, bool>> constraints;
        public T[] globalDomain;
        public int steps = 0;
        public Puzzle(int id, double difficulty, Board board, string solution) {
            this.id = id;
            this.difficulty = difficulty;
            this.board = board;
            this.solution = solution;
            constraints = new List<Func<Variable<T>, bool>>();
        }
        public abstract void Print();
        public abstract void CalculateDomain(Variable<T> var);
        public virtual List<Variable<T>> GetAllLinkedFields(Variable<int> var) {
            return new List<Variable<T>>();
        }
        public virtual List<(Variable<T>, char, int)> GetAllLinkedFields(Variable<string> var) {
            return new List<(Variable<T>, char, int)>();
        }
        public bool IsSolution() {
            HashSet<T> usedVals = new HashSet<T>();
            foreach (Variable<T> v in vec) {
                usedVals.Add(v.value);
                if (v.value == null) {
                    return false;
                }
                if (v.value.Equals(default(T))) {
                    return false;
                }
            }
            if (usedVals.Count != globalDomain.Length) {
                return false;
            }
            return true;
        }
        public bool IsAcceptable(Variable<T> v) {
            foreach (Func<Variable<T>, bool> fun in constraints) {
                if (!fun(v)) {
                    return false;
                }
            }
            return true;
        }
        public Puzzle<T> Clone() {
            return (Puzzle<T>)MemberwiseClone();
        }
    }
}
