namespace MarkThree.Guardian.Forms
{
	partial class OrderBookViewer
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

			// Disposed of the managed objects.
			if (disposing)
			{

				// Terminate the ticker thread.
				this.IsTickerRunning = false;
				if (!this.thread.Join(100))
					this.thread.Abort();

				// Destroy all the components.
				if (components != null)
					components.Dispose();

			}

			// Allow the base class to complete the clean up.
			base.Dispose(disposing);

		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}

		#endregion

	}

}
