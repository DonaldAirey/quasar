namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System.Reflection;
	using System;
	using System.Drawing;

	/// <summary>
	/// Summary description for Object.
	/// </summary>
	public class Object
	{

		// Public Read-Only Members
		public readonly int ObjectId;
		public readonly string Name;
		public readonly string Description;
		public readonly System.Boolean ReadOnly;
		public readonly System.Boolean Hidden;

		// Static Members
		private static System.Drawing.Image image16x16;
		private static System.Drawing.Image selectedImage16x16;
		private static System.Drawing.Image image32x32;
		private static System.Drawing.Image selectedImage32x32;

		static Object()
		{

			// The images use to identify this object to the user are loaded from the resources in this assembly.
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			// 16x16 Image			
			MarkThree.Guardian.Object.image16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Object 16x16.png"));
			MarkThree.Guardian.Object.image16x16.Tag = string.Format("{0}.{1}", typeof(MarkThree.Guardian.Object).FullName, "16x16");

			// 16x16 SelectedImage
			MarkThree.Guardian.Object.selectedImage16x16 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Object 16x16.png"));
			MarkThree.Guardian.Object.selectedImage16x16.Tag = string.Format("{0}.{1}.{2}", typeof(MarkThree.Guardian.Object).FullName, "16x16", "Selected");

			// 32x23 Image
			MarkThree.Guardian.Object.image32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Object 32x32.png"));
			MarkThree.Guardian.Object.image32x32.Tag = string.Format("{0}.{1}", typeof(MarkThree.Guardian.Object).FullName, "32x32");

			// 32x32 SelectedImage
			MarkThree.Guardian.Object.selectedImage32x32 =
				new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Object 32x32.png"));
			MarkThree.Guardian.Object.selectedImage32x32.Tag = string.Format("{0}.{1}.{2}", typeof(MarkThree.Guardian.Object).FullName, "32x32", "Selected");

		}

		public Object(int objectId)
		{

			// Initialize the object
			ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(objectId);
			this.ObjectId = objectRow.ObjectId;
			this.Name = objectRow.Name;
			this.Description = objectRow.IsDescriptionNull() ? string.Empty : objectRow.Description;
			this.ReadOnly = objectRow.ReadOnly;
			this.Hidden = objectRow.Hidden;

		}

		/// <summary>
		/// Creates an object from the object identifier.  This method assumes that the tables are locked.
		/// </summary>
		/// <param name="objectId">The unique object identifier.</param>
		/// <returns>One of the objects recognized by the Guaridan application.</returns>
		public static MarkThree.Guardian.Object CreateObject(int objectId)
		{

			// If the object record doesn't exist, then the record can't be created.  This is likely due to the fact that the data
			// model hasn't been loaded from the server at this time.
			ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(objectId);
			if (objectRow == null)
				return null;

			// This will use the data in the object record to build a call to the constructor for that object.
			TypeSpecification typeSpecification = new TypeSpecification(objectRow.TypeRow.Specification);
			Assembly assembly = Assembly.Load(typeSpecification.AssemblyName);
			return (MarkThree.Guardian.Object)assembly.CreateInstance(typeSpecification.TypeName, false,
				BindingFlags.CreateInstance, null, new object[] { objectRow.ObjectId },
				System.Globalization.CultureInfo.CurrentCulture, null);

		}

		public virtual MarkThree.Guardian.Object Create(int objectId) { return new Object(objectId); }

		public override string ToString() {return this.Name;}

		public virtual Image Image16x16 {get {return MarkThree.Guardian.Object.image16x16;}}

		public virtual Image SelectedImage16x16 {get {return MarkThree.Guardian.Object.selectedImage16x16;}}

		public virtual Image Image32x32 {get {return MarkThree.Guardian.Object.image32x32;}}

		public virtual Image SelectedImage32x32 {get {return MarkThree.Guardian.Object.selectedImage32x32;}}

		public override int GetHashCode()
		{
			return this.ObjectId.GetHashCode();
		}

		public override bool Equals(object obj)
		{

			if (typeof(MarkThree.Guardian.Object).IsAssignableFrom(obj.GetType()))
			{
				MarkThree.Guardian.Object markThreeObject = (MarkThree.Guardian.Object)obj;
				return this.ObjectId.Equals(markThreeObject.ObjectId);
			}

			return this.ObjectId.Equals(obj);

		}

		public static bool operator == (MarkThree.Guardian.Object object1, object object2)
		{

			return object1.Equals(object2);

		}

		public static bool operator != (MarkThree.Guardian.Object object1, object object2)
		{

			return !object1.Equals(object2);

		}

		public virtual void ShowProperties() {}

	}

}
