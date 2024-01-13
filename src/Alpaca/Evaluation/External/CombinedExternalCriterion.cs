using System;
using System.Collections.Generic;
using System.Linq;
using AlpacaAnalytics.Clustering;

namespace AlpacaAnalytics.Evaluation.External
{
    /// <summary>
    ///     Implements an external clustering evaluation criterion as a combination (weighted average) of other
    ///     <see cref="IExternalEvaluationCriterion{TInstance,TClass}" />.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    /// <typeparam name="TClass">The type of class considered.</typeparam>
    public class CombinedExternalCriterion<TInstance, TClass> : IExternalEvaluationCriterion<TInstance, TClass>
        where TInstance : IComparable<TInstance>

    {
        private readonly IDictionary<IExternalEvaluationCriterion<TInstance, TClass>, double> _criteria;


        /// <summary>
        ///     Creates a new <see cref="CombinedExternalCriterion{TInstance,TClass}" /> according to the given criteria and
        ///     respective weights.
        /// </summary>
        /// <param name="criteria">
        ///     A dictionary containing the several criteria to be used and how should they be combined, i.e., their associated
        ///     weights.
        /// </param>
        public CombinedExternalCriterion(IDictionary<IExternalEvaluationCriterion<TInstance, TClass>, double> criteria)
        {
            _criteria = criteria;
        }


        /// <inheritdoc />
        public double Evaluate(ClusterSet<TInstance> clusterSet, IDictionary<TInstance, TClass> instanceClasses)
        {
            return _criteria.Sum(
                criterion => criterion.Key.Evaluate(clusterSet, instanceClasses) * criterion.Value);
        }
    }
}