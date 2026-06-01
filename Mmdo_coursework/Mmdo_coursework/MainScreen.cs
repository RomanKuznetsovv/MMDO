using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mmdo_coursework
{
    public partial class MainScreen : Form
    {
        public MainScreen()
        {

            InitializeComponent();

            LoadInitialData();
        }

        private void LoadInitialData()
        {

            if (data.Columns.Count < 4)
            {
                MessageBox.Show("Будь ласка, переконайтеся, що в DataGridView створено 4 колонки.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            data.Rows.Clear();
            data.Rows.Add("S1", "19", "2", "3");
            data.Rows.Add("S2", "13", "2", "1");
            data.Rows.Add("S3", "15", "0", "3");
            data.Rows.Add("S4", "18", "3", "0");
            int profitRow = data.Rows.Add("Прибуток", "0", "7", "5");
            data.Rows[profitRow].DefaultCellStyle.BackColor = Color.LightYellow;
        }

        private async void btnSolve_Click_1(object sender, EventArgs e)
        {
            try
            {
                List<double[]> constraints = new List<double[]>();
                double[] objective = new double[2];
                bool profitFound = false;

                for (int i = 0; i < data.Rows.Count; i++)
                {
                    if (data.Rows[i].IsNewRow) continue;

                    string resName = data.Rows[i].Cells[0].Value?.ToString() ?? "";

                    if (string.IsNullOrWhiteSpace(resName)) continue;

                    if (resName.ToLower().Contains("прибуток"))
                    {
                        objective[0] = Convert.ToDouble(data.Rows[i].Cells[2].Value ?? "0");
                        objective[1] = Convert.ToDouble(data.Rows[i].Cells[3].Value ?? "0");
                        profitFound = true;
                        continue;
                    }

                    double b = Convert.ToDouble(data.Rows[i].Cells[1].Value ?? "0");
                    double x1 = Convert.ToDouble(data.Rows[i].Cells[2].Value ?? "0");
                    double x2 = Convert.ToDouble(data.Rows[i].Cells[3].Value ?? "0");

                    constraints.Add(new double[] { x1, x2, b });
                }

                if (!profitFound)
                {
                    MessageBox.Show("Не вдалося знайти рядок 'Прибуток' у таблиці. Алгоритм не може продовжити роботу.", "Помилка даних", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                GomorySolver solver = new GomorySolver();

                string htmlResult = solver.Solve(constraints, objective);

                // ==========================================
                // ВИВЕДЕННЯ РЕЗУЛЬТАТУ ЧЕРЕЗ WEBVIEW2
                // ==========================================

                await webView21.EnsureCoreWebView2Async(null);

                webView21.NavigateToString(htmlResult);
            }
            catch (FormatException)
            {
                MessageBox.Show("Помилка формату даних! Будь ласка, переконайтеся, що в таблиці введені лише числа.", "Помилка вводу", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася непередбачена помилка під час розрахунку:\n" + ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void systemSolutionBut_Click(object sender, EventArgs e)
        {
            SystemSolutionForm solutionWindow = new SystemSolutionForm(this);
            solutionWindow.Show();
            //this.Hide();
        }

       
    }
}