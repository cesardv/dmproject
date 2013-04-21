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

    using MongoDB.Driver;

    /// <summary>
    /// This is the GUI of the Connection Screen which aids user in connecting to mongo
    /// </summary>
    public partial class ConnectionGUI : Form
    {
        /// <summary>
        /// Get or sets the name of the selected MongoDb
        /// </summary>
        public string SelectedDbName { get; set; }

        public FormGUI MainForm
        {
            get
            {
                return this.Owner as FormGUI;
            }
        }

        public ConnectionGUI()
        {
            InitializeComponent();
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            //clear and hide error msg
            this.errorLbl.Text = string.Empty;
            this.errorLbl.Visible = false;

            if (string.IsNullOrWhiteSpace(connStrTbox.Text))
            {
                this.errorLbl.Text = "Connection String Invalid";
                this.errorLbl.Visible = true;
            }

            MongoClient client;
            try
            {
                client = new MongoClient(this.connStrTbox.Text);
                
                // You can store the client object in a global variable. MongoClient is thread-safe.
                if (this.MainForm != null)
                {
                    this.MainForm.MongoClient = client;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DB Connection Error");
                return;
            }

            MessageBox.Show(this, "Success connecting! Obtaining Server DBs now...", "Status");

            try
            {
                var server = client.GetServer();
                var DBs = server.GetDatabaseNames();
                this.dbsCBx.DataSource = DBs;

            }
            catch (Exception x)
            {
                MessageBox.Show(this, x.Message, "Error");
            }
        }

        private void dbsCBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedDbName = dbsCBx.Text;
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.SelectedDbName))
            {
                MessageBox.Show(this, "Please select a database from the menu", "No Database Selected");
                return;
            }

            this.MainForm.DbName = this.SelectedDbName;
            this.Close();
        }
    }
}
