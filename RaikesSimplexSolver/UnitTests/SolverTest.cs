using RaikesSimplexService.InsertTeamNameHere;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using RaikesSimplexService.DataModel;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SolverTest and is intended
    ///to contain all SolverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SolverTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Solve
        ///</summary>
        [TestMethod()]
        public void ExampleSolveTest()
        {
            #region Arrange
            var target = new Solver();

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { 2, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 32
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 18
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 3 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 36
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2, lc3};

            var goal = new Goal()
            {
                Coefficients = new double[2] { 80, 70 },
                ConstantTerm = 0
            };

            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Minimize
            };

            var expected = new Solution()
            {
                Decisions = new double[2] { 14, 4 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 1400
            };

            target.convertAllInequalities(model);

            double[] zRow = new double[10];
            zRow[0] = goal.Coefficients[0];
            zRow[1] = goal.Coefficients[1];
            for (int i = 2; i <= zRow.Length - 2; i++)
            {
                zRow[i] = 0;
            }
            zRow[9] = goal.ConstantTerm;

            System.Diagnostics.Debug.WriteLine(string.Join("\t", zRow));

            System.Diagnostics.Debug.WriteLine("this is the z row!");
            Matrix<double> m = target.convertToMatrix(model);

            //Print the Matrix!
            String matrix = m.ToString();
            matrix = matrix.Replace("  ", "\t");
            System.Diagnostics.Debug.WriteLine(matrix);

            //Assert
            //commented out below too...
            //CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            //Assert.AreEqual(expected.Quality, actual.Quality);
            //Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
            //var actual = target.Solve(model);
           }
            #endregion
    }
  
}