namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Configuration;
	using System.Drawing;
	using System.IO;
	using System.Reflection;
	using System.Threading;

	/// <summary>
	/// Summary description for SystemFolder.
	/// </summary>
	public class SystemFolder : MarkThree.Guardian.Folder
	{

		// Public Read-Only Members
		public readonly System.String Url;

		// Static Members
		private static System.Drawing.Image image16x16;
		private static System.Drawing.Image selectedImage16x16;
		private static System.Drawing.Image image32x32;
		private static System.Drawing.Image selectedImage32x32;

		public SystemFolder(int objectId) : base(objectId)
		{

			ClientMarketData.SystemFolderRow systemFolderRow = ClientMarketData.SystemFolder.FindBySystemFolderId(objectId);

			// These images are used to display the object in TreeViews, Properties Pages, etc.
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			// 16x16 Image			
			SystemFolder.image16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.SystemFolder 16x16.png"));
			SystemFolder.image16x16.Tag = string.Format("{0}.{1}", typeof(SystemFolder).FullName, "16x16");

			// 16x16 SelectedImage
			SystemFolder.selectedImage16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.SystemFolder 16x16.png"));
			SystemFolder.selectedImage16x16.Tag = string.Format("{0}.{1}.{2}", typeof(SystemFolder).FullName, "16x16", "Selected");

			// 32x23 Image
			SystemFolder.image32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.SystemFolder 32x32.png"));
			SystemFolder.image32x32.Tag = string.Format("{0}.{1}", typeof(SystemFolder).FullName, "32x32");

			// 32x32 SelectedImage
			SystemFolder.selectedImage32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.SystemFolder 32x32.png"));
			SystemFolder.selectedImage32x32.Tag = string.Format("{0}.{1}.{2}", typeof(SystemFolder).FullName, "32x32", "Selected");

			// Start the application off by opening up the start page.
			string onOpenText = ConfigurationManager.AppSettings["OnOpen"];
			if (onOpenText != null)
			{

				string[] onOpenArguments = onOpenText.Split(new char[] {','});
				this.Url = onOpenArguments[2].Trim();

			}

		}

		/// <summary>The small, unselected image for this object.</summary>
		public override Image Image16x16 {get {return SystemFolder.image16x16;}}

		/// <summary>The small, selected image for this object.</summary>
		public override Image SelectedImage16x16 {get {return SystemFolder.selectedImage16x16;}}

		/// <summary>The medium, unselected image for this object.</summary>
		public override Image Image32x32 {get {return SystemFolder.image32x32;}}

		/// <summary>The medium, selected image for this object.</summary>
		public override Image SelectedImage32x32 {get {return SystemFolder.selectedImage32x32;}}

	}

}
