using System;

namespace KMeans
{
    public class Cluster : ICloneable
    {
        private IItemsList _mCentroids;
        private IItemsList _mItems;

        public Cluster(IItemsList cntsList, IItemsList itemsList)
        {
            Items = itemsList;
            Centroids = cntsList;
        }

        public IItemsList Items
        {
            get => _mItems;
            set => _mItems = value;
        }

        public IItemsList Centroids
        {
            get => _mCentroids;
            set => _mCentroids = value;
        }

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