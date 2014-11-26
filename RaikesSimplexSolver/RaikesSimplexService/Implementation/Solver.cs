using System;
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
            int[] basicVariableIndecies = basicColumnIndecies(model);
            Matrix<double> matrix = convertToMatrix(model);
            Matrix<double> coefficientMatrix = convertToCoefficientsMatrix(model);
            Vector<double> zRow = matrix.Row(matrix.RowCount - 1);
            Matrix<double> basicMatrix = findBasicMatrix(matrix, basicVariableIndecies);
            int mindex;
            do
            {
                List<Vector<double>> primeVectors = calculatePrimeVectors(basicMatrix.Inverse(), coefficientMatrix);
                double min = 0;
                mindex = -1;
                for (int i = 0; i < model.Goal.Coefficients.Length; i++)
                {
                    double c = calculateNewCoefficient(i, zRow, primeVectors, basicVariableIndecies);
                    if (!basicVariableIndecies.Contains(i) && c < 0)
                    {
                        min = c;
                        mindex = i;
                    }
                }
                if (mindex != -1)
                {
                    int index = findIndexOfSmallestPositive(primeVectors[primeVectors.Count - 1], primeVectors[mindex]);
                    basicVariableIndecies[index] = mindex;
                }
            } while (mindex != -1);
            
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
                //System.Diagnostics.Debug.WriteLine(string.Join("\t", constraint.Coefficients));
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
            Matrix<double> m = Matrix<double>.Build.DenseOfRowArrays(list);

            return m;
        }

        public int findIndexOfSmallestPositive(Vector<double> xb, Vector<double> p1)
        {

            var vector = xb / p1;
            //divide
            System.Diagnostics.Debug.WriteLine(vector);
            var min = vector.AbsoluteMinimum();
            System.Diagnostics.Debug.WriteLine("minimum value = " + min);
            var minIndex = vector.AbsoluteMinimumIndex();
            System.Diagnostics.Debug.WriteLine("index = " + minIndex);
            return minIndex;

        }

        public int[] basicColumnIndecies(Model model) {
            int[] indecies = new int[model.Constraints.Count];
            int count = 0;
            for (int i = 0; i < model.Constraints[0].Coefficients.Length; i++)
            {
                bool hasOne = false;
                bool restZero = true;
                for (int j = 0; j < model.Constraints.Count; j++)
                {
                    if (!hasOne && model.Constraints[j].Coefficients[i] == 1)
                    {
                        hasOne = true;
                    }
                    else if (model.Constraints[j].Coefficients[i] != 0)
                    {
                        restZero = false;
                    }
                }
                if (hasOne && restZero)
                {
                    indecies[count] = i;
                    count++;
                }
            }
            return indecies;
        }

        public Matrix<double> findBasicMatrix(Matrix<double> matrix, int[] indices)
        {
            List<Vector<double>> list = new List<Vector<double>>();
            foreach (int i in indices)
            {
                // gets column vector without the zRow coefficient
                Vector<double> v = matrix.Column(i, 0, matrix.RowCount-1);
                list.Add(v);
            }
            Matrix<double> m = Matrix<double>.Build.DenseOfColumnVectors(list);
            return m;
        }

        public List<Vector<double>> calculatePrimeVectors(Matrix<double> bInv, Matrix<double> coefficientMatrix)
        {
            List<Vector<double>> primeVectors = new List<Vector<double>>();
            for (int i = 0; i < coefficientMatrix.ColumnCount; i++)
            {
                Vector<double> v = bInv * coefficientMatrix.Column(i);
                primeVectors.Add(v);
            }
            return primeVectors;
        }


        public double calculateNewCoefficient(int index, Vector<double> zRow, List<Vector<double>> primeVectors, int[] basicIndices)
        {
            var zVal = zRow.At(index);
            Vector<double> coVector = primeVectors[index];
            double[] b = new double[basicIndices.Length];
            for (int i = 0; i <= basicIndices.Length; i++)
            {
                b[i] = zRow[basicIndices[i]];
            }
            var V = Vector<double>.Build;
            var basicVars = V.DenseOfArray(b);
            var newCo = zVal - (basicVars * coVector);
            return newCo;
        }
        



    }
}
