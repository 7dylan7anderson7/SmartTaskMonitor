using System;

namespace SmartTaskMonitor.Core.Models
{
    public class TaskLog
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Success, Failed, Warning
        public double DurationSeconds { get; set; }
        public DateTime LastRun { get; set; }
        public int ErrorCount { get; set; }
    }
}