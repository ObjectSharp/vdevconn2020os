using System;

namespace VDevCon2020.ObjectSharp.FunctionApp.Models
{
    public class BackgroundJob
    {
        public string Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Name { get; set; }
        public int TaskCount { get; set; }
        public int BatchSize { get; set; }
        public int MinTaskLength { get; set; }
        public int MaxTaskLength { get; set; }
    }

    public class BackgroundJob<T> : BackgroundJob
    {
        public T Payload { get; set; }
    }
}
