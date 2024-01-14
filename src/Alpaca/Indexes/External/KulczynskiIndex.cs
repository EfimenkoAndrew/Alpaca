using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlpacaAnalytics.Indexes.External
{
    public class KulczynskiIndex
    {
        public double Calculate(int[] partitionA, int[] partitionB)
        {
            if (partitionA.Length != partitionB.Length)
            {
                throw new ArgumentException("Size of both partitions should be equal");
            }

            int numDataPoints = partitionA.Length;
            double intersectionAB = 0, intersectionBA = 0;
            double[] countA = new double[numDataPoints];
            double[] countB = new double[numDataPoints];

            // Count the number of points in each cluster in partition A and partition B
            for (int i = 0; i < numDataPoints; i++)
            {
                countA[partitionA[i]]++;
                countB[partitionB[i]]++;
            }

            // Calculate the intersection of clusters from partition A to partition B and vice versa
            for (int i = 0; i < numDataPoints; i++)
            {
                for (int j = i + 1; j < numDataPoints; j++)
                {
                    bool inSameClusterA = partitionA[i] == partitionA[j];
                    bool inSameClusterB = partitionB[i] == partitionB[j];

                    if (inSameClusterA && inSameClusterB)
                    {
                        intersectionAB++;
                        intersectionBA++;
                    }
                    else if (inSameClusterA)
                    {
                        intersectionAB++;
                    }
                    else if (inSameClusterB)
                    {
                        intersectionBA++;
                    }
                }
            }

            return 0.5 * (intersectionAB / countA.Sum() + intersectionBA / countB.Sum());
        }
    }

}
