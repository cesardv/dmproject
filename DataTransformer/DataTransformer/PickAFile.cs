namespace DataTransformer
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

    public partial class PickAFile : Form
    {

        /// <summary>
        /// Gets or sets the path of selected file
        /// </summary>
        public string SelectedFilePath { get; set; }


        public PickAFile()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Starts file browser
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event arguments</param>
        private void browseBtn_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.FileOk += (o, args) => this.UpdateTbx(this.openFileDialog1.FileName);
            this.openFileDialog1.ShowDialog(this);
            
        }

        /// <summary>
        /// Updates the file textbox with selected files full path
        /// </summary>
        /// <param name="fn"></param>
        private void UpdateTbx(string fn)
        {
            this.filepathTbx.Text = fn;
        }

        /// <summary>
        /// Updates the public property of SelectedFilePath of this form
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event arguments</param>
        private void filepathTbx_TextChanged(object sender, EventArgs e)
        {
            this.SelectedFilePath = this.filepathTbx.Text;
        }

        /// <summary>
        /// Just closes the dialog window
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">event arguments</param>
        private void OkBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
