using System;
using System.Linq;
using AlpacaAnalytics.Clustering;

namespace AlpacaAnalytics.Evaluation.Internal
{
    /// <summary>
    ///     Implements the internal evaluation method in [1] that measures the ratio between the smallest distance between
    ///     observations not in the same cluster to the largest intra-cluster distance. The Dunn Index has a value between zero
    ///     and infinity, and a higher index indicates a better clustering.
    ///     The aim is to identify sets of clusters that are compact, with a small variance between members of the cluster, and
    ///     well separated, where the means of different clusters are sufficiently far apart, as compared to the within cluster
    ///     variance [2].
    /// </summary>
    /// <remarks>
    ///     Notes:
    ///     - This formulation has a peculiar problem, in that if one of the clusters is badly behaved, where the others are
    ///     tightly packed, since the denominator contains a 'max' term instead of an average term, the Dunn Index for that set
    ///     of clusters will be uncharacteristically low [2].
    ///     References:
    ///     [1] -
    ///     <see href="https://doi.org/10.1080/01969727308546046">
    ///         Dunn, J. C. (1973). A fuzzy relative of the ISODATA process and its use in detecting compact well-separated
    ///         clusters.
    ///     </see>
    ///     [2] - <see href="https://en.wikipedia.org/wiki/Dunn_index" />
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class DunnIndex<TInstance> : IInternalEvaluationCriterion<TInstance> where TInstance : IComparable<TInstance>

    {
        /// <summary>
        ///     Creates a new <see cref="DunnIndex{TInstance}" /> with given dissimilarity metric.
        /// </summary>
        /// <param name="dissimilarityMetric">The metric used to calculate dissimilarity between cluster elements.</param>
        public DunnIndex(IDissimilarityMetric<TInstance> dissimilarityMetric)
        {
            DissimilarityMetric = dissimilarityMetric;
        }


        /// <inheritdoc />
        public IDissimilarityMetric<TInstance> DissimilarityMetric { get; }


        /// <inheritdoc />
        public double Evaluate(ClusterSet<TInstance> clusterSet)
        {
            // undefined if only one cluster
            if (clusterSet.Count < 2) return double.NaN;

            var minInterClusterSep = double.MaxValue;
            var maxIntraClusterSep = double.MinValue;
            for (var i = 0; i < clusterSet.Count; i++)
            {
                var cluster = clusterSet[i].ToList();

                for (var j = 0; j < cluster.Count; j++)
                {
                    var instance = cluster[j];

                    // gets dissimilarity with all other elems within the same cluster (get max intra distance)
                    for (var k = j + 1; k < cluster.Count; k++)
                        maxIntraClusterSep = Math.Max(maxIntraClusterSep,
                            DissimilarityMetric.Calculate(instance, cluster[k]));

                    // gets dissimilarity to other clusters' elems (gets min inter cluster)
                    for (var l = i + 1; l < clusterSet.Count; l++)
                        foreach (var other in clusterSet[l])
                            minInterClusterSep = Math.Min(minInterClusterSep,
                                DissimilarityMetric.Calculate(instance, other));
                }
            }

            return minInterClusterSep / maxIntraClusterSep;
        }
    }
}