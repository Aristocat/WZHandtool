namespace WZHandtool
{
    partial class UnhandledExceptionLog
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
            this.fieldExceptionLog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // fieldExceptionLog
            // 
            this.fieldExceptionLog.Location = new System.Drawing.Point(-1, 0);
            this.fieldExceptionLog.Name = "fieldExceptionLog";
            this.fieldExceptionLog.ReadOnly = true;
            this.fieldExceptionLog.Size = new System.Drawing.Size(353, 356);
            this.fieldExceptionLog.TabIndex = 0;
            this.fieldExceptionLog.Text = "";
            // 
            // UnhandledExceptionLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 356);
            this.Controls.Add(this.fieldExceptionLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UnhandledExceptionLog";
            this.Text = "UnhandledExceptionLog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UnhandledExceptionLog_OnFormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox fieldExceptionLog;
    }
}