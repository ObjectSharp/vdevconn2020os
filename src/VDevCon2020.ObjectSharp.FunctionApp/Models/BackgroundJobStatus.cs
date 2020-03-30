using System;

namespace VDevCon2020.ObjectSharp.FunctionApp.Models
{
    public class BackgroundJobStatus : BackgroundJob
    {
        public long ElapsedMiliseconds { get; set; }
        public double Throughput => ElapsedMiliseconds > 0 ? Math.Round(CompletedTaskCount * 1000.0 / ElapsedMiliseconds, 2) : 0;
        public string Message { get; set; }
        public int CompletedTaskCount { get; set; }
        public int Progress => TaskCount > 0 ? (int)(CompletedTaskCount * 100.0 / TaskCount) : 0;
    }
}
