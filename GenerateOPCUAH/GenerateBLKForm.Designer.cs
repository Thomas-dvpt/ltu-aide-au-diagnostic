namespace GenerateBLK
{
    partial class GenerateBLKForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenerateBLKForm));
            this.BackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.Status = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusProgressbar = new System.Windows.Forms.ToolStripProgressBar();
            this.Gif_Wait = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.InfoTrace = new System.Windows.Forms.RichTextBox();
            this.ContextMenuTrace = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuClearTrace = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PanelGenerationCode = new System.Windows.Forms.Panel();
            this.ToolTipText = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.PLC_H_Station_Target_Name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BpSelectS71500H = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.BpStartCodeGenerator = new System.Windows.Forms.Button();
            this.TimerRefresh = new System.Windows.Forms.Timer(this.components);
            this.Status.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Gif_Wait)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.ContextMenuTrace.SuspendLayout();
            this.PanelGenerationCode.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // BackgroundWorker
            // 
            this.BackgroundWorker.WorkerSupportsCancellation = true;
            // 
            // Status
            // 
            this.Status.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.StatusProgressbar});
            this.Status.Location = new System.Drawing.Point(0, 796);
            this.Status.Name = "Status";
            this.Status.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.Status.Size = new System.Drawing.Size(1132, 25);
            this.Status.TabIndex = 4;
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = false;
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(700, 19);
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StatusProgressbar
            // 
            this.StatusProgressbar.Name = "StatusProgressbar";
            this.StatusProgressbar.Size = new System.Drawing.Size(187, 17);
            // 
            // Gif_Wait
            // 
            this.Gif_Wait.Image = ((System.Drawing.Image)(resources.GetObject("Gif_Wait.Image")));
            this.Gif_Wait.Location = new System.Drawing.Point(518, 242);
            this.Gif_Wait.Margin = new System.Windows.Forms.Padding(4);
            this.Gif_Wait.Name = "Gif_Wait";
            this.Gif_Wait.Size = new System.Drawing.Size(80, 74);
            this.Gif_Wait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Gif_Wait.TabIndex = 6;
            this.Gif_Wait.TabStop = false;
            this.Gif_Wait.Visible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.Gif_Wait);
            this.groupBox3.Controls.Add(this.PanelGenerationCode);
            this.groupBox3.Controls.Add(this.InfoTrace);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(8, 259);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(1119, 533);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Informations";
            // 
            // InfoTrace
            // 
            this.InfoTrace.ContextMenuStrip = this.ContextMenuTrace;
            this.InfoTrace.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoTrace.Location = new System.Drawing.Point(21, 27);
            this.InfoTrace.Margin = new System.Windows.Forms.Padding(4);
            this.InfoTrace.Name = "InfoTrace";
            this.InfoTrace.ReadOnly = true;
            this.InfoTrace.Size = new System.Drawing.Size(1075, 498);
            this.InfoTrace.TabIndex = 0;
            this.InfoTrace.Text = "";
            // 
            // ContextMenuTrace
            // 
            this.ContextMenuTrace.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuTrace.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuClearTrace});
            this.ContextMenuTrace.Name = "ContextMenuTrace";
            this.ContextMenuTrace.Size = new System.Drawing.Size(139, 28);
            // 
            // ToolStripMenuClearTrace
            // 
            this.ToolStripMenuClearTrace.Name = "ToolStripMenuClearTrace";
            this.ToolStripMenuClearTrace.Size = new System.Drawing.Size(138, 24);
            this.ToolStripMenuClearTrace.Text = "Clear log";
            this.ToolStripMenuClearTrace.Click += new System.EventHandler(this.ToolStripMenuClearTrace_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 39);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Projet selectionné :";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(221, 37);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(822, 26);
            this.textBox1.TabIndex = 3;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 27);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(285, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "PLC code generation is in progress...";
            // 
            // PanelGenerationCode
            // 
            this.PanelGenerationCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PanelGenerationCode.Controls.Add(this.label4);
            this.PanelGenerationCode.Location = new System.Drawing.Point(366, 238);
            this.PanelGenerationCode.Margin = new System.Windows.Forms.Padding(4);
            this.PanelGenerationCode.Name = "PanelGenerationCode";
            this.PanelGenerationCode.Size = new System.Drawing.Size(368, 78);
            this.PanelGenerationCode.TabIndex = 7;
            this.PanelGenerationCode.Visible = false;
            // 
            // ToolTipText
            // 
            this.ToolTipText.BackColor = System.Drawing.Color.Yellow;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.PLC_H_Station_Target_Name);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Controls.Add(this.BpSelectS71500H);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(8, 15);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(1119, 135);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "S7-1500 Target";
            // 
            // PLC_H_Station_Target_Name
            // 
            this.PLC_H_Station_Target_Name.Location = new System.Drawing.Point(221, 87);
            this.PLC_H_Station_Target_Name.Margin = new System.Windows.Forms.Padding(4);
            this.PLC_H_Station_Target_Name.Name = "PLC_H_Station_Target_Name";
            this.PLC_H_Station_Target_Name.ReadOnly = true;
            this.PLC_H_Station_Target_Name.Size = new System.Drawing.Size(549, 26);
            this.PLC_H_Station_Target_Name.TabIndex = 2;
            this.PLC_H_Station_Target_Name.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 90);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "PLC S7-1500 station :";
            // 
            // BpSelectS71500H
            // 
            this.BpSelectS71500H.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BpSelectS71500H.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BpSelectS71500H.Location = new System.Drawing.Point(778, 78);
            this.BpSelectS71500H.Margin = new System.Windows.Forms.Padding(4);
            this.BpSelectS71500H.Name = "BpSelectS71500H";
            this.BpSelectS71500H.Size = new System.Drawing.Size(265, 44);
            this.BpSelectS71500H.TabIndex = 0;
            this.BpSelectS71500H.Text = "Select PLC S7-1500 station";
            this.BpSelectS71500H.UseVisualStyleBackColor = true;
            this.BpSelectS71500H.Click += new System.EventHandler(this.BpSelectS71500H_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.BpStartCodeGenerator);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(8, 158);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(1119, 93);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Fonction sBackJump";
            // 
            // BpStartCodeGenerator
            // 
            this.BpStartCodeGenerator.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BpStartCodeGenerator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BpStartCodeGenerator.Location = new System.Drawing.Point(43, 30);
            this.BpStartCodeGenerator.Margin = new System.Windows.Forms.Padding(4);
            this.BpStartCodeGenerator.Name = "BpStartCodeGenerator";
            this.BpStartCodeGenerator.Size = new System.Drawing.Size(1000, 44);
            this.BpStartCodeGenerator.TabIndex = 0;
            this.BpStartCodeGenerator.Text = "FC Block Export / Blocks Analyze / FC Import";
            this.BpStartCodeGenerator.UseVisualStyleBackColor = true;
            this.BpStartCodeGenerator.Click += new System.EventHandler(this.BpStartCodeGenerator_Click);
            // 
            // TimerRefresh
            // 
            this.TimerRefresh.Tick += new System.EventHandler(this.TimerRefresh_Tick);
            // 
            // GenerateBLKForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 821);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.Status);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GenerateBLKForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Aide au diagnostic";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.GenerateBLKForm_Shown);
            this.Status.ResumeLayout(false);
            this.Status.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Gif_Wait)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ContextMenuTrace.ResumeLayout(false);
            this.PanelGenerationCode.ResumeLayout(false);
            this.PanelGenerationCode.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker BackgroundWorker;
        private System.Windows.Forms.StatusStrip Status;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private System.Windows.Forms.ToolStripProgressBar StatusProgressbar;
        private System.Windows.Forms.PictureBox Gif_Wait;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RichTextBox InfoTrace;
        private System.Windows.Forms.ContextMenuStrip ContextMenuTrace;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuClearTrace;
        private System.Windows.Forms.ToolTip ToolTipText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button BpSelectS71500H;
        private System.Windows.Forms.TextBox PLC_H_Station_Target_Name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button BpStartCodeGenerator;
        private System.Windows.Forms.Timer TimerRefresh;
        private System.Windows.Forms.Panel PanelGenerationCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
    }
}

