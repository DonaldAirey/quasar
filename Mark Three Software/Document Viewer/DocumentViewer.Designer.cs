namespace MarkThree.Forms
{
	partial class DocumentViewer
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

			if (disposing)
			{
				
				if (components != null)
					components.Dispose();

				if (this.IsBackgroundThreadRunning)
				{
					this.IsBackgroundThreadRunning = false;
					if (!this.backgroundThread.Join(DocumentViewer.maxThreadWait))
						this.backgroundThread.Abort();
				}

			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel = new System.Windows.Forms.Panel();
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// panel
			// 
			this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.panel.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.panel.Location = new System.Drawing.Point(495, 239);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(17, 17);
			this.panel.TabIndex = 1;
			// 
			// hScrollBar
			// 
			this.hScrollBar.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.hScrollBar.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.hScrollBar.Location = new System.Drawing.Point(0, 239);
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(495, 17);
			this.hScrollBar.TabIndex = 2;
			this.hScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar_Scroll);
			// 
			// vScrollBar
			// 
			this.vScrollBar.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.vScrollBar.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.vScrollBar.Location = new System.Drawing.Point(495, 0);
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(17, 239);
			this.vScrollBar.TabIndex = 3;
			this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar_Scroll);
			// 
			// DocumentViewer
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.vScrollBar);
			this.Controls.Add(this.hScrollBar);
			this.Controls.Add(this.panel);
			this.Name = "DocumentViewer";
			this.Size = new System.Drawing.Size(512, 256);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.HScrollBar hScrollBar;
		private System.Windows.Forms.VScrollBar vScrollBar;

	}
}
