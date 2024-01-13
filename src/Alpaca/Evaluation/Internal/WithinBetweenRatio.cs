﻿using System;
using System.Collections.Generic;
using AlpacaAnalytics.Clustering;

namespace AlpacaAnalytics.Evaluation.Internal
{
    /// <summary>
    ///     Implements the within-between ratio (WB) internal evaluation method in [1] measuring the ratio of the
    ///     sum-of-squares within cluster (SSW) and sum-of-squares between clusters(SSB).
    ///     The result is multiplied by the negative of the number of clusters so that maximizing the ratio in some
    ///     <see cref="ClusteringResult{TInstance}" /> provides the optimal partition, i.e., the optimal
    ///     <see cref="ClusterSet{TInstance}" />.
    /// </summary>
    /// <remarks>
    ///     Notes:
    ///     - In the original formulation in [1] the value was minimized, hence this implementation returns the negative WB
    ///     ratio.
    ///     References:
    ///     [1] -
    ///     <see href="https://doi.org/10.1007/978-3-642-04921-7_32">
    ///         Zhao, Q., Xu, M., &amp; Fränti, P. (2009, April). Sum-of-Squares Based Cluster Validity Index and Significance
    ///         Analysis. In ICANNGA (Vol. 5495, pp. 313-322).
    ///     </see>
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class WithinBetweenRatio<TInstance> : IInternalEvaluationCriterion<TInstance>
        where TInstance : IComparable<TInstance>

    {
        private readonly CentroidFunction<TInstance> _centroidFunc;


        /// <summary>
        ///     Creates a new <see cref="WithinBetweenRatio{TInstance}" /> with given dissimilarity metric.
        /// </summary>
        /// <param name="dissimilarityMetric">The metric used to calculate dissimilarity between cluster elements.</param>
        /// <param name="centroidFunc">
        ///     A function to get an element representing the centroid of a <see cref="Cluster{TInstance}" />.
        /// </param>
        public WithinBetweenRatio(IDissimilarityMetric<TInstance> dissimilarityMetric,
            CentroidFunction<TInstance> centroidFunc)
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

            // gets clusters' centroids and total cluster
            var centroids = new List<TInstance>();
            Cluster<TInstance> totalCluster = null;
            foreach (var cluster in clusterSet)
            {
                totalCluster = totalCluster == null
                    ? new Cluster<TInstance>(cluster)
                    : new Cluster<TInstance>(totalCluster, cluster, 0);
                centroids.Add(_centroidFunc(cluster));
            }

            var centroid = _centroidFunc(totalCluster);

            var sumSquaresBetween = 0d;
            var sumSquaresWithin = 0d;
            for (var i = 0; i < clusterSet.Count; i++)
            {
                // updates sum of squared distances to centroid
                foreach (var instance in clusterSet[i])
                {
                    var distWithin = DissimilarityMetric.Calculate(instance, centroids[i]);
                    sumSquaresWithin += distWithin * distWithin;
                }

                // updates sum of squared distances of cluster centroid to global centroid
                var distBetween = DissimilarityMetric.Calculate(centroids[i], centroid);
                sumSquaresBetween += distBetween * distBetween * clusterSet[i].Count;
            }

            // - m * (SSW / SSB)
            return -clusterSet.Count * (sumSquaresWithin / sumSquaresBetween);
        }
    }
}