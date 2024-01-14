using System;
using System.Collections.Generic;
using System.Text;

namespace AlpacaAnalytics.Indexes.External
{
    public class RusselRaoClusteringIndex
    {
        public double Calculate(int[] labelsTrue, int[] labelsPred)
        {
            if (labelsTrue.Length != labelsPred.Length)
            {
                throw new ArgumentException("Input vectors should have the same length");
            }

            int intersection = 0;
            for (int i = 0; i < labelsTrue.Length; i++)
            {
                if (labelsTrue[i] == labelsPred[i]) intersection++;
            }

            return intersection / (double)labelsTrue.Length;
        }
    }
}
