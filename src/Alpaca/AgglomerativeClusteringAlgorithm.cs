using System;
using System.Collections.Generic;
using System.Linq;
using Alpaca.Linkage;

namespace Alpaca
{
    /// <summary>
    ///     Implements the agglomerative nesting clustering algorithm (program AGNES) in [1].
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    /// <remarks>
    ///     [1] Kaufman, L., &amp; Rousseeuw, P. J. (1990). Agglomerative nesting (program AGNES). Finding Groups in Data: An
    ///     Introduction to Cluster Analysis, 199-252.
    /// </remarks>
    public class AgglomerativeClusteringAlgorithm<TInstance> : IClusteringAlgorithm<TInstance>
        where TInstance : IComparable<TInstance>
    {
        private Cluster<TInstance>[] _clusters;
        private int _curClusterCount;
        private double[][] _dissimilarities;


        /// <summary>
        ///     Creates a new instance of <see cref="AgglomerativeClusteringAlgorithm{TInstance}" /> with the given set of
        ///     instances and linkage
        ///     criterion.
        /// </summary>
        /// <param name="linkageCriterion">The criterion used to measure dissimilarities within and between clusters.</param>
        public AgglomerativeClusteringAlgorithm(ILinkageCriterion<TInstance> linkageCriterion)
        {
            LinkageCriterion = linkageCriterion;
        }


        /// <inheritdoc />
        public ILinkageCriterion<TInstance> LinkageCriterion { get; }


        /// <inheritdoc />
        public ClusteringResult<TInstance> GetClustering(ISet<TInstance> instances)
        {
            // initial setting: every instance in its own cluster
            var currentClusters = instances.Select(instance => new Cluster<TInstance>(instance));

            // executes clustering algorithm
            var clustering = GetClustering(currentClusters);

            return clustering;
        }

        /// <inheritdoc />
        public ClusteringResult<TInstance> GetClustering(ClusterSet<TInstance> clusterSet)
        {
            return GetClustering(clusterSet, clusterSet.Dissimilarity);
        }

        /// <inheritdoc />
        public ClusteringResult<TInstance> GetClustering(IEnumerable<Cluster<TInstance>> clusters,
            double dissimilarity = 0d)
        {
            // initializes elements
            var currentClusters = clusters.ToArray();

            if (currentClusters.Length == 0) return new ClusteringResult<TInstance>(0);

            _clusters = new Cluster<TInstance>[currentClusters.Length * 2 - 1];
            _dissimilarities = new double[currentClusters.Length * 2 - 1][];
            _curClusterCount = 0;

            // calculates initial dissimilarities
            foreach (var cluster in currentClusters)
            {
                _clusters[_curClusterCount] = cluster;
                UpdateDissimilarities();
                _curClusterCount++;
            }

            var clustering = new ClusteringResult<TInstance>(currentClusters.Length)
            {
                [0] = new ClusterSet<TInstance>(currentClusters, dissimilarity)
            };
            var numSteps = currentClusters.Length;
            for (var i = 1; i < numSteps; i++)
            {
                // gets minimal dissimilarity between a pair of existing clusters
                var minDissimilarity = GetMinDissimilarity(out var clusterIdx1, out var clusterIdx2);

                // gets a copy of previous clusters, removes new cluster elements
                var cluster1 = _clusters[clusterIdx1];
                var cluster2 = _clusters[clusterIdx2];
                var newClusters = new Cluster<TInstance>[currentClusters.Length - 1];
                var idx = 0;
                foreach (var cluster in currentClusters)
                    if (!cluster.Equals(cluster1) && !cluster.Equals(cluster2))
                        newClusters[idx++] = cluster;
                _clusters[clusterIdx1] = null;
                _clusters[clusterIdx2] = null;

                // creates a new cluster from the union of closest clusters (save reference to parents)
                var newCluster = new Cluster<TInstance>(cluster1, cluster2, minDissimilarity);

                // adds cluster to list and calculates distance to all others
                newClusters[idx] = _clusters[_curClusterCount] = newCluster;
                UpdateDissimilarities();
                _curClusterCount++;

                // updates global list of clusters
                currentClusters = newClusters;
                clustering[i] = new ClusterSet<TInstance>(currentClusters, minDissimilarity);
            }

            _clusters = null;
            _dissimilarities = null;
            return clustering;
        }


        private double GetMinDissimilarity(out int clusterIdx1, out int clusterIdx2)
        {
            clusterIdx1 = clusterIdx2 = 0;

            // for each pair of clusters that still exist
            var minDissimilarity = double.MaxValue;
            for (var i = _curClusterCount - 1; i > 0; i--)
            {
                if (_clusters[i] == null) continue;
                for (var j = 0; j < i; j++)
                {
                    if (_clusters[j] == null) continue;

                    // check dissimilarity and register indexes if minimal
                    var dissimilarity = _dissimilarities[i][j];
                    if (minDissimilarity <= dissimilarity) continue;
                    minDissimilarity = dissimilarity;
                    clusterIdx1 = i;
                    clusterIdx2 = j;
                }
            }

            return minDissimilarity;
        }

        private void UpdateDissimilarities()
        {
            // update the dissimilarities / distances to all still existing clusters
            _dissimilarities[_curClusterCount] = new double[_curClusterCount];
            for (var j = 0; j < _curClusterCount; j++)
                if (_clusters[j] != null)
                    _dissimilarities[_curClusterCount][j] =
                        LinkageCriterion.Calculate(_clusters[j], _clusters[_curClusterCount]);
        }
    }
}