using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CSP {
    public class Solver<T> {
        public struct IsPuzzleNotCorrect<T1> {
            public bool IsNotCorrect;
            public Puzzle<T1> Puzzle;
            public TimeSpan elapsed;
        }
        public Puzzle<T> puzzle;
        public IsPuzzleNotCorrect<T> solvedPuzzle;

        public Solver(Puzzle<T> puzzle, string methodName, string heuristicName) {
            var timer = new Stopwatch();
            timer.Start();
            var functions = new Dictionary<string, Func<IsPuzzleNotCorrect<T>, Func<Variable<T>[], Variable<T>>, IsPuzzleNotCorrect<T>>>();
            var heuristics = new Dictionary<string, Func<Variable<T>[], Variable<T>>>();
            functions["Backtrack"] = this.Backtrack;
            functions["BacktrackWithForward"] = this.BacktrackWithForward;
            heuristics["NextEmpty"] = this.NextEmpty;
            heuristics["RandomEmpty"] = this.RandomEmpty;
            heuristics["SortEmpty"] = this.SortEmpty;
            heuristics["SDF"] = this.SDF;
            this.solvedPuzzle = functions[methodName](new IsPuzzleNotCorrect<T> { IsNotCorrect = false, Puzzle = puzzle } , heuristics[heuristicName]);
            timer.Stop();
            Console.WriteLine("Jolka number {0}", solvedPuzzle.Puzzle.id);
            solvedPuzzle.Puzzle.Print();
            solvedPuzzle.elapsed = timer.Elapsed;
        }
        public IsPuzzleNotCorrect<T> Backtrack(IsPuzzleNotCorrect<T> puzzle, Func<Variable<T>[], Variable<T>> heuristic) {
            if (puzzle.Puzzle.IsSolution()) {
                return puzzle;
            }
            Variable<T> emptyField = heuristic(puzzle.Puzzle.vec);
            foreach (T val in puzzle.Puzzle.globalDomain) {
                puzzle.Puzzle.steps += 1;
                emptyField.value = val;
                if (puzzle.Puzzle.IsAcceptable(emptyField)) {
                    if (Backtrack(puzzle, heuristic).IsNotCorrect) {
                        emptyField.value = default(T);
                    } else {
                        return puzzle;
                    }
                } else {
                    emptyField.value = default(T);
                }
            }
            puzzle.IsNotCorrect = true;
            return puzzle;
        }
        public IsPuzzleNotCorrect<T> BacktrackWithForward(IsPuzzleNotCorrect<T> puzzle, Func<Variable<T>[], Variable<T>> heuristic) {
            if (puzzle.Puzzle.IsSolution()) {
                return puzzle;
            }
            if (heuristic.Method.Name == "SDF") {
                foreach (var va in puzzle.Puzzle.vec.Where((a) =>Equals(a.value, null))) {
                    puzzle.Puzzle.CalculateDomain(va);
                }
            }
            Variable<T> emptyField = heuristic(puzzle.Puzzle.vec);
                if (heuristic.Method.Name != "SDF") {
                puzzle.Puzzle.CalculateDomain(emptyField);
            }
            foreach (T val in puzzle.Puzzle.globalDomain.Where((x, index) => emptyField.domain.mask[index])) {
                puzzle.Puzzle.steps += 1;
                emptyField.value = val;
                    if (BacktrackWithForward(puzzle, heuristic).IsNotCorrect) {
                        emptyField.value = default(T);
                    } else {
                        return puzzle;
                    }
            }
            puzzle.IsNotCorrect = true;
            return puzzle;
        }
        public Variable<T> NextEmpty(Variable<T>[] vec) {
            return vec.First((x) => Equals(x.value, default(T)));
        }
        public Variable<T> RandomEmpty(Variable<T>[] vec) {
            Random rnd = new Random();
            var emptyFields = vec.Where((x) => Equals(x.value, default(T))).ToArray();
            return emptyFields[rnd.Next(emptyFields.Length)];
        }
        public Variable<T> SortEmpty(Variable<T>[] vec) {
            return NextEmpty(vec.OrderBy((a) => a.x).ThenBy((a) => a.y).ToArray());
        }
        public Variable<T> SDF(Variable<T>[] vec) {
            return vec.OrderBy((a) => a.domain.mask.Count((b) => b)).ToArray().First((x) => Equals(x.value, default(T)));
        }
    }
}