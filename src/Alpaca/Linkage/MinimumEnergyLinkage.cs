﻿using System;
using System.Linq;

namespace Alpaca.Linkage
{
    /// <summary>
    ///     Implements the minimum (energy) E-distance method that minimizes a joint between-within measure of distance between
    ///     clusters.
    /// </summary>
    /// <remarks>
    ///     "This method extends <see cref="WardsMinimumVarianceLinkage{TInstance}" />, by defining a cluster distance and
    ///     objective function in terms of Euclidean distance, or any power of Euclidean distance in the interval (0, 2].
    ///     Ward's method is obtained as the special case when the power is 2. The ability of the proposed extension to
    ///     identify clusters with nearly equal centers is an important advantage over geometric or cluster center methods"
    ///     [1].
    ///     References:
    ///     [1] -
    ///     <see href="https://link.springer.com/article/10.1007/s00357-005-0012-9">
    ///         Szekely, G. J., &amp; Rizzo, M. L. (2005). Hierarchical clustering via joint between-within distances:
    ///         Extending Ward's minimum variance method. Journal of classification, 22(2), 151-183
    ///     </see>
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class MinimumEnergyLinkage<TInstance> : ILinkageCriterion<TInstance> where TInstance : IComparable<TInstance>
    {
        /// <summary>
        ///     Creates a new <see cref="MinimumEnergyLinkage{TInstance}" /> with given dissimilarity metric.
        /// </summary>
        /// <param name="metric">The metric used to calculate dissimilarity between cluster elements.</param>
        public MinimumEnergyLinkage(IDissimilarityMetric<TInstance> metric)
        {
            DissimilarityMetric = metric;
        }


        /// <summary>
        ///     Gets or sets the distance exponent in the interval (0, 2].
        /// </summary>
        public double DistanceExponent { get; set; } = 2;

        /// <inheritdoc />
        public IDissimilarityMetric<TInstance> DissimilarityMetric { get; }


        /// <inheritdoc />
        public double Calculate(Cluster<TInstance> cluster1, Cluster<TInstance> cluster2)
        {
            var c1c2Energy = GetEnergy(cluster1, cluster2);
            var c1c1Energy = GetEnergy(cluster1, cluster1);
            var c2c2Energy = GetEnergy(cluster2, cluster2);
            return (double)(cluster1.Count * cluster2.Count) / (cluster1.Count + cluster2.Count) *
                   (2 * c1c2Energy - c1c1Energy - c2c2Energy);
        }


        private double GetEnergy(Cluster<TInstance> cluster1, Cluster<TInstance> cluster2)
        {
            var sum = cluster1.Sum(
                instance1 => cluster2.Sum(instance2 => DissimilarityMetric.Calculate(instance1, instance2)));
            var dist = Math.Pow(sum, DistanceExponent);
            return dist / (cluster1.Count * cluster2.Count);
        }
    }
}