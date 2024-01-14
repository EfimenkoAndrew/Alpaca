using System;
using System.Linq;

namespace UnicornAnalytics.Clustering
{
    public class FuzzyCMeans
    {
        private int numClusters;
        private double fuzziness;
        private double[][] centroids;
        private double[][] memberships;

        public FuzzyCMeans(int numClusters, double fuzziness)
        {
            this.numClusters = numClusters;
            this.fuzziness = fuzziness;
        }

        public void Fit(double[][] data, int maxIterations)
        {
            int numDataPoints = data.Length;
            int numDimensions = data[0].Length;
            centroids = new double[numClusters][]; // Initialize centroids

            Random random = new Random();
            int[] indices = Enumerable.Range(0, numDataPoints).OrderBy(x => random.Next()).ToArray(); // Get random indices
            for (int i = 0; i < numClusters; i++) centroids[i] = data[indices[i]]; // Assign random centroids

            // Initialize membership array
            memberships = new double[numDataPoints][];

            for (int t = 0; t < maxIterations; t++)
            {
                for (int i = 0; i < numDataPoints; i++)
                {
                    memberships[i] = new double[numClusters];
                    for (int j = 0; j < numClusters; j++)
                    {
                        memberships[i][j] = CalculateMembership(data[i], centroids[j], data);
                    }
                }

                for (int j = 0; j < numClusters; j++)
                {
                    centroids[j] = CalculateCentroid(memberships, data, j);
                }
            }
        }

        private double CalculateMembership(double[] dataPoint, double[] centroid, double[][] data)
        {
            double membership = 0;
            double currentDistance = EuclideanDistance(dataPoint, centroid);
            for (int i = 0; i < numClusters; i++)
            {
                if (currentDistance != 0)
                {
                    double ratio = currentDistance / EuclideanDistance(dataPoint, centroids[i]);
                    membership += Math.Pow(ratio, 2 / (fuzziness - 1));
                }
            }
            return (membership == 0) ? 0 : 1 / membership;
        }

        private double[] CalculateCentroid(double[][] membership, double[][] data, int clusterIndex)
        {
            double[] centroid = new double[data[0].Length];
            double sumMembership = 0;
            for (int i = 0; i < data.Length; i++)
            {
                double currentMembership = Math.Pow(membership[i][clusterIndex], fuzziness);
                sumMembership += currentMembership;
                for (int j = 0; j < data[i].Length; j++)
                    centroid[j] += currentMembership * data[i][j];
            }
            for (int j = 0; j < centroid.Length; j++)
                centroid[j] /= sumMembership;
            return centroid;
        }

        private double EuclideanDistance(double[] a, double[] b)
        {
            return Math.Sqrt(a.Zip(b, (x, y) => (x - y) * (x - y)).Sum());
        }

        public double[][] GetCentroids()
        {
            return centroids;
        }

        public int[] GetClusterAssignments()
        {
            return memberships.Select(ms => Array.IndexOf(ms, ms.Max())).ToArray();
        }
    }

}
