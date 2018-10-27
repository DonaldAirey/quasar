namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.Threading;
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for FormObjectProperties.
	/// </summary>
	public class FormObjectProperties : System.Windows.Forms.Form
	{

		// Private Members
		protected System.ComponentModel.Container components;
		protected System.Windows.Forms.Button buttonOK;
		protected System.Windows.Forms.Button buttonCancel;
		protected System.Windows.Forms.Button buttonApply;
		protected System.Windows.Forms.TabPage tabPageGeneral;
		protected System.Windows.Forms.Label label3;
		protected System.Windows.Forms.Label label2;
		protected System.Windows.Forms.CheckBox checkBoxDeleted;
		protected System.Windows.Forms.Label labelAttributes;
		protected System.Windows.Forms.Label labelTypeText;
		protected System.Windows.Forms.TextBox textBoxDescription;
		protected System.Windows.Forms.Label label1;
		protected System.Windows.Forms.Label labelTypePrompt;
		protected System.Windows.Forms.TextBox textBoxName;
		protected System.Windows.Forms.TabControl tabControl;
		protected System.Windows.Forms.PictureBox pictureBox;
		protected System.Windows.Forms.Button buttonHelp;

		/// <summary>
		/// Dialog box for maintaining objects.
		/// </summary>
		public FormObjectProperties()
		{

			// Used by the designer to add components to components.
			components = null;

			// Initializer for the components managed by the designer.
			InitializeComponent();

		}

		#region Standard Dispose method
		/// <summary>
		/// Releases managed resources when the ApprasalViewer is destroyed.
		/// </summary>
		/// <param name="disposing">Indicates whether the object is being destroyed</param>
		protected override void Dispose(bool disposing)
		{

			// Remove any components that have been added in.
			if (disposing && components != null)
				components.Dispose();

			// Call the base class to remove the rest of the resources.
			base.Dispose(disposing);

		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormObjectProperties));
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonApply = new System.Windows.Forms.Button();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.tabPageGeneral = new System.Windows.Forms.TabPage();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBoxDeleted = new System.Windows.Forms.CheckBox();
			this.labelAttributes = new System.Windows.Forms.Label();
			this.labelTypeText = new System.Windows.Forms.Label();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelTypePrompt = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPageGeneral.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.AccessibleDescription = resources.GetString("buttonOK.AccessibleDescription");
			this.buttonOK.AccessibleName = resources.GetString("buttonOK.AccessibleName");
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonOK.Anchor")));
			this.buttonOK.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonOK.BackgroundImage")));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonOK.Dock")));
			this.buttonOK.Enabled = ((bool)(resources.GetObject("buttonOK.Enabled")));
			this.buttonOK.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonOK.FlatStyle")));
			this.buttonOK.Font = ((System.Drawing.Font)(resources.GetObject("buttonOK.Font")));
			this.buttonOK.Image = ((System.Drawing.Image)(resources.GetObject("buttonOK.Image")));
			this.buttonOK.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonOK.ImageAlign")));
			this.buttonOK.ImageIndex = ((int)(resources.GetObject("buttonOK.ImageIndex")));
			this.buttonOK.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonOK.ImeMode")));
			this.buttonOK.Location = ((System.Drawing.Point)(resources.GetObject("buttonOK.Location")));
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonOK.RightToLeft")));
			this.buttonOK.Size = ((System.Drawing.Size)(resources.GetObject("buttonOK.Size")));
			this.buttonOK.TabIndex = ((int)(resources.GetObject("buttonOK.TabIndex")));
			this.buttonOK.Text = resources.GetString("buttonOK.Text");
			this.buttonOK.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonOK.TextAlign")));
			this.buttonOK.Visible = ((bool)(resources.GetObject("buttonOK.Visible")));
			// 
			// buttonCancel
			// 
			this.buttonCancel.AccessibleDescription = resources.GetString("buttonCancel.AccessibleDescription");
			this.buttonCancel.AccessibleName = resources.GetString("buttonCancel.AccessibleName");
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonCancel.Anchor")));
			this.buttonCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonCancel.BackgroundImage")));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonCancel.Dock")));
			this.buttonCancel.Enabled = ((bool)(resources.GetObject("buttonCancel.Enabled")));
			this.buttonCancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonCancel.FlatStyle")));
			this.buttonCancel.Font = ((System.Drawing.Font)(resources.GetObject("buttonCancel.Font")));
			this.buttonCancel.Image = ((System.Drawing.Image)(resources.GetObject("buttonCancel.Image")));
			this.buttonCancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonCancel.ImageAlign")));
			this.buttonCancel.ImageIndex = ((int)(resources.GetObject("buttonCancel.ImageIndex")));
			this.buttonCancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonCancel.ImeMode")));
			this.buttonCancel.Location = ((System.Drawing.Point)(resources.GetObject("buttonCancel.Location")));
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonCancel.RightToLeft")));
			this.buttonCancel.Size = ((System.Drawing.Size)(resources.GetObject("buttonCancel.Size")));
			this.buttonCancel.TabIndex = ((int)(resources.GetObject("buttonCancel.TabIndex")));
			this.buttonCancel.Text = resources.GetString("buttonCancel.Text");
			this.buttonCancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonCancel.TextAlign")));
			this.buttonCancel.Visible = ((bool)(resources.GetObject("buttonCancel.Visible")));
			// 
			// buttonApply
			// 
			this.buttonApply.AccessibleDescription = resources.GetString("buttonApply.AccessibleDescription");
			this.buttonApply.AccessibleName = resources.GetString("buttonApply.AccessibleName");
			this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonApply.Anchor")));
			this.buttonApply.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonApply.BackgroundImage")));
			this.buttonApply.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonApply.Dock")));
			this.buttonApply.Enabled = ((bool)(resources.GetObject("buttonApply.Enabled")));
			this.buttonApply.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonApply.FlatStyle")));
			this.buttonApply.Font = ((System.Drawing.Font)(resources.GetObject("buttonApply.Font")));
			this.buttonApply.Image = ((System.Drawing.Image)(resources.GetObject("buttonApply.Image")));
			this.buttonApply.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonApply.ImageAlign")));
			this.buttonApply.ImageIndex = ((int)(resources.GetObject("buttonApply.ImageIndex")));
			this.buttonApply.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonApply.ImeMode")));
			this.buttonApply.Location = ((System.Drawing.Point)(resources.GetObject("buttonApply.Location")));
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonApply.RightToLeft")));
			this.buttonApply.Size = ((System.Drawing.Size)(resources.GetObject("buttonApply.Size")));
			this.buttonApply.TabIndex = ((int)(resources.GetObject("buttonApply.TabIndex")));
			this.buttonApply.Text = resources.GetString("buttonApply.Text");
			this.buttonApply.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonApply.TextAlign")));
			this.buttonApply.Visible = ((bool)(resources.GetObject("buttonApply.Visible")));
			// 
			// buttonHelp
			// 
			this.buttonHelp.AccessibleDescription = resources.GetString("buttonHelp.AccessibleDescription");
			this.buttonHelp.AccessibleName = resources.GetString("buttonHelp.AccessibleName");
			this.buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonHelp.Anchor")));
			this.buttonHelp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonHelp.BackgroundImage")));
			this.buttonHelp.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonHelp.Dock")));
			this.buttonHelp.Enabled = ((bool)(resources.GetObject("buttonHelp.Enabled")));
			this.buttonHelp.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonHelp.FlatStyle")));
			this.buttonHelp.Font = ((System.Drawing.Font)(resources.GetObject("buttonHelp.Font")));
			this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
			this.buttonHelp.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonHelp.ImageAlign")));
			this.buttonHelp.ImageIndex = ((int)(resources.GetObject("buttonHelp.ImageIndex")));
			this.buttonHelp.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonHelp.ImeMode")));
			this.buttonHelp.Location = ((System.Drawing.Point)(resources.GetObject("buttonHelp.Location")));
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonHelp.RightToLeft")));
			this.buttonHelp.Size = ((System.Drawing.Size)(resources.GetObject("buttonHelp.Size")));
			this.buttonHelp.TabIndex = ((int)(resources.GetObject("buttonHelp.TabIndex")));
			this.buttonHelp.Text = resources.GetString("buttonHelp.Text");
			this.buttonHelp.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonHelp.TextAlign")));
			this.buttonHelp.Visible = ((bool)(resources.GetObject("buttonHelp.Visible")));
			// 
			// tabPageGeneral
			// 
			this.tabPageGeneral.AccessibleDescription = resources.GetString("tabPageGeneral.AccessibleDescription");
			this.tabPageGeneral.AccessibleName = resources.GetString("tabPageGeneral.AccessibleName");
			this.tabPageGeneral.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabPageGeneral.Anchor")));
			this.tabPageGeneral.AutoScroll = ((bool)(resources.GetObject("tabPageGeneral.AutoScroll")));
			this.tabPageGeneral.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("tabPageGeneral.AutoScrollMargin")));
			this.tabPageGeneral.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("tabPageGeneral.AutoScrollMinSize")));
			this.tabPageGeneral.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabPageGeneral.BackgroundImage")));
			this.tabPageGeneral.Controls.Add(this.pictureBox);
			this.tabPageGeneral.Controls.Add(this.label3);
			this.tabPageGeneral.Controls.Add(this.label2);
			this.tabPageGeneral.Controls.Add(this.checkBoxDeleted);
			this.tabPageGeneral.Controls.Add(this.labelAttributes);
			this.tabPageGeneral.Controls.Add(this.labelTypeText);
			this.tabPageGeneral.Controls.Add(this.textBoxDescription);
			this.tabPageGeneral.Controls.Add(this.label1);
			this.tabPageGeneral.Controls.Add(this.labelTypePrompt);
			this.tabPageGeneral.Controls.Add(this.textBoxName);
			this.tabPageGeneral.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabPageGeneral.Dock")));
			this.tabPageGeneral.Enabled = ((bool)(resources.GetObject("tabPageGeneral.Enabled")));
			this.tabPageGeneral.Font = ((System.Drawing.Font)(resources.GetObject("tabPageGeneral.Font")));
			this.tabPageGeneral.ImageIndex = ((int)(resources.GetObject("tabPageGeneral.ImageIndex")));
			this.tabPageGeneral.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabPageGeneral.ImeMode")));
			this.tabPageGeneral.Location = ((System.Drawing.Point)(resources.GetObject("tabPageGeneral.Location")));
			this.tabPageGeneral.Name = "tabPageGeneral";
			this.tabPageGeneral.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabPageGeneral.RightToLeft")));
			this.tabPageGeneral.Size = ((System.Drawing.Size)(resources.GetObject("tabPageGeneral.Size")));
			this.tabPageGeneral.TabIndex = ((int)(resources.GetObject("tabPageGeneral.TabIndex")));
			this.tabPageGeneral.Text = resources.GetString("tabPageGeneral.Text");
			this.tabPageGeneral.ToolTipText = resources.GetString("tabPageGeneral.ToolTipText");
			this.tabPageGeneral.Visible = ((bool)(resources.GetObject("tabPageGeneral.Visible")));
			// 
			// pictureBox
			// 
			this.pictureBox.AccessibleDescription = resources.GetString("pictureBox.AccessibleDescription");
			this.pictureBox.AccessibleName = resources.GetString("pictureBox.AccessibleName");
			this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("pictureBox.Anchor")));
			this.pictureBox.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox.BackgroundImage")));
			this.pictureBox.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("pictureBox.Dock")));
			this.pictureBox.Enabled = ((bool)(resources.GetObject("pictureBox.Enabled")));
			this.pictureBox.Font = ((System.Drawing.Font)(resources.GetObject("pictureBox.Font")));
			this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
			this.pictureBox.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("pictureBox.ImeMode")));
			this.pictureBox.Location = ((System.Drawing.Point)(resources.GetObject("pictureBox.Location")));
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("pictureBox.RightToLeft")));
			this.pictureBox.Size = ((System.Drawing.Size)(resources.GetObject("pictureBox.Size")));
			this.pictureBox.SizeMode = ((System.Windows.Forms.PictureBoxSizeMode)(resources.GetObject("pictureBox.SizeMode")));
			this.pictureBox.TabIndex = ((int)(resources.GetObject("pictureBox.TabIndex")));
			this.pictureBox.TabStop = false;
			this.pictureBox.Text = resources.GetString("pictureBox.Text");
			this.pictureBox.Visible = ((bool)(resources.GetObject("pictureBox.Visible")));
			// 
			// label3
			// 
			this.label3.AccessibleDescription = resources.GetString("label3.AccessibleDescription");
			this.label3.AccessibleName = resources.GetString("label3.AccessibleName");
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label3.Anchor")));
			this.label3.AutoSize = ((bool)(resources.GetObject("label3.AutoSize")));
			this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label3.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label3.Dock")));
			this.label3.Enabled = ((bool)(resources.GetObject("label3.Enabled")));
			this.label3.Font = ((System.Drawing.Font)(resources.GetObject("label3.Font")));
			this.label3.Image = ((System.Drawing.Image)(resources.GetObject("label3.Image")));
			this.label3.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.ImageAlign")));
			this.label3.ImageIndex = ((int)(resources.GetObject("label3.ImageIndex")));
			this.label3.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label3.ImeMode")));
			this.label3.Location = ((System.Drawing.Point)(resources.GetObject("label3.Location")));
			this.label3.Name = "label3";
			this.label3.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label3.RightToLeft")));
			this.label3.Size = ((System.Drawing.Size)(resources.GetObject("label3.Size")));
			this.label3.TabIndex = ((int)(resources.GetObject("label3.TabIndex")));
			this.label3.Text = resources.GetString("label3.Text");
			this.label3.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label3.TextAlign")));
			this.label3.Visible = ((bool)(resources.GetObject("label3.Visible")));
			// 
			// label2
			// 
			this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
			this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
			this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
			this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
			this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
			this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
			this.label2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.ImageAlign")));
			this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
			this.label2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label2.ImeMode")));
			this.label2.Location = ((System.Drawing.Point)(resources.GetObject("label2.Location")));
			this.label2.Name = "label2";
			this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label2.RightToLeft")));
			this.label2.Size = ((System.Drawing.Size)(resources.GetObject("label2.Size")));
			this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
			this.label2.Text = resources.GetString("label2.Text");
			this.label2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.TextAlign")));
			this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
			// 
			// checkBoxDeleted
			// 
			this.checkBoxDeleted.AccessibleDescription = resources.GetString("checkBoxDeleted.AccessibleDescription");
			this.checkBoxDeleted.AccessibleName = resources.GetString("checkBoxDeleted.AccessibleName");
			this.checkBoxDeleted.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("checkBoxDeleted.Anchor")));
			this.checkBoxDeleted.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("checkBoxDeleted.Appearance")));
			this.checkBoxDeleted.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("checkBoxDeleted.BackgroundImage")));
			this.checkBoxDeleted.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxDeleted.CheckAlign")));
			this.checkBoxDeleted.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("checkBoxDeleted.Dock")));
			this.checkBoxDeleted.Enabled = ((bool)(resources.GetObject("checkBoxDeleted.Enabled")));
			this.checkBoxDeleted.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("checkBoxDeleted.FlatStyle")));
			this.checkBoxDeleted.Font = ((System.Drawing.Font)(resources.GetObject("checkBoxDeleted.Font")));
			this.checkBoxDeleted.Image = ((System.Drawing.Image)(resources.GetObject("checkBoxDeleted.Image")));
			this.checkBoxDeleted.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxDeleted.ImageAlign")));
			this.checkBoxDeleted.ImageIndex = ((int)(resources.GetObject("checkBoxDeleted.ImageIndex")));
			this.checkBoxDeleted.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("checkBoxDeleted.ImeMode")));
			this.checkBoxDeleted.Location = ((System.Drawing.Point)(resources.GetObject("checkBoxDeleted.Location")));
			this.checkBoxDeleted.Name = "checkBoxDeleted";
			this.checkBoxDeleted.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("checkBoxDeleted.RightToLeft")));
			this.checkBoxDeleted.Size = ((System.Drawing.Size)(resources.GetObject("checkBoxDeleted.Size")));
			this.checkBoxDeleted.TabIndex = ((int)(resources.GetObject("checkBoxDeleted.TabIndex")));
			this.checkBoxDeleted.Text = resources.GetString("checkBoxDeleted.Text");
			this.checkBoxDeleted.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxDeleted.TextAlign")));
			this.checkBoxDeleted.Visible = ((bool)(resources.GetObject("checkBoxDeleted.Visible")));
			// 
			// labelAttributes
			// 
			this.labelAttributes.AccessibleDescription = resources.GetString("labelAttributes.AccessibleDescription");
			this.labelAttributes.AccessibleName = resources.GetString("labelAttributes.AccessibleName");
			this.labelAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("labelAttributes.Anchor")));
			this.labelAttributes.AutoSize = ((bool)(resources.GetObject("labelAttributes.AutoSize")));
			this.labelAttributes.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("labelAttributes.Dock")));
			this.labelAttributes.Enabled = ((bool)(resources.GetObject("labelAttributes.Enabled")));
			this.labelAttributes.Font = ((System.Drawing.Font)(resources.GetObject("labelAttributes.Font")));
			this.labelAttributes.Image = ((System.Drawing.Image)(resources.GetObject("labelAttributes.Image")));
			this.labelAttributes.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelAttributes.ImageAlign")));
			this.labelAttributes.ImageIndex = ((int)(resources.GetObject("labelAttributes.ImageIndex")));
			this.labelAttributes.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("labelAttributes.ImeMode")));
			this.labelAttributes.Location = ((System.Drawing.Point)(resources.GetObject("labelAttributes.Location")));
			this.labelAttributes.Name = "labelAttributes";
			this.labelAttributes.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("labelAttributes.RightToLeft")));
			this.labelAttributes.Size = ((System.Drawing.Size)(resources.GetObject("labelAttributes.Size")));
			this.labelAttributes.TabIndex = ((int)(resources.GetObject("labelAttributes.TabIndex")));
			this.labelAttributes.Text = resources.GetString("labelAttributes.Text");
			this.labelAttributes.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelAttributes.TextAlign")));
			this.labelAttributes.Visible = ((bool)(resources.GetObject("labelAttributes.Visible")));
			// 
			// labelTypeText
			// 
			this.labelTypeText.AccessibleDescription = resources.GetString("labelTypeText.AccessibleDescription");
			this.labelTypeText.AccessibleName = resources.GetString("labelTypeText.AccessibleName");
			this.labelTypeText.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("labelTypeText.Anchor")));
			this.labelTypeText.AutoSize = ((bool)(resources.GetObject("labelTypeText.AutoSize")));
			this.labelTypeText.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("labelTypeText.Dock")));
			this.labelTypeText.Enabled = ((bool)(resources.GetObject("labelTypeText.Enabled")));
			this.labelTypeText.Font = ((System.Drawing.Font)(resources.GetObject("labelTypeText.Font")));
			this.labelTypeText.Image = ((System.Drawing.Image)(resources.GetObject("labelTypeText.Image")));
			this.labelTypeText.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelTypeText.ImageAlign")));
			this.labelTypeText.ImageIndex = ((int)(resources.GetObject("labelTypeText.ImageIndex")));
			this.labelTypeText.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("labelTypeText.ImeMode")));
			this.labelTypeText.Location = ((System.Drawing.Point)(resources.GetObject("labelTypeText.Location")));
			this.labelTypeText.Name = "labelTypeText";
			this.labelTypeText.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("labelTypeText.RightToLeft")));
			this.labelTypeText.Size = ((System.Drawing.Size)(resources.GetObject("labelTypeText.Size")));
			this.labelTypeText.TabIndex = ((int)(resources.GetObject("labelTypeText.TabIndex")));
			this.labelTypeText.Text = resources.GetString("labelTypeText.Text");
			this.labelTypeText.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelTypeText.TextAlign")));
			this.labelTypeText.Visible = ((bool)(resources.GetObject("labelTypeText.Visible")));
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.AccessibleDescription = resources.GetString("textBoxDescription.AccessibleDescription");
			this.textBoxDescription.AccessibleName = resources.GetString("textBoxDescription.AccessibleName");
			this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("textBoxDescription.Anchor")));
			this.textBoxDescription.AutoSize = ((bool)(resources.GetObject("textBoxDescription.AutoSize")));
			this.textBoxDescription.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("textBoxDescription.BackgroundImage")));
			this.textBoxDescription.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("textBoxDescription.Dock")));
			this.textBoxDescription.Enabled = ((bool)(resources.GetObject("textBoxDescription.Enabled")));
			this.textBoxDescription.Font = ((System.Drawing.Font)(resources.GetObject("textBoxDescription.Font")));
			this.textBoxDescription.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("textBoxDescription.ImeMode")));
			this.textBoxDescription.Location = ((System.Drawing.Point)(resources.GetObject("textBoxDescription.Location")));
			this.textBoxDescription.MaxLength = ((int)(resources.GetObject("textBoxDescription.MaxLength")));
			this.textBoxDescription.Multiline = ((bool)(resources.GetObject("textBoxDescription.Multiline")));
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.PasswordChar = ((char)(resources.GetObject("textBoxDescription.PasswordChar")));
			this.textBoxDescription.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("textBoxDescription.RightToLeft")));
			this.textBoxDescription.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("textBoxDescription.ScrollBars")));
			this.textBoxDescription.Size = ((System.Drawing.Size)(resources.GetObject("textBoxDescription.Size")));
			this.textBoxDescription.TabIndex = ((int)(resources.GetObject("textBoxDescription.TabIndex")));
			this.textBoxDescription.Text = resources.GetString("textBoxDescription.Text");
			this.textBoxDescription.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("textBoxDescription.TextAlign")));
			this.textBoxDescription.Visible = ((bool)(resources.GetObject("textBoxDescription.Visible")));
			this.textBoxDescription.WordWrap = ((bool)(resources.GetObject("textBoxDescription.WordWrap")));
			// 
			// label1
			// 
			this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
			this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
			this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			// 
			// labelTypePrompt
			// 
			this.labelTypePrompt.AccessibleDescription = resources.GetString("labelTypePrompt.AccessibleDescription");
			this.labelTypePrompt.AccessibleName = resources.GetString("labelTypePrompt.AccessibleName");
			this.labelTypePrompt.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("labelTypePrompt.Anchor")));
			this.labelTypePrompt.AutoSize = ((bool)(resources.GetObject("labelTypePrompt.AutoSize")));
			this.labelTypePrompt.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("labelTypePrompt.Dock")));
			this.labelTypePrompt.Enabled = ((bool)(resources.GetObject("labelTypePrompt.Enabled")));
			this.labelTypePrompt.Font = ((System.Drawing.Font)(resources.GetObject("labelTypePrompt.Font")));
			this.labelTypePrompt.Image = ((System.Drawing.Image)(resources.GetObject("labelTypePrompt.Image")));
			this.labelTypePrompt.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelTypePrompt.ImageAlign")));
			this.labelTypePrompt.ImageIndex = ((int)(resources.GetObject("labelTypePrompt.ImageIndex")));
			this.labelTypePrompt.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("labelTypePrompt.ImeMode")));
			this.labelTypePrompt.Location = ((System.Drawing.Point)(resources.GetObject("labelTypePrompt.Location")));
			this.labelTypePrompt.Name = "labelTypePrompt";
			this.labelTypePrompt.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("labelTypePrompt.RightToLeft")));
			this.labelTypePrompt.Size = ((System.Drawing.Size)(resources.GetObject("labelTypePrompt.Size")));
			this.labelTypePrompt.TabIndex = ((int)(resources.GetObject("labelTypePrompt.TabIndex")));
			this.labelTypePrompt.Text = resources.GetString("labelTypePrompt.Text");
			this.labelTypePrompt.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelTypePrompt.TextAlign")));
			this.labelTypePrompt.Visible = ((bool)(resources.GetObject("labelTypePrompt.Visible")));
			// 
			// textBoxName
			// 
			this.textBoxName.AccessibleDescription = resources.GetString("textBoxName.AccessibleDescription");
			this.textBoxName.AccessibleName = resources.GetString("textBoxName.AccessibleName");
			this.textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("textBoxName.Anchor")));
			this.textBoxName.AutoSize = ((bool)(resources.GetObject("textBoxName.AutoSize")));
			this.textBoxName.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("textBoxName.BackgroundImage")));
			this.textBoxName.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("textBoxName.Dock")));
			this.textBoxName.Enabled = ((bool)(resources.GetObject("textBoxName.Enabled")));
			this.textBoxName.Font = ((System.Drawing.Font)(resources.GetObject("textBoxName.Font")));
			this.textBoxName.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("textBoxName.ImeMode")));
			this.textBoxName.Location = ((System.Drawing.Point)(resources.GetObject("textBoxName.Location")));
			this.textBoxName.MaxLength = ((int)(resources.GetObject("textBoxName.MaxLength")));
			this.textBoxName.Multiline = ((bool)(resources.GetObject("textBoxName.Multiline")));
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.PasswordChar = ((char)(resources.GetObject("textBoxName.PasswordChar")));
			this.textBoxName.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("textBoxName.RightToLeft")));
			this.textBoxName.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("textBoxName.ScrollBars")));
			this.textBoxName.Size = ((System.Drawing.Size)(resources.GetObject("textBoxName.Size")));
			this.textBoxName.TabIndex = ((int)(resources.GetObject("textBoxName.TabIndex")));
			this.textBoxName.Text = resources.GetString("textBoxName.Text");
			this.textBoxName.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("textBoxName.TextAlign")));
			this.textBoxName.Visible = ((bool)(resources.GetObject("textBoxName.Visible")));
			this.textBoxName.WordWrap = ((bool)(resources.GetObject("textBoxName.WordWrap")));
			// 
			// tabControl
			// 
			this.tabControl.AccessibleDescription = resources.GetString("tabControl.AccessibleDescription");
			this.tabControl.AccessibleName = resources.GetString("tabControl.AccessibleName");
			this.tabControl.Alignment = ((System.Windows.Forms.TabAlignment)(resources.GetObject("tabControl.Alignment")));
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("tabControl.Anchor")));
			this.tabControl.Appearance = ((System.Windows.Forms.TabAppearance)(resources.GetObject("tabControl.Appearance")));
			this.tabControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("tabControl.BackgroundImage")));
			this.tabControl.Controls.Add(this.tabPageGeneral);
			this.tabControl.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("tabControl.Dock")));
			this.tabControl.Enabled = ((bool)(resources.GetObject("tabControl.Enabled")));
			this.tabControl.Font = ((System.Drawing.Font)(resources.GetObject("tabControl.Font")));
			this.tabControl.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("tabControl.ImeMode")));
			this.tabControl.ItemSize = ((System.Drawing.Size)(resources.GetObject("tabControl.ItemSize")));
			this.tabControl.Location = ((System.Drawing.Point)(resources.GetObject("tabControl.Location")));
			this.tabControl.Name = "tabControl";
			this.tabControl.Padding = ((System.Drawing.Point)(resources.GetObject("tabControl.Padding")));
			this.tabControl.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("tabControl.RightToLeft")));
			this.tabControl.SelectedIndex = 0;
			this.tabControl.ShowToolTips = ((bool)(resources.GetObject("tabControl.ShowToolTips")));
			this.tabControl.Size = ((System.Drawing.Size)(resources.GetObject("tabControl.Size")));
			this.tabControl.TabIndex = ((int)(resources.GetObject("tabControl.TabIndex")));
			this.tabControl.Text = resources.GetString("tabControl.Text");
			this.tabControl.Visible = ((bool)(resources.GetObject("tabControl.Visible")));
			// 
			// FormObjectProperties
			// 
			this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
			this.AccessibleName = resources.GetString("$this.AccessibleName");
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.buttonApply);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.tabControl);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimizeBox = false;
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "FormObjectProperties";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.ShowInTaskbar = false;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.TransparencyKey = System.Drawing.Color.Aqua;
			this.tabPageGeneral.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Shows a dialog box for maintaining an object or fund.
		/// </summary>
		/// <param name="objectId">Primary identifier for the object.</param>
		public virtual void Show(MarkThree.Guardian.Object guardianObject)
		{

			try
			{

				// Make sure locks are not nested.
				Debug.Assert(!ClientMarketData.IsLocked);

				// Lock the tables needed for the dialog.
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TypeLock.AcquireReaderLock(CommonTimeout.LockWait);
				
				// Find the object in the data model.
				ClientMarketData.ObjectRow objectRow = ClientMarketData.Object.FindByObjectId(guardianObject.ObjectId);
				if (objectRow == null)
					throw new Exception("Some else has deleted this object.");

				// General Tab
				this.textBoxName.Text = objectRow.Name;
				this.labelTypeText.Text = objectRow.TypeRow.Description;
				this.textBoxDescription.Text = (objectRow.IsDescriptionNull()) ? string.Empty : objectRow.Description;
				this.pictureBox.Image = guardianObject.Image32x32;

			}
			finally
			{

				// Release the tables used for the dialog.
				if (ClientMarketData.ObjectLock.IsReaderLockHeld) ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.TypeLock.IsReaderLockHeld) ClientMarketData.TypeLock.ReleaseReaderLock();

				// Make sure all locks have been released
				Debug.Assert(!ClientMarketData.IsLocked);

			}

			// Display the dialog.
			ShowDialog();

		}

	}

}
