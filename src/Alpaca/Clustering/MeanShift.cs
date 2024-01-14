using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlpacaAnalytics.Clustering
{
    public class MeanShift
    {
        private double _bandwidth;

        public double[][] Centroids { get; private set; }

        public int[] ClusterAssignments { get; private set; }

        public MeanShift(double bandwidth)
        {
            this._bandwidth = bandwidth;
        }

        public void Fit(double[][] data)
        {
            List<double[]> newCentroids = new List<double[]>();
            bool[] visited = new bool[data.Length];
            ClusterAssignments = new int[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                if (!visited[i])
                {
                    double[] centroid = data[i];
                    while (true)
                    {
                        List<double[]> inBandwidth = new List<double[]>();

                        for (int j = 0; j < data.Length; j++)
                        {
                            if (EuclideanDistance(data[j], centroid) <= _bandwidth)
                            {
                                inBandwidth.Add(data[j]);
                                visited[j] = true;
                            }
                        }

                        double[] newCentroid = inBandwidth.Aggregate((x, y) =>
                           x.Zip(y, (a, b) => a + b).ToArray());

                        for (int k = 0; k < newCentroid.Length; k++)
                        {
                            newCentroid[k] /= inBandwidth.Count;
                        }

                        if (EuclideanDistance(newCentroid, centroid) < 1e-5)
                        {
                            break;
                        }

                        centroid = newCentroid;
                    }

                    newCentroids.Add(centroid);

                    int clusterId = newCentroids.Count - 1;
                    for (int l = 0; l < data.Length; l++)
                    {
                        if (EuclideanDistance(data[l], centroid) <= _bandwidth)
                        {
                            ClusterAssignments[l] = clusterId;
                        }
                    }
                }
            }

            Centroids = newCentroids.ToArray();
        }

        private double EuclideanDistance(double[] a, double[] b)
        {
            return Math.Sqrt(a.Zip(b, (x, y) => (x - y) * (x - y)).Sum());
        }

        public double[][] GetCentroids()
        {
            return Centroids;
        }

        public int[] GetClusterAssignments()
        {
            return ClusterAssignments;
        }
    }

}
