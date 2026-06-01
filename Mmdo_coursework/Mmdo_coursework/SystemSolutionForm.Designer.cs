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
            nudVars = new ReaLTaiizor.Controls.CrownNumeric();
            materialLabel1 = new ReaLTaiizor.Controls.MaterialLabel();
            materialLabel2 = new ReaLTaiizor.Controls.MaterialLabel();
            nudConstraints = new ReaLTaiizor.Controls.CrownNumeric();
            materialLabel3 = new ReaLTaiizor.Controls.MaterialLabel();
            rbMin = new ReaLTaiizor.Controls.MaterialRadioButton();
            rbMax = new ReaLTaiizor.Controls.MaterialRadioButton();
            panelInput = new Panel();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            btnSolve = new ReaLTaiizor.Controls.CyberButton();
            materialLabel4 = new ReaLTaiizor.Controls.MaterialLabel();
            toggleInteger = new ReaLTaiizor.Controls.CyberSwitch();
            ((System.ComponentModel.ISupportInitialize)nudVars).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudConstraints).BeginInit();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
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
            back_Button.Location = new Point(582, 91);
            back_Button.Name = "back_Button";
            back_Button.PenWidth = 15;
            back_Button.Rounding = true;
            back_Button.RoundingInt = 70;
            back_Button.Size = new Size(162, 39);
            back_Button.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            back_Button.TabIndex = 0;
            back_Button.Tag = "Cyber";
            back_Button.TextButton = "Назад";
            back_Button.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            back_Button.Timer_Effect_1 = 5;
            back_Button.Timer_RGB = 300;
            back_Button.Click += back_Button_Click;
            // 
            // nudVars
            // 
            nudVars.Location = new Point(218, 18);
            nudVars.Name = "nudVars";
            nudVars.Size = new Size(54, 27);
            nudVars.TabIndex = 3;
            // 
            // materialLabel1
            // 
            materialLabel1.AutoSize = true;
            materialLabel1.Depth = 0;
            materialLabel1.Font = new Font("Roboto", 17.5F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel1.Location = new Point(7, 18);
            materialLabel1.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialLabel1.Name = "materialLabel1";
            materialLabel1.Size = new Size(185, 24);
            materialLabel1.TabIndex = 6;
            materialLabel1.Text = "Кількість змінних : ";
            // 
            // materialLabel2
            // 
            materialLabel2.AutoSize = true;
            materialLabel2.Depth = 0;
            materialLabel2.Font = new Font("Roboto", 17.5F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel2.Location = new Point(7, 68);
            materialLabel2.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialLabel2.Name = "materialLabel2";
            materialLabel2.Size = new Size(201, 24);
            materialLabel2.TabIndex = 7;
            materialLabel2.Text = "Кількість обмежень :";
            // 
            // nudConstraints
            // 
            nudConstraints.Location = new Point(218, 67);
            nudConstraints.Name = "nudConstraints";
            nudConstraints.Size = new Size(54, 27);
            nudConstraints.TabIndex = 8;
            // 
            // materialLabel3
            // 
            materialLabel3.AutoSize = true;
            materialLabel3.Depth = 0;
            materialLabel3.Font = new Font("Roboto", 17.5F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel3.Location = new Point(7, 118);
            materialLabel3.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialLabel3.Name = "materialLabel3";
            materialLabel3.Size = new Size(112, 24);
            materialLabel3.TabIndex = 9;
            materialLabel3.Text = "Тип задачі :";
            // 
            // rbMin
            // 
            rbMin.AutoSize = true;
            rbMin.Depth = 0;
            rbMin.Location = new Point(131, 106);
            rbMin.Margin = new Padding(0);
            rbMin.MouseLocation = new Point(-1, -1);
            rbMin.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            rbMin.Name = "rbMin";
            rbMin.Ripple = true;
            rbMin.Size = new Size(77, 46);
            rbMin.TabIndex = 12;
            rbMin.TabStop = true;
            rbMin.Text = "Min";
            rbMin.UseAccentColor = false;
            rbMin.UseVisualStyleBackColor = true;
            // 
            // rbMax
            // 
            rbMax.AutoSize = true;
            rbMax.Depth = 0;
            rbMax.Location = new Point(218, 106);
            rbMax.Margin = new Padding(0);
            rbMax.MouseLocation = new Point(-1, -1);
            rbMax.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            rbMax.Name = "rbMax";
            rbMax.Ripple = true;
            rbMax.Size = new Size(82, 46);
            rbMax.TabIndex = 11;
            rbMax.TabStop = true;
            rbMax.Text = "Max";
            rbMax.UseAccentColor = false;
            rbMax.UseVisualStyleBackColor = true;
            // 
            // panelInput
            // 
            panelInput.Location = new Point(783, 18);
            panelInput.Name = "panelInput";
            panelInput.Size = new Size(505, 464);
            panelInput.TabIndex = 13;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.White;
            webView21.Location = new Point(7, 208);
            webView21.Name = "webView21";
            webView21.Size = new Size(770, 745);
            webView21.TabIndex = 27;
            webView21.ZoomFactor = 1D;
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
            btnSolve.Location = new Point(582, 33);
            btnSolve.Name = "btnSolve";
            btnSolve.PenWidth = 15;
            btnSolve.Rounding = true;
            btnSolve.RoundingInt = 70;
            btnSolve.Size = new Size(162, 39);
            btnSolve.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            btnSolve.TabIndex = 28;
            btnSolve.Tag = "Cyber";
            btnSolve.TextButton = "Розрахувати";
            btnSolve.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            btnSolve.Timer_Effect_1 = 5;
            btnSolve.Timer_RGB = 300;
            btnSolve.Click += btnSolve_Click_1;
            // 
            // materialLabel4
            // 
            materialLabel4.AutoSize = true;
            materialLabel4.Depth = 0;
            materialLabel4.Font = new Font("Roboto", 17.5F, FontStyle.Regular, GraphicsUnit.Pixel);
            materialLabel4.Location = new Point(7, 164);
            materialLabel4.MouseState = ReaLTaiizor.Helper.MaterialDrawHelper.MaterialMouseState.HOVER;
            materialLabel4.Name = "materialLabel4";
            materialLabel4.Size = new Size(251, 24);
            materialLabel4.TabIndex = 29;
            materialLabel4.Text = "Цілочисельний розв'язок :";
            // 
            // toggleInteger
            // 
            toggleInteger.Alpha = 50;
            toggleInteger.BackColor = Color.Transparent;
            toggleInteger.Background = true;
            toggleInteger.Background_WidthPen = 2F;
            toggleInteger.BackgroundPen = true;
            toggleInteger.Checked = false;
            toggleInteger.ColorBackground = Color.FromArgb(37, 52, 68);
            toggleInteger.ColorBackground_1 = Color.FromArgb(37, 52, 68);
            toggleInteger.ColorBackground_2 = Color.FromArgb(41, 63, 86);
            toggleInteger.ColorBackground_Pen = Color.FromArgb(29, 200, 238);
            toggleInteger.ColorBackground_Value_1 = Color.FromArgb(28, 200, 238);
            toggleInteger.ColorBackground_Value_2 = Color.FromArgb(100, 208, 232);
            toggleInteger.ColorLighting = Color.FromArgb(29, 200, 238);
            toggleInteger.ColorPen_1 = Color.FromArgb(37, 52, 68);
            toggleInteger.ColorPen_2 = Color.FromArgb(41, 63, 86);
            toggleInteger.ColorValue = Color.FromArgb(29, 200, 238);
            toggleInteger.CyberSwitchStyle = ReaLTaiizor.Enum.Cyber.StateStyle.Custom;
            toggleInteger.Font = new Font("Arial", 11F);
            toggleInteger.ForeColor = Color.FromArgb(245, 245, 245);
            toggleInteger.Lighting = false;
            toggleInteger.LinearGradient_Background = false;
            toggleInteger.LinearGradient_Value = false;
            toggleInteger.LinearGradientPen = false;
            toggleInteger.Location = new Point(264, 164);
            toggleInteger.Name = "toggleInteger";
            toggleInteger.PenWidth = 10;
            toggleInteger.RGB = false;
            toggleInteger.Rounding = true;
            toggleInteger.RoundingInt = 90;
            toggleInteger.Size = new Size(44, 25);
            toggleInteger.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            toggleInteger.TabIndex = 30;
            toggleInteger.Tag = "Cyber";
            toggleInteger.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            toggleInteger.Timer_RGB = 300;
            // 
            // SystemSolutionForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1300, 1055);
            ControlBox = false;
            Controls.Add(toggleInteger);
            Controls.Add(materialLabel4);
            Controls.Add(btnSolve);
            Controls.Add(webView21);
            Controls.Add(panelInput);
            Controls.Add(rbMax);
            Controls.Add(rbMin);
            Controls.Add(materialLabel3);
            Controls.Add(nudConstraints);
            Controls.Add(materialLabel2);
            Controls.Add(materialLabel1);
            Controls.Add(nudVars);
            Controls.Add(back_Button);
            DoubleBuffered = true;
            Name = "SystemSolutionForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SystemSolutionForm";
            ((System.ComponentModel.ISupportInitialize)nudVars).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudConstraints).EndInit();
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ReaLTaiizor.Controls.CyberButton back_Button;
        private ReaLTaiizor.Controls.CrownNumeric nudVars;
        private ReaLTaiizor.Controls.MaterialLabel materialLabel1;
        private ReaLTaiizor.Controls.MaterialLabel materialLabel2;
        private ReaLTaiizor.Controls.CrownNumeric nudConstraints;
        private ReaLTaiizor.Controls.MaterialLabel materialLabel3;
        private ReaLTaiizor.Controls.MaterialRadioButton rbMin;
        private ReaLTaiizor.Controls.MaterialRadioButton rbMax;
        private Panel panelInput;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private ReaLTaiizor.Controls.CyberButton btnSolve;
        private ReaLTaiizor.Controls.MaterialLabel materialLabel4;
        private ReaLTaiizor.Controls.CyberSwitch toggleInteger;
    }
}