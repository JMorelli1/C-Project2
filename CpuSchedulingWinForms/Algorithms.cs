using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CpuSchedulingWinForms
{
    public static class Algorithms
    {

        public static List<ProcessControlBlock> fcfsAlgorithm(List<ProcessControlBlock> pcbs)
        {
            int num;
            pcbs.Sort((a, b) => a.ArrivalTime.CompareTo(b.ArrivalTime));
 
            for (num = 0; num <= pcbs.Count - 1; num++){
                
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

            return pcbs;
        }

        public static List<ProcessControlBlock> sjfAlgorithm(List<ProcessControlBlock> pcbs){
            int currentTime = 0;

            var sorted = pcbs
                .OrderBy(p => p.BurstTime)
                .ThenBy(p => p.ArrivalTime)
                .ToList();

            foreach (var pcb in sorted)
            {
                // Wait for the process if it hasn't arrived yet
                if (currentTime < pcb.ArrivalTime)
                    currentTime = pcb.ArrivalTime;

                pcb.StartTime = currentTime;
                pcb.CompletionTime = pcb.StartTime + pcb.BurstTime;
                currentTime = pcb.CompletionTime;
            }

            return pcbs;
        }


        public static List<ProcessControlBlock> priorityAlgorithm(List<ProcessControlBlock> pcbs){
            int currentTime = 0;

            var sorted = pcbs
                .OrderBy(p => p.Priority)
                .ThenBy(p => p.ArrivalTime)
                .ToList();

            foreach (var pcb in sorted)
            {
                // Idle
                if (currentTime < pcb.ArrivalTime)
                    currentTime = pcb.ArrivalTime;

                pcb.StartTime = currentTime;
                pcb.CompletionTime = pcb.StartTime + pcb.BurstTime;

                currentTime = pcb.CompletionTime;
            }

            return pcbs;
        }


        public static List<ProcessControlBlock> roundRobinAlgorithm(List<ProcessControlBlock> pcbs, int quantumTime){
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
                    // Idle
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

            return pcbs;
        }

        public static List<ProcessControlBlock> srtfAlgorithm(List<ProcessControlBlock> pcbs){
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
            
            return pcbs;
        }

        public static List<ProcessControlBlock> hrrnAlgorithm(List<ProcessControlBlock> pcbs){
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

            return pcbs;
        }

        public static List<ProcessControlBlock> runAlgorithm(List<ProcessControlBlock> pcbs, string selectedType){
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
                    int quantumTime = getTimeQuantum();
                    roundRobinAlgorithm(pcbs, quantumTime);
                    break;
                case "SRTF":
                    srtfAlgorithm(pcbs);
                    break;
                case "HRRN":
                    hrrnAlgorithm(pcbs);
                    break;
            }

            return pcbs;
        }

        public static List<AlgorithmResults> runAlgorithms(List<ProcessControlBlock> pcbs, string selectedType){
            int quantumTime = getTimeQuantum();

            List<AlgorithmResults> results = new List<AlgorithmResults>();
            int count = 0;

            while(count < 6){
                string algorithm = "";

                // Create deep copy for multiple runs
                List<ProcessControlBlock> deepCopy = pcbs
                    .Select(item => new ProcessControlBlock
                    {
                        ID = item.ID,
                        BurstTime = item.BurstTime,
                        ArrivalTime = item.ArrivalTime,
                        Priority = item.Priority,
                        RemainingTime = item.RemainingTime
                    }).ToList();
                
                switch(count){
                    case 0:
                        algorithm = "FCFS";
                        fcfsAlgorithm(deepCopy);
                        break;
                    case 1:
                        algorithm = "SJF";
                        sjfAlgorithm(deepCopy);
                        break;
                    case 2:
                        algorithm = "PRIORITY";
                        priorityAlgorithm(deepCopy);
                        break;
                    case 3:
                        algorithm = "RR";
                        roundRobinAlgorithm(deepCopy, quantumTime);
                        break;
                    case 4:
                        algorithm = "SRTF";
                        srtfAlgorithm(deepCopy);
                        break;
                    case 5:
                        algorithm = "HRRN";
                        hrrnAlgorithm(deepCopy);
                        break;
                }

                results.Add(new AlgorithmResults(algorithm, deepCopy));
                count++;
            }
            return results;
        }

        private static int getTimeQuantum(){
            string timeQuantumInput = Microsoft.VisualBasic.Interaction.InputBox("Enter time quantum: ", "Time Quantum", "", -1, -1);

            if(string.IsNullOrEmpty(timeQuantumInput))
                throw new Exception("Can not run without time quantum");

            return Convert.ToInt32(timeQuantumInput);
        }
    }
}

