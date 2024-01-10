namespace Alpaca
{
    /// <summary>
    ///     Represents an interface for metrics measuring the dissimilarity/distance between instances.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public interface IDissimilarityMetric<in TInstance>
    {
        /// <summary>
        ///     Calculates the distance between two instances according to this metric.
        /// </summary>
        /// <param name="instance1">The first instance.</param>
        /// <param name="instance2">The second instance.</param>
        /// <returns>A value representing the distance between two instances according to this metric.</returns>
        double Calculate(TInstance instance1, TInstance instance2);
    }
}