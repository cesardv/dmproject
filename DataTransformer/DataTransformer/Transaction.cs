namespace DataTransformer
{
    using System;

    /// <summary>
    /// Represents a transaction in a Database
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class by reading in a CSV row, which has receipt# on first column, and items next
        /// </summary>
        /// <param name="trans">The array of the transaction</param>
        public Transaction(string[] trans)
        {
            // set the Ids:
            this.Tid = Convert.ToInt32(trans[0]);

            // this.Guid = Guid.NewGuid();
            this.Items = new int[trans.Length - 1];

            for (var i = 0; i < trans.Length; i++)
            {
                if (Array.IndexOf(trans, trans[i]) != 0)
                {
                    this.Items[i - 1] = Convert.ToInt32(trans[i]);
                }
            }
        }

        /// <summary>
        /// Gets or sets the 'simple' Id of this
        /// </summary>
        public int Tid { get; set; }

        /// <summary>
        /// Gets or sets the items integer array
        /// </summary>
        public int[] Items { get; set; }

        /// <summary>
        /// Gets or sets a long Id
        /// </summary>
        // public Guid Guid { get; set; } // MongoDb adds this as a regular field, and add ObjectID by default
    }
}
