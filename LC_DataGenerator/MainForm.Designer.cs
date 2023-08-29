namespace LC_DataGenerator
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.outRichTextBox = new System.Windows.Forms.RichTextBox();
            this.randomStringListButton = new System.Windows.Forms.Button();
            this.randomYMDButton = new System.Windows.Forms.Button();
            this.randomENFilenameButton = new System.Windows.Forms.Button();
            this.randomZHFilenameButton = new System.Windows.Forms.Button();
            this.randomSentenceButton = new System.Windows.Forms.Button();
            this.randomDoubleButton = new System.Windows.Forms.Button();
            this.randomFileSizeButton = new System.Windows.Forms.Button();
            this.randomIntButton = new System.Windows.Forms.Button();
            this.randomHMSButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // outRichTextBox
            // 
            this.outRichTextBox.Location = new System.Drawing.Point(12, 12);
            this.outRichTextBox.Name = "outRichTextBox";
            this.outRichTextBox.Size = new System.Drawing.Size(526, 426);
            this.outRichTextBox.TabIndex = 0;
            this.outRichTextBox.Text = "";
            // 
            // randomStringListButton
            // 
            this.randomStringListButton.Location = new System.Drawing.Point(544, 12);
            this.randomStringListButton.Name = "randomStringListButton";
            this.randomStringListButton.Size = new System.Drawing.Size(244, 33);
            this.randomStringListButton.TabIndex = 1;
            this.randomStringListButton.Text = "随机单词列表";
            this.randomStringListButton.UseVisualStyleBackColor = true;
            this.randomStringListButton.Click += new System.EventHandler(this.randomStringListButton_Click);
            // 
            // randomYMDButton
            // 
            this.randomYMDButton.Location = new System.Drawing.Point(545, 52);
            this.randomYMDButton.Name = "randomYMDButton";
            this.randomYMDButton.Size = new System.Drawing.Size(243, 34);
            this.randomYMDButton.TabIndex = 2;
            this.randomYMDButton.Text = "随机年月日";
            this.randomYMDButton.UseVisualStyleBackColor = true;
            this.randomYMDButton.Click += new System.EventHandler(this.randomYMDButton_Click);
            // 
            // randomENFilenameButton
            // 
            this.randomENFilenameButton.Location = new System.Drawing.Point(545, 129);
            this.randomENFilenameButton.Name = "randomENFilenameButton";
            this.randomENFilenameButton.Size = new System.Drawing.Size(243, 31);
            this.randomENFilenameButton.TabIndex = 3;
            this.randomENFilenameButton.Text = "随机文件名（英文）";
            this.randomENFilenameButton.UseVisualStyleBackColor = true;
            this.randomENFilenameButton.Click += new System.EventHandler(this.randomENFilenameButton_Click);
            // 
            // randomZHFilenameButton
            // 
            this.randomZHFilenameButton.Location = new System.Drawing.Point(545, 166);
            this.randomZHFilenameButton.Name = "randomZHFilenameButton";
            this.randomZHFilenameButton.Size = new System.Drawing.Size(243, 31);
            this.randomZHFilenameButton.TabIndex = 4;
            this.randomZHFilenameButton.Text = "随机文件名（中文）";
            this.randomZHFilenameButton.UseVisualStyleBackColor = true;
            this.randomZHFilenameButton.Click += new System.EventHandler(this.randomZHFilenameButton_Click);
            // 
            // randomSentenceButton
            // 
            this.randomSentenceButton.Location = new System.Drawing.Point(544, 92);
            this.randomSentenceButton.Name = "randomSentenceButton";
            this.randomSentenceButton.Size = new System.Drawing.Size(243, 31);
            this.randomSentenceButton.TabIndex = 5;
            this.randomSentenceButton.Text = "随机句子";
            this.randomSentenceButton.UseVisualStyleBackColor = true;
            this.randomSentenceButton.Click += new System.EventHandler(this.randomSentenceButton_Click);
            // 
            // randomDoubleButton
            // 
            this.randomDoubleButton.Location = new System.Drawing.Point(545, 203);
            this.randomDoubleButton.Name = "randomDoubleButton";
            this.randomDoubleButton.Size = new System.Drawing.Size(243, 31);
            this.randomDoubleButton.TabIndex = 6;
            this.randomDoubleButton.Text = "随机小数";
            this.randomDoubleButton.UseVisualStyleBackColor = true;
            this.randomDoubleButton.Click += new System.EventHandler(this.randomDoubleButton_Click);
            // 
            // randomFileSizeButton
            // 
            this.randomFileSizeButton.Location = new System.Drawing.Point(545, 277);
            this.randomFileSizeButton.Name = "randomFileSizeButton";
            this.randomFileSizeButton.Size = new System.Drawing.Size(243, 31);
            this.randomFileSizeButton.TabIndex = 7;
            this.randomFileSizeButton.Text = "随机文件大小";
            this.randomFileSizeButton.UseVisualStyleBackColor = true;
            this.randomFileSizeButton.Click += new System.EventHandler(this.randomFileSizeButton_Click);
            // 
            // randomIntButton
            // 
            this.randomIntButton.Location = new System.Drawing.Point(545, 240);
            this.randomIntButton.Name = "randomIntButton";
            this.randomIntButton.Size = new System.Drawing.Size(243, 31);
            this.randomIntButton.TabIndex = 8;
            this.randomIntButton.Text = "随机整数";
            this.randomIntButton.UseVisualStyleBackColor = true;
            this.randomIntButton.Click += new System.EventHandler(this.randomIntButton_Click);
            // 
            // randomHMSButton
            // 
            this.randomHMSButton.Location = new System.Drawing.Point(545, 314);
            this.randomHMSButton.Name = "randomHMSButton";
            this.randomHMSButton.Size = new System.Drawing.Size(243, 31);
            this.randomHMSButton.TabIndex = 9;
            this.randomHMSButton.Text = "随机时分秒";
            this.randomHMSButton.UseVisualStyleBackColor = true;
            this.randomHMSButton.Click += new System.EventHandler(this.randomHMSButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.randomHMSButton);
            this.Controls.Add(this.randomIntButton);
            this.Controls.Add(this.randomFileSizeButton);
            this.Controls.Add(this.randomDoubleButton);
            this.Controls.Add(this.randomSentenceButton);
            this.Controls.Add(this.randomZHFilenameButton);
            this.Controls.Add(this.randomENFilenameButton);
            this.Controls.Add(this.randomYMDButton);
            this.Controls.Add(this.randomStringListButton);
            this.Controls.Add(this.outRichTextBox);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox outRichTextBox;
        private System.Windows.Forms.Button randomStringListButton;
        private System.Windows.Forms.Button randomYMDButton;
        private System.Windows.Forms.Button randomENFilenameButton;
        private System.Windows.Forms.Button randomZHFilenameButton;
        private System.Windows.Forms.Button randomSentenceButton;
        private System.Windows.Forms.Button randomDoubleButton;
        private System.Windows.Forms.Button randomFileSizeButton;
        private System.Windows.Forms.Button randomIntButton;
        private System.Windows.Forms.Button randomHMSButton;
    }
}

