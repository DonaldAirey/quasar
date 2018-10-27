namespace MarkThree
{

	using System;
	using System.Collections;

	/// <summary>
	/// A table of viewer information that can be accessed using the object type with which the viewer is associated.
	/// </summary>
	public class ViewerSection
	{

		// Private Members
		private ArrayList arrayList;

		public ViewerSection() {this.arrayList = new ArrayList();}

		/// <summary>
		/// Adds a ViewerInfo item to the list.
		/// </summary>
		/// <param name="key">The name of the persistent store.</param>
		/// <param name="persistentStoreInfo">The data related to managing a persistent store.</param>
		public ViewerInfo Add(ViewerInfo viewerInfo) {this.arrayList.Add(viewerInfo); return viewerInfo;}

		public IEnumerator GetEnumerator() {return this.arrayList.GetEnumerator();}
		
	}

}
