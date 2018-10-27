/*************************************************************************************************************************
*
*	File:			OrderEntryDialog.cs
*	Description:	This control is used input orders quickly.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Common.Controls
{

	using AxMicrosoft.Office.Interop.Owc11;
	using Shadows.Quasar.Client;
	using Shadows.Quasar.Common;
	using Shadows.Quasar.Viewers;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using System.Threading;
	using System.Web.Services.Protocols;
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Xml.XPath;
	using System.Reflection;

	/// <summary>
	/// Summary description for OrderEntryDialog.
	/// </summary>
	public class OrderEntryDialog : System.Windows.Forms.Form
	{

		private System.Windows.Forms.Button symbolLookup;
		private System.Windows.Forms.Button help;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button ok;
		private Shadows.Quasar.Viewers.Order.OrderViewer orderViewer;
		private System.ComponentModel.Container components = null;

		public OrderEntryDialog(int blotterId)
		{

			// Required for Windows Form Designer support
			InitializeComponent();

			this.orderViewer.OpenViewer();
			this.orderViewer.OpenDocument(blotterId);

		}

		#region Dispose Method
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cancel = new System.Windows.Forms.Button();
			this.ok = new System.Windows.Forms.Button();
			this.symbolLookup = new System.Windows.Forms.Button();
			this.help = new System.Windows.Forms.Button();
			this.orderViewer = new Shadows.Quasar.Viewers.Order.OrderViewer();
			this.SuspendLayout();
			// 
			// cancel
			// 
			this.cancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(332, 200);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(69, 23);
			this.cancel.TabIndex = 1;
			this.cancel.Text = "Cancel";
			// 
			// ok
			// 
			this.ok.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.ok.Location = new System.Drawing.Point(244, 200);
			this.ok.Name = "ok";
			this.ok.Size = new System.Drawing.Size(68, 23);
			this.ok.TabIndex = 2;
			this.ok.Text = "OK";
			this.ok.Click += new System.EventHandler(this.ok_Click);
			// 
			// symbolLookup
			// 
			this.symbolLookup.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.symbolLookup.Location = new System.Drawing.Point(140, 200);
			this.symbolLookup.Name = "symbolLookup";
			this.symbolLookup.Size = new System.Drawing.Size(88, 23);
			this.symbolLookup.TabIndex = 3;
			this.symbolLookup.Text = "Symol Lookup";
			// 
			// help
			// 
			this.help.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.help.Location = new System.Drawing.Point(417, 200);
			this.help.Name = "help";
			this.help.Size = new System.Drawing.Size(68, 23);
			this.help.TabIndex = 4;
			this.help.Text = "Help";
			// 
			// orderViewer
			// 
			this.orderViewer.BlockOrderId = 0;
			this.orderViewer.BlotterId = 0;
			this.orderViewer.DocumentVersion = 0;
			this.orderViewer.IsLedger = false;
			this.orderViewer.IsRefreshNeeded = true;
			this.orderViewer.IsStylesheetChanged = true;
			this.orderViewer.Location = new System.Drawing.Point(8, 8);
			this.orderViewer.MoveAfterReturn = true;
			this.orderViewer.Name = "orderViewer";
			this.orderViewer.ResetViewer = true;
			this.orderViewer.Size = new System.Drawing.Size(488, 184);
			this.orderViewer.StylesheetId = 0;
			this.orderViewer.TabIndex = 0;
			// 
			// OrderEntryDialog
			// 
			this.AcceptButton = this.ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(504, 229);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.orderViewer,
																		  this.help,
																		  this.symbolLookup,
																		  this.ok,
																		  this.cancel});
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OrderEntryDialog";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Equity Order";
			this.ResumeLayout(false);

		}
		#endregion

		public LocalOrderSet LocalOrderSet {get {return this.orderViewer.LocalOrderSet;}}
		
		private void ok_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;

			Close();
		}

	}

}
