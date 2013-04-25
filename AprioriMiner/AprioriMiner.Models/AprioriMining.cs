// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AprioriMining.cs" company="CesarVelez">
//   Cesar Velez - Copyright 2013
// </copyright>
// <summary>
//   Miner class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AprioriMiner.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Miner class
    /// </summary>
    public class AprioriMining
    {
        /// <summary>
        /// Executes the algorithm
        /// </summary>
        /// <param name="db">The collection of transactions</param>
        /// <param name="uniqueItems">The item set that contains the total number of items (50 in our case)</param>
        /// <param name="supportThreshold">The minimum support</param>
        /// <returns>An item set collection</returns>
        public static ItemsetCollection DoApriori(ItemsetCollection db, Itemset uniqueItems, double supportThreshold)
        {
            // var I = db.GetUniqueItems();
            var I = uniqueItems;
            var L = new ItemsetCollection(); //resultant large itemsets
            var Li = new ItemsetCollection(); //large itemset in each iteration
            var Ci = new ItemsetCollection(); //pruned itemset in each iteration

            // first iteration (1-item itemsets)
            foreach (var item in I)
            {
                Ci.Add(new Itemset() { item });
            }

            // next iterations
            var k = 2;
            while (Ci.Count != 0)
            {
                // set Li from Ci (pruning)
                Li.Clear();
                foreach (Itemset itemset in Ci)
                {
                    itemset.Support = db.FindSupport(itemset);
                    if (itemset.Support >= supportThreshold)
                    {
                        Li.Add(itemset);
                        L.Add(itemset);
                    }
                }

                // set Ci for next iteration (find supersets of Li)
                Ci.Clear();
                Ci.AddRange(Bit.FindSubsets(Li.GetUniqueItems(), k)); // get k-item subsets
                k += 1;
            }

            return L;
        }

        public static List<AssociationRule> Mine(ItemsetCollection db, ItemsetCollection L, double confidenceThreshold)
        {
            var allRules = new List<AssociationRule>();

            foreach (Itemset itemset in L)
            {
                ItemsetCollection subsets = Bit.FindSubsets(itemset, 0); // get all subsets
                foreach (Itemset subset in subsets)
                {
                    double confidence = (db.FindSupport(itemset) / db.FindSupport(subset)) * 100.0;
                    if (confidence >= confidenceThreshold)
                    {
                        var rule = new AssociationRule();
                        rule.X.AddRange(subset);
                        rule.Y.AddRange(itemset.Remove(subset));
                        rule.Support = db.FindSupport(itemset);
                        rule.Confidence = confidence;
                        if (rule.X.Count > 0 && rule.Y.Count > 0)
                        {
                            allRules.Add(rule);
                        }
                    }
                }
            }

            return allRules;
        }
    }
}
