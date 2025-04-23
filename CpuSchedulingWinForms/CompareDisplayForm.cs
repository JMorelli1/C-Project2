using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ScottPlot;

public class CompareDisplay : Form
{
    private DataGridView dataGrid;
    private ScottPlot.WinForms.FormsPlot turnaroundTimePlot;
    private ScottPlot.WinForms.FormsPlot waitTimePlot;
    private ScottPlot.WinForms.FormsPlot completionTimePlot;

    public CompareDisplay(List<AlgorithmResults> algorithmResults)
    {
        this.Text = "Scheduling Results";
        this.Width = 700;
        this.Height = 900;

        InitializeGrid();
        InitializePlot();
        PopulateResults(algorithmResults);
    }

    private void InitializeGrid()
    {
        dataGrid = new DataGridView
        {
            Width = 645,
            Height = 192,
            Top = 10,
            Left = 10,
            ReadOnly = true,
            AllowUserToAddRows = false,
            ColumnCount = 6
        };

        dataGrid.Columns[0].Name = "Algorithm";
        dataGrid.Columns[1].Name = "Avg Turnaround Time";
        dataGrid.Columns[2].Name = "Avg Waiting Time";
        dataGrid.Columns[3].Name = "Avg Completion Time";
        dataGrid.Columns[4].Name = "Throughput";
        dataGrid.Columns[5].Name = "CPU Utilization";

        this.Controls.Add(dataGrid);
    }

    private void InitializePlot()
    {
        turnaroundTimePlot = new ScottPlot.WinForms.FormsPlot
        {
            Width = 660,
            Height = 200,
            Top = 250,
            Left = 10
        };

        waitTimePlot = new ScottPlot.WinForms.FormsPlot
        {
            Width = 660,
            Height = 200,
            Top = 450,
            Left = 10
        };
        
        completionTimePlot = new ScottPlot.WinForms.FormsPlot
        {
            Width = 660,
            Height = 200,
            Top = 650,
            Left = 10
        };

        this.Controls.Add(turnaroundTimePlot);
        this.Controls.Add(waitTimePlot);
        this.Controls.Add(completionTimePlot);
    }

    private void PopulateResults(List<AlgorithmResults> algorithmResults)
    {
        var avgTurnaroundValues = new List<double>();
        var avgWaitValues = new List<double>();
        var avgCompletionTimes = new List<double>();
        var labels = new List<string>();
        int index = 0;

        foreach(AlgorithmResults results in algorithmResults){
            double totalTAT = 0, totalWT = 0, totalCT = 0;
            Metrics calculatedMetrics = CalculateMetrics(results.pcbs);

            foreach (var pcb in results.pcbs)
            {
                totalTAT += pcb.TurnaroundTime;
                totalWT += pcb.WaitingTime;
                totalCT += pcb.CompletionTime;
            }

            int count = results.pcbs.Count;
            double avgTAT = totalTAT / count;
            double avgWT = totalWT / count;
            double avgCT = totalCT / count;

            avgTurnaroundValues.Add(avgTAT);
            avgWaitValues.Add(avgWT);
            avgCompletionTimes.Add(avgCT);
            labels.Add(results.algorithm);

            dataGrid.Rows.Add(
                    results.algorithm,
                    Math.Round(avgTAT, 2),
                    Math.Round(avgWT, 2),
                    Math.Round(avgCT, 2),
                    Math.Round(calculatedMetrics.Throughput, 2),
                    Math.Round(calculatedMetrics.Utilization, 2)
                );
            index++;
        }

        // --- ScottPlot v5 Bar Chart Setup ---

        turnaroundTimePlot.Plot.Clear();
        waitTimePlot.Plot.Clear();
        completionTimePlot.Plot.Clear();
        
        // Turnaround Time Plot
        var bars = new List<Bar>();
        for (int i = 0; i < avgTurnaroundValues.Count; i++)
        {
            bars.Add(new Bar
            {
                Position = i,
                Value = avgTurnaroundValues[i],
                FillColor = Colors.Blue.WithAlpha(180),
            });
        }

        turnaroundTimePlot.Plot.Add.Bars(bars);

        turnaroundTimePlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
            positions: bars.Select(b => b.Position).ToArray(),
            labels: labels.ToArray()
        );

        turnaroundTimePlot.Plot.Axes.Bottom.Label.Text = "Algorithm";
        turnaroundTimePlot.Plot.Axes.Left.Label.Text = "Avg Turnaround Time";
        turnaroundTimePlot.Plot.Title("Avg Turnaround Time");

        turnaroundTimePlot.Refresh();

        // Wait Time Plot
        var bars2 = new List<Bar>();

        for (int i = 0; i < avgWaitValues.Count; i++)
        {
            bars2.Add(new Bar
            {
                Position = i,
                Value = avgWaitValues[i],
                FillColor = Colors.Blue.WithAlpha(180),
            });
        }

        waitTimePlot.Plot.Add.Bars(bars2);

        // Set custom tick labels
        waitTimePlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
            positions: bars.Select(b => b.Position).ToArray(),
            labels: labels.ToArray()
        );

        // Axis labels and title
        waitTimePlot.Plot.Axes.Bottom.Label.Text = "Algorithm";
        waitTimePlot.Plot.Axes.Left.Label.Text = "Avg Wait Time";
        waitTimePlot.Plot.Title("Avg Wait Time");

        waitTimePlot.Refresh();

        // Completion Time Plot
        var bars3 = new List<Bar>();

        for (int i = 0; i < avgCompletionTimes.Count; i++)
        {
            bars3.Add(new Bar
            {
                Position = i,
                Value = avgCompletionTimes[i],
                FillColor = Colors.Blue.WithAlpha(180),
            });
        }

        completionTimePlot.Plot.Add.Bars(bars3);

        // Set custom tick labels
        completionTimePlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
            positions: bars.Select(b => b.Position).ToArray(),
            labels: labels.ToArray()
        );

        // Axis labels and title
        completionTimePlot.Plot.Axes.Bottom.Label.Text = "Algorithm";
        completionTimePlot.Plot.Axes.Left.Label.Text = "Avg Wait Time";
        completionTimePlot.Plot.Title("Avg Wait Time");

        completionTimePlot.Refresh();
    }

    public class Metrics{
        public double Utilization{ get; } = -1;
        public double Throughput{ get; } = -1;

        public Metrics(double utilization, double throughput){
            Utilization = utilization;
            Throughput = throughput;
        }

    }

    public static Metrics CalculateMetrics(List<ProcessControlBlock> pcbs){
        if (pcbs == null || pcbs.Count == 0)
            return new Metrics(0, 0);

        double totalBusyTime = pcbs.Sum(p => p.BurstTime);
        int completedProcesses = pcbs.Count;
        int totalTime = pcbs.Max(p => p.CompletionTime);

        if (totalTime == 0)
            return new Metrics(0, 0);

        double utilization = totalBusyTime / totalTime * 100.0;
        double throughput = (double)completedProcesses / totalTime;

        return new Metrics(utilization, throughput);
    }
}
