namespace MarkThree.Forms
{

	/// <summary>
	/// A common root for all attributes of a ViewerStyle.
	/// </summary>
	abstract public class ViewerAttribute
	{

		/// <summary>
		/// Make a copy of the attribute.
		/// </summary>
		/// <returns>A copy of the attribute.</returns>
		abstract public ViewerAttribute Clone();
	
	}

}
