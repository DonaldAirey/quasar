/*************************************************************************************************************************
*
*	File:			AccountViewer.cs
*	Description:	This control is used to display and manage a account.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.Account
{

	using Shadows.Quasar.Client;
	using Shadows.Quasar.Common;
	using Shadows.Quasar.Common.Controls;
	using Shadows.Quasar.Viewers;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.IO;
	using System.Threading;
	using System.Windows.Forms;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Xml.XPath;
	using System.Reflection;

	/// <summary>
	/// A Collection of Viewers use to input and manage trade data.
	/// </summary>
	public class AccountViewer : Shadows.Quasar.Viewers.Viewer
	{

		private bool hasAppraisalViewer;
		private int accountId;
		private System.Windows.Forms.ToolBar toolBarStandard;
		private System.Windows.Forms.ToolBarButton toolBarButtonQuasarToday;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButtonPrint;
		private System.Windows.Forms.ToolBarButton toolBarButtonMoveToFolder;
		private System.Windows.Forms.ToolBarButton toolBarButtonDelete;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButtonCut;
		private System.Windows.Forms.ToolBarButton toolBarButtonCopy;
		private System.Windows.Forms.ToolBarButton toolBarButtonPaste;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButtonRebalance;
		private System.Windows.Forms.ToolBarButton toolBarButtonClearProposed;
		private System.Windows.Forms.ToolBarButton toolBarButtonSend;
		private System.Windows.Forms.ToolBarButton toolBarButton4;
		private System.Windows.Forms.ToolBarButton toolBarButtonHelp;
		private System.Windows.Forms.ToolBarButton toolBarButtonRebalanceSelection;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageAppraisal;
		private System.Windows.Forms.TabPage tabPageSector;
		private System.Windows.Forms.TabPage tabPageRisk;
		private System.EventHandler endOpenEventHandler;
		private Shadows.Quasar.Viewers.Appraisal.AppraisalViewer appraisalViewer;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemSaveAppraisal;
		private System.Windows.Forms.MenuItem menuItemAction;
		private System.Windows.Forms.MenuItem menuItemCheckCompliance;
		private System.Windows.Forms.MenuItem menuItemRebalance;
		private System.Windows.Forms.MenuItem menuItemSendOrders;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItemLast;
		private System.Windows.Forms.MenuItem menuItemClose;
		private System.Windows.Forms.ImageList imageListStandard;
		private System.ComponentModel.IContainer components = null;

		// Thread safe access to the account id.
		[Browsable(false)]
		public int AccountId
		{
			get {lock (this) return this.accountId;}
			set {lock (this) this.accountId = value;}
		}

		/// <summary>
		/// Public access for the Viewer's menus.
		/// </summary>
		[Browsable(false)]
		public override Menu Menu
		{
			get {return this.contextMenu;}
		}

		/// <summary>
		/// Public access for the Viewer's Toolbars.
		/// </summary>
		[Browsable(false)]
		public override ToolBar ToolBarStandard
		{
			get {return this.toolBarStandard;}
		}

		/// <summary>
		/// Constructor for the AccountViewer
		/// </summary>
		public AccountViewer()
		{

			try
			{

				// Call the IDE supplied initialization.
				InitializeComponent();

				// Update the menu items to reflect the state of the user's pricing peference.
				this.menuItemLast.Checked = (ClientPreferences.Pricing == Pricing.Last) ? true : false;
				this.menuItemClose.Checked = (ClientPreferences.Pricing == Pricing.Close) ? true : false;
				
				// This will eventually be driven by a configuration setting.  For now, all account viewers assume that an
				// appraisal viewer is available.
				this.hasAppraisalViewer = true;
				
				// This delegate is called from a background thread when all the child viewers have completed their 'Open' commands.
				this.endOpenEventHandler = new System.EventHandler(EndOpenForeground);

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

		#region Dispose method
		/// <summary>
		/// Releases managed resources when the ApprasalViewer is destroyed.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being destroyed</param>
		protected override void Dispose(bool disposing)
		{

			// Remove any components that have been added in.
			if (disposing && components != null)
				components.Dispose();

			// Call the base class to remove the rest of the resources.
			base.Dispose(disposing);

		}
		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AccountViewer));
			this.toolBarStandard = new System.Windows.Forms.ToolBar();
			this.toolBarButtonQuasarToday = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonPrint = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonMoveToFolder = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonDelete = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonCut = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonCopy = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonPaste = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonClearProposed = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonRebalance = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonRebalanceSelection = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonSend = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton4 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonHelp = new System.Windows.Forms.ToolBarButton();
			this.imageListStandard = new System.Windows.Forms.ImageList(this.components);
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageAppraisal = new System.Windows.Forms.TabPage();
			this.appraisalViewer = new Shadows.Quasar.Viewers.Appraisal.AppraisalViewer();
			this.tabPageSector = new System.Windows.Forms.TabPage();
			this.tabPageRisk = new System.Windows.Forms.TabPage();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItemFile = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemSaveAppraisal = new System.Windows.Forms.MenuItem();
			this.menuItemAction = new System.Windows.Forms.MenuItem();
			this.menuItemCheckCompliance = new System.Windows.Forms.MenuItem();
			this.menuItemRebalance = new System.Windows.Forms.MenuItem();
			this.menuItemSendOrders = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItemLast = new System.Windows.Forms.MenuItem();
			this.menuItemClose = new System.Windows.Forms.MenuItem();
			this.tabControl.SuspendLayout();
			this.tabPageAppraisal.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolBarStandard
			// 
			this.toolBarStandard.AccessibleDescription = resources.GetString("toolBarStandard.AccessibleDescription");
			this.toolBarStandard.AccessibleName = resources.GetString("toolBarStandard.AccessibleName");
			this.toolBarStandard.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("toolBarStandard.Anchor")));
			this.toolBarStandard.Appearance = ((System.Windows.Forms.ToolBarAppearance)(resources.GetObject("toolBarStandard.Appearance")));
			this.toolBarStandard.AutoSize = ((bool)(resources.GetObject("toolBarStandard.AutoSize")));
			this.toolBarStandard.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("toolBarStandard.BackgroundImage")));
			this.toolBarStandard.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																							   this.toolBarButtonQuasarToday,
																							   this.toolBarButton1,
																							   this.toolBarButtonPrint,
																							   this.toolBarButtonMoveToFolder,
																							   this.toolBarButtonDelete,
																							   this.toolBarButton2,
																							   this.toolBarButtonCut,
																							   this.toolBarButtonCopy,
																							   this.toolBarButtonPaste,
																							   this.toolBarButton3,
																							   this.toolBarButtonClearProposed,
																							   this.toolBarButtonRebalance,
																							   this.toolBarButtonRebalanceSelection,
																							   this.toolBarButtonSend,
																							   this.toolBarButton4,
																							   this.toolBarButtonHelp});
			this.toolBarStandard.ButtonSize = ((System.Drawing.Size)(resources.GetObject("toolBarStandard.ButtonSize")));
			this.toolBarStandard.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("toolBarStandard.Dock")));
			this.toolBarStandard.DropDownArrows = ((bool)(resources.GetObject("toolBarStandard.DropDownArrows")));
			this.toolBarStandard.Enabled = ((bool)(resources.GetObject("toolBarStandard.Enabled")));
			this.toolBarStandard.Font = ((System.Drawing.Font)(resources.GetObject("toolBarStandard.Font")));
			this.toolBarStandard.ImageList = this.imageListStandard;
			this.toolBarStandard.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("toolBarStandard.ImeMode")));
			this.toolBarStandard.Location = ((System.Drawing.Point)(resources.GetObject("toolBarStandard.Location")));
			this.toolBarStandard.Name = "toolBarStandard";
			this.toolBarStandard.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("toolBarStandard.RightToLeft")));
			this.toolBarStandard.ShowToolTips = ((bool)(resources.GetObject("toolBarStandard.ShowToolTips")));
			this.toolBarStandard.Size = ((System.Drawing.Size)(resources.GetObject("toolBarStandard.Size")));
			this.toolBarStandard.TabIndex = ((int)(resources.GetObject("toolBarStandard.TabIndex")));
			this.toolBarStandard.TextAlign = ((System.Windows.Forms.ToolBarTextAlign)(resources.GetObject("toolBarStandard.TextAlign")));
			this.toolBarStandard.Visible = ((bool)(resources.GetObject("toolBarStandard.Visible")));
			this.toolBarStandard.Wrappable = ((bool)(resources.GetObject("toolBarStandard.Wrappable")));
			this.toolBarStandard.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarStandard_ButtonClick);
			// 
			// toolBarButtonQuasarToday
			// 
			this.toolBarButtonQuasarToday.Enabled = ((bool)(resources.GetObject("toolBarButtonQuasarToday.Enabled")));
			this.toolBarButtonQuasarToday.ImageIndex = ((int)(resources.GetObject("toolBarButtonQuasarToday.ImageIndex")));
			this.toolBarButtonQuasarToday.Tag = Shadows.Quasar.Common.CommonButton.QuasarToday;
			this.toolBarButtonQuasarToday.Text = resources.GetString("toolBarButtonQuasarToday.Text");
			this.toolBarButtonQuasarToday.ToolTipText = resources.GetString("toolBarButtonQuasarToday.ToolTipText");
			this.toolBarButtonQuasarToday.Visible = ((bool)(resources.GetObject("toolBarButtonQuasarToday.Visible")));
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Enabled = ((bool)(resources.GetObject("toolBarButton1.Enabled")));
			this.toolBarButton1.ImageIndex = ((int)(resources.GetObject("toolBarButton1.ImageIndex")));
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.toolBarButton1.Text = resources.GetString("toolBarButton1.Text");
			this.toolBarButton1.ToolTipText = resources.GetString("toolBarButton1.ToolTipText");
			this.toolBarButton1.Visible = ((bool)(resources.GetObject("toolBarButton1.Visible")));
			// 
			// toolBarButtonPrint
			// 
			this.toolBarButtonPrint.Enabled = ((bool)(resources.GetObject("toolBarButtonPrint.Enabled")));
			this.toolBarButtonPrint.ImageIndex = ((int)(resources.GetObject("toolBarButtonPrint.ImageIndex")));
			this.toolBarButtonPrint.Tag = Shadows.Quasar.Common.CommonButton.Print;
			this.toolBarButtonPrint.Text = resources.GetString("toolBarButtonPrint.Text");
			this.toolBarButtonPrint.ToolTipText = resources.GetString("toolBarButtonPrint.ToolTipText");
			this.toolBarButtonPrint.Visible = ((bool)(resources.GetObject("toolBarButtonPrint.Visible")));
			// 
			// toolBarButtonMoveToFolder
			// 
			this.toolBarButtonMoveToFolder.Enabled = ((bool)(resources.GetObject("toolBarButtonMoveToFolder.Enabled")));
			this.toolBarButtonMoveToFolder.ImageIndex = ((int)(resources.GetObject("toolBarButtonMoveToFolder.ImageIndex")));
			this.toolBarButtonMoveToFolder.Tag = Shadows.Quasar.Common.CommonButton.MoveToFolder;
			this.toolBarButtonMoveToFolder.Text = resources.GetString("toolBarButtonMoveToFolder.Text");
			this.toolBarButtonMoveToFolder.ToolTipText = resources.GetString("toolBarButtonMoveToFolder.ToolTipText");
			this.toolBarButtonMoveToFolder.Visible = ((bool)(resources.GetObject("toolBarButtonMoveToFolder.Visible")));
			// 
			// toolBarButtonDelete
			// 
			this.toolBarButtonDelete.Enabled = ((bool)(resources.GetObject("toolBarButtonDelete.Enabled")));
			this.toolBarButtonDelete.ImageIndex = ((int)(resources.GetObject("toolBarButtonDelete.ImageIndex")));
			this.toolBarButtonDelete.Tag = Shadows.Quasar.Common.CommonButton.Delete;
			this.toolBarButtonDelete.Text = resources.GetString("toolBarButtonDelete.Text");
			this.toolBarButtonDelete.ToolTipText = resources.GetString("toolBarButtonDelete.ToolTipText");
			this.toolBarButtonDelete.Visible = ((bool)(resources.GetObject("toolBarButtonDelete.Visible")));
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.Enabled = ((bool)(resources.GetObject("toolBarButton2.Enabled")));
			this.toolBarButton2.ImageIndex = ((int)(resources.GetObject("toolBarButton2.ImageIndex")));
			this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.toolBarButton2.Text = resources.GetString("toolBarButton2.Text");
			this.toolBarButton2.ToolTipText = resources.GetString("toolBarButton2.ToolTipText");
			this.toolBarButton2.Visible = ((bool)(resources.GetObject("toolBarButton2.Visible")));
			// 
			// toolBarButtonCut
			// 
			this.toolBarButtonCut.Enabled = ((bool)(resources.GetObject("toolBarButtonCut.Enabled")));
			this.toolBarButtonCut.ImageIndex = ((int)(resources.GetObject("toolBarButtonCut.ImageIndex")));
			this.toolBarButtonCut.Tag = Shadows.Quasar.Common.CommonButton.Cut;
			this.toolBarButtonCut.Text = resources.GetString("toolBarButtonCut.Text");
			this.toolBarButtonCut.ToolTipText = resources.GetString("toolBarButtonCut.ToolTipText");
			this.toolBarButtonCut.Visible = ((bool)(resources.GetObject("toolBarButtonCut.Visible")));
			// 
			// toolBarButtonCopy
			// 
			this.toolBarButtonCopy.Enabled = ((bool)(resources.GetObject("toolBarButtonCopy.Enabled")));
			this.toolBarButtonCopy.ImageIndex = ((int)(resources.GetObject("toolBarButtonCopy.ImageIndex")));
			this.toolBarButtonCopy.Tag = Shadows.Quasar.Common.CommonButton.Copy;
			this.toolBarButtonCopy.Text = resources.GetString("toolBarButtonCopy.Text");
			this.toolBarButtonCopy.ToolTipText = resources.GetString("toolBarButtonCopy.ToolTipText");
			this.toolBarButtonCopy.Visible = ((bool)(resources.GetObject("toolBarButtonCopy.Visible")));
			// 
			// toolBarButtonPaste
			// 
			this.toolBarButtonPaste.Enabled = ((bool)(resources.GetObject("toolBarButtonPaste.Enabled")));
			this.toolBarButtonPaste.ImageIndex = ((int)(resources.GetObject("toolBarButtonPaste.ImageIndex")));
			this.toolBarButtonPaste.Tag = Shadows.Quasar.Common.CommonButton.Paste;
			this.toolBarButtonPaste.Text = resources.GetString("toolBarButtonPaste.Text");
			this.toolBarButtonPaste.ToolTipText = resources.GetString("toolBarButtonPaste.ToolTipText");
			this.toolBarButtonPaste.Visible = ((bool)(resources.GetObject("toolBarButtonPaste.Visible")));
			// 
			// toolBarButton3
			// 
			this.toolBarButton3.Enabled = ((bool)(resources.GetObject("toolBarButton3.Enabled")));
			this.toolBarButton3.ImageIndex = ((int)(resources.GetObject("toolBarButton3.ImageIndex")));
			this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.toolBarButton3.Text = resources.GetString("toolBarButton3.Text");
			this.toolBarButton3.ToolTipText = resources.GetString("toolBarButton3.ToolTipText");
			this.toolBarButton3.Visible = ((bool)(resources.GetObject("toolBarButton3.Visible")));
			// 
			// toolBarButtonClearProposed
			// 
			this.toolBarButtonClearProposed.Enabled = ((bool)(resources.GetObject("toolBarButtonClearProposed.Enabled")));
			this.toolBarButtonClearProposed.ImageIndex = ((int)(resources.GetObject("toolBarButtonClearProposed.ImageIndex")));
			this.toolBarButtonClearProposed.Text = resources.GetString("toolBarButtonClearProposed.Text");
			this.toolBarButtonClearProposed.ToolTipText = resources.GetString("toolBarButtonClearProposed.ToolTipText");
			this.toolBarButtonClearProposed.Visible = ((bool)(resources.GetObject("toolBarButtonClearProposed.Visible")));
			// 
			// toolBarButtonRebalance
			// 
			this.toolBarButtonRebalance.Enabled = ((bool)(resources.GetObject("toolBarButtonRebalance.Enabled")));
			this.toolBarButtonRebalance.ImageIndex = ((int)(resources.GetObject("toolBarButtonRebalance.ImageIndex")));
			this.toolBarButtonRebalance.Tag = "Rebalance";
			this.toolBarButtonRebalance.Text = resources.GetString("toolBarButtonRebalance.Text");
			this.toolBarButtonRebalance.ToolTipText = resources.GetString("toolBarButtonRebalance.ToolTipText");
			this.toolBarButtonRebalance.Visible = ((bool)(resources.GetObject("toolBarButtonRebalance.Visible")));
			// 
			// toolBarButtonRebalanceSelection
			// 
			this.toolBarButtonRebalanceSelection.Enabled = ((bool)(resources.GetObject("toolBarButtonRebalanceSelection.Enabled")));
			this.toolBarButtonRebalanceSelection.ImageIndex = ((int)(resources.GetObject("toolBarButtonRebalanceSelection.ImageIndex")));
			this.toolBarButtonRebalanceSelection.Text = resources.GetString("toolBarButtonRebalanceSelection.Text");
			this.toolBarButtonRebalanceSelection.ToolTipText = resources.GetString("toolBarButtonRebalanceSelection.ToolTipText");
			this.toolBarButtonRebalanceSelection.Visible = ((bool)(resources.GetObject("toolBarButtonRebalanceSelection.Visible")));
			// 
			// toolBarButtonSend
			// 
			this.toolBarButtonSend.Enabled = ((bool)(resources.GetObject("toolBarButtonSend.Enabled")));
			this.toolBarButtonSend.ImageIndex = ((int)(resources.GetObject("toolBarButtonSend.ImageIndex")));
			this.toolBarButtonSend.Text = resources.GetString("toolBarButtonSend.Text");
			this.toolBarButtonSend.ToolTipText = resources.GetString("toolBarButtonSend.ToolTipText");
			this.toolBarButtonSend.Visible = ((bool)(resources.GetObject("toolBarButtonSend.Visible")));
			// 
			// toolBarButton4
			// 
			this.toolBarButton4.Enabled = ((bool)(resources.GetObject("toolBarButton4.Enabled")));
			this.toolBarButton4.ImageIndex = ((int)(resources.GetObject("toolBarButton4.ImageIndex")));
			this.toolBarButton4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			this.toolBarButton4.Text = resources.GetString("toolBarButton4.Text");
			this.toolBarButton4.ToolTipText = resources.GetString("toolBarButton4.ToolTipText");
			this.toolBarButton4.Visible = ((bool)(resources.GetObject("toolBarButton4.Visible")));
			// 
			// toolBarButtonHelp
			// 
			this.toolBarButtonHelp.Enabled = ((bool)(resources.GetObject("toolBarButtonHelp.Enabled")));
			this.toolBarButtonHelp.ImageIndex = ((int)(resources.GetObject("toolBarButtonHelp.ImageIndex")));
			this.toolBarButtonHelp.Text = resources.GetString("toolBarButtonHelp.Text");
			this.toolBarButtonHelp.ToolTipText = resources.GetString("toolBarButtonHelp.ToolTipText");
			this.toolBarButtonHelp.Visible = ((bool)(resources.GetObject("toolBarButtonHelp.Visible")));
			// 
			// imageListStandard
			// 
			this.imageListStandard.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageListStandard.ImageSize = ((System.Drawing.Size)(resources.GetObject("imageListStandard.ImageSize")));
			this.imageListStandard.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStandard.ImageStream")));
			this.imageListStandard.TransparentColor = System.Drawing.Color.Aqua;
			// 
			// tabControl
			// 
			this.tabControl.AccessibleDescription = resources.GetString("tabControl.AccessibleDescription");
			this.tabControl.AccessibleName = resources.GetString("tabControl.AccessibleName");
			this.tabControl.Alignment = ((System.Windows.Forms.TabAlignment)(resources.GetObject("tabControl.Alignment")));
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabControl.Anchor")));
			this.tabControl.Appearance = ((System.Windows.Forms.TabAppearance)(resources.GetObject("tabControl.Appearance")));
			this.tabControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabControl.BackgroundImage")));
			this.tabControl.Controls.Add(this.tabPageAppraisal);
			this.tabControl.Controls.Add(this.tabPageSector);
			this.tabControl.Controls.Add(this.tabPageRisk);
			this.tabControl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabControl.Dock")));
			this.tabControl.Enabled = ((bool)(resources.GetObject("tabControl.Enabled")));
			this.tabControl.Font = ((System.Drawing.Font)(resources.GetObject("tabControl.Font")));
			this.tabControl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabControl.ImeMode")));
			this.tabControl.ItemSize = ((System.Drawing.Size)(resources.GetObject("tabControl.ItemSize")));
			this.tabControl.Location = ((System.Drawing.Point)(resources.GetObject("tabControl.Location")));
			this.tabControl.Name = "tabControl";
			this.tabControl.Padding = ((System.Drawing.Point)(resources.GetObject("tabControl.Padding")));
			this.tabControl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabControl.RightToLeft")));
			this.tabControl.SelectedIndex = 0;
			this.tabControl.ShowToolTips = ((bool)(resources.GetObject("tabControl.ShowToolTips")));
			this.tabControl.Size = ((System.Drawing.Size)(resources.GetObject("tabControl.Size")));
			this.tabControl.TabIndex = ((int)(resources.GetObject("tabControl.TabIndex")));
			this.tabControl.Text = resources.GetString("tabControl.Text");
			this.tabControl.Visible = ((bool)(resources.GetObject("tabControl.Visible")));
			// 
			// tabPageAppraisal
			// 
			this.tabPageAppraisal.AccessibleDescription = resources.GetString("tabPageAppraisal.AccessibleDescription");
			this.tabPageAppraisal.AccessibleName = resources.GetString("tabPageAppraisal.AccessibleName");
			this.tabPageAppraisal.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPageAppraisal.Anchor")));
			this.tabPageAppraisal.AutoScroll = ((bool)(resources.GetObject("tabPageAppraisal.AutoScroll")));
			this.tabPageAppraisal.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPageAppraisal.AutoScrollMargin")));
			this.tabPageAppraisal.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPageAppraisal.AutoScrollMinSize")));
			this.tabPageAppraisal.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageAppraisal.BackgroundImage")));
			this.tabPageAppraisal.Controls.Add(this.appraisalViewer);
			this.tabPageAppraisal.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPageAppraisal.Dock")));
			this.tabPageAppraisal.Enabled = ((bool)(resources.GetObject("tabPageAppraisal.Enabled")));
			this.tabPageAppraisal.Font = ((System.Drawing.Font)(resources.GetObject("tabPageAppraisal.Font")));
			this.tabPageAppraisal.ImageIndex = ((int)(resources.GetObject("tabPageAppraisal.ImageIndex")));
			this.tabPageAppraisal.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPageAppraisal.ImeMode")));
			this.tabPageAppraisal.Location = ((System.Drawing.Point)(resources.GetObject("tabPageAppraisal.Location")));
			this.tabPageAppraisal.Name = "tabPageAppraisal";
			this.tabPageAppraisal.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPageAppraisal.RightToLeft")));
			this.tabPageAppraisal.Size = ((System.Drawing.Size)(resources.GetObject("tabPageAppraisal.Size")));
			this.tabPageAppraisal.TabIndex = ((int)(resources.GetObject("tabPageAppraisal.TabIndex")));
			this.tabPageAppraisal.Text = resources.GetString("tabPageAppraisal.Text");
			this.tabPageAppraisal.ToolTipText = resources.GetString("tabPageAppraisal.ToolTipText");
			this.tabPageAppraisal.Visible = ((bool)(resources.GetObject("tabPageAppraisal.Visible")));
			// 
			// appraisalViewer
			// 
			this.appraisalViewer.AccessibleDescription = resources.GetString("appraisalViewer.AccessibleDescription");
			this.appraisalViewer.AccessibleName = resources.GetString("appraisalViewer.AccessibleName");
			this.appraisalViewer.AccountId = 0;
			this.appraisalViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("appraisalViewer.Anchor")));
			this.appraisalViewer.AutoScroll = ((bool)(resources.GetObject("appraisalViewer.AutoScroll")));
			this.appraisalViewer.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("appraisalViewer.AutoScrollMargin")));
			this.appraisalViewer.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("appraisalViewer.AutoScrollMinSize")));
			this.appraisalViewer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("appraisalViewer.BackgroundImage")));
			this.appraisalViewer.ChildAccountIds = null;
			this.appraisalViewer.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("appraisalViewer.Dock")));
			this.appraisalViewer.DocumentVersion = 0;
			this.appraisalViewer.Enabled = ((bool)(resources.GetObject("appraisalViewer.Enabled")));
			this.appraisalViewer.Font = ((System.Drawing.Font)(resources.GetObject("appraisalViewer.Font")));
			this.appraisalViewer.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("appraisalViewer.ImeMode")));
			this.appraisalViewer.IsRefreshNeeded = true;
			this.appraisalViewer.IsStylesheetChanged = true;
			this.appraisalViewer.Location = ((System.Drawing.Point)(resources.GetObject("appraisalViewer.Location")));
			this.appraisalViewer.ModelId = 0;
			this.appraisalViewer.MoveAfterReturn = true;
			this.appraisalViewer.Name = "appraisalViewer";
			this.appraisalViewer.Pricing = Shadows.Quasar.Common.Pricing.Close;
			this.appraisalViewer.ResetViewer = true;
			this.appraisalViewer.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("appraisalViewer.RightToLeft")));
			this.appraisalViewer.Size = ((System.Drawing.Size)(resources.GetObject("appraisalViewer.Size")));
			this.appraisalViewer.StylesheetId = 0;
			this.appraisalViewer.TabIndex = ((int)(resources.GetObject("appraisalViewer.TabIndex")));
			this.appraisalViewer.Visible = ((bool)(resources.GetObject("appraisalViewer.Visible")));
			this.appraisalViewer.EndCloseDocument += new System.EventHandler(this.childViewer_EndCloseDocument);
			this.appraisalViewer.EndOpenDocument += new System.EventHandler(this.childViewer_EndOpenDocument);
			// 
			// tabPageSector
			// 
			this.tabPageSector.AccessibleDescription = resources.GetString("tabPageSector.AccessibleDescription");
			this.tabPageSector.AccessibleName = resources.GetString("tabPageSector.AccessibleName");
			this.tabPageSector.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPageSector.Anchor")));
			this.tabPageSector.AutoScroll = ((bool)(resources.GetObject("tabPageSector.AutoScroll")));
			this.tabPageSector.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPageSector.AutoScrollMargin")));
			this.tabPageSector.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPageSector.AutoScrollMinSize")));
			this.tabPageSector.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageSector.BackgroundImage")));
			this.tabPageSector.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPageSector.Dock")));
			this.tabPageSector.Enabled = ((bool)(resources.GetObject("tabPageSector.Enabled")));
			this.tabPageSector.Font = ((System.Drawing.Font)(resources.GetObject("tabPageSector.Font")));
			this.tabPageSector.ImageIndex = ((int)(resources.GetObject("tabPageSector.ImageIndex")));
			this.tabPageSector.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPageSector.ImeMode")));
			this.tabPageSector.Location = ((System.Drawing.Point)(resources.GetObject("tabPageSector.Location")));
			this.tabPageSector.Name = "tabPageSector";
			this.tabPageSector.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPageSector.RightToLeft")));
			this.tabPageSector.Size = ((System.Drawing.Size)(resources.GetObject("tabPageSector.Size")));
			this.tabPageSector.TabIndex = ((int)(resources.GetObject("tabPageSector.TabIndex")));
			this.tabPageSector.Text = resources.GetString("tabPageSector.Text");
			this.tabPageSector.ToolTipText = resources.GetString("tabPageSector.ToolTipText");
			this.tabPageSector.Visible = ((bool)(resources.GetObject("tabPageSector.Visible")));
			// 
			// tabPageRisk
			// 
			this.tabPageRisk.AccessibleDescription = resources.GetString("tabPageRisk.AccessibleDescription");
			this.tabPageRisk.AccessibleName = resources.GetString("tabPageRisk.AccessibleName");
			this.tabPageRisk.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPageRisk.Anchor")));
			this.tabPageRisk.AutoScroll = ((bool)(resources.GetObject("tabPageRisk.AutoScroll")));
			this.tabPageRisk.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPageRisk.AutoScrollMargin")));
			this.tabPageRisk.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPageRisk.AutoScrollMinSize")));
			this.tabPageRisk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageRisk.BackgroundImage")));
			this.tabPageRisk.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPageRisk.Dock")));
			this.tabPageRisk.Enabled = ((bool)(resources.GetObject("tabPageRisk.Enabled")));
			this.tabPageRisk.Font = ((System.Drawing.Font)(resources.GetObject("tabPageRisk.Font")));
			this.tabPageRisk.ImageIndex = ((int)(resources.GetObject("tabPageRisk.ImageIndex")));
			this.tabPageRisk.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPageRisk.ImeMode")));
			this.tabPageRisk.Location = ((System.Drawing.Point)(resources.GetObject("tabPageRisk.Location")));
			this.tabPageRisk.Name = "tabPageRisk";
			this.tabPageRisk.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPageRisk.RightToLeft")));
			this.tabPageRisk.Size = ((System.Drawing.Size)(resources.GetObject("tabPageRisk.Size")));
			this.tabPageRisk.TabIndex = ((int)(resources.GetObject("tabPageRisk.TabIndex")));
			this.tabPageRisk.Text = resources.GetString("tabPageRisk.Text");
			this.tabPageRisk.ToolTipText = resources.GetString("tabPageRisk.ToolTipText");
			this.tabPageRisk.Visible = ((bool)(resources.GetObject("tabPageRisk.Visible")));
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuItemFile,
																						this.menuItemAction,
																						this.menuItem2});
			this.contextMenu.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("contextMenu.RightToLeft")));
			// 
			// menuItemFile
			// 
			this.menuItemFile.Enabled = ((bool)(resources.GetObject("menuItemFile.Enabled")));
			this.menuItemFile.Index = 0;
			this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItem1,
																						 this.menuItemSaveAppraisal});
			this.menuItemFile.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItemFile.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemFile.Shortcut")));
			this.menuItemFile.ShowShortcut = ((bool)(resources.GetObject("menuItemFile.ShowShortcut")));
			this.menuItemFile.Text = resources.GetString("menuItemFile.Text");
			this.menuItemFile.Visible = ((bool)(resources.GetObject("menuItemFile.Visible")));
			// 
			// menuItem1
			// 
			this.menuItem1.Enabled = ((bool)(resources.GetObject("menuItem1.Enabled")));
			this.menuItem1.Index = 0;
			this.menuItem1.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem1.Shortcut")));
			this.menuItem1.ShowShortcut = ((bool)(resources.GetObject("menuItem1.ShowShortcut")));
			this.menuItem1.Text = resources.GetString("menuItem1.Text");
			this.menuItem1.Visible = ((bool)(resources.GetObject("menuItem1.Visible")));
			// 
			// menuItemSaveAppraisal
			// 
			this.menuItemSaveAppraisal.Enabled = ((bool)(resources.GetObject("menuItemSaveAppraisal.Enabled")));
			this.menuItemSaveAppraisal.Index = 1;
			this.menuItemSaveAppraisal.MergeOrder = 1;
			this.menuItemSaveAppraisal.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemSaveAppraisal.Shortcut")));
			this.menuItemSaveAppraisal.ShowShortcut = ((bool)(resources.GetObject("menuItemSaveAppraisal.ShowShortcut")));
			this.menuItemSaveAppraisal.Text = resources.GetString("menuItemSaveAppraisal.Text");
			this.menuItemSaveAppraisal.Visible = ((bool)(resources.GetObject("menuItemSaveAppraisal.Visible")));
			this.menuItemSaveAppraisal.Click += new System.EventHandler(this.menuItemSaveAppraisal_Click);
			// 
			// menuItemAction
			// 
			this.menuItemAction.Enabled = ((bool)(resources.GetObject("menuItemAction.Enabled")));
			this.menuItemAction.Index = 1;
			this.menuItemAction.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						   this.menuItemCheckCompliance,
																						   this.menuItemRebalance,
																						   this.menuItemSendOrders});
			this.menuItemAction.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemAction.Shortcut")));
			this.menuItemAction.ShowShortcut = ((bool)(resources.GetObject("menuItemAction.ShowShortcut")));
			this.menuItemAction.Text = resources.GetString("menuItemAction.Text");
			this.menuItemAction.Visible = ((bool)(resources.GetObject("menuItemAction.Visible")));
			// 
			// menuItemCheckCompliance
			// 
			this.menuItemCheckCompliance.Enabled = ((bool)(resources.GetObject("menuItemCheckCompliance.Enabled")));
			this.menuItemCheckCompliance.Index = 0;
			this.menuItemCheckCompliance.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemCheckCompliance.Shortcut")));
			this.menuItemCheckCompliance.ShowShortcut = ((bool)(resources.GetObject("menuItemCheckCompliance.ShowShortcut")));
			this.menuItemCheckCompliance.Text = resources.GetString("menuItemCheckCompliance.Text");
			this.menuItemCheckCompliance.Visible = ((bool)(resources.GetObject("menuItemCheckCompliance.Visible")));
			this.menuItemCheckCompliance.Click += new System.EventHandler(this.menuItemCheckCompliance_Click);
			// 
			// menuItemRebalance
			// 
			this.menuItemRebalance.Enabled = ((bool)(resources.GetObject("menuItemRebalance.Enabled")));
			this.menuItemRebalance.Index = 1;
			this.menuItemRebalance.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemRebalance.Shortcut")));
			this.menuItemRebalance.ShowShortcut = ((bool)(resources.GetObject("menuItemRebalance.ShowShortcut")));
			this.menuItemRebalance.Text = resources.GetString("menuItemRebalance.Text");
			this.menuItemRebalance.Visible = ((bool)(resources.GetObject("menuItemRebalance.Visible")));
			// 
			// menuItemSendOrders
			// 
			this.menuItemSendOrders.Enabled = ((bool)(resources.GetObject("menuItemSendOrders.Enabled")));
			this.menuItemSendOrders.Index = 2;
			this.menuItemSendOrders.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemSendOrders.Shortcut")));
			this.menuItemSendOrders.ShowShortcut = ((bool)(resources.GetObject("menuItemSendOrders.ShowShortcut")));
			this.menuItemSendOrders.Text = resources.GetString("menuItemSendOrders.Text");
			this.menuItemSendOrders.Visible = ((bool)(resources.GetObject("menuItemSendOrders.Visible")));
			// 
			// menuItem2
			// 
			this.menuItem2.Enabled = ((bool)(resources.GetObject("menuItem2.Enabled")));
			this.menuItem2.Index = 2;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemLast,
																					  this.menuItemClose});
			this.menuItem2.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem2.Shortcut")));
			this.menuItem2.ShowShortcut = ((bool)(resources.GetObject("menuItem2.ShowShortcut")));
			this.menuItem2.Text = resources.GetString("menuItem2.Text");
			this.menuItem2.Visible = ((bool)(resources.GetObject("menuItem2.Visible")));
			// 
			// menuItemLast
			// 
			this.menuItemLast.Enabled = ((bool)(resources.GetObject("menuItemLast.Enabled")));
			this.menuItemLast.Index = 0;
			this.menuItemLast.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemLast.Shortcut")));
			this.menuItemLast.ShowShortcut = ((bool)(resources.GetObject("menuItemLast.ShowShortcut")));
			this.menuItemLast.Text = resources.GetString("menuItemLast.Text");
			this.menuItemLast.Visible = ((bool)(resources.GetObject("menuItemLast.Visible")));
			this.menuItemLast.Click += new System.EventHandler(this.menuItemLast_Click);
			// 
			// menuItemClose
			// 
			this.menuItemClose.Enabled = ((bool)(resources.GetObject("menuItemClose.Enabled")));
			this.menuItemClose.Index = 1;
			this.menuItemClose.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItemClose.Shortcut")));
			this.menuItemClose.ShowShortcut = ((bool)(resources.GetObject("menuItemClose.ShowShortcut")));
			this.menuItemClose.Text = resources.GetString("menuItemClose.Text");
			this.menuItemClose.Visible = ((bool)(resources.GetObject("menuItemClose.Visible")));
			this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
			// 
			// AccountViewer
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.Controls.Add(this.tabControl);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.Name = "AccountViewer";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
			this.tabControl.ResumeLayout(false);
			this.tabPageAppraisal.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Opens the Account Viewer.
		/// </summary>
		public override void OpenViewer()
		{

			// Open the Account Viewer in the background.
			QueueCommand(new ThreadHandler(OpenViewerCommand));

		}

		/// <summary>
		/// Closes the Account Viewer.
		/// </summary>
		public override void CloseViewer()
		{

			// Close the Account Viewer in the background.
			QueueCommand(new ThreadHandler(CloseViewerCommand));

		}

		
		/// <summary>
		/// Opens the document for the given object and it's argument.
		/// </summary>
		/// <param name="accountId">The primary identifier of the object to open.</param>
		/// <param name="argument">Options that can be used to further specify the document's properties.</param>
		public override void OpenDocument(int accountId)
		{

			// The rest of the initialization of this document must be done in the background.
			QueueCommand(new ThreadHandler(OpenDocumentCommand), accountId);

		}

		/// <summary>
		/// Closes the Account Document.
		/// </summary>
		public override void CloseDocument()
		{

			// Execute a command in the background to close the document.
			QueueCommand(new ThreadHandler(CloseDocumentCommand));

		}

		/// <summary>
		/// Opens the Account Biewer.
		/// </summary>
		private void OpenViewerCommand(params object[] argument)
		{

			// Open up each of the views for the account.  Note that we can only clear the execution and placement
			// viewers.  An 'Open' command has no relevance until there is a block order to be viewed.
			this.appraisalViewer.OpenViewer();

			// Broadcast the event that the viewer is now open.
			OnEndOpenViewer();

		}
						
		/// <summary>
		/// Closes out the account document and all the child viewers.
		/// </summary>
		private void CloseViewerCommand(params object[] argument)
		{

			// Close out the child viewers.
			this.appraisalViewer.CloseViewer();

			// Broadcast the event that the viewer is now closed.
			OnEndCloseViewer();

		}
		
		/// <summary>
		/// Opens the Account Document.
		/// </summary>
		/// <param name="accountId">The account identifier.</param>
		private void OpenDocumentCommand(params object[] argument)
		{

			// Extract the command arguments.
			int accountId = (int)argument[0];
			
			try
			{

				// The Account Identifier Attribute.
				this.accountId = accountId;

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Each account can have a stylesheet assigned to it so Fixed Income traders view Fixed Income data, 
				// equity traders Equity data, and so forth.  If no account is assigned, a default will be provided.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(accountId);
				if (accountRow == null)
					throw new ArgumentException("This account has been deleted", accountId.ToString());

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Open up each of the views for the account.
			this.appraisalViewer.OpenDocument(this.accountId);

		}
						
		/// <summary>
		/// Closes out the account document and all the child viewers.
		/// </summary>
		private void CloseDocumentCommand(params object[] argument)
		{

			// Close out the viewers if they've been opened.
			this.appraisalViewer.CloseDocument();

		}
		
		/// <summary>
		/// Called when a child window is ready to be displayed.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void childViewer_EndOpenDocument(object sender, System.EventArgs e)
		{

			// The blotter isn't considered 'Opened' until all the child viewers have been opened.
			if ((this.appraisalViewer.IsDocumentOpen || !this.hasAppraisalViewer))
			{

				// Once all the children have completed their opening operations, the blotter configure them in the 
				// foreground.
				Invoke(this.endOpenEventHandler, new object[] {this, EventArgs.Empty});

				// Broadcast the fact that all the child viewers are open.  Generally, this will be handled by the container
				// which will move the blotter document to the top window.
				OnEndOpenDocument();

			}

		}

		/// <summary>
		/// Called when a child window is finished closing.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void childViewer_EndCloseDocument(object sender, System.EventArgs e)
		{

			// The account viewer isn't considered 'Closed' until all the child viewers have been opened.
			if ((this.appraisalViewer.IsDocumentOpen || !this.hasAppraisalViewer))
			{

				// Broadcast the fact that all the child viewers are closed.
				OnEndCloseDocument();

			}

		}
		
		/// <summary>
		/// Configures the child viewers based on the menu selections.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void EndOpenForeground(object sender, EventArgs eventArgs)
		{

			// This is left intensionally blank.  This is where the viewers would be arranged and ordered in a multi-viewer pane.

		}

		/// <summary>
		/// Handles the user request to save the document.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event argument</param>
		private void menuItemSaveAppraisal_Click(object sender, System.EventArgs e)
		{

			// Pass the operation on to the base class.
			this.appraisalViewer.SaveAs();

		}

		/// <summary>
		/// Handles the user pressing of a toolbar button.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event argument</param>
		public void toolBarStandard_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{

			// Clear the appraisal of all proposed orders.
			if (e.Button == this.toolBarButtonClearProposed)
				ServiceQueue.Execute(new ThreadHandler(ProposedOrder.ClearAccount), this.appraisalViewer.AccountId);

			// Rebalance the appraisal according to the current model.
			if (e.Button == this.toolBarButtonRebalance)
				ServiceQueue.Execute(new ThreadHandler(ProposedOrder.Rebalance), this.appraisalViewer.AccountId,
					this.appraisalViewer.ModelId);

			// Rebalance the appraisal using only the selected items.
			if (e.Button == this.toolBarButtonRebalanceSelection)
				ServiceQueue.Execute(new ThreadHandler(ProposedOrder.RebalanceSelection), this.appraisalViewer.AccountId,
					this.appraisalViewer.ModelId, this.appraisalViewer.GetSelectedPositions());

			// Send the selected proposed orders to the blotter.
			if (e.Button == this.toolBarButtonSend)
				ServiceQueue.Execute(new ThreadHandler(ProposedOrder.Send), this.appraisalViewer.GetSelectedPositions());

		}

		/// <summary>
		/// Handles the user's request for the closing prices.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event argument</param>
		private void menuItemClose_Click(object sender, System.EventArgs e)
		{
		
			// Change the pricing method.  This is available to any method that needs to do a calculation.
			ClientPreferences.Pricing = Pricing.Close;

			// Update the menu items to reflect the state of the user's pricing peference.
			this.menuItemLast.Checked = false;
			this.menuItemClose.Checked = true;

			// Change the pricing method.  This is available to any method that needs to do a calculation.
			ClientPreferences.Pricing = Pricing.Last;

			// Tell the appraisal that is shoulod use the closing prices.
			this.appraisalViewer.Pricing = Pricing.Close;

		}

		/// <summary>
		/// Handles the user's request for the real-time prices.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event argument</param>
		private void menuItemLast_Click(object sender, System.EventArgs e)
		{
		
			// Update the menu items to reflect the state of the user's pricing peference.
			this.menuItemLast.Checked = true;
			this.menuItemClose.Checked = false;
			
			// Change the pricing method.  This is available to any method that needs to do a calculation.
			ClientPreferences.Pricing = Pricing.Last;

			// Tell the appraisal that it should use the last prices.
			this.appraisalViewer.Pricing = Pricing.Last;

		}

		private void menuItemCheckCompliance_Click(object sender, System.EventArgs e)
		{

			// Close the Account Viewer in the background.
			QueueCommand(new ThreadHandler(ComplianceCommand));
			
		}

		private void ComplianceCommand(params object[] argument)
		{

			try
			{
			
				Assembly assembly = Assembly.Load("Library.Rules");
				foreach (Type type in assembly.GetTypes())
					assembly.CreateInstance(type.FullName);

			}
			catch (BatchException batchException)
			{

				// Display each error from the batch until the user hits the 'Cancel' button, or until they are all displayed.
				foreach (RemoteException remoteException in batchException.Exceptions)
					if (MessageBox.Show(this.TopLevelControl, remoteException.Message, "Quasar Error", MessageBoxButtons.OKCancel,
						MessageBoxIcon.Error) == DialogResult.Cancel)
						break;

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

	}

}
