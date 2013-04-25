// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Bit.cs" company="CesarVelez">
//   Cesar Velez - Copyright 2013
// </copyright>
// <summary>
//   Bit class finds the 2^N subsets of an Itemset with Cardinality N using logical/bitwise AND operator (&)
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AprioriMiner.Models
{
    using System;
    using System.Linq;

    /// <summary>
    /// Bit class finds the 2^N subsets of an Itemset with Cardinality N using logical/bitwise AND operator (&)
    /// </summary>
    public class Bit
    {
        public static ItemsetCollection FindSubsets(Itemset itemset, int n)
        {
            var subsets = new ItemsetCollection();

            int subsetCount = (int)Math.Pow(2, itemset.Count);

            for (int i = 0; i < subsetCount; i++)
            {
                if (n == 0 || GetOnCount(i, itemset.Count) == n)
                {
                    string binary = DecimalToBinary(i, itemset.Count);

                    Itemset subset = new Itemset();

                    for (int charIndex = 0; charIndex < binary.Length; charIndex++)
                    {
                        if (binary[charIndex] == '1')
                        {
                            subset.Add(itemset[charIndex]);
                        }
                    }

                    subsets.Add(subset);
                }
            }

            return subsets;
        }

        public static int GetBit(int value, int position)
        {
            int bit = value & (int)Math.Pow(2, position);
            return bit > 0 ? 1 : 0;
        }

        public static string DecimalToBinary(int value, int length)
        {
            var binary = string.Empty;
            for (int position = 0; position < length; position++)
            {
                binary = GetBit(value, position) + binary;
            }
            return binary;
        }

        public static int GetOnCount(int value, int length)
        {
            string binary = DecimalToBinary(value, length);
            return (from char c in binary.ToCharArray()
                    where c == '1'
                    select c).Count();
        }
    }
}
