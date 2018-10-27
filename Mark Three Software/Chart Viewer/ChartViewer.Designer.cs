namespace MarkThree.Forms
{
	partial class ChartViewer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{

			try
			{

				if (disposing && (components != null))
				{
					components.Dispose();
				}

				base.Dispose(disposing);

			}
			catch
			{

				// There is a bug with the ActiveX controls.  If the control is never made visible, then it has no window handle 
				// which is apparely needed to destroy it.  This creates an exception, which wouldn't be too bad because the
				// exeption is only taken when the application is behing destroyed.  The problem is that, apparently, when you take
				// an exception on a 'Dispose' operation, the OS keeps on trying until it is successful.  This leaves the
				// application in a zombie like state: not quite alive, not dead either.

			}

		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartViewer));
			this.axChartSpace = new AxOWC11.AxChartSpace();
			((System.ComponentModel.ISupportInitialize)(this.axChartSpace)).BeginInit();
			this.SuspendLayout();
			// 
			// axChartSpace
			// 
			this.axChartSpace.DataSource = null;
			this.axChartSpace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.axChartSpace.Enabled = true;
			this.axChartSpace.Location = new System.Drawing.Point(0, 0);
			this.axChartSpace.Name = "axChartSpace";
			this.axChartSpace.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axChartSpace.OcxState")));
			this.axChartSpace.Size = new System.Drawing.Size(321, 220);
			this.axChartSpace.TabIndex = 0;
			// 
			// ChartViewer
			// 
			this.Controls.Add(this.axChartSpace);
			this.Name = "ChartViewer";
			this.Size = new System.Drawing.Size(321, 220);
			((System.ComponentModel.ISupportInitialize)(this.axChartSpace)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		protected AxOWC11.AxChartSpace axChartSpace;

	}
}
