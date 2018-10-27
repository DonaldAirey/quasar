namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Drawing;

	/// <summary>A large company that contains a relationship (clearing, brokerage) with other companies.</summary>
	public class ClearingBroker : MarkThree.Guardian.Blotter
	{

		// Static Members
		private static MarkThree.Guardian.FormClearingBrokerProperties formClearingBrokerProperties;
		private static System.Drawing.Image image16x16;
		private static System.Drawing.Image selectedImage16x16;
		private static System.Drawing.Image image32x32;
		private static System.Drawing.Image selectedImage32x32;

		static ClearingBroker()
		{

			// These images are used to display the object in TreeViews, Properties Pages, etc.
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			// 16x16 Image			
			ClearingBroker.image16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.ClearingBroker 16x16.png"));
			ClearingBroker.image16x16.Tag = string.Format("{0}.{1}", typeof(ClearingBroker).FullName, "16x16");

			// 16x16 SelectedImage
			ClearingBroker.selectedImage16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.ClearingBroker 16x16.png"));
			ClearingBroker.selectedImage16x16.Tag = string.Format("{0}.{1}.{2}", typeof(ClearingBroker).FullName, "16x16", "Selected");

			// 32x23 Image
			ClearingBroker.image32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.ClearingBroker 32x32.png"));
			ClearingBroker.image32x32.Tag = string.Format("{0}.{1}", typeof(ClearingBroker).FullName, "32x32");

			// 32x32 SelectedImage
			ClearingBroker.selectedImage32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.ClearingBroker 32x32.png"));
			ClearingBroker.selectedImage32x32.Tag = string.Format("{0}.{1}.{2}", typeof(ClearingBroker).FullName, "32x32", "Selected");

			// Create the property pages used to manage the data on this object.
			ClearingBroker.formClearingBrokerProperties = new FormClearingBrokerProperties();

		}
		
		/// <summary>
		/// A large company that contains a relationship (clearing, brokerage) with other companies.
		/// </summary>
		/// <param name="objectId">The primary key of the object.</param>
		public ClearingBroker(int objectId) : base(objectId) {}

		// Primary Identifier of this object.
		public int ClearingBrokerId {get {return this.BlotterId;}}

		/// <summary>The small, unselected image for this object.</summary>
		public override Image Image16x16 {get {return ClearingBroker.image16x16;}}

		/// <summary>The small, selected image for this object.</summary>
		public override Image SelectedImage16x16 {get {return ClearingBroker.selectedImage16x16;}}

		/// <summary>The medium, unselected image for this object.</summary>
		public override Image Image32x32 {get {return ClearingBroker.image32x32;}}

		/// <summary>The medium, selected image for this object.</summary>
		public override Image SelectedImage32x32 {get {return ClearingBroker.selectedImage32x32;}}

		/// <summary>
		/// Show the property pages.
		/// </summary>
		public override void ShowProperties()
		{

			// Show the properties and hide them when they are dismissed.  The properties dialog box is otherwise self-contained.
			ClearingBroker.formClearingBrokerProperties.Show(this);
			ClearingBroker.formClearingBrokerProperties.Hide();
		
		}

	}

}
