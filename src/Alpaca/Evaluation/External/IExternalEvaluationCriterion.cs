﻿using System;
using System.Collections.Generic;
using AlpacaAnalytics.Clustering;
using AlpacaAnalytics.Linkage;

namespace AlpacaAnalytics.Evaluation.External
{
    /// <summary>
    ///     Represents an interface for external criteria to evaluate how well the result of
    ///     <see cref="AgglomerativeClusteringAlgorithm{TInstance}" /> matches the classification of instances according to a
    ///     set of gold
    ///     standard classes. We can think of this as supervised clustering evaluation methods, i.e., external validation
    ///     methods.
    /// </summary>
    /// <remarks>
    ///     These methods are useful for when we have some known partition over the instances and want to evaluate the quality
    ///     of the clustering according to that partition. It can also be used to select the most appropriate
    ///     <see cref="ILinkageCriterion{TInstance}" /> for a given annotated data-set.
    /// </remarks>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    /// <typeparam name="TClass">The type of class considered.</typeparam>
    public interface IExternalEvaluationCriterion<TInstance, TClass> where TInstance : IComparable<TInstance>
    {
        /// <summary>
        ///     Evaluates a given <see cref="ClusterSet{TInstance}" /> partition according to the given class partition.
        /// </summary>
        /// <param name="clusterSet">The clustering partition.</param>
        /// <param name="instanceClasses">The instances' classes.</param>
        /// <returns>The evaluation of the given clustering according to this criterion.</returns>
        double Evaluate(ClusterSet<TInstance> clusterSet, IDictionary<TInstance, TClass> instanceClasses);
    }
}