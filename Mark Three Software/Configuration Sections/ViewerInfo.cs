namespace MarkThree
{

	using System;
	using System.Collections;

	/// <summary>
	/// Information about a viewer from the configuration file.  This contains information about what class of objects
	/// the viewer is associated with, the name of the assembly where the viewer is found, and the Type of the viewer.
	/// </summary>
	public class ViewerInfo
	{

		// Private Members
		public readonly Type Type;
		public readonly Type ViewerType;

		/// <summary>
		/// Initializes information about a viewer.
		/// </summary>
		/// <param name="objectType">The object with which this viewer is associated.</param>
		/// <param name="assemblyName">The name of the assembly where the viewer is found.</param>
		/// <param name="TypeName">The Type of the viewer.</param>
		public ViewerInfo(Type objectType, Type viewerType)
		{

			// Initialize the object.
			this.Type = objectType;
			this.ViewerType = viewerType;

		}

	}
	
}
