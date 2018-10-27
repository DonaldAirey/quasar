namespace MarkThree.Forms
{
	partial class ColumnSelector
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListBox listBoxAvailableFields;
		private System.Windows.Forms.ListBox listBoxDisplayedFields;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Button buttonMoveUp;
		private System.Windows.Forms.Button buttonMoveDown;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxAvailableFields = new System.Windows.Forms.ListBox();
			this.listBoxDisplayedFields = new System.Windows.Forms.ListBox();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonMoveUp = new System.Windows.Forms.Button();
			this.buttonMoveDown = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "A&vailable fields:";
			// 
			// listBoxAvailableFields
			// 
			this.listBoxAvailableFields.Location = new System.Drawing.Point(6, 24);
			this.listBoxAvailableFields.Name = "listBoxAvailableFields";
			this.listBoxAvailableFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxAvailableFields.Size = new System.Drawing.Size(185, 186);
			this.listBoxAvailableFields.TabIndex = 1;
			// 
			// listBoxDisplayedFields
			// 
			this.listBoxDisplayedFields.Location = new System.Drawing.Point(284, 24);
			this.listBoxDisplayedFields.Name = "listBoxDisplayedFields";
			this.listBoxDisplayedFields.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxDisplayedFields.Size = new System.Drawing.Size(185, 186);
			this.listBoxDisplayedFields.TabIndex = 2;
			// 
			// buttonAdd
			// 
			this.buttonAdd.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonAdd.Location = new System.Drawing.Point(199, 24);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(76, 23);
			this.buttonAdd.TabIndex = 3;
			this.buttonAdd.Text = "&Add ->";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonRemove.Location = new System.Drawing.Point(199, 56);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(76, 23);
			this.buttonRemove.TabIndex = 4;
			this.buttonRemove.Text = "<- &Remove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(285, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(179, 16);
			this.label2.TabIndex = 5;
			this.label2.Text = "Show these fields in this order:";
			// 
			// buttonMoveUp
			// 
			this.buttonMoveUp.Location = new System.Drawing.Point(288, 216);
			this.buttonMoveUp.Name = "buttonMoveUp";
			this.buttonMoveUp.Size = new System.Drawing.Size(75, 23);
			this.buttonMoveUp.TabIndex = 6;
			this.buttonMoveUp.Text = "Move &Up";
			this.buttonMoveUp.Click += new System.EventHandler(this.buttonMoveUp_Click);
			// 
			// buttonMoveDown
			// 
			this.buttonMoveDown.Location = new System.Drawing.Point(376, 216);
			this.buttonMoveDown.Name = "buttonMoveDown";
			this.buttonMoveDown.Size = new System.Drawing.Size(75, 23);
			this.buttonMoveDown.TabIndex = 7;
			this.buttonMoveDown.Text = "Move &Down";
			this.buttonMoveDown.Click += new System.EventHandler(this.buttonMoveDown_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(288, 248);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 8;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(376, 248);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Cancel";
			// 
			// ColumnSelector
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(476, 280);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonMoveDown);
			this.Controls.Add(this.buttonMoveUp);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.listBoxDisplayedFields);
			this.Controls.Add(this.listBoxAvailableFields);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ColumnSelector";
			this.ShowInTaskbar = false;
			this.Text = "Show Fields";
			this.ResumeLayout(false);

		}
		#endregion

	}

}