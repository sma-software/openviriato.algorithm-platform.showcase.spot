using System.Globalization;
using System.Linq;
using NodaTime;
using SMA.AlgorithmPlatform.AlgorithmInterface;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.AlgorithmPlatform.CSharpClient.AIDM;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Solution;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Algorithms.Utils.TimeWindow;
using SMA.Apps.Utils.Collections.Generic.Extensions;
using TimeWindow = SMA.Algorithms.Utils.TimeWindow.TimeWindow;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Services {
    public class TrainPersistenceService {
        private readonly IAlgorithmInterface _algorithmInterface;

        public TrainPersistenceService(IAlgorithmInterface algorithmInterface) {
            _algorithmInterface = algorithmInterface;
        }

        public void WriteToViriato(SpotSolution solution, TimeWindow timeWindowFirstCycle, int numberOfCycles) {
            _algorithmInterface.ShowStatusMessage("Updating Trains", string.Format(CultureInfo.InvariantCulture, "Train Count: {0}", solution.ScheduledTrains.Count));
            foreach (var train in solution.ScheduledTrains) {
                var lastUpdatedTrainId = ProcessAndPersistTrain(timeWindowFirstCycle, train).ID;
                Enumerable
                    .Range(1, numberOfCycles - 1)
                    .Apply(i => {
                        var copiedTrain = _algorithmInterface.CopyTrain(lastUpdatedTrainId);
                        var currentArrivalTime = copiedTrain.TrainPathNodes[0].ArrivalTime.Plus(timeWindowFirstCycle.Duration);
                        var currentDepartureTime = copiedTrain.TrainPathNodes[0].DepartureTime.Plus(timeWindowFirstCycle.Duration);
                        _algorithmInterface.UpdateTrainTrajectoryStopTimes(copiedTrain.ID, CreateUpdateStopTimesTrainPathNode(copiedTrain.TrainPathNodes[0].ID, currentArrivalTime, currentDepartureTime));
                        lastUpdatedTrainId = copiedTrain.ID;
                    });
            }
        }

        private IAlgorithmTrain ProcessAndPersistTrain(TimeWindow timeWindowFirstCycle, ISpotTrain spotTrain) {
            var createdInstanceOfTemplateTrain = _algorithmInterface.CopyTrain(spotTrain.ID);
            var trimmedAlgorithmTrain = CancelBeforeAndAfterIfStopsAreNotOnSpotTrain(spotTrain, createdInstanceOfTemplateTrain);
            var updateTimesTrainPathNodes = spotTrain
                .TrainPathNodes
                .Zip(trimmedAlgorithmTrain.TrainPathNodes)
                .Select(e => CreateUpdateTimesTrainPathNode(e.Item1, e.Item2))
                .ToImmutableList();

            var persistedTrain = _algorithmInterface.UpdateTrainTimes(trimmedAlgorithmTrain.ID, updateTimesTrainPathNodes);

            return ShiftAndPersistTrainToFirstCycle(timeWindowFirstCycle, persistedTrain);
        }

        private IAlgorithmTrain ShiftAndPersistTrainToFirstCycle(TimeWindow timeWindowFirstCycle, IAlgorithmTrain persistedTrain) {
            while (persistedTrain.TrainPathNodes.Last().ArrivalTime > timeWindowFirstCycle.EndTime) {
                var firstTpn = persistedTrain.TrainPathNodes.First();
                var updatedFirstNode = new UpdateStopTimesTrainPathNode(firstTpn.ID, firstTpn.ArrivalTime.Plus(-timeWindowFirstCycle.Duration), firstTpn.DepartureTime.Plus(-timeWindowFirstCycle.Duration), null, null);
                persistedTrain = _algorithmInterface.UpdateTrainTrajectoryStopTimes(persistedTrain.ID, updatedFirstNode);
            }

            return persistedTrain;
        }

        private IAlgorithmTrain CancelBeforeAndAfterIfStopsAreNotOnSpotTrain(ISpotTrain spotTrain, IAlgorithmTrain copiedAlgorithmTrain) {
            var firstNodeOnAlgorithmTrain = copiedAlgorithmTrain.TrainPathNodes[spotTrain.TrainPathNodes.First().SequenceNumber];
            var lastNodeOnAlgorithmTrain = copiedAlgorithmTrain.TrainPathNodes[spotTrain.TrainPathNodes.Last().SequenceNumber];
            _algorithmInterface.CancelTrainBefore(copiedAlgorithmTrain.ID, firstNodeOnAlgorithmTrain.ID);
            return _algorithmInterface.CancelTrainAfter(copiedAlgorithmTrain.ID, lastNodeOnAlgorithmTrain.ID);
        }

        private static IUpdateTimesTrainPathNode CreateUpdateTimesTrainPathNode(ISpotTrainPathNode spotTpn, IAlgorithmTrainPathNode copiedTpn) {
            return new UpdateTimesTrainPathNode(
                copiedTpn.ID,
                spotTpn.ArrivalTime,
                spotTpn.DepartureTime,
                (Duration?)null,
                (Duration?)null,
                spotTpn.StopStatus);
        }

        private static IUpdateStopTimesTrainPathNode CreateUpdateStopTimesTrainPathNode(long id, LocalDateTime arrivalTime, LocalDateTime departureTime) {
            return new UpdateStopTimesTrainPathNode(id, arrivalTime, departureTime, null, null);
        }
    }
}