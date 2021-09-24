using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Solution {
    public class SpotSolution {
        public static SpotSolution CreateEmpty() {
            return new SpotSolution(ImmutableList<ISpotTrain>.Empty, ImmutableList<IPassengerRelationCharacteristics>.Empty);
        }

        public SpotSolution(IImmutableList<ISpotTrain> scheduledTrains, IImmutableList<IPassengerRelationCharacteristics> passengerRelationCharacteristics) {
            ScheduledTrains = scheduledTrains;
            PassengerRelationCharacteristics = passengerRelationCharacteristics;
        }

        public IImmutableList<ISpotTrain> ScheduledTrains { get; }
        public IImmutableList<IPassengerRelationCharacteristics> PassengerRelationCharacteristics { get; }
    }
}