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

	/// <summary>Property sheet for Branches.</summary>
	public class FormBranchProperties : FormBlotterProperties
	{

		private System.Windows.Forms.TabPage tabPageBranch;

		/// <summary>
		/// Property sheet for Branches.
		/// </summary>
		public FormBranchProperties()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormBranchProperties));
			this.tabPageBranch = new System.Windows.Forms.TabPage();
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
			this.tabControl.Controls.Add(this.tabPageBranch);
			this.tabControl.Name = "tabControl";
			this.tabControl.Controls.SetChildIndex(this.tabPageBranch, 0);
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
			// tabPageBranch
			// 
			this.tabPageBranch.AccessibleDescription = resources.GetString("tabPageBranch.AccessibleDescription");
			this.tabPageBranch.AccessibleName = resources.GetString("tabPageBranch.AccessibleName");
			this.tabPageBranch.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPageBranch.Anchor")));
			this.tabPageBranch.AutoScroll = ((bool)(resources.GetObject("tabPageBranch.AutoScroll")));
			this.tabPageBranch.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPageBranch.AutoScrollMargin")));
			this.tabPageBranch.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPageBranch.AutoScrollMinSize")));
			this.tabPageBranch.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageBranch.BackgroundImage")));
			this.tabPageBranch.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPageBranch.Dock")));
			this.tabPageBranch.Enabled = ((bool)(resources.GetObject("tabPageBranch.Enabled")));
			this.tabPageBranch.Font = ((System.Drawing.Font)(resources.GetObject("tabPageBranch.Font")));
			this.tabPageBranch.ImageIndex = ((int)(resources.GetObject("tabPageBranch.ImageIndex")));
			this.tabPageBranch.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPageBranch.ImeMode")));
			this.tabPageBranch.Location = ((System.Drawing.Point)(resources.GetObject("tabPageBranch.Location")));
			this.tabPageBranch.Name = "tabPageBranch";
			this.tabPageBranch.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPageBranch.RightToLeft")));
			this.tabPageBranch.Size = ((System.Drawing.Size)(resources.GetObject("tabPageBranch.Size")));
			this.tabPageBranch.TabIndex = ((int)(resources.GetObject("tabPageBranch.TabIndex")));
			this.tabPageBranch.Text = resources.GetString("tabPageBranch.Text");
			this.tabPageBranch.ToolTipText = resources.GetString("tabPageBranch.ToolTipText");
			this.tabPageBranch.Visible = ((bool)(resources.GetObject("tabPageBranch.Visible")));
			// 
			// FormBranchProperties
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
			this.Name = "FormBranchProperties";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.tabPageGeneral.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Shows a property dialog for managing collections of traders.
		/// </summary>
		/// <param name="branch">A branch.</param>
		public void Show(Branch branch)
		{

			try
			{

				// Lock the tables needed for the dialog.
				Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BranchLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Find the branch in the data model.
				ClientMarketData.BranchRow branchRow = ClientMarketData.Branch.FindByBranchId(branch.BranchId);
				if (branchRow == null)
					throw new Exception("Some else has deleted this branch.");

				// Fill in the 'General' tab with the generic information.
				this.textBoxName.Text = branchRow.BlotterRow.ObjectRow.Name;
				this.labelTypeText.Text = branchRow.BlotterRow.ObjectRow.TypeRow.Description;
				this.textBoxDescription.Text = (branchRow.BlotterRow.ObjectRow.IsDescriptionNull()) ?
					"" : branchRow.BlotterRow.ObjectRow.Description;
				this.pictureBox.Image = branch.Image32x32;

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BranchLock.IsReaderLockHeld) ClientMarketData.BranchLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.TypeLock.IsReaderLockHeld) ClientMarketData.TypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Display the dialog.
			this.ShowDialog();

		}

	}

}
