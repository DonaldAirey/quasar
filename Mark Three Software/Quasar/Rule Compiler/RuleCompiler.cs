/*************************************************************************************************************************
*
*	File:			RuleCompiler.cs
*	Description:	This dialog box is a primitive compile environment for rules.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Rule
{

	using Shadows.Quasar.Rules;
	using Shadows.Quasar.Common;
	using System;
	using System.CodeDom;
	using System.CodeDom.Compiler;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Reflection;
	using System.Threading;
	using System.Windows.Forms;
	using Microsoft.CSharp;

	/// <summary>
	/// Summary description for RuleCompiler.
	/// </summary>
	public class RuleCompiler : System.Windows.Forms.Form
	{

		private string sourceFileName;
		private static CompilerResults compilerResults;
		private System.Windows.Forms.TextBox textBoxSourceCode;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.TextBox textBoxOutput;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItemBuildRules;
		private System.Windows.Forms.MenuItem menuItemOpen;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuItemNew;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItemExit;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItemRun;
		private System.Windows.Forms.MenuItem menuItemSaveAs;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		public static CompilerResults CompilerResults {get {return RuleCompiler.compilerResults;}}

		static RuleCompiler()
		{

			RuleCompiler.compilerResults = null;

		}
		
		public RuleCompiler()
		{

			// Initialize the source file name.
			sourceFileName = null;

			// Required for Windows Form Designer support
			InitializeComponent();

		}

		#region Dispose Method
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBoxSourceCode = new System.Windows.Forms.TextBox();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.textBoxOutput = new System.Windows.Forms.TextBox();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemOpen = new System.Windows.Forms.MenuItem();
			this.menuItemSave = new System.Windows.Forms.MenuItem();
			this.menuItemSaveAs = new System.Windows.Forms.MenuItem();
			this.menuItemNew = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItemExit = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItemBuildRules = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItemRun = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// textBoxSourceCode
			// 
			this.textBoxSourceCode.AcceptsReturn = true;
			this.textBoxSourceCode.AcceptsTab = true;
			this.textBoxSourceCode.Dock = System.Windows.Forms.DockStyle.Top;
			this.textBoxSourceCode.Multiline = true;
			this.textBoxSourceCode.Name = "textBoxSourceCode";
			this.textBoxSourceCode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxSourceCode.Size = new System.Drawing.Size(648, 168);
			this.textBoxSourceCode.TabIndex = 0;
			this.textBoxSourceCode.Text = "";
			this.textBoxSourceCode.WordWrap = false;
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
			this.splitter1.Location = new System.Drawing.Point(0, 168);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(648, 3);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// textBoxOutput
			// 
			this.textBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxOutput.Location = new System.Drawing.Point(0, 171);
			this.textBoxOutput.Multiline = true;
			this.textBoxOutput.Name = "textBoxOutput";
			this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxOutput.Size = new System.Drawing.Size(648, 154);
			this.textBoxOutput.TabIndex = 2;
			this.textBoxOutput.Text = "";
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItem1,
																					 this.menuItem5,
																					 this.menuItem3});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemOpen,
																					  this.menuItemSave,
																					  this.menuItemSaveAs,
																					  this.menuItemNew,
																					  this.menuItem2,
																					  this.menuItemExit});
			this.menuItem1.Text = "&File";
			// 
			// menuItemOpen
			// 
			this.menuItemOpen.Index = 0;
			this.menuItemOpen.Text = "&Open";
			this.menuItemOpen.Click += new System.EventHandler(this.menuItemOpen_Click);
			// 
			// menuItemSave
			// 
			this.menuItemSave.Index = 1;
			this.menuItemSave.Text = "&Save";
			this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
			// 
			// menuItemSaveAs
			// 
			this.menuItemSaveAs.Index = 2;
			this.menuItemSaveAs.Text = "&Save As...";
			this.menuItemSaveAs.Click += new System.EventHandler(this.menuItemSaveAs_Click);
			// 
			// menuItemNew
			// 
			this.menuItemNew.Index = 3;
			this.menuItemNew.Text = "&New";
			this.menuItemNew.Click += new System.EventHandler(this.menuItemNew_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 4;
			this.menuItem2.Text = "-";
			// 
			// menuItemExit
			// 
			this.menuItemExit.Index = 5;
			this.menuItemExit.Text = "&Exit";
			this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 1;
			this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemBuildRules});
			this.menuItem5.Text = "&Build";
			// 
			// menuItemBuildRules
			// 
			this.menuItemBuildRules.Index = 0;
			this.menuItemBuildRules.Text = "&Build Rules";
			this.menuItemBuildRules.Click += new System.EventHandler(this.menuItemBuildRules_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemRun});
			this.menuItem3.Text = "&Debug";
			// 
			// menuItemRun
			// 
			this.menuItemRun.Index = 0;
			this.menuItemRun.Text = "&Run";
			this.menuItemRun.Click += new System.EventHandler(this.menuItemRun_Click);
			// 
			// RuleCompiler
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(648, 325);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.textBoxOutput,
																		  this.splitter1,
																		  this.textBoxSourceCode});
			this.Menu = this.mainMenu;
			this.Name = "RuleCompiler";
			this.Text = "Rules";
			this.ResumeLayout(false);

		}
		#endregion

		private void menuItemExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void menuItemNew_Click(object sender, System.EventArgs e)
		{

			CodeNamespace codeNamespace = RuleBuilder.CreateNamespace();
		
			CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider();

			CodeGeneratorOptions codeGeneratorOptions = new CodeGeneratorOptions();
			codeGeneratorOptions.BracingStyle = "C";
			StringWriter stringWriter = new StringWriter();
			ICodeGenerator codeGenerator = new CSharpCodeProvider().CreateGenerator();
			codeGenerator.GenerateCodeFromNamespace(codeNamespace, stringWriter, codeGeneratorOptions);

			this.textBoxSourceCode.Text = stringWriter.ToString();

		}

		private void menuItemBuildRules_Click(object sender, System.EventArgs e)
		{
		
			RuleCompiler.compilerResults = Language.Compile(Language.StandardParameters(), this.textBoxSourceCode.Text);

			this.textBoxOutput.Clear();
			if (RuleCompiler.compilerResults.Errors.Count == 0)
				this.textBoxOutput.Text += "Build: succeeded";
			else
				foreach (CompilerError compileError in RuleCompiler.compilerResults.Errors)
					this.textBoxOutput.Text += compileError + "\r\n";

		}

		private void menuItemRun_Click(object sender, System.EventArgs e)
		{

			ThreadArgument threadArgument = new ThreadArgument(new ThreadHandler(Language.ExecutionCommand),
				RuleCompiler.compilerResults);
			Thread thread = new Thread(new ThreadStart(threadArgument.StartThread));
			thread.IsBackground = true;
			thread.Start();

		}

		private void menuItemSave_Click(object sender, System.EventArgs e)
		{

			if (sourceFileName == null)
			{

				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.AddExtension = true;
				saveFileDialog.DefaultExt = ".cs";
				saveFileDialog.OverwritePrompt = true;
				saveFileDialog.Title = "Save Rule";
				if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
					return;
				sourceFileName = saveFileDialog.FileName;

			}
			
			FileStream fileStream = new FileStream(sourceFileName, FileMode.OpenOrCreate, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(this.textBoxSourceCode.Text);
			streamWriter.Close();

		}

		private void menuItemSaveAs_Click(object sender, System.EventArgs e)
		{

			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.AddExtension = true;
			saveFileDialog.DefaultExt = ".cs";
			saveFileDialog.Filter = "Rules (*.cs;*.vb)|*.cs;*.vb";
			saveFileDialog.FilterIndex = 0;
			saveFileDialog.OverwritePrompt = true;
			saveFileDialog.Title = "Save Rule As";
			if (saveFileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			sourceFileName = saveFileDialog.FileName;

			FileStream fileStream = new FileStream(sourceFileName, FileMode.OpenOrCreate, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.Write(this.textBoxSourceCode.Text);
			streamWriter.Close();

		}

		private void menuItemOpen_Click(object sender, System.EventArgs e)
		{

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Open Rule";
			openFileDialog.Filter = "Rules (*.cs;*.vb)|*.cs;*.vb";
			openFileDialog.FilterIndex = 0;
			if (openFileDialog.ShowDialog(this) != DialogResult.OK)
				return;

			sourceFileName = openFileDialog.FileName;
			FileStream fileStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read);
			StreamReader streamReader = new StreamReader(fileStream);
			this.textBoxSourceCode.Clear();
			this.textBoxSourceCode.Text = streamReader.ReadToEnd();
			streamReader.Close();

		}

	}

}
