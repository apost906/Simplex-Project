﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using RaikesSimplexService.Contracts;
using RaikesSimplexService.DataModel;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace RaikesSimplexService.InsertTeamNameHere
{
    
    public class Solver : ISolver
    {

        int sCount = 0;
        int aCount = 0;
        public Solution Solve(Model model)
        {
            
            convertAllInequalities(model);
            double[] basicVariableIndecies = basicColumnIndecies(model);
            Matrix<double> matrix = convertToMatrix(model);
            Vector<double> rhs = matrix.Column(matrix.ColumnCount - 1);
            Vector<double> zRow = matrix.Row(matrix.RowCount - 1);
            double min = 0;
            int mindex = -1;
            for(int i = 0; i < model.Goal.Coefficients.Length; i++) {
                if (!basicVariableIndecies.Contains(i) && model.Goal.Coefficients[i] < 0)
                {
                    min = model.Goal.Coefficients[i];
                    mindex = i;
                }
            }
            if (mindex != -1)
            {

            }
            
            throw new NotImplementedException();
        }

        public void convertAllInequalities(Model model){
            foreach (LinearConstraint lc in model.Constraints)
            {
                if (lc.Relationship == Relationship.LessThanOrEquals)
                {
                    sCount++;
                }
                else if (lc.Relationship == Relationship.GreaterThanOrEquals)
                {
                    sCount++;
                    aCount++;
                }
            }

            int sOffset = 0;
            int aOffset = 0;
            int totalLength = aCount + sCount + 2;
            var newConstraints = new List<LinearConstraint>() { };
            var wGoal = new Goal()
            {
                Coefficients = new double[totalLength],
                ConstantTerm = 0
            };
            foreach (LinearConstraint constraint in model.Constraints)
            {
                LinearConstraint newLC = convertInequality(constraint, sCount, sOffset, aOffset, totalLength);
                newConstraints.Add(newLC);
                sOffset++;
                if (constraint.Relationship == Relationship.GreaterThanOrEquals)
                {
                    aOffset++;
                    for (int i = 0; i < newLC.Coefficients.Length; i++)
                    {
                        wGoal.Coefficients[i] += newLC.Coefficients[i];
                    }
                    wGoal.ConstantTerm += constraint.Value;
                }
            }
            model.setConstraints(newConstraints);
           
            foreach (LinearConstraint constraint in model.Constraints)
            {
                System.Diagnostics.Debug.WriteLine(string.Join("\t", constraint.Coefficients));
            }
            
            /*if (aCount > 0)
            {
                String summary = "";
                for (int i = 0; i < totalLength - aCount - 1; i++)
                {
                    summary += (-1 * wGoal.Coefficients[i]) + "\t";
                }
                for (int i = totalLength - aCount - 1; i < totalLength; i++)
                {
                    summary += wGoal.Coefficients[i] + "\t";
                }
                System.Diagnostics.Debug.WriteLine(summary);
            }*/
            model.setGoal(wGoal);
        }

        public LinearConstraint convertInequality(LinearConstraint lc, int sCount, int sOffset, int aOffset, int totalLength)
        {
            LinearConstraint newConstraint = new LinearConstraint()
            {
                Coefficients = new double[totalLength],
                Relationship = Relationship.Equals,
                Value = lc.Value,
            };
            newConstraint.Coefficients.SetValue(lc.Coefficients[0], 0);
            newConstraint.Coefficients.SetValue(lc.Coefficients[1], 1);
            if (lc.Relationship == Relationship.LessThanOrEquals)
            {
                newConstraint.Coefficients.SetValue(1, sOffset + 2);
            }
            else if (lc.Relationship == Relationship.GreaterThanOrEquals)
            {
                newConstraint.Coefficients.SetValue(-1, sOffset + 2);
                newConstraint.Coefficients.SetValue(1, aOffset + 2 + sCount);
            }
            return newConstraint;
        }

        public Matrix<double> convertToMatrix(Model model)
        {
            List<double[]> list = new List<double[]>();
            foreach (LinearConstraint lc in model.Constraints)
            {
                double[] a = new double[lc.Coefficients.Length + 1];
                lc.Coefficients.CopyTo(a, 0);
                a[a.Length - 1] = lc.Value;
                list.Add(a);
            }
            double[] zRow = new double[model.Goal.Coefficients.Length + 1];
            model.Goal.Coefficients.CopyTo(zRow, 0);
            zRow[zRow.Length - 1] = model.Goal.ConstantTerm;
            list.Add(zRow);
            Matrix<double> m = Matrix<double>.Build.DenseOfRowArrays(list);

            return m;
        }

        public Matrix<double> convertToCoefficientsMatrix(Model model)
        {
            List<double[]> list = new List<double[]>();
            foreach (LinearConstraint lc in model.Constraints)
            {
                double[] a = new double[lc.Coefficients.Length + 1];
                lc.Coefficients.CopyTo(a, 0);
                a[a.Length - 1] = lc.Value;
                list.Add(a);
            }
            double[] zRow = new double[model.Goal.Coefficients.Length + 1];
            model.Goal.Coefficients.CopyTo(zRow, 0);
            zRow[zRow.Length - 1] = model.Goal.ConstantTerm;
            list.Add(zRow);
            Matrix<double> m = Matrix<double>.Build.DenseOfRowArrays(list);

            return m;
        }

        public double[] basicColumnIndecies(Model model) {
            double[] indecies = new double[model.Constraints.Count];
            int count = 0;
            for(int i = 0; i < model.Constraints[0].Coefficients.Length) {
                bool hasOne = false;
                bool restZero = true;
                for(int j = 0; j < model.Constraints.Count; j++) {
                    if(!hasOne && model.Constraints[j].Coefficients[i] == 1) {
                        hasOne = true;
                    } else if(model.Constraints[j].Coefficients[i] != 0) {
                        restZero = false;
                    }
                }
                if(hasOne && restZero) {
                    indecies[count] = i;
                    count++;
                }
            }
            return indecies;
        }

        public double calculateNewCoefficient(int index, Matrix<double> matrix, Vector<double> basic)
        {
            var zVal = matrix.At(matrix.RowCount - 1, index);
            Vector<double> coVector = matrix.Column(index, 0, matrix.RowCount - 1);
            var newCo = zVal - (basic * coVector);
            return newCo;
        }
        



    }
}
