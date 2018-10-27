namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Drawing;

	/// <summary>
	/// Summary description for Broker.
	/// </summary>
	public class Broker : MarkThree.Guardian.Blotter
	{

		// Static Members
		private static MarkThree.Guardian.FormBrokerProperties formBrokerProperties;
		private static System.Drawing.Image image16x16;
		private static System.Drawing.Image selectedImage16x16;
		private static System.Drawing.Image image32x32;
		private static System.Drawing.Image selectedImage32x32;

		static Broker()
		{

			// These images are used to display the object in TreeViews, Properties Pages, etc.
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			// 16x16 Image			
			Broker.image16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Broker 16x16.png"));
			Broker.image16x16.Tag = string.Format("{0}.{1}", typeof(Broker).FullName, "16x16");

			// 16x16 SelectedImage
			Broker.selectedImage16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Broker 16x16.png"));
			Broker.selectedImage16x16.Tag = string.Format("{0}.{1}.{2}", typeof(Broker).FullName, "16x16", "Selected");

			// 32x23 Image
			Broker.image32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Broker 32x32.png"));
			Broker.image32x32.Tag = string.Format("{0}.{1}", typeof(Broker).FullName, "32x32");

			// 32x32 SelectedImage
			Broker.selectedImage32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Broker 32x32.png"));
			Broker.selectedImage32x32.Tag = string.Format("{0}.{1}.{2}", typeof(Broker).FullName, "16x16", "Selected");

			// Create the property pages used to manage the data on this object.
			Broker.formBrokerProperties = new FormBrokerProperties();

		}

		public Broker(int objectId) : base(objectId) {}

		// Primary Identifier of this object.
		public int BrokerId {get {return this.BlotterId;}}

		/// <summary>The small, unselected image for this object.</summary>
		public override Image Image16x16 {get {return Broker.image16x16;}}

		/// <summary>The small, selected image for this object.</summary>
		public override Image SelectedImage16x16 {get {return Broker.selectedImage16x16;}}

		/// <summary>The medium, unselected image for this object.</summary>
		public override Image Image32x32 {get {return Broker.image32x32;}}

		/// <summary>The medium, selected image for this object.</summary>
		public override Image SelectedImage32x32 {get {return Broker.selectedImage32x32;}}

		/// <summary>
		/// Show the property pages.
		/// </summary>
		public override void ShowProperties()
		{

			// Show the properties and hide them when they are dismissed.  The properties dialog box is otherwise self-contained.
			Broker.formBrokerProperties.Show(this);
			Broker.formBrokerProperties.Hide();
		
		}

	}

}
