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
            this.comboBoxEntityType = new System.Windows.Forms.ComboBox();
            this.numericUpDownLane = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownX = new System.Windows.Forms.NumericUpDown();
            this.buttonToggleNight = new System.Windows.Forms.Button();
            this.labelLane = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanvas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLane)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownX)).BeginInit();
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
            // comboBoxEntityType
            //
            this.comboBoxEntityType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEntityType.Items.AddRange(new object[] {
            "Car",
            "Bus",
            "EmergencyVehicle",
            "Pedestrian",
            "Bicycle",
            "BusStation",
            "RoadHazard"});
            this.comboBoxEntityType.Location = new System.Drawing.Point(12, 520);
            this.comboBoxEntityType.Name = "comboBoxEntityType";
            this.comboBoxEntityType.Size = new System.Drawing.Size(120, 23);
            this.comboBoxEntityType.TabIndex = 6;
            this.comboBoxEntityType.SelectedIndex = 0;
            //
            // labelLane
            //
            this.labelLane.AutoSize = true;
            this.labelLane.Location = new System.Drawing.Point(138, 524);
            this.labelLane.Name = "labelLane";
            this.labelLane.Size = new System.Drawing.Size(34, 15);
            this.labelLane.TabIndex = 7;
            this.labelLane.Text = "Lane:";
            //
            // numericUpDownLane
            //
            this.numericUpDownLane.Location = new System.Drawing.Point(176, 521);
            this.numericUpDownLane.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            this.numericUpDownLane.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numericUpDownLane.Name = "numericUpDownLane";
            this.numericUpDownLane.Size = new System.Drawing.Size(45, 23);
            this.numericUpDownLane.TabIndex = 8;
            //
            // labelX
            //
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(227, 524);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(17, 15);
            this.labelX.TabIndex = 9;
            this.labelX.Text = "X:";
            //
            // numericUpDownX
            //
            this.numericUpDownX.Location = new System.Drawing.Point(250, 521);
            this.numericUpDownX.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            this.numericUpDownX.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numericUpDownX.Name = "numericUpDownX";
            this.numericUpDownX.Size = new System.Drawing.Size(60, 23);
            this.numericUpDownX.TabIndex = 10;
            //
            // buttonAddEntity
            //
            this.buttonAddEntity.Location = new System.Drawing.Point(320, 520);
            this.buttonAddEntity.Name = "buttonAddEntity";
            this.buttonAddEntity.Size = new System.Drawing.Size(80, 25);
            this.buttonAddEntity.TabIndex = 1;
            this.buttonAddEntity.Text = "Add";
            this.buttonAddEntity.UseVisualStyleBackColor = true;
            this.buttonAddEntity.Click += new System.EventHandler(this.OnAddEntityClick);
            //
            // buttonDeleteEntity
            //
            this.buttonDeleteEntity.Location = new System.Drawing.Point(12, 550);
            this.buttonDeleteEntity.Name = "buttonDeleteEntity";
            this.buttonDeleteEntity.Size = new System.Drawing.Size(100, 30);
            this.buttonDeleteEntity.TabIndex = 2;
            this.buttonDeleteEntity.Text = "Delete All";
            this.buttonDeleteEntity.UseVisualStyleBackColor = true;
            this.buttonDeleteEntity.Click += new System.EventHandler(this.OnDeleteEntityClick);
            //
            // buttonSave
            //
            this.buttonSave.Location = new System.Drawing.Point(118, 550);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(100, 30);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.OnSaveClick);
            //
            // buttonLoad
            //
            this.buttonLoad.Location = new System.Drawing.Point(224, 550);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(100, 30);
            this.buttonLoad.TabIndex = 4;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.OnLoadClick);
            //
            // buttonToggleNight
            //
            this.buttonToggleNight.Location = new System.Drawing.Point(330, 550);
            this.buttonToggleNight.Name = "buttonToggleNight";
            this.buttonToggleNight.Size = new System.Drawing.Size(100, 30);
            this.buttonToggleNight.TabIndex = 11;
            this.buttonToggleNight.Text = "Toggle Night";
            this.buttonToggleNight.UseVisualStyleBackColor = true;
            this.buttonToggleNight.Click += new System.EventHandler(this.OnToggleNightClick);
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
            this.ClientSize = new System.Drawing.Size(800, 595);
            this.Controls.Add(this.labelAnalytics);
            this.Controls.Add(this.buttonToggleNight);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonDeleteEntity);
            this.Controls.Add(this.numericUpDownX);
            this.Controls.Add(this.labelX);
            this.Controls.Add(this.numericUpDownLane);
            this.Controls.Add(this.labelLane);
            this.Controls.Add(this.comboBoxEntityType);
            this.Controls.Add(this.buttonAddEntity);
            this.Controls.Add(this.pictureBoxCanvas);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Traffic Simulator";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanvas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLane)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownX)).EndInit();
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
        private System.Windows.Forms.ComboBox comboBoxEntityType;
        private System.Windows.Forms.NumericUpDown numericUpDownLane;
        private System.Windows.Forms.NumericUpDown numericUpDownX;
        private System.Windows.Forms.Button buttonToggleNight;
        private System.Windows.Forms.Label labelLane;
        private System.Windows.Forms.Label labelX;
    }
}
