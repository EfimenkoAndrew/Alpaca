using System;

namespace Alpaca.KMeans
{
    public class Cluster : ICloneable
    {
        public Cluster(IItemsList cntsList, IItemsList itemsList)
        {
            Items = itemsList;
            Centroids = cntsList;
        }

        public IItemsList Items { get; set; }

        public IItemsList Centroids { get; set; }

        public object Clone()
        {
            var targetItemsList = new ItemsList();
            var targetCentroidsList = new ItemsList();
            var targetCluster = (Cluster)MemberwiseClone();

            foreach (var centroid in Centroids)
                targetCentroidsList.Add(centroid);

            foreach (var item in Items)
                targetItemsList.Add(item);

            targetCluster.Items = targetItemsList;
            targetCluster.Centroids = targetCentroidsList;

            return targetCluster;
        }
    }
}