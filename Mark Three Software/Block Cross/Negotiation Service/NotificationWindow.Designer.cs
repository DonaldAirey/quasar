namespace MarkThree.Guardian.Forms
{
	partial class NotificationWindow
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{

			if (disposing)
			{

				NotificationWindow.slots.RemoveAt(NotificationWindow.slots.BinarySearch(this.slotNumber));

				if (components != null)
					components.Dispose();

				System.Windows.Forms.Application.ExitThread();

			}

			base.Dispose(disposing);

		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // NotificationWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "NotificationWindow";
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer timer;
	}
}
