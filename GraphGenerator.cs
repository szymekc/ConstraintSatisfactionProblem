using ScottPlot;
using System;
using System.Collections.Generic;

namespace CSP {
    class GraphGenerator {
        static public void RunSudoku(double[] difficulty, double[] dataB, double[] dataBF, string heuristicB="", string heuristicBF="", string type ="") {
            var plt = new ScottPlot.Plot(1280, 720);
            int pointCount = dataB.Length;
            plt.PlotBar(difficulty, dataB, label: "Backtrack w/ " + heuristicB, barWidth: .3, xOffset: -0.2);
            plt.PlotBar(difficulty, dataBF, label: "Backtrack with forward w/ " + heuristicBF, barWidth: .3, xOffset: 0.2);
            plt.Title("Average " + type +" over sudoku difficulty");
            plt.Axis(y1: 0);
            if (type == "time") {
                plt.YLabel("Execution time [ms]");
            } else {
                plt.YLabel("Steps");
            }
            plt.XLabel("Difficulty");
            plt.Legend(location: legendLocation.upperRight);
            plt.Ticks(useExponentialNotation: false, useMultiplierNotation: false);
            plt.SaveFig(heuristicB + heuristicBF + type + "BvsBF" + ".png");
        }

        static public void RunJolka(List<Solver<string>> backtracks, List<Solver<string>> backtrackfws, string heuristicB = "", string heuristicBF = "", string type = "") {
            var plt = new ScottPlot.Plot(1280, 720);
            List<double> xlist = new List<double>();
            List<double> yfwlist = new List<double>();
            List<double> ylist = new List<double>();

            foreach (var el in backtrackfws) {
                xlist.Add(el.solvedPuzzle.Puzzle.id);
                if (type == "time") {
                    yfwlist.Add(el.solvedPuzzle.elapsed.TotalMilliseconds);
                } else {
                    yfwlist.Add(el.solvedPuzzle.Puzzle.steps);
                }
            }
            foreach (var el in backtracks) {
                if (type == "time") {
                    ylist.Add(el.solvedPuzzle.elapsed.TotalMilliseconds);
                } else {
                    ylist.Add(el.solvedPuzzle.Puzzle.steps);
                }
            }
            double[] xs = xlist.ToArray();
            ylist.Insert(2, (double)1);
            double[] dataB = Tools.Log10(ylist.ToArray());
            double[] dataBF = Tools.Log10(yfwlist.ToArray());
            plt.PlotBar(xs, dataB, label: "Backtrack w/ " + heuristicB, barWidth: .3, xOffset: -0.2, showValues: true);
            plt.PlotBar(xs, dataBF, label: "Backtrack with forward w/ " + heuristicBF, barWidth: .3, xOffset: 0.2, showValues:true);
            plt.Title(type + " over Jolka id");
            plt.Ticks(useExponentialNotation: false, useMultiplierNotation: false, logScaleY: true);
            plt.Axis(y1: 0);
            plt.XTicks(new string[] {"0", "1", "2", "3", "4" });
            if (type == "time") {
                plt.YLabel("Execution time [10 ^ y ms]");
            } else {
                plt.YLabel("Steps [10 ^ y]");
            }
            plt.XLabel("Jolka ID");
            plt.Legend(location: legendLocation.upperRight);
            plt.SaveFig(heuristicB + heuristicBF + type + "JolkaBvsBF" + ".png");
        }

    }
}
