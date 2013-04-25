// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssociationRule.cs" company="CesarVelez">
//   Cesar Velez - Copyright 2013
// </copyright>
// <summary>
//   Represents an association rule
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AprioriMiner.Models
{
    using System;

    /// <summary>
    /// Represents an association rule
    /// </summary>
    public class AssociationRule
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationRule"/> class
        /// </summary>
        public AssociationRule()
        {
            this.X = new Itemset();
            this.Y = new Itemset();
            this.Support = 0.0;
            this.Confidence = 0.0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Left (L) side of the rule
        /// </summary>
        public Itemset X { get; set; }

        /// <summary>
        /// Gets or sets the Right (R) side of the rule
        /// </summary>
        public Itemset Y { get; set; }

        /// <summary>
        /// Gets or sets minimum support for this association rule. The proportion of the database for which rules applies to.
        /// </summary>
        public double Support { get; set; }

        /// <summary>
        /// Gets or sets the Confidence for this rule.
        /// The predictive accuracy of the rule, defined as the proportion of transaction for which rule is satisfied.
        /// Confidence(L->R) = count(LuR)/count(L) = support(LuR)/support(L)
        /// </summary>
        public double Confidence { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// ToString for a rule
        /// </summary>
        /// <returns>A string of the rule</returns>
        public override string ToString()
        {
            return this.X + " => " + this.Y + " (support: " + Math.Round(this.Support, 2) + "%, confidence: " + Math.Round(this.Confidence, 2) + "%)";
        }

        #endregion
    }
}
