using System;
using System.Linq;

namespace Alpaca.Linkage
{
    /// <summary>
    ///     Implements the unweighted pair-group average method or UPGMA, i.e., returns the mean distance between the elements
    ///     in each cluster.
    /// </summary>
    /// <remarks>
    ///     Average linkage tries to strike a balance between <see cref="SingleLinkage{TInstance}" /> and
    ///     <see cref="CompleteLinkage{TInstance}" />. It uses average pairwise dissimilarity, so clusters tend to be
    ///     relatively compact and relatively far apart. However, it is not clear what properties the resulting clusters have
    ///     when we cut an average linkage tree at given distance. Single and complete linkage trees each had simple
    ///     interpretations [1].
    ///     References:
    ///     [1] - <see href="http://www.stat.cmu.edu/~ryantibs/datamining/lectures/05-clus2-marked.pdf" />.
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class AverageLinkage<TInstance> : ILinkageCriterion<TInstance> where TInstance : IComparable<TInstance>

    {
        /// <summary>
        ///     Creates a new <see cref="AverageLinkage{TInstance}" /> with given dissimilarity metric.
        /// </summary>
        /// <param name="metric">The metric used to calculate dissimilarity between cluster elements.</param>
        public AverageLinkage(IDissimilarityMetric<TInstance> metric)
        {
            DissimilarityMetric = metric;
        }


        /// <inheritdoc />
        public IDissimilarityMetric<TInstance> DissimilarityMetric { get; }


        /// <inheritdoc />
        public double Calculate(Cluster<TInstance> cluster1, Cluster<TInstance> cluster2)
        {
            var sum = cluster1.Sum(
                instance1 => cluster2.Sum(instance2 => DissimilarityMetric.Calculate(instance1, instance2)));
            return sum / (cluster1.Count * cluster2.Count);
        }
    }
}