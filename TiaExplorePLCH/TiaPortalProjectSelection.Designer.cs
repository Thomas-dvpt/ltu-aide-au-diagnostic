namespace TiaExplorePLCH
{
    partial class TiaPortalProjectSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TiaPortalProjectSelection));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.BpSelectCurrentProject = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CurrentProjectsSelection = new System.Windows.Forms.ComboBox();
            this.openFileDialogTIAPortalProject = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.BpSelectCurrentProject);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.CurrentProjectsSelection);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(1033, 154);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Current instance of TIA Portal";
            // 
            // BpSelectCurrentProject
            // 
            this.BpSelectCurrentProject.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BpSelectCurrentProject.Location = new System.Drawing.Point(427, 92);
            this.BpSelectCurrentProject.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BpSelectCurrentProject.Name = "BpSelectCurrentProject";
            this.BpSelectCurrentProject.Size = new System.Drawing.Size(180, 34);
            this.BpSelectCurrentProject.TabIndex = 3;
            this.BpSelectCurrentProject.Text = "Validate selection";
            this.BpSelectCurrentProject.UseVisualStyleBackColor = true;
            this.BpSelectCurrentProject.Click += new System.EventHandler(this.BpSelectCurrentProject_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Current projects :";
            // 
            // CurrentProjectsSelection
            // 
            this.CurrentProjectsSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CurrentProjectsSelection.Enabled = false;
            this.CurrentProjectsSelection.FormattingEnabled = true;
            this.CurrentProjectsSelection.Location = new System.Drawing.Point(187, 42);
            this.CurrentProjectsSelection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CurrentProjectsSelection.Name = "CurrentProjectsSelection";
            this.CurrentProjectsSelection.Size = new System.Drawing.Size(823, 28);
            this.CurrentProjectsSelection.TabIndex = 0;
            // 
            // openFileDialogTIAPortalProject
            // 
            this.openFileDialogTIAPortalProject.DefaultExt = "ap17";
            this.openFileDialogTIAPortalProject.Filter = "Projet TIA Portal V17|*.ap17*|Projet TIA Portal V16|*.ap16*|Projet TIA Portal V15" +
    "|*.ap15*";
            this.openFileDialogTIAPortalProject.Title = "Sélection du projet TIA Portal";
            // 
            // TiaPortalProjectSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1065, 185);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TiaPortalProjectSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TIA Portal project selection";
            this.Load += new System.EventHandler(this.TiaPortalProjectSelection_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox CurrentProjectsSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BpSelectCurrentProject;
        private System.Windows.Forms.OpenFileDialog openFileDialogTIAPortalProject;
    }
}