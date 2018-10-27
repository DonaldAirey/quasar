namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for FormBlotterProperties.
	/// </summary>
	public class FormBlotterProperties : FormObjectProperties
	{
		private System.Windows.Forms.TabPage tabPageBlotter;

		/// <summary>
		/// Dialog box for maintaining blotters.
		/// </summary>
		public FormBlotterProperties()
		{

			// Initializer for the components managed by the designer.
			InitializeComponent();

		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormBlotterProperties));
			this.tabPageBlotter = new System.Windows.Forms.TabPage();
			this.tabPageGeneral.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Name = "buttonOK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Name = "buttonCancel";
			// 
			// buttonApply
			// 
			this.buttonApply.Name = "buttonApply";
			// 
			// tabPageGeneral
			// 
			this.tabPageGeneral.Name = "tabPageGeneral";
			// 
			// label3
			// 
			this.label3.Name = "label3";
			// 
			// label2
			// 
			this.label2.Name = "label2";
			// 
			// checkBoxDeleted
			// 
			this.checkBoxDeleted.Name = "checkBoxDeleted";
			// 
			// labelAttributes
			// 
			this.labelAttributes.Name = "labelAttributes";
			// 
			// labelTypeText
			// 
			this.labelTypeText.Name = "labelTypeText";
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.Name = "textBoxDescription";
			// 
			// label1
			// 
			this.label1.Name = "label1";
			// 
			// labelTypePrompt
			// 
			this.labelTypePrompt.Name = "labelTypePrompt";
			// 
			// textBoxName
			// 
			this.textBoxName.Name = "textBoxName";
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageBlotter);
			this.tabControl.Name = "tabControl";
			this.tabControl.Controls.SetChildIndex(this.tabPageBlotter, 0);
			this.tabControl.Controls.SetChildIndex(this.tabPageGeneral, 0);
			// 
			// pictureBox
			// 
			this.pictureBox.Name = "pictureBox";
			// 
			// buttonHelp
			// 
			this.buttonHelp.Name = "buttonHelp";
			// 
			// tabPageBlotter
			// 
			this.tabPageBlotter.AccessibleDescription = resources.GetString("tabPageBlotter.AccessibleDescription");
			this.tabPageBlotter.AccessibleName = resources.GetString("tabPageBlotter.AccessibleName");
			this.tabPageBlotter.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPageBlotter.Anchor")));
			this.tabPageBlotter.AutoScroll = ((bool)(resources.GetObject("tabPageBlotter.AutoScroll")));
			this.tabPageBlotter.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPageBlotter.AutoScrollMargin")));
			this.tabPageBlotter.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPageBlotter.AutoScrollMinSize")));
			this.tabPageBlotter.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageBlotter.BackgroundImage")));
			this.tabPageBlotter.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPageBlotter.Dock")));
			this.tabPageBlotter.Enabled = ((bool)(resources.GetObject("tabPageBlotter.Enabled")));
			this.tabPageBlotter.Font = ((System.Drawing.Font)(resources.GetObject("tabPageBlotter.Font")));
			this.tabPageBlotter.ImageIndex = ((int)(resources.GetObject("tabPageBlotter.ImageIndex")));
			this.tabPageBlotter.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPageBlotter.ImeMode")));
			this.tabPageBlotter.Location = ((System.Drawing.Point)(resources.GetObject("tabPageBlotter.Location")));
			this.tabPageBlotter.Name = "tabPageBlotter";
			this.tabPageBlotter.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPageBlotter.RightToLeft")));
			this.tabPageBlotter.Size = ((System.Drawing.Size)(resources.GetObject("tabPageBlotter.Size")));
			this.tabPageBlotter.TabIndex = ((int)(resources.GetObject("tabPageBlotter.TabIndex")));
			this.tabPageBlotter.Text = resources.GetString("tabPageBlotter.Text");
			this.tabPageBlotter.ToolTipText = resources.GetString("tabPageBlotter.ToolTipText");
			this.tabPageBlotter.Visible = ((bool)(resources.GetObject("tabPageBlotter.Visible")));
			// 
			// FormBlotterProperties
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "FormBlotterProperties";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.tabPageGeneral.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Shows a dialog box for maintaining an blotter or fund.
		/// </summary>
		/// <param name="blotterId">Primary identifier for the blotter.</param>
		public virtual void Show(Blotter blotter)
		{

			try
			{

				// Make sure locks are not nested.
				Debug.Assert(!ClientMarketData.IsLocked);

				// Lock the tables needed for the dialog.
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Find the blotter in the data model.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(blotter.BlotterId);
				if (blotterRow == null)
					throw new Exception("Some else has deleted this blotter.");

				// General Tab
				this.textBoxName.Text = blotterRow.ObjectRow.Name;
				this.labelTypeText.Text = blotterRow.ObjectRow.TypeRow.Description;
				this.textBoxDescription.Text = (blotterRow.ObjectRow.IsDescriptionNull()) ?
					"" : blotterRow.ObjectRow.Description;
				this.pictureBox.Image = blotter.Image32x32;

			}
			finally
			{

				// Release the tables used for the dialog.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.TypeLock.IsReaderLockHeld) ClientMarketData.TypeLock.ReleaseReaderLock();

				// Make sure all locks have been released
				Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Display the dialog.
			this.ShowDialog();

		}

	}

}
