// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

namespace Alpaca.Math.AForge.Math.Metrics
{
    /// <summary>
    ///   Please use Alpaca.Math.Distances.PearsonCorrelation instead.
    /// </summary>
    [Obsolete("Please use Alpaca.Math.Distances.PearsonCorrelation instead.")]
    public sealed class PearsonCorrelation : ISimilarity
    {
        /// <summary>
        ///   Please use Alpaca.Math.Distances.PearsonCorrelation instead.
        /// </summary>
        public double GetSimilarityScore(double[] p, double[] q)
        {
            return new Alpaca.Math.Distances.PearsonCorrelation().Similarity(p, q);
        }
    }
}
