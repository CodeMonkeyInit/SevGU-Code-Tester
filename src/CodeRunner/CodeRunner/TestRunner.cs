using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CodeRunner
{
    public class TestRunner
    {
        private readonly CodeRunnerConfiguration _runnerConfiguration;

        public TestRunner(CodeRunnerConfiguration runnerConfiguration)
        {
            _runnerConfiguration = runnerConfiguration;
        }

        public async Task<TestRunResult> Run(TestRunData testRunData)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = _runnerConfiguration.ProgramFileName,
                Arguments = _runnerConfiguration.ProgramArguments,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var testRunResult = new TestRunResult();

            var process = Process.Start(processStartInfo);

            await process.StandardInput.WriteAsync(testRunData.InputData);

            process.StandardInput.Close();

            var programOutputTask = process.StandardOutput.ReadToEndAsync();

            do
            {
                process.Refresh();

                testRunResult.PeakMemoryUsageInBytes = process.WorkingSet64;
                testRunResult.ExecutionTime = DateTime.Now - process.StartTime;

                if (testRunResult.PeakMemoryUsageInBytes > _runnerConfiguration.MemoryLimitInBytes)
                {
                    return await SetTestResultAndKill(testRunResult, process, RunResult.MemoryLimit);
                }

                if (testRunResult.ExecutionTime.TotalMilliseconds > _runnerConfiguration.ExecutionTimeLimitInMs)
                {
                    return await SetTestResultAndKill(testRunResult, process, RunResult.TimeLimit);
                }
            } while (!process.WaitForExit(_runnerConfiguration.ProcessUpdateFrequencyInMs));

            var programOutput = await programOutputTask;

            var outputFilePath =
                $"{Path.GetDirectoryName(_runnerConfiguration.ProgramFileName)}{Path.PathSeparator}output.txt";

            if (string.IsNullOrWhiteSpace(programOutput) && File.Exists(outputFilePath))
            {
                programOutput = File.ReadAllText(outputFilePath);
            }

            if (programOutput.Trim() != testRunData.ExpectedOutput)
            {
                await GetErrors(testRunResult, process);

                testRunResult.RunResult = RunResult.Fail;
            }

            return testRunResult;
        }

        private static async Task<TestRunResult> SetTestResultAndKill(TestRunResult testRunResult, Process process,
            RunResult runResult)
        {
            testRunResult.RunResult = runResult;

            process.Kill();

            await GetErrors(testRunResult, process);

            return testRunResult;
        }

        private static async Task GetErrors(TestRunResult testRunResult, Process process) =>
            testRunResult.Errors = await process.StandardError.ReadToEndAsync();
    }
}
