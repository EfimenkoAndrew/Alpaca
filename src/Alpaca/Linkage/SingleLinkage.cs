using System;
using System.Linq;
using AlpacaAnalytics.Clustering;

namespace AlpacaAnalytics.Linkage
{
    /// <summary>
    ///     Implements the minimum or single-linkage clustering method, i.e., returns the minimum value of all pairwise
    ///     distances between the elements in each cluster. The method is also known as nearest neighbor clustering.
    /// </summary>
    /// <remarks>
    ///     A drawback of this method is that it tends to produce long thin clusters in which nearby elements of the same
    ///     cluster have small distances, but elements at opposite ends of a cluster may be much farther from each other than
    ///     two elements of other clusters [1].
    ///     Since the merge criterion is strictly local, a chain of points can be extended for long distances without regard to
    ///     the overall shape of the emerging cluster. This effect is called chaining [2].
    ///     References:
    ///     [1] - <see href="https://en.wikipedia.org/wiki/Single-linkage_clustering" />.
    ///     [2] -
    ///     <see href="https://nlp.stanford.edu/IR-book/html/htmledition/single-link-and-complete-link-clustering-1.html" />
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class SingleLinkage<TInstance> : ILinkageCriterion<TInstance> where TInstance : IComparable<TInstance>

    {
        /// <summary>
        ///     Creates a new <see cref="SingleLinkage{TInstance}" /> with given dissimilarity metric.
        /// </summary>
        /// <param name="metric">The metric used to calculate dissimilarity between cluster elements.</param>
        public SingleLinkage(IDissimilarityMetric<TInstance> metric)
        {
            DissimilarityMetric = metric;
        }


        /// <inheritdoc />
        public IDissimilarityMetric<TInstance> DissimilarityMetric { get; }


        /// <inheritdoc />
        public double Calculate(Cluster<TInstance> cluster1, Cluster<TInstance> cluster2)
        {
            return cluster1.Min(
                instance1 => cluster2.Min(instance2 => DissimilarityMetric.Calculate(instance1, instance2)));
        }
    }
}