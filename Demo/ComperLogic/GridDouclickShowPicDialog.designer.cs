namespace ETCF
{
    partial class GridDouclickShowPicDialog
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
            this.pictureBoxVeh = new System.Windows.Forms.PictureBox();
            this.labelVeh = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.blackList = new CCWin.SkinControl.SkinButton();
            this.skinLabel1 = new CCWin.SkinControl.SkinLabel();
            this.sbtnCheck = new CCWin.SkinControl.SkinButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVeh)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBoxVeh
            // 
            this.pictureBoxVeh.Location = new System.Drawing.Point(8, 18);
            this.pictureBoxVeh.Name = "pictureBoxVeh";
            this.pictureBoxVeh.Size = new System.Drawing.Size(514, 306);
            this.pictureBoxVeh.TabIndex = 0;
            this.pictureBoxVeh.TabStop = false;
            this.pictureBoxVeh.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxVeh_MouseDoubleClick);
            // 
            // labelVeh
            // 
            this.labelVeh.AutoSize = true;
            this.labelVeh.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelVeh.ForeColor = System.Drawing.Color.Black;
            this.labelVeh.Location = new System.Drawing.Point(6, 17);
            this.labelVeh.Name = "labelVeh";
            this.labelVeh.Size = new System.Drawing.Size(77, 14);
            this.labelVeh.TabIndex = 1;
            this.labelVeh.Text = "车辆车型：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.blackList);
            this.groupBox1.Controls.Add(this.skinLabel1);
            this.groupBox1.Controls.Add(this.sbtnCheck);
            this.groupBox1.Controls.Add(this.labelVeh);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(531, 90);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "车辆信息";
            // 
            // blackList
            // 
            this.blackList.BackColor = System.Drawing.Color.Black;
            this.blackList.BaseColor = System.Drawing.Color.LightGray;
            this.blackList.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.blackList.DownBack = null;
            this.blackList.Location = new System.Drawing.Point(451, 56);
            this.blackList.MouseBack = null;
            this.blackList.Name = "blackList";
            this.blackList.NormlBack = null;
            this.blackList.Size = new System.Drawing.Size(79, 28);
            this.blackList.TabIndex = 4;
            this.blackList.Text = "加入黑名单";
            this.blackList.UseVisualStyleBackColor = false;
            this.blackList.Click += new System.EventHandler(this.blackList_Click);
            // 
            // skinLabel1
            // 
            this.skinLabel1.AllowDrop = true;
            this.skinLabel1.BackColor = System.Drawing.Color.Transparent;
            this.skinLabel1.BorderColor = System.Drawing.Color.White;
            this.skinLabel1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinLabel1.Location = new System.Drawing.Point(203, 60);
            this.skinLabel1.Name = "skinLabel1";
            this.skinLabel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.skinLabel1.Size = new System.Drawing.Size(157, 27);
            this.skinLabel1.TabIndex = 3;
            this.skinLabel1.Text = "skinLabel1  ";
            this.skinLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sbtnCheck
            // 
            this.sbtnCheck.BackColor = System.Drawing.Color.Black;
            this.sbtnCheck.BaseColor = System.Drawing.Color.LightGray;
            this.sbtnCheck.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.sbtnCheck.DownBack = null;
            this.sbtnCheck.Location = new System.Drawing.Point(366, 56);
            this.sbtnCheck.MouseBack = null;
            this.sbtnCheck.Name = "sbtnCheck";
            this.sbtnCheck.NormlBack = null;
            this.sbtnCheck.Size = new System.Drawing.Size(79, 28);
            this.sbtnCheck.TabIndex = 2;
            this.sbtnCheck.Text = "加入白名单";
            this.sbtnCheck.UseVisualStyleBackColor = false;
            this.sbtnCheck.Click += new System.EventHandler(this.wihiteList_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBoxVeh);
            this.groupBox2.Location = new System.Drawing.Point(12, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(531, 322);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "图片信息";
            // 
            // GridDouclickShowPicDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 435);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "GridDouclickShowPicDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "稽查车辆详细信息";
            this.Load += new System.EventHandler(this.GridDouclickShowPicDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVeh)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxVeh;
        private System.Windows.Forms.Label labelVeh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private CCWin.SkinControl.SkinButton sbtnCheck;
        private CCWin.SkinControl.SkinLabel skinLabel1;
        private CCWin.SkinControl.SkinButton blackList;
    }
}