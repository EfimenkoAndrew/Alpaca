using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AlpacaAnalytics.Clustering
{
    /// <summary>
    ///     Represents the result of a clustering algorithm, consisting in the sequence of <see cref="ClusterSet{TInstance}" />
    ///     elements that were found during the agglomeration of all clusters.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class ClusteringResult<TInstance> : IEnumerable<ClusterSet<TInstance>>
        where TInstance : IComparable<TInstance>
    {
        private readonly ClusterSet<TInstance>[] _clusterSets;


        /// <summary>
        ///     Creates a new <see cref="ClusteringResult{TInstance}" /> of the given size.
        /// </summary>
        /// <param name="size">The maximum number of <see cref="ClusterSet{TInstance}" /> to be added by the algorithm.</param>
        public ClusteringResult(int size)
        {
            _clusterSets = new ClusterSet<TInstance>[size];
        }


        /// <summary>
        ///     Gets the number of <see cref="ClusterSet{TInstance}" /> found by the algorithm.
        /// </summary>
        public int Count => _clusterSets.Length;

        /// <summary>
        ///     Gets or sets the <see cref="ClusterSet{TInstance}" /> at the given index of the sequence.
        /// </summary>
        /// <param name="index">The index of the cluster set we want to get or set.</param>
        /// <returns>The <see cref="ClusterSet{TInstance}" /> at the given index of the sequence.</returns>
        public ClusterSet<TInstance> this[int index]
        {
            get => _clusterSets[index];
            set => _clusterSets[index] = value;
        }

        /// <summary>
        ///     Gets the <see cref="Cluster{TInstance}" /> corresponding to the agglomeration of all the
        ///     <typeparamref name="TInstance" />
        ///     elements considered by the algorithm.
        /// </summary>
        public Cluster<TInstance> SingleCluster => _clusterSets[_clusterSets.Length - 1][0];

        /// <inheritdoc />
        public IEnumerator<ClusterSet<TInstance>> GetEnumerator()
        {
            return ((IEnumerable<ClusterSet<TInstance>>)_clusterSets).GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        ///     Saves the <see cref="ClusterSet{TInstance}" /> objects stored in this <see cref="ClusteringResult{TInstance}" /> in
        ///     a comma-separated values (CSV) file.
        /// </summary>
        /// <param name="filePath">The path to the file in which to save the clustering results.</param>
        /// <param name="sepChar">The character used to separate the fields in the CSV file.</param>
        public void SaveToCsv(string filePath, char sepChar = ',')
        {
            using (var fs = new FileStream(filePath, FileMode.Create))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.WriteLine($"Num. clusters{sepChar}Dissimilarity{sepChar}Cluster{sepChar}Instance");
                foreach (var clusterSet in this.Reverse())
                    for (var i = 0; i < clusterSet.Count; i++)
                    {
                        var cluster = clusterSet[i];
                        foreach (var instance in cluster)
                            sw.WriteLine(
                                $"{clusterSet.Count}{sepChar}{clusterSet.Dissimilarity}{sepChar}{i}{sepChar}{instance}");
                    }
            }
        }
    }
}