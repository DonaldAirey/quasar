namespace MarkThree.MiddleTier.Administrator
{
	partial class FormAdministrator
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
			if (disposing && (components != null))
			{
				components.Dispose();
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.administerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.accountMappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.administerToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(292, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// administerToolStripMenuItem
			// 
			this.administerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.accountMappingsToolStripMenuItem});
			this.administerToolStripMenuItem.Name = "administerToolStripMenuItem";
			this.administerToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
			this.administerToolStripMenuItem.Text = "&Administer";
			// 
			// accountMappingsToolStripMenuItem
			// 
			this.accountMappingsToolStripMenuItem.Name = "accountMappingsToolStripMenuItem";
			this.accountMappingsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.accountMappingsToolStripMenuItem.Text = "&Account Mappings";
			this.accountMappingsToolStripMenuItem.Click += new System.EventHandler(this.accountMappingsToolStripMenuItem_Click);
			// 
			// FormMiddleTierAdministrator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "FormMiddleTierAdministrator";
			this.Text = "Middle Tier Administrator";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem administerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem accountMappingsToolStripMenuItem;
	}
}

