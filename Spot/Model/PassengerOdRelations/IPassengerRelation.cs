using System.Linq;
using SMA.AlgorithmPlatform.AlgorithmInterface.AIDM;
using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations {
    public interface IPassengerRelation {
        IAlgorithmNode OriginNode { get; }
        IAlgorithmNode DestinationNode { get; }
        IImmutableList<IPassengerTravelRoute> TravelRoutes { get; }
        int TotalDemand { get; }
        bool HasRoutes { get; }
    }

    public class PassengerRelation : IPassengerRelation {
        public PassengerRelation(IAlgorithmNode originNode, IAlgorithmNode destinationNode, IImmutableList<IPassengerTravelRoute> travelRoutes, int totalDemand) {
            OriginNode = originNode;
            DestinationNode = destinationNode;
            TravelRoutes = travelRoutes;
            TotalDemand = totalDemand;
        }

        public IAlgorithmNode OriginNode { get; }
        public IAlgorithmNode DestinationNode { get; }
        public IImmutableList<IPassengerTravelRoute> TravelRoutes { get; }
        public int TotalDemand { get; }
        public bool HasRoutes => TravelRoutes.Any();
    }
}