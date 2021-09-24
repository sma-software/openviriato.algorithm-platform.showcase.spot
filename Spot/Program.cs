using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using SMA.AlgorithmPlatform.CSharpClient;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot {
    public class Program {
        public static int Main(string[] args) {
            var parser = new Parser(with => {
                with.CaseInsensitiveEnumValues = true;
            });
            var parserResult = parser.ParseArguments<CommandLineOptions>(args);

            return parserResult.MapResult(RunAlgorithm, HandleError);
        }

        private static int RunAlgorithm(CommandLineOptions options) {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            if (!Uri.TryCreate(options.ApiUri, UriKind.Absolute, out var unused)) {
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                Console.WriteLine($"Invalid uri: --uri={options.ApiUri}");
                return 1;
            }

            var apiUri = new Uri(options.ApiUri, UriKind.Absolute);
            using (var algorithmInterface = AlgorithmInterfaceFactory.Create(apiUri)) {
                var algorithm = new SpotAlgorithm(algorithmInterface);
                try {
                    algorithm.Run();
                } catch (Exception e) {
                    algorithmInterface.NotifyUser("Error occured.", e.Message);
                }
            }

            return 0;
        }

        private static int HandleError(IEnumerable<Error> errors) {
            if (errors.Any(
                e =>
                    e.Tag != ErrorType.HelpRequestedError &&
                    e.Tag != ErrorType.HelpVerbRequestedError)) {
                return 1;
            }

            return 0;
        }

        private class CommandLineOptions {
#pragma warning disable S3996 // This is a command line argument, we parse the Uri immediately afterwards.
            [Option('u', "uri", Required = true)]
            public string ApiUri { get; set; }
#pragma warning restore S3996
        }
    }
}
