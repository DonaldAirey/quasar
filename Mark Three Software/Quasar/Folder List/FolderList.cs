/************************************************************************************************************************
*
*	File:			FolderList.cs
*	Description:	This user control is used to navigate between accounts, users, trading desks, securities and any other
*					object that might be avaiable to a user.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Common.Controls
{

	using Shadows.Quasar.Client;
	using Shadows.Quasar.Common;
	using Shadows.Quasar.Common.Controls;
	using System;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.ComponentModel;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// Graphical control used to browse through financial objects as though they were a file system.
	/// </summary>
	public class FolderList : System.Windows.Forms.UserControl
	{

		private bool hasNewData;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemRename;
		private System.Windows.Forms.MenuItem menuItemOpen;
		private System.Windows.Forms.MenuItem menuItemDelete;
		private System.Windows.Forms.MenuItem menuItemProperties;
		private Shadows.Quasar.Client.ClientMarketData clientClientMarketData;
		private delegate void ObjectNodeDelegate(ObjectNode objectNode);
		private ObjectNodeDelegate objectNodeDelegate;
		private ManualResetEvent handleCreatedEvent;
		private static ObjectNode dragNode;
		private static ObjectNode highlightNode;
		private CommandQueue commandQueue;

		/// <summary>
		/// Will notify clients when the user has selected an object from the hierarchy.
		/// </summary>
		public event ObjectOpenEventHandler ObjectOpen;

		// Thread safe access to the 'Data Changed' flag.
		private bool HasNewData
		{
			get {bool tempFlag; lock (this) tempFlag = this.hasNewData; return tempFlag;}
			set {lock (this) this.hasNewData = value;}
		}

		/// <summary>
		/// Initializes a new instance of the <code>Shadows.Quasar.Controls.FolderList</code> class.
		/// </summary>
		public FolderList()
		{

			// Initialize the components using the Visual Studio supplied routine.
			InitializeComponent();

			// IMPORTANT CONCEPT: The thread queue serializes all the worker threads associated with this control.  That is, it
			// forces them to execute in order, in the background.  Each cell control has one queue for commands that shouldn't or
			// can't be handled by the main window thread: the 'commandQueue'.  These commands typically are task that are done
			// client-side.
			this.commandQueue = new CommandQueue();

			// This delegate is used to pass the tree structure from the background initialization thread into the main
			// application thread.  That is, we're going to launch a background thread to get the data from the server.
			// When that thread has the data, it needs to be placed in the TreeView control.  Since we can't access the
			// control from outside this thread, we need to provide a delegate that can populate the TreeView control.
			objectNodeDelegate = new ObjectNodeDelegate(RefreshTree);

			// The coordination of this thread is a little complicated.  The background thread will marshal the data. It
			// can't be placed in the foreground thread until the window has been created (even though we launch the
			// thread from this initializer, the window where the data goes won't be created until sometime after this
			// initialize has exited).  So the background thread needs to wait for two things: the data and the window.
			// The 'HandleCreated' event will signal the background thread using the 'handleCreatedEvent' that the handle has
			// been created.  The background thread can wait on this event after it's collected the data.
			this.treeView.HandleCreated += new EventHandler(treeView_HandleCreated);
			this.handleCreatedEvent = new ManualResetEvent(false);

			try
			{

				// Lock all the tables that we'll reference while building a blotter document.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
			
				// Install the event handlers for refreshing the FolderList when the data model changes.
				ClientMarketData.Object.ObjectRowChanged += new ClientMarketData.ObjectRowChangeEventHandler(this.ObjectRowChangeEvent);
				ClientMarketData.Object.ObjectRowDeleted += new ClientMarketData.ObjectRowChangeEventHandler(this.ObjectRowChangeEvent);
				ClientMarketData.ObjectTree.ObjectTreeRowChanged += new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ObjectTreeRowChangeEvent);
				ClientMarketData.ObjectTree.ObjectTreeRowDeleted += new ClientMarketData.ObjectTreeRowChangeEventHandler(this.ObjectTreeRowChangeEvent);
				ClientMarketData.EndMerge += new EventHandler(this.DrawEvent);

			}
			catch (Exception e)
			{

				// Catch the most general error and send it to the debug console.
				Debug.WriteLine(e.Message);

			}
			finally
			{

				// Release the tables used to build the folder list.
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// This will force a refresh when the control is initialized.  After that, changes will be event driven.
			this.HasNewData = true;
			this.DrawEvent(this, EventArgs.Empty);

		}

		#region Standard Dispose Method
		/// <summary>Releases all the resources used by Shadows.Quasar.Container.FormMain</summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only
		/// unmanaged resources.
		/// </param>
		protected override void Dispose(bool disposing)
		{

			// Kill the thread if it's still waiting for data.
			CommandQueue.Abort();

			// Clean up the resources used by installed components.
			if (disposing && components != null) 
				components.Dispose();

			// Call the base class to finish up the cleanup.
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FolderList));
			this.treeView = new System.Windows.Forms.TreeView();
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			this.menuItemOpen = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItemDelete = new System.Windows.Forms.MenuItem();
			this.menuItemRename = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemProperties = new System.Windows.Forms.MenuItem();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.clientClientMarketData = new Shadows.Quasar.Client.ClientMarketData(this.components);
			this.SuspendLayout();
			// 
			// treeView
			// 
			this.treeView.AllowDrop = true;
			this.treeView.ContextMenu = this.contextMenu;
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.ImageList = this.imageList;
			this.treeView.LabelEdit = true;
			this.treeView.Name = "treeView";
			this.treeView.Size = new System.Drawing.Size(150, 112);
			this.treeView.Sorted = true;
			this.treeView.TabIndex = 0;
			this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView_MouseDown);
			this.treeView.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView_DragOver);
			this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
			this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
			this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView_ItemDrag);
			this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView_DragDrop);
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuItemOpen,
																						this.menuItem3,
																						this.menuItemDelete,
																						this.menuItemRename,
																						this.menuItem1,
																						this.menuItemProperties});
			// 
			// menuItemOpen
			// 
			this.menuItemOpen.Index = 0;
			this.menuItemOpen.Text = "&Open";
			this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.Text = "-";
			// 
			// menuItemDelete
			// 
			this.menuItemDelete.Index = 2;
			this.menuItemDelete.Text = "&Delete";
			// 
			// menuItemRename
			// 
			this.menuItemRename.Index = 3;
			this.menuItemRename.Text = "&Rename";
			this.menuItemRename.Click += new System.EventHandler(this.menuItemRename_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 4;
			this.menuItem1.Text = "-";
			// 
			// menuItemProperties
			// 
			this.menuItemProperties.Index = 5;
			this.menuItemProperties.Text = "&Properties";
			this.menuItemProperties.Click += new System.EventHandler(this.menuItemProperties_Click);
			// 
			// imageList
			// 
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Aqua;
			// 
			// FolderList
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.treeView});
			this.Name = "FolderList";
			this.Size = new System.Drawing.Size(150, 112);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Public access to the AppraisalViewer's thread queue.
		/// </summary>
		[Browsable(false)]
		public CommandQueue CommandQueue {get {return this.commandQueue;}}

		/// <summary>
		/// Handles a change to the Folder List data.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void ObjectRowChangeEvent(object sender, ClientMarketData.ObjectRowChangeEvent objectRowChangeEvent)
		{

			// Setting this flag will cause the next refresh to incrementally update the Folder List viewer if the 
			// structure hasn't changed.
			this.HasNewData = true;

		}

		/// <summary>
		/// Handles a change to the Folder List data.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void ObjectTreeRowChangeEvent(object sender, ClientMarketData.ObjectTreeRowChangeEvent objectTreeRowChangeEvent)
		{

			// Setting this flag will cause the next refresh to incrementally update the Folder List viewer if the structure hasn't
			// changed.
			this.HasNewData = true;

		}

		/// <summary>
		/// Will initiate a Folder List refresh if the data or the structure of the document has changed.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">This aregument isn't used.</param>
		private void DrawEvent(object sender, EventArgs e)
		{

			// If new data or a new structure is available after the data model has changed, then will initiate a refresh
			// of the Folder List contents.
			if (this.HasNewData)
			{

				// Start a worker thread to refresh the appraisal.
				CommandQueue.Execute(new ThreadHandler(FolderListThread));

				// Clear the flag until an event indicates there is new data.
				this.HasNewData = false;

			}

		}

		/// <summary>
		/// This procedure is used to update the TreeView control from the background.
		/// </summary>
		private void FolderListThread(params object[] argument)
		{

			// This node acts as the root that we'll use to build the tree folder list tree.  It will eventually be
			// passed back to the main thread and into the TreeView control.
			ObjectNode rootNode = new ObjectNode();

			try
			{

				// Lock all the tables that we'll reference while building a blotter document.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.FolderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.LoginLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Recursively build the Tree View from the user's folder.
				ClientMarketData.LoginRow loginRow = ClientMarketData.Login.FindByLoginId(ClientPreferences.LoginId);
				if (loginRow != null && !loginRow.IsFolderIdNull())
				{
					ClientMarketData.FolderRow folderRow = ClientMarketData.Folder.FindByFolderId(loginRow.FolderId);
					rootNode.Nodes.Add(new ObjectNode(folderRow.ObjectRow));
				}
				
			}
			catch (Exception e)
			{

				// Catch the most general error and send it to the debug console.
				Debug.WriteLine(e.Message);

			}
			finally
			{

				// Release the tables used to build the folder list.
				if (ClientMarketData.FolderLock.IsReaderLockHeld) ClientMarketData.FolderLock.ReleaseReaderLock();
				if (ClientMarketData.LoginLock.IsReaderLockHeld) ClientMarketData.LoginLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// Wait for the window handle to be created before trying to write to the control.  The thread only needs to wait once.
			// After that, this event is permanently signaled.
			this.handleCreatedEvent.WaitOne();

			// At this point, the tree is complete.  This background thread needs to wait for a signal from the main application
			// thread that the TreeView control has been created.  Once we get the signal, pass the entire tree structure back to
			// the main thread so it can copy the tree members.  Note that the handle needs to be checked before invoking the
			// foreground method, even though we waited above for the "Handle Create" event.  This test is meant to prevent events
			// from being called when the application is shutting down.
			if (this.IsHandleCreated)
				Invoke(objectNodeDelegate, new object[] {rootNode});

		}

		/// <summary>
		/// Populates the Tree Control with data collected in the initialization thread.
		/// </summary>
		/// <param name="objectNode">Contains a hierarchical organization of objects.</param>
		private void RefreshTree(ObjectNode objectNode)
		{

			// This will recursively reconstruct the tree from the relational data gathered from the background thread.
			RecurseRefresh(this.treeView.Nodes, objectNode.Nodes);

		}

		/// <summary>
		/// Recursively updates the TreeView control with a new tree structure.
		/// </summary>
		/// <param name="targetNodeCollection">The target tree for the changes.</param>
		/// <param name="sourceNodeCollection">The source tree containing the new structure.</param>
		private void RecurseRefresh(TreeNodeCollection targetNodeCollection, TreeNodeCollection sourceNodeCollection)
		{

			// The main idea here is to run through all the nodes looking for Adds, Deletes and Updates.  When we find a
			// Node that shouldn't be in the structure, it's deleted from the tree.  Since the tree is made up of linked
			// lists, we can't rightly delete the current link in the list.  For this reason we need to use the index into
			// the list for spanning the structure instead of the collection operations.  When we find an element that
			// needs to be removed, we can delete it and the index will get us safely to the next element in the list. The
			// first loop scans the list already in the TreeView structure.
			for (int targetIndex = 0; targetIndex < targetNodeCollection.Count; targetIndex++)
			{

				// Get a reference to the current node in the target (older) list of nodes.
				ObjectNode childTargetNode = (ObjectNode)targetNodeCollection[targetIndex];

				// If we don't find the target (older) element in the source (newer) list, it will be deleted.
				bool found = false;

				// Cycle through all of the source (newer) elements looking for changes and removing any elements that
				// exist in both lists.
				for (int sourceIndex = 0; sourceIndex < sourceNodeCollection.Count; sourceIndex++)
				{

					// Get a reference to the current node in the source (newer) list of nodes.
					ObjectNode childSourceNode = (ObjectNode)sourceNodeCollection[sourceIndex];

					// If the elements are equal (as defined by the key element), then recurse into the structure looking
					// for changes to the children.  After that, check the Node for any changes since it was added to the
					// tree.
					if (childTargetNode.ObjectId == childSourceNode.ObjectId)
					{

						// Recurse down into the tree structures bringing all the children in sync with the new structure.
						RecurseRefresh(childTargetNode.Nodes, childSourceNode.Nodes);

						// Check the Nodes Name.  Update it if there's a change.
						if (childTargetNode.Text != childSourceNode.Name)
							childTargetNode.Text = childSourceNode.Name;

						// At this point, we've checked all the children and applied any changes to the node.  Remove it
						// from the list.  Any elements left in the source list are assumed to be new members and will be
						// added to the tree structure.  That's why it's important to remove the ones already in the tree.
						sourceNodeCollection.Remove(childSourceNode);
						sourceIndex--;

						// This will signal the target loop that this element still exists in the structure.  If it isn't
						// found, it'll be deleted.
						found = true;

					}

				}

				// If the target (older) element isn't found in the source (newer) tree, it's deleted.
				if (!found)
				{
					targetNodeCollection.Remove(childTargetNode);
					targetIndex--;
				}

			}

			// Any element that doesn't already exist in the target (older) tree, is copied from the source (newer) tree,
			// along with all it's children.
			for (int nodeIndex = 0; nodeIndex < sourceNodeCollection.Count; nodeIndex++)
			{
				ObjectNode objectNode = (ObjectNode)sourceNodeCollection[nodeIndex--];
				sourceNodeCollection.Remove(objectNode);
				targetNodeCollection.Add(objectNode);
			}

		}

		/// <summary>
		/// Construct a hierarchical tree structure by recursively scanning the parent-child relations.
		/// </summary>
		/// <param name="objectNode">The current node in the tree structure.</param>
		private void RecurseNodes(ObjectNode objectNode)
		{

			// Recursion is sometimes difficult to follow.  At this point, we have an object node with a valid object. We
			// know that all the ancestors of this node have been populated already, so we're just concerned with finding
			// the descendants.  The 'ObjectTree' table can be used effectively for this.  Simply trace the parent
			// relation back to the 'ObjectTree' and you have a list of children.  For every child we find, we're going to
			// recurse down until there are no more descendants.
			ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(objectNode.ObjectId);
			foreach (ClientMarketData.ObjectTreeRow childRelationRow in objectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
			{

				// Trace the 'child id' column back to the 'objects' table and get the full record that belongs to
				// this relation.  We can create a new node from this information and add it to the tree.
				ObjectNode childNode = new ObjectNode(childRelationRow.ObjectRowByFKObjectObjectTreeChildId);
				objectNode.Nodes.Add(childNode);

				// Finally, go look for any children of this node.
				RecurseNodes(childNode);

			}

		}

		/// <summary>
		/// Invokes an Open Object event.
		/// </summary>
		/// <param name="sender">The object that originates the event.</param>
		/// <param name="objectArgs">The argument used to open the object.</param>
		public void OnObjectOpen(object sender, ObjectArgs objectArgs)
		{

			// If there are any listerners, then multicast the request to open a viewer.
			if (this.ObjectOpen != null)
				this.ObjectOpen(this, objectArgs);

		}
		
		/// <summary>
		/// Called when an object in the TreeView is selected.
		/// </summary>
		/// <param name="sender">The object that created the event.</param>
		/// <param name="e">Parameters specific to the event.</param>
		private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{

			// Invoke the multicaster to open the objects.
			this.OnObjectOpen(this, CreateObjectArgs((ObjectNode)e.Node));
			
		}

		/// <summary>
		/// Called when the Handle is created.
		/// </summary>
		/// <param name="sender">The object that created the event.</param>
		/// <param name="e">Parameters specific to the event.</param>
		private void treeView_HandleCreated(object sender, EventArgs e)
		{

			// Signal the background thread that the TreeView can now accept data.
			this.handleCreatedEvent.Set();

		}

		/// <summary>
		/// Creates a structure that can be used to open or operate on a generic object.
		/// </summary>
		/// <param name="objectNode">A node in the TreeView that represents an object in the Database.</param>
		/// <returns>A structure that specifies what object to view, what type of object it is, and any optional
		/// parameters that might be needed to open the object.</returns>
		private ObjectArgs CreateObjectArgs(ObjectNode objectNode)
		{

			// The returned value represents an object selected from the Folder List.  It has all the parameters needed to
			// open a reader.
			return new ObjectArgs(objectNode.ObjectTypeCode, objectNode.ObjectId, objectNode.Name);

		}

		/// <summary>
		/// Event handler for changing the name of a label.
		/// </summary>
		/// <param name="sender">The window control that generated the 'LabelEdit' event.</param>
		/// <param name="e">Event Parameters used to control the actions taken by the event handler.</param>
		private void treeView_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
		{

			// The TreeView has a bug in it: if you leave the edit mode without typing anything, the returned text of the control 
			// will be an empty string. Since we don't want to bother the server or the user with this nonsense, we'll filter out
			// the possiblity here.
			if (e.Label == null)
			{
				e.CancelEdit = true;
				return;
			}
			
			// Extract the object's properties from the node.
			ObjectNode objectNode = (ObjectNode)e.Node;

			// This command batch is constructed below and sent to the server for execution.
			RemoteBatch remoteBatch = new RemoteBatch();

			// This will insure that table locks are cleaned up.
			try
			{

				// Lock the table
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Find the object in the data model and make sure it still exists.
				ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(objectNode.ObjectId);
				if (objectRow == null)
					throw new Exception("This object has been deleted.");

				// Construct a command to rename the object.
				RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
				RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Object");
				RemoteMethod remoteMethod = remoteType.Methods.Add("Update");
				remoteMethod.Parameters.Add("rowVersion", objectRow.RowVersion);
				remoteMethod.Parameters.Add("objectId", objectRow.ObjectId);
				remoteMethod.Parameters.Add("name", e.Label);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				// Cancel the tree operation if we can't execute the command.  The text in the tree control will revert to the 
				// previous value.
				e.CancelEdit = true;

			}
			finally
			{

				// Release table locks.
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// If the command batch was built successfully, then execute it.  If any part of it should fail, cancel the edit and
			// display the server errors.
			if (remoteBatch != null)
				try
				{

					// Call the web server to rename the object on the database.  Note that this method must be called when there are
					// no locks to prevent deadlocking.  That is why it appears in it's own 'try/catch' block.
					ClientMarketData.Execute(remoteBatch);

				}
				catch (BatchException batchException)
				{

					// Undo the editing action.  This will restore the name of the object to what it was before the operation.
					e.CancelEdit = true;

					// Display each error in the batch.
					foreach (RemoteException remoteException in batchException.Exceptions)
						MessageBox.Show(remoteException.Message, "Quasar Error");

				}

		}

		/// <summary>
		/// Starts a drag-and-drop operation on the FolderList.
		/// </summary>
		/// <param name="sender">The control that is the source of the event.</param>
		/// <param name="e">Arguments associated with the event.</param>
		private void treeView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{

			// The 'dragNode' is a static value used for drag-and-drop operations within this control.  We set it here as
			// a static value rather than pass it around because an 'ObjectNode' address is no use outside this control. 
			// The 'ObjectArgs' is a more generic version of the selected item that has value to other controls.
			ObjectArgs objectArgs = CreateObjectArgs(dragNode = (ObjectNode)e.Item);

			// Extract the parameters that are useful in describing and opening up an object.  This is the value that will
			// be passed around to other controls that can consume a drag-and-drop operation from the Folder List.
			DataObject dataObject = new DataObject("ObjectArgs", objectArgs);

			// Begin the drag-and-drop operation with the element selected.
			this.DoDragDrop(dataObject, DragDropEffects.Copy | DragDropEffects.Move);

		}

		/// <summary>
		/// Handles the DragDrop event.
		/// </summary>
		/// <param name="sender">The object that generated the event.</param>
		/// <param name="e">The event parameters.</param>
		private void treeView_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{

			// Get the source and destination for the object being moved.  It's possible for either the source or the
			// destination to be invalid.  For example, trying to move a system folder is illegal.  It's also illegal to
			// try to drop an object outside the tree structure.
			ObjectNode toNode = (ObjectNode)this.treeView.GetNodeAt(this.treeView.PointToClient(new Point(e.X, e.Y)));
			ObjectNode fromNode = (ObjectNode)dragNode.Parent;
			ObjectNode childNode = (ObjectNode)dragNode;

			this.commandQueue.Execute(new ThreadHandler(DragDropHandler), toNode, fromNode, childNode);

		}

		private void DragDropHandler(params object[] arguments)
		{

			ObjectNode toNode = (ObjectNode)arguments[0];
			ObjectNode fromNode = (ObjectNode)arguments[1];
			ObjectNode childNode = (ObjectNode)arguments[2];

			// Make sure the user has selected a valid source and destination for the operation.  It's illegal to move the node
			//		1.  To the root of the tree
			//		2.  From the root of the tree
			if (toNode == null || fromNode == null)
				return;

			// Don't allow for circular references.
			try
			{

				// Lock the tables needed for this operation.
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);

				// It's critical that circular references aren't created, either by accident or design.  First, find the object 
				// record associated with the destination node.
				ClientMarketData.ObjectRow parentRow = ClientMarketData.Object.FindByObjectId(toNode.ObjectId);
				if (parentRow == null)
					throw new Exception("This object has been deleted");

				// This is the object that is being dragged.  Find the row
				ClientMarketData.ObjectRow childRow = ClientMarketData.Object.FindByObjectId(childNode.ObjectId);
				if (childRow == null)
					throw new Exception("This object has been deleted");

				if (Shadows.Quasar.Common.Relationship.IsChildObject(childRow, parentRow))
				{
					MessageBox.Show(this.TopLevelControl, "This would create a circular references.", "Quasar Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

			}
			finally
			{

				// Release table locks.
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}
			
			// Any commands created below will be constructed in this object and sent to the server for execution.
			RemoteBatch remoteBatch = new RemoteBatch();
				
			// Change the default model of an account.  When the destination is an account group, account or sub account and the
			// source is a model, a command will be constructed to change the default model.
			if (toNode.ObjectTypeCode == ObjectType.Account && dragNode.ObjectTypeCode == ObjectType.Model)
			{

				try
				{

					// Lock the tables needed for this operation.
					Debug.Assert(!ClientMarketData.AreLocksHeld);
					ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);

					// Find the account used, throw an error if it's been deleted.
					ClientMarketData.AccountRow accountRow;
					if ((accountRow = ClientMarketData.Account.FindByAccountId(toNode.ObjectId)) == null)
						throw new Exception("This account has been deleted");

					// Construct a command to change the default model associated with the account.
					RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
					RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Account");
					RemoteMethod remoteMethod = remoteType.Methods.Add("Update");
					remoteMethod.Parameters.Add("rowVersion", accountRow.RowVersion);
					remoteMethod.Parameters.Add("accountId", accountRow.AccountId);
					remoteMethod.Parameters.Add("modelId", dragNode.ObjectId);
					
				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
				finally
				{

					// Release table locks.
					if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
					Debug.Assert(!ClientMarketData.AreLocksHeld);

				}

			}
			else
			{

				// If we made it here, the drag-and-drop is interpreted as a command to move a child from one parent to another.
				try
				{

					// Lock the tables needed for this operation.
					Debug.Assert(!ClientMarketData.AreLocksHeld);
					ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);

					// Extract the primary identifiers from the user interface nodes.
					int fromId = fromNode.ObjectId;
					int toId = toNode.ObjectId;
					int childId = dragNode.ObjectId;

					// Find the object in the data model and make sure it still exists.
					ClientMarketData.ObjectTreeRow objectTreeRow = ClientMarketData.ObjectTree.FindByParentIdChildId(fromNode.ObjectId, dragNode.ObjectId);
					if (objectTreeRow == null)
						throw new Exception("This relationship has been deleted by someone else.");
				
					// Moving a child object from one parent to another must be accomplished as a transaction.  Otherwise, an
					// orhpan object will be created if the operation fails midway through.
					RemoteTransaction remoteTransaction = remoteBatch.Transactions.Add();
					RemoteAssembly remoteAssembly = remoteBatch.Assemblies.Add("Service.Core");
					RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.ObjectTree");

					// Construct a command delete the old parent relation.
					RemoteMethod deleteObjectTree = remoteType.Methods.Add("Delete");
					deleteObjectTree.Transaction = remoteTransaction;
					deleteObjectTree.Parameters.Add("rowVersion", objectTreeRow.RowVersion);
					deleteObjectTree.Parameters.Add("parentId", fromId);
					deleteObjectTree.Parameters.Add("childId", childId);

					// Construct a command insert a new parent relation.
					RemoteMethod insertObjectTree = remoteType.Methods.Add("Insert");
					insertObjectTree.Transaction = remoteTransaction;
					insertObjectTree.Parameters.Add("parentId", toId);
					insertObjectTree.Parameters.Add("childId", childId);

				}
				catch (Exception exception)
				{

					// Write the error and stack trace out to the debug listener
					Debug.WriteLine(String.Format("{0}, {1}", exception.Message, exception.StackTrace));

				}
				finally
				{

					// Release table locks.
					if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld) ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
					Debug.Assert(!ClientMarketData.AreLocksHeld);

				}

			}

			// If the command batch was built successfully, then execute it.
			if (remoteBatch != null)
				try
				{

					// Call the web server to rename the object on the database.  Note that this method must be called when there are
					// no locks to prevent deadlocking.  That is why it appears in it's own 'try/catch' block.
					ClientMarketData.Execute(remoteBatch);

				}
				catch (BatchException batchException)
				{

					// Display each error in the batch.
					foreach (RemoteException remoteException in batchException.Exceptions)
						MessageBox.Show(remoteException.Message, "Quasar Error");

				}
				
		}

		/// <summary>
		/// Provides feedback when the cursor is dragged over objects in the Folder List.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">Event argument.</param>
		private void treeView_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{

			// Determine whether the object being dragged is in a format we can use.
			bool acceptable_format = false;
			foreach(String format in e.Data.GetFormats(true))
				if (format == "ObjectArgs")
					acceptable_format = true;

			// Determine whether the cursor is over an item that can be a destination for a drag-and-drop operation.
			ObjectNode toNode = (ObjectNode)this.treeView.GetNodeAt(this.treeView.PointToClient(new Point(e.X, e.Y)));

			// If the format is acceptable and we have a valid destination, set the effect to show a drag-and-drop
			// operation is valid.  Otherwise, give the user feedback the the operation will fail.
			e.Effect = (acceptable_format && toNode != null) ? DragDropEffects.Move : DragDropEffects.None;

		}

		/// <summary>
		/// Opens the object currently selected.
		/// </summary>
		/// <param name="sender">The control that initiated the event.</param>
		/// <param name="e">Parameters associated with the event.</param>
		private void menuItemOpen_Click(object sender, System.EventArgs e)
		{

			// Send the argument off to anyone listening.  Generally, this will be the container application which will,
			// genreally, pass the event parameters to some other component.
			if (highlightNode != null)
				this.OnObjectOpen(this, CreateObjectArgs(highlightNode));

		}

		/// <summary>
		/// Context menu item handler that renames the selected item in the FolderList
		/// </summary>
		/// <param name="sender">The control that initiated the event.</param>
		/// <param name="e">Parameters associated with the event.</param>
		private void menuItemRename_Click(object sender, System.EventArgs e)
		{

			// If an item has been highlighted, then initiate the editing mode.
			if (highlightNode != null)
				highlightNode.BeginEdit();

		}

		/// <summary>
		/// Handles the mouse button down events.
		/// </summary>
		/// <param name="sender">The control that initiated the event.</param>
		/// <param name="e">Parameters associated with the event.</param>
		private void treeView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{

			// A right mouse down event means that we're going to highlight some element and bring up a context menu.  
			// For now, save the highlighted node in a static element and let the context menu decide what action to take.
			if (e.Button == MouseButtons.Right)
			{
				highlightNode = (ObjectNode)this.treeView.GetNodeAt(new Point(e.X, e.Y));
			}

		}

		/// <summary>
		/// Handles the properties of the objects in the Object Tree.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemProperties_Click(object sender, System.EventArgs e)
		{

			// Only invoke the properties if the node is highlighted.
			if (highlightNode == null)
				return;

			// Handle the different properties based on the object type.
			switch (highlightNode.ObjectTypeCode)
			{

				case ObjectType.Blotter:

					// Invoke the blotter properties dialog.
					BlotterPropertiesDialog blotterPropertiesDialog;
					blotterPropertiesDialog = new  BlotterPropertiesDialog();
					blotterPropertiesDialog.Show(highlightNode.ObjectId);
					blotterPropertiesDialog.Close();

					break;
				
				case ObjectType.Account:

					// Invoke the account properties dialog.
					AccountPropertiesDialog accountPropertiesDialog;
					accountPropertiesDialog = new AccountPropertiesDialog();
					accountPropertiesDialog.Show(highlightNode.ObjectId);
					accountPropertiesDialog.Close();

					break;

			}

		}

	}

}
