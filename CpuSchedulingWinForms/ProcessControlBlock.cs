public class ProcessControlBlock
{
    // Required process metadata
    public int ID { get; set; }
    public int ArrivalTime { get; set; }
    public int BurstTime { get; set; }
    public int Priority { get; set; } = -1; // -1 if unused

    // For Round Robin or execution tracking
    public int RemainingTime { get; set; }

    // Result values (for Gantt chart / stats)
    public int StartTime { get; set; } = -1;
    public int CompletionTime { get; set; } = -1;
    public int TurnaroundTime => CompletionTime >= 0 && ArrivalTime >= 0
        ? CompletionTime - ArrivalTime
        : -1;
    public int WaitingTime => TurnaroundTime >= 0 && BurstTime >= 0
        ? TurnaroundTime - BurstTime
        : -1;

    // Optional: track if the process has started/completed
    public bool IsStarted => StartTime >= 0;
    public bool IsCompleted => RemainingTime == 0;

    public ProcessControlBlock(){
    
    }

    public ProcessControlBlock(int id, int arrivalTime, int burstTime, int priority = -1){
        ID = id;
        ArrivalTime = arrivalTime;
        BurstTime = burstTime;
        RemainingTime = burstTime;
        Priority = priority;
    }

    // Optional: toString override for debugging
    public override string ToString(){
        return $"P{ID}: AT={ArrivalTime}, BT={BurstTime}, RT={RemainingTime}, PR={Priority}, CT={CompletionTime}";
    }
}