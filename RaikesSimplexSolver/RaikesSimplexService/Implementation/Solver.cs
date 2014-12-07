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
        public Solution Solve(Model model)
        {
            double[] decision = new double[model.Constraints[0].Coefficients.Length];
            double optimalValue = 0;
            convertAllInequalities(model);
            int[] basicVariableIndecies = basicColumnIndecies(model);
            bool twoPhase = isTwoPhase(model);
            reduce(model, basicVariableIndecies);

            if (twoPhase)
            {
                // Ashley's method to remove wRow and make zGoal
                reduce(model, basicVariableIndecies);


                // add zRow to coefficients
                // make wRow the new goal
                // reduce
                // remove wRow
                // make last of coefficients (zRow) the new goal
                // reduce
            }

            Matrix<double> matrix = convertToMatrix(model);
            Vector<double> zRow = matrix.Row(matrix.RowCount - 1);
            Matrix<double> basicMatrix = findBasicMatrix(matrix, basicVariableIndecies);
            List<Vector<double>> primeVectors = calculatePrimeVectors(basicMatrix.Inverse(), matrix);

            for(int i = 0; i < basicVariableIndecies.Length; i++) {
                
                if(basicVariableIndecies[i] < decision.Length) {
                    double xbPrime = primeVectors[primeVectors.Count-1].At(i);
                    decision[basicVariableIndecies[i]] = xbPrime;
                }
            }

            for(int i = 0; i < decision.Length; i++) {
                optimalValue += decision[i] * zRow[i] * -1;
            }

            Solution s = new Solution(decision, optimalValue, false, SolutionQuality.Optimal);

            return s;
        }

        public void convertAllInequalities(Model model){
            int sCount = 0;
            int aCount = 0;
            foreach (LinearConstraint lc in model.Constraints)
            {
                if (lc.Relationship.Equals(Relationship.GreaterThanOrEquals))
                {
                    sCount++;
                    aCount++;
                }
                else
                {
                    sCount++;
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
            if (aCount > 0)
            {
                // do Ashley's method to add zRow to Constraints
            }
            foreach (LinearConstraint constraint in model.Constraints)
            {
                LinearConstraint newLC = convertInequality(constraint, sCount, sOffset, aOffset, totalLength);
                newConstraints.Add(newLC);
                sOffset++;
                if (constraint.Relationship.Equals(Relationship.GreaterThanOrEquals))
                {
                    aOffset++;
                    for (int i = 0; i < newLC.Coefficients.Length; i++)
                    {
                        wGoal.Coefficients[i] += newLC.Coefficients[i];
                    }
                    wGoal.ConstantTerm += constraint.Value;
                }
                else
                {
                    for (int i = 0; i < model.Goal.Coefficients.Length; i++)
                    {
                        wGoal.Coefficients[i] = -1 * model.Goal.Coefficients[i];
                    }
                    wGoal.ConstantTerm = model.Goal.ConstantTerm;
                }
            }
            model.setConstraints(newConstraints);
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
            if (lc.Relationship.Equals(Relationship.LessThanOrEquals))
            {
                newConstraint.Coefficients.SetValue(1, sOffset + 2);
            }
            else if (lc.Relationship.Equals(Relationship.GreaterThanOrEquals))
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

        public int findIndexOfSmallestPositive(Vector<double> xb, Vector<double> p1)
        {

            var vector = xb / p1;
            //divide
            var min = vector.AbsoluteMinimum();
            var minIndex = vector.AbsoluteMinimumIndex();
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
                Vector<double> v = bInv * coefficientMatrix.Column(i, 0, coefficientMatrix.RowCount-1);
                primeVectors.Add(v);
            }
            return primeVectors;
        }


        public double calculateNewCoefficient(int index, Vector<double> zRow, List<Vector<double>> primeVectors, int[] basicIndices)
        {
            var zVal = zRow.At(index);
            Vector<double> coVector = primeVectors[index];
            double[] b = new double[basicIndices.Length];
            for (int i = 0; i < basicIndices.Length; i++)
            {
                b[i] = zRow[basicIndices[i]];
            }
            var V = Vector<double>.Build;
            var basicVars = V.DenseOfArray(b);
            double vectorMult = basicVars * coVector;
            double newCo = zVal - vectorMult;
            return newCo;
        }

        public void reduce(Model model, int[] basicVariableIndecies)
        {
            Matrix<double> matrix = convertToMatrix(model);
            List<Vector<double>> primeVectors = new List<Vector<double>>();
            Vector<double> zRow = matrix.Row(matrix.RowCount - 1);     
            int mindex = -1;

            do
            {
                Matrix<double> basicMatrix = findBasicMatrix(matrix, basicVariableIndecies);
                primeVectors = calculatePrimeVectors(basicMatrix.Inverse(), matrix);
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
        }

        private bool isTwoPhase(Model model)
        {
            foreach(LinearConstraint lc in model.Constraints) {
                if (lc.Relationship.Equals(Relationship.GreaterThanOrEquals))
                {
                    return true;
                }
            }
            return false;
        }


    }
}
