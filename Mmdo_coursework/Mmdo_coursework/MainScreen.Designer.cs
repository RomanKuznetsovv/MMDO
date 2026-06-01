namespace Mmdo_coursework
{
    partial class MainScreen
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
            btnSolve = new ReaLTaiizor.Controls.CyberButton();
            systemSolutionBut = new ReaLTaiizor.Controls.CyberButton();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            data = new DataGridView();
            Column5 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
            Column7 = new DataGridViewTextBoxColumn();
            Column8 = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            ((System.ComponentModel.ISupportInitialize)data).BeginInit();
            SuspendLayout();
            // 
            // btnSolve
            // 
            btnSolve.Alpha = 20;
            btnSolve.BackColor = Color.Transparent;
            btnSolve.Background = true;
            btnSolve.Background_WidthPen = 4F;
            btnSolve.BackgroundPen = true;
            btnSolve.ColorBackground = Color.FromArgb(37, 52, 68);
            btnSolve.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            btnSolve.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            btnSolve.ColorBackground_Pen = Color.FromArgb(29, 200, 238);
            btnSolve.ColorLighting = Color.FromArgb(29, 200, 238);
            btnSolve.ColorPen_1 = Color.FromArgb(37, 52, 68);
            btnSolve.ColorPen_2 = Color.FromArgb(41, 63, 86);
            btnSolve.CyberButtonStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            btnSolve.Effect_1 = true;
            btnSolve.Effect_1_ColorBackground = Color.FromArgb(29, 200, 238);
            btnSolve.Effect_1_Transparency = 25;
            btnSolve.Effect_2 = true;
            btnSolve.Effect_2_ColorBackground = Color.White;
            btnSolve.Effect_2_Transparency = 20;
            btnSolve.Font = new Font("Arial", 11F);
            btnSolve.ForeColor = Color.FromArgb(245, 245, 245);
            btnSolve.Lighting = false;
            btnSolve.LinearGradient_Background = false;
            btnSolve.LinearGradientPen = false;
            btnSolve.Location = new Point(32, 259);
            btnSolve.Name = "btnSolve";
            btnSolve.PenWidth = 15;
            btnSolve.Rounding = true;
            btnSolve.RoundingInt = 70;
            btnSolve.Size = new Size(162, 39);
            btnSolve.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            btnSolve.TabIndex = 4;
            btnSolve.Tag = "Cyber";
            btnSolve.TextButton = "Розрахувати";
            btnSolve.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            btnSolve.Timer_Effect_1 = 5;
            btnSolve.Timer_RGB = 300;
            btnSolve.Click += btnSolve_Click_1;
            // 
            // systemSolutionBut
            // 
            systemSolutionBut.Alpha = 20;
            systemSolutionBut.BackColor = Color.Transparent;
            systemSolutionBut.Background = true;
            systemSolutionBut.Background_WidthPen = 4F;
            systemSolutionBut.BackgroundPen = true;
            systemSolutionBut.ColorBackground = Color.FromArgb(37, 52, 68);
            systemSolutionBut.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            systemSolutionBut.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            systemSolutionBut.ColorBackground_Pen = Color.FromArgb(29, 200, 238);
            systemSolutionBut.ColorLighting = Color.FromArgb(29, 200, 238);
            systemSolutionBut.ColorPen_1 = Color.FromArgb(37, 52, 68);
            systemSolutionBut.ColorPen_2 = Color.FromArgb(41, 63, 86);
            systemSolutionBut.CyberButtonStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            systemSolutionBut.Effect_1 = true;
            systemSolutionBut.Effect_1_ColorBackground = Color.FromArgb(29, 200, 238);
            systemSolutionBut.Effect_1_Transparency = 25;
            systemSolutionBut.Effect_2 = true;
            systemSolutionBut.Effect_2_ColorBackground = Color.White;
            systemSolutionBut.Effect_2_Transparency = 20;
            systemSolutionBut.Font = new Font("Arial", 11F);
            systemSolutionBut.ForeColor = Color.FromArgb(245, 245, 245);
            systemSolutionBut.Lighting = false;
            systemSolutionBut.LinearGradient_Background = false;
            systemSolutionBut.LinearGradientPen = false;
            systemSolutionBut.Location = new Point(291, 259);
            systemSolutionBut.Name = "systemSolutionBut";
            systemSolutionBut.PenWidth = 15;
            systemSolutionBut.Rounding = true;
            systemSolutionBut.RoundingInt = 70;
            systemSolutionBut.Size = new Size(174, 39);
            systemSolutionBut.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            systemSolutionBut.TabIndex = 7;
            systemSolutionBut.Tag = "Cyber";
            systemSolutionBut.TextButton = "Розв'язок системи";
            systemSolutionBut.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            systemSolutionBut.Timer_Effect_1 = 5;
            systemSolutionBut.Timer_RGB = 300;
            systemSolutionBut.Click += systemSolutionBut_Click;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Location = new Point(536, -1);
            webView21.Name = "webView21";
            webView21.Size = new Size(923, 1052);
            webView21.TabIndex = 8;
            webView21.ZoomFactor = 1D;
            // 
            // data
            // 
            data.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            data.BackgroundColor = SystemColors.Control;
            data.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            data.Columns.AddRange(new DataGridViewColumn[] { Column5, Column6, Column7, Column8 });
            data.GridColor = SystemColors.ActiveCaptionText;
            data.Location = new Point(1, -1);
            data.Name = "data";
            data.RowHeadersWidth = 51;
            data.Size = new Size(529, 243);
            data.TabIndex = 9;
            // 
            // Column5
            // 
            Column5.HeaderText = "Види сировини";
            Column5.MinimumWidth = 6;
            Column5.Name = "Column5";
            // 
            // Column6
            // 
            Column6.HeaderText = "Запаси сировини";
            Column6.MinimumWidth = 6;
            Column6.Name = "Column6";
            // 
            // Column7
            // 
            Column7.HeaderText = "П1 (шт. на 1 од.)";
            Column7.MinimumWidth = 6;
            Column7.Name = "Column7";
            // 
            // Column8
            // 
            Column8.HeaderText = "П2 (шт. на 1 од.)";
            Column8.MinimumWidth = 6;
            Column8.Name = "Column8";
            // 
            // MainScreen
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1465, 1055);
            Controls.Add(data);
            Controls.Add(webView21);
            Controls.Add(systemSolutionBut);
            Controls.Add(btnSolve);
            Name = "MainScreen";
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ((System.ComponentModel.ISupportInitialize)data).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private ReaLTaiizor.Controls.CyberButton btnSolve;
        private ReaLTaiizor.Controls.CyberButton systemSolutionBut;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private DataGridView data;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column6;
        private DataGridViewTextBoxColumn Column7;
        private DataGridViewTextBoxColumn Column8;
    }
}