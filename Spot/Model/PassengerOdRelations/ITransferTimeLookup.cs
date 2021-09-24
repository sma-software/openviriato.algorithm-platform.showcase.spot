using NodaTime;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations {
    public interface ITransferTimeLookup {
        Duration GetRequiredMinimumTransferDuration(ISpotLineConstraint arrivingLineConstraint, ISpotPathNodeConstraint arrivingNodeConstraint, ISpotLineConstraint departureLineConstraint, ISpotPathNodeConstraint departureNodeConstraint);
    }

    public class TransferTimesLookup : ITransferTimeLookup {
        private readonly Duration _defaultMinimumTransferTime;

        public TransferTimesLookup(Duration defaultMinimumTransferTime) {
            _defaultMinimumTransferTime = defaultMinimumTransferTime;
        }

        public Duration GetRequiredMinimumTransferDuration(ISpotLineConstraint arrivingLineConstraint, ISpotPathNodeConstraint arrivingNodeConstraint, ISpotLineConstraint departureLineConstraint, ISpotPathNodeConstraint departureNodeConstraint) {
            return _defaultMinimumTransferTime;
        }
    }
}