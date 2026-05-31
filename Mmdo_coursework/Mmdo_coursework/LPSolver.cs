using System;
using System.Collections.Generic;
using System.Text;

namespace Mmdo_coursework // Увага: перевір свій namespace
{
    public class LPSolver
    {
        // ---------------------------------------------------------
        // 1. ДРОБИ
        // ---------------------------------------------------------
        public struct Fraction : IComparable<Fraction>
        {
            public long Num { get; }
            public long Den { get; }
            public Fraction(long num, long den)
            {
                if (den == 0) throw new DivideByZeroException();
                long gcd = GCD(Math.Abs(num), Math.Abs(den));
                Num = (num * den < 0 ? -Math.Abs(num) : Math.Abs(num)) / gcd;
                Den = Math.Abs(den) / gcd;
            }
            public static Fraction FromDouble(double val)
            {
                long sign = Math.Sign(val); val = Math.Abs(val);
                return new Fraction(sign * (long)Math.Round(val * 10000), 10000);
            }
            private static long GCD(long a, long b) { while (b != 0) { long temp = b; b = a % b; a = temp; } return a; }
            public static Fraction operator +(Fraction a, Fraction b) => new Fraction(a.Num * b.Den + b.Num * a.Den, a.Den * b.Den);
            public static Fraction operator -(Fraction a, Fraction b) => new Fraction(a.Num * b.Den - b.Num * a.Den, a.Den * b.Den);
            public static Fraction operator *(Fraction a, Fraction b) => new Fraction(a.Num * b.Num, a.Den * b.Den);
            public static Fraction operator /(Fraction a, Fraction b) => new Fraction(a.Num * b.Den, a.Den * b.Num);
            public static Fraction operator -(Fraction a) => new Fraction(-a.Num, a.Den);
            public static bool operator <(Fraction a, Fraction b) => a.Num * b.Den < b.Num * a.Den;
            public static bool operator >(Fraction a, Fraction b) => a.Num * b.Den > b.Num * a.Den;
            public int CompareTo(Fraction other) => (this.Num * other.Den).CompareTo(other.Num * this.Den);
            public override string ToString() => Num == 0 ? "0" : Den == 1 ? Num.ToString() : $"{Num}/{Den}";
        }

        // ---------------------------------------------------------
        // 2. ЧИСЛО З 'M'
        // ---------------------------------------------------------
        public struct MValue : IComparable<MValue>
        {
            public Fraction Real;
            public Fraction M;

            public MValue(Fraction real, Fraction m) { Real = real; M = m; }
            public MValue(Fraction real) { Real = real; M = new Fraction(0, 1); }

            public static MValue operator +(MValue a, MValue b) => new MValue(a.Real + b.Real, a.M + b.M);
            public static MValue operator -(MValue a, MValue b) => new MValue(a.Real - b.Real, a.M - b.M);
            public static MValue operator *(MValue a, Fraction scalar) => new MValue(a.Real * scalar, a.M * scalar);
            public static MValue operator /(MValue a, Fraction scalar) => new MValue(a.Real / scalar, a.M / scalar);

            public int CompareTo(MValue other)
            {
                int mCmp = M.CompareTo(other.M);
                return mCmp != 0 ? mCmp : Real.CompareTo(other.Real);
            }
            public static bool operator <(MValue a, MValue b) => a.CompareTo(b) < 0;
            public static bool operator >(MValue a, MValue b) => a.CompareTo(b) > 0;

            public override string ToString()
            {
                if (M.Num == 0) return Real.ToString(); // Звичайне число
                string mStr = M.Num == M.Den ? "M" : (M.Num == -M.Den ? "-M" : $"{M}M");
                if (Real.Num == 0) return mStr; // Лише M
                if (M.Num > 0) return $"{Real} + {mStr}";
                return $"{Real} - {mStr.Substring(1)}";
            }
        }

        // ---------------------------------------------------------
        // 3. ОСНОВНИЙ АЛГОРИТМ
        // ---------------------------------------------------------
        public string Solve(double[] obj, double[,] matrix, double[] rhs, string[] signs, bool isMax, bool forceArtificial)
        {
            int numVars = obj.Length;
            int numCons = rhs.Length;

            List<string> varNames = new List<string>();
            List<int> basis = new List<int>();
            int varCounter = 1;

            // Основні змінні x1, x2...
            for (int j = 0; j < numVars; j++) varNames.Add($"x<sub>{varCounter++}</sub>");

            List<List<Fraction>> A = new List<List<Fraction>>();
            List<Fraction> B = new List<Fraction>();
            for (int i = 0; i < numCons; i++)
            {
                A.Add(new List<Fraction>());
                for (int j = 0; j < numVars; j++) A[i].Add(Fraction.FromDouble(matrix[i, j]));
                B.Add(Fraction.FromDouble(rhs[i]));
            }

            // Нормалізація (позбавлення від'ємних вільних членів)
            for (int i = 0; i < numCons; i++)
            {
                if (B[i] < new Fraction(0, 1))
                {
                    B[i] = -B[i];
                    for (int j = 0; j < numVars; j++) A[i][j] = -A[i][j];
                    signs[i] = signs[i] == "<=" ? ">=" : (signs[i] == ">=" ? "<=" : "=");
                }
            }

            List<MValue> cRow = new List<MValue>();
            for (int j = 0; j < numVars; j++)
                cRow.Add(new MValue(Fraction.FromDouble(isMax ? obj[j] : -obj[j])));

            bool needsArtificial = forceArtificial;

            // Додаємо балансуючі та штучні змінні (всі вони будуть x3, x4, x5...)
            for (int i = 0; i < numCons; i++)
            {
                if (signs[i] == "<=")
                {
                    varNames.Add($"x<sub>{varCounter++}</sub>");
                    cRow.Add(new MValue(new Fraction(0, 1)));
                    for (int k = 0; k < numCons; k++) A[k].Add(k == i ? new Fraction(1, 1) : new Fraction(0, 1));
                    basis.Add(varNames.Count - 1);
                }
                else if (signs[i] == ">=")
                {
                    needsArtificial = true;
                    // Балансуюча
                    varNames.Add($"x<sub>{varCounter++}</sub>");
                    cRow.Add(new MValue(new Fraction(0, 1)));
                    for (int k = 0; k < numCons; k++) A[k].Add(k == i ? new Fraction(-1, 1) : new Fraction(0, 1));

                    // Штучна
                    varNames.Add($"x<sub>{varCounter++}</sub>"); // Штучним також даємо x_i
                    cRow.Add(new MValue(new Fraction(0, 1), new Fraction(-1, 1))); // Штраф -M
                    for (int k = 0; k < numCons; k++) A[k].Add(k == i ? new Fraction(1, 1) : new Fraction(0, 1));
                    basis.Add(varNames.Count - 1);
                }
                else // "="
                {
                    needsArtificial = true;
                    varNames.Add($"x<sub>{varCounter++}</sub>");
                    cRow.Add(new MValue(new Fraction(0, 1), new Fraction(-1, 1))); // Штраф -M
                    for (int k = 0; k < numCons; k++) A[k].Add(k == i ? new Fraction(1, 1) : new Fraction(0, 1));
                    basis.Add(varNames.Count - 1);
                }
            }

            List<List<MValue>> tableau = new List<List<MValue>>();
            for (int i = 0; i < numCons; i++)
            {
                List<MValue> row = new List<MValue>();
                for (int j = 0; j < varNames.Count; j++) row.Add(new MValue(A[i][j]));
                row.Add(new MValue(B[i]));
                tableau.Add(row);
            }

            StringBuilder html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'><style>");
            html.AppendLine("body { font-family: 'Segoe UI', Tahoma, sans-serif; padding: 10px; font-size: 13px; color: #2c3e50;}");
            html.AppendLine("table { border-collapse: collapse; width: 100%; margin-bottom: 15px; text-align: center; border: 1px solid #7f8c8d; font-family: Consolas, monospace;}");
            html.AppendLine("th, td { border: 1px solid #bdc3c7; padding: 6px; } th { background-color: #f8f9fa; }");
            html.AppendLine(".pivot-row { background-color: #ffe6e6; } .pivot-col { background-color: #ffe6e6; } .pivot-cell { background-color: #ffb3b3; font-weight: bold; border: 2px solid #e74c3c; }");
            html.AppendLine("h2 { border-bottom: 1px solid #bdc3c7; padding-bottom: 5px; color: #2980b9; font-size: 16px; margin-top: 20px;}");
            html.AppendLine(".step-info { margin: 5px 0 15px 10px; padding-left: 10px; border-left: 3px solid #3498db; color: #555; }");
            html.AppendLine("</style></head><body>");

            string methodTitle = needsArtificial ? "МЕТОД ШТУЧНОГО БАЗИСУ (М-метод)" : "СИМПЛЕКС-МЕТОД";
            html.AppendLine($"<h2>{methodTitle} - {(isMax ? "Максимізація" : "Мінімізація")}</h2>");

            int step = 1;
            while (step < 30) // Запобіжник від нескінченних циклів
            {
                MValue[] delta = new MValue[varNames.Count + 1];
                for (int j = 0; j <= varNames.Count; j++)
                {
                    MValue sum = new MValue(new Fraction(0, 1));
                    for (int i = 0; i < numCons; i++)
                    {
                        MValue cBasis = cRow[basis[i]];
                        Fraction aVal = tableau[i][j].Real;
                        sum = sum + new MValue(cBasis.Real * aVal, cBasis.M * aVal);
                    }
                    delta[j] = j < varNames.Count ? sum - cRow[j] : sum;
                }

                int pivotCol = -1;
                MValue minDelta = new MValue(new Fraction(0, 1));

                for (int j = 0; j < varNames.Count; j++)
                {
                    if (delta[j] < minDelta) { minDelta = delta[j]; pivotCol = j; }
                }

                if (pivotCol == -1) // Оптимальний план знайдено
                {
                    PrintTableau(html, $"Крок {step}. План оптимальний.", tableau, delta, cRow.ToArray(), varNames, basis, -1, -1);

                    // Перевірка на сумісність при штучному базисі
                    bool hasArtificialInBasis = false;
                    for (int i = 0; i < basis.Count; i++)
                    {
                        if (cRow[basis[i]].M < new Fraction(0, 1) && tableau[i][tableau[i].Count - 1].Real > new Fraction(0, 1))
                            hasArtificialInBasis = true;
                    }

                    if (hasArtificialInBasis)
                    {
                        html.AppendLine("<h3 style='color:#c0392b;'>Система обмежень несумісна. Задача не має розв'язку.</h3>");
                    }
                    else
                    {
                        html.AppendLine("<h3 style='color:#27ae60;'>ФІНАЛЬНИЙ ОПТИМАЛЬНИЙ ПЛАН:</h3><ul style='font-size:15px; background-color: #f4f6f7; padding: 15px 30px;'>");
                        for (int j = 0; j < numVars; j++) // Виводимо тільки початкові змінні x1, x2...
                        {
                            Fraction val = new Fraction(0, 1);
                            for (int i = 0; i < basis.Count; i++) if (basis[i] == j) val = tableau[i][tableau[i].Count - 1].Real;
                            html.AppendLine($"<li>{varNames[j]} = <b>{val}</b></li>");
                        }
                        Fraction zOpt = delta[varNames.Count].Real;
                        if (!isMax) zOpt = -zOpt;
                        html.AppendLine($"<li style='margin-top:8px;'><b>Z = {zOpt}</b></li></ul>");
                    }
                    break;
                }

                int pivotRow = -1;
                Fraction minRatio = new Fraction(long.MaxValue, 1);
                bool foundRatio = false;

                for (int i = 0; i < numCons; i++)
                {
                    if (tableau[i][pivotCol].Real > new Fraction(0, 1))
                    {
                        Fraction ratio = tableau[i][tableau[i].Count - 1].Real / tableau[i][pivotCol].Real;
                        if (!foundRatio || ratio < minRatio) { minRatio = ratio; pivotRow = i; foundRatio = true; }
                    }
                }

                if (pivotRow == -1)
                {
                    PrintTableau(html, $"Крок {step}.", tableau, delta, cRow.ToArray(), varNames, basis, pivotRow, pivotCol);
                    html.AppendLine("<h3 style='color:#c0392b;'>Функція не обмежена. Оптимального розв'язку не існує.</h3>");
                    break;
                }

                PrintTableau(html, $"Крок {step}.", tableau, delta, cRow.ToArray(), varNames, basis, pivotRow, pivotCol);
                html.AppendLine($"<div class='step-info'>Напрямний стовпець: <b>A<sub>{pivotCol + 1}</sub></b>. Напрямний рядок: <b>{varNames[basis[pivotRow]]}</b>.<br/>Напрямний елемент: <b>{tableau[pivotRow][pivotCol]}</b>.</div>");

                // Перерахунок таблиці 
                basis[pivotRow] = pivotCol;
                Fraction pivotVal = tableau[pivotRow][pivotCol].Real;

                for (int j = 0; j <= varNames.Count; j++)
                    tableau[pivotRow][j] = tableau[pivotRow][j] / pivotVal;

                for (int i = 0; i < numCons; i++)
                {
                    if (i != pivotRow)
                    {
                        Fraction factor = tableau[i][pivotCol].Real;
                        for (int j = 0; j <= varNames.Count; j++)
                            tableau[i][j] = tableau[i][j] - tableau[pivotRow][j] * factor;
                    }
                }
                step++;
            }

            html.AppendLine("</body></html>");
            return html.ToString();
        }

        private void PrintTableau(StringBuilder html, string title, List<List<MValue>> tab, MValue[] delta, MValue[] cRow, List<string> vNames, List<int> basis, int pRow, int pCol)
        {
            html.AppendLine($"<h3 style='margin-top:20px;'>{title}</h3><table>");

            // ================= РЯДОК 1: C =================
            html.AppendLine("<tr><th>C</th><th>-</th>");
            for (int j = 0; j < vNames.Count; j++) html.AppendLine($"<th>{cRow[j]}</th>");
            html.AppendLine("</tr>");

            // ================= РЯДОК 2: B та A0, A1... =================
            html.AppendLine("<tr><th>B</th><th>A<sub>0</sub></th>");
            for (int j = 0; j < vNames.Count; j++)
            {
                string css = j == pCol ? "class='pivot-col'" : "";
                html.AppendLine($"<th {css}>A<sub>{j + 1}</sub></th>");
            }
            html.AppendLine("</tr>");

            // ================= РЯДКИ З ДАНИМИ =================
            for (int i = 0; i < tab.Count; i++)
            {
                string rCss = i == pRow ? "class='pivot-row'" : "";
                html.AppendLine($"<tr {rCss}>");

                // Колонка базису
                html.AppendLine($"<th>{vNames[basis[i]]}</th>");

                // Колонка A0
                html.AppendLine($"<td>{tab[i][tab[i].Count - 1]}</td>");

                // Колонки матриці
                for (int j = 0; j < vNames.Count; j++)
                {
                    string cCss = (i == pRow && j == pCol) ? "class='pivot-cell'" : (j == pCol ? "class='pivot-col'" : "");
                    html.AppendLine($"<td {cCss}>{tab[i][j]}</td>");
                }
                html.AppendLine("</tr>");
            }

            // ================= РЯДОК ДЕЛЬТА =================
            html.AppendLine("<tr><th>&Delta;</th>");
            html.AppendLine($"<td><b>{delta[vNames.Count]}</b></td>"); // Z значення під A0
            for (int j = 0; j < vNames.Count; j++)
            {
                string css = j == pCol ? "class='pivot-col'" : "";
                html.AppendLine($"<td {css}><b>{delta[j]}</b></td>");
            }
            html.AppendLine("</tr></table>");
        }
    }
}