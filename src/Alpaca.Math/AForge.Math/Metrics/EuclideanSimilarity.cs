// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2010
// contacts@aforgenet.com
//

using Accord.Math.Distances;

namespace Alpaca.Math.AForge.Math.Metrics
{
    /// <summary>
    ///   Please use Accord.Math.Distances.Euclidean instead.
    /// </summary>
    [Obsolete("Please use Accord.Math.Distances.Euclidean instead.")]
    public sealed class EuclideanSimilarity : ISimilarity
    {
        /// <summary>
        ///   Please use Accord.Math.Distances.Euclidean instead.
        /// </summary>
        public double GetSimilarityScore(double[] p, double[] q)
        {
            return new Euclidean().Similarity(p, q);
        }
    }
}
