namespace PathFinder
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolFindPath = new System.Windows.Forms.ToolStripButton();
            this.toolStripTest = new System.Windows.Forms.ToolStripButton();
            this.toolShortPath = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolFindPath,
            this.toolStripTest,
            this.toolShortPath});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(805, 35);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolFindPath
            // 
            this.toolFindPath.Image = ((System.Drawing.Image)(resources.GetObject("toolFindPath.Image")));
            this.toolFindPath.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolFindPath.Name = "toolFindPath";
            this.toolFindPath.Size = new System.Drawing.Size(128, 32);
            this.toolFindPath.Text = "路径计算";
            this.toolFindPath.Click += new System.EventHandler(this.toolFindPath_Click);
            // 
            // toolStripTest
            // 
            this.toolStripTest.Image = ((System.Drawing.Image)(resources.GetObject("toolStripTest.Image")));
            this.toolStripTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripTest.Name = "toolStripTest";
            this.toolStripTest.Size = new System.Drawing.Size(86, 32);
            this.toolStripTest.Text = "测试";
            this.toolStripTest.Click += new System.EventHandler(this.toolStripTest_Click);
            // 
            // toolShortPath
            // 
            this.toolShortPath.Image = ((System.Drawing.Image)(resources.GetObject("toolShortPath.Image")));
            this.toolShortPath.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolShortPath.Name = "toolShortPath";
            this.toolShortPath.Size = new System.Drawing.Size(128, 32);
            this.toolShortPath.Text = "最短路径";
            this.toolShortPath.Click += new System.EventHandler(this.toolShortPath_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 652);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolFindPath;
        private System.Windows.Forms.ToolStripButton toolStripTest;
        private System.Windows.Forms.ToolStripButton toolShortPath;
    }
}

