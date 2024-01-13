using System;
using System.Collections.Generic;
using AlpacaAnalytics.Clustering;

namespace AlpacaAnalytics.Evaluation.Internal
{
    /// <summary>
    ///     Implements an internal evaluation method measuring the complement of the ratio of the sum of squared distances
    ///     between elements in different clusters to the total sum of squares. This criterion considers only the separation
    ///     between the clusters given some partition scheme (<see cref="ClusterSet{TInstance}" />).
    /// </summary>
    /// <remarks>
    ///     In order to select the optimal partition / <see cref="ClusterSet{TInstance}" /> using this criterion given
    ///     some <see cref="ClusteringResult{TInstance}" /> one has to find the 'knee' in the plot of the criterion value vs.
    ///     the number of clusters.
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class RSquared<TInstance> : IInternalEvaluationCriterion<TInstance> where TInstance : IComparable<TInstance>

    {
        private readonly CentroidFunction<TInstance> _centroidFunc;


        /// <summary>
        ///     Creates a new <see cref="RSquared{TInstance}" /> with given dissimilarity metric.
        /// </summary>
        /// <param name="dissimilarityMetric">The metric used to calculate dissimilarity between cluster elements.</param>
        /// <param name="centroidFunc">
        ///     A function to get an element representing the centroid of a <see cref="Cluster{TInstance}" />.
        /// </param>
        public RSquared(IDissimilarityMetric<TInstance> dissimilarityMetric,
            CentroidFunction<TInstance> centroidFunc)
        {
            _centroidFunc = centroidFunc;
            DissimilarityMetric = dissimilarityMetric;
        }


        /// <inheritdoc />
        public IDissimilarityMetric<TInstance> DissimilarityMetric { get; }


        /// <inheritdoc />
        public double Evaluate(ClusterSet<TInstance> clusterSet)
        {
            // undefined if only one cluster
            if (clusterSet.Count < 2) return double.NaN;

            // gets clusters' centroids and total cluster
            var centroids = new List<TInstance>();
            Cluster<TInstance> totalCluster = null;
            foreach (var cluster in clusterSet)
            {
                totalCluster = totalCluster == null
                    ? new Cluster<TInstance>(cluster)
                    : new Cluster<TInstance>(totalCluster, cluster, 0);
                centroids.Add(_centroidFunc(cluster));
            }

            var centroid = _centroidFunc(totalCluster);

            var sumDistTotal = 0d;
            var sumDistWithin = 0d;
            for (var i = 0; i < clusterSet.Count; i++)
                // updates sum of squared distances to centroids
                foreach (var instance in clusterSet[i])
                {
                    var distWithin = DissimilarityMetric.Calculate(instance, centroids[i]);
                    sumDistWithin += distWithin * distWithin;

                    var distTotal = DissimilarityMetric.Calculate(instance, centroid);
                    sumDistTotal += distTotal * distTotal;
                }

            return (sumDistTotal - sumDistWithin) / sumDistTotal;
        }
    }
}