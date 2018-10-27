namespace MarkThree.Guardian.Server
{
    partial class ESignal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ESignal));
            this.dataManager = new AxDBCCTRLLib.AxDataManager();
            ((System.ComponentModel.ISupportInitialize)(this.dataManager)).BeginInit();

            this.SuspendLayout();
            // 
            // dataManager
            // 
            this.dataManager.Enabled = false;
            this.dataManager.Location = new System.Drawing.Point(0, 0);
            this.dataManager.Name = "dataManager";
            this.dataManager.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("dataManager.OcxState")));
            this.dataManager.Size = new System.Drawing.Size(100, 100);
            this.dataManager.Visible = true;
            this.dataManager.TabStop = false;
            this.dataManager.Disconnected += new AxDBCCTRLLib._IDataManagerEvents_DisconnectedEventHandler(this.DMDisconnected);
            this.dataManager.UpdateInternationalLong += new AxDBCCTRLLib._IDataManagerEvents_UpdateInternationalLongEventHandler(this.DMUpdateInternationalLong);
            this.dataManager.Connected += new System.EventHandler(this.DMConnected);

            // 
            // ESignal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(0F, 0F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Visible = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = true;
            this.Controls.Add(this.dataManager);
            this.Name = "ESignal";
            this.Text = "ESignal";
            this.Load += new System.EventHandler(this.ESignal_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ESignal_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataManager)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private AxDBCCTRLLib.AxDataManager dataManager;
    }
}