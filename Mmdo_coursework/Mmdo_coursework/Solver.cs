using System;
using System.Collections.Generic;
using System.Text;

namespace Mmdo_coursework
{
    public class Solver
    {
        public string Solve(double[] obj, double[,] matrix, double[] rhs, string[] signs, bool isMax, bool requireInteger = false)
        {
            int numVars = obj.Length;
            int numCons = rhs.Length;
            int varCounter = 1;

            List<string> varNames = new List<string>();
            List<int> basis = new List<int>();

            for (int j = 0; j < numVars; j++) varNames.Add($"x<sub>{varCounter++}</sub>");

            List<string> origVarNames = new List<string>();
            for (int i = 1; i <= numVars; i++) origVarNames.Add($"x<sub>{i}</sub>");
            string fStringHtml = $"F({string.Join(", ", origVarNames)})";

            List<List<Fraction>> A = InitializeMatrix(matrix, numVars, numCons);
            List<Fraction> B = InitializeRHS(rhs, A, signs, numVars, numCons);
            List<MValue> cRow = InitializeObjectiveRow(obj, numVars, isMax);

            bool needsArtificial = SetupInitialBasis(signs, numCons, ref varCounter, varNames, cRow, A, basis, isMax);

            List<List<MValue>> tableau = CreateInitialTableau(A, B, varNames.Count, numCons);

            StringBuilder html = BuildHtmlHeader(needsArtificial, isMax, requireInteger);

            int step = 1;
            while (step < 150)
            {
                MValue[] delta = CalculateDeltas(tableau, cRow, basis, varNames.Count, numCons);

                if (TryFindDualPivot(tableau, delta, numCons, varNames.Count, out int dualPivotRow, out int dualPivotCol))
                {
                    if (dualPivotCol == -1)
                    {
                        PrintTableau(html, $"Крок {step}. Двоїстий симплекс-метод.", tableau, delta, cRow.ToArray(), varNames, basis, dualPivotRow, -1);
                        html.AppendLine("<h3 style='color:#c0392b;'>Задача не має розв'язку (немає від'ємних елементів для двоїстого кроку).</h3>");
                        break;
                    }

                    PrintDualStepInfo(html, step, tableau, delta, cRow, varNames, basis, dualPivotRow, dualPivotCol);
                    PerformPivotStep(tableau, basis, dualPivotRow, dualPivotCol, varNames.Count, numCons);
                    step++;
                    continue;
                }

                int pivotCol = FindDirectPivotColumn(delta, varNames.Count);

                if (pivotCol == -1)
                {
                    if (CheckArtificialVariablesInBasis(cRow, basis, tableau))
                    {
                        PrintTableau(html, $"Крок {step}. План оптимальний, але містить штучні змінні.", tableau, delta, cRow.ToArray(), varNames, basis, -1, -1);
                        html.AppendLine("<h3 style='color:#c0392b;'>Система обмежень несумісна. Задача не має розв'язку.</h3>");
                        break;
                    }

                    if (!requireInteger)
                    {
                        PrintOptimalPlan(html, step, tableau, delta, cRow, varNames, basis, numVars, isMax, fStringHtml);
                        break;
                    }

                    if (TryFindFractionalRow(tableau, basis, numVars, numCons, out int cutRow, out Fraction maxFrac))
                    {
                        PrintTableau(html, $"Крок {step}. План оптимальний, але не цілочисельний.", tableau, delta, cRow.ToArray(), varNames, basis, -1, -1);
                        html.AppendLine($"<div class='step-info'>Додаємо відсікання Гоморі для рядка <b>{varNames[basis[cutRow]]}</b> (найбільша дробова частина {maxFrac}).</div>");

                        AddGomoryCut(tableau, cRow, varNames, basis, ref numCons, ref varCounter, cutRow, maxFrac);
                        step++;
                        continue;
                    }

                    PrintIntegerOptimalPlan(html, step, tableau, delta, cRow, varNames, basis, numVars, isMax, fStringHtml);
                    break;
                }

                int pivotRow = FindDirectPivotRow(tableau, basis, numCons, pivotCol);

                if (pivotRow == -1)
                {
                    PrintTableau(html, $"Крок {step}.", tableau, delta, cRow.ToArray(), varNames, basis, pivotRow, pivotCol);
                    html.AppendLine("<h3 style='color:#c0392b;'>Функція не обмежена. Оптимального розв'язку не існує.</h3>");
                    break;
                }

                PrintDirectStepInfo(html, step, tableau, delta, cRow, varNames, basis, pivotRow, pivotCol, isMax);
                PerformPivotStep(tableau, basis, pivotRow, pivotCol, varNames.Count, numCons);
                step++;
            }

            if (step >= 150)
            {
                html.AppendLine("<h3 style='color:#c0392b;'>Досягнуто ліміту ітерацій. Можливо, задача зациклилася.</h3>");
            }

            html.AppendLine("</body></html>");
            return html.ToString();
        }

        private List<List<Fraction>> InitializeMatrix(double[,] matrix, int numVars, int numCons)
        {
            var A = new List<List<Fraction>>();
            for (int i = 0; i < numCons; i++)
            {
                A.Add(new List<Fraction>());
                for (int j = 0; j < numVars; j++) A[i].Add(Fraction.FromDouble(matrix[i, j]));
            }
            return A;
        }

        private List<Fraction> InitializeRHS(double[] rhs, List<List<Fraction>> A, string[] signs, int numVars, int numCons)
        {
            var B = new List<Fraction>();
            for (int i = 0; i < numCons; i++) B.Add(Fraction.FromDouble(rhs[i]));

            for (int i = 0; i < numCons; i++)
            {
                if (B[i] < new Fraction(0))
                {
                    B[i] = -B[i];
                    for (int j = 0; j < numVars; j++) A[i][j] = -A[i][j];
                    signs[i] = signs[i] == "<=" ? ">=" : (signs[i] == ">=" ? "<=" : "=");
                }
            }
            return B;
        }

        private List<MValue> InitializeObjectiveRow(double[] obj, int numVars, bool isMax)
        {
            var cRow = new List<MValue>();
            for (int j = 0; j < numVars; j++)
                cRow.Add(new MValue(Fraction.FromDouble(isMax ? obj[j] : -obj[j])));
            return cRow;
        }

        private bool SetupInitialBasis(string[] signs, int numCons, ref int varCounter, List<string> varNames, List<MValue> cRow, List<List<Fraction>> A, List<int> basis, bool isMax)
        {
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

            int currentBalancingVarIndex = cRow.Count - numCons;
            int indexTracker = currentBalancingVarIndex;
            for (int i = 0; i < numCons; i++)
            {
                if (signs[i] == "<=") { basis[i] = indexTracker++; }
                else if (signs[i] == ">=") { indexTracker++; }
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

            return needsArtificial;
        }

        private List<List<MValue>> CreateInitialTableau(List<List<Fraction>> A, List<Fraction> B, int totalVars, int numCons)
        {
            var tableau = new List<List<MValue>>();
            for (int i = 0; i < numCons; i++)
            {
                List<MValue> row = new List<MValue>();
                for (int j = 0; j < totalVars; j++) row.Add(new MValue(A[i][j]));
                row.Add(new MValue(B[i]));
                tableau.Add(row);
            }
            return tableau;
        }

        private MValue[] CalculateDeltas(List<List<MValue>> tableau, List<MValue> cRow, List<int> basis, int totalVars, int numCons)
        {
            MValue[] delta = new MValue[totalVars + 1];
            for (int j = 0; j <= totalVars; j++)
            {
                MValue sum = new MValue(new Fraction(0));
                for (int i = 0; i < numCons; i++)
                {
                    MValue cBasis = cRow[basis[i]];
                    Fraction aVal = tableau[i][j].Real;
                    sum = sum + new MValue(cBasis.Real * aVal, cBasis.M * aVal);
                }
                delta[j] = j < totalVars ? sum - cRow[j] : sum;
            }
            return delta;
        }

        private bool TryFindDualPivot(List<List<MValue>> tableau, MValue[] delta, int numCons, int totalVars, out int dualPivotRow, out int dualPivotCol)
        {
            dualPivotRow = -1;
            dualPivotCol = -1;
            Fraction minB = new Fraction(0);

            for (int i = 0; i < numCons; i++)
            {
                Fraction bVal = tableau[i][tableau[i].Count - 1].Real;
                if (bVal < minB) { minB = bVal; dualPivotRow = i; }
            }

            if (dualPivotRow == -1) return false;

            MValue minDualRatio = new MValue(new Fraction(0));
            bool foundDualRatio = false;

            for (int j = 0; j < totalVars; j++)
            {
                Fraction aVal = tableau[dualPivotRow][j].Real;
                if (aVal < new Fraction(0))
                {
                    MValue ratio = delta[j] / (-aVal);
                    if (!foundDualRatio || ratio < minDualRatio)
                    {
                        minDualRatio = ratio; dualPivotCol = j; foundDualRatio = true;
                    }
                }
            }
            return true;
        }

        private int FindDirectPivotColumn(MValue[] delta, int totalVars)
        {
            int pivotCol = -1;
            MValue minDelta = new MValue(new Fraction(0));

            for (int j = 0; j < totalVars; j++)
            {
                if (delta[j] < minDelta) { minDelta = delta[j]; pivotCol = j; }
            }
            return pivotCol;
        }

        private int FindDirectPivotRow(List<List<MValue>> tableau, List<int> basis, int numCons, int pivotCol)
        {
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
                    else if (ratio == minRatio && basis[i] < basis[pivotRow])
                    {
                        pivotRow = i;
                    }
                }
            }
            return pivotRow;
        }

        private void PerformPivotStep(List<List<MValue>> tableau, List<int> basis, int pivotRow, int pivotCol, int totalVars, int numCons)
        {
            basis[pivotRow] = pivotCol;
            Fraction pivotVal = tableau[pivotRow][pivotCol].Real;

            for (int j = 0; j <= totalVars; j++)
                tableau[pivotRow][j] = tableau[pivotRow][j] / pivotVal;

            for (int i = 0; i < numCons; i++)
            {
                if (i != pivotRow)
                {
                    Fraction factor = tableau[i][pivotCol].Real;
                    for (int j = 0; j <= totalVars; j++)
                        tableau[i][j] = tableau[i][j] - tableau[pivotRow][j] * factor;
                }
            }
        }

        private bool CheckArtificialVariablesInBasis(List<MValue> cRow, List<int> basis, List<List<MValue>> tableau)
        {
            for (int i = 0; i < basis.Count; i++)
            {
                if (cRow[basis[i]].M.Num != 0 && tableau[i][tableau[i].Count - 1].Real > new Fraction(0))
                    return true;
            }
            return false;
        }

        private bool TryFindFractionalRow(List<List<MValue>> tableau, List<int> basis, int numVars, int numCons, out int cutRow, out Fraction maxFrac)
        {
            cutRow = -1;
            maxFrac = new Fraction(0);

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
            return cutRow != -1;
        }

        private void AddGomoryCut(List<List<MValue>> tableau, List<MValue> cRow, List<string> varNames, List<int> basis, ref int numCons, ref int varCounter, int cutRow, Fraction maxFrac)
        {
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
        }

        private StringBuilder BuildHtmlHeader(bool needsArtificial, bool isMax, bool requireInteger)
        {
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

            return html;
        }

        private void PrintDualStepInfo(StringBuilder html, int step, List<List<MValue>> tableau, MValue[] delta, List<MValue> cRow, List<string> varNames, List<int> basis, int dualPivotRow, int dualPivotCol)
        {
            PrintTableau(html, $"Крок {step}. Двоїстий симплекс-метод:", tableau, delta, cRow.ToArray(), varNames, basis, dualPivotRow, dualPivotCol);
            html.AppendLine("<div class='step-info'>");
            html.AppendLine($"Знайдено від'ємний вільний член. Виконуємо крок двоїстого симплекс-методу.<br/>");
            html.AppendLine($"Напрямний рядок - <b>{varNames[basis[dualPivotRow]]}</b><br/>");
            html.AppendLine($"Напрямний стовпець - <b>A<sub>{dualPivotCol + 1}</sub></b><br/>");
            html.AppendLine($"Напрямний елемент - <b>{tableau[dualPivotRow][dualPivotCol]}</b><br/>");
            html.AppendLine("</div>");
        }

        private void PrintDirectStepInfo(StringBuilder html, int step, List<List<MValue>> tableau, MValue[] delta, List<MValue> cRow, List<string> varNames, List<int> basis, int pivotRow, int pivotCol, bool isMax)
        {
            PrintTableau(html, $"Крок {step}. Симплекс-метод:", tableau, delta, cRow.ToArray(), varNames, basis, pivotRow, pivotCol);
            string evalState = isMax ? "від'ємні" : "додатні";
            html.AppendLine("<div class='step-info'>");
            html.AppendLine($"Поточний план неоптимальний (є {evalState} оцінки). Виконуємо крок прямого симплекс-методу.<br/>");
            html.AppendLine($"Напрямний стовпець - <b>A<sub>{pivotCol + 1}</sub></b><br/>");
            html.AppendLine($"Напрямний рядок - <b>{varNames[basis[pivotRow]]}</b><br/>");
            html.AppendLine($"Напрямний елемент - <b>{tableau[pivotRow][pivotCol]}</b><br/>");
            html.AppendLine("</div>");
        }

        private void PrintOptimalPlan(StringBuilder html, int step, List<List<MValue>> tableau, MValue[] delta, List<MValue> cRow, List<string> varNames, List<int> basis, int numVars, bool isMax, string fStringHtml)
        {
            PrintTableau(html, $"Крок {step}. План оптимальний.", tableau, delta, cRow.ToArray(), varNames, basis, -1, -1);
            html.AppendLine("<h3 style='color:#27ae60;'>ФІНАЛЬНИЙ ОПТИМАЛЬНИЙ ПЛАН:</h3>");
            PrintFinalPlan(html, numVars, varNames, basis, tableau, delta, isMax, fStringHtml);
        }

        private void PrintIntegerOptimalPlan(StringBuilder html, int step, List<List<MValue>> tableau, MValue[] delta, List<MValue> cRow, List<string> varNames, List<int> basis, int numVars, bool isMax, string fStringHtml)
        {
            PrintTableau(html, $"Крок {step}. Знайдено цілочисельний розв'язок.", tableau, delta, cRow.ToArray(), varNames, basis, -1, -1);
            html.AppendLine("<h3 style='color:#27ae60;'>ФІНАЛЬНИЙ ЦІЛОЧИСЕЛЬНИЙ ОПТИМАЛЬНИЙ ПЛАН:</h3>");
            PrintFinalPlan(html, numVars, varNames, basis, tableau, delta, isMax, fStringHtml);
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