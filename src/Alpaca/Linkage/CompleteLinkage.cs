using System;
using System.Linq;

namespace Alpaca.Linkage
{
    /// <summary>
    ///     Implements the maximum or complete-linkage clustering method, i.e., returning the maximum value of all pairwise
    ///     distances between the elements in each cluster. The method is also known as farthest neighbor clustering.
    /// </summary>
    /// <remarks>
    ///     Complete linkage clustering avoids a drawback of <see cref="SingleLinkage{TInstance}" /> - the
    ///     so-called chaining phenomenon, where clusters formed via single linkage clustering may be forced together due to
    ///     single elements being close to each other, even though many of the elements in each cluster may be very distant to
    ///     each other. Complete linkage tends to find compact clusters of approximately equal diameter (
    ///     <see href="https://en.wikipedia.org/wiki/Complete-linkage_clustering" />).
    ///     However, complete-link clustering suffers from a different problem. It pays too much attention to outliers, points
    ///     that do not fit well into the global structure of the cluster (
    ///     <see href="https://nlp.stanford.edu/IR-book/html/htmledition/single-link-and-complete-link-clustering-1.html" />).
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class CompleteLinkage<TInstance> : ILinkageCriterion<TInstance> where TInstance : IComparable<TInstance>

    {
        /// <summary>
        ///     Creates a new <see cref="CompleteLinkage{TInstance}" /> with given dissimilarity metric.
        /// </summary>
        /// <param name="metric">The metric used to calculate dissimilarity between cluster elements.</param>
        public CompleteLinkage(IDissimilarityMetric<TInstance> metric)
        {
            DissimilarityMetric = metric;
        }


        /// <inheritdoc />
        public IDissimilarityMetric<TInstance> DissimilarityMetric { get; }


        /// <inheritdoc />
        public double Calculate(Cluster<TInstance> cluster1, Cluster<TInstance> cluster2)
        {
            return cluster1.Max(
                instance1 => cluster2.Max(instance2 => DissimilarityMetric.Calculate(instance1, instance2)));
        }
    }
}