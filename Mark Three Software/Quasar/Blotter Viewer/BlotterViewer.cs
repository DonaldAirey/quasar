/*************************************************************************************************************************
*
*	File:			BlotterViewer.cs
*	Description:	This control is used to display and manage a blotter.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.Blotter
{

	using Shadows.Quasar.Client;
	using Shadows.Quasar.Common;
	using Shadows.Quasar.Common.Controls;
	using Shadows.Quasar.Rule;
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
	using System.Web.Services.Protocols;

	/// <summary>
	/// A Collection of Viewers use to input and manage trade data.
	/// </summary>
	public class BlotterViewer : Shadows.Quasar.Viewers.Viewer
	{

		private int blotterId;
		private bool hasBlockOrderViewer;
		private bool hasPlacementViewer;
		private bool hasExecutionViewer;
		private bool hasTicketViewer;
		private bool hasOrderBookViewer;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ToolBar toolBarStandard;
		private System.Windows.Forms.ToolBarButton toolBarButtonNew;
		private System.Windows.Forms.ToolBarButton toolBarButton1;
		private System.Windows.Forms.ToolBarButton toolBarButtonPrint;
		private System.Windows.Forms.ToolBarButton toolBarButtonDelete;
		private System.Windows.Forms.ToolBarButton toolBarButton2;
		private System.Windows.Forms.ToolBarButton toolBarButtonCut;
		private System.Windows.Forms.ToolBarButton toolBarButtonCopy;
		private System.Windows.Forms.ToolBarButton toolBarButtonPaste;
		private System.Windows.Forms.ToolBarButton toolBarButton3;
		private System.Windows.Forms.ToolBarButton toolBarButtonHelp;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menuItemFileSaveAs;
		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItemOrder;
		private System.Windows.Forms.MenuItem menuItemBasket;
		private System.Windows.Forms.MenuItem menuItemStrategy;
		private System.Windows.Forms.MenuItem menuItemSecurity;
		private System.Windows.Forms.MenuItem menuItemBlotter;
		private System.Windows.Forms.MenuItem menuItemSortAscending;
		private System.Windows.Forms.MenuItem menuItemSortDescending;
		private System.Windows.Forms.MenuItem menuItemSortAscendingName;
		private System.Windows.Forms.MenuItem menuItemSortAscendingQuantityExecuted;
		private System.Windows.Forms.MenuItem menuItemSortDescendingName;
		private System.Windows.Forms.MenuItem menuItemSortAscendingQuantityLeaves;
		private System.Windows.Forms.MenuItem menuItemSortAscendingQuantityOrdered;
		private System.Windows.Forms.MenuItem menuItemSortAscendingSymbol;
		private System.Windows.Forms.MenuItem menuItemSortDescendingQuantityExecuted;
		private System.Windows.Forms.MenuItem menuItemSortDescendingQuantityOrdered;
		private System.Windows.Forms.MenuItem menuItemSortDescendingQuantityLeaves;
		private System.Windows.Forms.MenuItem menuItemSortDescendingSymbol;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemMergeBlock;
		private System.Windows.Forms.MenuItem menuItemUnmergeBlock;
		private System.Windows.Forms.MenuItem menuItemCloseBlock;
		private System.EventHandler endOpenEventHandler;
		private System.EventHandler executionDelegate;
		private System.EventHandler placementDelegate;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemRules;
		private System.Windows.Forms.MenuItem menuRunOnce;
		private System.Windows.Forms.MenuItem menuItemRunContinuously;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageMainBlotter;
		private System.Windows.Forms.TabPage tabPageTicketSummary;
		private System.Windows.Forms.TabPage tabPageOrderBook;
		private Shadows.Quasar.Viewers.Ticket.TicketViewer ticketViewer;
		private Shadows.Quasar.Viewers.BlockOrder.BlockOrderViewer blockOrderViewer;
		private System.Windows.Forms.Splitter splitter1;
		private Shadows.Quasar.Viewers.Execution.ExecutionViewer executionViewer;
		private Shadows.Quasar.Viewers.Placement.PlacementViewer placementViewer;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem4;
		private Shadows.Quasar.Viewers.OrderBookViewer orderBookViewer;
		private System.ComponentModel.IContainer components = null;

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
		/// Constructor for the BlotterViewer
		/// </summary>
		public BlotterViewer()
		{

			// Call the IDE supplied initialization.
			InitializeComponent();

			// This delegate is called from a background thread when all the child viewers have completed their 'Open' commands.
			this.endOpenEventHandler = new System.EventHandler(EndOpenForeground);

			// These delegates are used communicate to a generic execution and placement viewers. 
			this.executionDelegate = new System.EventHandler(ExecutionForeground);
			this.placementDelegate = new System.EventHandler(PlacementForeground);

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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BlotterViewer));
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.toolBarStandard = new System.Windows.Forms.ToolBar();
			this.toolBarButtonNew = new System.Windows.Forms.ToolBarButton();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItemFile = new System.Windows.Forms.MenuItem();
			this.menuItemFileSaveAs = new System.Windows.Forms.MenuItem();
			this.menuItemBlotter = new System.Windows.Forms.MenuItem();
			this.menuItemSortAscending = new System.Windows.Forms.MenuItem();
			this.menuItemSortAscendingName = new System.Windows.Forms.MenuItem();
			this.menuItemSortAscendingQuantityExecuted = new System.Windows.Forms.MenuItem();
			this.menuItemSortAscendingQuantityLeaves = new System.Windows.Forms.MenuItem();
			this.menuItemSortAscendingQuantityOrdered = new System.Windows.Forms.MenuItem();
			this.menuItemSortAscendingSymbol = new System.Windows.Forms.MenuItem();
			this.menuItemSortDescending = new System.Windows.Forms.MenuItem();
			this.menuItemSortDescendingName = new System.Windows.Forms.MenuItem();
			this.menuItemSortDescendingQuantityExecuted = new System.Windows.Forms.MenuItem();
			this.menuItemSortDescendingQuantityLeaves = new System.Windows.Forms.MenuItem();
			this.menuItemSortDescendingQuantityOrdered = new System.Windows.Forms.MenuItem();
			this.menuItemSortDescendingSymbol = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemMergeBlock = new System.Windows.Forms.MenuItem();
			this.menuItemUnmergeBlock = new System.Windows.Forms.MenuItem();
			this.menuItemCloseBlock = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItemRules = new System.Windows.Forms.MenuItem();
			this.menuRunOnce = new System.Windows.Forms.MenuItem();
			this.menuItemRunContinuously = new System.Windows.Forms.MenuItem();
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonPrint = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonDelete = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonCut = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonCopy = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonPaste = new System.Windows.Forms.ToolBarButton();
			this.toolBarButton3 = new System.Windows.Forms.ToolBarButton();
			this.toolBarButtonHelp = new System.Windows.Forms.ToolBarButton();
			this.menuItemOrder = new System.Windows.Forms.MenuItem();
			this.menuItemBasket = new System.Windows.Forms.MenuItem();
			this.menuItemStrategy = new System.Windows.Forms.MenuItem();
			this.menuItemSecurity = new System.Windows.Forms.MenuItem();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageMainBlotter = new System.Windows.Forms.TabPage();
			this.executionViewer = new Shadows.Quasar.Viewers.Execution.ExecutionViewer();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.placementViewer = new Shadows.Quasar.Viewers.Placement.PlacementViewer();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.blockOrderViewer = new Shadows.Quasar.Viewers.BlockOrder.BlockOrderViewer();
			this.tabPageTicketSummary = new System.Windows.Forms.TabPage();
			this.ticketViewer = new Shadows.Quasar.Viewers.Ticket.TicketViewer();
			this.tabPageOrderBook = new System.Windows.Forms.TabPage();
			this.orderBookViewer = new Shadows.Quasar.Viewers.OrderBookViewer();
			this.tabControl.SuspendLayout();
			this.tabPageMainBlotter.SuspendLayout();
			this.tabPageTicketSummary.SuspendLayout();
			this.tabPageOrderBook.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageList.ImageSize = new System.Drawing.Size(18, 18);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Aqua;
			// 
			// toolBarStandard
			// 
			this.toolBarStandard.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBarStandard.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																							   this.toolBarButtonNew,
																							   this.toolBarButton1,
																							   this.toolBarButtonPrint,
																							   this.toolBarButtonDelete,
																							   this.toolBarButton2,
																							   this.toolBarButtonCut,
																							   this.toolBarButtonCopy,
																							   this.toolBarButtonPaste,
																							   this.toolBarButton3,
																							   this.toolBarButtonHelp});
			this.toolBarStandard.ButtonSize = new System.Drawing.Size(23, 22);
			this.toolBarStandard.DropDownArrows = true;
			this.toolBarStandard.ImageList = this.imageList;
			this.toolBarStandard.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.toolBarStandard.Location = new System.Drawing.Point(0, 0);
			this.toolBarStandard.Name = "toolBarStandard";
			this.toolBarStandard.ShowToolTips = true;
			this.toolBarStandard.Size = new System.Drawing.Size(512, 28);
			this.toolBarStandard.TabIndex = 0;
			this.toolBarStandard.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBarStandard_ButtonClick);
			// 
			// toolBarButtonNew
			// 
			this.toolBarButtonNew.DropDownMenu = this.contextMenu;
			this.toolBarButtonNew.ImageIndex = 12;
			this.toolBarButtonNew.Style = System.Windows.Forms.ToolBarButtonStyle.DropDownButton;
			this.toolBarButtonNew.ToolTipText = "New Order";
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuItemFile,
																						this.menuItemBlotter,
																						this.menuItem3});
			this.contextMenu.RightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// menuItemFile
			// 
			this.menuItemFile.Index = 0;
			this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemFileSaveAs});
			this.menuItemFile.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItemFile.Text = "&File";
			// 
			// menuItemFileSaveAs
			// 
			this.menuItemFileSaveAs.Index = 0;
			this.menuItemFileSaveAs.Text = "&Save As...";
			this.menuItemFileSaveAs.Click += new System.EventHandler(this.menuItemSaveAs_Click);
			// 
			// menuItemBlotter
			// 
			this.menuItemBlotter.Index = 1;
			this.menuItemBlotter.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.menuItemSortAscending,
																							this.menuItemSortDescending,
																							this.menuItem1,
																							this.menuItemMergeBlock,
																							this.menuItemUnmergeBlock,
																							this.menuItemCloseBlock,
																							this.menuItem2,
																							this.menuItem4});
			this.menuItemBlotter.MergeOrder = 3;
			this.menuItemBlotter.Text = "&Blotter";
			// 
			// menuItemSortAscending
			// 
			this.menuItemSortAscending.Index = 0;
			this.menuItemSortAscending.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								  this.menuItemSortAscendingName,
																								  this.menuItemSortAscendingQuantityExecuted,
																								  this.menuItemSortAscendingQuantityLeaves,
																								  this.menuItemSortAscendingQuantityOrdered,
																								  this.menuItemSortAscendingSymbol});
			this.menuItemSortAscending.Text = "Sort &Ascending";
			// 
			// menuItemSortAscendingName
			// 
			this.menuItemSortAscendingName.Index = 0;
			this.menuItemSortAscendingName.Text = "&Name";
			this.menuItemSortAscendingName.Click += new System.EventHandler(this.menuItemSortAscendingName_Click);
			// 
			// menuItemSortAscendingQuantityExecuted
			// 
			this.menuItemSortAscendingQuantityExecuted.Index = 1;
			this.menuItemSortAscendingQuantityExecuted.Text = "Quantity &Executed";
			this.menuItemSortAscendingQuantityExecuted.Click += new System.EventHandler(this.menuItemSortAscendingQuantityExecuted_Click);
			// 
			// menuItemSortAscendingQuantityLeaves
			// 
			this.menuItemSortAscendingQuantityLeaves.Index = 2;
			this.menuItemSortAscendingQuantityLeaves.Text = "Quantity &Leaves";
			this.menuItemSortAscendingQuantityLeaves.Click += new System.EventHandler(this.menuItemSortAscendingQuantityLeaves_Click);
			// 
			// menuItemSortAscendingQuantityOrdered
			// 
			this.menuItemSortAscendingQuantityOrdered.Index = 3;
			this.menuItemSortAscendingQuantityOrdered.Text = "Quantity &Ordered";
			this.menuItemSortAscendingQuantityOrdered.Click += new System.EventHandler(this.menuItemSortAscendingQuantityOrdered_Click);
			// 
			// menuItemSortAscendingSymbol
			// 
			this.menuItemSortAscendingSymbol.Index = 4;
			this.menuItemSortAscendingSymbol.Text = "&Symbol";
			this.menuItemSortAscendingSymbol.Click += new System.EventHandler(this.menuItemSortAscendingSymbol_Click);
			// 
			// menuItemSortDescending
			// 
			this.menuItemSortDescending.Index = 1;
			this.menuItemSortDescending.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								   this.menuItemSortDescendingName,
																								   this.menuItemSortDescendingQuantityExecuted,
																								   this.menuItemSortDescendingQuantityLeaves,
																								   this.menuItemSortDescendingQuantityOrdered,
																								   this.menuItemSortDescendingSymbol});
			this.menuItemSortDescending.Text = "Sort Descending";
			// 
			// menuItemSortDescendingName
			// 
			this.menuItemSortDescendingName.Index = 0;
			this.menuItemSortDescendingName.Text = "&Name";
			this.menuItemSortDescendingName.Click += new System.EventHandler(this.menuItemSortDescendingName_Click);
			// 
			// menuItemSortDescendingQuantityExecuted
			// 
			this.menuItemSortDescendingQuantityExecuted.Index = 1;
			this.menuItemSortDescendingQuantityExecuted.Text = "Quantity &Executed";
			this.menuItemSortDescendingQuantityExecuted.Click += new System.EventHandler(this.menuItemSortDescendingQuantityExecuted_Click);
			// 
			// menuItemSortDescendingQuantityLeaves
			// 
			this.menuItemSortDescendingQuantityLeaves.Index = 2;
			this.menuItemSortDescendingQuantityLeaves.Text = "Quantity &Leaves";
			this.menuItemSortDescendingQuantityLeaves.Click += new System.EventHandler(this.menuItemSortDescendingQuantityLeaves_Click);
			// 
			// menuItemSortDescendingQuantityOrdered
			// 
			this.menuItemSortDescendingQuantityOrdered.Index = 3;
			this.menuItemSortDescendingQuantityOrdered.Text = "Quantity &Ordered";
			this.menuItemSortDescendingQuantityOrdered.Click += new System.EventHandler(this.menuItemSortDescendingQuantityOrdered_Click);
			// 
			// menuItemSortDescendingSymbol
			// 
			this.menuItemSortDescendingSymbol.Index = 4;
			this.menuItemSortDescendingSymbol.Text = "&Symbol";
			this.menuItemSortDescendingSymbol.Click += new System.EventHandler(this.menuItemSortDescendingSymbol_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// menuItemMergeBlock
			// 
			this.menuItemMergeBlock.Index = 3;
			this.menuItemMergeBlock.Text = "&Merge Block";
			// 
			// menuItemUnmergeBlock
			// 
			this.menuItemUnmergeBlock.Index = 4;
			this.menuItemUnmergeBlock.Text = "&Unmerge Block";
			// 
			// menuItemCloseBlock
			// 
			this.menuItemCloseBlock.Index = 5;
			this.menuItemCloseBlock.Text = "&Close Block";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 6;
			this.menuItem2.Text = "&Placement";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 7;
			this.menuItem4.Text = "&BlockOrder";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemRules,
																					  this.menuRunOnce,
																					  this.menuItemRunContinuously});
			this.menuItem3.MergeOrder = 3;
			this.menuItem3.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItem3.Text = "&Tools";
			// 
			// menuItemRules
			// 
			this.menuItemRules.Index = 0;
			this.menuItemRules.Text = "&Rules";
			this.menuItemRules.Click += new System.EventHandler(this.menuItemRules_Click);
			// 
			// menuRunOnce
			// 
			this.menuRunOnce.Index = 1;
			this.menuRunOnce.Text = "Run &Once";
			this.menuRunOnce.Click += new System.EventHandler(this.menuRunOnce_Click);
			// 
			// menuItemRunContinuously
			// 
			this.menuItemRunContinuously.Index = 2;
			this.menuItemRunContinuously.Text = "Run &Continuously";
			this.menuItemRunContinuously.Click += new System.EventHandler(this.menuItemRunContinuously_Click);
			// 
			// toolBarButton1
			// 
			this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButtonPrint
			// 
			this.toolBarButtonPrint.ImageIndex = 9;
			this.toolBarButtonPrint.ToolTipText = "Print";
			// 
			// toolBarButtonDelete
			// 
			this.toolBarButtonDelete.ImageIndex = 2;
			this.toolBarButtonDelete.ToolTipText = "Delete";
			// 
			// toolBarButton2
			// 
			this.toolBarButton2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButtonCut
			// 
			this.toolBarButtonCut.ImageIndex = 1;
			this.toolBarButtonCut.ToolTipText = "Cut";
			// 
			// toolBarButtonCopy
			// 
			this.toolBarButtonCopy.ImageIndex = 0;
			this.toolBarButtonCopy.ToolTipText = "Copy";
			// 
			// toolBarButtonPaste
			// 
			this.toolBarButtonPaste.ImageIndex = 8;
			// 
			// toolBarButton3
			// 
			this.toolBarButton3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarButtonHelp
			// 
			this.toolBarButtonHelp.ImageIndex = 5;
			this.toolBarButtonHelp.ToolTipText = "Help";
			// 
			// menuItemOrder
			// 
			this.menuItemOrder.Index = -1;
			this.menuItemOrder.Text = "&Order";
			// 
			// menuItemBasket
			// 
			this.menuItemBasket.Index = -1;
			this.menuItemBasket.Text = "&Basket";
			// 
			// menuItemStrategy
			// 
			this.menuItemStrategy.Index = -1;
			this.menuItemStrategy.Text = "&Strategy";
			// 
			// menuItemSecurity
			// 
			this.menuItemSecurity.Index = -1;
			this.menuItemSecurity.Text = "&Security";
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageMainBlotter);
			this.tabControl.Controls.Add(this.tabPageTicketSummary);
			this.tabControl.Controls.Add(this.tabPageOrderBook);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(512, 256);
			this.tabControl.TabIndex = 1;
			// 
			// tabPageMainBlotter
			// 
			this.tabPageMainBlotter.Controls.Add(this.executionViewer);
			this.tabPageMainBlotter.Controls.Add(this.splitter2);
			this.tabPageMainBlotter.Controls.Add(this.placementViewer);
			this.tabPageMainBlotter.Controls.Add(this.splitter1);
			this.tabPageMainBlotter.Controls.Add(this.blockOrderViewer);
			this.tabPageMainBlotter.Location = new System.Drawing.Point(4, 22);
			this.tabPageMainBlotter.Name = "tabPageMainBlotter";
			this.tabPageMainBlotter.Size = new System.Drawing.Size(504, 230);
			this.tabPageMainBlotter.TabIndex = 3;
			this.tabPageMainBlotter.Text = "Main Blotter";
			// 
			// executionViewer
			// 
			this.executionViewer.BlockOrderId = 0;
			this.executionViewer.BlotterId = 0;
			this.executionViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.executionViewer.DocumentVersion = 0;
			this.executionViewer.IsExecutionChanged = false;
			this.executionViewer.IsExecutionSelected = false;
			this.executionViewer.IsRefreshNeeded = true;
			this.executionViewer.IsStylesheetChanged = true;
			this.executionViewer.Location = new System.Drawing.Point(259, 131);
			this.executionViewer.MoveAfterReturn = true;
			this.executionViewer.Name = "executionViewer";
			this.executionViewer.ResetViewer = true;
			this.executionViewer.SelectedExecutionId = 0;
			this.executionViewer.SelectedRowType = 0;
			this.executionViewer.Size = new System.Drawing.Size(245, 99);
			this.executionViewer.StylesheetId = 0;
			this.executionViewer.TabIndex = 6;
			this.executionViewer.EndCloseDocument += new System.EventHandler(this.childViewer_EndCloseDocument);
			this.executionViewer.EndOpenDocument += new System.EventHandler(this.childViewer_EndOpenDocument);
			this.executionViewer.ReleaseFocus += new System.EventHandler(this.childViewer_ReleaseFocus);
			this.executionViewer.ObjectOpen += new Shadows.Quasar.Common.ObjectOpenEventHandler(this.childviewer_ObjectOpen);
			// 
			// splitter2
			// 
			this.splitter2.Location = new System.Drawing.Point(256, 131);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(3, 99);
			this.splitter2.TabIndex = 0;
			this.splitter2.TabStop = false;
			// 
			// placementViewer
			// 
			this.placementViewer.BlockOrderId = 0;
			this.placementViewer.BlotterId = 0;
			this.placementViewer.Dock = System.Windows.Forms.DockStyle.Left;
			this.placementViewer.DocumentVersion = 0;
			this.placementViewer.IsRefreshNeeded = true;
			this.placementViewer.IsStylesheetChanged = true;
			this.placementViewer.Location = new System.Drawing.Point(0, 131);
			this.placementViewer.MoveAfterReturn = true;
			this.placementViewer.Name = "placementViewer";
			this.placementViewer.ResetViewer = true;
			this.placementViewer.Size = new System.Drawing.Size(256, 99);
			this.placementViewer.StylesheetId = 0;
			this.placementViewer.TabIndex = 5;
			this.placementViewer.EndCloseDocument += new System.EventHandler(this.childViewer_EndCloseDocument);
			this.placementViewer.EndOpenDocument += new System.EventHandler(this.childViewer_EndOpenDocument);
			this.placementViewer.ReleaseFocus += new System.EventHandler(this.childViewer_ReleaseFocus);
			this.placementViewer.ObjectOpen += new Shadows.Quasar.Common.ObjectOpenEventHandler(this.childviewer_ObjectOpen);
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 128);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(504, 3);
			this.splitter1.TabIndex = 0;
			this.splitter1.TabStop = false;
			// 
			// blockOrderViewer
			// 
			this.blockOrderViewer.BlotterId = 0;
			this.blockOrderViewer.Dock = System.Windows.Forms.DockStyle.Top;
			this.blockOrderViewer.DocumentVersion = 0;
			this.blockOrderViewer.IsBlockOrderChanged = false;
			this.blockOrderViewer.IsBlockOrderSelected = false;
			this.blockOrderViewer.IsRefreshNeeded = true;
			this.blockOrderViewer.IsStylesheetChanged = true;
			this.blockOrderViewer.Location = new System.Drawing.Point(0, 0);
			this.blockOrderViewer.MoveAfterReturn = false;
			this.blockOrderViewer.Name = "blockOrderViewer";
			this.blockOrderViewer.ResetViewer = true;
			this.blockOrderViewer.SelectedBlockOrderId = 0;
			this.blockOrderViewer.SelectedRowType = 0;
			this.blockOrderViewer.Size = new System.Drawing.Size(504, 128);
			this.blockOrderViewer.SortMethod = 0;
			this.blockOrderViewer.StylesheetId = 0;
			this.blockOrderViewer.TabIndex = 4;
			this.blockOrderViewer.Execution += new System.EventHandler(this.blockOrderViewer_Execution);
			this.blockOrderViewer.EndCloseDocument += new System.EventHandler(this.childViewer_EndCloseDocument);
			this.blockOrderViewer.Placement += new System.EventHandler(this.blockOrderViewer_Placement);
			this.blockOrderViewer.EndOpenDocument += new System.EventHandler(this.childViewer_EndOpenDocument);
			this.blockOrderViewer.CloseBlockOrder += new Shadows.Quasar.Viewers.BlockOrder.BlockOrderEventHandler(this.blockOrderViewer_CloseBlockOrder);
			this.blockOrderViewer.OpenBlockOrder += new Shadows.Quasar.Viewers.BlockOrder.BlockOrderEventHandler(this.blockOrderViewer_OpenBlockOrder);
			this.blockOrderViewer.ObjectOpen += new Shadows.Quasar.Common.ObjectOpenEventHandler(this.childviewer_ObjectOpen);
			// 
			// tabPageTicketSummary
			// 
			this.tabPageTicketSummary.Controls.Add(this.ticketViewer);
			this.tabPageTicketSummary.Location = new System.Drawing.Point(4, 22);
			this.tabPageTicketSummary.Name = "tabPageTicketSummary";
			this.tabPageTicketSummary.Size = new System.Drawing.Size(504, 230);
			this.tabPageTicketSummary.TabIndex = 3;
			this.tabPageTicketSummary.Text = "Ticket Summary";
			// 
			// ticketViewer
			// 
			this.ticketViewer.BlotterId = 0;
			this.ticketViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ticketViewer.DocumentVersion = 0;
			this.ticketViewer.IsRefreshNeeded = true;
			this.ticketViewer.IsStylesheetChanged = true;
			this.ticketViewer.Location = new System.Drawing.Point(0, 0);
			this.ticketViewer.MoveAfterReturn = true;
			this.ticketViewer.Name = "ticketViewer";
			this.ticketViewer.ResetViewer = true;
			this.ticketViewer.Size = new System.Drawing.Size(504, 230);
			this.ticketViewer.StylesheetId = 0;
			this.ticketViewer.TabIndex = 7;
			this.ticketViewer.EndCloseDocument += new System.EventHandler(this.childViewer_EndCloseDocument);
			this.ticketViewer.EndOpenDocument += new System.EventHandler(this.childViewer_EndOpenDocument);
			this.ticketViewer.ReleaseFocus += new System.EventHandler(this.childViewer_ReleaseFocus);
			this.ticketViewer.ObjectOpen += new Shadows.Quasar.Common.ObjectOpenEventHandler(this.childviewer_ObjectOpen);
			// 
			// tabPageOrderBook
			// 
			this.tabPageOrderBook.Controls.Add(this.orderBookViewer);
			this.tabPageOrderBook.Location = new System.Drawing.Point(4, 22);
			this.tabPageOrderBook.Name = "tabPageOrderBook";
			this.tabPageOrderBook.Size = new System.Drawing.Size(504, 230);
			this.tabPageOrderBook.TabIndex = 3;
			this.tabPageOrderBook.Text = "Order Book";
			// 
			// orderBookViewer
			// 
			this.orderBookViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.orderBookViewer.Location = new System.Drawing.Point(0, 0);
			this.orderBookViewer.Name = "orderBookViewer";
			this.orderBookViewer.Size = new System.Drawing.Size(504, 230);
			this.orderBookViewer.TabIndex = 0;
			// 
			// BlotterViewer
			// 
			this.Controls.Add(this.tabControl);
			this.Name = "BlotterViewer";
			this.Size = new System.Drawing.Size(512, 256);
			this.ReleaseFocus += new System.EventHandler(this.childViewer_ReleaseFocus);
			this.tabControl.ResumeLayout(false);
			this.tabPageMainBlotter.ResumeLayout(false);
			this.tabPageTicketSummary.ResumeLayout(false);
			this.tabPageOrderBook.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Opens the Blotter Viewer.
		/// </summary>
		public override void OpenViewer()
		{

			// Open the Blotter Viewer in the background.
			ExecuteCommand(Command.OpenViewer);

		}

		/// <summary>
		/// Closes the Blotter Viewer.
		/// </summary>
		public override void CloseViewer()
		{

			// Close the Blotter Viewer in the background.
			ExecuteCommand(Command.CloseViewer);

		}

		
		/// <summary>
		/// Opens the document for the given object and it's argument.
		/// </summary>
		/// <param name="blotterId">The primary identifier of the object to open.</param>
		/// <param name="argument">Options that can be used to further specify the document's properties.</param>
		public override void OpenDocument(int blotterId)
		{

			// The rest of the initialization of this document must be done in the background.
			ExecuteCommand(Command.OpenDocument, blotterId);

		}

		/// <summary>
		/// Closes the Blotter Document.
		/// </summary>
		public override void CloseDocument()
		{

			// Execute a command in the background to close the document.
			ExecuteCommand(Command.CloseDocument);

		}

		/// <summary>
		/// Arbiter of commands to be executed on the command thread.
		/// </summary>
		/// <param name="command">The command</param>
		/// <param name="key">A command specific identifier for the object of the command.</param>
		/// <param name="argument">Command specific argument, that is, additional data used to execute the command.</param>
		protected override void ThreadHandler(int command, object key, object argument)
		{

			// The most likely errors will be parsing errors.
			try
			{

				// This section will parse the key element and argument, then call a method to handle the command.  There is also some
				// basic field validation that takes place here.
				switch (command)
				{

					case Command.OpenViewer:

						// Open the Blotter Viewer
						OpenViewerCommand();
						break;

					case Command.CloseViewer:

						// Closes the Blotter Viewer
						CloseViewerCommand();
						break;

					case Command.OpenDocument:

						// Open the Blotter Document
						OpenDocumentCommand((int)key);
						break;

					case Command.CloseDocument:

						// Close the Blotter Document
						CloseDocumentCommand();
						break;

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}

		/// <summary>
		/// Opens the Blotter Biewer.
		/// </summary>
		private void OpenViewerCommand()
		{

			// Open up each of the views for the blotter.  Note that we can only clear the execution and placement
			// viewers.  An 'Open' command has no relevance until there is a block order to be viewed.
			this.blockOrderViewer.OpenViewer();
			this.placementViewer.OpenViewer();
			this.executionViewer.OpenViewer();
			this.ticketViewer.OpenViewer();

			// Broadcast the event that the viewer is now open.
			OnEndOpenViewer();

		}
						
		/// <summary>
		/// Closes out the blotter document and all the child viewers.
		/// </summary>
		private void CloseViewerCommand()
		{

			// Close out the child viewers.
			this.blockOrderViewer.CloseViewer();
			this.placementViewer.CloseViewer();
			this.executionViewer.CloseViewer();
			this.ticketViewer.CloseViewer();

			// Broadcast the event that the viewer is now closed.
			OnEndCloseViewer();

		}
		
		/// <summary>
		/// Opens the Blotter Document.
		/// </summary>
		/// <param name="blotterId">The blotter identifier.</param>
		private void OpenDocumentCommand(int blotterId)
		{

			try
			{

				// The Blotter Identifier Attribute.
				this.blotterId = blotterId;

				// Lock the tables.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.StylesheetLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Each blotter can have a stylesheet assigned to it so Fixed Income traders view Fixed Income data, 
				// equity traders Equity data, and so forth.  If no blotter is assigned, a default will be provided.
				ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(blotterId);
				if (blotterRow == null)
					throw new ArgumentException("This blotter has been deleted", blotterId.ToString());

				// If a viewer is avaiable for the objects associated with the blotter, then we'll enable the viewers for
				// those objects.  For example, debt blotters don't require placement viewers, so there won't be one
				// associated with that blotter.
				this.hasBlockOrderViewer = !blotterRow.IsBlockOrderStylesheetIdNull();
				this.hasPlacementViewer = !blotterRow.IsPlacementStylesheetIdNull();
				this.hasExecutionViewer = !blotterRow.IsExecutionStylesheetIdNull();
				this.hasTicketViewer = !blotterRow.IsTicketStylesheetIdNull();
				this.hasOrderBookViewer = true;

			}
			catch (Exception exception)
			{

				// Write the error out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld) ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.StylesheetLock.IsReaderLockHeld) ClientMarketData.StylesheetLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Open up each of the views for the blotter.
			if (this.hasBlockOrderViewer) this.blockOrderViewer.OpenDocument(this.blotterId);
			if (this.hasExecutionViewer) this.executionViewer.OpenDocument(this.blotterId);
			if (this.hasPlacementViewer) this.placementViewer.OpenDocument(this.blotterId);
			if (this.hasTicketViewer) this.ticketViewer.OpenDocument(this.blotterId);
			if (this.hasOrderBookViewer) this.orderBookViewer.OpenDocument(this.blotterId);

		}
						
		/// <summary>
		/// Closes out the blotter document and all the child viewers.
		/// </summary>
		private void CloseDocumentCommand()
		{

			// Close out the viewers if they've been opened.
			if (this.hasBlockOrderViewer) this.blockOrderViewer.CloseDocument();
			if (this.hasPlacementViewer) this.placementViewer.CloseDocument();
			if (this.hasExecutionViewer) this.executionViewer.CloseDocument();
			if (this.hasTicketViewer) this.ticketViewer.CloseDocument();
			if (this.hasOrderBookViewer) this.orderBookViewer.CloseDocument();

		}
		
		/// <summary>
		/// Saves the current document.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSaveAs_Click(object sender, System.EventArgs e)
		{

			// Call the base class to save the Spreadsheet data.
			this.blockOrderViewer.SaveAs();

		}

		/// <summary>
		/// Loads the configuration of the Blotter Screens from the application settings file.
		/// </summary>
		public override void LoadSettings()
		{

			try
			{

				// The blotter is made up of several views.  Each may be independently visible or hidden.
				if (AppSettings.FindKey("blotterViewer.blockOrderViewer.Visible") != null)
				{

					bool blockOrderViewerVisible = AppSettings.GetBoolean("blotterViewer.blockOrderViewer.Visible");
					bool placementViewerVisible = AppSettings.GetBoolean("blotterViewer.placementViewer.Visible");
					bool executionViewerVisible = AppSettings.GetBoolean("blotterViewer.executionViewer.Visible");

					// BlockOrderViewer
					this.blockOrderViewer.Visible = blockOrderViewerVisible;
					this.blockOrderViewer.Location = AppSettings.GetPoint("blotterViewer.blockOrderViewer.Location");
					this.blockOrderViewer.Size = AppSettings.GetSize("blotterViewer.blockOrderViewer.Size");

					// Splitter1
					this.splitter1.Visible = blockOrderViewerVisible && (placementViewerVisible || executionViewerVisible);
					this.splitter1.Location = AppSettings.GetPoint("blotterViewer.splitter1.Location");
					this.splitter1.Size = AppSettings.GetSize("blotterViewer.splitter1.Size");

					// PlacementViewer
					this.placementViewer.Visible = placementViewerVisible;
					this.placementViewer.Location = AppSettings.GetPoint("blotterViewer.placementViewer.Location");
					this.placementViewer.Size = AppSettings.GetSize("blotterViewer.placementViewer.Size");

					// Splitter2
					this.splitter2.Visible = placementViewerVisible && executionViewerVisible;
					this.splitter2.Location = AppSettings.GetPoint("blotterViewer.splitter2.Location");
					this.splitter2.Size = AppSettings.GetSize("blotterViewer.splitter2.Size");

					// ExecutionViewer
					this.executionViewer.Visible = executionViewerVisible;
					this.executionViewer.Location = AppSettings.GetPoint("blotterViewer.executionViewer.Location");
					this.executionViewer.Size = AppSettings.GetSize("blotterViewer.executionViewer.Size");

				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}

		}
		
		/// <summary>
		/// Saves the configuration of the blotter.
		/// </summary>
		public override void SaveSettings()
		{

			// BlockOrderViewer
			AppSettings.SetValue("blotterViewer.blockOrderViewer.Visible", this.blockOrderViewer.Visible);
			AppSettings.SetValue("blotterViewer.blockOrderViewer.Location", this.blockOrderViewer.Location);
			AppSettings.SetValue("blotterViewer.blockOrderViewer.Size", this.blockOrderViewer.Size);

			// PlacementViewer
			AppSettings.SetValue("blotterViewer.placementViewer.Visible", this.placementViewer.Visible);
			AppSettings.SetValue("blotterViewer.placementViewer.Location", this.placementViewer.Location);
			AppSettings.SetValue("blotterViewer.placementViewer.Size", this.placementViewer.Size);

			// ExecutionViewer
			AppSettings.SetValue("blotterViewer.executionViewer.Visible", this.executionViewer.Visible);
			AppSettings.SetValue("blotterViewer.executionViewer.Location", this.executionViewer.Location);
			AppSettings.SetValue("blotterViewer.executionViewer.Size", this.executionViewer.Size);

			// Splitter1
			AppSettings.SetValue("blotterViewer.splitter1.Location", this.splitter1.Location);
			AppSettings.SetValue("blotterViewer.splitter1.Size", this.splitter1.Size);

			// Splitter2
			AppSettings.SetValue("blotterViewer.splitter2.Location", this.splitter2.Location);
			AppSettings.SetValue("blotterViewer.splitter2.Size", this.splitter2.Size);

		}
		
		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSortAscendingName_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.blockOrderViewer.SortMethod = 0;
			this.blockOrderViewer.DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSortAscendingQuantityExecuted_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.blockOrderViewer.SortMethod = 1;
			this.blockOrderViewer.DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSortAscendingQuantityLeaves_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.blockOrderViewer.SortMethod = 2;
			this.blockOrderViewer.DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSortAscendingQuantityOrdered_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.blockOrderViewer.SortMethod = 3;
			this.blockOrderViewer.DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSortAscendingSymbol_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.blockOrderViewer.SortMethod = 4;
			this.blockOrderViewer.DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSortDescendingName_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.blockOrderViewer.SortMethod = 100;
			this.blockOrderViewer.DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSortDescendingQuantityExecuted_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.blockOrderViewer.SortMethod = 101;
			this.blockOrderViewer.DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSortDescendingQuantityLeaves_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.blockOrderViewer.SortMethod = 102;
			this.blockOrderViewer.DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSortDescendingQuantityOrdered_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.blockOrderViewer.SortMethod = 103;
			this.blockOrderViewer.DrawDocument();

		}

		/// <summary>
		/// Sorts the blotter.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemSortDescendingSymbol_Click(object sender, System.EventArgs e)
		{

			// Change the sort method and force the blotter to refresh the entire document.
			this.blockOrderViewer.SortMethod = 104;
			this.blockOrderViewer.DrawDocument();

		}

		/// <summary>
		/// Synchronizes the other views with the currently selected block.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event Arguments.</param>
		private void blockOrderViewer_OpenBlockOrder(object sender, Shadows.Quasar.Viewers.BlockOrder.BlockOrderEventArgs e)
		{

			// Pass the event along to the placement viewer.
			if (this.hasPlacementViewer)
				this.placementViewer.OpenBlockOrder(e.BlockOrderId);

			// Pass the event along to the execution viewer.
			if (this.hasExecutionViewer)
				this.executionViewer.OpenBlockOrder(e.BlockOrderId);

			// Pass the event along to the Order Book Viewer.
			if (this.hasOrderBookViewer)
				this.orderBookViewer.OpenBlockOrder(e.BlockOrderId);

		}

		/// <summary>
		/// Synchronizes the other views with the currently selected block.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event Arguments.</param>
		private void blockOrderViewer_CloseBlockOrder(object sender, Shadows.Quasar.Viewers.BlockOrder.BlockOrderEventArgs e)
		{

			// Pass the event along to the placement viewer.
			if (this.hasPlacementViewer)
				this.placementViewer.CloseBlockOrder(e.BlockOrderId);

			// Pass the event along to the execution viewer.
			if (this.hasExecutionViewer)
				this.executionViewer.CloseBlockOrder(e.BlockOrderId);

			// Pass the event along to the Order Book Viewer.
			if (this.hasOrderBookViewer)
				this.orderBookViewer.CloseBlockOrder(e.BlockOrderId);

		}

		private void toolBarStandard_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{

			try
			{

				// Pass the command to delete the current item to the window that has the focus.
				if (e.Button == this.toolBarButtonDelete)
				{
					if (this.executionViewer.ContainsFocus)
						this.executionViewer.DeleteActiveRow();
					if (this.placementViewer.ContainsFocus)
						this.placementViewer.DeleteActiveRow();
				}

				if (e.Button == this.toolBarButtonNew)
				{

					OrderEntryDialog orderEntryDialog = new OrderEntryDialog(this.blotterId);

					if (orderEntryDialog.ShowDialog() == DialogResult.OK && orderEntryDialog.LocalOrderSet.Order.Count != 0)
						ServiceQueue.Execute(new ThreadHandler(InsertLocalOrderThread), orderEntryDialog.LocalOrderSet);

					// Dialog boxes are only hidden when they exit.  They must be disposed of to release the resources.
					orderEntryDialog.Dispose();

				}
			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}


		}

		/// <summary>
		/// This thread updates the sector targets in a model.
		/// </summary>
		/// <param name="objectStart">Thread initialization parameters.</param>
		private void InsertLocalOrderThread(params object[] argument)
		{

			// Extract the initialization parameters.
			LocalOrderSet localOrderSet = (LocalOrderSet)argument[0];

			// This is the command batch that will be sent to the server when we have created all the orders.
			RemoteBatch remoteBatch = new RemoteBatch();
			RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Trading");
			RemoteType orderType = remoteAssembly.Types.Add("Shadows.WebService.Trading.Order");

			foreach (LocalOrderSet.OrderRow localOrder in localOrderSet.Order)
			{

				// Add the order.
				RemoteMethod remoteMethod = orderType.Methods.Add("Insert");
				remoteMethod.Parameters.Add("orderId", DataType.Int, Direction.ReturnValue);
				remoteMethod.Parameters.Add("blotterId", this.blotterId);
				remoteMethod.Parameters.Add("accountId", localOrder.AccountId);
				remoteMethod.Parameters.Add("securityId", localOrder.SecurityId);
				remoteMethod.Parameters.Add("settlementId", localOrder.SettlementId);
				remoteMethod.Parameters.Add("brokerId", localOrder.IsBrokerIdNull() ? (object)DBNull.Value : localOrder.BrokerId);
				remoteMethod.Parameters.Add("transactionTypeCode", localOrder.TransactionTypeCode);
				remoteMethod.Parameters.Add("timeInForceCode", localOrder.TimeInForceCode);
				remoteMethod.Parameters.Add("orderTypeCode", localOrder.OrderTypeCode);
				remoteMethod.Parameters.Add("conditionCode", localOrder.IsConditionCodeNull() ? (object)DBNull.Value : localOrder.ConditionCode);
				remoteMethod.Parameters.Add("quantity", localOrder.Quantity);
				remoteMethod.Parameters.Add("price1", localOrder.IsPrice1Null() ? (object)DBNull.Value : localOrder.Price1);
				remoteMethod.Parameters.Add("price2", localOrder.IsPrice2Null() ? (object)DBNull.Value : (object)localOrder.Price2);
				remoteMethod.Parameters.Add("note", localOrder.IsNoteNull() ? (object)DBNull.Value : (object)localOrder.Note);

			}

			ClientMarketData.Send(remoteBatch);

		}

		/// <summary>
		/// Handles a loss of focus to the placement viewer.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void childViewer_ReleaseFocus(object sender, System.EventArgs e)
		{

			// Give the focus back to the block order viewer.
			this.blockOrderViewer.Focus();

		}

		/// <summary>
		/// Handles the entry of an execution to the block.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void blockOrderViewer_Execution(object sender, EventArgs e)
		{

			// Selecting another child window must be done in the foreground.
			Invoke(this.executionDelegate, new object[] {this, EventArgs.Empty});
			
		}

		private void ExecutionForeground(object sender, EventArgs e)
		{

			// Call the execution viewer to start a new execution with the value entered by the user onto the block.
			this.executionViewer.Focus();
			this.executionViewer.Select();

		}

		/// <summary>
		/// Handles the entry of an placement to the block.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void blockOrderViewer_Placement(object sender, EventArgs e)
		{
		
			// Selecting another child window must be done in the foreground.
			Invoke(this.placementDelegate, new object[] {this, EventArgs.Empty});

		}

		private void PlacementForeground(object sender, EventArgs e)
		{

			// Call the placement viewer to start a new placement with the value entered by the user onto the block.
			this.placementViewer.Focus();
			this.placementViewer.Select();

		}

		/// <summary>
		/// Called when a child window is ready to be displayed.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">Event argument.</param>
		private void childViewer_EndOpenDocument(object sender, System.EventArgs e)
		{

			// The blotter isn't considered 'Opened' until all the child viewers have been opened.
			if ((this.blockOrderViewer.IsDocumentOpen || !this.hasBlockOrderViewer) &&
				(this.executionViewer.IsDocumentOpen || !this.hasExecutionViewer) &&
				(this.placementViewer.IsDocumentOpen || !this.hasPlacementViewer) &&
				(this.ticketViewer.IsDocumentOpen || !this.hasTicketViewer) &&
				(this.orderBookViewer.IsDocumentOpen || !this.hasOrderBookViewer))
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

			// The blotter isn't considered 'Closeed' until all the child viewers have been opened.
			if ((!this.blockOrderViewer.IsDocumentOpen || !this.hasBlockOrderViewer) &&
				(!this.executionViewer.IsDocumentOpen || !this.hasExecutionViewer) &&
				(!this.placementViewer.IsDocumentOpen || !this.hasPlacementViewer) &&
				(!this.ticketViewer.IsDocumentOpen || !this.hasTicketViewer) &&
				(!this.orderBookViewer.IsDocumentOpen || !this.hasOrderBookViewer))
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
		
			// WORKAROUND: The Forms Library appears to select a window when you make it visible, even if the parent
			// window doesn't have the focus.  That is, when a window is made visible because of a new configuration of 
			// the blotter, that new window is given the focus.  This will disturb the flow of input in another window.
			// For example, the focus may be in the tree view and the user is scrolling through blotters.  If a change in
			// blotters makes visible a previously unseen viewer, that viewer will recieve the focus.  By disabling the
			// input on the controls while they are reconfigured, this action can be inhibited.
			this.Enabled = false;
			
			// Suspend Updates while we re-arrange the viewers.
			SuspendLayout();

			// Make the block orders, placements and executions visible.
			this.blockOrderViewer.Visible = this.hasBlockOrderViewer;
			this.splitter1.Visible = this.hasPlacementViewer || this.hasExecutionViewer;
			this.placementViewer.Visible = this.hasPlacementViewer;
			this.splitter2.Visible = this.hasPlacementViewer && this.hasExecutionViewer;
			this.executionViewer.Visible = this.hasExecutionViewer;

			// Redraw the reconfigured screen.
			ResumeLayout();

			// WORKAROUND: This will allow the viewers to get the input focus when the user decides the time is right.
			this.Enabled = true;

		}
		
		private void childviewer_ObjectOpen(object sender, Shadows.Quasar.Common.ObjectArgs event_args)
		{

			// Pass the request to open an object on to the container.
			this.OnObjectOpen(sender, event_args);
		
		}

		private void menuItemRules_Click(object sender, System.EventArgs e)
		{
			RuleCompiler ruleCompiler = new RuleCompiler();
			ruleCompiler.ShowDialog(this);
		}

		private void menuRunOnce_Click(object sender, System.EventArgs e)
		{

			if (RuleCompiler.CompilerResults != null)
				Language.ExecuteOnce(RuleCompiler.CompilerResults);

		}

		private void menuItemRunContinuously_Click(object sender, System.EventArgs e)
		{

			ThreadArgument threadArgument = new ThreadArgument(new ThreadHandler(Language.ExecutionCommand),
				RuleCompiler.CompilerResults);
			Thread thread = new Thread(new ThreadStart(threadArgument.StartThread));
			thread.IsBackground = true;
			thread.Start();
		
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			this.placementViewer.Enabled = true;
			this.placementViewer.Select();
		}

		private void menuItem4_Click(object sender, System.EventArgs e)
		{

			this.blockOrderViewer.Select();

		}

	}

}
