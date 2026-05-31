using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Mmdo_coursework
{
    public partial class SystemSolutionForm : Form
    {
        private List<ComboBox> objSigns = new List<ComboBox>();
        private List<TextBox> objCoefficients = new List<TextBox>();

        private List<List<ComboBox>> constraintSigns = new List<List<ComboBox>>();
        private List<List<TextBox>> constraintCoefficients = new List<List<TextBox>>();
        private List<ComboBox> constraintTypes = new List<ComboBox>();

        private List<ComboBox> rhsSigns = new List<ComboBox>();
        private List<TextBox> constraintRHS = new List<TextBox>();

        private Label lblOpt;
        private MainScreen _originalMainScreen;
        public SystemSolutionForm(MainScreen mainScreen)
        {
            InitializeComponent();
            _originalMainScreen = mainScreen;
            cmbMethod.Items.Clear();
            cmbMethod.Items.Add("Симплекс-метод");
            cmbMethod.Items.Add("Метод штучного базису");
            cmbMethod.SelectedIndex = 0;

            rbMax.Checked = true;
            rbMax.CheckedChanged += UpdateOptLabel;
            rbMin.CheckedChanged += UpdateOptLabel;

            nudVars.ValueChanged += GenerateInputFields;
            nudConstraints.ValueChanged += GenerateInputFields;

            GenerateInputFields(null, null);
        }

        private void UpdateOptLabel(object sender, EventArgs e)
        {
            if (lblOpt != null)
            {
                lblOpt.Text = rbMax.Checked ? "-> Max" : "-> Min";
                lblOpt.ForeColor = rbMax.Checked ? Color.DarkRed : Color.DarkBlue;
            }
        }

        private void GenerateInputFields(object sender, EventArgs e)
        {
            panelInput.Controls.Clear();
            objSigns.Clear(); objCoefficients.Clear();
            constraintSigns.Clear(); constraintCoefficients.Clear(); constraintTypes.Clear();
            rhsSigns.Clear(); constraintRHS.Clear();

            int vars = (int)nudVars.Value;
            int cons = (int)nudConstraints.Value;

            if (vars == 0 || cons == 0) return;

            // ЗБІЛЬШЕНО spacingX з 110 до 135, щоб знаки (+/-) не наїжджали на змінні
            int startX = 10, startY = 10, spacingX = 135, spacingY = 45;

            // ================= ЦІЛЬОВА ФУНКЦІЯ (Z) =================
            Label lblZ = new Label { Text = "Z =", Location = new Point(startX, startY + 4), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            panelInput.Controls.Add(lblZ);

            int currentX = startX + 45;
            for (int j = 0; j < vars; j++)
            {
                ComboBox cmbS = new ComboBox { Location = new Point(currentX, startY), Width = 40, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbS.Items.AddRange(new string[] { "+", "-" });
                cmbS.SelectedIndex = 0;
                objSigns.Add(cmbS);
                panelInput.Controls.Add(cmbS);

                TextBox tb = new TextBox { Location = new Point(currentX + 45, startY), Width = 45, TextAlign = HorizontalAlignment.Center };
                objCoefficients.Add(tb);
                panelInput.Controls.Add(tb);

                Label lbl = new Label { Text = $"x{j + 1}", Location = new Point(currentX + 95, startY + 4), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                panelInput.Controls.Add(lbl);

                currentX += spacingX;
            }

            lblOpt = new Label { Text = rbMax.Checked ? "-> Max" : "-> Min", Location = new Point(currentX, startY + 4), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.DarkRed };
            panelInput.Controls.Add(lblOpt);

            // ================= ОБМЕЖЕННЯ =================
            startY += spacingY + 15;
            for (int i = 0; i < cons; i++)
            {
                currentX = startX;
                List<ComboBox> rowSigns = new List<ComboBox>();
                List<TextBox> rowTb = new List<TextBox>();

                for (int j = 0; j < vars; j++)
                {
                    ComboBox cmbS = new ComboBox { Location = new Point(currentX, startY), Width = 40, DropDownStyle = ComboBoxStyle.DropDownList };
                    cmbS.Items.AddRange(new string[] { "+", "-" });
                    cmbS.SelectedIndex = 0;
                    rowSigns.Add(cmbS);
                    panelInput.Controls.Add(cmbS);

                    TextBox tb = new TextBox { Location = new Point(currentX + 45, startY), Width = 45, TextAlign = HorizontalAlignment.Center };
                    rowTb.Add(tb);
                    panelInput.Controls.Add(tb);

                    Label lbl = new Label { Text = $"x{j + 1}", Location = new Point(currentX + 95, startY + 4), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                    panelInput.Controls.Add(lbl);

                    currentX += spacingX;
                }
                constraintSigns.Add(rowSigns);
                constraintCoefficients.Add(rowTb);

                ComboBox cmbType = new ComboBox { Location = new Point(currentX, startY), Width = 55, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbType.Items.AddRange(new string[] { "<=", ">=", "=" });
                cmbType.SelectedIndex = 0;
                constraintTypes.Add(cmbType);
                panelInput.Controls.Add(cmbType);
                currentX += 65;

                ComboBox cmbRhsS = new ComboBox { Location = new Point(currentX, startY), Width = 40, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbRhsS.Items.AddRange(new string[] { "+", "-" });
                cmbRhsS.SelectedIndex = 0;
                rhsSigns.Add(cmbRhsS);
                panelInput.Controls.Add(cmbRhsS);

                TextBox tbRHS = new TextBox { Location = new Point(currentX + 45, startY), Width = 50, TextAlign = HorizontalAlignment.Center };
                constraintRHS.Add(tbRHS);
                panelInput.Controls.Add(tbRHS);

                startY += spacingY;
            }
        }

        private async void btnSolve_Click(object sender, EventArgs e) // Переконайся, що ця подія прив'язана до кнопки
        {
            try
            {
                int vars = (int)nudVars.Value;
                int cons = (int)nudConstraints.Value;

                if (vars == 0 || cons == 0) return;

                double ParseValue(ComboBox signCmb, TextBox tb)
                {
                    double val = string.IsNullOrWhiteSpace(tb.Text) ? 0 : Convert.ToDouble(tb.Text);
                    return signCmb.SelectedItem.ToString() == "-" ? -val : val;
                }

                double[] objective = new double[vars];
                for (int j = 0; j < vars; j++) objective[j] = ParseValue(objSigns[j], objCoefficients[j]);

                double[,] matrix = new double[cons, vars];
                double[] rhs = new double[cons];
                string[] signs = new string[cons];

                for (int i = 0; i < cons; i++)
                {
                    for (int j = 0; j < vars; j++) matrix[i, j] = ParseValue(constraintSigns[i][j], constraintCoefficients[i][j]);
                    rhs[i] = ParseValue(rhsSigns[i], constraintRHS[i]);
                    signs[i] = constraintTypes[i].SelectedItem.ToString();
                }

                bool isMax = rbMax.Checked;
                bool isArtificialMethod = cmbMethod.SelectedIndex == 1;

                LPSolver solver = new LPSolver();
                string htmlReport = solver.Solve(objective, matrix, rhs, signs, isMax, isArtificialMethod);

                await webView21.EnsureCoreWebView2Async(null);
                webView21.NavigateToString(htmlReport);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка:\n" + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ВАЖЛИВО: Переконайся, що в дизайнері кнопка "Розв'язок системи" викликає саме цей метод


        private void back_Button_Click(object sender, EventArgs e)
        {
            _originalMainScreen.Show();
            this.Close();
        }

        private async void btnSolve_Click_1(object sender, EventArgs e)
        {
            try
            {
                int vars = (int)nudVars.Value;
                int cons = (int)nudConstraints.Value;

                if (vars == 0 || cons == 0) return;

                // Локальна функція для парсингу числа з урахуванням знаку (+/-)
                double ParseValue(ComboBox signCmb, TextBox tb)
                {
                    double val = string.IsNullOrWhiteSpace(tb.Text) ? 0 : Convert.ToDouble(tb.Text);
                    return signCmb.SelectedItem.ToString() == "-" ? -val : val;
                }

                // Зчитуємо цільову функцію
                double[] objective = new double[vars];
                for (int j = 0; j < vars; j++)
                {
                    objective[j] = ParseValue(objSigns[j], objCoefficients[j]);
                }

                // Зчитуємо обмеження
                double[,] matrix = new double[cons, vars];
                double[] rhs = new double[cons];
                string[] signs = new string[cons];

                for (int i = 0; i < cons; i++)
                {
                    for (int j = 0; j < vars; j++)
                    {
                        matrix[i, j] = ParseValue(constraintSigns[i][j], constraintCoefficients[i][j]);
                    }
                    rhs[i] = ParseValue(rhsSigns[i], constraintRHS[i]);
                    signs[i] = constraintTypes[i].SelectedItem.ToString();
                }

                bool isMax = rbMax.Checked;
                bool isArtificialMethod = cmbMethod.SelectedIndex == 1; // 1 = Метод штучного базису

                // Запускаємо математичний рушій
                LPSolver solver = new LPSolver();
                string htmlReport = solver.Solve(objective, matrix, rhs, signs, isMax, isArtificialMethod);

                // Виводимо у WebView2
                await webView21.EnsureCoreWebView2Async(null);
                webView21.NavigateToString(htmlReport);
            }
            catch (FormatException)
            {
                MessageBox.Show("Перевірте правильність вводу. Всі поля повинні містити числа (або бути порожніми).", "Помилка формату", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка:\n" + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
