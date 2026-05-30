namespace Mmdo_coursework
{
    partial class SystemSolutionForm
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
            back_Button = new ReaLTaiizor.Controls.CyberButton();
            SuspendLayout();
            // 
            // back_Button
            // 
            back_Button.Alpha = 20;
            back_Button.BackColor = Color.Transparent;
            back_Button.Background = true;
            back_Button.Background_WidthPen = 4F;
            back_Button.BackgroundPen = true;
            back_Button.ColorBackground = Color.FromArgb(37, 52, 68);
            back_Button.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            back_Button.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            back_Button.ColorBackground_Pen = Color.FromArgb(29, 200, 238);
            back_Button.ColorLighting = Color.FromArgb(29, 200, 238);
            back_Button.ColorPen_1 = Color.FromArgb(37, 52, 68);
            back_Button.ColorPen_2 = Color.FromArgb(41, 63, 86);
            back_Button.CyberButtonStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            back_Button.Effect_1 = true;
            back_Button.Effect_1_ColorBackground = Color.FromArgb(29, 200, 238);
            back_Button.Effect_1_Transparency = 25;
            back_Button.Effect_2 = true;
            back_Button.Effect_2_ColorBackground = Color.White;
            back_Button.Effect_2_Transparency = 20;
            back_Button.Font = new Font("Arial", 11F);
            back_Button.ForeColor = Color.FromArgb(245, 245, 245);
            back_Button.Lighting = false;
            back_Button.LinearGradient_Background = false;
            back_Button.LinearGradientPen = false;
            back_Button.Location = new Point(12, 12);
            back_Button.Name = "back_Button";
            back_Button.PenWidth = 15;
            back_Button.Rounding = true;
            back_Button.RoundingInt = 70;
            back_Button.Size = new Size(162, 39);
            back_Button.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            back_Button.TabIndex = 0;
            back_Button.Tag = "Cyber";
            back_Button.TextButton = "Back";
            back_Button.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            back_Button.Timer_Effect_1 = 5;
            back_Button.Timer_RGB = 300;
            back_Button.Click += back_Button_Click;
            // 
            // SystemSolutionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1103, 632);
            ControlBox = false;
            Controls.Add(back_Button);
            DoubleBuffered = true;
            Name = "SystemSolutionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SystemSolutionForm";
            Load += SystemSolutionForm_Load;
            ResumeLayout(false);
        }

        #endregion

        private ReaLTaiizor.Controls.CyberButton back_Button;
    }
}