namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	/// <summary>
	/// Provides an animation effect that hightlights the foreground of a tile before fading to the original color.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class FadeAnimation : Animation
	{

		// Private Constants
		private const int up = 0;
		private const int same = 1;
		private const int down = 2;

		// Private Members
		private bool isSameSpecified;
		private System.String[][] styles;
		private readonly int[] directions = new int[] {up, same, down};

		/// <summary>
		/// Creates the effect used to highlight the foreground of a tile and gradually fade to the original color.
		/// </summary>
		/// <param name="styleTable">The table of styles used by a document viewer.</param>
		/// <param name="parentStyle">The original style for this effect.</param>
		/// <param name="viewerFadeAnimation">The parameters of the fade effect.</param>
		public FadeAnimation(Dictionary<string, Style> styleTable, Style parentStyle, ViewerFadeAnimation viewerFadeAnimation)
		{

			// When there is no color change requested for the absense of a transition from one data value to the next, a special
			// case is made to reset the existing animation sequence.  For example, if a price changes from 10.00 to 9.00, it will
			// initiate a 'down' animation sequence.  If another value of 9.0 is handled before the original fade is finished,
			// there is a problem with what should be displayed.  Switching to the 'Same' sequence could possibly leave a user with
			// less information in a ticker type of display.  If blue was used for the 'Same' sequence, then the user would see a
			// blue price fade to the original color, but it doesn't really tell them anything.  To increase the practical
			// information on the screen, when the 'Same' sequence isn't specified (that is, it is the default color), then the
			// existing sequence will be reset if it is active.
			this.isSameSpecified = viewerFadeAnimation.Same == DefaultDocument.ForeColor;

			// Initialize the object.  Three arrays are used for the fade from a change downward, no change or a change upward. The
			// direction is defined by the IComparable interface on the data in the tile.
			this.styles = new string[this.directions.Length][];
			this.styles[FadeAnimation.up] = new string[viewerFadeAnimation.Steps];
			this.styles[FadeAnimation.same] = new string[viewerFadeAnimation.Steps];
			this.styles[FadeAnimation.down] = new string[viewerFadeAnimation.Steps];

			// This array is used to align the desired highlight colors for each state changes with values that can be referenced 
			// in an interative loop like the one below.
			Color[] colors = new Color[this.directions.Length];
			colors[FadeAnimation.up] = viewerFadeAnimation.Up;
			colors[FadeAnimation.same] = viewerFadeAnimation.Same;
			colors[FadeAnimation.down] = viewerFadeAnimation.Down;

			// Iterate through each of the three directions and generate styles that will provide a transition from the highlighted
			// color to the quiescent foreground color of the tile.
			foreach (int direction in this.directions)
			{

				// Extract the color components of the start and end colors of this fade effect.
				decimal totalSteps = Convert.ToDecimal(viewerFadeAnimation.Steps);
				decimal startRed = colors[direction].R;
				decimal startGreen = colors[direction].G;
				decimal startBlue = colors[direction].B;
				decimal endRed = parentStyle.FontBrush.Color.R;
				decimal endGreen = parentStyle.FontBrush.Color.G;
				decimal endBlue = parentStyle.FontBrush.Color.B;

				// This will create a new style for each of the colors in the fade effect.  Animation is accomplished by changing 
				// the style on the cell to each one of these styles in sequence.
				for (int newStyleIndex = 0; newStyleIndex < viewerFadeAnimation.Steps; newStyleIndex++)
				{

					// Create a new style and name it using a combination of the parent style name, the direction of the movement (up,
					// down, any, none) and the step involved.
					this.styles[direction][newStyleIndex] = string.Format("{0}{1}{2}", parentStyle.StyleId, direction, newStyleIndex);
					Style style = new Style(this.styles[direction][newStyleIndex]);
					style.Parent = parentStyle;

					// The color used for the font is an involved calculation.  In round terms, the starting color is the color
					// specified in the 'Animation' parameters.  The ending color for the animated sequence is the color of the
					// font in the original style.  The steps are worked out so that the starting color morphs into the ending
					// color using the number of steps specified in the 'viewerFadeAnimation' parameters.
					decimal index = Convert.ToDecimal(newStyleIndex);
					decimal red = startRed - ((startRed - endRed) / totalSteps) * index;
					decimal green = startGreen - ((startGreen - endGreen) / totalSteps) * index;
					decimal blue = startBlue - ((startBlue - endBlue) / totalSteps) * index;
					Color brushColor = Color.FromArgb(Convert.ToInt32(red), Convert.ToInt32(green), Convert.ToInt32(blue));
					style.FontBrush = new SolidBrush(brushColor);

					// Add the color to the document's style table once it has been constructed.  As the animation thread cycles 
					// through all the styles in the array, it will pick up this style and use it as one step in the animation
					// sequence.
					styleTable.Add(this.styles[direction][newStyleIndex], style);

				}

			}

		}

		/// <summary>
		/// Sets the initial animation sequence for a tile.
		/// </summary>
		/// <param name="tile">A tile that appears in a viewer.</param>
		public override void SetSequence(Tile tile)
		{

			// The initial style is arbitrarily chosen from the 'same' sequence.  All the sequences should end with the original
			// (quiescent) color from the parent style.
			tile.StyleIndex = this.styles[FadeAnimation.same].Length - 1;
			tile.StyleArray = this.styles[FadeAnimation.same];

		}

		/// <summary>
		/// Sets the animation sequence for a tile when the data has changed.
		/// </summary>
		/// <param name="beforeTile">The current tile in the viewer.</param>
		/// <param name="afterTile">The updated version of the tile.</param>
		public override void SetSequence(Tile beforeTile, Tile afterTile)
		{

			// This will compare the data in the two tiles if they can be compared.  Otherwise the tiles are assumed to be
			// equivalent for the purpose of choosing an animation sequence.
			int compare = beforeTile.Data is IComparable && afterTile.Data is IComparable ?
				((IComparable)beforeTile.Data).CompareTo(afterTile.Data) : 0;

			// If there is no change in value and no specified animation sequence for this change, then reset the current
			// sequence.  That is, the tile will repeat the last animation sequence for the last transition detected.  Otherwise,
			// the animation sequence is chosen based on the result of comparing the two data items in the tiles.
			beforeTile.StyleIndex = 0;
			if (compare != 0 || !this.isSameSpecified)
				beforeTile.StyleArray = this.styles[compare + 1];

		}

	}

}
