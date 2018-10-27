namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Drawing;

	/// <summary>A large company that contains a relationship (clearing, brokerage) with other companies.</summary>
	public class Institution : MarkThree.Guardian.Blotter
	{

		// Static Members
		private static MarkThree.Guardian.FormInstitutionProperties formInstitutionProperties;
		private static System.Drawing.Image image16x16;
		private static System.Drawing.Image selectedImage16x16;
		private static System.Drawing.Image image32x32;
		private static System.Drawing.Image selectedImage32x32;

		static Institution()
		{

			// The images use to identify this object to the user are loaded from the resources in this assembly.
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			// 16x16 Image			
			Institution.image16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Institution 16x16.png"));
			Institution.image16x16.Tag = string.Format("{0}.{1}", typeof(Institution).FullName, "16x16");

			// 16x16 SelectedImage
			Institution.selectedImage16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Institution 16x16.png"));
			Institution.selectedImage16x16.Tag = string.Format("{0}.{1}.{2}", typeof(Institution).FullName, "16x16", "Selected");

			// 32x23 Image
			Institution.image32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Institution 32x32.png"));
			Institution.image32x32.Tag = string.Format("{0}.{1}", typeof(Institution).FullName, "32x32");

			// 32x32 SelectedImage
			Institution.selectedImage32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Institution 32x32.png"));
			Institution.selectedImage32x32.Tag = string.Format("{0}.{1}.{2}", typeof(Institution).FullName, "32x32", "Selected");

			// Create the property pages used to manage the data on this object.
			Institution.formInstitutionProperties = new FormInstitutionProperties();

		}
		
		/// <summary>
		/// A large company that contains a relationship (clearing, brokerage) with other companies.
		/// </summary>
		/// <param name="objectId">The primary key of the object.</param>
		public Institution(int objectId) : base(objectId) {}

		// Primary Identifier of this object.
		public int InstitutionId {get {return this.BlotterId;}}

		/// <summary>The small, unselected image for this object.</summary>
		public override Image Image16x16 {get {return Institution.image16x16;}}

		/// <summary>The small, selected image for this object.</summary>
		public override Image SelectedImage16x16 {get {return Institution.selectedImage16x16;}}

		/// <summary>The medium, unselected image for this object.</summary>
		public override Image Image32x32 {get {return Institution.image32x32;}}

		/// <summary>The medium, selected image for this object.</summary>
		public override Image SelectedImage32x32 {get {return Institution.selectedImage32x32;}}

		/// <summary>
		/// Show the property pages.
		/// </summary>
		public override void ShowProperties()
		{

			// Show the properties and hide them when they are dismissed.  The properties dialog box is otherwise self-contained.
			Institution.formInstitutionProperties.Show(this);
			Institution.formInstitutionProperties.Hide();
		
		}

	}

}
