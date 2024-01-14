using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlpacaAnalytics.Clustering
{
    public class AffinityPropagation
    {
        public int[] Exemplars { get; private set; }

        public double[][] Centroids { get; private set; }

        public void Fit(double[][] data, double damping, int maxIterations, double convergenceIters)
        {
            int N = data.Length;
            double[][] A = new double[N][];
            double[][] R = new double[N][];
            for (int i = 0; i < N; i++)
            {
                A[i] = new double[N];
                R[i] = new double[N];
            }

            int iterations = 0;
            bool coverged = false;
            Exemplars = new int[N];
            while ((!coverged && iterations < maxIterations) || iterations < convergenceIters)
            {
                iterations++;
                // ... Rest of your affinity propagation code

                // Add this at the end of your while loop
                if (!coverged)
                    Centroids = CalculateCentroids(data, Exemplars);
            }
        }

        private double[][] CalculateCentroids(double[][] data, int[] exemplars)
        {
            int dimension = data[0].Length;
            int numExemplars = exemplars.Distinct().Count();

            double[][] centroids = new double[numExemplars][];
            for (int i = 0; i < numExemplars; i++)
            {
                centroids[i] = new double[dimension];
            }

            int[] clusterSizes = new int[numExemplars];

            for (int i = 0; i < data.Length; i++)
            {
                int cluster = Array.IndexOf(exemplars, exemplars[i]);
                for (int j = 0; j < dimension; j++)
                {
                    centroids[cluster][j] += data[i][j];
                }
                clusterSizes[cluster]++;
            }

            for (int i = 0; i < numExemplars; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    centroids[i][j] /= clusterSizes[i];
                }
            }
            return centroids;
        }
    }
}
