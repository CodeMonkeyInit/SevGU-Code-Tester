using System;

namespace CodeRunner
{
    public class TestRunResult
    {
        public RunResult RunResult { get; set; } = RunResult.Success;

        public long PeakMemoryUsageInBytes { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public string Errors { get; set; } = string.Empty;
    }
}