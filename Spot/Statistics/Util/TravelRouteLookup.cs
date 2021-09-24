using System.Collections.Generic;
using System.Linq;
using SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Model.PassengerOdRelations;
using SMA.Apps.Utils.Collections.Generic;
using SMA.Apps.Utils.Collections.Generic.Extensions;

namespace SMA.AlgorithmPlatform.SmaAlgorithms.Spot.Statistics.Util {
    public class TravelRouteLookup {
        private readonly ISet<IList<long>> _containedRoutes;
        private readonly IList<IPassengerTravelRoute> _routes;

        public TravelRouteLookup() {
            _containedRoutes = new HashSet<IList<long>>(new ListEquals());
            _routes = new List<IPassengerTravelRoute>();
        }

        public void Add(IPassengerTravelRoute route) {
            var hash = HashRoute(route);
            if (!_containedRoutes.Contains(hash)) {
                _containedRoutes.Add(hash);
                _routes.Add(route);
            }
        }

        public IImmutableList<IPassengerTravelRoute> GetAll() {
            return _routes.ToImmutableList();
        }

        private static IList<long> HashRoute(IPassengerTravelRoute route) {
            var routeToVisitedTpns = new List<long>();
            foreach (var routePart in route.TravelRouteParts) {
                routePart.VisitedNodes.Select(tpn => tpn.ID).Apply(routeToVisitedTpns.Add);
            }

            return routeToVisitedTpns;
        }

        private class ListEquals : IEqualityComparer<IList<long>> {
            public bool Equals(IList<long> x, IList<long> y) {
                return Enumerable.SequenceEqual(x, y);
            }

            public int GetHashCode(IList<long> obj) {
                return (int)obj.Aggregate((a, b) => ((a * 17) + (b * 23)) % 6302141);
            }
        }
    }
}