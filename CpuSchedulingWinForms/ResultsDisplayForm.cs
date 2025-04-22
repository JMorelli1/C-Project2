using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.TickGenerators;

public class ResultsDisplay : Form
{
    private DataGridView dataGrid;
    private System.Windows.Forms.Label averageLabel;
    private ScottPlot.WinForms.FormsPlot turnaroundTimePlot;
    private ScottPlot.WinForms.FormsPlot waitTimePlot;

    public ResultsDisplay(List<ProcessControlBlock> pcbList)
    {
        this.Text = "Scheduling Results";
        this.Width = 900;
        this.Height = 1000;

        InitializeGrid();
        InitializePlot();
        PopulateResults(pcbList);
    }

    private void InitializeGrid()
    {
        dataGrid = new DataGridView
        {
            Width = 860,
            Height = 250,
            Top = 10,
            Left = 10,
            ReadOnly = true,
            AllowUserToAddRows = false,
            ColumnCount = 7
        };

        dataGrid.Columns[0].Name = "Process ID";
        dataGrid.Columns[1].Name = "Burst Time";
        dataGrid.Columns[2].Name = "Arrival Time";
        dataGrid.Columns[3].Name = "Priority";
        dataGrid.Columns[4].Name = "Completion Time";
        dataGrid.Columns[5].Name = "Turnaround Time";
        dataGrid.Columns[6].Name = "Waiting Time";

        this.Controls.Add(dataGrid);
    }

    private void InitializePlot()
    {
        turnaroundTimePlot = new ScottPlot.WinForms.FormsPlot
        {
            Width = 660,
            Height = 200,
            Top = 270,
            Left = 10
        };

        waitTimePlot = new ScottPlot.WinForms.FormsPlot
        {
            Width = 660,
            Height = 200,
            Top = 540,
            Left = 10
        };

        this.Controls.Add(turnaroundTimePlot);
        this.Controls.Add(waitTimePlot);
    }

    private void PopulateResults(List<ProcessControlBlock> pcbs)
    {
        double totalTAT = 0, totalWT = 0, totalCT = 0;

        var turnaroundValues = new List<double>();
        var waitValues = new List<double>();
        var labels = new List<string>();
        var cpuUtilization = CalculateCpuUtilization(pcbs);

        foreach (var pcb in pcbs)
        {
            dataGrid.Rows.Add(
                $"P{pcb.ID}",
                pcb.BurstTime,
                pcb.ArrivalTime,
                pcb.Priority,
                pcb.CompletionTime,
                pcb.TurnaroundTime,
                pcb.WaitingTime
            );

            turnaroundValues.Add(pcb.TurnaroundTime);
            waitValues.Add(pcb.WaitingTime);
            labels.Add($"P{pcb.ID}");

            totalTAT += pcb.TurnaroundTime;
            totalWT += pcb.WaitingTime;
            totalCT += pcb.CompletionTime;
        }

        int count = pcbs.Count;
        double avgTAT = totalTAT / count;
        double avgWT = totalWT / count;
        double avgCT = totalCT / count;

        averageLabel = new System.Windows.Forms.Label
        {
            Text = $"Average Completion Time: {avgCT} | Average Turnaround Time: {avgTAT} | Average Waiting Time: {Math.Round(avgWT, 2)} | CPU Utilization: {Math.Round(cpuUtilization, 2)}%",
            Top = 810,
            Left = 10,
            Width = 860
        };
        this.Controls.Add(averageLabel);

        // --- ScottPlot v5 Bar Chart Setup ---

        turnaroundTimePlot.Plot.Clear();
        waitTimePlot.Plot.Clear();
        
        // Turnaround Time Plot
        var bars = new List<Bar>();
        for (int i = 0; i < turnaroundValues.Count; i++)
        {
            bars.Add(new Bar
            {
                Position = i,
                Value = turnaroundValues[i],
                FillColor = Colors.Blue.WithAlpha(180),
            });
        }

        turnaroundTimePlot.Plot.Add.Bars(bars);

        turnaroundTimePlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
            positions: bars.Select(b => b.Position).ToArray(),
            labels: labels.ToArray()
        );

        turnaroundTimePlot.Plot.Axes.Bottom.Label.Text = "Process";
        turnaroundTimePlot.Plot.Axes.Left.Label.Text = "Turnaround Time";
        turnaroundTimePlot.Plot.Title("Turnaround Time");

        turnaroundTimePlot.Refresh();

        // Wait Time Plot
        var bars2 = new List<Bar>();

        for (int i = 0; i < waitValues.Count; i++)
        {
            bars2.Add(new Bar
            {
                Position = i,
                Value = waitValues[i],
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
        waitTimePlot.Plot.Axes.Bottom.Label.Text = "Process";
        waitTimePlot.Plot.Axes.Left.Label.Text = "Wait Time";
        waitTimePlot.Plot.Title("Wait Time");

        waitTimePlot.Refresh();
    }

    private static double CalculateCpuUtilization(List<ProcessControlBlock> pcbs)
{
    if (pcbs == null || pcbs.Count == 0)
        return 0;

    double totalBusyTime = pcbs.Sum(p => p.BurstTime);
    int totalTime = pcbs.Max(p => p.CompletionTime);

    if (totalTime == 0)
        return 0;

    double utilization = (totalBusyTime / totalTime) * 100.0;
    return utilization;
}
}
