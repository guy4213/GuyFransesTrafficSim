namespace TrafficSimulator
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pictureBoxCanvas = new System.Windows.Forms.PictureBox();
            this.buttonAddEntity = new System.Windows.Forms.Button();
            this.buttonDeleteEntity = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.labelAnalytics = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanvas)).BeginInit();
            this.SuspendLayout();
            //
            // pictureBoxCanvas
            //
            this.pictureBoxCanvas.BackColor = System.Drawing.Color.White;
            this.pictureBoxCanvas.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxCanvas.Name = "pictureBoxCanvas";
            this.pictureBoxCanvas.Size = new System.Drawing.Size(760, 500);
            this.pictureBoxCanvas.TabIndex = 0;
            this.pictureBoxCanvas.TabStop = false;
            this.pictureBoxCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            //
            // buttonAddEntity
            //
            this.buttonAddEntity.Location = new System.Drawing.Point(12, 520);
            this.buttonAddEntity.Name = "buttonAddEntity";
            this.buttonAddEntity.Size = new System.Drawing.Size(100, 30);
            this.buttonAddEntity.TabIndex = 1;
            this.buttonAddEntity.Text = "Add Entity";
            this.buttonAddEntity.UseVisualStyleBackColor = true;
            this.buttonAddEntity.Click += new System.EventHandler(this.OnAddEntityClick);
            //
            // buttonDeleteEntity
            //
            this.buttonDeleteEntity.Location = new System.Drawing.Point(118, 520);
            this.buttonDeleteEntity.Name = "buttonDeleteEntity";
            this.buttonDeleteEntity.Size = new System.Drawing.Size(100, 30);
            this.buttonDeleteEntity.TabIndex = 2;
            this.buttonDeleteEntity.Text = "Delete Entity";
            this.buttonDeleteEntity.UseVisualStyleBackColor = true;
            this.buttonDeleteEntity.Click += new System.EventHandler(this.OnDeleteEntityClick);
            //
            // buttonSave
            //
            this.buttonSave.Location = new System.Drawing.Point(224, 520);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(100, 30);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.OnSaveClick);
            //
            // buttonLoad
            //
            this.buttonLoad.Location = new System.Drawing.Point(330, 520);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(100, 30);
            this.buttonLoad.TabIndex = 4;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.OnLoadClick);
            //
            // labelAnalytics
            //
            this.labelAnalytics.AutoSize = true;
            this.labelAnalytics.Location = new System.Drawing.Point(440, 526);
            this.labelAnalytics.Name = "labelAnalytics";
            this.labelAnalytics.Size = new System.Drawing.Size(0, 15);
            this.labelAnalytics.TabIndex = 5;
            //
            // MainForm
            //
            this.ClientSize = new System.Drawing.Size(800, 570);
            this.Controls.Add(this.labelAnalytics);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonDeleteEntity);
            this.Controls.Add(this.buttonAddEntity);
            this.Controls.Add(this.pictureBoxCanvas);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Traffic Simulator";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanvas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxCanvas;
        private System.Windows.Forms.Button buttonAddEntity;
        private System.Windows.Forms.Button buttonDeleteEntity;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Label labelAnalytics;
    }
}
