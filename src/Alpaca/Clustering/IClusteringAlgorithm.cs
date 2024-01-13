using System;
using System.Collections.Generic;
using AlpacaAnalytics.Linkage;

namespace AlpacaAnalytics.Clustering
{
    /// <summary>
    ///     Represents an interface for hierarchical agglomerative clustering algorithms.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public interface IClusteringAlgorithm<TInstance> where TInstance : IComparable<TInstance>
    {
        /// <summary>
        ///     Gets the <see cref="ILinkageCriterion{TInstance}" /> used by this algorithm to create the clusters.
        /// </summary>
        ILinkageCriterion<TInstance> LinkageCriterion { get; }


        /// <summary>
        ///     Clusters the set of <typeparamref name="TInstance" /> given to the algorithm.
        /// </summary>
        /// <param name="instances">The instances to be clustered by the algorithm.</param>
        /// <returns>
        ///     A <see cref="ClusteringResult{TInstance}" /> containing all the <see cref="ClusterSet{TInstance}" /> found in each
        ///     step of the algorithm and the corresponding the dissimilarity/distance at which they were found.
        /// </returns>
        ClusteringResult<TInstance> GetClustering(ISet<TInstance> instances);

        /// <summary>
        ///     Runs the clustering algorithm over the set of given <see cref="Cluster{TInstance}" />.
        /// </summary>
        /// <param name="clusters">The initial clusters provided to the algorithm.</param>
        /// <param name="dissimilarity">The initial dissimilarity associated with the given clusters.</param>
        /// <returns>
        ///     A <see cref="ClusteringResult{TInstance}" /> containing all the <see cref="ClusterSet{TInstance}" /> found in each
        ///     step of the algorithm and the corresponding the dissimilarity/distance at which they were found.
        /// </returns>
        ClusteringResult<TInstance> GetClustering(IEnumerable<Cluster<TInstance>> clusters, double dissimilarity = 0d);

        /// <summary>
        ///     Runs the clustering algorithm over the given <see cref="ClusterSet{TInstance}" />.
        /// </summary>
        /// <param name="clusterSet">The initial clusters and dissimilarity provided to the algorithm.</param>
        /// <returns>
        ///     A <see cref="ClusteringResult{TInstance}" /> containing all the <see cref="ClusterSet{TInstance}" /> found in each
        ///     step of the algorithm and the corresponding the dissimilarity/distance at which they were found.
        /// </returns>
        ClusteringResult<TInstance> GetClustering(ClusterSet<TInstance> clusterSet);
    }
}