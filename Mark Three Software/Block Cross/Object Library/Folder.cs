namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Drawing;

	/// <summary>
	/// Summary description for Folder.
	/// </summary>
	public class Folder : MarkThree.Guardian.Object
	{

		// Static Members
		private static System.Drawing.Image image16x16;
		private static System.Drawing.Image selectedImage16x16;
		private static System.Drawing.Image image32x32;
		private static System.Drawing.Image selectedImage32x32;

		static Folder()
		{

			// These images are used to display the object in TreeViews, Properties Pages, etc.
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			// 16x16 Image			
			Folder.image16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Folder 16x16.bmp"));
			Folder.image16x16.Tag = string.Format("{0}.{1}", typeof(Folder).FullName, "16x16");

			// 16x16 SelectedImage
			Folder.selectedImage16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Folder 16x16.bmp"));
			Folder.selectedImage16x16.Tag = string.Format("{0}.{1}.{2}", typeof(Folder).FullName, "16x16", "Selected");

			// 32x23 Image
			Folder.image32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Folder 32x32.bmp"));
			Folder.image32x32.Tag = string.Format("{0}.{1}", typeof(Folder).FullName, "32x32");

			// 32x32 SelectedImage
			Folder.selectedImage32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Folder 32x32.bmp"));
			Folder.selectedImage32x32.Tag = string.Format("{0}.{1}.{2}", typeof(Folder).FullName, "32x32", "Selected");

		}
	
		/// <summary>
		/// A Folder for holding other objects.
		/// </summary>
		/// <param name="objectId">The primary key of the object.</param>
		public Folder(int objectId) : base(objectId) {}

		/// <summary>The small, unselected image for this object.</summary>
		public override Image Image16x16 {get {return Folder.image16x16;}}

		/// <summary>The small, selected image for this object.</summary>
		public override Image SelectedImage16x16 {get {return Folder.selectedImage16x16;}}

		/// <summary>The medium, unselected image for this object.</summary>
		public override Image Image32x32 {get {return Folder.image32x32;}}

		/// <summary>The medium, selected image for this object.</summary>
		public override Image SelectedImage32x32 {get {return Folder.selectedImage32x32;}}

	}

}
