using System;

namespace Alpaca.Linkage
{
    /// <summary>
    ///     Implements the centroid linkage clustering method, i.e., returns the distance between the centroid for each cluster
    ///     (a mean vector).
    /// </summary>
    /// <remarks>
    ///     Centroid linkage is equivalent to <see cref="AverageLinkage{TInstance}" /> of all pairs of documents from
    ///     different clusters. Thus, the difference between average and centroid clustering is that the former considers all
    ///     pairs of documents in computing average pairwise similarity, whereas centroid clustering excludes pairs from the
    ///     same cluster [1].
    ///     References:
    ///     [1] - <see href="https://nlp.stanford.edu/IR-book/html/htmledition/centroid-clustering-1.html" />.
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class CentroidLinkage<TInstance> : ILinkageCriterion<TInstance> where TInstance : IComparable<TInstance>

    {
        private readonly Func<Cluster<TInstance>, TInstance> _centroidFunc;


        /// <summary>
        ///     Creates a new <see cref="CentroidLinkage{TInstance}" /> with given dissimilarity metric and centroid
        ///     function.
        /// </summary>
        /// <param name="metric">The metric used to calculate dissimilarity between cluster elements.</param>
        /// <param name="centroidFunc">
        ///     A function to get an element representing the centroid of a <see cref="Cluster{TInstance}" />.
        /// </param>
        public CentroidLinkage(
            IDissimilarityMetric<TInstance> metric, Func<Cluster<TInstance>, TInstance> centroidFunc)
        {
            DissimilarityMetric = metric;
            _centroidFunc = centroidFunc;
        }


        /// <inheritdoc />
        public IDissimilarityMetric<TInstance> DissimilarityMetric { get; }


        /// <inheritdoc />
        public double Calculate(Cluster<TInstance> cluster1, Cluster<TInstance> cluster2)
        {
            return DissimilarityMetric.Calculate(_centroidFunc(cluster1), _centroidFunc(cluster2));
        }
    }
}