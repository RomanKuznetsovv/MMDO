using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

            List<string> varList = new List<string>();
            for (int i = 1; i <= vars; i++) varList.Add($"x{i}");
            string varsStr = string.Join(", ", varList);

            int startX = 10, startY = 10, spacingX = 135, spacingY = 45;

            Label lblF = new Label { Text = $"F({varsStr}) =", Location = new Point(startX, startY + 4), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            panelInput.Controls.Add(lblF);

            int currentX = startX + lblF.PreferredWidth + 5;
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

            startY += spacingY + 15;
            int firstConstraintY = startY;

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

            Label lblNonNeg = new Label
            {
                Text = $"{varsStr} >= 0",
                Location = new Point(startX + 45, startY), 
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80)
            };
            panelInput.Controls.Add(lblNonNeg);
        }

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

                double ParseValue(ComboBox signCmb, TextBox tb)
                {
                    double val = string.IsNullOrWhiteSpace(tb.Text) ? 0 : Convert.ToDouble(tb.Text);
                    return signCmb.SelectedItem.ToString() == "-" ? -val : val;
                }

                double[] objective = new double[vars];
                for (int j = 0; j < vars; j++)
                {
                    objective[j] = ParseValue(objSigns[j], objCoefficients[j]);
                }

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

                bool requireInteger = toggleInteger.Checked;

                LPSolver solver = new LPSolver();
                string htmlReport = solver.Solve(objective, matrix, rhs, signs, isMax, requireInteger);

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