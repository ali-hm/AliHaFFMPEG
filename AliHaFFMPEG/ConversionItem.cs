namespace AliHaFFMPEG
{
    public enum QueueStatus
    {
        Pending,
        Converting,
        Done,
        Failed,
        Canceled
    }

    public class QueueItem
    {
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public QueueStatus Status { get; set; }
        public double? TotalSeconds { get; set; }
    }
}
