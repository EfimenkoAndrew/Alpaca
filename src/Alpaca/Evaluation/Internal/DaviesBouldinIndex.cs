using System;
using System.Collections.Generic;
using System.Linq;

namespace Alpaca.Evaluation.Internal
{
    /// <summary>
    ///     Implements the internal evaluation method in [1] that measures the "ratio of the within cluster scatter to the
    ///     between cluster separation" [2].
    /// </summary>
    /// <remarks>
    ///     "It happens to be the average similarity between each cluster and its most similar one, averaged over all the
    ///     clusters[...]. This affirms the idea that no cluster has to be similar to another, and hence the best clustering
    ///     scheme essentially minimizes the Davies–Bouldin (BD) index" [2].
    ///     Notes:
    ///     - This implementation corresponds to - BD so that a higher index provides a better partitioning.
    ///     - This implementation returns <c>double.Nan</c> if the partition contains singleton clusters (undefined
    ///     dispersion).
    ///     References:
    ///     [1] -
    ///     <see href="https://doi.org/10.1109/TPAMI.1979.4766909">
    ///         Davies, D. L., &amp; Bouldin, D. W. (1979). A cluster separation measure. IEEE transactions on pattern analysis
    ///         and machine intelligence, (2), 224-227.
    ///     </see>
    ///     [2] - <see href="https://en.wikipedia.org/wiki/Davies%E2%80%93Bouldin_index" />
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class DaviesBouldinIndex<TInstance> : IInternalEvaluationCriterion<TInstance>
        where TInstance : IComparable<TInstance>

    {
        private readonly CentroidFunction<TInstance> _centroidFunc;


        /// <summary>
        ///     Creates a new <see cref="DaviesBouldinIndex{TInstance}" /> with given dissimilarity metric.
        /// </summary>
        /// <param name="dissimilarityMetric">The metric used to calculate dissimilarity between cluster elements.</param>
        /// <param name="centroidFunc">
        ///     A function to get an element representing the centroid of a <see cref="Cluster{TInstance}" />.
        /// </param>
        public DaviesBouldinIndex(
            IDissimilarityMetric<TInstance> dissimilarityMetric, CentroidFunction<TInstance> centroidFunc)
        {
            _centroidFunc = centroidFunc;
            DissimilarityMetric = dissimilarityMetric;
        }


        /// <summary>
        ///     Gets or sets the distance exponent.
        /// </summary>
        public double DistanceExponent { get; set; } = 2;

        /// <inheritdoc />
        public IDissimilarityMetric<TInstance> DissimilarityMetric { get; }


        /// <inheritdoc />
        public double Evaluate(ClusterSet<TInstance> clusterSet)
        {
            // undefined if only one cluster
            if (clusterSet.Count < 2) return double.NaN;

            var centroids = new List<TInstance>();
            var dispersions = new List<double>();
            for (var i = 0; i < clusterSet.Count; i++)
            {
                var centroid = _centroidFunc(clusterSet[i]);
                centroids.Add(centroid);
                dispersions.Add(CalcDispersion(clusterSet[i].ToList(), centroid));
            }

            var sum = 0d;
            for (var i = 0; i < clusterSet.Count; i++)
            {
                // gets max compactness of clusters compared to the distance between the cluster centroids
                var maxDisp = double.MinValue;
                for (var j = 0; j < clusterSet.Count; j++)
                    if (j != i)
                        maxDisp = Math.Max(maxDisp,
                            (dispersions[i] + dispersions[j]) /
                            DissimilarityMetric.Calculate(centroids[i], centroids[j]));

                sum += maxDisp;
            }

            return -sum / clusterSet.Count;
        }


        private double CalcDispersion(List<TInstance> cluster, TInstance centroid)
        {
            return cluster.Count < 2
                ? double.NaN
                : cluster.Sum(instance => DissimilarityMetric.Calculate(instance, centroid)) / cluster.Count;
        }
    }
}