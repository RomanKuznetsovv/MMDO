using System;
using System.Collections.Generic;
using System.Text;

namespace Mmdo_coursework
{
    public class LPSolver
    {
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

            public Fraction(long num) : this(num, 1) { }

            public static Fraction FromDouble(double val)
            {
                long sign = Math.Sign(val); val = Math.Abs(val);
                long whole = (long)val;
                double frac = val - whole;
                long num = (long)Math.Round(frac * 100000);
                long den = 100000;
                return new Fraction(sign * (whole * den + num), den);
            }

            private static long GCD(long a, long b)
            {
                while (b != 0) { long temp = b; b = a % b; a = temp; }
                return a;
            }

            public Fraction GetFractionalPart()
            {
                if (Den == 1 || Num == 0) return new Fraction(0);
                long floor = (Num >= 0) ? (Num / Den) : ((Num - Den + 1) / Den);
                return this - new Fraction(floor, 1);
            }

            public static Fraction operator +(Fraction a, Fraction b)
            {
                long gcdDen = GCD(a.Den, b.Den);
                long num = a.Num * (b.Den / gcdDen) + b.Num * (a.Den / gcdDen);
                long den = (a.Den / gcdDen) * b.Den;
                return new Fraction(num, den);
            }

            public static Fraction operator -(Fraction a, Fraction b)
            {
                long gcdDen = GCD(a.Den, b.Den);
                long num = a.Num * (b.Den / gcdDen) - b.Num * (a.Den / gcdDen);
                long den = (a.Den / gcdDen) * b.Den;
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
            public static bool operator <(Fraction a, Fraction b) => (a - b).Num < 0;
            public static bool operator >(Fraction a, Fraction b) => (a - b).Num > 0;
            public static bool operator <=(Fraction a, Fraction b) => (a - b).Num <= 0;
            public static bool operator >=(Fraction a, Fraction b) => (a - b).Num >= 0;

            public static bool operator ==(Fraction a, Fraction b) => a.Num == b.Num && a.Den == b.Den;
            public static bool operator !=(Fraction a, Fraction b) => !(a == b);

            public override bool Equals(object obj) => obj is Fraction f && this == f;
            public override int GetHashCode() => (Num, Den).GetHashCode();

            public int CompareTo(Fraction other) => (this - other).Num.CompareTo(0);

            public override string ToString() => Num == 0 ? "0" : Den == 1 ? Num.ToString() : $"{Num}/{Den}";
        }

        public struct MValue : IComparable<MValue>
        {
            public Fraction Real;
            public Fraction M;

            public MValue(Fraction real, Fraction m) { Real = real; M = m; }
            public MValue(Fraction real) { Real = real; M = new Fraction(0); }

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

            public static bool operator ==(MValue a, MValue b) => a.Real == b.Real && a.M == b.M;
            public static bool operator !=(MValue a, MValue b) => !(a == b);

            public override bool Equals(object obj) => obj is MValue m && this == m;
            public override int GetHashCode() => (Real, M).GetHashCode();

            public override string ToString()
            {
                if (M.Num == 0) return Real.ToString();
                string mStr = M.Num == M.Den ? "M" : (M.Num == -M.Den ? "-M" : $"{M}M");
                if (Real.Num == 0) return mStr;
                if (M.Num > 0) return $"{Real} + {mStr}";
                return $"{Real} - {mStr.Substring(1)}";
            }
        }

        public string Solve(double[] obj, double[,] matrix, double[] rhs, string[] signs, bool isMax, bool requireInteger = false)
        {
            int numVars = obj.Length;
            int numCons = rhs.Length;

            List<string> varNames = new List<string>();
            List<int> basis = new List<int>();
            int varCounter = 1;

            for (int j = 0; j < numVars; j++) varNames.Add($"x<sub>{varCounter++}</sub>");

            List<string> origVarNames = new List<string>();
            for (int i = 1; i <= numVars; i++) origVarNames.Add($"x<sub>{i}</sub>");
            string fStringHtml = $"F({string.Join(", ", origVarNames)})";

            List<List<Fraction>> A = new List<List<Fraction>>();
            List<Fraction> B = new List<Fraction>();
            for (int i = 0; i < numCons; i++)
            {
                A.Add(new List<Fraction>());
                for (int j = 0; j < numVars; j++) A[i].Add(Fraction.FromDouble(matrix[i, j]));
                B.Add(Fraction.FromDouble(rhs[i]));
            }

            for (int i = 0; i < numCons; i++)
            {
                if (B[i] < new Fraction(0))
                {
                    B[i] = -B[i];
                    for (int j = 0; j < numVars; j++) A[i][j] = -A[i][j];
                    signs[i] = signs[i] == "<=" ? ">=" : (signs[i] == ">=" ? "<=" : "=");
                }
            }

            List<MValue> cRow = new List<MValue>();
            for (int j = 0; j < numVars; j++)
                cRow.Add(new MValue(Fraction.FromDouble(isMax ? obj[j] : -obj[j])));

            bool needsArtificial = false;

            for (int i = 0; i < numCons; i++)
            {
                if (signs[i] == "<=")
                {
                    varNames.Add($"x<sub>{varCounter++}</sub>");
                    cRow.Add(new MValue(new Fraction(0)));
                    for (int k = 0; k < numCons; k++) A[k].Add(k == i ? new Fraction(1) : new Fraction(0));
                }
                else if (signs[i] == ">=")
                {
                    varNames.Add($"x<sub>{varCounter++}</sub>");
                    cRow.Add(new MValue(new Fraction(0)));
                    for (int k = 0; k < numCons; k++) A[k].Add(k == i ? new Fraction(-1) : new Fraction(0));
                }
            }

            for (int i = 0; i < numCons; i++) basis.Add(-1);

            int currentBalancingVarIndex = numVars;
            for (int i = 0; i < numCons; i++)
            {
                if (signs[i] == "<=")
                {
                    basis[i] = currentBalancingVarIndex;
                    currentBalancingVarIndex++;
                }
                else if (signs[i] == ">=")
                {
                    currentBalancingVarIndex++;
                }
            }

            Fraction mPenalty = new Fraction(isMax ? -1 : 1);

            for (int i = 0; i < numCons; i++)
            {
                if (signs[i] == ">=" || signs[i] == "=")
                {
                    needsArtificial = true;
                    varNames.Add($"x<sub>{varCounter++}</sub>");
                    cRow.Add(new MValue(new Fraction(0), mPenalty));
                    for (int k = 0; k < numCons; k++) A[k].Add(k == i ? new Fraction(1) : new Fraction(0));
                    basis[i] = varNames.Count - 1;
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
            html.AppendLine(".step-info { margin: 10px 0 20px 0px; padding-left: 15px; border-left: 4px solid #3498db; color: #444; font-size: 14px; line-height: 1.6; }");
            html.AppendLine("</style></head><body>");

            string methodTitle = needsArtificial ? "МЕТОД ШТУЧНОГО БАЗИСУ (М-метод)" : "СИМПЛЕКС-МЕТОД";
            if (requireInteger) methodTitle += " З ВІДСІКАННЯМИ ГОМОРІ";
            html.AppendLine($"<h2>{methodTitle} - {(isMax ? "Max" : "Min")}</h2>");

            int step = 1;
            while (step < 150)
            {
                MValue[] delta = new MValue[varNames.Count + 1];
                for (int j = 0; j <= varNames.Count; j++)
                {
                    MValue sum = new MValue(new Fraction(0));
                    for (int i = 0; i < numCons; i++)
                    {
                        MValue cBasis = cRow[basis[i]];
                        Fraction aVal = tableau[i][j].Real;
                        sum = sum + new MValue(cBasis.Real * aVal, cBasis.M * aVal);
                    }
                    delta[j] = j < varNames.Count ? sum - cRow[j] : sum;
                }

                int dualPivotRow = -1;
                Fraction minB = new Fraction(0);
                for (int i = 0; i < numCons; i++)
                {
                    Fraction bVal = tableau[i][tableau[i].Count - 1].Real;
                    if (bVal < minB)
                    {
                        minB = bVal;
                        dualPivotRow = i;
                    }
                }

                if (dualPivotRow != -1)
                {
                    int dualPivotCol = -1;
                    MValue minDualRatio = new MValue(new Fraction(0));
                    bool foundDualRatio = false;

                    for (int j = 0; j < varNames.Count; j++)
                    {
                        Fraction aVal = tableau[dualPivotRow][j].Real;
                        if (aVal < new Fraction(0))
                        {
                            MValue ratio = delta[j] / (-aVal);
                            if (!foundDualRatio || ratio < minDualRatio)
                            {
                                minDualRatio = ratio;
                                dualPivotCol = j;
                                foundDualRatio = true;
                            }
                        }
                    }

                    if (dualPivotCol == -1)
                    {
                        PrintTableau(html, $"Крок {step}. Двоїстий симплекс-метод.", tableau, delta, cRow.ToArray(), varNames, basis, dualPivotRow, -1);
                        html.AppendLine("<h3 style='color:#c0392b;'>Задача не має розв'язку (немає від'ємних елементів для двоїстого кроку).</h3>");
                        break;
                    }

                    PrintTableau(html, $"Крок {step}. Двоїстий симплекс-метод:", tableau, delta, cRow.ToArray(), varNames, basis, dualPivotRow, dualPivotCol);
                    html.AppendLine("<div class='step-info'>");
                    html.AppendLine($"Знайдено від'ємний вільний член. Виконуємо крок двоїстого симплекс-методу.<br/>");
                    html.AppendLine($"Напрямний рядок - <b>{varNames[basis[dualPivotRow]]}</b><br/>");
                    html.AppendLine($"Напрямний стовпець - <b>A<sub>{dualPivotCol + 1}</sub></b><br/>");
                    html.AppendLine($"Напрямний елемент - <b>{tableau[dualPivotRow][dualPivotCol]}</b><br/>");
                    html.AppendLine("</div>");

                    basis[dualPivotRow] = dualPivotCol;
                    Fraction dualPivotVal = tableau[dualPivotRow][dualPivotCol].Real;

                    for (int j = 0; j <= varNames.Count; j++)
                        tableau[dualPivotRow][j] = tableau[dualPivotRow][j] / dualPivotVal;

                    for (int i = 0; i < numCons; i++)
                    {
                        if (i != dualPivotRow)
                        {
                            Fraction factor = tableau[i][dualPivotCol].Real;
                            for (int j = 0; j <= varNames.Count; j++)
                                tableau[i][j] = tableau[i][j] - tableau[dualPivotRow][j] * factor;
                        }
                    }
                    step++;
                    continue;
                }

                int pivotCol = -1;
                MValue minDelta = new MValue(new Fraction(0));

                for (int j = 0; j < varNames.Count; j++)
                {
                    if (delta[j] < minDelta) { minDelta = delta[j]; pivotCol = j; }
                }

                if (pivotCol == -1)
                {
                    bool hasArtificialInBasis = false;
                    for (int i = 0; i < basis.Count; i++)
                    {
                        if (cRow[basis[i]].M.Num != 0 && tableau[i][tableau[i].Count - 1].Real > new Fraction(0))
                            hasArtificialInBasis = true;
                    }

                    if (hasArtificialInBasis)
                    {
                        PrintTableau(html, $"Крок {step}. План оптимальний, але містить штучні змінні.", tableau, delta, cRow.ToArray(), varNames, basis, -1, -1);
                        html.AppendLine("<h3 style='color:#c0392b;'>Система обмежень несумісна. Задача не має розв'язку.</h3>");
                        break;
                    }

                    if (!requireInteger)
                    {
                        PrintTableau(html, $"Крок {step}. План оптимальний.", tableau, delta, cRow.ToArray(), varNames, basis, -1, -1);
                        html.AppendLine("<h3 style='color:#27ae60;'>ФІНАЛЬНИЙ ОПТИМАЛЬНИЙ ПЛАН:</h3>");
                        PrintFinalPlan(html, numVars, varNames, basis, tableau, delta, isMax, fStringHtml);
                        break;
                    }

                    int cutRow = -1;
                    Fraction maxFrac = new Fraction(0);

                    for (int i = 0; i < numCons; i++)
                    {
                        if (basis[i] < numVars)
                        {
                            Fraction bVal = tableau[i][tableau[i].Count - 1].Real;
                            Fraction frac = bVal.GetFractionalPart();

                            if (frac > new Fraction(0) && frac > maxFrac)
                            {
                                maxFrac = frac;
                                cutRow = i;
                            }
                        }
                    }

                    if (cutRow == -1)
                    {
                        PrintTableau(html, $"Крок {step}. Знайдено цілочисельний розв'язок.", tableau, delta, cRow.ToArray(), varNames, basis, -1, -1);
                        html.AppendLine("<h3 style='color:#27ae60;'>ФІНАЛЬНИЙ ЦІЛОЧИСЕЛЬНИЙ ОПТИМАЛЬНИЙ ПЛАН:</h3>");
                        PrintFinalPlan(html, numVars, varNames, basis, tableau, delta, isMax, fStringHtml);
                        break;
                    }

                    PrintTableau(html, $"Крок {step}. План оптимальний, але не цілочисельний.", tableau, delta, cRow.ToArray(), varNames, basis, -1, -1);
                    html.AppendLine($"<div class='step-info'>Додаємо відсікання Гоморі для рядка <b>{varNames[basis[cutRow]]}</b> (найбільша дробова частина {maxFrac}).</div>");

                    string sName = $"x<sub>{varCounter++}</sub>";
                    varNames.Add(sName);
                    cRow.Add(new MValue(new Fraction(0)));

                    for (int i = 0; i < numCons; i++)
                    {
                        MValue rhsVal = tableau[i][tableau[i].Count - 1];
                        tableau[i].RemoveAt(tableau[i].Count - 1);
                        tableau[i].Add(new MValue(new Fraction(0)));
                        tableau[i].Add(rhsVal);
                    }

                    List<MValue> newRow = new List<MValue>();
                    for (int j = 0; j < varNames.Count - 1; j++)
                    {
                        Fraction aVal = tableau[cutRow][j].Real;
                        newRow.Add(new MValue(-aVal.GetFractionalPart()));
                    }
                    newRow.Add(new MValue(new Fraction(1)));
                    newRow.Add(new MValue(-maxFrac));

                    tableau.Add(newRow);
                    basis.Add(varNames.Count - 1);
                    numCons++;

                    step++;
                    continue;
                }

                int pivotRow = -1;
                Fraction minRatio = new Fraction(0);
                bool foundRatio = false;

                for (int i = 0; i < numCons; i++)
                {
                    if (tableau[i][pivotCol].Real > new Fraction(0))
                    {
                        Fraction ratio = tableau[i][tableau[i].Count - 1].Real / tableau[i][pivotCol].Real;
                        if (!foundRatio || ratio < minRatio)
                        {
                            minRatio = ratio; pivotRow = i; foundRatio = true;
                        }
                        else if (ratio == minRatio)
                        {
                            if (basis[i] < basis[pivotRow]) pivotRow = i;
                        }
                    }
                }

                if (pivotRow == -1)
                {
                    PrintTableau(html, $"Крок {step}.", tableau, delta, cRow.ToArray(), varNames, basis, pivotRow, pivotCol);
                    html.AppendLine("<h3 style='color:#c0392b;'>Функція не обмежена. Оптимального розв'язку не існує.</h3>");
                    break;
                }

                PrintTableau(html, $"Крок {step}. Симплекс-метод:", tableau, delta, cRow.ToArray(), varNames, basis, pivotRow, pivotCol);

                string evalState = isMax ? "від'ємні" : "додатні";
                html.AppendLine("<div class='step-info'>");
                html.AppendLine($"Поточний план неоптимальний (є {evalState} оцінки). Виконуємо крок прямого симплекс-методу.<br/>");
                html.AppendLine($"Напрямний стовпець - <b>A<sub>{pivotCol + 1}</sub></b><br/>");
                html.AppendLine($"Напрямний рядок - <b>{varNames[basis[pivotRow]]}</b><br/>");
                html.AppendLine($"Напрямний елемент - <b>{tableau[pivotRow][pivotCol]}</b><br/>");
                html.AppendLine("</div>");

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

            if (step >= 150)
            {
                html.AppendLine("<h3 style='color:#c0392b;'>Досягнуто ліміту ітерацій. Можливо, задача зациклилася.</h3>");
            }

            html.AppendLine("</body></html>");
            return html.ToString();
        }

        private void PrintFinalPlan(StringBuilder html, int numVars, List<string> varNames, List<int> basis, List<List<MValue>> tableau, MValue[] delta, bool isMax, string fStringHtml)
        {
            html.AppendLine("<ul style='font-size:15px; background-color: #f4f6f7; padding: 15px 30px;'>");
            for (int j = 0; j < numVars; j++)
            {
                Fraction val = new Fraction(0);
                for (int i = 0; i < basis.Count; i++)
                    if (basis[i] == j) val = tableau[i][tableau[i].Count - 1].Real;

                html.AppendLine($"<li>{varNames[j]} = <b>{val}</b></li>");
            }
            Fraction zOpt = delta[varNames.Count].Real;
            if (!isMax) zOpt = -zOpt;

            html.AppendLine($"<li style='margin-top:8px; font-size: 16px;'><b>{fStringHtml} = {zOpt}</b></li></ul>");
        }

        private void PrintTableau(StringBuilder html, string title, List<List<MValue>> tab, MValue[] delta, MValue[] cRow, List<string> vNames, List<int> basis, int pRow, int pCol)
        {
            html.AppendLine($"<h3 style='margin-top:20px;'>{title}</h3><table>");

            html.AppendLine("<tr><th>C</th><th>-</th>");
            for (int j = 0; j < vNames.Count; j++) html.AppendLine($"<th>{cRow[j]}</th>");
            html.AppendLine("</tr>");

            html.AppendLine("<tr><th>B</th><th>A<sub>0</sub></th>");
            for (int j = 0; j < vNames.Count; j++)
            {
                string css = j == pCol ? "class='pivot-col'" : "";
                html.AppendLine($"<th {css}>A<sub>{j + 1}</sub></th>");
            }
            html.AppendLine("</tr>");

            for (int i = 0; i < tab.Count; i++)
            {
                string rCss = i == pRow ? "class='pivot-row'" : "";
                html.AppendLine($"<tr {rCss}>");

                html.AppendLine($"<th>{vNames[basis[i]]}</th>");
                html.AppendLine($"<td>{tab[i][tab[i].Count - 1]}</td>");

                for (int j = 0; j < vNames.Count; j++)
                {
                    string cCss = (i == pRow && j == pCol) ? "class='pivot-cell'" : (j == pCol ? "class='pivot-col'" : "");
                    html.AppendLine($"<td {cCss}>{tab[i][j]}</td>");
                }
                html.AppendLine("</tr>");
            }

            html.AppendLine("<tr><th>&Delta;</th>");
            html.AppendLine($"<td><b>{delta[vNames.Count]}</b></td>");
            for (int j = 0; j < vNames.Count; j++)
            {
                string css = j == pCol ? "class='pivot-col'" : "";
                html.AppendLine($"<td {css}><b>{delta[j]}</b></td>");
            }
            html.AppendLine("</tr></table>");
        }
    }
}