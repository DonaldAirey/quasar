namespace MarkThree.Guardian.Forms
{

	using MarkThree.Client;
	using MarkThree.Forms;
	using MarkThree.Guardian;
	using MarkThree.Guardian.Forms;
	using MarkThree.Guardian.Client;
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.ComponentModel;
	using System.Reflection;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// Graphical control used to browse through financial objects as though they were a file system.
	/// </summary>
	public partial class FolderList : System.Windows.Forms.UserControl
	{

		private bool hasNewData;
		private bool isOpenEventAllowed;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemRename;
		private System.Windows.Forms.MenuItem menuItemOpen;
		private System.Windows.Forms.MenuItem menuItemDelete;
		private System.Windows.Forms.MenuItem menuItemProperties;
		private System.Windows.Forms.TreeView treeView;
		private delegate void TreeNodeDelegate(TreeNode treeNode, ArrayList images);
		private delegate void RenameNodeDelegate(TreeNode treeNode, string name);
		private delegate void MessageDelegate(string message, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon);
		private static TreeNode highlightNode;
		private ClientMarketData clientMarketData;

		/// <summary>
		/// Will notify clients when the user has selected an object from the hierarchy.
		/// </summary>
		public event OpenObjectEventHandler OpenObject;

		/// <summary>
		/// Initializes a new instance of the <code>MarkThree.Guardian.FolderList</code> class.
		/// </summary>
		public FolderList()
		{

			// Initialize the components using the Visual Studio supplied routine.
			InitializeComponent();

			// Initialize the object
			this.isOpenEventAllowed = true;

		}

		/// <summary>
		/// Handles the creation of a window handle.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnHandleCreated(EventArgs e)
		{

#if DEBUG
			// This will prevent the background thread from killing the designer when it attempts to load libraries.
			if (!this.DesignMode)
			{
#endif

				// This control window uses the events of the data model to drive changes out to the user interface.  Those event 
				// handlers, since they are part of the client data model, can only be accessed from a background thread capable of
				// locking the data model from other threads.
				ThreadPool.QueueUserWorkItem(new WaitCallback(InitializeData));

#if DEBUG
			}
#endif

			// Let the base class handle the remainder of the event.
			base.OnHandleCreated(e);

		}

		/// <summary>
		/// Handles the destruction of a window handle.
		/// </summary>
		/// <param name="e">The event arguments.</param>
		protected override void OnHandleDestroyed(EventArgs e)
		{

#if DEBUG
			// This will prevent the background thread from killing the designer when it attempts to unload libraries.
			if (!this.DesignMode)
			{
#endif

				// When the window handle is destroyed, this control will remove itself from the data model event handlers.
				ThreadPool.QueueUserWorkItem(new WaitCallback(DestroyData));

#if DEBUG
			}
#endif

			// Allow the base class to handle the remainder of the event.
			base.OnHandleDestroyed(e);

		}

		/// <summary>
		/// Display an error message.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="errorEventArgs">The event arguments.</param>
		private void ShowMessage(string message, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon)
		{

			// Display the message.
			MessageBox.Show(this.TopLevelControl, message, Application.SafeTopLevelCaptionFormat, messageBoxButtons,
				messageBoxIcon);

		}

		/// <summary>
		/// Initializes the interaction with the data model.
		/// </summary>
		/// </summary>
		/// <param name="parameter">Not used.</param>
		private void InitializeData(object parameter)
		{

			try
			{

				// Lock all the tables that we'll reference while building a blotter document.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);

				// This will provide the initial view of the hierachy tree.  After the initial view, the rest of the changes are driven
				// from events.
				ThreadPool.QueueUserWorkItem(new WaitCallback(FolderListThread), new ArrayList());

				// Install the event handlers for refreshing the FolderList when the data model changes.
				ClientMarketData.User.UserRowChanged += new ClientMarketData.UserRowChangeEventHandler(ChangeUserRow);
				ClientMarketData.Object.ObjectRowChanged += new ClientMarketData.ObjectRowChangeEventHandler(ChangeObjectRow);
				ClientMarketData.ObjectTree.ObjectTreeRowChanged += new ClientMarketData.ObjectTreeRowChangeEventHandler(ChangeObjectTreeRow);
				ClientMarketData.EndMerge += new EventHandler(EndMerge);

			}
			catch (Exception exception)
			{

				// Catch the most general error and send it to the debug console.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the tables used to build the folder list.
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld)
					ClientMarketData.UserLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Removes this control from the data model event handlers.
		/// </summary>
		/// <param name="parameter">Not used.</param>
		private void DestroyData(object parameter)
		{

			try
			{

				// Lock the tables.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Remove the event handlers.
				ClientMarketData.User.UserRowChanged -= new ClientMarketData.UserRowChangeEventHandler(ChangeUserRow);
				ClientMarketData.Object.ObjectRowChanged -= new ClientMarketData.ObjectRowChangeEventHandler(ChangeObjectRow);
				ClientMarketData.ObjectTree.ObjectTreeRowChanged -= new ClientMarketData.ObjectTreeRowChangeEventHandler(ChangeObjectTreeRow);
				ClientMarketData.EndMerge -= new EventHandler(EndMerge);

			}
			catch (Exception exception)
			{

				// Catch the most general error and send it to the debug console.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the tables.
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld)
					ClientMarketData.UserLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		public void Open(object viewerObject)
		{

			object subObject = viewerObject;
			if (viewerObject is BlotterMatchDetail)
				subObject = ((BlotterMatchDetail)viewerObject).Blotter;
			
			TreeNode selectedNode = Find(subObject);
			if (selectedNode != null)
			{

				this.isOpenEventAllowed = false;
				this.treeView.SelectedNode = selectedNode;
				this.isOpenEventAllowed = true;

			}

		}

		private TreeNode Find(object viewerObject)
		{

			foreach (TreeNode childNode in this.treeView.Nodes)
			{
				TreeNode foundNode = RecurseFind(childNode, viewerObject);
				if (foundNode != null)
					return foundNode;
			}

			return null;

		}

		private TreeNode RecurseFind(TreeNode treeNode, object viewerObject)
		{

			if (treeNode.Tag.Equals(viewerObject))
				return treeNode;

			foreach (TreeNode childNode in treeNode.Nodes)
			{
				TreeNode foundNode = RecurseFind(childNode, viewerObject);
				if (foundNode != null)
					return foundNode;
			}

			return null;

		}

		/// <summary>
		/// Handles a change to the ClientMarketData.User table.
		/// </summary>
		/// <param name="sender">User that generated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void ChangeUserRow(object sender, ClientMarketData.UserRowChangeEvent userRowChangeEvent)
		{

			// Setting this flag will cause the next refresh to incrementally update the Folder List viewer if the 
			// structure hasn't changed.
			this.hasNewData = true;

		}

		/// <summary>
		/// Handles a change to the ClientMarketData.Object table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void ChangeObjectRow(object sender, ClientMarketData.ObjectRowChangeEvent objectRowChangeEvent)
		{

			// Setting this flag will cause the next refresh to incrementally update the Folder List viewer if the 
			// structure hasn't changed.
			this.hasNewData = true;

		}

		/// <summary>
		/// Handles a change to the ClientMarketData.ObjectTree table.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void ChangeObjectTreeRow(object sender, ClientMarketData.ObjectTreeRowChangeEvent objectTreeRowChangeEvent)
		{

			// Setting this flag will cause the next refresh to incrementally update the Folder List viewer if the structure hasn't
			// changed.
			this.hasNewData = true;

		}

		/// <summary>
		/// Handles changes to the data model after a reconcilliation.
		/// </summary>
		/// <param name="sender">Object that generated the event.</param>
		/// <param name="e">The event arguments.</param>
		private void EndMerge(object sender, EventArgs e)
		{

			// If any elements of the data model that are displayed in this control have been modified during the reconcillation,
			// then a new version of the tree will be generated.  This method will spawn a worker thread to regenerate the data
			// that appears in the view and will send it to the foreground where it will be used to udpate the current view. Once
			// the event has been handled, it is reset until the data used to generate this view is changed.
			if (this.hasNewData)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(FolderListThread));
				this.hasNewData = false;
			}

		}

		/// <summary>
		/// This procedure is used to update the TreeView control from the background.
		/// </summary>
		private void FolderListThread(object parameter)
		{

			// This node acts as the root that we'll use to build the tree folder list tree.  It will eventually be passed back to
			// the message loop thread and into the TreeView control.
			TreeNode rootNode = new TreeNode();

			// This will hold the set of images associated with the objects in the tree.  It is also passed to the foreground where
			// it is used to generate the ImageList that holds the images for this TreeView.  The images in this list are
			// associated with the nodes in the view through the image tag, which serves as a unique identifier for the images.
			ArrayList images = new ArrayList();

			// The user identifier drives the selection of items for the navigation tree.  The top level item will be the system
			// folder(s) assigned to this user.  But before the tree can be constructed, we need to know what user is currently
			// logged in.
			int userId = Preferences.UserId;

			try
			{

				// Lock all the tables that we'll reference while building a blotter document.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.BlotterLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.FolderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SystemFolderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.UserLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Build the hierarchy by recursing into all the children of the user's folder.
				ClientMarketData.UserRow userRow = ClientMarketData.User.FindByUserId(userId);
				if (userRow != null && !userRow.IsSystemFolderIdNull())
				{
					ClientMarketData.SystemFolderRow systemFolderRow =
						ClientMarketData.SystemFolder.FindBySystemFolderId(userRow.SystemFolderId);
					rootNode.Nodes.Add(CreateTreeNode(images, systemFolderRow.FolderRow.ObjectRow));
				}

			}
			catch (Exception exception)
			{

				// Catch the most general error and send it to the debug console.
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the tables used to build the folder list.
				if (ClientMarketData.BlotterLock.IsReaderLockHeld)
					ClientMarketData.BlotterLock.ReleaseReaderLock();
				if (ClientMarketData.FolderLock.IsReaderLockHeld)
					ClientMarketData.FolderLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				if (ClientMarketData.SystemFolderLock.IsReaderLockHeld)
					ClientMarketData.SystemFolderLock.ReleaseReaderLock();
				if (ClientMarketData.TypeLock.IsReaderLockHeld)
					ClientMarketData.TypeLock.ReleaseReaderLock();
				if (ClientMarketData.UserLock.IsReaderLockHeld)
					ClientMarketData.UserLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// At this point, the tree is complete.  Pass the tree to the foreground for drawing.
			Invoke(new TreeNodeDelegate(UpdateTree), new object[] { rootNode, images });

		}

		/// <summary>
		/// Construct a hierarchical tree structure by recursively scanning the parent-child relations.
		/// </summary>
		/// <param name="treeNode">The current node in the tree structure.</param>
		private void RecurseNodes(ArrayList images, TreeNode treeNode, ClientMarketData.ObjectRow objectRow)
		{

			// Add all the children of this object to the tree.  This, in turn, will add all the children of the children, and so
			// on until we reach the end of the tree.  Simple, elegant, powerful.
			foreach (ClientMarketData.ObjectTreeRow childRelationRow in
				objectRow.GetObjectTreeRowsByObjectObjectTreeParentId())
				treeNode.Nodes.Add(CreateTreeNode(images, childRelationRow.ObjectRowByObjectObjectTreeChildId));

		}

		/// <summary>
		/// Creates a TreeNode with the attributes of an object found in the database.
		/// </summary>
		/// <param name="objectRow">A row that describes how to construct the attributes for this node.</param>
		/// <returns>A node that contains the attributes of the object found in the database, and its descendants.</returns>
		private TreeNode CreateTreeNode(ArrayList images, ClientMarketData.ObjectRow objectRow)
		{

			// This node will be attached to the tree view in a hierarchical order according to the data found in the 'ObjectTree'
			// data structure.  The node itself will be given the properties of the object in the database used to create this 
			// node.
			TreeNode treeNode = new TreeNode();

			// IMPORTANT CONCEPT: This will extract the specification for the type from the database and construct an instance of 
			// an object of that type.  For instance, the specification 'MarkThree.Guardian.Blotter, Object Library' would be used
			// to build a 'MarkThree.Guardian.Blotter' object from the 'Object Library'.  The object identifier is used to populate
			// the object from the persistent store with the properties of that item.  Once the object is built, it is installed in
			// the tree. When the user selects and item on the tree to be opened, this object is passed to the container which will
			// know how to view the object based on its type and the viewer mapped to that type.
			TypeSpecification typeSpecification = new TypeSpecification(objectRow.TypeRow.Specification);
			Assembly assembly = Assembly.Load(typeSpecification.AssemblyName);
			MarkThree.Guardian.Object folderObject = (MarkThree.Guardian.Object)assembly.CreateInstance(
				typeSpecification.TypeName, false, BindingFlags.CreateInstance, null, new object[] { objectRow.ObjectId },
				System.Globalization.CultureInfo.CurrentCulture, null);

			// The attributes of the object just created become the main attributes of the TreeNode.
			treeNode.Tag = folderObject;
			treeNode.Text = folderObject.Name;

			// The 'Image' is the picture that is displayed in the TreeView along with the text.  It's the visual que that shows
			// what kind of object is stored on the tree.  The 'ImageKey' is a unique key that allows the TreeView control to map a
			// node to an item in the ImageList.  
			if (folderObject.Image16x16 != null)
			{
				treeNode.ImageKey = (string)folderObject.Image16x16.Tag;
				images.Add(folderObject.Image16x16);
			}

			// This is the image that is displayed when the object is selected.
			if (folderObject.SelectedImage16x16 != null)
			{
				treeNode.SelectedImageKey = (string)folderObject.SelectedImage16x16.Tag;
				images.Add(folderObject.SelectedImage16x16);
			}

			// And recursivley add the children.
			RecurseNodes(images, treeNode, objectRow);

			// This represents the node that was created above, and all the children found in the hierarchy.
			return treeNode;

		}

		/// <summary>
		/// Populates the Tree Control with data collected in the initialization thread.
		/// </summary>
		/// <param name="treeNode">Contains a hierarchical organization of objects.</param>
		private void UpdateTree(TreeNode treeNode, ArrayList images)
		{

			// Every new tree will have a set of images that are used to display next to the object name.  This will run through
			// that list and add any new images.
			foreach (Image image in images)
			{
				string key = (string)image.Tag;
				if (!this.treeView.ImageList.Images.ContainsKey(key))
					this.treeView.ImageList.Images.Add(key, image);
			}

			// This will recursively reconstruct the tree from the relational data gathered from the background thread.
			RecurseRefresh(this.treeView.Nodes, treeNode.Nodes);

		}

		/// <summary>
		/// Recursively updates the TreeView control with a new tree structure.
		/// </summary>
		/// <param name="targetNodeCollection">The target tree for the changes.</param>
		/// <param name="sourceNodeCollection">The source tree containing the new structure.</param>
		private void RecurseRefresh(TreeNodeCollection targetNodeCollection, TreeNodeCollection sourceNodeCollection)
		{

			// The main idea here is to run through all the nodes looking for Adds, Deletes and Updates.  When we find a Node that
			// shouldn't be in the structure, it's deleted from the tree.  Since the tree is made up of linked lists, we can't
			// rightly delete the current link in the list.  For this reason we need to use the index into the list for spanning
			// the structure instead of the collection operations.  When we find an element that needs to be removed, we can delete
			// it and the index will get us safely to the next element in the list. The first loop scans the list already in the
			// TreeView structure.
			for (int targetIndex = 0; targetIndex < targetNodeCollection.Count; targetIndex++)
			{

				// Get a reference to the current node in the target (older) list of nodes.
				TreeNode childTargetNode = (TreeNode)targetNodeCollection[targetIndex];

				// If we don't find the target (older) element in the source (newer) list, it will be deleted.
				bool found = false;

				// Cycle through all of the source (newer) elements looking for changes and removing any elements that
				// exist in both lists.
				for (int sourceIndex = 0; sourceIndex < sourceNodeCollection.Count; sourceIndex++)
				{

					// Get a reference to the current node in the source (newer) list of nodes.
					TreeNode childSourceNode = (TreeNode)sourceNodeCollection[sourceIndex];

					// If the elements are equal (as defined by the equality operator of the object), then recurse into the 
					// structure looking for changes to the children.  After that, check the Node for any changes since it was
					// added to the tree.
					if (childTargetNode.Tag.Equals(childSourceNode.Tag))
					{

						// Recurse down into the tree structures bringing all the children in sync with the new structure.
						RecurseRefresh(childTargetNode.Nodes, childSourceNode.Nodes);

						// Check the Nodes Name.  Update it if there's a change.  Note that checking the names before the copy
						// reduces the number of events that might be associated with updating a tree control.
						if (childTargetNode.Text != childSourceNode.Text)
							childTargetNode.Text = childSourceNode.Text;

						// At this point, we've checked all the children and applied any changes to the node.  Remove it from the
						// list.  Any elements left in the source list are assumed to be new members and will be added to the tree
						// structure.  That's why it's important to remove the ones already in the tree.
						sourceNodeCollection.Remove(childSourceNode);
						sourceIndex--;

						// This will signal the target loop that this element still exists in the structure.  If it isn't found,
						// it'll be deleted.
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

			// Any element that doesn't already exist in the target (older) tree, is copied from the source (newer) tree, along
			// with all it's children.
			for (int nodeIndex = 0; nodeIndex < sourceNodeCollection.Count; nodeIndex++)
			{
				TreeNode treeNode = (TreeNode)sourceNodeCollection[nodeIndex--];
				sourceNodeCollection.Remove(treeNode);
				targetNodeCollection.Add(treeNode);
			}

		}

		/// <summary>
		/// Invokes an Open Object event.
		/// </summary>
		/// <param name="sender">The object that originates the event.</param>
		/// <param name="objectArgs">The argument used to open the object.</param>
		public void OnOpenObject(object viewerObject)
		{

			// If there are any listerners, then multicast the request to open a viewer.
			if (this.isOpenEventAllowed && this.OpenObject != null)
				this.OpenObject(this, viewerObject);

		}

		/// <summary>
		/// Called when an object in the TreeView is selected.
		/// </summary>
		/// <param name="sender">The object that created the event.</param>
		/// <param name="e">Parameters specific to the event.</param>
		private void treeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{

			// Invoke the multicaster to open the objects.
			OnOpenObject(e.Node.Tag);

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
			// the possiblity here.  Call a worker thread to update the server data model with the new name.
			if (e.Label != null)
				ThreadPool.QueueUserWorkItem(new WaitCallback(RenameObject), new object[] { e.Node, e.Label });

			// The native action of the TreeView is to accept the new text as the name of the node.  This action will be inhibited
			// here and the name, if it successfully changed, will filter through the data model instead.
			e.CancelEdit = true;

		}

		/// <summary>
		/// Rename the object.
		/// </summary>
		/// <param name="parameters">The object to be renamed.</param>
		private void RenameObject(object parameter)
		{

			// Extract the thread arguments
			object[] parameters = (object[])parameter;
			TreeNode treeNode = (TreeNode)parameters[0];
			string name = (string)parameters[1];

			// Extract the object that is associated with the TreeView node.
			MarkThree.Guardian.Object commonObject = (MarkThree.Guardian.Object)treeNode.Tag;

			// This command batch is constructed below and sent to the server for execution.  Note that the batch is designed to
			// live beyond the block of code that locks the data model.  This is to prevent the data model from being locked while
			// a relatively long server database operation is underway.
			bool isBatchValid = true;
			Batch batch = new Batch();

			try
			{

				// Lock the table
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Find the object in the data model and make sure it still exists.
				ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(commonObject.ObjectId);
				if (objectRow == null)
					throw new Exception("This object has been deleted.");

				// Construct a command to rename the object.
				AssemblyPlan assembly = batch.Assemblies.Add("Core Service");
				TypePlan type = assembly.Types.Add("MarkThree.Guardian.Core.Object");
				TransactionPlan transaction = batch.Transactions.Add();
				MethodPlan method = transaction.Methods.Add(type, "Update");
				method.Parameters.Add(new InputParameter("rowVersion", objectRow.RowVersion));
				method.Parameters.Add(new InputParameter("objectId", objectRow.ObjectId));
				method.Parameters.Add(new InputParameter("name", name));

			}
			catch (Exception exception)
			{

				// This serves as an indication that the batch couldn't be constructed.
				isBatchValid = false;

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release table locks.
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// If the command batch was built successfully, then execute it.  If any part of it should fail, cancel the edit and
			// display the server errors.
			if (isBatchValid)
				try
				{

					// Call the web server to rename the object on the database.  Note that this method must be called when there 
					// are no locks to prevent deadlocking.  That is why it appears in it's own 'try/catch' block.
					ClientMarketData.Execute(batch);

				}
				catch (BatchException batchException)
				{

					// Display each error in the batch.
					foreach (Exception exception in batchException.Exceptions)
						Invoke(new MessageDelegate(ShowMessage), exception.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);

				}

		}

		/// <summary>
		/// Starts a drag-and-drop operation on the FolderList.
		/// </summary>
		/// <param name="sender">The control that is the source of the event.</param>
		/// <param name="e">Arguments associated with the event.</param>
		private void treeView_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e)
		{

			// Extract the parameters that are useful in describing and opening up an object.  This is the value that will be
			// passed around to other controls that can consume a drag-and-drop operation from the Folder List.
			DataObject dataObject = new DataObject(typeof(TreeNode).ToString(), e.Item);

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

			// This is the destination for the drag-and-drop operation.  If nothing is selected, that'll effectively end this
			// operation.
			TreeNode toNode = this.treeView.GetNodeAt(this.treeView.PointToClient(new Point(e.X, e.Y)));
			if (toNode == null)
				return;

			// If the object being dropped into this control isn't recognized (currently, only other nodes are handled), then end
			// the operation.
			TreeNode childNode = (TreeNode)e.Data.GetData(typeof(TreeNode).ToString());
			if (childNode == null)
				return;

			// This is the node from which the child is to be removed (if moving).
			TreeNode fromNode = childNode.Parent;

			// The drag-and-drop operation will require the data model to complete the reset of the processing, so pass the objects
			// that were selected using the mouse onto a background thread where the data model can be accessed.
			ThreadPool.QueueUserWorkItem(new WaitCallback(MoveChild), new object[] { toNode.Tag, fromNode.Tag, childNode.Tag });

		}

		/// <summary>
		/// Moves a child object from one parent to another.
		/// </summary>
		/// <param name="parameter">An array consiting of the target parent, the current parent and the child to be moved.</param>
		private void MoveChild(object parameter)
		{

			// Extract the objects selected by the drag-and-drop operation.
			object[] parameters = (object[])parameter;
			MarkThree.Guardian.Object toObject = (MarkThree.Guardian.Object)parameters[0];
			MarkThree.Guardian.Object fromObject = (MarkThree.Guardian.Object)parameters[1];
			MarkThree.Guardian.Object childObject = (MarkThree.Guardian.Object)parameters[2];

			try
			{

				// Lock the tables needed for this operation.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// It's critical that circular references aren't created, either by accident or design.  First, find the object
				// record associated with the destination node.
				ClientMarketData.ObjectRow parentRow = ClientMarketData.Object.FindByObjectId(toObject.ObjectId);
				if (parentRow == null)
					throw new Exception("This object has been deleted");

				// This is the object that is being dragged.  Find the row
				ClientMarketData.ObjectRow childRow = ClientMarketData.Object.FindByObjectId(childObject.ObjectId);
				if (childRow == null)
					throw new Exception("This object has been deleted");

				// This will remove the possibility of a circular relationship.
				if (MarkThree.Guardian.Relationship.IsChildObject(childRow, parentRow))
				{
					Invoke(new MessageDelegate(ShowMessage), Properties.Resources.CircularReferenceError, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release table locks.
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Any commands created below will be constructed in this object and sent to the server for execution.
			bool isBatchValid = true;
			Batch batch = new Batch();

			// If we made it here, the drag-and-drop is interpreted as a command to move a child from one parent to another.
			try
			{

				// Lock the tables needed for this operation.
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Extract the primary identifiers from the user interface nodes.
				// This is the object that is being dragged.  Find the row
				ClientMarketData.ObjectRow childRow = ClientMarketData.Object.FindByObjectId(childObject.ObjectId);
				if (childRow == null)
					throw new Exception("This object has been deleted");

				// Find the object in the data model and make sure it still exists.
				ClientMarketData.ObjectTreeRow objectTreeRow = null;
				foreach (ClientMarketData.ObjectTreeRow innerObjectTreeRow in childRow.GetObjectTreeRowsByObjectObjectTreeChildId())
					if (innerObjectTreeRow.ParentId == fromObject.ObjectId)
						objectTreeRow = innerObjectTreeRow;
				if (objectTreeRow == null)
					throw new Exception("This relationship has been deleted by someone else.");

				// Moving a child object from one parent to another must be accomplished as a transaction.  Otherwise, an
				// orhpan object will be created if the operation fails midway through.
				TransactionPlan transaction = batch.Transactions.Add();
				AssemblyPlan assembly = batch.Assemblies.Add("Core Service");
				TypePlan type = assembly.Types.Add("MarkThree.Guardian.Core.ObjectTree");

				// Construct a command delete the old parent relation.
				MethodPlan deleteObjectTree = transaction.Methods.Add(type, "Update");
				deleteObjectTree.Parameters.Add(new InputParameter("objectTreeId", objectTreeRow.ObjectTreeId));
				deleteObjectTree.Parameters.Add(new InputParameter("parentId", toObject.ObjectId));
				deleteObjectTree.Parameters.Add(new InputParameter("childId", childObject.ObjectId));
				deleteObjectTree.Parameters.Add(new InputParameter("rowVersion", objectTreeRow.RowVersion));

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

				// This indicates that the batch shouldn't be executed.
				isBatchValid = false;

			}
			finally
			{

				// Release table locks.
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectTreeLock.IsReaderLockHeld)
					ClientMarketData.ObjectTreeLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

			// If the command batch was built successfully, then execute it.
			if (isBatchValid)
				try
				{

					// Call the web server to rename the object on the database.  Note that this method must be called when there are
					// no locks to prevent deadlocking.  That is why it appears in it's own 'try/catch' block.
					ClientMarketData.Execute(batch);

				}
				catch (BatchException batchException)
				{

					// Display each error in the batch.
					foreach (Exception exception in batchException.Exceptions)
						MessageBox.Show(exception.Message, "Guardian Error");

				}

		}

		/// <summary>
		/// Provides feedback when the cursor is dragged over objects in the Folder List.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">Event argument.</param>
		private void treeView_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{

			// A 'TreeNode' object is the only kind of drag item currently supported.
			bool acceptable_format = false;
			foreach (String format in e.Data.GetFormats(true))
				if (format == typeof(TreeNode).ToString())
					acceptable_format = true;

			// Determine whether the cursor is over an item that can be a destination for a drag-and-drop operation.
			TreeNode toNode = (TreeNode)this.treeView.GetNodeAt(this.treeView.PointToClient(new Point(e.X, e.Y)));

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
				OnOpenObject(highlightNode.Tag);

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
				FolderList.highlightNode = this.treeView.GetNodeAt(new Point(e.X, e.Y));

		}

		/// <summary>
		/// Handles the properties of the objects in the Object Tree.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">Event argument.</param>
		private void menuItemProperties_Click(object sender, System.EventArgs e)
		{

			// Only invoke the properties if the node is highlighted.
			if (FolderList.highlightNode == null)
				return;

			MarkThree.Guardian.Object guardianObject = (MarkThree.Guardian.Object)FolderList.highlightNode.Tag;
			guardianObject.ShowProperties();


		}

	}

}
