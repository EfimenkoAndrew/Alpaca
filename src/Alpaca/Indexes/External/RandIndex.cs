using System;
using System.Collections.Generic;
using System.Text;

namespace AlpacaAnalytics.Indexes.External
{
    public class RandIndex
    {
        public double Calculate(int[] labelsTrue, int[] labelsPred)
        {
            if (labelsTrue.Length != labelsPred.Length)
            {
                throw new ArgumentException("Size of both label arrays must be equal.");
            }

            double a = 0, b = 0;

            for (int i = 0; i < labelsTrue.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    bool inSameClusterInTrue = labelsTrue[i] == labelsTrue[j];
                    bool inSameClusterInPred = labelsPred[i] == labelsPred[j];

                    if (inSameClusterInTrue && inSameClusterInPred) // a and b are in the same cluster in both true and pred clustering
                        a += 1;

                    if (!inSameClusterInTrue && !inSameClusterInPred) // a and b are not in the same cluster in both true and pred clustering
                        b += 1;
                }
            }

            double total = labelsTrue.Length * (labelsTrue.Length - 1) / 2; // total number of pairs of points
            return (a + b) / total; // return Rand Index
        }
    }
}
