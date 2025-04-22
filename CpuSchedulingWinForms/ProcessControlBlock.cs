public class ProcessControlBlock
{
    public int ID { get; set; }
    public int ArrivalTime { get; set; }
    public int BurstTime { get; set; }
    public int Priority { get; set; } = -1; // -1 if unused
    public int RemainingTime { get; set; }
    public int StartTime { get; set; } = -1;
    public int CompletionTime { get; set; } = -1;
    public int TurnaroundTime => CompletionTime >= 0 && ArrivalTime >= 0
        ? CompletionTime - ArrivalTime
        : -1;
    public int WaitingTime => TurnaroundTime >= 0 && BurstTime >= 0
        ? TurnaroundTime - BurstTime
        : -1;
    public int ResponseTime => ArrivalTime >= 0 && StartTime >= 0 
        ? StartTime - ArrivalTime 
        : -1;

    public ProcessControlBlock(){
    
    }

    public ProcessControlBlock(int id, int arrivalTime, int burstTime, int priority = -1){
        ID = id;
        ArrivalTime = arrivalTime;
        BurstTime = burstTime;
        RemainingTime = burstTime;
        Priority = priority;
    }
}