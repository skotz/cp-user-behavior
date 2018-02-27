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
            this.btnParseDatabase = new System.Windows.Forms.Button();
            this.ofdGetUserBehaviors = new System.Windows.Forms.OpenFileDialog();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnParseDatabase
            // 
            this.btnParseDatabase.Location = new System.Drawing.Point(12, 12);
            this.btnParseDatabase.Name = "btnParseDatabase";
            this.btnParseDatabase.Size = new System.Drawing.Size(182, 23);
            this.btnParseDatabase.TabIndex = 0;
            this.btnParseDatabase.Text = "Create Training Data";
            this.btnParseDatabase.UseVisualStyleBackColor = true;
            this.btnParseDatabase.Click += new System.EventHandler(this.btnParseDatabase_Click);
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(475, 289);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(182, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Get Suggested Article";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(699, 356);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnParseDatabase);
            this.Name = "MainForm";
            this.Text = "User Behavior Challenge - Scott Clayton";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnParseDatabase;
        private System.Windows.Forms.OpenFileDialog ofdGetUserBehaviors;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.Button button1;
    }
}

