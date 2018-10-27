namespace MarkThree.Guardian.Forms
{

	using MarkThree.Guardian;
	using System;
	using System.Windows.Forms;

	/// <summary>
	/// Represents an object that can be selected in the Guardian Bar.
	/// </summary>
	class GuardianViewItem : ListViewItem
	{

		private MarkThree.Guardian.Object guardianObject;

		public MarkThree.Guardian.Object Object { get { return this.guardianObject; } }

		/// <summary>
		/// Initializes a new instance of the GuardianViewItem class.
		/// </summary>
		public GuardianViewItem(MarkThree.Guardian.Object guardianObject, int imageIndex) :
			base(guardianObject.Name, imageIndex)
		{

			// Initialize the class members.
			this.guardianObject = guardianObject;

		}

		public override string ToString()
		{

			string[] assemblyParts = this.guardianObject.GetType().Assembly.FullName.Split(',');
			return string.Format("{{ObjectId={0}}}", this.guardianObject.ObjectId);

		}

	}

}
