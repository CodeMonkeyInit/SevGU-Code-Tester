using System.Collections.Generic;

namespace CodeRunner
{
    public class CodeRunnerConfiguration
    {
        public string ProgramFileName { get; set; }

        public string ProgramArguments { get; set; }

        public long MemoryLimitInBytes { get; set; }

        public long ExecutionTimeLimitInMs { get; set; }

        public int ProcessUpdateFrequencyInMs { get; set; }

        public IEnumerable<TestRunData> TestRunData { get; set; }
    }
}