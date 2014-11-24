using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using RaikesSimplexService.Contracts;
using RaikesSimplexService.DataModel;

namespace RaikesSimplexService.InsertTeamNameHere
{
    
    public class Solver : ISolver
    {

        int sCount = 0;
        int aCount = 0;
        public Solution Solve(Model model)
        {
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
            if (aCount > 0)
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
            }
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
            //newConstraint.Coefficients.SetValue(lc.Value, totalLength - 1);
            if (lc.Relationship == Relationship.LessThanOrEquals)
            {
                newConstraint.Coefficients.SetValue(1, sOffset + 2);
            }
            else if (lc.Relationship == Relationship.GreaterThanOrEquals)
            {
                newConstraint.Coefficients.SetValue(-1, sOffset + 2);
                newConstraint.Coefficients.SetValue(1, aOffset + 2 + sCount);
            }
            System.Diagnostics.Debug.WriteLine(string.Join("\t", newConstraint.Coefficients));
            return newConstraint;
        }
    }
}
