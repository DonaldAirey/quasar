namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Drawing;

	/// <summary>
	/// Summary description for Blotter.
	/// </summary>
	public class Blotter : MarkThree.Guardian.Object
	{

		// Public Read-Only Members
		public readonly System.Boolean IsWorkingOrderStylesheetIdNull;
		public readonly System.Boolean IsDestinationOrderStylesheetIdNull;
		public readonly System.Boolean IsMatchesStylesheetIdNull;
		public readonly System.Boolean IsAdvertisementStylesheetIdNull;
		public readonly System.Int32 WorkingOrderStylesheetId;
		public readonly System.Int32 DestinationOrderStylesheetId;
		public readonly System.Int32 MatchesStylesheetId;
		public readonly System.Int32 AdvertisementStylesheetId;

		// Static Members
		private static MarkThree.Guardian.FormBlotterProperties formBlotterProperties;
		private static System.Drawing.Bitmap image16x16;
		private static System.Drawing.Bitmap selectedImage16x16;
		private static System.Drawing.Bitmap image32x32;
		private static System.Drawing.Bitmap selectedImage32x32;

		/// <summary>
		/// A working surface for trading.
		/// </summary>
		static Blotter()
		{

			// These images are used to display the object in TreeViews, Properties Pages, etc.
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			// 16x16 Image			
			Blotter.image16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Blotter 16x16.png"));
			Blotter.image16x16.Tag = string.Format("{0}.{1}", typeof(Blotter).FullName, "16x16");

			// 16x16 SelectedImage
			Blotter.selectedImage16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Blotter 16x16.png"));
			Blotter.selectedImage16x16.Tag = string.Format("{0}.{1}.{2}", typeof(Blotter).FullName, "16x16", "Selected");

			// 32x23 Image
			Blotter.image32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Blotter 32x32.png"));
			Blotter.image32x32.Tag = string.Format("{0}.{1}", typeof(Blotter).FullName, "32x32");

			// 32x32 SelectedImage
			Blotter.selectedImage32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Blotter 32x32.png"));
			Blotter.selectedImage32x32.Tag = string.Format("{0}.{1}.{2}", typeof(Blotter).FullName, "32x32", "Selected");

			// Create the property pages used to manage the data on this object.
			Blotter.formBlotterProperties = new FormBlotterProperties();

		}
		
		public Blotter(int objectId) : base(objectId)
		{

			// Initialize the object
			ClientMarketData.BlotterRow blotterRow = ClientMarketData.Blotter.FindByBlotterId(objectId);
			if (!(this.IsWorkingOrderStylesheetIdNull = blotterRow.IsWorkingOrderStylesheetIdNull()))
				this.WorkingOrderStylesheetId = blotterRow.WorkingOrderStylesheetId;
			if (!(this.IsDestinationOrderStylesheetIdNull = blotterRow.IsDestinationOrderStylesheetIdNull()))
				this.DestinationOrderStylesheetId = blotterRow.DestinationOrderStylesheetId;
			if (!(this.IsMatchesStylesheetIdNull = blotterRow.IsMatchStylesheetIdNull()))
				this.MatchesStylesheetId = blotterRow.MatchStylesheetId;
			if (!(this.IsAdvertisementStylesheetIdNull = blotterRow.IsAdvertisementStylesheetIdNull()))
				this.AdvertisementStylesheetId = blotterRow.AdvertisementStylesheetId;

		}

		/// <summary>The Primary Identifier of this object.</summary>
		public int BlotterId {get {return this.ObjectId;}}

		/// <summary>The small, unselected image for this object.</summary>
		public override Image Image16x16 {get {return Blotter.image16x16;}}

		/// <summary>The small, selected image for this object.</summary>
		public override Image SelectedImage16x16 {get {return Blotter.selectedImage16x16;}}

		/// <summary>The medium, unselected image for this object.</summary>
		public override Image Image32x32 {get {return Blotter.image32x32;}}

		/// <summary>The medium, selected image for this object.</summary>
		public override Image SelectedImage32x32 {get {return Blotter.selectedImage32x32;}}

		/// <summary>
		/// Show the property pages.
		/// </summary>
		public override void ShowProperties()
		{

			// Show the properties and hide them when they are dismissed.  The properties dialog box is otherwise self-contained.
			Blotter.formBlotterProperties.Show(this);
			Blotter.formBlotterProperties.Hide();
		
		}

	}

}
