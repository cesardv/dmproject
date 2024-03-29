﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemsetCollection.cs" company="CesarVelez">
//   Cesar Velez - Copyright 2013
// </copyright>
// <summary>
//   A wrapper class for a list of item sets, which basically represent the database data
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AprioriMiner.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A wrapper class for a list of item sets, which basically represent the database data
    /// </summary>
    public class ItemsetCollection : List<Itemset>
    {
        #region Methods

        /// <summary>
        /// Gets the unique item
        /// </summary>
        /// <returns></returns>
        public Itemset GetUniqueItems()
        {
            var unique = new Itemset();

            foreach (Itemset itemset in this)
            {
                unique.AddRange(from item in itemset
                                where !unique.Contains(item)
                                select item);
            }

            return (unique);
        }

        public double FindSupport(int item)
        {
            int matchCount = (from itemset in this
                              where itemset.Contains(item)
                              select itemset).Count();

            double support = ((double)matchCount / (double)this.Count) * 100.0;
            return (support);
        }

        public double FindSupport(Itemset itemset)
        {
            int matchCount = (from i in this
                              where i.Contains(itemset)
                              select i).Count();

            double support = ((double)matchCount / (double)this.Count) * 100.0;
            return (support);
        }

        public override string ToString()
        {
            return (string.Join("\r\n", (from itemset in this select itemset.ToString()).ToArray()));
        }

        #endregion
    }
}
