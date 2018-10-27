namespace MarkThree.Guardian.Forms
{

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	public partial class VolumeChartViewer : MarkThree.Forms.Viewer
	{

		private string[] symbols = {"A", "AA", "AACC", "AACE", "AAI", "AAON", "AAP", "AAPL", "ABAX", "ABC", "ABCB", "ABCO", "ABCW", "ABFS"};

		public Bitmap[] priceGraphs;
		public Bitmap[] volumeGraphs;
	
		public VolumeChartViewer()
		{

			InitializeComponent();

			this.priceGraphs = new Bitmap[symbols.Length];
			this.volumeGraphs = new Bitmap[symbols.Length];

			// The images use to identify this object to the user are loaded from the resources in this assembly.
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			int symbolIndex = 0;
			foreach (string symbol in symbols)
			{

				this.priceGraphs[symbolIndex] =
					new Bitmap(assembly.GetManifestResourceStream(string.Format("MarkThree.Guardian.Forms.{0}_1.png", symbol)));
				this.volumeGraphs[symbolIndex] =
					new Bitmap(assembly.GetManifestResourceStream(string.Format("MarkThree.Guardian.Forms.{0}_2.png", symbol)));
				symbolIndex++;

			}

		}

		public override void Open(object tag)
		{
			base.Open(tag);
		}

		/// <summary>
		/// Opens the a block order in the execution viewer.
		/// </summary>
		/// <param name="matchId">The primary identifier of the object to open.</param>
		public void OpenMatch(int matchId)
		{

			int symbolIndex = matchId % symbols.Length;
			this.pictureBox1.Image = this.priceGraphs[symbolIndex];
			this.pictureBox2.Image = this.volumeGraphs[symbolIndex];

		}

		/// <summary>
		/// Closes the a block order in the execution viewer.
		/// </summary>
		/// <param name="matchId">The primary identifier of the object to close.</param>
		public void CloseMatch() { }

	}

}

