using System;
using System.Linq;

namespace Alpaca.Indexers.Silhouette
{
    internal class SilhouetteIndex
    {
        public double CalculateSilhouetteScore(double[][] dataPoints, int[] clusterAssignments)
        {
            double sumOfScores = 0;

            for (int dataIndex = 0; dataIndex < dataPoints.Length; dataIndex++)
            {
                double averageDistanceSameCluster = CalculateAverageDistanceToSameCluster(dataPoints, clusterAssignments, dataIndex, clusterAssignments[dataIndex]);
                double minimumAverageDistanceDifferentCluster = Double.PositiveInfinity;

                for (int clusterIndex = 0; clusterIndex <= clusterAssignments.Max(); clusterIndex++)
                {
                    if (clusterIndex != clusterAssignments[dataIndex])
                    {
                        double averageDistanceDifferentCluster = CalculateAverageDistanceToSameCluster(dataPoints, clusterAssignments, dataIndex, clusterIndex);
                        minimumAverageDistanceDifferentCluster = (averageDistanceDifferentCluster < minimumAverageDistanceDifferentCluster) ? averageDistanceDifferentCluster : minimumAverageDistanceDifferentCluster;
                    }
                }

                sumOfScores += (minimumAverageDistanceDifferentCluster - averageDistanceSameCluster) / Math.Max(averageDistanceSameCluster, minimumAverageDistanceDifferentCluster);
            }

            return sumOfScores / dataPoints.Length;
        }

        private double CalculateAverageDistanceToSameCluster(double[][] dataPoints, int[] clusterAssignments, int dataPointIndex, int clusterAssignment)
        {
            double distanceSum = 0;
            int numDataPointsInCluster = 0;

            for (int dataIndex = 0; dataIndex < dataPoints.Length; dataIndex++)
            {
                if (clusterAssignments[dataIndex] == clusterAssignment)
                {
                    distanceSum += CalculateEuclideanDistance(dataPoints[dataIndex], dataPoints[dataPointIndex]);
                    numDataPointsInCluster++;
                }
            }

            return distanceSum / numDataPointsInCluster;
        }

        private double CalculateEuclideanDistance(double[] pointA, double[] pointB)
        {
            var distanceSquareSum = 0d;
            for (int dimensionIndex = 0; dimensionIndex < pointA.Length; dimensionIndex++)
            {
                distanceSquareSum += (pointA[dimensionIndex] - pointB[dimensionIndex]) * (pointA[dimensionIndex] - pointB[dimensionIndex]);
            }
            return Math.Sqrt(distanceSquareSum);
        }
    }
}
