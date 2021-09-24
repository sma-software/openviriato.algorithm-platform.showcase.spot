using System.Globalization;
using System.Linq;
using SMA.AlgorithmPlatform.AlgorithmInterface;
using SMA.AlgorithmPlatform.SmaAlgorithms.RailwayUtils.Infrastructure.Caches;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Extensions;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.UserParameters;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.UserParameters.Parser;
using SMA.Apps.Utils.Answers;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Scenario {
    public class SpotScenarioFactory {
        private readonly IAlgorithmInterface _algorithmInterface;
        private readonly INodeLookup _nodeLookup;

        public SpotScenarioFactory(IAlgorithmInterface algorithmInterface, INodeLookup nodeLookup) {
            _algorithmInterface = algorithmInterface;
            _nodeLookup = nodeLookup;
        }

        public Result<ISpotScenario> Create(ISpotUserParameters userParameters, ITransferTimeLookup transferTimeLookup) {
            var spotLines = userParameters
                .AlgorithmTrainsForSpotLineConstraints
                .GroupBy(t => t.Code)
                .Select(t => t.First().ToSpotLine())
                .ToImmutableList();

            var validationAnswers = ValidateLinesAgreeableWithParams(userParameters, spotLines);
            if (!validationAnswers.Allowed) {
                return validationAnswers.ToFailedResult();
            }

            _algorithmInterface.ShowStatusMessage("Parsing travel routes");
            var passengerRelationsResult = new PassengerRelationsParser(
                    _algorithmInterface,
                    spotLines,
                    _nodeLookup,
                    userParameters.MaximalNumberOfTransfers)
                .ReadRelationsFromFile(userParameters.FilePathToRoutesAsCsvFile);
            if (!passengerRelationsResult.Success) {
                return passengerRelationsResult.Answers.ToFailedResult();
            }

            return new SpotScenario(
                userParameters.CycleTimeWindow,
                userParameters.NumberOfCycles,
                spotLines,
                passengerRelationsResult.Value,
                transferTimeLookup,
                userParameters.AdditionalRunTimeFactor);
        }

        private static IAnswers ValidateLinesAgreeableWithParams(ISpotUserParameters userParameters, ImmutableList<ISpotLineConstraint> spotLines) {
            foreach (ISpotLineConstraint spotLineConstraint in spotLines) {
                foreach (var spotPathNodeConstraint in spotLineConstraint.PathNodes.Skip(1)) {
                    if (spotPathNodeConstraint.MinRunTime.Value > userParameters.CycleTimeWindow.Duration) {
                        return new Answers(string.Format(CultureInfo.InvariantCulture, "Can not Start scenario, as MinRunTime {0} on TrainPathNode {1} exceeds the cycle duration of {2}", spotPathNodeConstraint.MinRunTime, spotPathNodeConstraint.ID, userParameters.CycleTimeWindow.Duration));
                    }
                }
            }

            return new Answers();
        }
    }
}