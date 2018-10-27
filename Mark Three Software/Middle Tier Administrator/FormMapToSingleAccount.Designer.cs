namespace MarkThree.MiddleTier.Administrator
{
	partial class FormMapToSingleAccount
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
			this.checkBoxEnableMapping = new System.Windows.Forms.CheckBox();
			this.groupBoxAccountMapping = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonBrowse = new System.Windows.Forms.Button();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.textBoxAccount = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.labelAccount = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.groupBoxAccountMapping.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkBoxEnableMapping
			// 
			this.checkBoxEnableMapping.AutoSize = true;
			this.checkBoxEnableMapping.Location = new System.Drawing.Point(13, 12);
			this.checkBoxEnableMapping.Name = "checkBoxEnableMapping";
			this.checkBoxEnableMapping.Size = new System.Drawing.Size(136, 17);
			this.checkBoxEnableMapping.TabIndex = 1;
			this.checkBoxEnableMapping.Text = "&Enable these mappings";
			this.checkBoxEnableMapping.UseVisualStyleBackColor = true;
			// 
			// groupBoxAccountMapping
			// 
			this.groupBoxAccountMapping.Controls.Add(this.label1);
			this.groupBoxAccountMapping.Controls.Add(this.buttonBrowse);
			this.groupBoxAccountMapping.Controls.Add(this.textBoxPassword);
			this.groupBoxAccountMapping.Controls.Add(this.textBoxAccount);
			this.groupBoxAccountMapping.Controls.Add(this.labelPassword);
			this.groupBoxAccountMapping.Controls.Add(this.labelAccount);
			this.groupBoxAccountMapping.Location = new System.Drawing.Point(13, 36);
			this.groupBoxAccountMapping.Name = "groupBoxAccountMapping";
			this.groupBoxAccountMapping.Size = new System.Drawing.Size(479, 121);
			this.groupBoxAccountMapping.TabIndex = 0;
			this.groupBoxAccountMapping.TabStop = false;
			this.groupBoxAccountMapping.Text = "Account Mapping";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(429, 29);
			this.label1.TabIndex = 0;
			this.label1.Text = "When any of these certificates are presented by a web client and authenticated, t" +
				"he user can automatically be logged in as a specific Windows user.";
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Enabled = false;
			this.buttonBrowse.Location = new System.Drawing.Point(381, 50);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
			this.buttonBrowse.TabIndex = 5;
			this.buttonBrowse.Text = "&Browse...";
			this.buttonBrowse.UseVisualStyleBackColor = true;
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(83, 83);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(282, 20);
			this.textBoxPassword.TabIndex = 7;
			this.textBoxPassword.UseSystemPasswordChar = true;
			// 
			// textBoxAccount
			// 
			this.textBoxAccount.Location = new System.Drawing.Point(83, 52);
			this.textBoxAccount.Name = "textBoxAccount";
			this.textBoxAccount.Size = new System.Drawing.Size(282, 20);
			this.textBoxAccount.TabIndex = 4;
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(23, 86);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(57, 13);
			this.labelPassword.TabIndex = 6;
			this.labelPassword.Text = "&Password:";
			// 
			// labelAccount
			// 
			this.labelAccount.Location = new System.Drawing.Point(26, 55);
			this.labelAccount.Name = "labelAccount";
			this.labelAccount.Size = new System.Drawing.Size(54, 13);
			this.labelAccount.TabIndex = 3;
			this.labelAccount.Text = "&Account:";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(252, 168);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(333, 168);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonHelp
			// 
			this.buttonHelp.Location = new System.Drawing.Point(414, 168);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(75, 23);
			this.buttonHelp.TabIndex = 4;
			this.buttonHelp.Text = "Help";
			this.buttonHelp.UseVisualStyleBackColor = true;
			// 
			// FormMapToSingleAccount
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(504, 203);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBoxAccountMapping);
			this.Controls.Add(this.checkBoxEnableMapping);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMapToSingleAccount";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Map to Account";
			this.groupBoxAccountMapping.ResumeLayout(false);
			this.groupBoxAccountMapping.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxEnableMapping;
		private System.Windows.Forms.GroupBox groupBoxAccountMapping;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label labelAccount;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.TextBox textBoxAccount;
		private System.Windows.Forms.Label label1;
	}
}