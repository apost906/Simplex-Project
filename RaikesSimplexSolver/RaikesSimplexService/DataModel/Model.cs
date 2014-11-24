using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace RaikesSimplexService.DataModel
{
    /// <summary>
    /// Defines a model that has expressions and constraints.
    /// </summary>
    [DataContract]
    public class Model
    {
        /// <summary>
        /// Data member that contains the function you want to optimize.
        /// </summary>
        [DataMember]
        public Goal Goal { get; set; }

        /// <summary>
        /// Data member that contains a list of constraint equations.
        /// </summary>
        [DataMember]
        public List<LinearConstraint> Constraints { get; set; }

        /// <summary>
        /// Data member that indicated whether to maximize of minimize the goal function.
        /// </summary>
        [DataMember]
        public GoalKind GoalKind { get; set; }

        public void setConstraints(List<LinearConstraint> constraints)
        {
            this.Constraints = constraints;
        }

        public void setGoal(Goal goal)
        {
            this.Goal = goal;
        }

        public String Definition()
        {
            String summary = "Decision variables:\n\t";
            for (int i = 0; i < this.Goal.Coefficients.Length; i++)
            {
                summary += " " + "X" + (i + 1);
            }
            summary += "\nv" + this.GoalKind.ToString();
            for (int i = 0; i < this.Goal.Coefficients.Length; i++)
            {
                summary += " " + this.Goal.Coefficients[i];
            }
            summary += "\n";
            for (int i = 0; i < this.Constraints.Count; i++)
            {
                summary += "R" + i + "\t";

                for (int j = 0; j < this.Constraints[i].Coefficients.Length; j++)
                {
                    summary += " " + this.Constraints[i].Coefficients[j];
                }
                summary += " " + RelationshipRepresentation(this.Constraints[i].Relationship);
                summary += " " + this.Constraints[i].Value + "\n";
            }
            return summary;
        }

        private String RelationshipRepresentation(Relationship r)
        {
            String sign;
            if (r.Equals(Relationship.LessThanOrEquals))
            {
                sign = "<=";
            }
            else if (r.Equals(Relationship.GreaterThanOrEquals))
            {
                sign = ">=";
            }
            else
            {
                sign = "=";
            }
            return sign;
        }

        public string ToString { get; set; }
    }
}
