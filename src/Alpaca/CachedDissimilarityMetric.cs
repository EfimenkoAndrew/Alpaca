using System;
using System.Collections.Generic;
using System.Linq;
using Alpaca.Linkage;

namespace Alpaca
{
    /// <summary>
    ///     Represents a cache to store dissimilarities between all instances of a known set, as dictated by a base
    ///     <see cref="IDissimilarityMetric{TInstance}" />.
    /// </summary>
    /// <remarks>
    ///     This class is useful to use during the execution of <see cref="AgglomerativeClusteringAlgorithm{TInstance}" /> as
    ///     many
    ///     <see cref="ILinkageCriterion{TInstance}" /> classes rely on pair-wise dissimilarities between the
    ///     instances.
    ///     In that sense, the set of instances has to be known beforehand and must not change and no verification is done in
    ///     <see cref="Calculate" />.
    ///     This means that if cluster centroids are used to measure dissimilarities, they have to be included in the original
    ///     set, otherwise the value will not be present in the cache.
    /// </remarks>
    /// <typeparam name="TInstance"></typeparam>
    public class CachedDissimilarityMetric<TInstance> : IDissimilarityMetric<TInstance>, IDisposable
    {
        private readonly IDissimilarityMetric<TInstance> _dissimilarityMeasure;
        private readonly IDictionary<TInstance, int> _elementIdxs = new Dictionary<TInstance, int>();
        private double[][] _dissimilarities;


        /// <summary>
        ///     Creates a new <see cref="CachedDissimilarityMetric{TInstance}" /> according to the given base dissimilarity metric
        ///     and the known set of instances.
        /// </summary>
        /// <param name="dissimilarityMeasure">The metric to be used to cache the dissimilarities between all instances.</param>
        /// <param name="allInstances">The set of instances for which to calculate the pair-wise dissimilarities.</param>
        public CachedDissimilarityMetric(
            IDissimilarityMetric<TInstance> dissimilarityMeasure, ISet<TInstance> allInstances)
        {
            _dissimilarityMeasure = dissimilarityMeasure;

            // registers instances' indexes
            var instances = allInstances.ToArray();
            for (var i = 0; i < instances.Length; i++)
                _elementIdxs.Add(instances[i], i);

            // initializes cache with distances between all instances
            _dissimilarities = new double[instances.Length][];
            for (var i = 0; i < allInstances.Count; i++)
            for (var j = i; j < allInstances.Count; j++)
            {
                if (i == 0) _dissimilarities[j] = new double[instances.Length];
                Store(i, j, instances);
            }
        }


        /// <inheritdoc />
        public void Dispose()
        {
            _elementIdxs.Clear();
            _dissimilarities = null;
        }

        /// <inheritdoc />
        public double Calculate(TInstance instance1, TInstance instance2)
        {
            return _dissimilarities[_elementIdxs[instance1]][_elementIdxs[instance2]];
        }


        private void Store(int i, int j, IList<TInstance> elementList)
        {
            var d = _dissimilarityMeasure.Calculate(elementList[i], elementList[j]);
            _dissimilarities[i][j] = _dissimilarities[j][i] = d;
        }
    }
}