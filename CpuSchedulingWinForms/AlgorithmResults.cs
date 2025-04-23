using System.Collections.Generic;

public class AlgorithmResults {
    public string algorithm{get;set;}
    public List<ProcessControlBlock> pcbs{get;set;}
    public AlgorithmResults(string algorithm, List<ProcessControlBlock> pcbs)
    {
        this.algorithm = algorithm;
        this.pcbs = pcbs;
    }
}