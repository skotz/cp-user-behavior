namespace Example
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ofdGetUserBehaviors = new System.Windows.Forms.OpenFileDialog();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.groupTrain = new System.Windows.Forms.GroupBox();
            this.btnLoadTrain = new System.Windows.Forms.Button();
            this.groupScore = new System.Windows.Forms.GroupBox();
            this.txtScoreArticle = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtScoreUser = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnScore = new System.Windows.Forms.Button();
            this.groupRecommend = new System.Windows.Forms.GroupBox();
            this.txtRecommendNum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRecommendUser = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRecommend = new System.Windows.Forms.Button();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.bgScore = new System.ComponentModel.BackgroundWorker();
            this.bgRecommend = new System.ComponentModel.BackgroundWorker();
            this.groupTrain.SuspendLayout();
            this.groupScore.SuspendLayout();
            this.groupRecommend.SuspendLayout();
            this.SuspendLayout();
            // 
            // ofdGetUserBehaviors
            // 
            this.ofdGetUserBehaviors.FileName = "UserBehaviour.txt";
            // 
            // bgWorker
            // 
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            // 
            // groupTrain
            // 
            this.groupTrain.Controls.Add(this.btnLoadTrain);
            this.groupTrain.Location = new System.Drawing.Point(12, 12);
            this.groupTrain.Name = "groupTrain";
            this.groupTrain.Size = new System.Drawing.Size(282, 49);
            this.groupTrain.TabIndex = 2;
            this.groupTrain.TabStop = false;
            this.groupTrain.Text = "Train the Classifier";
            // 
            // btnLoadTrain
            // 
            this.btnLoadTrain.Location = new System.Drawing.Point(6, 19);
            this.btnLoadTrain.Name = "btnLoadTrain";
            this.btnLoadTrain.Size = new System.Drawing.Size(270, 23);
            this.btnLoadTrain.TabIndex = 0;
            this.btnLoadTrain.Text = "Load Data and Train";
            this.btnLoadTrain.UseVisualStyleBackColor = true;
            this.btnLoadTrain.Click += new System.EventHandler(this.btnLoadTrain_Click);
            // 
            // groupScore
            // 
            this.groupScore.Controls.Add(this.txtScoreArticle);
            this.groupScore.Controls.Add(this.label2);
            this.groupScore.Controls.Add(this.txtScoreUser);
            this.groupScore.Controls.Add(this.label1);
            this.groupScore.Controls.Add(this.btnScore);
            this.groupScore.Enabled = false;
            this.groupScore.Location = new System.Drawing.Point(12, 67);
            this.groupScore.Name = "groupScore";
            this.groupScore.Size = new System.Drawing.Size(138, 101);
            this.groupScore.TabIndex = 2;
            this.groupScore.TabStop = false;
            this.groupScore.Text = "Score an Article";
            // 
            // txtScoreArticle
            // 
            this.txtScoreArticle.Location = new System.Drawing.Point(67, 45);
            this.txtScoreArticle.Name = "txtScoreArticle";
            this.txtScoreArticle.Size = new System.Drawing.Size(65, 20);
            this.txtScoreArticle.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Article ID";
            // 
            // txtScoreUser
            // 
            this.txtScoreUser.Location = new System.Drawing.Point(67, 19);
            this.txtScoreUser.Name = "txtScoreUser";
            this.txtScoreUser.Size = new System.Drawing.Size(65, 20);
            this.txtScoreUser.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "User ID";
            // 
            // btnScore
            // 
            this.btnScore.Location = new System.Drawing.Point(6, 71);
            this.btnScore.Name = "btnScore";
            this.btnScore.Size = new System.Drawing.Size(126, 23);
            this.btnScore.TabIndex = 0;
            this.btnScore.Text = "Score";
            this.btnScore.UseVisualStyleBackColor = true;
            this.btnScore.Click += new System.EventHandler(this.btnScore_Click);
            // 
            // groupRecommend
            // 
            this.groupRecommend.Controls.Add(this.txtRecommendNum);
            this.groupRecommend.Controls.Add(this.label3);
            this.groupRecommend.Controls.Add(this.txtRecommendUser);
            this.groupRecommend.Controls.Add(this.label4);
            this.groupRecommend.Controls.Add(this.btnRecommend);
            this.groupRecommend.Enabled = false;
            this.groupRecommend.Location = new System.Drawing.Point(156, 67);
            this.groupRecommend.Name = "groupRecommend";
            this.groupRecommend.Size = new System.Drawing.Size(138, 101);
            this.groupRecommend.TabIndex = 2;
            this.groupRecommend.TabStop = false;
            this.groupRecommend.Text = "Recommend Articles";
            // 
            // txtRecommendNum
            // 
            this.txtRecommendNum.Location = new System.Drawing.Point(67, 45);
            this.txtRecommendNum.Name = "txtRecommendNum";
            this.txtRecommendNum.Size = new System.Drawing.Size(65, 20);
            this.txtRecommendNum.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "# Articles";
            // 
            // txtRecommendUser
            // 
            this.txtRecommendUser.Location = new System.Drawing.Point(67, 19);
            this.txtRecommendUser.Name = "txtRecommendUser";
            this.txtRecommendUser.Size = new System.Drawing.Size(65, 20);
            this.txtRecommendUser.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "User ID";
            // 
            // btnRecommend
            // 
            this.btnRecommend.Location = new System.Drawing.Point(6, 71);
            this.btnRecommend.Name = "btnRecommend";
            this.btnRecommend.Size = new System.Drawing.Size(126, 23);
            this.btnRecommend.TabIndex = 0;
            this.btnRecommend.Text = "Recommend";
            this.btnRecommend.UseVisualStyleBackColor = true;
            this.btnRecommend.Click += new System.EventHandler(this.btnRecommend_Click);
            // 
            // rtbOutput
            // 
            this.rtbOutput.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOutput.Location = new System.Drawing.Point(12, 174);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(282, 170);
            this.rtbOutput.TabIndex = 3;
            this.rtbOutput.Text = "";
            // 
            // bgScore
            // 
            this.bgScore.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgScore_DoWork);
            this.bgScore.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgScore_RunWorkerCompleted);
            // 
            // bgRecommend
            // 
            this.bgRecommend.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgRecommend_DoWork);
            this.bgRecommend.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgRecommend_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 356);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.groupRecommend);
            this.Controls.Add(this.groupScore);
            this.Controls.Add(this.groupTrain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Recommender - Scott Clayton";
            this.groupTrain.ResumeLayout(false);
            this.groupScore.ResumeLayout(false);
            this.groupScore.PerformLayout();
            this.groupRecommend.ResumeLayout(false);
            this.groupRecommend.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog ofdGetUserBehaviors;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.GroupBox groupTrain;
        private System.Windows.Forms.Button btnLoadTrain;
        private System.Windows.Forms.GroupBox groupScore;
        private System.Windows.Forms.TextBox txtScoreArticle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtScoreUser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnScore;
        private System.Windows.Forms.GroupBox groupRecommend;
        private System.Windows.Forms.TextBox txtRecommendNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRecommendUser;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRecommend;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.ComponentModel.BackgroundWorker bgScore;
        private System.ComponentModel.BackgroundWorker bgRecommend;
    }
}

