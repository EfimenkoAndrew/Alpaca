﻿using System;
using AlpacaAnalytics.Clustering;

namespace AlpacaAnalytics.Linkage
{
    /// <summary>
    ///     Implements Ward's minimum variance method, i.e., returns the total within-cluster variance, corresponding to a
    ///     weighted squared distance between cluster centers.
    /// </summary>
    /// <remarks>
    ///     "With hierarchical clustering, the sum of squares starts out at zero (because every point is in its own cluster)
    ///     and then grows as we merge clusters. Ward's method keeps this growth as small as possible. This is nice if you
    ///     believe that the sum of squares should be small. Notice that the number of points shows up in [the formula], as
    ///     well as their geometric separation. Given two pairs of clusters whose centers are equally far apart, Ward's method
    ///     will prefer to merge the smaller ones." [1]
    ///     References:
    ///     [1] - <see href="http://www.stat.cmu.edu/~cshalizi/350/lectures/08/lecture-08.pdf" />.
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class WardsMinimumVarianceLinkage<TInstance> : ILinkageCriterion<TInstance>
        where TInstance : IComparable<TInstance>
    {
        private readonly CentroidFunction<TInstance> _centroidFunc;


        /// <summary>
        ///     Creates a new <see cref="WardsMinimumVarianceLinkage{TInstance}" /> with given dissimilarity metric and centroid
        ///     function.
        /// </summary>
        /// <param name="metric">The metric used to calculate dissimilarity between cluster elements.</param>
        /// <param name="centroidFunc">
        ///     A function to get an element representing the centroid of a <see cref="Cluster{TInstance}" />.
        /// </param>
        public WardsMinimumVarianceLinkage(
            IDissimilarityMetric<TInstance> metric, CentroidFunction<TInstance> centroidFunc)
        {
            DissimilarityMetric = metric;
            _centroidFunc = centroidFunc;
        }


        /// <inheritdoc />
        public IDissimilarityMetric<TInstance> DissimilarityMetric { get; }


        /// <inheritdoc />
        public double Calculate(Cluster<TInstance> cluster1, Cluster<TInstance> cluster2)
        {
            var centroidDist =
                DissimilarityMetric.Calculate(_centroidFunc(cluster1), _centroidFunc(cluster2));
            return centroidDist * centroidDist *
                   (cluster1.Count * cluster2.Count / ((double)cluster1.Count + cluster2.Count));
        }
    }
}