using System;
using System.Linq;
using AlpacaAnalytics.Clustering;

namespace AlpacaAnalytics.Evaluation.Internal
{
    /// <summary>
    ///     Implements an internal evaluation method measuring the root-mean-square standard deviation (RMSSD), i.e., the
    ///     square root of the variance between all elements. This criterion considers only the compactness of the clustering
    ///     partition.
    /// </summary>
    /// <remarks>
    ///     In order to select the optimal partition / <see cref="ClusterSet{TInstance}" /> using this criterion given
    ///     some <see cref="ClusteringResult{TInstance}" /> one has to find the 'knee' in the plot of the criterion value vs.
    ///     the number of clusters.
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class RootMeanSquareStdDev<TInstance> : IInternalEvaluationCriterion<TInstance>
        where TInstance : IComparable<TInstance>

    {
        private readonly CentroidFunction<TInstance> _centroidFunc;


        /// <summary>
        ///     Creates a new <see cref="RootMeanSquareStdDev{TInstance}" /> with given dissimilarity metric.
        /// </summary>
        /// <param name="dissimilarityMetric">The metric used to calculate dissimilarity between cluster elements.</param>
        /// <param name="centroidFunc">
        ///     A function to get an element representing the centroid of a <see cref="Cluster{TInstance}" />.
        /// </param>
        public RootMeanSquareStdDev(
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
            for (var i = 0; i < clusterSet.Count; i++)
            {
                n += clusterSet[i].Count;

                // updates sum of squared distances to centroids
                foreach (var instance in clusterSet[i])
                {
                    var dist = DissimilarityMetric.Calculate(instance, centroids[i]);
                    sum += dist * dist;
                }
            }

            return Math.Sqrt(sum / n);
        }
    }
}