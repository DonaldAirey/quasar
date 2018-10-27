namespace MarkThree.Guardian.Forms
{

	using MarkThree;
	using MarkThree.Client;
	using MarkThree.Forms;
	using MarkThree.Guardian;
	using System;
	using System.Drawing;
	using System.Collections.Generic;
	using System.Collections;
	using System.Configuration;
	using System.ComponentModel;
	using System.Reflection;
	using System.Resources;
	using System.Threading;
	using System.Xml;
	using System.Windows.Forms;

	/// <summary>
	/// Main Window for the Guardian Client Container.
	/// </summary>
	/// <remarks>
	/// Guardian is constructed as a 'Thin Frame Container'.  Most of the application's logic is kept in dynamically
	/// loaded modules.  This Form provides a common frame for the viewers, navigators and services to use.
	/// </remarks>
	public partial class FormMain : Form
	{

		// Private Members
		private MarkThree.Forms.Viewer activeViewer;
		private System.Collections.Generic.Dictionary<Type, Viewer> viewerTable;
		private System.Collections.Generic.Dictionary<Type, Viewer> viewerTypeTable;

		// Delegates
		private delegate void OpenObjectHandler(object viewerObject);

		/// <summary>
		/// Initializes a new instance of the <code>MarkThree.Guardian.Container.FormMain</code> class.
		/// </summary>
		public FormMain()
		{

			// Initialize the components that are supported by the designer.
			InitializeComponent();

			// Initialize the dynamically loaded viewers.  These are viewers that are not part of this static frame window, but 
			// can be added in through a common viewer interface.
			InitializeViewers();

			// This will initialize any of the add-in tools that go with the application.
			InitializeTools();

			// Read the persistent settings.
			LoadSettings();

			// This text is available for any banner or frame window that needs to communicate the application name.
			Application.SafeTopLevelCaptionFormat = MarkThree.Guardian.Forms.Properties.Resources.ApplicationName;

            // start up the TOMS scraper ... if so configured
            //MarkThree.TomsScraper.Start();

			// The application needs some document to display when it starts.  A method to open a document can't be called from the
			// constructor because there is no handle for the 'Invoke' methods used to coordinate the opening of documents.  This
			// event handler will take care of opening the first document when there is a system of window handles available.
			this.HandleCreated += new EventHandler(HandleCreatedEventHandler);

		}

		/// <summary>
		/// Completes the initialization of the Form.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The event arguments.</param>
		private void HandleCreatedEventHandler(object sender, System.EventArgs e)
		{

			// Start the application off by opening up the start page.
			string onOpenText = ConfigurationManager.AppSettings["OnOpen"];
			if (onOpenText != null)
			{

				// This will open up the object specified in the configuration file.  The first step to opening up the object is to
				// create an object of the specified type.
				string[] onOpenArguments = onOpenText.Split(new char[] { ',' });
				string typeName = onOpenArguments[0].Trim();
				string assemblyName = onOpenArguments[1].Trim();
				Assembly assembly = Assembly.Load(assemblyName);
				Type type = Assembly.Load(assemblyName).GetType(typeName);

				// This will extract the arguments for the object.
				object[] parameters = new object[onOpenArguments.Length - 2];
				Array.Copy(onOpenArguments, 2, parameters, 0, parameters.Length);

				// This will create an object of the specified type, giving it the arguments also specified in the application
				// configuration file.
				object viewerObject = assembly.CreateInstance(typeName, false, BindingFlags.CreateInstance,
					null, parameters, System.Globalization.CultureInfo.CurrentCulture, null);

				// Now that an object has been created and initialize, it can be opened.
				OpenObject(this, viewerObject);

			}

		}

		/// <summary>Initializes the Dynamically Loaded Viewers</summary>
		/// <remarks>
		/// The application is constructed as a container for navigators, document viewers and special purpose dialog boxes. Said
		/// differently, we want the application to be extensible and have incremental upgrades, so there's no hard coded way of
		/// saying "If I click on this icon, I want this user control displayed."  The binding between the database objects and the
		/// Viewers (special purpose user interface controls) is done dynamically through a configuration file.  The configuration
		/// file has a list of DLLs and the objects in those DLLs that can be used for viewing.  Each of the viewers or special
		/// purpose dialog boxes has an integer associated with it.  This integer is the 'type code' which corresponds to the type
		/// codes found in the 'types' table on the database.  When we get a request to open up one of these objects, we use the
		/// type code to map the 'Open' request to one of the dynamically loaded modules.  This hash table is used to associate the
		/// type code with the proper viewer.
		/// </remarks>
		public void InitializeViewers()
		{

			// The viewing area of the frame can have several viewers which all sit on top of each other in the same space. The
			// active viewer is the one that is visible and sits on top of all the rest.
			this.activeViewer = null;

			// This table contains a mapping between all the object types and the viewers used to present the data in those 
			// types.  When an object is opened, this table is searched for that object's type and the viewer that is part of the
			// key pair is loaded into the viewer area and the object is opened up into that viewer.
			this.viewerTable = new Dictionary<Type, Viewer>();

			// Though a viewer can be specified to display several different types, only one instance of that viewer will be 
			// created. This table helps organize the viewers during initialization so there is only one instance of a viewer for
			// the entire application.
			this.viewerTypeTable = new Dictionary<Type, Viewer>();

			// Load any dynamic viewers into the application.
			ViewerSection viewerSection = (ViewerSection)ConfigurationManager.GetSection("viewers");
			if (viewerSection != null)
			{

				// This loop will load each add-in module and associate it with an object type code.
				foreach (ViewerInfo viewerInfo in viewerSection)
				{

					// The most likely error to catch is the dynamically loaded control doesn't exist.  In any case, a failure to 
					// load any of the add-ins is not fatal.
					try
					{

						// This holds the next viewer to be initialized from the application settings.
						Viewer viewer = null;

						// If the specified viewer has already been created for another type, it will be re-used.  The idea here is
						// that the viewer will have the intelligence to work out how to display any given data type.
						if (!viewerTypeTable.TryGetValue(viewerInfo.ViewerType, out viewer))
						{
							viewer = (Viewer)viewerInfo.ViewerType.Assembly.CreateInstance(viewerInfo.ViewerType.ToString());
							this.viewerTypeTable.Add(viewerInfo.ViewerType, viewer);
						}

						// This is where the mapping between the object type and the viewer used to display that object is kept.
						// When a navigator selects an object, it will send it to a Viewer.  The Viewer will pull apart the data in
						// the object and determine how to show it to the user.  But the first step is to pass the object on to the
						// appropriate Viewer.
						this.viewerTable.Add(viewerInfo.Type, viewer);

						// The Viewer will fill the panel in which it is placed.
						viewer.Dock = DockStyle.Fill;

						// This will give the viewers a method to ask the container to open a new object.
						viewer.ObjectOpen += new OpenObjectEventHandler(this.OpenObject);

						// This is the panel where all the viewers will be displayed.
						this.splitContainer2.Panel2.Controls.Add(viewer);

					}
					catch (Exception exception)
					{

						// Catch the most general errors and emit them to the debug devide.
						string errorMessage = String.Format("Viewer {0} couldn't be loaded: {1}", viewerInfo.ViewerType, exception.Message);
						MessageBox.Show(errorMessage, "Guardian Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

					}

				}

			}

		}

		/// <summary>
		/// Initializes the add-in tools.
		/// </summary>
		public void InitializeTools()
		{

			// This list is used to collect all the Add-In controls that are defined in the configuration file.  Once the Add-Ins
			// have been instantiated, they will be added to the application frame as a range of controls.
			ArrayList toolStripItemList = new ArrayList();

			// Load any dynamic tools into the application.
			ToolSection toolSection = (ToolSection)ConfigurationManager.GetSection("tools");
			if (toolSection != null)
			{

				// This loop will load each add-in module and associate it with an object type code.
				foreach (ToolInfo toolInfo in toolSection)
				{

					ToolStripItem toolStripItem = new ToolStripMenuItem();
					toolStripItem.Text = toolInfo.Text;
					toolStripItem.Size = new System.Drawing.Size(170, 22);
					toolStripItem.Click += new EventHandler(ToolHandler);
					toolStripItemList.Add(toolStripItem);

				}

			}

			// Copy the menu items into the 'Tools' menu.
			ToolStripItem[] toolStripItems = new ToolStripItem[toolStripItemList.Count];
			toolStripItemList.CopyTo(toolStripItems, 0);
			this.toolsToolStripMenuItem.DropDownItems.AddRange(toolStripItems);

		}

		/// <summary>
		/// Handles a tool button action.
		/// </summary>
		/// <param name="sender">The object that initiated this event.</param>
		/// <param name="eventArgs">The event arguments.</param>
		public void ToolHandler(object sender, EventArgs eventArgs)
		{

			// Find the selected tool based on the text of the tool bar item and invoke the 'Show' method.  For example, a tool
			// that dynamically handled the options would display the Options Dialog at this point.
			ToolSection toolSection = (ToolSection)ConfigurationManager.GetSection("tools");
			foreach (ToolInfo toolInfo in toolSection)
				if (toolInfo.Text == ((ToolStripMenuItem)sender).Text)
				{
					object tool = toolInfo.ToolType.Assembly.CreateInstance(toolInfo.ToolType.ToString());
					MethodInfo methodInfo = toolInfo.ToolType.GetMethod("Show", new Type[] { typeof(System.Windows.Forms.IWin32Window) });
					methodInfo.Invoke(tool, new object[] { this });
				}

		}

		/// <summary>Reads the applications settings from the config file.</summary>
		/// <remarks>
		/// The state of the application is stored in the config file, either by an administrator or at the end of the
		/// program's execution.  This method reads the state of the various panes and menu items and places the
		/// applications GUI in the same state it was last run.
		/// </remarks>
		private void LoadSettings()
		{

			// This method will modify windows and menus as it reads in elements from the config file.  Suspending the layout will
			// minimize the unnecessary screen updates.
			this.SuspendLayout();

			// Because of an oversight on the part of the engineers at Microsoft, these Properties were not made available to the
			// IDE to persist in the Settings file.
			this.Size = Properties.Settings.Default.FormMainSize;

			// Load the configuration of each of the viewers.
			foreach (KeyValuePair<Type, Viewer> keyValuePair in this.viewerTypeTable)
				keyValuePair.Value.LoadSettings();

			// At this point, all the controls have been positioned and resized.
			this.ResumeLayout();

		}

		/// <summary>
		/// Save the user's application settings to a persistent store.
		/// </summary>
		private void SaveSettings()
		{

			// FormMain
			Properties.Settings.Default.FormMainWindowState = this.WindowState;
			Properties.Settings.Default.FormMainLocation = this.WindowState == FormWindowState.Normal ? this.Location :
				this.RestoreBounds.Location;
			Properties.Settings.Default.FormMainSize = this.WindowState == FormWindowState.Normal ? this.Size :
				this.RestoreBounds.Size;

			// Menu Items
			Properties.Settings.Default.GuardianBarToolStripMenuItemChecked = this.guardianBarToolStripMenuItem.Checked;
			Properties.Settings.Default.GuardianBarToolStripMenuItemCheckState = this.guardianBarToolStripMenuItem.CheckState;
			Properties.Settings.Default.FolderListToolStripMenuItemChecked = this.folderListToolStripMenuItem.Checked;
			Properties.Settings.Default.FolderListToolStripMenuItemCheckState = this.folderListToolStripMenuItem.CheckState;
			Properties.Settings.Default.ToolBarsToolStripMenuItemChecked = this.toolbarsToolStripMenuItem.Checked;
			Properties.Settings.Default.ToolBarsToolStripMenuItemCheckState = this.toolbarsToolStripMenuItem.CheckState;
			Properties.Settings.Default.StatusBarToolStripMenuItemChecked = this.statusBarToolStripMenuItem.Checked;
			Properties.Settings.Default.StatusBarToolStripMenuItemCheckState = this.statusBarToolStripMenuItem.CheckState;

			// ToolStrip
			Properties.Settings.Default.ToolStripDock = this.toolStrip.Dock;
			Properties.Settings.Default.ToolStripVisible = this.toolStrip.Visible;

			// SplitterContainer1
			Properties.Settings.Default.SplitContainer1Panel1Collapsed = this.splitContainer1.Panel1Collapsed;
			Properties.Settings.Default.SplitContainer1SplitterDistance = this.splitContainer1.SplitterDistance;

			// SplitterContainer2
			Properties.Settings.Default.SplitContainer2Panel1Collapsed = this.splitContainer2.Panel1Collapsed;
			Properties.Settings.Default.SplitContainer2SplitterDistance = this.splitContainer2.SplitterDistance;

			// Status Bar
			Properties.Settings.Default.StatusStripVisible = this.statusStrip.Visible;
			Properties.Settings.Default.StatusStripDock = this.statusStrip.Dock;
			Properties.Settings.Default.StatusStripSize = this.statusStrip.Size;

			// Save the location, configuration and size of each of the viewers.
			foreach (KeyValuePair<Type, Viewer> keyValuePair in this.viewerTypeTable)
				keyValuePair.Value.SaveSettings();

			// Save the settings.
			Properties.Settings.Default.Save();

		}

		/// <summary>
		/// Invoked when the user clicks on a toolbar button.
		/// </summary>
		/// <param name="sender">The object that originated the event</param>
		/// <param name="e">Event specific parameters.</param>
		private void toolBarStandard_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{

			try
			{

				if (e.Button.Tag != null && e.Button.Tag.GetType() == typeof(CommonButton))
				{

					switch ((CommonButton)e.Button.Tag)
					{

					case CommonButton.GuardianToday:

						OpenObject(this, new WebPage(new object[] { ConfigurationManager.AppSettings["StartPage"] }));
						break;

					}

				}

			}
			catch (Exception exception)
			{
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);
			}

		}

		/// <summary>
		/// Opens an Object into a Viewer.
		/// </summary>
		/// <param name="sender">The object that created the event.</param>
		/// <param name="objectArgs">The event parameters.</param>
		/// <remarks>
		/// This will start the process of opening up a document.  Since viewers and documents may take a 
		/// finite amount of time to open, the operation is handled asynchronously.  This will call 'OpenViewer'
		/// if a different viewer needs to be opened.  It will always call'OpenDocument' to open the object into
		/// the viewer associated with the document.
		/// </remarks>
		private void OpenObject(object sender, object viewerObject)
		{

			BeginInvoke(new OpenObjectHandler(OpenObjectInForeground), new object[] { viewerObject });

		}

		/// <summary>
		/// Opens an object using the main thread containing a message handler.
		/// </summary>
		/// <param name="viewerObject">The object to be opened.</param>
		private void OpenObjectInForeground(object viewerObject)
		{

			try
			{

				// Activate the application when opening an object.  This is used in situations where a notification comes in while
				// another application is active.  When the user clicks on the notification widow, they will be routed to this
				// application, which then needs to be active to take the user's input.
				this.Activate();

				// Close down any documents that are currently open in the viewer.
				if (this.activeViewer != null)
					this.activeViewer.Close();

				// If an object type was selected that doesn't have a viewer (or the viewer couldn't be loaded), then
				// reject the operation and leave the last viewer up.
				Viewer viewer = this.viewerTable[viewerObject.GetType()];
				if (viewer == null)
					throw new Exception("Viewer not found");

				// This will synchronize the navigators with the open document.
				this.folderList.Open(viewerObject);
				this.guardianBar.Open(viewerObject);
				this.Text = viewerObject.ToString() + " - " + Properties.Settings.Default.ApplicationName;

				// This is an optimization: if the viewer for the current object is the same as the viewer for the last
				// object, then we'll skip the step of swapping the screen elements around and just reuse the current
				// viewer.  Otherwise, there's some modest reorgainization of the screen required to activate the proper
				// viewer.
				if (this.activeViewer != viewer)
				{

					// The code below will modify the layout of the FormMain frame window.  It will swap out the current
					// viewer its menus and toolbars and replace it with the new viewer and it's resources.  Suspending
					// the layout will minimize the screen distractions.
					SuspendLayout();

					// Swap the new active viewer with the previous one.  Also, have the viewer cleared out so it's empty
					// the next time it's activated.  This gets rid of the disturbing effect of seeing the previous data
					// when you select a new report.  It shows up momentarily while the new document is constructed.
					// Clearing it out now will give it a chance to create a blank report in the background.
					viewer.BringToFront();

					this.menuStrip.SuspendLayout();
					this.toolStrip.SuspendLayout();

					// Clear the previous menu and tools from the child viewer out of the main container area.
					ToolStripManager.RevertMerge(this.menuStrip);
					ToolStripManager.RevertMerge(this.toolStrip);

					// Merge the container's menu with the menu of the active viewer.
					ToolStripManager.Merge(viewer.MenuStrip, this.menuStrip);
					ToolStripManager.Merge(viewer.ToolBarStandard, this.toolStrip);

					this.menuStrip.ResumeLayout();
					this.toolStrip.ResumeLayout();

					// Let the screen process the changes.
					this.ResumeLayout();

					this.activeViewer = viewer;

				}

				// Opening is a somewhat involved operation.  Because it may take a few seconds to open a report, the
				// message loop can't be suspended while the user waits.  The operation is done asynchronously.  This
				// operation will kick off an asynchronous operation in the viewer that will gather all the resources
				// needed and draw the document.  When that operation is complete, the viewer will kick off an event that
				// will signal the 'Open' operation is complete.  At that point, a message will be broadcast from the 
				// control and picked up by the 'viewer_EndOpenDocument' method below which will complete the operation of
				// opening the viewer and document.
				this.activeViewer.Open(viewerObject);

			}
			catch (Exception exception)
			{

				// These user interface errors need to be show to the users.
				MessageBox.Show(exception.Message, "Guardian Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

			}

		}

		/// <summary>Handles the closing of the container's Frame Window.</summary>
		/// <remarks>
		/// This event handler will save all the state of the application to the configuration file.  This data will
		/// be read the next time the application is started and the panes, menu and status bar states will be
		/// restored to their previous state.
		/// </remarks>
		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{

			// Close down any documents that are currently open in the viewer.
			if (this.activeViewer != null)
				this.activeViewer.Close();

            // sut down the TOMS scraper
            //MarkThree.Guardian.Client.TomsScraper.Stop();


			SaveSettings();

		}

		private void resetConnectionToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// Force the user to enter the connection information the next time a Web Request is made.
			WebTransactionProtocol.IsUrlPrompted = true;
			WebTransactionProtocol.IsCredentialPrompted = true;
			WebTransactionProtocol.OnCredentialsChanged();

		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// Exit the application.
			Application.Exit();

		}

		/// <summary>
		/// Invoked when the menu item to toggle the visibility of the Guardian Bar is clicked.
		/// </summary>
		/// <param name="sender">The object that originated the event</param>
		/// <param name="e">Event specific parameters.</param>
		private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// Toggle the visibility of the Status Bar.  Update the MenuItem to reflect the change.
			this.statusBarToolStripMenuItem.Checked = !this.statusBarToolStripMenuItem.Checked;
			this.statusStrip.Visible = this.statusBarToolStripMenuItem.Checked;

		}

		/// <summary>
		/// Invoked when the menu item to toggle the visibility of the Guardian Bar is clicked.
		/// </summary>
		/// <param name="sender">The object that originated the event</param>
		/// <param name="e">Event specific parameters.</param>
		private void guardianBarToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// Toggle the visibility of the Guardian Bar and its splitter.  Update the Menu Item to reflect the change.
			this.guardianBarToolStripMenuItem.Checked = !this.guardianBarToolStripMenuItem.Checked;
			this.splitContainer1.Panel1Collapsed = !this.guardianBarToolStripMenuItem.Checked;

		}

		/// <summary>
		/// Invoked when the menu item to toggle the visibility of the Folder List is clicked.
		/// </summary>
		/// <param name="sender">The object that originated the event</param>
		/// <param name="e">Event specific parameters.</param>
		private void folderListToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// Toggle the visibility of the Folder List and its splitter.  Update the MenuItem to reflect the change.
			this.folderListToolStripMenuItem.Checked = !this.folderListToolStripMenuItem.Checked;
			this.splitContainer2.Panel1Collapsed = !this.folderListToolStripMenuItem.Checked;

		}

		private void toolbarsToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// Toggle the visibility of the Status Bar.  Update the MenuItem to reflect the change.
			this.toolbarsToolStripMenuItem.Checked = !this.toolbarsToolStripMenuItem.Checked;
			this.toolStrip.Visible = this.toolbarsToolStripMenuItem.Checked;

		}

		private void toolStripButtonUserPreferences_Click(object sender, EventArgs e)
		{

			// Find the selected tool based on the text of the tool bar item and invoke the 'Show' method.  For example, a tool
			// that dynamically handled the options would display the Options Dialog at this point.
			ToolSection toolSection = (ToolSection)ConfigurationManager.GetSection("tools");
			foreach (ToolInfo toolInfo in toolSection)
				if (toolInfo.Text == "&Options...")
				{
					object tool = toolInfo.ToolType.Assembly.CreateInstance(toolInfo.ToolType.ToString());
					MethodInfo methodInfo = toolInfo.ToolType.GetMethod("Show", new Type[] { typeof(System.Windows.Forms.IWin32Window) });
					methodInfo.Invoke(tool, new object[] { this });
				}

		}

		private void toolStripButtonShowMenu_Click(object sender, EventArgs e)
		{

			// Toggle the state of the menu strip.
			this.menuStrip.Visible = !this.menuStrip.Visible;

		}

	}

}
