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
            this.comboBoxRoad = new System.Windows.Forms.ComboBox();
            this.numericUpDownLane = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownOffset = new System.Windows.Forms.NumericUpDown();
            this.buttonToggleNight = new System.Windows.Forms.Button();
            this.buttonRun = new System.Windows.Forms.Button();
            this.labelLane = new System.Windows.Forms.Label();
            this.labelOffset = new System.Windows.Forms.Label();
            this.labelStartLight = new System.Windows.Forms.Label();
            this.comboBoxStartLight = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanvas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLane)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOffset)).BeginInit();
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
            this.pictureBoxCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBoxCanvas_MouseDown);
            this.pictureBoxCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBoxCanvas_MouseMove);
            this.pictureBoxCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBoxCanvas_MouseUp);
            this.pictureBoxCanvas.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PictureBoxCanvas_MouseWheel);
            //
            // comboBoxRoad
            //
            this.comboBoxRoad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRoad.Items.AddRange(new object[] {
            "1 - West",
            "2 - North",
            "3 - East",
            "4 - South"});
            this.comboBoxRoad.Location = new System.Drawing.Point(12, 520);
            this.comboBoxRoad.Name = "comboBoxRoad";
            this.comboBoxRoad.Size = new System.Drawing.Size(110, 23);
            this.comboBoxRoad.TabIndex = 6;
            this.comboBoxRoad.SelectedIndex = 0;
            //
            // labelLane
            //
            this.labelLane.AutoSize = true;
            this.labelLane.Location = new System.Drawing.Point(128, 524);
            this.labelLane.Name = "labelLane";
            this.labelLane.Size = new System.Drawing.Size(34, 15);
            this.labelLane.TabIndex = 7;
            this.labelLane.Text = "Lane:";
            //
            // numericUpDownLane
            //
            this.numericUpDownLane.Location = new System.Drawing.Point(166, 521);
            this.numericUpDownLane.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numericUpDownLane.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numericUpDownLane.Name = "numericUpDownLane";
            this.numericUpDownLane.Size = new System.Drawing.Size(42, 23);
            this.numericUpDownLane.TabIndex = 8;
            //
            // labelOffset
            //
            this.labelOffset.AutoSize = true;
            this.labelOffset.Location = new System.Drawing.Point(214, 524);
            this.labelOffset.Name = "labelOffset";
            this.labelOffset.Size = new System.Drawing.Size(45, 15);
            this.labelOffset.TabIndex = 9;
            this.labelOffset.Text = "Offset:";
            //
            // numericUpDownOffset
            //
            this.numericUpDownOffset.Increment = new decimal(new int[] { 20, 0, 0, 0 });
            this.numericUpDownOffset.Location = new System.Drawing.Point(262, 521);
            this.numericUpDownOffset.Maximum = new decimal(new int[] { 700, 0, 0, 0 });
            this.numericUpDownOffset.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numericUpDownOffset.Name = "numericUpDownOffset";
            this.numericUpDownOffset.Size = new System.Drawing.Size(60, 23);
            this.numericUpDownOffset.TabIndex = 10;
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
            this.comboBoxEntityType.Location = new System.Drawing.Point(330, 520);
            this.comboBoxEntityType.Name = "comboBoxEntityType";
            this.comboBoxEntityType.Size = new System.Drawing.Size(130, 23);
            this.comboBoxEntityType.TabIndex = 11;
            this.comboBoxEntityType.SelectedIndex = 0;
            //
            // buttonAddEntity
            //
            this.buttonAddEntity.Location = new System.Drawing.Point(468, 519);
            this.buttonAddEntity.Name = "buttonAddEntity";
            this.buttonAddEntity.Size = new System.Drawing.Size(70, 25);
            this.buttonAddEntity.TabIndex = 1;
            this.buttonAddEntity.Text = "+ Add";
            this.buttonAddEntity.UseVisualStyleBackColor = true;
            this.buttonAddEntity.Click += new System.EventHandler(this.OnAddEntityClick);
            //
            // labelStartLight
            //
            this.labelStartLight.AutoSize = true;
            this.labelStartLight.Location = new System.Drawing.Point(548, 524);
            this.labelStartLight.Name = "labelStartLight";
            this.labelStartLight.Size = new System.Drawing.Size(66, 15);
            this.labelStartLight.TabIndex = 14;
            this.labelStartLight.Text = "Start light:";
            //
            // comboBoxStartLight
            //
            this.comboBoxStartLight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxStartLight.Items.AddRange(new object[] {
            "1 - West",
            "2 - North",
            "3 - East",
            "4 - South"});
            this.comboBoxStartLight.Location = new System.Drawing.Point(620, 520);
            this.comboBoxStartLight.Name = "comboBoxStartLight";
            this.comboBoxStartLight.Size = new System.Drawing.Size(110, 23);
            this.comboBoxStartLight.TabIndex = 15;
            this.comboBoxStartLight.SelectedIndex = 1;
            this.comboBoxStartLight.SelectedIndexChanged += new System.EventHandler(this.OnStartLightChanged);
            //
            // buttonRun
            //
            this.buttonRun.BackColor = System.Drawing.Color.FromArgb(47, 125, 79);
            this.buttonRun.ForeColor = System.Drawing.Color.White;
            this.buttonRun.Location = new System.Drawing.Point(12, 552);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(90, 30);
            this.buttonRun.TabIndex = 12;
            this.buttonRun.Text = "▶ Run";
            this.buttonRun.UseVisualStyleBackColor = false;
            this.buttonRun.Click += new System.EventHandler(this.OnRunClick);
            //
            // buttonDeleteEntity
            //
            this.buttonDeleteEntity.Location = new System.Drawing.Point(108, 552);
            this.buttonDeleteEntity.Name = "buttonDeleteEntity";
            this.buttonDeleteEntity.Size = new System.Drawing.Size(90, 30);
            this.buttonDeleteEntity.TabIndex = 2;
            this.buttonDeleteEntity.Text = "Delete All";
            this.buttonDeleteEntity.UseVisualStyleBackColor = true;
            this.buttonDeleteEntity.Click += new System.EventHandler(this.OnDeleteEntityClick);
            //
            // buttonSave
            //
            this.buttonSave.Location = new System.Drawing.Point(204, 552);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(80, 30);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.OnSaveClick);
            //
            // buttonLoad
            //
            this.buttonLoad.Location = new System.Drawing.Point(290, 552);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(80, 30);
            this.buttonLoad.TabIndex = 4;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.OnLoadClick);
            //
            // buttonToggleNight
            //
            this.buttonToggleNight.Location = new System.Drawing.Point(376, 552);
            this.buttonToggleNight.Name = "buttonToggleNight";
            this.buttonToggleNight.Size = new System.Drawing.Size(90, 30);
            this.buttonToggleNight.TabIndex = 5;
            this.buttonToggleNight.Text = "☾ Night";
            this.buttonToggleNight.UseVisualStyleBackColor = true;
            this.buttonToggleNight.Click += new System.EventHandler(this.OnToggleNightClick);
            //
            // labelAnalytics
            //
            this.labelAnalytics.AutoSize = true;
            this.labelAnalytics.Location = new System.Drawing.Point(476, 559);
            this.labelAnalytics.Name = "labelAnalytics";
            this.labelAnalytics.Size = new System.Drawing.Size(0, 15);
            this.labelAnalytics.TabIndex = 13;
            //
            // MainForm
            //
            this.ClientSize = new System.Drawing.Size(800, 595);
            this.Controls.Add(this.labelAnalytics);
            this.Controls.Add(this.buttonToggleNight);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonDeleteEntity);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.buttonAddEntity);
            this.Controls.Add(this.comboBoxStartLight);
            this.Controls.Add(this.labelStartLight);
            this.Controls.Add(this.comboBoxEntityType);
            this.Controls.Add(this.numericUpDownOffset);
            this.Controls.Add(this.labelOffset);
            this.Controls.Add(this.numericUpDownLane);
            this.Controls.Add(this.labelLane);
            this.Controls.Add(this.comboBoxRoad);
            this.Controls.Add(this.pictureBoxCanvas);
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.Text = "Traffic Simulator";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanvas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLane)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownOffset)).EndInit();
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
        private System.Windows.Forms.ComboBox comboBoxRoad;
        private System.Windows.Forms.NumericUpDown numericUpDownLane;
        private System.Windows.Forms.NumericUpDown numericUpDownOffset;
        private System.Windows.Forms.Button buttonToggleNight;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Label labelLane;
        private System.Windows.Forms.Label labelOffset;
        private System.Windows.Forms.Label labelStartLight;
        private System.Windows.Forms.ComboBox comboBoxStartLight;
    }
}
