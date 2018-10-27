namespace MarkThree.Forms
{

	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// A common interface for adding document viewers to a generic application framework.
	/// </summary>
	/// <copyright>Copyright © 2006 - Mark Three Software, Inc.  All Rights Reserved.</copyright>
	public class Viewer : System.Windows.Forms.UserControl
	{

		// Private Members
		private System.Boolean isOpen;
		private System.String text;
		private System.EventHandler textChangeDelegate;
		private System.EventHandler iconChangeDelegate;
		private Icon icon;
		private ToolStrip toolStripAdvanced;
		private ToolStrip toolStripStandard;
		private MenuStrip menuStrip;

		/// <summary>Occurs when the document is starting to open the document.</summary>
		public event EventHandler Opening;

		/// <summary>Occurs when the doucment is completely open and ready to use.</summary>
		public event EventHandler EndOpenDocument;

		/// <summary>Occurs when the document is finalizing.</summary>
		public event EventHandler Closing;

		/// <summary>Occurs when the focus has been released.</summary>
		public event EventHandler ReleaseFocus;

		/// <summary>Occurs when the viewer wants to open an object in another viewer.</summary>
		public event OpenObjectEventHandler ObjectOpen;

		/// <summary>Occurs when the text for the viewer has changed.</summary>
		public new event EventHandler TextChanged;

		/// <summary>Occurs when the icon for the viewer has changed.</summary>
		public event EventHandler IconChanged;

		/// <summary>
		/// Constructor for the common viewer.
		/// </summary>
		public Viewer()
		{

			// All viewers and their documents are initialize closed.
			this.isOpen = false;

			// This is used to identify the viewer.  Generally it will appear in the title of a containing window.
			this.text = string.Empty;

			// This delegate allows the text change event to be initiated from a background thread as well as the foreground.
			// Events designed to be handled by a foreground thread need to be executed on the foreground thread.
			this.textChangeDelegate = new EventHandler(TextChangedCommand);

			// This delegate allows the icon change event to be initiated from a background thread as well as the foreground.
			// Events designed to be handled by a foreground thread need to be executed on the foreground thread.
			this.iconChangeDelegate = new EventHandler(IconChangedCommand);

			// The base control has some standard, empty menus and resources that can be used when the parent class doesn't want to
			// override the menu and tool bar buttons.  By using an empty strip, the container doesn't need to check for a special
			// condition like a 'null' when it merges and reverts the strip items from the child viewers.
			this.toolStripAdvanced = new System.Windows.Forms.ToolStrip();
			this.toolStripStandard = new System.Windows.Forms.ToolStrip();
			this.menuStrip = new System.Windows.Forms.MenuStrip();

			// menuStrip
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(150, 24);
			this.menuStrip.Text = "menuStrip";
			this.menuStrip.Visible = false;

			// toolStripAdvanced
			this.toolStripAdvanced.Location = new System.Drawing.Point(0, 0);
			this.toolStripAdvanced.Name = "toolStripAdvanced";
			this.toolStripAdvanced.Size = new System.Drawing.Size(150, 25);
			this.toolStripAdvanced.Text = "toolStripAdvanced";
			this.toolStripAdvanced.Visible = false;

			// toolStripStandard
			this.toolStripStandard.Location = new System.Drawing.Point(0, 0);
			this.toolStripStandard.Name = "toolStripStandard";
			this.toolStripStandard.Size = new System.Drawing.Size(150, 25);
			this.toolStripStandard.Text = "toolStripStandard";
			this.toolStripStandard.Visible = false;

		}

		/// <summary>Indicates whether the document is open or closed.</summary>
		[Browsable(false)]
		public bool IsOpen {get {lock (this) return this.isOpen;}}

		/// <summary>Gets or sets text of the viewer.</summary>
		[Browsable(false)]
		public new string Text {get {lock (this) return this.text;} set {lock (this) this.text = value;}}

		/// <summary>Gets or sets icon of the viewer.</summary>
		[Browsable(false)]
		public Icon Icon {get {lock (this) return this.icon;} set {lock (this) this.icon = value;}}

		/// <summary>
		/// Multicasts an event to say that this control is releasing the focus.
		/// </summary>
		protected virtual void OnReleaseFocus()
		{

			// Multicast the event that this control no longer wants the focus.
			if (ReleaseFocus != null)
				this.ReleaseFocus(this, EventArgs.Empty);

		}

		/// <summary>
		/// Used to broadcast an indication that the text of this viewer has changed.
		/// </summary>
		/// <remarks>This operation is thread-safe.</remarks>
		/// <param name="eventArgs">The event arguments.</param>
		protected override void OnTextChanged(EventArgs eventArgs)
		{

			// If this is a non-message loop thread, then use the 'Invoke' to call the foreground with the event.  A consumer of
			// this event will not expect to be in the background, so the 'Invoke' is provided to make this method thread-safe.  If
			// the thread is already in the message-loop thread, then its a simple matter to call the broadcaster.
			if (this.InvokeRequired)
				BeginInvoke(this.textChangeDelegate, new object[] {this, eventArgs});
			else
				TextChangedCommand(this, eventArgs);

		}

		/// <summary>
		/// Broadcasts an indication that the viewer's text has changed.
		/// </summary>
		/// <param name="sender">The originator of this message.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void TextChangedCommand(object sender, EventArgs eventArgs)
		{

			// Broadcast the event to any consumers.
			if (this.TextChanged != null)
				this.TextChanged(sender, eventArgs);

		}
		
		/// <summary>
		/// Used to broadcast an indication that the icon of this viewer has changed.
		/// </summary>
		/// <remarks>This operation is thread-safe.</remarks>
		/// <param name="eventArgs">The event arguments.</param>
		protected void OnIconChanged(EventArgs eventArgs) {

			// If this is a non-message loop thread, then use the 'Invoke' to call the foreground with the event.  A consumer of
			// this event will not expect to be in the background, so the 'Invoke' is provided to make this method thread-safe.  If
			// the thread is already in the message-loop thread, then its a simple matter to call the broadcaster.
			if (this.InvokeRequired)
				BeginInvoke(this.iconChangeDelegate, new object[] {this, eventArgs});
			else
				IconChangedCommand(this, eventArgs);

		}

		/// <summary>
		/// Broadcasts an indication that the viewer's icon has changed.
		/// </summary>
		/// <param name="sender">The originator of this message.</param>
		/// <param name="eventArgs">The event arguments.</param>
		private void IconChangedCommand(object sender, EventArgs eventArgs) {

			// Broadcast the event to any consumers.
			if (this.IconChanged != null)
				this.IconChanged(sender, eventArgs);

		}

		/// <summary>
		/// Opens the viewer resources in a background thread.
		/// </summary>
		/// <param name="tag">Describes the viewer object to be opened.</param>
		protected virtual void OpenCommand() {}

		/// <summary>
		/// Opens the Block Order Document in the background.
		/// </summary>
		/// <param name="userId">The unique identifier for the group of trades on the blotter.</param>
		private void OpenThread(object parameter)
		{

			// Call the overridable method to open the viewer.  An inheriting class can use this method to open the resources that
			// require a lot of time.
			OpenCommand();

			// There may be other viewers that want to know when a new object is available in the viewer.
			if (this.Opening != null)
				this.Opening(this, EventArgs.Empty);

			// Opening a document may take some time.  This thread is executed in the background.  When the document is 
			// constructed and ready to use, this will broadcast the event to any listeners.
			if (this.EndOpenDocument != null)
				this.EndOpenDocument(this, EventArgs.Empty);

		}

		/// <summary>
		/// Opens the Document in the viewer.
		/// </summary>
		/// <param name="tag">A specification of the object to display in the viewer.</param>
		public virtual void Open(object tag)
		{

			// This will indicate that the document has been opened.
			lock (this)
				this.isOpen = true;

			// The opening argument is used to redraw the viewer and to identify the contents of the viewer.
			this.Tag = tag;
		
			// This will guarantee that the completion of the initialization process will take place in a background thread.  The
			// opening of a document will often require access to the data model which can hold up a thread indefinitely.  Note
			// also that if the object is opened from the foreground, the operation will take place asynchronously.  If the object
			// is opened from the background, the opening operation will take place synchronously.
			if (this.InvokeRequired)
				OpenThread(null);
			else
				ThreadPool.QueueUserWorkItem(new WaitCallback(OpenThread));

		}
		
		/// <summary>
		/// Closes the Viewer in a background thread.
		/// </summary>
		protected virtual void CloseCommand() {}

		/// <summary>
		/// Closes the Viewer.
		/// </summary>
		public virtual void Close()
		{

			// This will indicate that the document has been closed.
			lock (this)
				this.isOpen = false;

			// This will guarantee that the completion of the finalization process will take place in a background thread.  If a 
			// viewer has resources that need to be written to a file or to a database, the background thread allows this activity
			// to progress while the foreground thread is unhindered.
			if (this.InvokeRequired)
				CloseThread(null);
			else
				ThreadPool.QueueUserWorkItem(new WaitCallback(CloseThread));

		}
		
		/// <summary>
		/// Closes the viewer from a background thread.
		/// </summary>
		private void CloseThread(object parameter)
		{

			// This virtual method allows the inheriting classes to close the resources from a background thread without having to
			// worry about all the overhead of creating and starting a thread.
			CloseCommand();

			// Other objects may be waiting for this viewer to close.  Advise them when it is finished.
			if (this.Closing != null)
				this.Closing(this, EventArgs.Empty);

		}

		/// <summary>
		/// Used to broadcast a request to open some object currently selected in this viewer.
		/// </summary>
		/// <param name="sender">The object that originates the event.</param>
		/// <param name="objectArgs">The argument used to open the object.</param>
		public virtual void OnObjectOpen(object tag)
		{

			// If there are any listerners, then multicast the request to open a viewer.
			if (this.ObjectOpen != null)
				this.ObjectOpen(this, tag);

		}

		/// <summary>
		/// Draws the document from a background thread.
		/// </summary>
		/// <param name="arguments">The arguments for drawing the document.</param>
		protected virtual void DrawDocumentCommand(object parameter) {}

		/// <summary>
		/// Draws the current document in the viewer.
		/// </summary>
		public virtual void DrawDocument()
		{

			// This will start a background thread that will construct the document then push it into the viewer.
			ThreadPool.QueueUserWorkItem(new WaitCallback(DrawDocumentCommand), this.Tag);

		}
		
		/// <summary>
		/// Saves data to the disk.
		/// </summary>
		public virtual void SaveAs() {}

		/// <summary>
		/// Loads the User Preferences from a persistent data store.
		/// </summary>
		public virtual void LoadSettings() {}

		/// <summary>
		/// Saves the User Preferences to a persistent data store.
		/// </summary>
		public virtual void SaveSettings() {}

		/// <summary>
		/// Removes the settings from the persistent User Preferences store.
		/// </summary>
		public virtual void PurgeSettings() {}

		/// <summary>
		/// A menu that is merged with the container menu.
		/// </summary>
		[Browsable(false)]
		public virtual MenuStrip MenuStrip {get {return this.menuStrip;}}

		/// <summary>
		/// A toolbar that is specific to the document viewer.
		/// </summary>
		[Browsable(false)]
		public virtual ToolStrip ToolBarStandard {get {return this.toolStripStandard;}}

		/// <summary>
		/// An advanced set of tools that are specific to the viewer.
		/// </summary>
		[Browsable(false)]
		public virtual ToolStrip ToolBarAdvanced {get {return this.toolStripAdvanced;}}

	}

}
