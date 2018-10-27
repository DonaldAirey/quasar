namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Data;
	using System.Windows.Forms;

	/// <summary>
	/// A cursor that indicates to the user where a column will be located after it is moved.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public partial class DestinationCursor : Control
	{

		// Private Members
		private bool isOnTarget;

		/// <summary>
		/// Create the cursor used to indicate the destination of a column drag-and-drop operation.
		/// </summary>
		/// <param name="headerHeight"></param>
		public DestinationCursor(float headerHeight)
		{

			// The IDE managed components are initialized here.
			InitializeComponent();

			// Initialize the object.
			this.SetTopLevel(true);
			this.BringToFront();
			this.BackColor = System.Drawing.Color.Red;
			this.Size = new Size(9, Convert.ToInt32(headerHeight + 20));
			base.Visible = true;
			this.Visible = false;

		}

		/// <summary>
		/// The creation parameters for the column cursor.
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{

				// Thiw will create a window that floats above all other windows, has no task list, edges or other trappings of windows.
				CreateParams createParams = new CreateParams();
				createParams.ClassName = base.CreateParams.ClassName;
				createParams.Style = unchecked((int)(WindowStyle.Popup | WindowStyle.Clipsiblings | WindowStyle.Clipchildren | WindowStyle.Disabled));
				createParams.ExStyle = unchecked((int)(ExtendedWindowStyle.Windowedge | ExtendedWindowStyle.Toolwindow | ExtendedWindowStyle.Topmost));
				return createParams;

			}

		}

		public new Point Location
		{

			get
			{
				Point point = base.Location;
				point.X += 4;
				point.Y += 12;
				return point;
			}

			set
			{
				value.X -= 4;
				value.Y -= 12;
				base.Location = value;
			}

		}

		public new bool Visible
		{

			get { return this.isOnTarget; }

			set
			{

				if (this.isOnTarget = value)
				{

					Point[] points = new Point[7];

					GraphicsPath graphicsPath = new GraphicsPath();

					points[0] = new Point(3, 0);
					points[1] = new Point(6, 0);
					points[2] = new Point(6, 4);
					points[3] = new Point(9, 4);
					points[4] = new Point(4, 9);
					points[5] = new Point(0, 4);
					points[6] = new Point(3, 4);

					graphicsPath.AddPolygon(points);

					points[0] = new Point(3, this.ClientRectangle.Height - 0);
					points[1] = new Point(6, this.ClientRectangle.Height - 0);
					points[2] = new Point(6, this.ClientRectangle.Height - 4);
					points[3] = new Point(10, this.ClientRectangle.Height - 4);
					points[4] = new Point(4, this.ClientRectangle.Height - 10);
					points[5] = new Point(-1, this.ClientRectangle.Height - 4);
					points[6] = new Point(3, this.ClientRectangle.Height - 4);

					graphicsPath.AddPolygon(points);

					this.Region = new Region(graphicsPath);

				}
				else
					this.Region = new Region(new Rectangle(0, 0, 0, 0));

			}

		}

	}
}
