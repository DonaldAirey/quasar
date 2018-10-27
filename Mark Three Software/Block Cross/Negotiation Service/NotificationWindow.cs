namespace MarkThree.Guardian.Forms
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Collections.Generic;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Drawing.Drawing2D;
	using System.Reflection;
	using System.Media;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// A pop-up window that notifies the user of a potential to cross an order with another trader.
	/// </summary>
	public partial class NotificationWindow : UserControl
	{

		// Constants
		private const System.Int32 defaultGap = 17;
		private const System.Int32 defaultHeight = 116;
		private const System.Int32 defaultWaitTime = 30000;
		private const System.Int32 defaultWidth = 181;
		private const System.Int32 defaultLogoWidth = 135;
		private const System.Int32 defaultLogoHeight = 52;
		private const System.Int32 increment = 8;
		private const System.Int32 popUpTime = 500;
		private const System.Int32 popUpSteps = 30;

		// Enumerations
		private enum AnimationState { Initial, Opening, Waiting, Closing, Closed };

		// Private Static Members
		private static System.Collections.ArrayList slots;

		// Private Instance Members
		private MarkThree.Guardian.Forms.NotificationWindow.AnimationState animationState;
		private System.DateTime startTime;
		private System.Drawing.SolidBrush textBrush;
		private System.Drawing.Font titleFont;
		private System.Drawing.Font bodyFont;
		private System.Drawing.Image imageCompanyLogo;
		private System.Drawing.Rectangle clientRectangle;
		private System.Drawing.Size size;
		private System.Drawing.Color slateBlueColor;
		private System.Drawing.Color lightBlueColor;
		private System.Drawing.Color innerBorderBlueColor;
		private System.Drawing.Color innerBorderSlateColor;
		private System.Drawing.Color darkBlueColor;
		private System.Drawing.Color textColor;
		private System.Drawing.Image imageTitle;
		private System.Drawing.Image imageSelectedCancel;
		private System.Drawing.Image imageUnselectedCancel;
		private System.Drawing.Pen slateBluePen;
		private System.Drawing.Pen lightBluePen;
		private System.Drawing.Pen innerBorderBluePen;
		private System.Drawing.Pen innerBorderSlatePen;
		private System.Drawing.Pen darkBluePen;
		private System.Drawing.Pen whitePen;
		private System.Int32 matchId;
		private System.Int32 slotNumber;
		private System.Int32 waitTime;
		private System.Media.SoundPlayer soundPlayerType;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button logoButton;
		private System.Windows.Forms.Label labelOptions;
		private System.String symbol;
		private System.String message;

		// Public Events
		internal MatchEventHandler Accept;
		internal MatchEventHandler Decline;
		internal EventHandler ChangeOptions;

		/// <summary>
		/// Initailize the static elements of the notification window.
		/// </summary>
		static NotificationWindow()
		{

			// This keeps track of the number of notification windows active at any one time.
			NotificationWindow.slots = new ArrayList();

		}
		
		/// <summary>
		/// Provides a window that notifies the user of asynchronous events.
		/// </summary>
		public NotificationWindow()
		{

			// Initialize the IDE maintained components.
			InitializeComponent();

			try
			{


				// This keeps the notification windows from popping up on top of each other.  The 'slot' is basically the position on 
				// the screen where the notification will be posted.  This position is guaranteed to be free of other notification
				// windows because all the used slots are kept in a static list of used slots.
				this.slotNumber = 0;
				while (true)
				{
					int index = NotificationWindow.slots.BinarySearch(this.slotNumber);
					if (index < 0)
					{
						NotificationWindow.slots.Insert(~index, this.slotNumber);
						break;
					}
					this.slotNumber++;
				}

				// Load up the notification sound and the image that will appear in the title bar of the notification window.
				System.Reflection.Assembly assembly = Assembly.GetAssembly(typeof(NotificationWindow));
				this.soundPlayerType = new SoundPlayer(assembly.GetManifestResourceStream("MarkThree.Guardian.Forms.Type.wav"));
				this.soundPlayerType.LoadCompleted += new AsyncCompletedEventHandler(soundPlayerType_LoadCompleted);
				this.imageTitle = new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Forms.Guardian.png"));

				// This is the image used for the cancel button.
				this.imageSelectedCancel = new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Forms.Selected Cancel.png"));
				this.imageUnselectedCancel = new Bitmap(assembly.GetManifestResourceStream("MarkThree.Guardian.Forms.Unselected Cancel.png"));

				// Initialize the configurable properties with default values.
				this.waitTime = NotificationWindow.defaultWaitTime;
				this.symbol = string.Empty;
				this.message = string.Empty;
				this.imageCompanyLogo = null;
				this.size = new Size(NotificationWindow.defaultWidth, NotificationWindow.defaultHeight);

				// This is the font used to draw the title text.
				this.titleFont = new Font("Arial", 8.0f);
				this.bodyFont = new Font("Tahoma", 12.0f, FontStyle.Bold);
                
				// These are the colors used by this control.
				this.slateBlueColor = Color.FromArgb(166, 180, 207);
				this.lightBlueColor = Color.FromArgb(207, 222, 244);
				this.innerBorderBlueColor = Color.FromArgb(114, 142, 184);
				this.innerBorderSlateColor = Color.FromArgb(185, 201, 239);
				this.darkBlueColor = Color.FromArgb(69, 86, 144);
				this.textColor = Color.FromArgb(31, 51, 107);

				// This brush is used for the text fields.
				this.textBrush = new SolidBrush(this.textColor);

				// These are the pens used to draw the control.
				this.slateBluePen = new Pen(this.slateBlueColor);
				this.lightBluePen = new Pen(this.lightBlueColor);
				this.innerBorderBluePen = new Pen(this.innerBorderBlueColor);
				this.innerBorderSlatePen = new Pen(this.innerBorderSlateColor);
				this.darkBluePen = new Pen(this.darkBlueColor);
				this.whitePen = new Pen(Color.White);

				// Cancel Button
				this.cancelButton = new Button();
				this.cancelButton.FlatAppearance.BorderSize = 0;
				this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
				this.cancelButton.BackColor = System.Drawing.Color.Transparent;
				this.cancelButton.Size = this.imageUnselectedCancel.Size;
				this.cancelButton.Image = this.imageUnselectedCancel;
				this.cancelButton.Enabled = true;
				this.cancelButton.Visible = true;
				this.cancelButton.Click += new EventHandler(cancelButton_Click);
				this.cancelButton.MouseEnter += new EventHandler(cancelButton_MouseEnter);
				this.cancelButton.MouseLeave += new EventHandler(cancelButton_MouseLeave);
				this.Controls.Add(this.cancelButton);

				// Options Label
				this.labelOptions = new System.Windows.Forms.Label();
				this.labelOptions.BackColor = System.Drawing.Color.Transparent;
				this.labelOptions.Location = new System.Drawing.Point(64, 52);
				this.labelOptions.Size = new System.Drawing.Size(55, 14);
				this.labelOptions.TextAlign = ContentAlignment.MiddleRight;
				this.labelOptions.Font = new Font("Tahoma", 8.0f, FontStyle.Regular);
				this.labelOptions.ForeColor = this.textColor;
				this.labelOptions.Name = "LabelOptions";
				this.labelOptions.Text = "Options";
				this.labelOptions.MouseEnter += new EventHandler(labelOptions_MouseHover);
				this.labelOptions.MouseLeave += new EventHandler(labelOptions_MouseLeave);
				this.labelOptions.Click += new EventHandler(labelOptions_Click);
				this.labelOptions.Enabled = true;
				this.Controls.Add(this.labelOptions);

				// Corporate Logo
				this.logoButton = new Button();
				this.logoButton.FlatAppearance.BorderSize = 0;
				this.logoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
				this.logoButton.BackColor = System.Drawing.Color.Transparent;
				this.logoButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
				this.logoButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
				this.logoButton.Enabled = true;
				this.logoButton.Visible = true;
                this.logoButton.Cursor = System.Windows.Forms.Cursors.Hand;
				this.logoButton.Click += new EventHandler(logoButton_Click);
				this.Controls.Add(this.logoButton);

			}
			catch (Exception exception)
			{

				EventLog.Error(exception.Message);

			}

			// This will enable the built-in double buffering scheme during the OnPaint method to eliminate flicker.
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			// Even though the control was set to be a top-level control in the 'CreateParams' override, it must be set here also
			// or the control is never made visible.
			SetTopLevel(true);

			ThreadPool.QueueUserWorkItem(new WaitCallback(InitializeData), this.MatchId);

		}

		void soundPlayerType_LoadCompleted(object sender, AsyncCompletedEventArgs e)
		{

			// Once the sound has been loaded up, play it back in the foreground thread.  Using the 'Play' method appears to cause
			// problems when multiple threads try to play the same sound.  This should eventually be fixed because the application 
			// won't do anything until this sound has finished.
			this.soundPlayerType.PlaySync();

		}

		/// <summary>
		/// Initializes the data required for this component.
		/// </summary>
		/// <param name="parameter">Thread initialization data (not used).</param>
		private void InitializeData(object parameter)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Hook this service into the data model.  It will watch for any new matches and create a pop-up window when an
				// opportunity arises.
				ClientMarketData.Match.MatchRowChanging += new DataSetMarket.MatchRowChangeEventHandler(Match_MatchRowChanging);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Initializes the data required for this component.
		/// </summary>
		/// <param name="parameter">Thread initialization data (not used).</param>
		private void UninitializeData(object parameter)
		{

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Hook this service into the data model.  It will watch for any new matches and create a pop-up window when an
				// opportunity arises.
				ClientMarketData.Match.MatchRowChanging -= new DataSetMarket.MatchRowChangeEventHandler(Match_MatchRowChanging);

			}
			catch (Exception exception)
			{

				// Write the error and stack trace out to the debug listener
				EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		/// <summary>
		/// Event handler for a match.
		/// </summary>
		/// <param name="sender">The object that originated the event.</param>
		/// <param name="e">The event arguments.</param>
		void Match_MatchRowChanging(object sender, DataSetMarket.MatchRowChangeEvent e)
		{

			// When a new, pending match record has been added to the data mode, start a thread that will
			// display the notification window.
			if (e.Action == DataRowAction.Commit)
			{
				if (e.Row.MatchId == this.matchId && e.Row.StatusCode != Status.Active)
					this.animationState = AnimationState.Closing;
			}

		}

		/// <summary>
		/// This window requires non-standard style flags that can only be set during creation.
		/// </summary>
		protected override CreateParams CreateParams
		{

			get
			{

				// This window will not have any border (WindowEdge) and will act and behave generally like a top-level tool 
				// window.
				CreateParams createParams = new CreateParams();
				createParams.ClassName = base.CreateParams.ClassName;
				createParams.Style = unchecked((int)(WindowStyle.Popup | WindowStyle.Clipsiblings | WindowStyle.Clipchildren));
				createParams.ExStyle = unchecked((int)(ExtendedWindowStyle.Windowedge | ExtendedWindowStyle.Toolwindow |
					ExtendedWindowStyle.Topmost));
				return createParams;

			}

		}

		/// <summary>
		/// Paints the column cursor.
		/// </summary>
		/// <param name="paintEventArgs">Arguments for painting in the graphics context.</param>
		protected override void OnPaint(PaintEventArgs paintEventArgs)
		{

			// This is a shorthand reference to the Graphics context where the painting is done.
			Graphics graphics = paintEventArgs.Graphics;

			// The outer border is reverse-engineered from the MSN notification window.
			graphics.DrawLine(darkBluePen, new Point(0, this.size.Height - 1), new Point(this.size.Width - 1, this.size.Height - 1));
			graphics.DrawLine(darkBluePen, new Point(this.size.Width - 1, this.size.Height - 1), new Point(this.size.Width - 1, 0));
			graphics.DrawLine(slateBluePen, new Point(this.size.Width - 1, 0), new Point(0, 0));
			graphics.DrawLine(slateBluePen, new Point(0, 0), new Point(0, this.size.Height - 1));
			graphics.DrawLine(lightBluePen, new Point(1, this.size.Height - 2), new Point(this.size.Width - 2, this.size.Height - 2));
			graphics.DrawLine(lightBluePen, new Point(this.size.Width - 2, this.size.Height - 2), new Point(this.size.Width - 2, 1));
			graphics.DrawLine(whitePen, new Point(this.size.Width - 3, 1), new Point(1, 1));
			graphics.DrawLine(whitePen, new Point(1, 1), new Point(1, this.size.Height - 3));

			// Background Image.  The background image is reverse engineered from the MSN notification window.  The values were
			// derived empirically.
			Rectangle backgroundRectangle = new Rectangle(this.ClientRectangle.Left + 2, this.ClientRectangle.Top + 2,
				this.ClientRectangle.Width - 4, this.size.Height - 4);
			LinearGradientBrush backgroundBrush = new LinearGradientBrush(backgroundRectangle, this.lightBlueColor,
				Color.White, LinearGradientMode.Vertical);
			Blend blend = new Blend();
			blend.Factors = new float[] { 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.50f };
			blend.Positions = new float[] { 0.0f, 0.10f, 0.18f, 0.36f, 0.80f, 1.0f };
			backgroundBrush.Blend = blend;
			graphics.FillRectangle(backgroundBrush, backgroundRectangle);

			// Title Area
			graphics.DrawImage(this.imageTitle, new Rectangle(6, 4, 16, 16));
			Rectangle titleTextRectangle = new Rectangle(22, 6, 112, 14);
			graphics.DrawString("Cross Notification", this.titleFont, this.textBrush, titleTextRectangle);

			// Inner Border
			graphics.DrawLine(innerBorderSlatePen, new Point(this.clientRectangle.Left, this.clientRectangle.Bottom),
				new Point(this.clientRectangle.Right, this.clientRectangle.Bottom));
			graphics.DrawLine(innerBorderSlatePen, new Point(this.clientRectangle.Right, this.clientRectangle.Bottom),
				new Point(this.clientRectangle.Right, this.clientRectangle.Top));
			graphics.DrawLine(innerBorderBluePen, new Point(this.clientRectangle.Right, this.clientRectangle.Top),
				new Point(this.clientRectangle.Left, this.clientRectangle.Top));
			graphics.DrawLine(innerBorderBluePen, new Point(this.clientRectangle.Left, this.clientRectangle.Top),
				new Point(this.clientRectangle.Left, this.clientRectangle.Bottom));

			// The Message Text
			Rectangle messageTextRectangle = new Rectangle(this.clientRectangle.Left, this.clientRectangle.Bottom - 18,
				this.clientRectangle.Width, 16);                                                            
			StringFormat stringFormat = StringFormat.GenericDefault;
            stringFormat.Alignment = StringAlignment.Far;
			stringFormat.LineAlignment = StringAlignment.Center;
			graphics.DrawString(this.message, this.bodyFont, this.textBrush, messageTextRectangle, stringFormat);

		}

		/// <summary>
		/// The start of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public System.Int32 MatchId
		{

			get { return this.matchId; }
			set { this.matchId = value; }

		}

		/// <summary>
		/// The start of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(true)]
		[Category("Appearance")]
		[Description("This is the symbol that is displayed in the notification message.")]
		public string Symbol
		{

			get { return this.symbol; }
			set { this.symbol = value; }

		}

		/// <summary>
		/// The start of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(true)]
		[Category("Appearance")]
		[Description("This is the message that is displayed at the bottom of the notification pop-up screen.")]
		public string Message
		{

			get { return this.message; }
			set { this.message = value; }

		}
		
		/// <summary>
		/// The start of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(true)]
		[Category("Appearance")]
		[Description("This is the company logo that is shown in the notification window.")]
		public Image CompanyLogo
		{

			get { return this.imageCompanyLogo; }
			set { this.imageCompanyLogo = value; }

		}

		/// <summary>
		/// The start of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(true)]
		[Category("Layout")]
		[Description("The maximum size of the control in pixels.")]
		public new Size Size
		{

			get { return this.size; }
			set { this.size = value; }

		}

		/// <summary>
		/// The start of the range of times displayed in the domain control.
		/// </summary>
		[Browsable(true)]
		[Category("Behavior")]
		[Description("This is the time, in milliseconds, the notification will be displayed.")]
		public int WaitTime
		{

			get { return this.waitTime; }
			set { this.waitTime = value; }

		}

		/// <summary>
		/// Show the notification window with an accompanying sound.
		/// </summary>
		public new void Show()
		{

			// This window is animated, so there's the actual size that appears on the screen, and there's the potential size. That
			// is, the size the window will be when it is fully displayed.  The 'size' parameter contains the window's potential
			// size.  It is initially dislayed with no height and grows vertically until the desired height is obtained.
			base.Size = new Size(this.size.Width, 0);

			// If a company logo hasn't been provided, the symbol name is used in large, distinctive letters.
			if (this.imageCompanyLogo == null)
			{

				this.imageCompanyLogo = new Bitmap(NotificationWindow.defaultLogoWidth, NotificationWindow.defaultLogoHeight);
				Graphics graphicsBitmap = Graphics.FromImage(this.imageCompanyLogo);
				Font font = new Font("Tahoma", 20.0f, FontStyle.Bold | FontStyle.Italic);
				Rectangle rectangle = new Rectangle(Point.Empty, this.imageCompanyLogo.Size);
				StringFormat stringFormat = StringFormat.GenericDefault;
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				stringFormat.FormatFlags = StringFormatFlags.NoWrap;
				graphicsBitmap.DrawString(this.symbol, font, this.textBrush, rectangle, stringFormat);
				graphicsBitmap.Dispose();

			}

			// A large button in the center of the window will hold either the symbol of the company or the comany logo.  When it
			// is pressed, the negotiation is accepted.
			this.logoButton.Image = this.imageCompanyLogo;

			// I believe there is a better way to maintain the client rectangle than this one, but it works for now.  This defines
			// the area where the non-frame elements are displayed.  The values have been reverse-engineered from the MSN
			// notification window.
			this.clientRectangle = new Rectangle(2, 24, this.size.Width - 5, this.size.Height - 27);

			// This is the working area of the screen that has the system task bar.  It is used to calculate the position of the
			// notification window so it pops up in the lower, right hand corner.  That is, it should pop up in the same location
			// and with the same animiation effects as Micrsoft Instant Messaging.
			Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

			// Lay the elements out in the window.
			this.SuspendLayout();

			// This is 'slot' where the notification window is displayed.
			this.Location = new Point(workingArea.Right - (this.Width + NotificationWindow.defaultGap),
				workingArea.Bottom - (this.size.Height * this.slotNumber));

			// This is where the "Cancel" button is displayed.
			this.cancelButton.Location = new Point(this.size.Width - this.cancelButton.Image.Width - 6, 6);

			// This is where the "Options" label is displayed.
			this.labelOptions.Location = new Point(this.clientRectangle.Right + 2 - this.labelOptions.Width,
				this.clientRectangle.Top);

			// This is where the company name of logo is displayed.
			this.logoButton.Location = new Point((this.clientRectangle.Width - this.logoButton.Image.Width) / 2
				+ this.clientRectangle.Left, (this.clientRectangle.Height - this.logoButton.Image.Height) / 2 +
				this.clientRectangle.Top);
			this.logoButton.Size = new Size(this.logoButton.Image.Width, this.logoButton.Image.Height);

			// All the elements have been placed in the window.
			this.ResumeLayout(false);

			// The animation is driven by a series of states and a timer.  This will start the process that will make the
			// notification window grow.
			this.animationState = AnimationState.Initial;
			this.timer.Interval = NotificationWindow.popUpTime / NotificationWindow.popUpSteps;
			this.timer.Start();

			// This works like a dialog box in that it has it's own message thread to handle the timer messages.
			Application.Run();

		}

		/// <summary>
		/// Animate the window on opening and closing.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The event arguments.</param>
		private void timer_Tick(object sender, EventArgs e)
		{

			// The animation is driven by the periodic timer and a series of states.
			switch (this.animationState)
			{

			case AnimationState.Initial:

				// This initializes the animation and plays the opening sound.
				this.Visible = true;
				this.startTime = DateTime.Now;
				this.animationState = AnimationState.Opening;
				this.soundPlayerType.LoadAsync();
				break;

			case AnimationState.Opening:

				// This will calculate by how much the window is to grow on this cycle.
				int increment = this.Height + NotificationWindow.increment > this.size.Height ? this.size.Height - this.Height :
					NotificationWindow.increment;

				// Grow the window.
				this.SuspendLayout();
				this.Top -= increment;
				this.Height += increment;
				this.ResumeLayout(false);

				// If the window has reached its potential height, then go into the waiting state.
				if (this.Height >= this.size.Height)
					this.animationState = AnimationState.Waiting;

				break;

			case AnimationState.Closing:

				// This will calculate by how much this window is to shrink on this cycle.
				int decrement = this.Height - NotificationWindow.increment < 0 ? -this.Height :
					-NotificationWindow.increment;

				// Shrink the window.
				this.SuspendLayout();
				this.Height += decrement;
				this.Top -= decrement;
				this.ResumeLayout(false);

				// If the window has shrunk to nothing, move on to closing it.
				if (this.Height == 0)
					this.animationState = AnimationState.Closed;

				break;

			case AnimationState.Closed:

				// This will close and dispose the window.
				this.Visible = false;
				ThreadPool.QueueUserWorkItem(new WaitCallback(UninitializeData));
				this.timer.Stop();
				this.Dispose();
				break;

			}

		}

		/// <summary>
		/// Removes the highlighting from the Cancel button when the mouse is no longer over it.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The event arguments.</param>
		void cancelButton_MouseLeave(object sender, EventArgs e)
		{

			// Remove the highlighting.
			this.cancelButton.Image = this.imageUnselectedCancel;

		}

		/// <summary>
		/// Highlights the Cancel button when the mouse is over it.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The event arguments.</param>
		void cancelButton_MouseEnter(object sender, EventArgs e)
		{

			// Highlight the Cancel button.
			this.cancelButton.Image = this.imageSelectedCancel;

		}

		/// <summary>
		/// Handles the click of the Cancel Button.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The event arguments.</param>
		void cancelButton_Click(object sender, EventArgs e)
		{

			// Immediately close the notification window.
			this.animationState = AnimationState.Closing;

			// Advise any listeners that the negotiation has been rejected.
			if (this.Decline != null)
				this.Decline(this, new MatchEventArgs(this.matchId));

		}

		/// <summary>
		/// Removes the highlighting from the "Options" when the mouse is no longer over it.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The event arguments.</param>
		void labelOptions_MouseLeave(object sender, EventArgs e)
		{

			// Reconstitute the font without the underline.
			Font font = this.labelOptions.Font;
			this.labelOptions.Font = new Font(font.Name, font.Size, font.Style & ~FontStyle.Underline);

		}

		/// <summary>
		/// Highlights the "Options" text when the mouse is over it.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The event arguments.</param>
		void labelOptions_MouseHover(object sender, EventArgs e)
		{

			// This will reconstitute the font with an underline to highlight the fact that the mouse can select
			// the "Options".
			Font font = this.labelOptions.Font;
			this.labelOptions.Font = new Font(font.Name, font.Size, font.Style | FontStyle.Underline);

		}

		/// <summary>
		/// Handles the selection of the "Options" text.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The event arguments.</param>
		void labelOptions_Click(object sender, EventArgs e)
		{

			// Broadcast the event to anyone listening.
			if (this.ChangeOptions != null)
				this.ChangeOptions(this, EventArgs.Empty);

		}

		/// <summary>
		/// Handles the selection of the company logo to accept the negotiation.
		/// </summary>
		/// <param name="sender">The object that originated this event.</param>
		/// <param name="e">The event arguments.</param>
		void logoButton_Click(object sender, EventArgs e)
		{

			// Immediately close the notification window.
			this.animationState = AnimationState.Closing;

			// Advise any listeners that the negotiation has been rejected.
			if (this.Accept != null)
				this.Accept(this, new MatchEventArgs(this.matchId));

		}

	}

}
