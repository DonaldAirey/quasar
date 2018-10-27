/************************************************************************************************************************
*
*	File:			ObjectNode.cs
*	Description:	This is the class used to manage the TreeNodes of the FolderList TreeView control.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Common.Controls
{

	using Shadows.Quasar.Client;
	using Shadows.Quasar.Common;
	using System;
	using System.Windows.Forms;

	/// <summary>
	/// Represents a node in a hierarchial organization of Quasar objects.
	/// </summary>
	class ObjectNode : TreeNode
	{

		// These mappings are used to map the object's category field to an image index in the ImageList.
		private static int []unselectedAccountIndex = {2, 3, 4};
		private static int []selectedAccountIndex = {2, 3, 4};
		private static int []unselectedBlotterIndex = {5, 6};
		private static int []selectedBlotterIndex = {5, 6};
		private static int []unselectedFolderIndex = {0, 1};
		private static int []selectedFolderIndex = {0, 1};
		private static int []unselectedModelIndex = {7, 7};
		private static int []selectedModelIndex = {7, 7};

		// Class Members
		private int objectId;
		private int objectTypeCode;
		private string name;

		// Public access to members
		public int ObjectId {get {return this.objectId;}}
		public int ObjectTypeCode {get {return this.objectTypeCode;}}
		public string Name {get {return this.name;}}

		/// <summary>
		/// Initializes a new instance of the ObjectNode class.
		/// </summary>
		public ObjectNode() : base() {}

		/// <summary>
		/// Initializes a new instance of the ObjectNode class.
		/// </summary>
		public ObjectNode(ClientMarketData.ObjectRow objectRow) :
			base(objectRow.Name, UnselectedIndex(objectRow), SelectedIndex(objectRow))
		{

			// Add the attributes to the parent node.
			this.objectId = objectRow.ObjectId;
			this.objectTypeCode = objectRow.ObjectTypeCode;
			this.name = objectRow.Name;

			// And recursivley add the children.
			RecurseNodes(this, objectRow);
		
		}

		/// <summary>
		/// Construct a hierarchical tree structure by recursively scanning the parent-child relations.
		/// </summary>
		/// <param name="objectNode">The current node in the tree structure.</param>
		private void RecurseNodes(ObjectNode objectNode, ClientMarketData.ObjectRow objectRow)
		{

			// Add all the children of this object to the tree.  This, in turn, will add all the children of the children, and so
			// on until we reach the end of the tree.  Simple, elegant, powerful.
			foreach (ClientMarketData.ObjectTreeRow childRelationRow in
				objectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
				objectNode.Nodes.Add(new ObjectNode(childRelationRow.ObjectRowByFKObjectObjectTreeChildId));

		}

		/// <summary>
		/// Provides a mapping between the document type and the typeCode of the selected image for the node.
		/// </summary>
		/// <param name="typeCode">Identifies what kind of Quasar object the node prepresents.</param>
		/// <returns></returns>
		private static int UnselectedIndex(ClientMarketData.ObjectRow objectRow)
		{

			switch (objectRow.ObjectTypeCode)
			{

			case ObjectType.Account:

				foreach (ClientMarketData.AccountRow accountRow in objectRow.GetAccountRows())
					return unselectedAccountIndex[accountRow.AccountTypeCode];
				break;

			case ObjectType.Blotter:

				foreach (ClientMarketData.BlotterRow blotterRow in objectRow.GetBlotterRows())
					return unselectedBlotterIndex[blotterRow.BlotterTypeCode];
				break;

			case ObjectType.Folder:

				foreach (ClientMarketData.FolderRow folderRow in objectRow.GetFolderRows())
					return unselectedFolderIndex[folderRow.FolderTypeCode];
				break;

			case ObjectType.Model:

				foreach (ClientMarketData.ModelRow modelRow in objectRow.GetModelRows())
					return unselectedModelIndex[modelRow.ModelTypeCode];
				break;

			}
			
			// This catches any generic types and maps them to the first bitmap as a default.
			return 0;

		}

		/// <summary>
		/// Provides a mapping between the document type and the typeCode of the unselected image for the node.
		/// </summary>
		/// <param name="typeCode">Identifies what kind of Quasar object the node prepresents.</param>
		/// <returns></returns>
		private static int SelectedIndex(ClientMarketData.ObjectRow objectRow)
		{

			switch (objectRow.ObjectTypeCode)
			{

			case ObjectType.Account:

				foreach (ClientMarketData.AccountRow accountRow in objectRow.GetAccountRows())
					return selectedAccountIndex[accountRow.AccountTypeCode];
				break;

			case ObjectType.Blotter:

				foreach (ClientMarketData.BlotterRow blotterRow in objectRow.GetBlotterRows())
					return selectedBlotterIndex[blotterRow.BlotterTypeCode];
				break;

			case ObjectType.Folder:

				foreach (ClientMarketData.FolderRow folderRow in objectRow.GetFolderRows())
					return selectedFolderIndex[folderRow.FolderTypeCode];
				break;

			case ObjectType.Model:

				foreach (ClientMarketData.ModelRow modelRow in objectRow.GetModelRows())
					return selectedModelIndex[modelRow.ModelTypeCode];
				break;

			}
			
			// This catches any generic types and maps them to the first bitmap as a default.
			return 0;


		}

	}

}
