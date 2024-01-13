using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlpacaAnalytics.Clustering
{
    /// <summary>
    ///     Represents a set of <typeparamref name="TInstance" /> elements arranged in a hierarchical form.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class Cluster<TInstance> :
        IEnumerable<TInstance>, IEquatable<Cluster<TInstance>>, IComparable<Cluster<TInstance>>
        where TInstance : IComparable<TInstance>
    {
        private const double MIN_DISSIMILARITY = double.Epsilon;

        /// <summary>
        ///     Gets an empty cluster.
        /// </summary>
        public static readonly Cluster<TInstance> Empty = new Cluster<TInstance>(new List<TInstance>());


        private readonly TInstance[] _cluster;

        private readonly int _hashCode;


        /// <summary>
        ///     Creates a new <see cref="Cluster{TInstance}" /> by joining the two given clusters.
        /// </summary>
        /// <param name="parent1">The first parent of the new cluster.</param>
        /// <param name="parent2">The second parent of the new cluster.</param>
        /// <param name="dissimilarity">The dissimilarity/distance at which the new cluster was found.</param>
        public Cluster(Cluster<TInstance> parent1, Cluster<TInstance> parent2, double dissimilarity)
        {
            // parent order is not important
            Parent1 = parent1.CompareTo(parent2) > 0 ? parent2 : parent1;
            Parent2 = parent1.CompareTo(parent2) > 0 ? parent1 : parent2;
            Dissimilarity = dissimilarity;

            // copies and sorts elements 
            _cluster = new TInstance[parent1._cluster.Length + parent2._cluster.Length];
            int idx = 0, i = 0, j = 0;
            while (i < parent1.Count && j < parent2.Count)
            {
                // checks elements from each parent, chooses lowest
                _cluster[idx++] = parent1._cluster[i].CompareTo(parent2._cluster[j]) <= 0
                    ? parent1._cluster[i++]
                    : parent2._cluster[j++];

                // if all the elements of one parent were copied, just copy those of the other parent
                if (i >= parent1.Count)
                {
                    Array.Copy(parent2._cluster, j, _cluster, idx, parent2._cluster.Length - j);
                    break;
                }

                if (j >= parent2.Count)
                {
                    Array.Copy(parent1._cluster, i, _cluster, idx, parent1._cluster.Length - i);
                    break;
                }
            }

            _hashCode = ProduceHashCode();
        }

        /// <summary>
        ///     Creates a new <see cref="Cluster{TInstance}" /> with a single <typeparamref name="TInstance" /> element.
        /// </summary>
        /// <param name="instance">The single element in the new cluster.</param>
        /// <param name="dissimilarity">The dissimilarity/distance at which the new cluster was found.</param>
        public Cluster(TInstance instance, double dissimilarity = 0) : this(new[] { instance }, dissimilarity)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="Cluster{TInstance}" /> with the given <typeparamref name="TInstance" /> elements.
        /// </summary>
        /// <param name="instances">The elements in the new cluster.</param>
        /// <param name="dissimilarity">The dissimilarity/distance at which the new cluster was found.</param>
        public Cluster(IEnumerable<TInstance> instances, double dissimilarity = 0)
        {
            Dissimilarity = dissimilarity;
            var list = instances as List<TInstance> ?? instances.ToList();
            list.Sort();
            _cluster = list.ToArray();
            _hashCode = ProduceHashCode();
        }

        /// <summary>
        ///     Creates a new <see cref="Cluster{TInstance}" /> which is an exact copy of the given cluster.
        /// </summary>
        /// <param name="cluster">The cluster to be copied into the new cluster.</param>
        public Cluster(Cluster<TInstance> cluster)
        {
            _cluster = cluster._cluster.ToArray();
            Parent1 = cluster.Parent1;
            Parent2 = cluster.Parent2;
            Dissimilarity = cluster.Dissimilarity;
            _hashCode = cluster._hashCode;
        }


        /// <summary>
        ///     Gets the number of elements in this cluster.
        /// </summary>
        public int Count => _cluster.Length;

        /// <summary>
        ///     Gets the dissimilarity / distance at which this cluster was found by the clustering algorithm.
        /// </summary>
        public double Dissimilarity { get; }

        /// <summary>
        ///     Gets this cluster's first parent, if the cluster was formed by joining two existing clusters. Otherwise returns
        ///     <c>null</c>.
        /// </summary>
        public Cluster<TInstance> Parent1 { get; }

        /// <summary>
        ///     Gets this cluster's second parent, if the cluster was formed by joining two existing clusters. Otherwise returns
        ///     <c>null</c>.
        /// </summary>
        public Cluster<TInstance> Parent2 { get; }

        /// <summary>
        ///     Compares this cluster with another cluster instance. Comparison is performed by count (number of items) first, then
        ///     by string representation of the items.
        /// </summary>
        /// <param name="other">The cluster to compare to.</param>
        /// <returns>
        ///     <c>-1</c> if <paramref name="other" /> is <c>null</c>, the result of <see cref="Count" /> comparison between
        ///     the clusters, or the result of <c>string.CompareOrdinal"</c> if the clusters have the same count.
        /// </returns>
        public int CompareTo(Cluster<TInstance> other)
        {
            // compares by count first, then by string representation of the elements
            if (other == null) return -1;
            var countCompare = _cluster.Length.CompareTo(other._cluster.Length);
            return countCompare == 0 ? string.CompareOrdinal(ToString(), other.ToString()) : countCompare;
        }

        /// <inheritdoc />
        public IEnumerator<TInstance> GetEnumerator()
        {
            return ((IEnumerable<TInstance>)_cluster).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Checks whether this cluster is equal to another. Equality between cluster occurs when they are the same object or
        ///     when the clusters contain the same elements, were created based on the same parent clusters and have the same
        ///     associated dissimilarity.
        /// </summary>
        /// <param name="other">The other cluster to verify equality.</param>
        /// <returns><c>true</c> if the clusters are equal, <c>false</c> otherwise.</returns>
        public bool Equals(Cluster<TInstance> other)
        {
            return !(other is null) &&
                   (ReferenceEquals(this, other) ||
                    _hashCode == other._hashCode &&
                     Math.Abs(Dissimilarity - other.Dissimilarity) < MIN_DISSIMILARITY &&
                     (Parent1 != null && Parent2 != null &&
                       Equals(Parent1, other.Parent1) && Equals(Parent2, other.Parent2) ||
                      _cluster.SequenceEqual(other._cluster)));
        }


        /// <inheritdoc />
        public override bool Equals(object other)
        {
            return !(other is null) &&
                   (ReferenceEquals(this, other) ||
                    other.GetType() == GetType() && Equals((Cluster<TInstance>)other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _hashCode;
        }

        /// <summary>
        ///     Gets a string representing this cluster in the form (item1;item2;...;itemN).
        /// </summary>
        /// <returns>A string representing the cluster.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder("(");
            foreach (var instance in _cluster)
                sb.Append($"{instance};");
            if (_cluster.Length > 0) sb.Remove(sb.Length - 1, 1);
            sb.Append(")");
            return sb.ToString();
        }


        /// <summary>
        ///     Creates a new <see cref="Cluster{TInstance}" /> which is an exact copy of this cluster.
        /// </summary>
        /// <returns>A new <see cref="Cluster{TInstance}" /> which is an exact copy of this cluster.</returns>
        public Cluster<TInstance> Clone()
        {
            return new Cluster<TInstance>(this);
        }

        /// <summary>
        ///     Checks whether this cluster contains the given item.
        /// </summary>
        /// <param name="item">The item whose presence in the cluster we want to check.</param>
        /// <returns><c>true</c> if the cluster contains the given item, <c>false</c> otherwise.</returns>
        public bool Contains(TInstance item)
        {
            return _cluster.Contains(item);
        }


        private int ProduceHashCode()
        {
            unchecked
            {
                var hashCode = Dissimilarity.GetHashCode();
                foreach (var instance in this) hashCode += hashCode * 397 ^ instance.GetHashCode();
                return hashCode;
            }
        }
    }
}