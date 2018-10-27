namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class AnimationSequence
	{

		// Public Members
		public MarkThree.Forms.Tile Tile;
		public System.Int32 StyleIndex;
		public System.String[] StyleArray;

		public AnimationSequence(Tile tile, string[] styleArray)
		{

			// Initialize the object
			this.Tile = tile;
			this.StyleIndex = 0;
			this.StyleArray = styleArray;

		}

	}

}
