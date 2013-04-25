// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Itemset.cs" company="CesarVelez">
//   Cesar Velez - Copyright 2013
// </copyright>
// <summary>
//   <see cref="Itemset" /> class represents a list of integers which represent items in a transaction
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AprioriMiner.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// <see cref="Itemset"/> class represents a list of integers which represent items in a transaction
    /// </summary>
    public class Itemset : List<int>
    {
        #region Properties

        /// <summary>
        /// Gets or sets support of this Itemset per the Data. Support(Itemset) = Count(Itemset)/TotalNumberOfTransactions
        /// </summary>
        public double Support { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns whether or not this Item set contains another
        /// </summary>
        /// <param name="itemset">Item set we're testing</param>
        /// <returns>A boolean indicating if item set is a subset</returns>
        public bool Contains(Itemset itemset)
        {
            return this.Intersect(itemset).Count() == itemset.Count;
        }

        /// <summary>
        /// Removes the subset
        /// </summary>
        /// <param name="itemset">subset item set</param>
        /// <returns>A new item set without the subset</returns>
        public Itemset Remove(Itemset itemset)
        {
            var removed = new Itemset();
            removed.AddRange(from item in this
                             where !itemset.Contains(item)
                             select item);
            return removed;
        }

        /// <summary>
        /// ToString of this set
        /// </summary>
        /// <returns>A string representation</returns>
        public override string ToString()
        {
            return "{" + string.Join(", ", this.ToArray()) + "}" + (this.Support > 0 ? " (support: " + Math.Round(this.Support, 2) + "%)" : string.Empty);
        }

        #endregion
    }
}
