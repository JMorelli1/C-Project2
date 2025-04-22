using System;
using System.Collections.Generic;
using System.IO;

public class FileReader
{
    public static List<ProcessControlBlock> LoadProcessesFromCsv(string filePath)
    {
        var pcbList = new List<ProcessControlBlock>();

        Console.WriteLine(filePath);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("CSV file not found.", filePath);
        }

        using (var reader = new StreamReader(filePath))
        {
            string? line;
            bool isFirstLine = true;

            while ((line = reader.ReadLine()) != null)
            {
                if (isFirstLine)
                {
                    isFirstLine = false; // skip header
                    continue;
                }

                var fields = line.Split(',');

                if (fields.Length < 3)
                    continue; // skip malformed lines

                int id = int.Parse(fields[0]);
                int arrivalTime = int.Parse(fields[1]);
                int burstTime = int.Parse(fields[2]);
                int priority = fields.Length > 3 ? int.Parse(fields[3]) : -1;

                var pcb = new ProcessControlBlock(id, arrivalTime, burstTime, priority);
                pcbList.Add(pcb);
            }
        }

        return pcbList;
    }
}