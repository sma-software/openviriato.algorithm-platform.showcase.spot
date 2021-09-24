using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.Trains;
using SMA.Apps.Utils.Collections.Generic;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations {
    public interface IPassengerTravelRoutePart {
        ISpotLineConstraint SpotLineConstraint { get; }
        ISpotPathNodeConstraint StartLinePathNodeConstraint { get; }
        ISpotPathNodeConstraint EndLinePathNodeConstraint { get; }
        IImmutableList<ISpotPathNodeConstraint> VisitedNodes { get; }
    }

    public class PassengerTravelRoutePart : IPassengerTravelRoutePart {
        public PassengerTravelRoutePart(ISpotLineConstraint spotLineConstraint, IImmutableList<ISpotPathNodeConstraint> visitedNodes) {
            SpotLineConstraint = spotLineConstraint;
            VisitedNodes = visitedNodes;
        }

        public ISpotLineConstraint SpotLineConstraint { get; }
        public ISpotPathNodeConstraint StartLinePathNodeConstraint => VisitedNodes.First();
        public ISpotPathNodeConstraint EndLinePathNodeConstraint => VisitedNodes.Last();
        public IImmutableList<ISpotPathNodeConstraint> VisitedNodes { get; }
    }
}