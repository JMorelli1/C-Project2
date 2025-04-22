using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CpuSchedulingWinForms
{
    public static class Algorithms
    {

        public static void fcfsAlgorithm(List<ProcessControlBlock> pcbs)
        {
            var np = pcbs.Count;
            int num;
            pcbs.Sort((a, b) => a.ArrivalTime.CompareTo(b.ArrivalTime));
 
                for (num = 0; num <= np - 1; num++){
                    
                    ProcessControlBlock pcb = pcbs[num];
                    if (num == 0)
                    {
                        pcb.StartTime = 0;
                        pcb.CompletionTime = pcb.BurstTime;
                    }
                    else
                    {   
                        ProcessControlBlock prevPCB = pcbs[num-1];
                        if(prevPCB.CompletionTime < pcb.ArrivalTime){
                            pcb.StartTime = pcb.ArrivalTime;
                        }
                        else{
                            pcb.StartTime = prevPCB.CompletionTime; 
                        } 
                        pcb.CompletionTime = pcb.StartTime + pcb.BurstTime;
                    }
                }

                // --- Launch Results Display ---
                ResultsDisplay resultsForm = new ResultsDisplay(pcbs);
                resultsForm.ShowDialog();
        }

        public static void sjfAlgorithm(List<ProcessControlBlock> pcbs){
            var np = pcbs.Count;

            double[] wtp = new double[np];
            double[] p = new double[np];
            double twt = 0.0, awt; 
            int x, num;
            double temp = 0.0;
            bool found = false;
            
            List<long> burstTimes = pcbs.Select(pcb => (long)pcb.BurstTime).ToList();
            for (num = 0; num <= np - 1; num++)
            {
                p[num] = burstTimes[num]; // Creates a secondary list of burstTimes
            }
            for (x = 0; x <= np - 2; x++)
            {
                for (num = 0; num <= np - 2; num++)
                {
                    if (p[num] > p[num + 1])
                    {
                        temp = p[num];
                        p[num] = p[num + 1];
                        p[num + 1] = temp;
                    }
                }
            }
            for (num = 0; num <= np - 1; num++)
            {
                if (num == 0)
                {
                    for (x = 0; x <= np - 1; x++)
                    {
                        if (p[num] == burstTimes[x] && found == false)
                        {   
                            wtp[num] = 0;
                            MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time:", MessageBoxButtons.OK, MessageBoxIcon.None);
                            //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                            burstTimes[x] = 0;
                            found = true;
                        }
                    }
                    found = false;
                }
                else
                {
                    for (x = 0; x <= np - 1; x++)
                    {
                        if (p[num] == burstTimes [x] && found == false)
                        {
                            wtp[num] = wtp[num - 1] + p[num - 1];
                            MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time", MessageBoxButtons.OK, MessageBoxIcon.None);
                            //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                            burstTimes[x] = 0;
                            found = true;
                        }
                    }
                    found = false;
                }
            }
            for (num = 0; num <= np - 1; num++)
            {
                twt = twt + wtp[num];
            }
            MessageBox.Show("Average waiting time for " + np + " processes" + " = " + (awt = twt / np) + " sec(s)", "Average waiting time", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void priorityAlgorithm(List<ProcessControlBlock> pcbs){
                int np = pcbs.Count;

                double[] sp = new double[np];
                double[] wtp = new double[np + 1];
                int x, num;
                double twt = 0.0;
                double awt;
                int temp = 0;
                bool found = false;

                List<long> burstTimes = pcbs.Select(pcb => (long)pcb.BurstTime).ToList();
                List<long> priorties = pcbs.Select(pcb => (long)pcb.Priority).ToList();

                for (num = 0; num <= np - 1; num++)
                {
                    sp[num] = priorties[num];
                }
                for (x = 0; x <= np - 2; x++)
                {
                    for (num = 0; num <= np - 2; num++)
                    {
                        if (sp[num] > sp[num + 1])
                        {
                            temp = (int)sp[num];
                            sp[num] = sp[num + 1];
                            sp[num + 1] = temp;
                        }
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    if (num == 0)
                    {
                        for (x = 0; x <= np - 1; x++)
                        {
                            if (sp[num] == priorties[x] && found == false)
                            {
                                wtp[num] = 0;
                                MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time", MessageBoxButtons.OK);
                                //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                                temp = x;
                                priorties[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                    else
                    {
                        for (x = 0; x <= np - 1; x++)
                        {
                            if (sp[num] == priorties[x] && found == false)
                            {
                                wtp[num] = wtp[num - 1] + burstTimes[temp];
                                MessageBox.Show("Waiting time for P" + (x + 1) + " = " + wtp[num], "Waiting time", MessageBoxButtons.OK);
                                //Console.WriteLine("\nWaiting time for P" + (x + 1) + " = " + wtp[num]);
                                temp = x;
                                priorties[x] = 0;
                                found = true;
                            }
                        }
                        found = false;
                    }
                }
                for (num = 0; num <= np - 1; num++)
                {
                    twt = twt + wtp[num];
                }
                MessageBox.Show("Average waiting time for " + np + " processes" + " = " + (awt = twt / np) + " sec(s)", "Average waiting time", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void roundRobinAlgorithm(List<ProcessControlBlock> pcbs, int quantumTime){
            int np = pcbs.Count;
            int i, counter = 0;
            double total = 0.0;

            int x = np;

            Helper.QuantumTime = quantumTime.ToString();

            // Initialize RemainingTime for all processes
            foreach (var pcb in pcbs)
            {
                pcb.RemainingTime = pcb.BurstTime;
            }

            for (total = 0, i = 0; x != 0;)
            {

                bool foundProcess = false;

                for (int j = 0; j < np; j++)
                {
                    if (pcbs[j].ArrivalTime <= total && pcbs[j].RemainingTime > 0)
                    {
                        i = j;
                        foundProcess = true;
                        break;
                    }
                }

                if (!foundProcess)
                {
                    // No process is ready -> CPU idle
                    total++;
                    continue;
                }

                if (pcbs[i].RemainingTime <= quantumTime && pcbs[i].RemainingTime > 0)
                {
                    total += pcbs[i].RemainingTime;
                    pcbs[i].RemainingTime = 0;
                    counter = 1;
                }
                else if (pcbs[i].RemainingTime > 0)
                {
                    pcbs[i].RemainingTime -= quantumTime;
                    total += quantumTime;
                }

                if (pcbs[i].RemainingTime == 0 && counter == 1)
                {
                    x--;

                    var pcb = pcbs[i];
                    pcb.CompletionTime = (int)total;
                    pcb.StartTime = pcb.StartTime == -1 ? (int)(total - pcb.BurstTime) : pcb.StartTime;

                    counter = 0;
                }

                if (i == np - 1)
                {
                    i = 0;
                }
                else if (i + 1 < np && pcbs[i + 1].ArrivalTime <= total)
                {
                    i++;
                }
                else
                {
                    i = 0;
                }
            }

            // --- Launch Results Display ---
            ResultsDisplay resultsForm = new ResultsDisplay(pcbs);
            resultsForm.ShowDialog();
        }

        public static void srtfAlgorithm(List<ProcessControlBlock> pcbs){
            int time = 0;
            int completed = 0;

            while (completed < pcbs.Count){
                // Get processes that have arrived and are not completed
                var available = pcbs
                    .Where(p => p.ArrivalTime <= time && p.RemainingTime > 0)
                    .OrderBy(p => p.RemainingTime)
                    .ThenBy(p => p.ArrivalTime)
                    .ToList();

                if (available.Any()){
                    var current = available.First();

                    // Set start time if not already done
                    if (current.StartTime == -1)
                        current.StartTime = time;

                    // Execute 1 unit of time
                    current.RemainingTime--;
                    time++;

                    // If finished, record completion
                    if (current.RemainingTime == 0){
                        current.CompletionTime = time;
                        completed++;
                    }
                }
                else{
                    // CPU Idle 
                    time++;
                }
            }
            
            // --- Launch Results Display ---
            ResultsDisplay resultsForm = new ResultsDisplay(pcbs);
            resultsForm.ShowDialog();
        }

        public static void hrrnAlgorithm(List<ProcessControlBlock> pcbs){
            int time = 0;
            int completed = 0;

            var remaining = new List<ProcessControlBlock>(pcbs);

            while (completed < pcbs.Count)
            {
                // Get processes that have arrived but not completed
                var available = remaining
                    .Where(p => p.ArrivalTime <= time && p.CompletionTime == -1)
                    .ToList();

                if (!available.Any())
                {
                    // Idle, no process has arrived yet
                    time++;
                    continue;
                }

                // Calculate response ratios
                var selected = available
                    .Select(p => new
                    {
                        Process = p,
                        ResponseRatio = (double)(time - p.ArrivalTime + p.BurstTime) / p.BurstTime
                    })
                    .OrderByDescending(x => x.ResponseRatio)
                    .First()
                    .Process;

                // Set start and completion time
                selected.StartTime = time;
                time += selected.BurstTime;
                selected.CompletionTime = time;

                completed++;
            }

            // --- Launch Results Display ---
            ResultsDisplay resultsForm = new ResultsDisplay(pcbs);
            resultsForm.ShowDialog();
        }

        public static void runAlgorithm(List<ProcessControlBlock> pcbs, string selectedType){
            switch(selectedType){
                case "FCFS":
                    fcfsAlgorithm(pcbs);
                    break;
                case "SJF":
                    sjfAlgorithm(pcbs);
                    break;
                case "PRIORITY":
                    priorityAlgorithm(pcbs);
                    break;
                case "RR":
                    string timeQuantumInput = Microsoft.VisualBasic.Interaction.InputBox("Enter time quantum: ", "Time Quantum", "", -1, -1);
                    int quantumTime = Convert.ToInt32(timeQuantumInput);

                    roundRobinAlgorithm(pcbs, quantumTime);
                    break;
                case "SRTF":
                    srtfAlgorithm(pcbs);
                    break;
                case "HRRN":
                    hrrnAlgorithm(pcbs);
                    break;
            }
        }
    }
}

