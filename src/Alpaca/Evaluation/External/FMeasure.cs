using System;
using System.Collections.Generic;
using System.Linq;
using AlpacaAnalytics.Clustering;

namespace AlpacaAnalytics.Evaluation.External
{
    /// <summary>
    ///     Evaluates the given partition according to the F-measure, i.e., it measures the accuracy of the clustering by
    ///     measuring the percentage of decisions that are correct (true positives + true negatives).
    /// </summary>
    /// <typeparam name="TInstance">The type of instance considered.</typeparam>
    /// <typeparam name="TClass">The type of class considered.</typeparam>
    /// <remarks>
    ///     see: <a href="https://nlp.stanford.edu/IR-book/html/htmledition/evaluation-of-clustering-1.html" />
    /// </remarks>
    public class FMeasure<TInstance, TClass> : IExternalEvaluationCriterion<TInstance, TClass>
        where TInstance : IComparable<TInstance>

    {
        /// <summary>
        ///     Creates a new instance of <see cref="FMeasure{TInstance,TClass}" /> with the given recall weight.
        /// </summary>
        /// <param name="recallWeight">The weight given to recall in comparison to precision.</param>
        public FMeasure(double recallWeight)
        {
            RecallWeight = recallWeight;
        }


        /// <summary>
        ///     Gets the weight given to recall in comparison to precision.
        /// </summary>
        public double RecallWeight { get; }


        /// <inheritdoc />
        public double Evaluate(ClusterSet<TInstance> clusterSet, IDictionary<TInstance, TClass> instanceClasses)
        {
            // counts the positives for each cluster 
            var truePositives = 0L;
            var falseNegatives = 0L;
            var positives = 0L;
            for (var i = 0; i < clusterSet.Count; i++)
            {
                var clusterGroup = clusterSet[i];

                // gets class counts
                var clusterClassCounts = new Dictionary<TClass, int>();
                foreach (var instance in clusterGroup)
                {
                    var instanceClass = instanceClasses[instance];
                    if (clusterClassCounts.ContainsKey(instanceClass))
                        clusterClassCounts[instanceClass]++;
                    else clusterClassCounts[instanceClass] = 1;

                    // updates false negatives (pairs of same class in diff clusters)
                    for (var j = i + 1; j < clusterSet.Count; j++)
                        falseNegatives += clusterSet[j]
                            .Count(instance2 => instanceClass.Equals(instanceClasses[instance2]));
                }

                // updates positives
                positives += Combinatorics.GetCombinations(clusterGroup.Count, 2);

                // updates true positives (pairs of same class within cluster)
                truePositives += clusterClassCounts.Values
                    .Where(count => count > 1)
                    .Sum(count => Combinatorics.GetCombinations(count, 2));
            }

            var precision = (double)truePositives / positives;
            var recall = (double)truePositives / (truePositives + falseNegatives);

            // returns f-measure
            var weightSquare = RecallWeight * RecallWeight;
            return (weightSquare + 1) * precision * recall / (weightSquare * precision + recall);
        }
    }
}