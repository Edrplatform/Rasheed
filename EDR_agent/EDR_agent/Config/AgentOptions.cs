using System;

namespace EDR_agent.Config
{
    public class AgentOptions
    {
        public int BatchSize { get; set; } = 50;
        public int FlushIntervalMs { get; set; } = 2000;
        public int ChannelCapacity { get; set; } = 1000;
        public int WorkerCount { get; set; } = 2;
        public string RawFilePath { get; set; } = string.Empty; // to be set at runtime
        public string NormalizedFilePath { get; set; } = string.Empty; // to be set at runtime
        public int? RotationSizeMb { get; set; } = null; // optional
    }
}