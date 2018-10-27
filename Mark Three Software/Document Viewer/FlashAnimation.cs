namespace MarkThree.Forms
{

	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;

	/// <summary>
	/// Provides an animation effect that changes the foreground and background colors periodically.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	class FlashAnimation : Animation
	{

		// Private Members
		private System.String[] styles;

		/// <summary>
		/// Creates the effect used to change the foreground and background colors periodically.
		/// </summary>
		/// <param name="styleTable">The table of styles used by a document viewer.</param>
		/// <param name="parentStyle">The original style for this effect.</param>
		/// <param name="viewerFadeAnimation">The parameters of the fade effect.</param>
		public FlashAnimation(Dictionary<string, Style> styleTable, Style parentStyle, ViewerFlashAnimation viewerFlashAnimation)
		{

			// The number of styles in this sequence depends on the number of repetitions and the duty cycle specified in the
			// ViewerFlashAnimation parameter.  The idea is that every period a thread is run that will cycle the tile through each
			// of the styles specified in this array.
			this.styles = new string[(viewerFlashAnimation.On + viewerFlashAnimation.Off) * viewerFlashAnimation.Repeat + 1];

			// This counter is used to keep track of the styles as they are created and added to the list of styles that will be 
			// cycled through to provide the animated effect.
			int styleIndex = 0;

			// This will create a new style for the Flash effect using the foreground and background colors.
			Style style = new Style(string.Format("{0}Flash", parentStyle.StyleId));
			style.Parent = parentStyle;
			style.FontBrush = new SolidBrush(viewerFlashAnimation.Foreground);
			style.InteriorBrush = new SolidBrush(viewerFlashAnimation.Background);
			styleTable.Add(style.StyleId, style);

			// There is always an extra starting style because the Animation thread runs asynchronously to the invokation of the
			// animation effect.  For example, the animation thread may be running every second.  If the tile is updated a 900
			// milliseconds after an animation thread is run, then the first style in the sequence will be displayed for a mere 100
			// miliseconds.  This ends up being very disturbing to look at.  However, if the first sequence is always repeated,
			// then it will appear on the screen anywhere from a fraction over 1 second to 2 seconds which appears much more
			// consistent.
			this.styles[styleIndex++] = style.StyleId;

			// This will create the sequence that cycles through the flash style and the normal parent style for the specified duty
			// cycle and number of repetitions.
			for (int repeatIndex = 0; repeatIndex < viewerFlashAnimation.Repeat; repeatIndex++)
			{
				for (int onIndex = 0; onIndex < viewerFlashAnimation.On; onIndex++)
					this.styles[styleIndex++] = style.StyleId;
				for (int offIndex = 0; offIndex < viewerFlashAnimation.Off; offIndex++)
					this.styles[styleIndex++] = parentStyle.StyleId;
			}

		}

		/// <summary>
		/// Sets the initial animation sequence for a tile.
		/// </summary>
		/// <param name="tile">A tile that appears in a viewer.</param>
		public override void SetSequence(Tile beforeTile, Tile afterTile)
		{

			// This will start the flashing sequence whenver the data is different.
			beforeTile.StyleIndex = 0;
			beforeTile.StyleArray = this.styles;

		}

		/// <summary>
		/// Sets the animation sequence for a tile when the data has changed.
		/// </summary>
		/// <param name="beforeTile">The current tile in the viewer.</param>
		/// <param name="afterTile">The updated version of the tile.</param>
		public override void SetSequence(Tile tile)
		{

			// This will initialize the animation sequence to the parent (quiescent) style.
			tile.StyleIndex = this.styles.Length - 1;
			tile.StyleArray = this.styles;

		}

	}

}
