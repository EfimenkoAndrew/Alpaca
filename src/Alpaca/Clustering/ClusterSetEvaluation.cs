using System;

namespace AlpacaAnalytics.Clustering
{
    /// <summary>
    ///     Represents the result of evaluating some <see cref="ClusterSet{TInstance}" /> according to some criterion.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public struct ClusterSetEvaluation<TInstance> where TInstance : IComparable<TInstance>
    {
        /// <summary>
        ///     Creates a new <see cref="ClusterSetEvaluation{TInstance}" />.
        /// </summary>
        /// <param name="clusterSet">The cluster-set that was evaluated.</param>
        /// <param name="evaluationValue">The value of the evaluation.</param>
        public ClusterSetEvaluation(ClusterSet<TInstance> clusterSet, double evaluationValue)
        {
            ClusterSet = clusterSet;
            EvaluationValue = evaluationValue;
        }


        /// <summary>
        ///     Gets the cluster-set that was evaluated.
        /// </summary>
        public ClusterSet<TInstance> ClusterSet { get; }

        /// <summary>
        ///     Gets the value of the evaluation.
        /// </summary>
        public double EvaluationValue { get; }


        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{ClusterSet}][{EvaluationValue:0.00}]";
        }
    }
}