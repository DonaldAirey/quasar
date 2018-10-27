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

	/// <summary>Property sheet for Brokers.</summary>
	public class FormBrokerProperties : FormBlotterProperties
	{

		private System.Windows.Forms.TabPage tabPageBroker;

		/// <summary>
		/// Property sheet for Brokers.
		/// </summary>
		public FormBrokerProperties()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormBrokerProperties));
			this.tabPageBroker = new System.Windows.Forms.TabPage();
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
			this.tabControl.Controls.Add(this.tabPageBroker);
			this.tabControl.Name = "tabControl";
			this.tabControl.Controls.SetChildIndex(this.tabPageBroker, 0);
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
			// tabPageBroker
			// 
			this.tabPageBroker.AccessibleDescription = resources.GetString("tabPageBroker.AccessibleDescription");
			this.tabPageBroker.AccessibleName = resources.GetString("tabPageBroker.AccessibleName");
			this.tabPageBroker.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPageBroker.Anchor")));
			this.tabPageBroker.AutoScroll = ((bool)(resources.GetObject("tabPageBroker.AutoScroll")));
			this.tabPageBroker.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPageBroker.AutoScrollMargin")));
			this.tabPageBroker.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPageBroker.AutoScrollMinSize")));
			this.tabPageBroker.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageBroker.BackgroundImage")));
			this.tabPageBroker.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPageBroker.Dock")));
			this.tabPageBroker.Enabled = ((bool)(resources.GetObject("tabPageBroker.Enabled")));
			this.tabPageBroker.Font = ((System.Drawing.Font)(resources.GetObject("tabPageBroker.Font")));
			this.tabPageBroker.ImageIndex = ((int)(resources.GetObject("tabPageBroker.ImageIndex")));
			this.tabPageBroker.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPageBroker.ImeMode")));
			this.tabPageBroker.Location = ((System.Drawing.Point)(resources.GetObject("tabPageBroker.Location")));
			this.tabPageBroker.Name = "tabPageBroker";
			this.tabPageBroker.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPageBroker.RightToLeft")));
			this.tabPageBroker.Size = ((System.Drawing.Size)(resources.GetObject("tabPageBroker.Size")));
			this.tabPageBroker.TabIndex = ((int)(resources.GetObject("tabPageBroker.TabIndex")));
			this.tabPageBroker.Text = resources.GetString("tabPageBroker.Text");
			this.tabPageBroker.ToolTipText = resources.GetString("tabPageBroker.ToolTipText");
			this.tabPageBroker.Visible = ((bool)(resources.GetObject("tabPageBroker.Visible")));
			// 
			// FormBrokerProperties
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
			this.Name = "FormBrokerProperties";
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
		/// <param name="customer">A customer.</param>
		public void Show(Broker customer)
		{

			try
			{

				// Lock the tables needed for the dialog.
				Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BrokerLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SourceLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Find the customer in the data model.
				ClientMarketData.BrokerRow brokerRow = ClientMarketData.Broker.FindByBrokerId(customer.BrokerId);
				if (brokerRow == null)
					throw new Exception("Some else has deleted this customer.");

				// Fill in the 'General' tab with the generic information.
				this.textBoxName.Text = brokerRow.SourceRow.BlotterRow.ObjectRow.Name;
				this.labelTypeText.Text = brokerRow.SourceRow.BlotterRow.ObjectRow.TypeRow.Description;
				this.textBoxDescription.Text = (brokerRow.SourceRow.BlotterRow.ObjectRow.IsDescriptionNull()) ?
					"" : brokerRow.SourceRow.BlotterRow.ObjectRow.Description;
				this.pictureBox.Image = customer.Image32x32;

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.BrokerLock.IsReaderLockHeld)
					ClientMarketData.BrokerLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.TypeLock.IsReaderLockHeld)
					ClientMarketData.TypeLock.ReleaseReaderLock();
				if (ClientMarketData.SourceLock.IsReaderLockHeld)
					ClientMarketData.SourceLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Display the dialog.
			this.ShowDialog();

		}

	}

}
