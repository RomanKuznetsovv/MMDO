using System;
using System.Collections.Generic;
using System.Text;

namespace Mmdo_coursework 
{
    public class GomorySolver
    {
        // ==========================================
        // ВБУДОВАНИЙ КЛАС ДЛЯ РОБОТИ ЗІ ЗВИЧАЙНИМИ ДРОБАМИ
        // ==========================================
        public struct Fraction : IComparable<Fraction>
        {
            public long Num { get; } // Чисельник
            public long Den { get; } // Знаменник

            public Fraction(long num, long den)
            {
                if (den == 0) throw new DivideByZeroException("Знаменник не може дорівнювати нулю.");
                long gcd = GCD(Math.Abs(num), Math.Abs(den));
                Num = (num * den < 0 ? -Math.Abs(num) : Math.Abs(num)) / gcd;
                Den = Math.Abs(den) / gcd;
            }

            public Fraction(long num) : this(num, 1) { }

            public static Fraction FromDouble(double val)
            {
                long sign = Math.Sign(val);
                val = Math.Abs(val);
                // Переводимо введені користувачем десяткові числа у точні дроби
                long num = (long)Math.Round(val * 10000);
                long den = 10000;
                return new Fraction(sign * num, den);
            }

            private static long GCD(long a, long b)
            {
                while (b != 0) { long temp = b; b = a % b; a = temp; }
                return a;
            }

            public static Fraction operator +(Fraction a, Fraction b)
            {
                long gcdDen = GCD(a.Den, b.Den);
                long num = a.Num * (b.Den / gcdDen) + b.Num * (a.Den / gcdDen);
                long den = a.Den / gcdDen * b.Den;
                return new Fraction(num, den);
            }

            public static Fraction operator -(Fraction a, Fraction b)
            {
                long gcdDen = GCD(a.Den, b.Den);
                long num = a.Num * (b.Den / gcdDen) - b.Num * (a.Den / gcdDen);
                long den = a.Den / gcdDen * b.Den;
                return new Fraction(num, den);
            }

            public static Fraction operator *(Fraction a, Fraction b)
            {
                long gcd1 = GCD(Math.Abs(a.Num), b.Den);
                long gcd2 = GCD(Math.Abs(b.Num), a.Den);
                return new Fraction((a.Num / gcd1) * (b.Num / gcd2), (a.Den / gcd2) * (b.Den / gcd1));
            }

            public static Fraction operator /(Fraction a, Fraction b)
            {
                if (b.Num == 0) throw new DivideByZeroException();
                long gcd1 = GCD(Math.Abs(a.Num), Math.Abs(b.Num));
                long gcd2 = GCD(Math.Abs(b.Den), a.Den);
                long sign = Math.Sign(a.Num) * Math.Sign(b.Num);
                return new Fraction(sign * (Math.Abs(a.Num) / gcd1) * (Math.Abs(b.Den) / gcd2), (a.Den / gcd2) * (Math.Abs(b.Num) / gcd1));
            }

            public static Fraction operator -(Fraction a) => new Fraction(-a.Num, a.Den);
            public static bool operator <(Fraction a, Fraction b) => a.Num * b.Den < b.Num * a.Den;
            public static bool operator >(Fraction a, Fraction b) => a.Num * b.Den > b.Num * a.Den;
            public static bool operator <=(Fraction a, Fraction b) => a.Num * b.Den <= b.Num * a.Den;
            public static bool operator >=(Fraction a, Fraction b) => a.Num * b.Den >= b.Num * a.Den;
            public static bool operator ==(Fraction a, Fraction b) => a.Num == b.Num && a.Den == b.Den;
            public static bool operator !=(Fraction a, Fraction b) => !(a == b);

            public int CompareTo(Fraction other) => (this.Num * other.Den).CompareTo(other.Num * this.Den);
            public override bool Equals(object obj) => obj is Fraction f && this == f;
            public override int GetHashCode() => (Num, Den).GetHashCode();

            public override string ToString()
            {
                if (Num == 0) return "0";
                if (Den == 1) return Num.ToString();
                return $"{Num}/{Den}";
            }

            public Fraction GetFractionalPart()
            {
                long floor = (Num >= 0) ? (Num / Den) : ((Num - Den + 1) / Den);
                return this - new Fraction(floor, 1);
            }
        }

        // ==========================================
        // ЛОГІКА СИМПЛЕКС-МЕТОДУ ТА МЕТОДУ ГОМОРІ
        // ==========================================
        private List<List<Fraction>> tableau;
        private int numOriginalVars;
        private int numConstraints;
        private List<string> varNames;
        private List<int> basis;
        private Fraction[] originalObjective;

        public string Solve(List<double[]> constraints, double[] objective)
        {
            numOriginalVars = objective.Length;
            originalObjective = new Fraction[numOriginalVars];
            for (int i = 0; i < numOriginalVars; i++) originalObjective[i] = Fraction.FromDouble(objective[i]);

            numConstraints = constraints.Count;
            tableau = new List<List<Fraction>>();
            varNames = new List<string>();
            basis = new List<int>();

            for (int j = 0; j < numOriginalVars; j++) varNames.Add($"x<sub>{j + 1}</sub>");
            for (int i = 0; i < numConstraints; i++) varNames.Add($"x<sub>{numOriginalVars + i + 1}</sub>");

            for (int i = 0; i < numConstraints; i++)
            {
                List<Fraction> row = new List<Fraction>();
                for (int j = 0; j < numOriginalVars; j++) row.Add(Fraction.FromDouble(constraints[i][j]));
                for (int j = 0; j < numConstraints; j++) row.Add(i == j ? new Fraction(1) : new Fraction(0));
                row.Add(Fraction.FromDouble(constraints[i][numOriginalVars]));
                tableau.Add(row);
                basis.Add(numOriginalVars + i);
            }

            List<Fraction> zRow = new List<Fraction>();
            for (int j = 0; j < numOriginalVars; j++) zRow.Add(Fraction.FromDouble(-objective[j]));
            for (int j = 0; j < numConstraints; j++) zRow.Add(new Fraction(0));
            zRow.Add(new Fraction(0));
            tableau.Add(zRow);

            StringBuilder html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'><style>");
            html.AppendLine("body { font-family: 'Segoe UI', Tahoma, sans-serif; padding: 10px; color: #333; background-color: #fff; font-size: 13px; line-height: 1.4; }");
            html.AppendLine("table { border-collapse: collapse; width: 100%; margin-bottom: 15px; text-align: center; font-family: Consolas, monospace; border: 1px solid #7f8c8d; }");
            html.AppendLine("th, td { border: 1px solid #bdc3c7; padding: 4px 6px; font-size: 13px; }");
            html.AppendLine("th { background-color: #f8f9fa; font-weight: bold; }");
            html.AppendLine(".pivot-row { background-color: #ffe6e6; }");
            html.AppendLine(".pivot-col { background-color: #ffe6e6; }");
            html.AppendLine(".pivot-cell { background-color: #ffb3b3; font-weight: bold; border: 2px solid #e74c3c; }");
            html.AppendLine("h2 { border-bottom: 1px solid #bdc3c7; padding-bottom: 3px; color: #2c3e50; font-size: 16px; margin-top: 20px; }");
            html.AppendLine("h3 { color: #34495e; font-size: 14px; margin-top: 15px; margin-bottom: 5px; }");
            html.AppendLine(".step-info { margin: 5px 0 15px 10px; padding-left: 10px; border-left: 3px solid #3498db; color: #555; }");
            html.AppendLine("</style></head><body>");

            html.AppendLine("<h2>ЕТАП 1: БЕЗПЕРЕРВНИЙ РОЗВ'ЯЗОК (СИМПЛЕКС-МЕТОД)</h2>");

            RunSimplex(html);

            html.AppendLine("<h2>ЕТАП 2: ЦІЛОЧИСЕЛЬНИЙ РОЗВ'ЯЗОК (МЕТОД ГОМОРІ)</h2>");

            int cutCount = 1;
            while (cutCount <= 50)
            {
                int fractionalRow = FindFractionalRow();

                if (fractionalRow == -1)
                {
                    html.AppendLine("<h3 style='color: #27ae60;'>Всі змінні цілочисельні. Оптимальний план знайдено!</h3>");
                    break;
                }

                html.AppendLine($"<h3>--- Додавання відсікання Гоморі #{cutCount} ---</h3>");
                AddGomoryCut(fractionalRow, cutCount, html);

                bool feasible = RunDualSimplex(html);
                if (!feasible)
                {
                    html.AppendLine("<h3 style='color: #c0392b;'>Рішення не існує (задача несумісна після відсікання).</h3>");
                    html.AppendLine("</body></html>");
                    return html.ToString();
                }
                cutCount++;
            }

            Fraction[] solution = GetSolution();
            html.AppendLine("<h2>ФІНАЛЬНИЙ ОПТИМАЛЬНИЙ ПЛАН</h2>");
            html.AppendLine("<ul style='list-style-type: square; background-color: #f4f6f7; padding: 15px 30px; border: 1px solid #e0e0e0;'>");
            for (int j = 0; j < numOriginalVars; j++)
            {
                html.AppendLine($"<li>П{j + 1} ({varNames[j]}) = <b>{solution[j]}</b> од.</li>");
            }
            html.AppendLine($"<li style='margin-top: 8px; font-size: 15px; color: #d35400;'><b>Максимальний прибуток = {tableau[tableau.Count - 1][tableau[0].Count - 1]} грн.</b></li>");
            html.AppendLine("</ul>");

            html.AppendLine("</body></html>");
            return html.ToString();
        }

        private void RunSimplex(StringBuilder html)
        {
            int step = 1;
            while (true)
            {
                int pivotCol = -1;
                Fraction minZ = new Fraction(0);
                int zRowIndex = tableau.Count - 1;

                for (int j = 0; j < tableau[zRowIndex].Count - 1; j++)
                {
                    if (tableau[zRowIndex][j] < minZ)
                    {
                        minZ = tableau[zRowIndex][j];
                        pivotCol = j;
                    }
                }

                if (pivotCol == -1)
                {
                    PrintTableau(html, $"Крок {step}. План оптимальний (немає від'ємних оцінок).", -1, -1);
                    break;
                }

                int pivotRow = -1;
                Fraction minRatio = new Fraction(long.MaxValue, 1);
                bool hasRatio = false;

                for (int i = 0; i < tableau.Count - 1; i++)
                {
                    if (tableau[i][pivotCol] > new Fraction(0))
                    {
                        Fraction ratio = tableau[i][tableau[i].Count - 1] / tableau[i][pivotCol];
                        if (!hasRatio || ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                            hasRatio = true;
                        }
                    }
                }

                if (pivotRow == -1) throw new Exception("Функція необмежена.");

                PrintTableau(html, $"Крок {step}. Побудуємо початкову симплекс-таблицю:", pivotRow, pivotCol);
                html.AppendLine("<div class='step-info'>");
                html.AppendLine("Поточний план неоптимальний, у рядку оцінок наявні від'ємні оцінки.<br/>");
                html.AppendLine($"Напрямний стовпець - <b>A<sub>{pivotCol + 1}</sub></b><br/>");
                html.AppendLine($"Напрямний рядок - <b>{varNames[basis[pivotRow]]}</b><br/>");
                html.AppendLine($"Напрямний елемент - <b>{tableau[pivotRow][pivotCol]}</b><br/>");
                html.AppendLine("Розрахуємо наступну симплекс-таблицю...</div>");

                Pivot(pivotRow, pivotCol);
                step++;
            }
        }

        private bool RunDualSimplex(StringBuilder html)
        {
            int step = 1;
            while (true)
            {
                int pivotRow = -1;
                Fraction minB = new Fraction(0);

                for (int i = 0; i < tableau.Count - 1; i++)
                {
                    Fraction b = tableau[i][tableau[i].Count - 1];
                    if (b < minB)
                    {
                        minB = b;
                        pivotRow = i;
                    }
                }

                if (pivotRow == -1)
                {
                    PrintTableau(html, "План став допустимим (немає від'ємних вільних членів).", -1, -1);
                    return true;
                }

                int pivotCol = -1;
                Fraction minRatio = new Fraction(long.MaxValue, 1);
                bool hasRatio = false;
                int zRowIndex = tableau.Count - 1;

                for (int j = 0; j < tableau[pivotRow].Count - 1; j++)
                {
                    if (tableau[pivotRow][j] < new Fraction(0))
                    {
                        Fraction ratio = tableau[zRowIndex][j] / tableau[pivotRow][j];
                        if (ratio.Num < 0) ratio = -ratio;

                        if (!hasRatio || ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotCol = j;
                            hasRatio = true;
                        }
                    }
                }

                if (pivotCol == -1) return false;

                PrintTableau(html, $"Двоїстий симплекс, крок {step}:", pivotRow, pivotCol);
                html.AppendLine("<div class='step-info'>");
                html.AppendLine("Опорний план недопустимий (є від'ємні вільні члени).<br/>");
                html.AppendLine($"Напрямний рядок (min b < 0) - <b>{varNames[basis[pivotRow]]}</b><br/>");
                html.AppendLine($"Напрямний стовпець - <b>A<sub>{pivotCol + 1}</sub></b><br/>");
                html.AppendLine($"Напрямний елемент - <b>{tableau[pivotRow][pivotCol]}</b></div>");

                Pivot(pivotRow, pivotCol);
                step++;
            }
        }

        private void Pivot(int row, int col)
        {
            basis[row] = col;
            Fraction pivotVal = tableau[row][col];
            for (int j = 0; j < tableau[row].Count; j++) tableau[row][j] /= pivotVal;

            for (int i = 0; i < tableau.Count; i++)
            {
                if (i != row)
                {
                    Fraction factor = tableau[i][col];
                    for (int j = 0; j < tableau[i].Count; j++) tableau[i][j] -= factor * tableau[row][j];
                }
            }
        }

        private int FindFractionalRow()
        {
            int bestRow = -1;
            Fraction maxFraction = new Fraction(0);

            for (int i = 0; i < tableau.Count - 1; i++)
            {
                if (basis[i] < numOriginalVars)
                {
                    Fraction b = tableau[i][tableau[i].Count - 1];
                    Fraction frac = b.GetFractionalPart();

                    if (frac > new Fraction(0))
                    {
                        if (frac > maxFraction)
                        {
                            maxFraction = frac;
                            bestRow = i;
                        }
                    }
                }
            }
            return bestRow;
        }

        private void AddGomoryCut(int rowIndex, int cutIndex, StringBuilder html)
        {
            int colsCount = tableau[0].Count;
            string newVarName = $"x<sub>{varNames.Count + 1}</sub>";
            varNames.Add(newVarName);

            for (int i = 0; i < tableau.Count; i++) tableau[i].Insert(colsCount - 1, new Fraction(0));

            List<Fraction> newRow = new List<Fraction>();
            for (int j = 0; j < colsCount - 1; j++)
            {
                Fraction val = tableau[rowIndex][j];
                Fraction frac = val.GetFractionalPart();
                newRow.Add(-frac);
            }

            newRow.Add(new Fraction(1));

            Fraction b = tableau[rowIndex][tableau[rowIndex].Count - 1];
            Fraction bFrac = b.GetFractionalPart();
            newRow.Add(-bFrac);

            tableau.Insert(tableau.Count - 1, newRow);
            basis.Add(varNames.Count - 1);

            html.AppendLine($"<div class='step-info'>Рядок <b>{varNames[basis[rowIndex]]}</b> має дробову частину ({bFrac}).<br/>");
            html.AppendLine($"Формуємо додаткове обмеження і вводимо змінну <b>{newVarName}</b>.</div>");
        }

        private void PrintTableau(StringBuilder html, string title, int pivotRow, int pivotCol)
        {
            html.AppendLine($"<h3>{title}</h3>");
            html.AppendLine("<table>");

            int numCols = tableau[0].Count;
            int bColIndex = numCols - 1;

            html.AppendLine("<tr>");
            html.AppendLine("<th>C</th>");
            html.AppendLine("<th>-</th>");
            for (int j = 0; j < numCols - 1; j++)
            {
                Fraction cVal = (j < numOriginalVars) ? originalObjective[j] : new Fraction(0);
                html.AppendLine($"<th>{cVal}</th>");
            }
            html.AppendLine("</tr>");

            html.AppendLine("<tr>");
            html.AppendLine("<th>B</th>");
            html.AppendLine("<th>A<sub>0</sub></th>");
            for (int j = 0; j < numCols - 1; j++)
            {
                string cssClass = (j == pivotCol) ? "class='pivot-col'" : "";
                html.AppendLine($"<th {cssClass}>A<sub>{j + 1}</sub></th>");
            }
            html.AppendLine("</tr>");

            for (int i = 0; i < tableau.Count - 1; i++)
            {
                string rowCss = (i == pivotRow) ? "class='pivot-row'" : "";
                html.AppendLine($"<tr {rowCss}>");

                html.AppendLine($"<th>{varNames[basis[i]]}</th>");
                html.AppendLine($"<td>{tableau[i][bColIndex]}</td>");

                for (int j = 0; j < numCols - 1; j++)
                {
                    string cellCss = "";
                    if (i == pivotRow && j == pivotCol) cellCss = "class='pivot-cell'";
                    else if (j == pivotCol) cellCss = "class='pivot-col'";

                    html.AppendLine($"<td {cellCss}>{tableau[i][j]}</td>");
                }
                html.AppendLine("</tr>");
            }

            html.AppendLine("<tr>");
            html.AppendLine("<th>&Delta;</th>");

            Fraction zValue = tableau[tableau.Count - 1][bColIndex];
            html.AppendLine($"<td><b>{zValue}</b></td>");

            for (int j = 0; j < numCols - 1; j++)
            {
                string cellCss = (j == pivotCol) ? "class='pivot-col'" : "";
                html.AppendLine($"<td {cellCss}><b>{tableau[tableau.Count - 1][j]}</b></td>");
            }
            html.AppendLine("</tr>");

            html.AppendLine("</table>");
        }

        private Fraction[] GetSolution()
        {
            Fraction[] solution = new Fraction[numOriginalVars];
            int bCol = tableau[0].Count - 1;

            for (int j = 0; j < numOriginalVars; j++)
            {
                solution[j] = new Fraction(0);
                for (int i = 0; i < basis.Count; i++)
                {
                    if (basis[i] == j)
                    {
                        solution[j] = tableau[i][bCol];
                        break;
                    }
                }
            }
            return solution;
        }
    }
}