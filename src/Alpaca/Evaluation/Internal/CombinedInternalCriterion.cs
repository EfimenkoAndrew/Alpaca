using System;
using System.Collections.Generic;
using System.Linq;

namespace Alpaca.Evaluation.Internal
{
    /// <summary>
    ///     Implements an internal clustering evaluation criterion as a combination (weighted average) of other
    ///     <see cref="IInternalEvaluationCriterion{TInstance}" />.
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    public class CombinedInternalCriterion<TInstance> : IInternalEvaluationCriterion<TInstance>
        where TInstance : IComparable<TInstance>

    {
        private readonly IDictionary<IInternalEvaluationCriterion<TInstance>, double> _criteria;


        /// <summary>
        ///     Creates a new <see cref="CombinedInternalCriterion{TInstance}" /> according to the given criteria and respective
        ///     weights.
        /// </summary>
        /// <param name="criteria">
        ///     A dictionary containing the several criteria to be used and how should they be combined, i.e., their associated
        ///     weights.
        /// </param>
        public CombinedInternalCriterion(IDictionary<IInternalEvaluationCriterion<TInstance>, double> criteria)
        {
            _criteria = criteria;
        }


        /// <inheritdoc />
        public IDissimilarityMetric<TInstance> DissimilarityMetric => null;


        /// <inheritdoc />
        public double Evaluate(ClusterSet<TInstance> clusterSet)
        {
            return _criteria.Sum(criterion => criterion.Key.Evaluate(clusterSet) * criterion.Value);
        }
    }
}