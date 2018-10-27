namespace MarkThree.Guardian.Forms
{
	partial class BlotterViewer
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

			// Manually dispose of any tab pages that have been removed from the tab control in order to hide them.
			if (this.tabControl != null)
			{
				if (!this.tabControl.TabPages.Contains(this.tabPageExecution))
					this.tabPageExecution.Dispose();
			}

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
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageWorking = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.workingOrderViewer = new MarkThree.Guardian.Forms.WorkingOrderViewer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.blotterDetailViewer = new MarkThree.Guardian.Forms.BlotterDetailViewer();
			this.consoleViewer = new MarkThree.Forms.ConsoleViewer();
			this.tabPageMatch = new System.Windows.Forms.TabPage();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.matchViewer = new MarkThree.Guardian.Forms.MatchViewer();
			this.negotiationConsole = new MarkThree.Guardian.Forms.NegotiationConsole();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.matchHistoryViewer = new MarkThree.Guardian.Forms.MatchHistoryViewer();
			this.splitContainer5 = new System.Windows.Forms.SplitContainer();
			this.orderBookViewer = new MarkThree.Guardian.Forms.OrderBookViewer();
			this.splitContainer6 = new System.Windows.Forms.SplitContainer();
			this.quoteViewer = new MarkThree.Guardian.Forms.QuoteViewer();
			this.volumeChartViewer = new MarkThree.Guardian.Forms.VolumeChartViewer();
			this.tabPageExecution = new System.Windows.Forms.TabPage();
			this.executionViewer = new MarkThree.Guardian.ExecutionViewer();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.customizeWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.workingOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.executionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.detailWindowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.executionWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.detailWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.consoleWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.quoteWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.blotterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mergeBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unmergeBlockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.manualExecutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.orderFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonExitBox = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonEnterBox = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonOrderForm = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonPrint = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonCut = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonCopy = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonPaste = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonFilter1 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonFilter2 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonFilter3 = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonShowMatch = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonShowVolume = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.tabControl.SuspendLayout();
			this.tabPageWorking.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.tabPageMatch.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			this.splitContainer5.Panel1.SuspendLayout();
			this.splitContainer5.Panel2.SuspendLayout();
			this.splitContainer5.SuspendLayout();
			this.splitContainer6.Panel1.SuspendLayout();
			this.splitContainer6.Panel2.SuspendLayout();
			this.splitContainer6.SuspendLayout();
			this.tabPageExecution.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPageWorking);
			this.tabControl.Controls.Add(this.tabPageMatch);
			this.tabControl.Controls.Add(this.tabPageExecution);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabControl.ItemSize = new System.Drawing.Size(70, 30);
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(612, 256);
			this.tabControl.TabIndex = 0;
			// 
			// tabPageWorking
			// 
			this.tabPageWorking.Controls.Add(this.splitContainer1);
			this.tabPageWorking.Location = new System.Drawing.Point(4, 34);
			this.tabPageWorking.Name = "tabPageWorking";
			this.tabPageWorking.Size = new System.Drawing.Size(604, 218);
			this.tabPageWorking.TabIndex = 0;
			this.tabPageWorking.Text = "Working";
			this.tabPageWorking.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.workingOrderViewer);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Panel2Collapsed = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer1Panel2Collapsed;
			this.splitContainer1.Size = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer1Size;
			this.splitContainer1.SplitterDistance = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer1SplitterDistance;
			this.splitContainer1.TabIndex = 0;
			// 
			// workingOrderViewer
			// 
			this.workingOrderViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.workingOrderViewer.Icon = null;
			this.workingOrderViewer.Location = new System.Drawing.Point(0, 0);
			this.workingOrderViewer.Name = "workingOrderViewer";
			this.workingOrderViewer.Size = new System.Drawing.Size(604, 128);
			this.workingOrderViewer.TabIndex = 5;
			this.workingOrderViewer.EndOpenDocument += new System.EventHandler(this.childViewer_EndOpenDocument);
			this.workingOrderViewer.OpenWorkingOrder += new MarkThree.Guardian.WorkingOrderEventHandler(this.workingOrderViewer_OpenWorkingOrder);
			this.workingOrderViewer.OrderFormSelected += new System.EventHandler(this.orderFormToolStripMenuItem_Click);
			this.workingOrderViewer.CloseWorkingOrder += new System.EventHandler(this.workingOrderViewer_CloseWorkingOrder);
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.blotterDetailViewer);
			this.splitContainer2.Panel1Collapsed = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer2Panel1Collapsed;
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.consoleViewer);
			this.splitContainer2.Panel2Collapsed = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer2Panel2Collapsed;
			this.splitContainer2.Size = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer2Size;
			this.splitContainer2.SplitterDistance = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer2SplitterDistance;
			this.splitContainer2.TabIndex = 0;
			// 
			// blotterDetailViewer
			// 
			this.blotterDetailViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.blotterDetailViewer.Icon = null;
			this.blotterDetailViewer.Location = new System.Drawing.Point(0, 0);
			this.blotterDetailViewer.Name = "blotterDetailViewer";
			this.blotterDetailViewer.Size = new System.Drawing.Size(256, 86);
			this.blotterDetailViewer.TabIndex = 2;
			this.blotterDetailViewer.EndOpenDocument += new System.EventHandler(this.childViewer_EndOpenDocument);
			// 
			// consoleViewer
			// 
			this.consoleViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.consoleViewer.Icon = null;
			this.consoleViewer.Location = new System.Drawing.Point(0, 0);
			this.consoleViewer.MaximumLength = 32;
			this.consoleViewer.Name = "consoleViewer";
			this.consoleViewer.Size = new System.Drawing.Size(344, 86);
			this.consoleViewer.TabIndex = 4;
			this.consoleViewer.EndOpenDocument += new System.EventHandler(this.childViewer_EndOpenDocument);
			// 
			// tabPageMatch
			// 
			this.tabPageMatch.Controls.Add(this.splitContainer3);
			this.tabPageMatch.Location = new System.Drawing.Point(4, 34);
			this.tabPageMatch.Name = "tabPageMatch";
			this.tabPageMatch.Size = new System.Drawing.Size(604, 218);
			this.tabPageMatch.TabIndex = 1;
			this.tabPageMatch.Text = "Match";
			this.tabPageMatch.UseVisualStyleBackColor = true;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.matchViewer);
			this.splitContainer3.Panel1.Controls.Add(this.negotiationConsole);
			this.splitContainer3.Panel1MinSize = 100;
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
			this.splitContainer3.Size = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer3Size;
			this.splitContainer3.SplitterDistance = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer3SplitterDistance;
			this.splitContainer3.TabIndex = 0;
			// 
			// matchViewer
			// 
			this.matchViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.matchViewer.Icon = null;
			this.matchViewer.Location = new System.Drawing.Point(0, 0);
			this.matchViewer.Name = "matchViewer";
			this.matchViewer.Size = new System.Drawing.Size(284, 100);
			this.matchViewer.TabIndex = 0;
			this.matchViewer.EndOpenDocument += new System.EventHandler(this.childViewer_EndOpenDocument);
			this.matchViewer.SelectionChanged += new System.EventHandler(this.matchViewer_SelectionChanged);
			// 
			// negotiationConsole
			// 
			this.negotiationConsole.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.negotiationConsole.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.negotiationConsole.Dock = System.Windows.Forms.DockStyle.Right;
			this.negotiationConsole.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.negotiationConsole.Icon = null;
			this.negotiationConsole.Location = new System.Drawing.Point(284, 0);
			this.negotiationConsole.Name = "negotiationConsole";
			this.negotiationConsole.Size = new System.Drawing.Size(320, 100);
			this.negotiationConsole.TabIndex = 1;
			// 
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add(this.matchHistoryViewer);
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.splitContainer5);
			this.splitContainer4.Size = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer4Size;
			this.splitContainer4.SplitterDistance = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer4SplitterDistance;
			this.splitContainer4.TabIndex = 0;
			// 
			// matchHistoryViewer
			// 
			this.matchHistoryViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.matchHistoryViewer.Icon = null;
			this.matchHistoryViewer.Location = new System.Drawing.Point(0, 0);
			this.matchHistoryViewer.Name = "matchHistoryViewer";
			this.matchHistoryViewer.Size = new System.Drawing.Size(604, 75);
			this.matchHistoryViewer.TabIndex = 0;
			// 
			// splitContainer5
			// 
			this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer5.Location = new System.Drawing.Point(0, 0);
			this.splitContainer5.Name = "splitContainer5";
			// 
			// splitContainer5.Panel1
			// 
			this.splitContainer5.Panel1.Controls.Add(this.orderBookViewer);
			// 
			// splitContainer5.Panel2
			// 
			this.splitContainer5.Panel2.Controls.Add(this.splitContainer6);
			this.splitContainer5.Size = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer5Size;
			this.splitContainer5.SplitterDistance = global::MarkThree.Guardian.Forms.Settings.Default.SplitContainer5SplitterDistance;
			this.splitContainer5.TabIndex = 0;
			// 
			// orderBookViewer
			// 
			this.orderBookViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.orderBookViewer.Icon = null;
			this.orderBookViewer.Location = new System.Drawing.Point(0, 0);
			this.orderBookViewer.Name = "orderBookViewer";
			this.orderBookViewer.Size = new System.Drawing.Size(171, 35);
			this.orderBookViewer.TabIndex = 0;
			// 
			// splitContainer6
			// 
			this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer6.Location = new System.Drawing.Point(0, 0);
			this.splitContainer6.Name = "splitContainer6";
			// 
			// splitContainer6.Panel1
			// 
			this.splitContainer6.Panel1.Controls.Add(this.quoteViewer);
			// 
			// splitContainer6.Panel2
			// 
			this.splitContainer6.Panel2.Controls.Add(this.volumeChartViewer);
			this.splitContainer6.Size = new System.Drawing.Size(429, 35);
			this.splitContainer6.SplitterDistance = 196;
			this.splitContainer6.TabIndex = 5;
			// 
			// quoteViewer
			// 
			this.quoteViewer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.quoteViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.quoteViewer.Icon = null;
			this.quoteViewer.Location = new System.Drawing.Point(0, 0);
			this.quoteViewer.Name = "quoteViewer";
			this.quoteViewer.Size = new System.Drawing.Size(196, 35);
			this.quoteViewer.TabIndex = 0;
			this.quoteViewer.Tag = -2147483648;
			// 
			// volumeChartViewer
			// 
			this.volumeChartViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.volumeChartViewer.Icon = null;
			this.volumeChartViewer.Location = new System.Drawing.Point(0, 0);
			this.volumeChartViewer.Name = "volumeChartViewer";
			this.volumeChartViewer.Size = new System.Drawing.Size(229, 35);
			this.volumeChartViewer.TabIndex = 4;
			// 
			// tabPageExecution
			// 
			this.tabPageExecution.Controls.Add(this.executionViewer);
			this.tabPageExecution.Location = new System.Drawing.Point(4, 34);
			this.tabPageExecution.Name = "tabPageExecution";
			this.tabPageExecution.Size = new System.Drawing.Size(604, 218);
			this.tabPageExecution.TabIndex = 4;
			this.tabPageExecution.Text = "Execution";
			this.tabPageExecution.UseVisualStyleBackColor = true;
			// 
			// executionViewer
			// 
			this.executionViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.executionViewer.Icon = null;
			this.executionViewer.Location = new System.Drawing.Point(0, 0);
			this.executionViewer.Name = "executionViewer";
			this.executionViewer.Size = new System.Drawing.Size(604, 218);
			this.executionViewer.TabIndex = 0;
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.blotterToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(612, 24);
			this.menuStrip.TabIndex = 5;
			this.menuStrip.Text = "menuStrip";
			this.menuStrip.Visible = false;
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAsToolStripMenuItem});
			this.fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.saveAsToolStripMenuItem.MergeIndex = 1;
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.saveAsToolStripMenuItem.Text = "&Save As...";
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.customizeWindowToolStripMenuItem,
            this.detailWindowToolStripMenuItem,
            this.consoleWindowToolStripMenuItem,
            this.quoteWindowToolStripMenuItem});
			this.viewToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(172, 6);
			// 
			// customizeWindowToolStripMenuItem
			// 
			this.customizeWindowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.workingOrderToolStripMenuItem,
            this.executionToolStripMenuItem,
            this.detailWindowToolStripMenuItem1});
			this.customizeWindowToolStripMenuItem.Name = "customizeWindowToolStripMenuItem";
			this.customizeWindowToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.customizeWindowToolStripMenuItem.Text = "&Customize Window";
			// 
			// workingOrderToolStripMenuItem
			// 
			this.workingOrderToolStripMenuItem.Name = "workingOrderToolStripMenuItem";
			this.workingOrderToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.workingOrderToolStripMenuItem.Text = "&Working Order";
			this.workingOrderToolStripMenuItem.Click += new System.EventHandler(this.workingOrderToolStripMenuItem_Click);
			// 
			// executionToolStripMenuItem
			// 
			this.executionToolStripMenuItem.Name = "executionToolStripMenuItem";
			this.executionToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.executionToolStripMenuItem.Text = "&Execution";
			this.executionToolStripMenuItem.Click += new System.EventHandler(this.executionToolStripMenuItem_Click);
			// 
			// detailWindowToolStripMenuItem1
			// 
			this.detailWindowToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.executionWindowToolStripMenuItem});
			this.detailWindowToolStripMenuItem1.Name = "detailWindowToolStripMenuItem1";
			this.detailWindowToolStripMenuItem1.Size = new System.Drawing.Size(155, 22);
			this.detailWindowToolStripMenuItem1.Text = "&Detail Window";
			// 
			// executionWindowToolStripMenuItem
			// 
			this.executionWindowToolStripMenuItem.Name = "executionWindowToolStripMenuItem";
			this.executionWindowToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.executionWindowToolStripMenuItem.Text = "&Execution";
			this.executionWindowToolStripMenuItem.Click += new System.EventHandler(this.executionToolStripMenuItem_Click_1);
			// 
			// detailWindowToolStripMenuItem
			// 
			this.detailWindowToolStripMenuItem.Name = "detailWindowToolStripMenuItem";
			this.detailWindowToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.detailWindowToolStripMenuItem.Text = "&Detail Window";
			this.detailWindowToolStripMenuItem.Click += new System.EventHandler(this.menuItemDetailWindow_Click);
			// 
			// consoleWindowToolStripMenuItem
			// 
			this.consoleWindowToolStripMenuItem.Name = "consoleWindowToolStripMenuItem";
			this.consoleWindowToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.consoleWindowToolStripMenuItem.Text = "&Console Window";
			this.consoleWindowToolStripMenuItem.Click += new System.EventHandler(this.consoleWindowToolStripMenuItem_Click);
			// 
			// quoteWindowToolStripMenuItem
			// 
			this.quoteWindowToolStripMenuItem.Checked = true;
			this.quoteWindowToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.quoteWindowToolStripMenuItem.Name = "quoteWindowToolStripMenuItem";
			this.quoteWindowToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
			this.quoteWindowToolStripMenuItem.Text = "&Quote Window";
			this.quoteWindowToolStripMenuItem.Click += new System.EventHandler(this.quoteWindowToolStripMenuItem_Click);
			// 
			// blotterToolStripMenuItem
			// 
			this.blotterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mergeBlockToolStripMenuItem,
            this.unmergeBlockToolStripMenuItem,
            this.manualExecutionToolStripMenuItem,
            this.orderFormToolStripMenuItem});
			this.blotterToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.blotterToolStripMenuItem.MergeIndex = 3;
			this.blotterToolStripMenuItem.Name = "blotterToolStripMenuItem";
			this.blotterToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
			this.blotterToolStripMenuItem.Text = "&Blotter";
			// 
			// mergeBlockToolStripMenuItem
			// 
			this.mergeBlockToolStripMenuItem.Name = "mergeBlockToolStripMenuItem";
			this.mergeBlockToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.mergeBlockToolStripMenuItem.Text = "&Merge Block";
			// 
			// unmergeBlockToolStripMenuItem
			// 
			this.unmergeBlockToolStripMenuItem.Name = "unmergeBlockToolStripMenuItem";
			this.unmergeBlockToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.unmergeBlockToolStripMenuItem.Text = "&Unmerge Block";
			// 
			// manualExecutionToolStripMenuItem
			// 
			this.manualExecutionToolStripMenuItem.Name = "manualExecutionToolStripMenuItem";
			this.manualExecutionToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.manualExecutionToolStripMenuItem.Text = "Manual &Execution";
			// 
			// orderFormToolStripMenuItem
			// 
			this.orderFormToolStripMenuItem.Name = "orderFormToolStripMenuItem";
			this.orderFormToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
			this.orderFormToolStripMenuItem.Text = "&Order Form";
			this.orderFormToolStripMenuItem.Click += new System.EventHandler(this.orderFormToolStripMenuItem_Click);
			// 
			// toolStrip
			// 
			this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonExitBox,
            this.toolStripButtonEnterBox,
            this.toolStripSeparator2,
            this.toolStripButtonOrderForm,
            this.toolStripButtonPrint,
            this.toolStripButtonDelete,
            this.toolStripSeparator3,
            this.toolStripButtonCut,
            this.toolStripButtonCopy,
            this.toolStripButtonPaste,
            this.toolStripSeparator4,
            this.toolStripButtonFilter1,
            this.toolStripButtonFilter2,
            this.toolStripButtonFilter3,
            this.toolStripButtonShowMatch,
            this.toolStripButtonShowVolume,
            this.toolStripSeparator6});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(612, 39);
			this.toolStrip.TabIndex = 6;
			this.toolStrip.Text = "toolStrip";
			this.toolStrip.Visible = false;
			// 
			// toolStripButtonExitBox
			// 
			this.toolStripButtonExitBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonExitBox.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Trafficlight_Red_32x32;
			this.toolStripButtonExitBox.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonExitBox.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonExitBox.MergeIndex = 0;
			this.toolStripButtonExitBox.Name = "toolStripButtonExitBox";
			this.toolStripButtonExitBox.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonExitBox.Text = "Exit Matching Box";
			this.toolStripButtonExitBox.ToolTipText = "Remove Orders from the Box";
			this.toolStripButtonExitBox.Click += new System.EventHandler(this.toolStripButtonExitBox_Click);
			// 
			// toolStripButtonEnterBox
			// 
			this.toolStripButtonEnterBox.Checked = true;
			this.toolStripButtonEnterBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripButtonEnterBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonEnterBox.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Trafficlight_Green_32x32;
			this.toolStripButtonEnterBox.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonEnterBox.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonEnterBox.MergeIndex = 1;
			this.toolStripButtonEnterBox.Name = "toolStripButtonEnterBox";
			this.toolStripButtonEnterBox.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonEnterBox.Text = "toolStripButton1";
			this.toolStripButtonEnterBox.Click += new System.EventHandler(this.toolStripButtonEnterBox_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripSeparator2.MergeIndex = 2;
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
			// 
			// toolStripButtonOrderForm
			// 
			this.toolStripButtonOrderForm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonOrderForm.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Order_Form_32x32;
			this.toolStripButtonOrderForm.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonOrderForm.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonOrderForm.MergeIndex = 3;
			this.toolStripButtonOrderForm.Name = "toolStripButtonOrderForm";
			this.toolStripButtonOrderForm.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonOrderForm.Text = "toolStripButton1";
			this.toolStripButtonOrderForm.ToolTipText = "Order Form";
			this.toolStripButtonOrderForm.Click += new System.EventHandler(this.orderFormToolStripMenuItem_Click);
			// 
			// toolStripButtonPrint
			// 
			this.toolStripButtonPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonPrint.Enabled = false;
			this.toolStripButtonPrint.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Print_32x32;
			this.toolStripButtonPrint.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonPrint.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonPrint.MergeIndex = 4;
			this.toolStripButtonPrint.Name = "toolStripButtonPrint";
			this.toolStripButtonPrint.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonPrint.Text = "toolStripButton2";
			this.toolStripButtonPrint.ToolTipText = "Print";
			this.toolStripButtonPrint.Visible = false;
			// 
			// toolStripButtonDelete
			// 
			this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonDelete.Enabled = false;
			this.toolStripButtonDelete.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Delete_32x32;
			this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonDelete.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonDelete.MergeIndex = 5;
			this.toolStripButtonDelete.Name = "toolStripButtonDelete";
			this.toolStripButtonDelete.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonDelete.Text = "toolStripButton3";
			this.toolStripButtonDelete.ToolTipText = "Delete";
			this.toolStripButtonDelete.Visible = false;
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripSeparator3.MergeIndex = 6;
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
			this.toolStripSeparator3.Visible = false;
			// 
			// toolStripButtonCut
			// 
			this.toolStripButtonCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonCut.Enabled = false;
			this.toolStripButtonCut.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Cut_32x32;
			this.toolStripButtonCut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonCut.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonCut.MergeIndex = 7;
			this.toolStripButtonCut.Name = "toolStripButtonCut";
			this.toolStripButtonCut.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonCut.Text = "toolStripButton4";
			this.toolStripButtonCut.ToolTipText = "Cut";
			this.toolStripButtonCut.Visible = false;
			// 
			// toolStripButtonCopy
			// 
			this.toolStripButtonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonCopy.Enabled = false;
			this.toolStripButtonCopy.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Copy_32x32;
			this.toolStripButtonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonCopy.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonCopy.MergeIndex = 8;
			this.toolStripButtonCopy.Name = "toolStripButtonCopy";
			this.toolStripButtonCopy.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonCopy.Text = "toolStripButton5";
			this.toolStripButtonCopy.ToolTipText = "Copy";
			this.toolStripButtonCopy.Visible = false;
			// 
			// toolStripButtonPaste
			// 
			this.toolStripButtonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonPaste.Enabled = false;
			this.toolStripButtonPaste.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Paste_32x32;
			this.toolStripButtonPaste.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonPaste.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonPaste.MergeIndex = 9;
			this.toolStripButtonPaste.Name = "toolStripButtonPaste";
			this.toolStripButtonPaste.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonPaste.Text = "toolStripButton6";
			this.toolStripButtonPaste.ToolTipText = "Paste";
			this.toolStripButtonPaste.Visible = false;
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripSeparator4.MergeIndex = 10;
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 39);
			this.toolStripSeparator4.Visible = false;
			// 
			// toolStripButtonFilter1
			// 
			this.toolStripButtonFilter1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonFilter1.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Box_32x32;
			this.toolStripButtonFilter1.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonFilter1.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonFilter1.MergeIndex = 11;
			this.toolStripButtonFilter1.Name = "toolStripButtonFilter1";
			this.toolStripButtonFilter1.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonFilter1.Text = "toolStripButton9";
			this.toolStripButtonFilter1.ToolTipText = "Show All Orders";
			this.toolStripButtonFilter1.Click += new System.EventHandler(this.toolStripButton9_Click);
			// 
			// toolStripButtonFilter2
			// 
			this.toolStripButtonFilter2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonFilter2.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Box_Into_32x32;
			this.toolStripButtonFilter2.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonFilter2.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonFilter2.MergeIndex = 12;
			this.toolStripButtonFilter2.Name = "toolStripButtonFilter2";
			this.toolStripButtonFilter2.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonFilter2.Text = "toolStripButton10";
			this.toolStripButtonFilter2.ToolTipText = "Show Submitted Orders";
			this.toolStripButtonFilter2.Click += new System.EventHandler(this.toolStripButton10_Click);
			// 
			// toolStripButtonFilter3
			// 
			this.toolStripButtonFilter3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonFilter3.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Box_Delete_32x32;
			this.toolStripButtonFilter3.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonFilter3.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonFilter3.MergeIndex = 13;
			this.toolStripButtonFilter3.Name = "toolStripButtonFilter3";
			this.toolStripButtonFilter3.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonFilter3.Text = "toolStripButton11";
			this.toolStripButtonFilter3.ToolTipText = "Show Unsubmitted Orders";
			this.toolStripButtonFilter3.Click += new System.EventHandler(this.toolStripButton11_Click);
			// 
			// toolStripButtonShowMatch
			// 
			this.toolStripButtonShowMatch.Checked = true;
			this.toolStripButtonShowMatch.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripButtonShowMatch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonShowMatch.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Box_Preferences_32x32;
			this.toolStripButtonShowMatch.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonShowMatch.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonShowMatch.MergeIndex = 14;
			this.toolStripButtonShowMatch.Name = "toolStripButtonShowMatch";
			this.toolStripButtonShowMatch.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonShowMatch.Text = "toolStripButton8";
			this.toolStripButtonShowMatch.ToolTipText = "Show/Hide Matching Preferences";
			this.toolStripButtonShowMatch.Click += new System.EventHandler(this.toolStripButton8_Click);
			// 
			// toolStripButtonShowVolume
			// 
			this.toolStripButtonShowVolume.Checked = true;
			this.toolStripButtonShowVolume.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripButtonShowVolume.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonShowVolume.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Line_Chart_32x32;
			this.toolStripButtonShowVolume.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonShowVolume.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonShowVolume.MergeIndex = 15;
			this.toolStripButtonShowVolume.Name = "toolStripButtonShowVolume";
			this.toolStripButtonShowVolume.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonShowVolume.Text = "Volume";
			this.toolStripButtonShowVolume.ToolTipText = "Show/Hide Volume Columns";
			this.toolStripButtonShowVolume.Click += new System.EventHandler(this.toolStripButtonShowVolume_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripSeparator6.MergeIndex = 16;
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(6, 39);
			// 
			// BlotterViewer
			// 
			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.tabControl);
			this.Name = "BlotterViewer";
			this.Size = new System.Drawing.Size(612, 256);
			this.tabControl.ResumeLayout(false);
			this.tabPageWorking.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.tabPageMatch.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel2.ResumeLayout(false);
			this.splitContainer4.ResumeLayout(false);
			this.splitContainer5.Panel1.ResumeLayout(false);
			this.splitContainer5.Panel2.ResumeLayout(false);
			this.splitContainer5.ResumeLayout(false);
			this.splitContainer6.Panel1.ResumeLayout(false);
			this.splitContainer6.Panel2.ResumeLayout(false);
			this.splitContainer6.ResumeLayout(false);
			this.tabPageExecution.ResumeLayout(false);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageWorking;
		private System.Windows.Forms.TabPage tabPageMatch;
		private System.Windows.Forms.TabPage tabPageExecution;
		private MarkThree.Forms.ConsoleViewer consoleViewer;
		private MarkThree.Guardian.Forms.BlotterDetailViewer blotterDetailViewer;
		private MarkThree.Guardian.Forms.WorkingOrderViewer workingOrderViewer;
		private MarkThree.Guardian.Forms.VolumeChartViewer volumeChartViewer;
		private MarkThree.Guardian.Forms.MatchViewer matchViewer;
		private MarkThree.Guardian.Forms.NegotiationConsole negotiationConsole;
		private MarkThree.Guardian.ExecutionViewer executionViewer;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem customizeWindowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem workingOrderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem executionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem detailWindowToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem executionWindowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem detailWindowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blotterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mergeBlockToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unmergeBlockToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem manualExecutionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem orderFormToolStripMenuItem;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton toolStripButtonOrderForm;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton toolStripButtonPrint;
		private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton toolStripButtonCut;
		private System.Windows.Forms.ToolStripButton toolStripButtonCopy;
		private System.Windows.Forms.ToolStripButton toolStripButtonPaste;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripButton toolStripButtonShowMatch;
		private System.Windows.Forms.ToolStripButton toolStripButtonFilter1;
		private System.Windows.Forms.ToolStripButton toolStripButtonFilter2;
		private System.Windows.Forms.ToolStripButton toolStripButtonFilter3;
		private System.Windows.Forms.ToolStripMenuItem consoleWindowToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer5;
		private OrderBookViewer orderBookViewer;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private MatchHistoryViewer matchHistoryViewer;
		private System.Windows.Forms.ToolStripButton toolStripButtonExitBox;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripButton toolStripButtonShowVolume;
        private System.Windows.Forms.SplitContainer splitContainer6;
        private QuoteViewer quoteViewer;
        private System.Windows.Forms.ToolStripMenuItem quoteWindowToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton toolStripButtonEnterBox;
	}
}
