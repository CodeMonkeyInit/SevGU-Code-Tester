using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodeRunner
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var codeRunnerConfiguration =
                JsonConvert.DeserializeObject<CodeRunnerConfiguration>(await File.ReadAllTextAsync(args.First()));

            var testRunner = new TestRunner(codeRunnerConfiguration);

            var testRunnerTasks = codeRunnerConfiguration.TestRunData.Select(data => testRunner.Run(data));

            var testRuns = await Task.WhenAll(testRunnerTasks);

            Console.WriteLine(JsonConvert.SerializeObject(testRuns, Formatting.Indented));
        }
    }
}
