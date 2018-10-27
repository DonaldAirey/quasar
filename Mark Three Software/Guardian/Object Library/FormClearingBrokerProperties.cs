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

	/// <summary>Property sheet for ClearingBrokers.</summary>
	public class FormClearingBrokerProperties : FormBlotterProperties
	{

		private System.Windows.Forms.TabPage tabPageClearingBroker;

		/// <summary>
		/// Property sheet for ClearingBrokers.
		/// </summary>
		public FormClearingBrokerProperties()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormClearingBrokerProperties));
			this.tabPageClearingBroker = new System.Windows.Forms.TabPage();
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
			this.tabControl.Controls.Add(this.tabPageClearingBroker);
			this.tabControl.Name = "tabControl";
			this.tabControl.Controls.SetChildIndex(this.tabPageClearingBroker, 0);
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
			// tabPageClearingBroker
			// 
			this.tabPageClearingBroker.AccessibleDescription = resources.GetString("tabPageClearingBroker.AccessibleDescription");
			this.tabPageClearingBroker.AccessibleName = resources.GetString("tabPageClearingBroker.AccessibleName");
			this.tabPageClearingBroker.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPageClearingBroker.Anchor")));
			this.tabPageClearingBroker.AutoScroll = ((bool)(resources.GetObject("tabPageClearingBroker.AutoScroll")));
			this.tabPageClearingBroker.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPageClearingBroker.AutoScrollMargin")));
			this.tabPageClearingBroker.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPageClearingBroker.AutoScrollMinSize")));
			this.tabPageClearingBroker.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageClearingBroker.BackgroundImage")));
			this.tabPageClearingBroker.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPageClearingBroker.Dock")));
			this.tabPageClearingBroker.Enabled = ((bool)(resources.GetObject("tabPageClearingBroker.Enabled")));
			this.tabPageClearingBroker.Font = ((System.Drawing.Font)(resources.GetObject("tabPageClearingBroker.Font")));
			this.tabPageClearingBroker.ImageIndex = ((int)(resources.GetObject("tabPageClearingBroker.ImageIndex")));
			this.tabPageClearingBroker.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPageClearingBroker.ImeMode")));
			this.tabPageClearingBroker.Location = ((System.Drawing.Point)(resources.GetObject("tabPageClearingBroker.Location")));
			this.tabPageClearingBroker.Name = "tabPageClearingBroker";
			this.tabPageClearingBroker.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPageClearingBroker.RightToLeft")));
			this.tabPageClearingBroker.Size = ((System.Drawing.Size)(resources.GetObject("tabPageClearingBroker.Size")));
			this.tabPageClearingBroker.TabIndex = ((int)(resources.GetObject("tabPageClearingBroker.TabIndex")));
			this.tabPageClearingBroker.Text = resources.GetString("tabPageClearingBroker.Text");
			this.tabPageClearingBroker.ToolTipText = resources.GetString("tabPageClearingBroker.ToolTipText");
			this.tabPageClearingBroker.Visible = ((bool)(resources.GetObject("tabPageClearingBroker.Visible")));
			// 
			// FormClearingBrokerProperties
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
			this.Name = "FormClearingBrokerProperties";
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
		public void Show(ClearingBroker organization)
		{

			try
			{

				// Lock the tables needed for the dialog.
				Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ClearingBrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Find the organization in the data model.
				ClientMarketData.ClearingBrokerRow clearingBrokerRow = ClientMarketData.ClearingBroker.FindByClearingBrokerId(organization.ClearingBrokerId);
				if (clearingBrokerRow == null)
					throw new Exception("Some else has deleted this organization.");

				// Fill in the 'General' tab with the generic information.
				this.textBoxName.Text = clearingBrokerRow.BrokerRow.SourceRow.BlotterRow.ObjectRow.Name;
				this.labelTypeText.Text = clearingBrokerRow.BrokerRow.SourceRow.BlotterRow.ObjectRow.TypeRow.Description;
				this.textBoxDescription.Text = (clearingBrokerRow.BrokerRow.SourceRow.BlotterRow.ObjectRow.IsDescriptionNull()) ?
					"" : clearingBrokerRow.BrokerRow.SourceRow.BlotterRow.ObjectRow.Description;
				this.pictureBox.Image = organization.Image32x32;

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld)
					ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ClearingBrokerLock.IsReaderLockHeld)
					ClientMarketData.ClearingBrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.TypeLock.IsReaderLockHeld)
					ClientMarketData.TypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Display the dialog.
			this.ShowDialog();

		}

	}

}
