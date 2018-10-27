namespace MarkThree.Tools
{

	using System;
	using System.Configuration;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using System.Windows.Forms;

	public partial class FormMain : Form
	{
		public FormMain()
		{
			InitializeComponent();
		}

		private void serializeToolStripMenuItem_Click(object sender, EventArgs e)
		{

			this.openFileDialog.InitialDirectory = ConfigurationManager.AppSettings["path"];

			if (this.openFileDialog.ShowDialog() == DialogResult.OK)
			{

				Bitmap bitmap = new Bitmap(this.openFileDialog.FileName);
				MemoryStream memoryStream = new MemoryStream();
				bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
				this.textBox.Text = Convert.ToBase64String(memoryStream.GetBuffer());

			}

		}
	}
}