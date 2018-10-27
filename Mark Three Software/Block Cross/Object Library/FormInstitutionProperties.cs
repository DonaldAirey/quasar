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

	/// <summary>Property sheet for Institutions.</summary>
	public class FormInstitutionProperties : FormBlotterProperties
	{

		private System.Windows.Forms.TabPage tabPageInstitution;

		/// <summary>
		/// Property sheet for Institutions.
		/// </summary>
		public FormInstitutionProperties()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormInstitutionProperties));
			this.tabPageInstitution = new System.Windows.Forms.TabPage();
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
			this.tabControl.Controls.Add(this.tabPageInstitution);
			this.tabControl.Name = "tabControl";
			this.tabControl.Controls.SetChildIndex(this.tabPageInstitution, 0);
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
			// tabPageInstitution
			// 
			this.tabPageInstitution.AccessibleDescription = resources.GetString("tabPageInstitution.AccessibleDescription");
			this.tabPageInstitution.AccessibleName = resources.GetString("tabPageInstitution.AccessibleName");
			this.tabPageInstitution.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPageInstitution.Anchor")));
			this.tabPageInstitution.AutoScroll = ((bool)(resources.GetObject("tabPageInstitution.AutoScroll")));
			this.tabPageInstitution.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPageInstitution.AutoScrollMargin")));
			this.tabPageInstitution.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPageInstitution.AutoScrollMinSize")));
			this.tabPageInstitution.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageInstitution.BackgroundImage")));
			this.tabPageInstitution.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPageInstitution.Dock")));
			this.tabPageInstitution.Enabled = ((bool)(resources.GetObject("tabPageInstitution.Enabled")));
			this.tabPageInstitution.Font = ((System.Drawing.Font)(resources.GetObject("tabPageInstitution.Font")));
			this.tabPageInstitution.ImageIndex = ((int)(resources.GetObject("tabPageInstitution.ImageIndex")));
			this.tabPageInstitution.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPageInstitution.ImeMode")));
			this.tabPageInstitution.Location = ((System.Drawing.Point)(resources.GetObject("tabPageInstitution.Location")));
			this.tabPageInstitution.Name = "tabPageInstitution";
			this.tabPageInstitution.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPageInstitution.RightToLeft")));
			this.tabPageInstitution.Size = ((System.Drawing.Size)(resources.GetObject("tabPageInstitution.Size")));
			this.tabPageInstitution.TabIndex = ((int)(resources.GetObject("tabPageInstitution.TabIndex")));
			this.tabPageInstitution.Text = resources.GetString("tabPageInstitution.Text");
			this.tabPageInstitution.ToolTipText = resources.GetString("tabPageInstitution.ToolTipText");
			this.tabPageInstitution.Visible = ((bool)(resources.GetObject("tabPageInstitution.Visible")));
			// 
			// FormInstitutionProperties
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
			this.Name = "FormInstitutionProperties";
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
		/// <param name="organization">A organization.</param>
		public void Show(Institution organization)
		{

			try
			{

				// Lock the tables needed for the dialog.
				Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.InstitutionLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Find the organization in the data model.
				ClientMarketData.InstitutionRow institutionRow = ClientMarketData.Institution.FindByInstitutionId(organization.InstitutionId);
				if (institutionRow == null)
					throw new Exception("Some else has deleted this organization.");

				// Fill in the 'General' tab with the generic information.
				this.textBoxName.Text = institutionRow.SourceRow.BlotterRow.ObjectRow.Name;
				this.labelTypeText.Text = institutionRow.SourceRow.BlotterRow.ObjectRow.TypeRow.Description;
				this.textBoxDescription.Text = (institutionRow.SourceRow.BlotterRow.ObjectRow.IsDescriptionNull()) ?
					"" : institutionRow.SourceRow.BlotterRow.ObjectRow.Description;
				this.pictureBox.Image = organization.Image32x32;

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.InstitutionLock.IsReaderLockHeld) ClientMarketData.InstitutionLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.TypeLock.IsReaderLockHeld) ClientMarketData.TypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Display the dialog.
			this.ShowDialog();

		}

	}

}
