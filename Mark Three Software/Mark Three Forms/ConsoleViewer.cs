namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// Provides an area for displaying streaming lines of text information.
	/// </summary>
	public partial class ConsoleViewer : MarkThree.Forms.Viewer
	{

		// Private Members
		private const int defaultMaximumLines = 32;
		private int maximumLines;
		private ArrayList lines;
		private DisplayStringEvent displayStringHandler;
		private delegate void DisplayStringEvent(Stream content);

		/// <summary>
		/// Creates a viewer for streaming text data.
		/// </summary>
		public ConsoleViewer()
		{

			// The IDE supported components are initialized here.
			InitializeComponent();

			// Initialize the object.
			this.maximumLines = ConsoleViewer.defaultMaximumLines;
			this.lines = new ArrayList();

			// The document is constructed in the background and send to the foreground using these delegates.
			this.displayStringHandler = new DisplayStringEvent(DisplayString);

		}

		[Browsable(true)]
		[Category("Behavior")]
		[Description("The maximum number of lines that can appear in the console.")]
		public int MaximumLength { get { return this.maximumLines; } set { this.maximumLines = value; } }

		/// <summary>
		/// Foreground handling of the text.
		/// </summary>
		/// <param name="displayString"></param>
		public void DisplayString(Stream contents)
		{

			try
			{

				// Load the RTF contents into the control.
				this.richTextBox.LoadFile(contents, RichTextBoxStreamType.RichText);
				contents.Close();

			}
			catch
			{

				// There's not a lot that can be done here.  The console is an output device and if it fails, we an't just
				// spew error messages from one broken device to another.

			}

		}

		/// <summary>
		/// Forces the current contents of the buffer into the viewer when the handle is created.
		/// </summary>
		/// <param name="e">Empty event arguments.</param>
		protected override void OnHandleCreated(EventArgs e)
		{

			// Before the window is created, any output is stored in the internal buffers.  When the window is finally ready for 
			// the output, this start the process of generating the buffer in a background thread, which will send it to the
			// foreground o finally be displayed.
			Thread thread = new Thread(new ParameterizedThreadStart(WriteConsole));
			thread.IsBackground = true;
			thread.Start(null);

			// Allow the base class to complete the process of creating a window handle.
			base.OnHandleCreated(e);

		}

		/// <summary>
		/// Display an information string in the console.
		/// </summary>
		/// <param name="formatString">Format specification for the output.</param>
		/// <param name="parameters">A list of arguments to appear on the console.</param>
		public void Information(string formatString, params object[] parameters)
		{


			// Ask the background to store this text in the buffer and display it in the foreground.
			Thread thread = new Thread(new ParameterizedThreadStart(WriteConsole));
			thread.IsBackground = true;
			thread.Start(string.Format(formatString, parameters));

		}

		/// <summary>
		/// Display an error information string in the console.
		/// </summary>
		/// <param name="formatString">Format specification for the output.</param>
		/// <param name="parameters">A list of arguments to appear on the console.</param>
		public void Error(string formatString, params object[] parameters)
		{

			// Ask the background to store this text in the buffer and display it in the foreground.
			Thread thread = new Thread(new ParameterizedThreadStart(WriteConsole));
			thread.IsBackground = true;
			thread.Start(@"\cf1" + string.Format(formatString, parameters) + @"\cf0");

		}

		/// <summary>
		/// Display a warning information string in the console.
		/// </summary>
		/// <param name="formatString">Format specification for the output.</param>
		/// <param name="parameters">A list of arguments to appear on the console.</param>
		public void Warning(string formatString, params object[] parameters)
		{

			// Ask the background to store this text in the buffer and display it in the foreground.
			Thread thread = new Thread(new ParameterizedThreadStart(WriteConsole));
			thread.IsBackground = true;
			thread.Start(@"\cf2" + string.Format(formatString, parameters) + @"\cf0");

		}

		/// <summary>
		/// Add new text and write the contents of the internal buffer to the screen.
		/// </summary>
		/// <param name="parameter">The text to be added to the console.</param>
		public void WriteConsole(object parameter)
		{

			// This method will construct a stream of data and send it to the foreground to be displayed in the RTF window.  This 
			// will prevent the method from doing a bunch of extra work if there is no foreground window to which to write.
			if (!this.IsHandleCreated)
				return;

			// The buffer is a critical resource that can only be used by one thread at a time.
			lock (this)
			{

				// A null parameter will simply force the screen to redraw.  This is useful when the window is first created and
				// needs to get caught up with the action.  Otherwise, when a bunch of text is passed to this thread, it is added
				// to the internal buffers.  The window has a fixed capacity, so any data that overflows the maximum number of 
				// lines is scrolled out of the buffer.
				if (parameter != null)
				{
					string dataString = (string)parameter;
					this.lines.AddRange(dataString.Split(new char[] { '\n' }));
					if (this.lines.Count > this.maximumLines)
						this.lines.RemoveRange(0, this.lines.Count - this.maximumLines);
				}

				// The RTF window only accepts RTF formatted data through a stream.  This stream will hold the formatted buffer 
				// and is passed to the foreground to be displayed.
				MemoryStream memoryStream = new MemoryStream();
				StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.ASCII);

				// Format each line of the buffer in an RTF document.
				streamWriter.Write(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fswiss\fcharset0 Arial;}}");
				streamWriter.Write(@"{\colortbl ;\red255\green0\blue0;\red128\green128\blue0;}");
				foreach (string displayLine in this.lines)
				{
					streamWriter.Write(@"\f0\fs20");
					streamWriter.Write(displayLine);
					streamWriter.Write(@"\par");
				}
				streamWriter.Write("}");
				streamWriter.Flush();

				// Now that all the RTF data is written to the buffer, reset the memory stream so that when the RTF Control tries to 
				// read it, it will start from the right place.
				memoryStream.Position = 0L;

				// Send the data to the foreground.
				BeginInvoke(this.displayStringHandler, memoryStream);

			}

		}

	}

}

