namespace MarkThree.Guardian.Forms
{

	using MarkThree;
	using MarkThree.Forms;
	using MarkThree.Guardian;
	using MarkThree.Guardian.Client;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Configuration;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Threading;
	using System.Text.RegularExpressions;
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for GuardianBar.
	/// </summary>
	public class GuardianBar : System.Windows.Forms.UserControl
	{

		private bool isOpenEventAllowed;
		private System.Collections.ArrayList objectList;
		private System.Collections.ArrayList objectIdList;
		private System.Collections.ArrayList newObjectIdList;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Button Shortcuts;
		private System.Windows.Forms.ListView listView;
		private ImageList imageListLarge;
		private ImageList imageListSmall;
		private ManualResetEvent handleCreatedEvent;
		private ContextMenuStrip contextMenuStrip;
		private ToolStripMenuItem largeIconsToolStripMenuItem;
		private ToolStripMenuItem smallIconsToolStripMenuItem;
		private System.ComponentModel.IContainer components;
		private ToolStripMenuItem clearToolStripMenuItem;
		private UserPreferences userPreferences;
		private ObjectAvailableCallback objectAvailableCallback;

		// Delegates
		private delegate void ObjectAvailableCallback(ArrayList arrayList);

		/// <summary>
		/// Will notify clients when the user has selected an object.
		/// </summary>
		public event OpenObjectEventHandler OpenObject;

		/// <summary>
		/// Initializes a new instance of the <code>MarkThree.Guardian.Forms.GuardianBar.GuardianBar</code> class.
		/// </summary>
		public GuardianBar()
		{

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Initialize the object
			this.isOpenEventAllowed = true;

#if DEBUG
			// This will insure that the background thread to access the server isn't spawned when in the design mode.
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
#endif
				// Arrange the icons along the left side.
				this.listView.Alignment = ListViewAlignment.Left;

				// This will read in the list of identifiers from the user preferences file that define the objects that is managed by 
				// this control.
				InitializeObjectIdList();

				// The data model is not available when the application starts up.  It must be downloaded.  This presents a problem for the
				// initialization of this control.  We know what objects are stored in this control from the persistent list of identifiers.
				// However, those identifiers are useless until there are some actual objects in the data model from which to build the Guardian Objects.
				this.newObjectIdList = new ArrayList();

				// This is the actual list of objects, not to be confused with the object identifiers that persist when the 
				// application isn't running.
				this.objectList = new ArrayList();

				// This will hold up the background handling of the objects until a window has been created to handle the updates.
				this.listView.HandleCreated += new EventHandler(listView_HandleCreated);
				this.handleCreatedEvent = new ManualResetEvent(false);

				// When the event handlers have some data to display, it will use this event handler to notify the foreground.
				this.objectAvailableCallback = new ObjectAvailableCallback(OnObjectAvailable);

#if DEBUG
			}
#endif

		}

		/// <summary>Releases all the resources used by MarkThree.Guardian.Container.FormMain</summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only
		/// unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{

			// This will write a list of the objects currently in this control to the local persistent data store.
			UserPreferences.LocalSettings["guardianBar.ItemCount"] = this.listView.Items.Count;
			for (int index = 0; index < this.listView.Items.Count; index++)
				UserPreferences.LocalSettings[string.Format("guardianBar.Item{0}", index)] =
					((GuardianViewItem)this.listView.Items[index]).Object.ObjectId;

			// Clean up the resources used by installed components.
			if (disposing)
			{
				this.handleCreatedEvent.Close();
				if (components != null)
					components.Dispose();
			}

			// Call the base class to finish up the cleanup.
			base.Dispose(disposing);

		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.listView = new System.Windows.Forms.ListView();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageListLarge = new System.Windows.Forms.ImageList(this.components);
			this.imageListSmall = new System.Windows.Forms.ImageList(this.components);
			this.Shortcuts = new System.Windows.Forms.Button();
			this.panel = new System.Windows.Forms.Panel();
			this.userPreferences = new MarkThree.UserPreferences(this.components);
			this.contextMenuStrip.SuspendLayout();
			this.panel.SuspendLayout();
			this.SuspendLayout();
			// 
			// listView
			// 
			this.listView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
			this.listView.AllowDrop = true;
			this.listView.AutoArrange = false;
			this.listView.BackColor = System.Drawing.SystemColors.Control;
			this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listView.ContextMenuStrip = this.contextMenuStrip;
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.LargeImageList = this.imageListLarge;
			this.listView.Location = new System.Drawing.Point(0, 22);
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(124, 230);
			this.listView.SmallImageList = this.imageListSmall;
			this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listView.TabIndex = 1;
			this.listView.TabStop = false;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView_DragEnter);
			this.listView.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView_DragDrop);
			this.listView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView_ItemSelectionChanged);
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.largeIconsToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.clearToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip1";
			this.contextMenuStrip.Size = new System.Drawing.Size(142, 70);
			// 
			// largeIconsToolStripMenuItem
			// 
			this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
			this.largeIconsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.largeIconsToolStripMenuItem.Text = "Large Icons";
			this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.largeIconsToolStripMenuItem_Click);
			// 
			// smallIconsToolStripMenuItem
			// 
			this.smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
			this.smallIconsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.smallIconsToolStripMenuItem.Text = "Small Icons";
			this.smallIconsToolStripMenuItem.Click += new System.EventHandler(this.smallIconsToolStripMenuItem_Click);
			// 
			// clearToolStripMenuItem
			// 
			this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			this.clearToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.clearToolStripMenuItem.Text = "&Clear";
			this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
			// 
			// imageListLarge
			// 
			this.imageListLarge.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageListLarge.ImageSize = new System.Drawing.Size(32, 32);
			this.imageListLarge.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// imageListSmall
			// 
			this.imageListSmall.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageListSmall.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListSmall.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// Shortcuts
			// 
			this.Shortcuts.Dock = System.Windows.Forms.DockStyle.Top;
			this.Shortcuts.Location = new System.Drawing.Point(0, 0);
			this.Shortcuts.Name = "Shortcuts";
			this.Shortcuts.Size = new System.Drawing.Size(124, 22);
			this.Shortcuts.TabIndex = 0;
			this.Shortcuts.TabStop = false;
			this.Shortcuts.Text = "Shortcuts";
			// 
			// panel
			// 
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel.Controls.Add(this.listView);
			this.panel.Controls.Add(this.Shortcuts);
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(128, 256);
			this.panel.TabIndex = 0;
			// 
			// GuardianBar
			// 
			this.Controls.Add(this.panel);
			this.Name = "GuardianBar";
			this.Size = new System.Drawing.Size(128, 256);
			this.contextMenuStrip.ResumeLayout(false);
			this.panel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// This will read a list of identifiers of the objects that should appear in this control.
		/// </summary>
		private void InitializeObjectIdList()
		{

			// This is where the persistent object identifiers are stored for this object.  This list is compared against incoming
			// elements from the data model events to determine if it time to display the objects in the list.
			this.objectIdList = new ArrayList();

			try
			{

				// Check to make sure that there are persistent elements stored in the user preferences.
				if (UserPreferences.LocalSettings["guardianBar.ItemCount"] != null)
				{

					// Read the object identifiers from the user preferences.
					int itemCount = Convert.ToInt32(UserPreferences.LocalSettings["guardianBar.ItemCount"]);
					for (int index = 0; index < itemCount; index++)
						this.objectIdList.Add(UserPreferences.LocalSettings[String.Format("guardianBar.Item{0}", index)]);

				}
			}
			catch (Exception exception)
			{

				// Log any errors trying to read the persistent store.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);
			
			}

		}

		/// <summary>
		/// Called when the Window Handle is created.
		/// </summary>
		/// <param name="sender">The object that created the event.</param>
		/// <param name="e">Parameters specific to the event.</param>
		private void listView_HandleCreated(object sender, EventArgs e)
		{

			// This signals that the foreground window is available for updates.
			this.handleCreatedEvent.Set();

#if DEBUG
			// This will prevent the background initialization thread from running in the designer (background threads kill the designer.
			if (!this.DesignMode)
			{
#endif

				// This will start a thread that will complete the initializtion from the background.
				ThreadPool.QueueUserWorkItem(new WaitCallback(InitializationThread));

#if DEBUG
			}
#endif

		}

		/// <summary>
		/// Initialize the background attributes of this control.
		/// </summary>
		private void InitializationThread(object parameter)
		{

			// In the off chance that this control is initialized after all the data has been loaded, then this initialization will
			// generate the objects that appear in the list view.  Generally, this control will be initialized long before the data
			// arrives, but you never know how a control will be used and abused in the future.
			ArrayList objectList = new ArrayList();

			try
			{

				// Lock the tables required to build an object.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.FolderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SystemFolderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Install the event handlers
				ClientMarketData.Object.ObjectRowChanging += new DataSetMarket.ObjectRowChangeEventHandler(ChangeObjectRow);
				ClientMarketData.EndMerge += new EventHandler(EndMerge);

				// Create a list of objects that match the peristent identifers that were stored in the user preferences.
				foreach (int objectId in this.objectIdList)
				{
					MarkThree.Guardian.Object guardianObject = MarkThree.Guardian.Object.CreateObject(objectId);
					if (!System.Object.ReferenceEquals(guardianObject, null))
						objectList.Add(guardianObject);
				}

			}
			catch (Exception exception)
			{

				// Catch the most general error and send it to the debug console.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the tables used to build the objects.
				if (ClientMarketData.AccountLock.IsReaderLockHeld)
					ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.FolderLock.IsReaderLockHeld)
					ClientMarketData.FolderLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld)
					ClientMarketData.UserLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.SystemFolderLock.IsReaderLockHeld)
					ClientMarketData.SystemFolderLock.ReleaseReaderLock();
				if (ClientMarketData.TypeLock.IsReaderLockHeld)
					ClientMarketData.TypeLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// If the initial pass through the data model found some objects that are ready to be installed in the Guardian Bar
			// list, then pass them on to the foreground.  Note that this thread will wait here until there is a window to which
			// the results can be passed.
			if (objectList.Count != 0)
			{
				this.handleCreatedEvent.WaitOne();
				Invoke(this.objectAvailableCallback, new object[] { objectList });
			}
		
		}

		/// <summary>
		/// Handles a change to the Folder List data.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void ChangeObjectRow(object sender, ClientMarketData.ObjectRowChangeEvent objectRowChangeEvent)
		{

			// This will catch all the new objects added to the client data model and pass them on to be filtered further by a
			// handler outside of the Data Model event updates.
			if (objectRowChangeEvent.Action == DataRowAction.Commit)
				if (!objectRowChangeEvent.Row.HasVersion(DataRowVersion.Original))
					this.newObjectIdList.Add(objectRowChangeEvent.Row.ObjectId);

		}

		/// <summary>
		/// Will initiate a Folder List refresh if the data or the structure of the document has changed.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void EndMerge(object sender, EventArgs e)
		{

			// Create a thread to add any new objects to the data model.
			if (this.newObjectIdList.Count != 0)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(NewObjectThread), this.newObjectIdList);
				this.newObjectIdList = new ArrayList();
			}

		}

		/// <summary>
		/// Determines if any of the newly added objects should appear in this control.
		/// </summary>
		/// <param name="parameter">A list of newly added object identifiers.</param>
		private void NewObjectThread(object parameter)
		{

			// This will purge the list of new objects to the data model down to only the items that have previously been selected 
			// for this control.  The list of objects in this control is a critical resources that is shared between threads.
			ArrayList newObjectIdList = (ArrayList)parameter;
			lock (this)
			{
				for (int index = 0; index < newObjectIdList.Count; )
					if (!this.objectIdList.Contains(newObjectIdList[index]))
						newObjectIdList.RemoveAt(index);
					else
						index++;
			}

			// Any new objects are created in the background from the object identifier.  These objects are passed to the
			// foreground where they can be added to the control.
			ArrayList objectList = new ArrayList();

			try
			{

				// Lock the tables required to build an object.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.FolderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SystemFolderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TypeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This will build an object for every object identifier in the list.
				foreach (int objectId in newObjectIdList)
				{
					MarkThree.Guardian.Object guardianObject = MarkThree.Guardian.Object.CreateObject(objectId);
					if (!System.Object.ReferenceEquals(guardianObject, null))
						objectList.Add(guardianObject);
				}

			}
			catch (Exception exception)
			{

				// Catch the most general error and send it to the debug console.
				EventLog.Error("{0}, {1}", exception.Message, exception.Message);

			}
			finally
			{

				// Release the tables used to build the folder list.
				if (ClientMarketData.AccountLock.IsReaderLockHeld)
					ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.FolderLock.IsReaderLockHeld)
					ClientMarketData.FolderLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld)
					ClientMarketData.UserLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.SystemFolderLock.IsReaderLockHeld)
					ClientMarketData.SystemFolderLock.ReleaseReaderLock();
				if (ClientMarketData.TypeLock.IsReaderLockHeld)
					ClientMarketData.TypeLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Wait for the window handle to be created before trying to write to the control.  The thread only needs to wait once.
			// After that, this event is permanently signaled.
			this.handleCreatedEvent.WaitOne();

			// Send the new items to the foreground to be place in the control.  The 'this.InvokeRequired' is needed during the
			// shutdown to prevent items from being sent on a thread that no longer exists.
			if (this.InvokeRequired)
				Invoke(this.objectAvailableCallback, new object[] { objectList });

		}

		public void Open(object viewerObject)
		{

			object subObject = viewerObject;
			if (viewerObject is BlotterMatchDetail)
				subObject = ((BlotterMatchDetail)viewerObject).Blotter;

			for (int index = 0; index < this.listView.Items.Count; index++)
			{

				GuardianViewItem guardianViewItem = (GuardianViewItem)this.listView.Items[index];

				if (guardianViewItem.Object.Equals(subObject))
				{
					this.isOpenEventAllowed = false;
					this.listView.SelectedIndices.Clear();
					this.listView.SelectedIndices.Add(index);
					this.isOpenEventAllowed = true;
				}

			}

		}

		/// <summary>
		/// Handles objects added to the control.
		/// </summary>
		/// <param name="objectList">A list of objects to be added to the control.</param>
		private void OnObjectAvailable(ArrayList objectList)
		{

			// Add each of the items in the list to the viewer.
			foreach (MarkThree.Guardian.Object guardianObject in objectList)
				AddObject(guardianObject);

		}

		/// <summary>
		/// Multicasts a request to open up an object to anyone listening.
		/// </summary>
		/// <param name="objectArgs">Parameters used to open an object.</param>
		protected virtual void OnOpenObject(MarkThree.Guardian.Object guardianObject)
		{

			// Tell any object that is listening that it should open an object in a viewer.
			if (this.isOpenEventAllowed && this.OpenObject != null)
				OpenObject(this, guardianObject);

		}

		/// <summary>
		/// Handles the Drag-and-Drop event where an object is placed over this control.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void listView_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{

			// Set the cursor to show that copying is allowed when the item comes from the TreeView.
			e.Effect = DragDropEffects.None;
			foreach(String format in e.Data.GetFormats(true))
				if (format == typeof(TreeNode).ToString())
					e.Effect = DragDropEffects.Copy;

		}

		/// <summary>
		/// Handles the Drag-and-Drop event where the object is dropped into the control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void listView_DragDrop(object sender, DragEventArgs e)
		{

			// Extract the TreeNode from the drag-and-drop event.  The object is placed in the Tag of the TreeView and
			// can be added directly to the control.
			TreeNode childNode = (TreeNode)e.Data.GetData(typeof(TreeNode).ToString());
			AddObject((MarkThree.Guardian.Object)childNode.Tag);

		}

		/// <summary>
		/// Adds an object to the ListView control.
		/// </summary>
		/// <param name="guardianObject">The object to be added.</param>
		private void AddObject(MarkThree.Guardian.Object guardianObject)
		{

			// See if the object's image exists in the list of images yet.  If it doesn't both the small and large bitmaps are
			// added.
			int imageIndex = this.listView.LargeImageList.Images.IndexOfKey(guardianObject.GetType().ToString());
			if (imageIndex == -1)
			{
				imageIndex = this.listView.LargeImageList.Images.Count;
				this.listView.LargeImageList.Images.Add(guardianObject.GetType().ToString(), guardianObject.Image32x32);
				this.listView.SmallImageList.Images.Add(guardianObject.GetType().ToString(), guardianObject.Image16x16);
			}

			// The 'GuardianViewItem' adds some extra features to the standard ListViewItem, such as the MarkThree.Guardian.Object
			// that specifies what kind of object is added to the tree.  This extra information is used to open up a viewer when
			// the item is selected.
			this.listView.Items.Add(new GuardianViewItem(guardianObject, imageIndex));

		}

		/// <summary>
		/// Opens an object in a viewer when a new item in the list is selected.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{

			// If the item has been selected, then extract the object information from the ListView object and broadcast the event
			// to anyone listening.
			if (e.IsSelected)
				this.OnOpenObject(((GuardianViewItem)e.Item).Object);

		}

		/// <summary>
		/// Sets the view to use the large icons.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// Use the large images.
			this.listView.View = View.LargeIcon;

		}

		/// <summary>
		/// Sets the view to use the small icons.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// View the small images in the list view.
			this.listView.View = View.SmallIcon;

		}

		/// <summary>
		/// Clears the view of all the shortcuts.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{

			// Remove all the items from the control.
			this.listView.Clear();

		}

	}

}
