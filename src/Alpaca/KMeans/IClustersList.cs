using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Alpaca.KMeans
{
    public class ClustersList : ICloneable, IEnumerable<Cluster>
    {
        private readonly List<Cluster> _mClustersList;

        public ClustersList()
        {
            if (_mClustersList == null)
                _mClustersList = new List<Cluster>();
        }

        public Cluster this[int iIndex] => _mClustersList[iIndex];

        public object Clone()
        {
            var targetClustersList = new ClustersList();
            foreach (var cluster in _mClustersList)
            {
                var targetCentroidsList = new ItemsList();
                foreach (var centroid in (IItemsList)cluster.Centroids.Clone())
                    targetCentroidsList.Add(new Item(centroid.ItemText, (AttribList)centroid.AttribList.Clone(),
                        centroid.Distance, centroid.IsUser, centroid.Exists));

                var targetItemsList = new ItemsList();
                foreach (var item in (IItemsList)cluster.Items.Clone())
                    targetItemsList.Add(new Item(item.ItemText, (AttribList)item.AttribList.Clone(),
                        item.Distance, item.IsUser, item.Exists));

                targetClustersList.Add(new Cluster((IItemsList)targetCentroidsList.Clone(),
                    (IItemsList)targetItemsList.Clone()));
            }

            return targetClustersList;
        }

        public IEnumerator GetEnumerator()
        {
            return _mClustersList.GetEnumerator();
        }

        IEnumerator<Cluster> IEnumerable<Cluster>.GetEnumerator()
        {
            return (IEnumerator<Cluster>)GetEnumerator();
        }

        public void Add(Cluster cluster)
        {
            _mClustersList.Add(cluster);
        }

        public int Count()
        {
            return _mClustersList.Count();
        }
    }
}