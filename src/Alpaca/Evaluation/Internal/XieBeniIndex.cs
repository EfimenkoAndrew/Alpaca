using System;
using System.Linq;
using AlpacaAnalytics.Clustering;

namespace AlpacaAnalytics.Evaluation.Internal
{
    /// <summary>
    ///     Implements the internal evaluation method in [1] known as the Xie-Beni (XB) index. It defines the inter-cluster
    ///     separation as the minimum square distance between cluster centers, and the intra-cluster compactness as the mean
    ///     square distance between each data object and its cluster center.
    /// </summary>
    /// <remarks>
    ///     Notes:
    ///     - The formulation in [1] has a form of (Compactness) / (Separation) and therefore reaches the optimum clustering by
    ///     being minimized. This implementation thus corresponds to - XB.
    ///     References:
    ///     [1] -
    ///     <see href="https://doi.org/10.1109/34.85677">
    ///         Xie, X. L., &amp; Beni, G. (1991). A validity measure for fuzzy clustering. IEEE Transactions on pattern
    ///         analysis
    ///         and machine intelligence, 13(8), 841-847.
    ///     </see>
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class XieBeniIndex<TInstance> : IInternalEvaluationCriterion<TInstance>
        where TInstance : IComparable<TInstance>

    {
        private readonly CentroidFunction<TInstance> _centroidFunc;


        /// <summary>
        ///     Creates a new <see cref="XieBeniIndex{TInstance}" /> with given dissimilarity metric.
        /// </summary>
        /// <param name="dissimilarityMetric">The metric used to calculate dissimilarity between cluster elements.</param>
        /// <param name="centroidFunc">
        ///     A function to get an element representing the centroid of a <see cref="Cluster{TInstance}" />.
        /// </param>
        public XieBeniIndex(
            IDissimilarityMetric<TInstance> dissimilarityMetric, CentroidFunction<TInstance> centroidFunc)
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

            // gets clusters' centroids 
            var centroids = clusterSet.Select(t => _centroidFunc(t)).ToList();

            var n = 0;
            var sum = 0d;
            var minCentDist = double.MaxValue;
            for (var i = 0; i < clusterSet.Count; i++)
            {
                n += clusterSet[i].Count;

                // updates sum of distances to centroids
                foreach (var instance in clusterSet[i])
                {
                    var dist = DissimilarityMetric.Calculate(instance, centroids[i]);
                    sum += dist * dist;
                }

                // updates min between-cluster distance
                for (var j = i + 1; j < clusterSet.Count; j++)
                {
                    var betweenClusterDist = DissimilarityMetric.Calculate(centroids[i], centroids[j]);
                    minCentDist = Math.Min(minCentDist, betweenClusterDist * betweenClusterDist);
                }
            }

            return -sum / (n * minCentDist);
        }
    }
}