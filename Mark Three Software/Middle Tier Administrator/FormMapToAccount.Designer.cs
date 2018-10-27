namespace MarkThree.MiddleTier.Administrator
{
	partial class FormMapToAccount
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
			this.textBoxMapName = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.labelAccount = new System.Windows.Forms.Label();
			this.labelMapName = new System.Windows.Forms.Label();
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
			this.checkBoxEnableMapping.Size = new System.Drawing.Size(121, 17);
			this.checkBoxEnableMapping.TabIndex = 1;
			this.checkBoxEnableMapping.Text = "&Enable this mapping";
			this.checkBoxEnableMapping.UseVisualStyleBackColor = true;
			// 
			// groupBoxAccountMapping
			// 
			this.groupBoxAccountMapping.Controls.Add(this.label1);
			this.groupBoxAccountMapping.Controls.Add(this.buttonBrowse);
			this.groupBoxAccountMapping.Controls.Add(this.textBoxPassword);
			this.groupBoxAccountMapping.Controls.Add(this.textBoxAccount);
			this.groupBoxAccountMapping.Controls.Add(this.textBoxMapName);
			this.groupBoxAccountMapping.Controls.Add(this.labelPassword);
			this.groupBoxAccountMapping.Controls.Add(this.labelAccount);
			this.groupBoxAccountMapping.Controls.Add(this.labelMapName);
			this.groupBoxAccountMapping.Location = new System.Drawing.Point(13, 36);
			this.groupBoxAccountMapping.Name = "groupBoxAccountMapping";
			this.groupBoxAccountMapping.Size = new System.Drawing.Size(483, 153);
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
			this.label1.Text = "When this certificate is presented by a web client and authenticated, the user ca" +
				"n automatically be logged in as a specific Windows user.";
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Enabled = false;
			this.buttonBrowse.Location = new System.Drawing.Point(380, 84);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
			this.buttonBrowse.TabIndex = 5;
			this.buttonBrowse.Text = "&Browse...";
			this.buttonBrowse.UseVisualStyleBackColor = true;
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(82, 117);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(282, 20);
			this.textBoxPassword.TabIndex = 7;
			this.textBoxPassword.UseSystemPasswordChar = true;
			// 
			// textBoxAccount
			// 
			this.textBoxAccount.Location = new System.Drawing.Point(82, 86);
			this.textBoxAccount.Name = "textBoxAccount";
			this.textBoxAccount.Size = new System.Drawing.Size(282, 20);
			this.textBoxAccount.TabIndex = 4;
			// 
			// textBoxMapName
			// 
			this.textBoxMapName.Location = new System.Drawing.Point(82, 55);
			this.textBoxMapName.Name = "textBoxMapName";
			this.textBoxMapName.Size = new System.Drawing.Size(282, 20);
			this.textBoxMapName.TabIndex = 2;
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(22, 120);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(57, 13);
			this.labelPassword.TabIndex = 6;
			this.labelPassword.Text = "&Password:";
			// 
			// labelAccount
			// 
			this.labelAccount.Location = new System.Drawing.Point(25, 89);
			this.labelAccount.Name = "labelAccount";
			this.labelAccount.Size = new System.Drawing.Size(54, 13);
			this.labelAccount.TabIndex = 3;
			this.labelAccount.Text = "&Account:";
			// 
			// labelMapName
			// 
			this.labelMapName.Location = new System.Drawing.Point(17, 58);
			this.labelMapName.Name = "labelMapName";
			this.labelMapName.Size = new System.Drawing.Size(62, 13);
			this.labelMapName.TabIndex = 1;
			this.labelMapName.Text = "Map &Name:";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(255, 195);
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
			this.buttonCancel.Location = new System.Drawing.Point(336, 195);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonHelp
			// 
			this.buttonHelp.Location = new System.Drawing.Point(417, 195);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(75, 23);
			this.buttonHelp.TabIndex = 4;
			this.buttonHelp.Text = "Help";
			this.buttonHelp.UseVisualStyleBackColor = true;
			// 
			// FormMapToAccount
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(504, 232);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBoxAccountMapping);
			this.Controls.Add(this.checkBoxEnableMapping);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMapToAccount";
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
		private System.Windows.Forms.Label labelMapName;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.TextBox textBoxAccount;
		private System.Windows.Forms.TextBox textBoxMapName;
		private System.Windows.Forms.Label label1;
	}
}