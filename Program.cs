using System;
using System.Collections.Generic;
using System.Linq;

namespace CSP {
    class Program {
        static void Main(string[] args) {
            GraphJolka();
        }
        static void GraphJolka() {
            string jolkaDir = "Jolka";
            string jolkaHeuristic = "SortEmpty";
            string jolkaFwHeuristic = "SDF";
            List<Jolka> jolkasBacktrack = Loader.LoadJolka(jolkaDir);
            List<Jolka> jolkasBacktrackFw = Loader.LoadJolka(jolkaDir);
            List<Solver<string>> jolkaBacktrackSolvers = new List<Solver<string>>();
            List<Solver<string>> jolkaBacktrackFwSolvers = new List<Solver<string>>();
            foreach (Jolka jolka in jolkasBacktrack) {
                if (jolka.id == 0) continue;
                if (jolka.id == 3) continue;
                jolkaBacktrackSolvers.Add(new Solver<string>(jolka, "Backtrack", jolkaHeuristic));
            }
            foreach (Jolka jolka in jolkasBacktrackFw) {
                if (jolka.id == 0) continue;
                jolkaBacktrackFwSolvers.Add(new Solver<string>(jolka, "BacktrackWithForward", jolkaFwHeuristic));
            }
            GraphGenerator.RunJolka(jolkaBacktrackSolvers, jolkaBacktrackFwSolvers, jolkaHeuristic, jolkaFwHeuristic, "time");
            GraphGenerator.RunJolka(jolkaBacktrackSolvers, jolkaBacktrackFwSolvers, jolkaHeuristic, jolkaFwHeuristic, "steps");
        }
        static void GraphSudoku() {
            string sudokuFilename = "Sudoku.csv";
            string backtrackHeuristic = "NextEmpty";
            string backtrackFwHeuristic = "SDF";
            List<Sudoku> sudokuBacktrackBoards = Loader.LoadSudoku(sudokuFilename);
            List<Sudoku> sudokuBacktrackFwBoards = Loader.LoadSudoku(sudokuFilename);
            List<Solver<int>> sudokuBacktrackSolvers = new List<Solver<int>>();
            List<Solver<int>> sudokuBacktrackFwSolvers = new List<Solver<int>>();
            foreach (Sudoku sudoku in sudokuBacktrackBoards) {
                sudokuBacktrackSolvers.Add(new Solver<int>(sudoku, "Backtrack", backtrackHeuristic));
            }
            foreach (Sudoku sudoku in sudokuBacktrackFwBoards) {
                sudokuBacktrackFwSolvers.Add(new Solver<int>(sudoku, "BacktrackWithForward", backtrackFwHeuristic));
            }
            var groupedBacktrack = sudokuBacktrackSolvers.GroupBy((a) => a.solvedPuzzle.Puzzle.difficulty).ToList();
            var groupedBacktrackFw = sudokuBacktrackFwSolvers.GroupBy((a) => a.solvedPuzzle.Puzzle.difficulty).ToList();
            var averageBacktrackTimes = new List<double>();
            var averageBacktrackFwTimes = new List<double>();
            var averageBacktrackSteps = new List<double>();
            var averageBacktrackFwSteps = new List<double>();
            foreach (var el in groupedBacktrack) {
                double time = 0;
                double steps = 0;
                foreach (var elel in el) {
                    time += elel.solvedPuzzle.elapsed.TotalMilliseconds;
                    steps += elel.solvedPuzzle.Puzzle.steps;
                }
                time /= el.Count();
                steps /= el.Count();
                averageBacktrackTimes.Add(time);
                averageBacktrackSteps.Add(steps);
            }
            foreach (var el in groupedBacktrackFw) {
                double time = 0;
                double steps = 0;
                foreach (var elel in el) {
                    time += elel.solvedPuzzle.elapsed.TotalMilliseconds;
                    steps += elel.solvedPuzzle.Puzzle.steps;
                }
                time /= el.Count();
                steps /= el.Count();
                averageBacktrackFwTimes.Add(time);
                averageBacktrackFwSteps.Add(steps);
            }
            string type = "time";
            GraphGenerator.RunSudoku(Array.ConvertAll(Enumerable.ToArray<int>(Enumerable.Range(0, 10)), (a) => (double)a), averageBacktrackTimes.ToArray(), averageBacktrackFwTimes.ToArray(), backtrackHeuristic, backtrackFwHeuristic, type);
            type = "steps";
            GraphGenerator.RunSudoku(Array.ConvertAll(Enumerable.ToArray<int>(Enumerable.Range(0, 10)), (a) => (double)a), averageBacktrackSteps.ToArray(), averageBacktrackFwSteps.ToArray(), backtrackHeuristic, backtrackFwHeuristic, type);
        }
    }
}
