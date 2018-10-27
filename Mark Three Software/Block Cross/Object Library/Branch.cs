namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Drawing;

	/// <summary>
	/// Summary description for Branch.
	/// </summary>
	public class Branch : MarkThree.Guardian.Blotter
	{

		// Static Members
		private static MarkThree.Guardian.FormBranchProperties formBranchProperties;
		private static System.Drawing.Image image16x16;
		private static System.Drawing.Image selectedImage16x16;
		private static System.Drawing.Image image32x32;
		private static System.Drawing.Image selectedImage32x32;

		static Branch()
		{

			// These images are used to display the object in TreeViews, Properties Pages, etc.
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			// 16x16 Image			
			Branch.image16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Branch 16x16.png"));
			Branch.image16x16.Tag = string.Format("{0}.{1}", typeof(Branch).FullName, "16x16");

			// 16x16 SelectedImage
			Branch.selectedImage16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Branch 16x16.png"));
			Branch.selectedImage16x16.Tag = string.Format("{0}.{1}.{2}", typeof(Branch).FullName, "16x16", "Selected");

			// 32x23 Image
			Branch.image32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Branch 32x32.png"));
			Branch.image32x32.Tag = string.Format("{0}.{1}", typeof(Branch).FullName, "32x32");

			// 32x32 SelectedImage
			Branch.selectedImage32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Branch 32x32.png"));
			Branch.selectedImage32x32.Tag = string.Format("{0}.{1}.{2}", typeof(Branch).FullName, "32x32", "Selected");

			// Create the property pages used to manage the data on this object.
			Branch.formBranchProperties = new FormBranchProperties();

		}

		public Branch(int branchId) : base(branchId) {}

		public int BranchId {get {return this.ObjectId;}}

		public override Image Image16x16 {get {return Branch.image16x16;}}

		public override Image SelectedImage16x16 {get {return Branch.selectedImage16x16;}}

		public override Image Image32x32 {get {return Branch.image32x32;}}

		public override Image SelectedImage32x32 {get {return Branch.selectedImage32x32;}}

		/// <summary>
		/// Show the property pages.
		/// </summary>
		public override void ShowProperties()
		{

			// Show the properties and hide them when they are dismissed.  The properties dialog box is otherwise self-contained.
			Branch.formBranchProperties.Show(this);
			Branch.formBranchProperties.Hide();
		
		}

	}

}
