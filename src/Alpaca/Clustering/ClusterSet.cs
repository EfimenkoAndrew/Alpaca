using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AlpacaAnalytics.Clustering
{
    /// <summary>
    ///     Represents a set of <see cref="Cluster{TInstance}" /> elements that were found during the execution of the
    ///     clustering algorithm separated at some minimum distance.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class ClusterSet<TInstance> : IEnumerable<Cluster<TInstance>> where TInstance : IComparable<TInstance>
    {
        private readonly Cluster<TInstance>[] _clusters;


        /// <summary>
        ///     Creates a new <see cref="ClusterSet{TInstance}" /> with the given clusters and distance.
        /// </summary>
        /// <param name="clusters">The set of clusters.</param>
        /// <param name="dissimilarity">The dissimilarity/distance at which the clusters were found.</param>
        public ClusterSet(Cluster<TInstance>[] clusters, double dissimilarity = 0)
        {
            _clusters = clusters;
            Dissimilarity = dissimilarity;
        }


        /// <summary>
        ///     Gets the number of clusters.
        /// </summary>
        public int Count => _clusters.Length;

        /// <summary>
        ///     The minimum dissimilarity/distance at which the clusters were found.
        /// </summary>
        public double Dissimilarity { get; }

        /// <summary>
        ///     Gets the cluster at the give index.
        /// </summary>
        public Cluster<TInstance> this[int index] => _clusters[index];

        /// <inheritdoc />
        public IEnumerator<Cluster<TInstance>> GetEnumerator()
        {
            return ((IEnumerable<Cluster<TInstance>>)_clusters).GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        ///     Returns a string representation for the cluster-set in the form 'Dissimilarity   {cluster1, cluster2, ...,
        ///     clusterN}'.
        /// </summary>
        /// <returns>A string representation for the cluster-set.</returns>
        public override string ToString()
        {
            return ToString(true);
        }


        /// <summary>
        ///     Returns a string representation for the cluster-set in the form 'Dissimilarity   {cluster1, cluster2, ...,
        ///     clusterN}'. The presentation of the dissimilarity value is optional.
        /// </summary>
        /// <param name="includeDissimilarity">
        ///     Whether to include the value of <see cref="Dissimilarity" /> in the string representation.
        /// </param>
        /// <returns>A string representation for the cluster-set.</returns>
        public string ToString(bool includeDissimilarity)
        {
            var sb = includeDissimilarity
                ? new StringBuilder($"{Dissimilarity:0.000}\t{{")
                : new StringBuilder("{{");

            foreach (var cluster in this)
                sb.Append($"{cluster}, ");
            if (Count > 0) sb.Remove(sb.Length - 2, 2);
            sb.Append("}");
            return sb.ToString();
        }
    }
}