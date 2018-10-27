namespace MarkThree.Guardian.Forms
{

	partial class FormMain
	{
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.toolStripContainer = new System.Windows.Forms.ToolStripContainer();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.guardianBar = new MarkThree.Guardian.Forms.GuardianBar();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.folderList = new MarkThree.Guardian.Forms.FolderList();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.resetConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.guardianBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.folderListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolbarsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonUserPreferences = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonShowMenu = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButtonHelp = new System.Windows.Forms.ToolStripButton();
			this.userPreferences = new MarkThree.UserPreferences(this.components);
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.negotiationService = new MarkThree.Guardian.Forms.NegotiationService(this.components);
			this.toolStripContainer.BottomToolStripPanel.SuspendLayout();
			this.toolStripContainer.ContentPanel.SuspendLayout();
			this.toolStripContainer.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripContainer
			// 
			// 
			// toolStripContainer.BottomToolStripPanel
			// 
			this.toolStripContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
			// 
			// toolStripContainer.ContentPanel
			// 
			this.toolStripContainer.ContentPanel.Controls.Add(this.splitContainer1);
			this.toolStripContainer.ContentPanel.Size = new System.Drawing.Size(504, 144);
			this.toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer.Name = "toolStripContainer";
			this.toolStripContainer.Size = new System.Drawing.Size(504, 229);
			this.toolStripContainer.TabIndex = 0;
			this.toolStripContainer.Text = "toolStripContainer1";
			// 
			// toolStripContainer.TopToolStripPanel
			// 
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.menuStrip);
			this.toolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip);
			// 
			// statusStrip
			// 
			this.statusStrip.Dock = global::MarkThree.Guardian.Forms.Properties.Settings.Default.StatusStripDock;
			this.statusStrip.Location = new System.Drawing.Point(0, 0);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = global::MarkThree.Guardian.Forms.Properties.Settings.Default.StatusStripSize;
			this.statusStrip.TabIndex = 0;
			this.statusStrip.Visible = global::MarkThree.Guardian.Forms.Properties.Settings.Default.StatusStripVisible;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.guardianBar);
			this.splitContainer1.Panel1Collapsed = global::MarkThree.Guardian.Forms.Properties.Settings.Default.SplitContainer1Panel1Collapsed;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
			this.splitContainer1.Size = new System.Drawing.Size(504, 144);
			this.splitContainer1.SplitterDistance = global::MarkThree.Guardian.Forms.Properties.Settings.Default.SplitContainer1SplitterDistance;
			this.splitContainer1.TabIndex = 0;
			// 
			// guardianBar
			// 
			this.guardianBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.guardianBar.Location = new System.Drawing.Point(0, 0);
			this.guardianBar.Name = "guardianBar";
			this.guardianBar.Size = new System.Drawing.Size(128, 144);
			this.guardianBar.TabIndex = 0;
			this.guardianBar.OpenObject += new MarkThree.Forms.OpenObjectEventHandler(this.OpenObject);
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
			this.splitContainer2.Panel1.Controls.Add(this.folderList);
			this.splitContainer2.Panel1Collapsed = global::MarkThree.Guardian.Forms.Properties.Settings.Default.SplitContainer2Panel1Collapsed;
			this.splitContainer2.Size = new System.Drawing.Size(372, 144);
			this.splitContainer2.SplitterDistance = global::MarkThree.Guardian.Forms.Properties.Settings.Default.SplitContainer2SplitterDistance;
			this.splitContainer2.TabIndex = 0;
			// 
			// folderList
			// 
			this.folderList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.folderList.Location = new System.Drawing.Point(0, 0);
			this.folderList.Name = "folderList";
			this.folderList.Size = new System.Drawing.Size(128, 144);
			this.folderList.TabIndex = 0;
			this.folderList.OpenObject += new MarkThree.Forms.OpenObjectEventHandler(this.OpenObject);
			// 
			// menuStrip
			// 
			this.menuStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.menuStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(504, 24);
			this.menuStrip.TabIndex = 0;
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.resetConnectionToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.openToolStripMenuItem.Text = "&Open";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(167, 6);
			// 
			// resetConnectionToolStripMenuItem
			// 
			this.resetConnectionToolStripMenuItem.Name = "resetConnectionToolStripMenuItem";
			this.resetConnectionToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.resetConnectionToolStripMenuItem.Text = "&Reset Connection";
			this.resetConnectionToolStripMenuItem.Click += new System.EventHandler(this.resetConnectionToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(167, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.exitToolStripMenuItem.Text = "&Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.toolStripSeparator3,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.undoToolStripMenuItem.Text = "&Undo";
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(109, 6);
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.cutToolStripMenuItem.Text = "&Cut";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.guardianBarToolStripMenuItem,
            this.folderListToolStripMenuItem,
            this.toolStripSeparator4,
            this.toolbarsToolStripMenuItem,
            this.statusBarToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// guardianBarToolStripMenuItem
			// 
			this.guardianBarToolStripMenuItem.Checked = global::MarkThree.Guardian.Forms.Properties.Settings.Default.GuardianBarToolStripMenuItemChecked;
			this.guardianBarToolStripMenuItem.CheckState = global::MarkThree.Guardian.Forms.Properties.Settings.Default.GuardianBarToolStripMenuItemCheckState;
			this.guardianBarToolStripMenuItem.Name = "guardianBarToolStripMenuItem";
			this.guardianBarToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.guardianBarToolStripMenuItem.Text = "&Guardian Bar";
			this.guardianBarToolStripMenuItem.Click += new System.EventHandler(this.guardianBarToolStripMenuItem_Click);
			// 
			// folderListToolStripMenuItem
			// 
			this.folderListToolStripMenuItem.Checked = global::MarkThree.Guardian.Forms.Properties.Settings.Default.FolderListToolStripMenuItemChecked;
			this.folderListToolStripMenuItem.CheckState = global::MarkThree.Guardian.Forms.Properties.Settings.Default.FolderListToolStripMenuItemCheckState;
			this.folderListToolStripMenuItem.Name = "folderListToolStripMenuItem";
			this.folderListToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.folderListToolStripMenuItem.Text = "&Folder List";
			this.folderListToolStripMenuItem.Click += new System.EventHandler(this.folderListToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(144, 6);
			// 
			// toolbarsToolStripMenuItem
			// 
			this.toolbarsToolStripMenuItem.Checked = global::MarkThree.Guardian.Forms.Properties.Settings.Default.ToolBarsToolStripMenuItemChecked;
			this.toolbarsToolStripMenuItem.CheckState = global::MarkThree.Guardian.Forms.Properties.Settings.Default.ToolBarsToolStripMenuItemCheckState;
			this.toolbarsToolStripMenuItem.Name = "toolbarsToolStripMenuItem";
			this.toolbarsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.toolbarsToolStripMenuItem.Text = "&Toolbars";
			this.toolbarsToolStripMenuItem.Click += new System.EventHandler(this.toolbarsToolStripMenuItem_Click);
			// 
			// statusBarToolStripMenuItem
			// 
			this.statusBarToolStripMenuItem.Checked = global::MarkThree.Guardian.Forms.Properties.Settings.Default.StatusBarToolStripMenuItemChecked;
			this.statusBarToolStripMenuItem.CheckState = global::MarkThree.Guardian.Forms.Properties.Settings.Default.StatusBarToolStripMenuItemCheckState;
			this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
			this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.statusBarToolStripMenuItem.Text = "&Status Bar";
			this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.statusBarToolStripMenuItem_Click);
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.toolsToolStripMenuItem.Text = "&Tools";
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// toolStrip
			// 
			this.toolStrip.Dock = global::MarkThree.Guardian.Forms.Properties.Settings.Default.ToolStripDock;
			this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonUserPreferences,
            this.toolStripButtonShowMenu,
            this.toolStripSeparator5,
            this.toolStripButtonHelp});
			this.toolStrip.Location = new System.Drawing.Point(3, 24);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(157, 39);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Visible = global::MarkThree.Guardian.Forms.Properties.Settings.Default.ToolStripVisible;
			// 
			// toolStripButtonUserPreferences
			// 
			this.toolStripButtonUserPreferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonUserPreferences.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Preferences_32x32;
			this.toolStripButtonUserPreferences.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonUserPreferences.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonUserPreferences.MergeIndex = 100;
			this.toolStripButtonUserPreferences.Name = "toolStripButtonUserPreferences";
			this.toolStripButtonUserPreferences.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonUserPreferences.Text = "User Preferences";
			this.toolStripButtonUserPreferences.Click += new System.EventHandler(this.toolStripButtonUserPreferences_Click);
			// 
			// toolStripButtonShowMenu
			// 
			this.toolStripButtonShowMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonShowMenu.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Menu_Green_32x32;
			this.toolStripButtonShowMenu.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonShowMenu.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonShowMenu.MergeIndex = 101;
			this.toolStripButtonShowMenu.Name = "toolStripButtonShowMenu";
			this.toolStripButtonShowMenu.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonShowMenu.Text = "Show/Hide Menus";
			this.toolStripButtonShowMenu.Click += new System.EventHandler(this.toolStripButtonShowMenu_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripSeparator5.MergeIndex = 102;
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(6, 39);
			// 
			// toolStripButtonHelp
			// 
			this.toolStripButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonHelp.Enabled = false;
			this.toolStripButtonHelp.Image = global::MarkThree.Guardian.Forms.Properties.Resources.Help_32x32;
			this.toolStripButtonHelp.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonHelp.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripButtonHelp.MergeIndex = 103;
			this.toolStripButtonHelp.Name = "toolStripButtonHelp";
			this.toolStripButtonHelp.Size = new System.Drawing.Size(36, 36);
			this.toolStripButtonHelp.Text = "toolStripButton1";
			this.toolStripButtonHelp.ToolTipText = "Help";
			// 
			// notifyIcon
			// 
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Visible = true;
			// 
			// negotiationService
			// 
			this.negotiationService.OpenObject += new MarkThree.Forms.OpenObjectEventHandler(this.OpenObject);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(504, 229);
			this.Controls.Add(this.toolStripContainer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = global::MarkThree.Guardian.Forms.Properties.Settings.Default.FormMainLocation;
			this.MainMenuStrip = this.menuStrip;
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Block Cross";
			this.WindowState = global::MarkThree.Guardian.Forms.Properties.Settings.Default.FormMainWindowState;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.BottomToolStripPanel.PerformLayout();
			this.toolStripContainer.ContentPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer.TopToolStripPanel.PerformLayout();
			this.toolStripContainer.ResumeLayout(false);
			this.toolStripContainer.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolStripContainer toolStripContainer;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem resetConnectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem guardianBarToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem folderListToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem toolbarsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem statusBarToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private UserPreferences userPreferences;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private MarkThree.Guardian.Forms.NegotiationService negotiationService;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private GuardianBar guardianBar;
		private FolderList folderList;
		private System.Windows.Forms.ToolStripButton toolStripButtonUserPreferences;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripButton toolStripButtonHelp;
		private System.Windows.Forms.ToolStripButton toolStripButtonShowMenu;
	}
}

