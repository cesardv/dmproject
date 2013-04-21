namespace BookmarkerDB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using MongoDB.Bson;
    using MongoDB.Driver;

    /// <summary>
    /// Main Form
    /// </summary>
    public partial class FormGUI : Form
    {

        /// <summary>
        /// Gets or sets the connection Status to mongoDB
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// Gets or sets the mongoDB client
        /// </summary>
        public MongoClient MongoClient { get; set; }

        /// <summary>
        /// Gets the server of the current client
        /// </summary>
        public MongoServer MongoServer
        {
            get
            {
                return MongoClient.GetServer();
            }
        }

        /// <summary>
        /// Gets or sets the name of the database which we're connected to
        /// </summary>
        public string DbName { get; set; }

        public FormGUI()
        {
            this.InitializeComponent();
        }

        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ConnectToMongo();
        }

        private void FormGUI_Load(object sender, EventArgs e)
        {
            //if (!this.Connected)
            //{
            //    // MessageBox.Show("Not connected to any mongoDB database collection. Click OK to attempt connection.");
            //    this.ConnectToMongo();
            //}
        }

        private void ConnectToMongo()
        {
            var connForm = new ConnectionGUI();
            connForm.Closed += (sender, args) =>
            {
                this.DbName = connForm.SelectedDbName;
                var db = this.MongoServer.GetDatabase(this.DbName);

                this.InsertCollectionNamesMenu.DataSource = db.GetCollectionNames();
            };

            connForm.ShowDialog(this);

            if (this.MongoClient != null)
            {
                // MessageBox.Show(this, "We're connected!");
                toolStripStatusLabel1.Text = string.Format("Connected to \"{0}\" Database.", this.DbName);
            }
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
