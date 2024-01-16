using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnicornAnalytics.Clustering
{
    public class HierarchicalClustering
    {
        public double[][] Centroids { get; private set; }

        private List<List<int>> clusters;

        public void Cluster(double[][] data)
        {
            int count = data.Length;

            clusters = new List<List<int>>();
            for (int i = 0; i < count; i++)
            {
                clusters.Add(new List<int> { i });
            }

            while (clusters.Count > 1)
            {
                double minDistance = double.MaxValue;
                int[] minPair = new int[2];

                for (int i = 0; i < clusters.Count; i++)
                {
                    for (int j = i + 1; j < clusters.Count; j++)
                    {
                        double distance = FindDistance(data, clusters, i, j);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            minPair[0] = i;
                            minPair[1] = j;
                        }
                    }
                }

                clusters[minPair[0]].AddRange(clusters[minPair[1]]);
                clusters.RemoveAt(minPair[1]);
            }

            Centroids = clusters.Select(cluster => CalculateCentroid(data, cluster)).ToArray();
        }

        private double FindDistance(double[][] data, List<List<int>> clusters, int i, int j)
        {
            double maxDistance = double.NegativeInfinity;

            foreach (int point1 in clusters[i])
            {
                foreach (int point2 in clusters[j])
                {
                    double distance = EuclideanDistance(data[point1], data[point2]);
                    maxDistance = Math.Max(maxDistance, distance);
                }
            }

            return maxDistance;
        }

        private double[] CalculateCentroid(double[][] data, List<int> cluster)
        {
            int dimensions = data[0].Length;
            double[] centroid = new double[dimensions];
            foreach (var index in cluster)
            {
                for (int j = 0; j < dimensions; j++)
                {
                    centroid[j] += data[index][j];
                }
            }

            for (int i = 0; i < dimensions; i++)
            {
                centroid[i] /= cluster.Count;
            }

            return centroid;
        }

        private double EuclideanDistance(double[] vector1, double[] vector2)
        {
            double sum = 0.0;
            for (int i = 0; i < vector1.Length; i++)
                sum += Math.Pow(vector1[i] - vector2[i], 2);
            return Math.Sqrt(sum);
        }

        public int[][] GetClusters()
        {
            return clusters.Select(cluster => cluster.ToArray()).ToArray();
        }
    }
}
