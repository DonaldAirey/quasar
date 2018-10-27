namespace MarkThree.MiddleTier.Administrator
{
	partial class AccountMappings
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountMappings));
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPage1to1 = new System.Windows.Forms.TabPage();
			this.listViewOneToOne = new System.Windows.Forms.ListView();
			this.columnHeaderState = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderMappingName = new System.Windows.Forms.ColumnHeader();
			this.columnHeaderWindowsAccount = new System.Windows.Forms.ColumnHeader();
			this.imageListState = new System.Windows.Forms.ImageList(this.components);
			this.labelInstructions = new System.Windows.Forms.Label();
			this.buttonDelete = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonEditMap = new System.Windows.Forms.Button();
			this.groupBoxIssuer = new System.Windows.Forms.GroupBox();
			this.groupBoxSubject = new System.Windows.Forms.GroupBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonApply = new System.Windows.Forms.Button();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.openFileDialogCertificate = new System.Windows.Forms.OpenFileDialog();
			this.tabControl.SuspendLayout();
			this.tabPage1to1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPage1to1);
			this.tabControl.Location = new System.Drawing.Point(6, 7);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(599, 384);
			this.tabControl.TabIndex = 0;
			// 
			// tabPage1to1
			// 
			this.tabPage1to1.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage1to1.Controls.Add(this.listViewOneToOne);
			this.tabPage1to1.Controls.Add(this.labelInstructions);
			this.tabPage1to1.Controls.Add(this.buttonDelete);
			this.tabPage1to1.Controls.Add(this.buttonAdd);
			this.tabPage1to1.Controls.Add(this.buttonEditMap);
			this.tabPage1to1.Controls.Add(this.groupBoxIssuer);
			this.tabPage1to1.Controls.Add(this.groupBoxSubject);
			this.tabPage1to1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1to1.Name = "tabPage1to1";
			this.tabPage1to1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1to1.Size = new System.Drawing.Size(591, 358);
			this.tabPage1to1.TabIndex = 0;
			this.tabPage1to1.Text = "1-to-1";
			// 
			// listViewOneToOne
			// 
			this.listViewOneToOne.AutoArrange = false;
			this.listViewOneToOne.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderState,
            this.columnHeaderMappingName,
            this.columnHeaderWindowsAccount});
			this.listViewOneToOne.FullRowSelect = true;
			this.listViewOneToOne.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewOneToOne.Location = new System.Drawing.Point(6, 50);
			this.listViewOneToOne.Name = "listViewOneToOne";
			this.listViewOneToOne.Size = new System.Drawing.Size(318, 254);
			this.listViewOneToOne.StateImageList = this.imageListState;
			this.listViewOneToOne.TabIndex = 7;
			this.listViewOneToOne.UseCompatibleStateImageBehavior = false;
			this.listViewOneToOne.View = System.Windows.Forms.View.Details;
			this.listViewOneToOne.DoubleClick += new System.EventHandler(this.buttonEditMap_Click);
			// 
			// columnHeaderState
			// 
			this.columnHeaderState.Text = "";
			this.columnHeaderState.Width = 20;
			// 
			// columnHeaderMappingName
			// 
			this.columnHeaderMappingName.Text = "Mapping Name";
			this.columnHeaderMappingName.Width = 105;
			// 
			// columnHeaderWindowsAccount
			// 
			this.columnHeaderWindowsAccount.Text = "Windows Account";
			this.columnHeaderWindowsAccount.Width = 195;
			// 
			// imageListState
			// 
			this.imageListState.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListState.ImageStream")));
			this.imageListState.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListState.Images.SetKeyName(0, "Disabled.png");
			this.imageListState.Images.SetKeyName(1, "Enabled.png");
			// 
			// labelInstructions
			// 
			this.labelInstructions.Location = new System.Drawing.Point(15, 12);
			this.labelInstructions.Name = "labelInstructions";
			this.labelInstructions.Size = new System.Drawing.Size(570, 26);
			this.labelInstructions.TabIndex = 0;
			this.labelInstructions.Text = resources.GetString("labelInstructions.Text");
			this.labelInstructions.UseMnemonic = false;
			// 
			// buttonDelete
			// 
			this.buttonDelete.Location = new System.Drawing.Point(226, 310);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(75, 23);
			this.buttonDelete.TabIndex = 6;
			this.buttonDelete.Text = "De&lete...";
			this.buttonDelete.UseVisualStyleBackColor = true;
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// buttonAdd
			// 
			this.buttonAdd.Location = new System.Drawing.Point(121, 310);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(75, 23);
			this.buttonAdd.TabIndex = 5;
			this.buttonAdd.Text = "A&dd...";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			// 
			// buttonEditMap
			// 
			this.buttonEditMap.Location = new System.Drawing.Point(15, 310);
			this.buttonEditMap.Name = "buttonEditMap";
			this.buttonEditMap.Size = new System.Drawing.Size(75, 23);
			this.buttonEditMap.TabIndex = 4;
			this.buttonEditMap.Text = "&Edit Map...";
			this.buttonEditMap.UseVisualStyleBackColor = true;
			this.buttonEditMap.Click += new System.EventHandler(this.buttonEditMap_Click);
			// 
			// groupBoxIssuer
			// 
			this.groupBoxIssuer.Location = new System.Drawing.Point(352, 178);
			this.groupBoxIssuer.Name = "groupBoxIssuer";
			this.groupBoxIssuer.Size = new System.Drawing.Size(227, 80);
			this.groupBoxIssuer.TabIndex = 3;
			this.groupBoxIssuer.TabStop = false;
			this.groupBoxIssuer.Text = "Issuer";
			// 
			// groupBoxSubject
			// 
			this.groupBoxSubject.Location = new System.Drawing.Point(352, 50);
			this.groupBoxSubject.Name = "groupBoxSubject";
			this.groupBoxSubject.Size = new System.Drawing.Size(227, 122);
			this.groupBoxSubject.TabIndex = 2;
			this.groupBoxSubject.TabStop = false;
			this.groupBoxSubject.Text = "Subject";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(287, 397);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(368, 397);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonApply
			// 
			this.buttonApply.Enabled = false;
			this.buttonApply.Location = new System.Drawing.Point(449, 397);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(75, 23);
			this.buttonApply.TabIndex = 2;
			this.buttonApply.Text = "Apply";
			this.buttonApply.UseVisualStyleBackColor = true;
			this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
			// 
			// buttonHelp
			// 
			this.buttonHelp.Enabled = false;
			this.buttonHelp.Location = new System.Drawing.Point(530, 397);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(75, 23);
			this.buttonHelp.TabIndex = 3;
			this.buttonHelp.Text = "Help";
			this.buttonHelp.UseVisualStyleBackColor = true;
			// 
			// openFileDialogCertificate
			// 
			this.openFileDialogCertificate.DefaultExt = "*.cer";
			this.openFileDialogCertificate.Filter = "Certificate Import Files (*.cer)|*.cer|All Files|*.*\"";
			this.openFileDialogCertificate.Title = "Open";
			// 
			// AccountMappings
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(611, 427);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tabControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AccountMappings";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Account Mappings";
			this.tabControl.ResumeLayout(false);
			this.tabPage1to1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPage1to1;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.GroupBox groupBoxSubject;
		private System.Windows.Forms.Button buttonDelete;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.Button buttonEditMap;
		private System.Windows.Forms.GroupBox groupBoxIssuer;
		private System.Windows.Forms.OpenFileDialog openFileDialogCertificate;
		private System.Windows.Forms.Label labelInstructions;
		private System.Windows.Forms.ListView listViewOneToOne;
		private System.Windows.Forms.ColumnHeader columnHeaderState;
		private System.Windows.Forms.ColumnHeader columnHeaderMappingName;
		private System.Windows.Forms.ColumnHeader columnHeaderWindowsAccount;
		private System.Windows.Forms.ImageList imageListState;
	}
}