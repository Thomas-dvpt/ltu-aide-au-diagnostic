namespace TiaExplorePLCH
{
    partial class TiaPortalStationSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TiaPortalStationSelection));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BpSelectCurrentStation = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CurrentStationsSelection = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BpSelectCurrentStation);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.CurrentStationsSelection);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(591, 112);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PLC Station selection";
            // 
            // BpSelectCurrentStation
            // 
            this.BpSelectCurrentStation.Enabled = false;
            this.BpSelectCurrentStation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BpSelectCurrentStation.Location = new System.Drawing.Point(240, 68);
            this.BpSelectCurrentStation.Name = "BpSelectCurrentStation";
            this.BpSelectCurrentStation.Size = new System.Drawing.Size(135, 28);
            this.BpSelectCurrentStation.TabIndex = 4;
            this.BpSelectCurrentStation.Text = "Validate selection";
            this.BpSelectCurrentStation.UseVisualStyleBackColor = true;
            this.BpSelectCurrentStation.Click += new System.EventHandler(this.BpSelectCurrentStation_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Current stations :";
            // 
            // CurrentStationsSelection
            // 
            this.CurrentStationsSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CurrentStationsSelection.Enabled = false;
            this.CurrentStationsSelection.FormattingEnabled = true;
            this.CurrentStationsSelection.Location = new System.Drawing.Point(140, 30);
            this.CurrentStationsSelection.Name = "CurrentStationsSelection";
            this.CurrentStationsSelection.Size = new System.Drawing.Size(420, 24);
            this.CurrentStationsSelection.TabIndex = 1;
            // 
            // TiaPortalStationSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 136);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TiaPortalStationSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TIA Portal Station selection";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.TiaPortalStationSelection_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox CurrentStationsSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BpSelectCurrentStation;
    }
}