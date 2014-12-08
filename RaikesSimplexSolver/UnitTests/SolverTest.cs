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
        public void Test1() //2nd example on Ashu's revised simplex ppt
        {
            var target = new Solver();

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 35
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 2 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 38
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[2] { 2, 2 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 50
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2, lc3 };

            var goal = new Goal()
            {
                Coefficients = new double[2] { 350, 450 },
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
                Decisions = new double[2] { 12, 13 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 10050
            };

            //     target.convertAllInequalities(model);


            //     Matrix<double> m = target.convertToMatrix(model);

            //Print the Matrix!
            //         String matrix = m.ToString();
            //        matrix = matrix.Replace("  ", "\t");
            //       System.Diagnostics.Debug.WriteLine(matrix);

            Solution s = target.Solve(model);
            System.Diagnostics.Debug.WriteLine("Optimal Solution: ");
            System.Diagnostics.Debug.WriteLine("x1 = " + s.Decisions[0]);
            System.Diagnostics.Debug.WriteLine("x2 = " + s.Decisions[1]);
            System.Diagnostics.Debug.WriteLine("Z = " + s.OptimalValue);

            //Assert
            //commented out below too...
            //CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            //Assert.AreEqual(expected.Quality, actual.Quality);
            //Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
            //var actual = target.Solve(model);
        }

        [TestMethod()]
        public void Test2() //hw 4 #2
        {
            var target = new Solver();

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 1
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 2, -1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 1
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[2] { 0, 3 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 2
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2, lc3 };

            var goal = new Goal()
            {
                Coefficients = new double[2] { 6, 3 },
                ConstantTerm = 0
            };

            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Maximize
            };

            var expected = new Solution()
            {
                Decisions = new double[2] { 1, 0 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 6
            };

            Solution s = target.Solve(model);
            System.Diagnostics.Debug.WriteLine("Optimal Solution: ");
            System.Diagnostics.Debug.WriteLine("x1 = " + s.Decisions[0]);
            System.Diagnostics.Debug.WriteLine("x2 = " + s.Decisions[1]);
            System.Diagnostics.Debug.WriteLine("Z = " + s.OptimalValue);

        }


        [TestMethod()]
        public void Test3() //hw 4 #1
        {
            var target = new Solver();

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 1
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 2, -1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 1
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[2] { 0, 3 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 2
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2, lc3 };

            var goal = new Goal()
            {
                Coefficients = new double[2] { 6, 3 },
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
                Decisions = new double[2] { (2 / 3), (1 / 3) },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 5
            };

            Solution s = target.Solve(model);
            System.Diagnostics.Debug.WriteLine("Optimal Solution: ");
            System.Diagnostics.Debug.WriteLine("x1 = " + s.Decisions[0]);
            System.Diagnostics.Debug.WriteLine("x2 = " + s.Decisions[1]);
            System.Diagnostics.Debug.WriteLine("Z = " + s.OptimalValue);
        }

        [TestMethod()]
        public void Test4() //hw4 #3
        {
            var target = new Solver();

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[3] { 1, 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 40
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[3] { 2, 1, -1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 10
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[3] { 0, -1, 1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 10
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2, lc3 };

            var goal = new Goal()
            {
                Coefficients = new double[3] { 2, 3, 1 },
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
                Decisions = new double[3] { 10, 10, 20 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 70
            };

            Solution s = target.Solve(model);
            System.Diagnostics.Debug.WriteLine("Optimal Solution: ");
            System.Diagnostics.Debug.WriteLine("x1 = " + s.Decisions[0]);
            System.Diagnostics.Debug.WriteLine("x2 = " + s.Decisions[1]);
            System.Diagnostics.Debug.WriteLine("x3 = " + s.Decisions[2]);
            System.Diagnostics.Debug.WriteLine("Z = " + s.OptimalValue);
        }


        [TestMethod()]
        public void Test5() //one from FB that broke everything (has an equals relationship)
        {
            var target = new Solver();

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { -1, 1 },
                Relationship = Relationship.Equals,
                Value = 0
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 2
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2 };

            var goal = new Goal()
            {
                Coefficients = new double[2] { 1, 1 },
                ConstantTerm = 0
            };

            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Maximize
            };

            var expected = new Solution()
            {
                Decisions = new double[2] { 1, 1 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 2
            };

            Solution s = target.Solve(model);
            System.Diagnostics.Debug.WriteLine("Optimal Solution: ");
            System.Diagnostics.Debug.WriteLine("x1 = " + s.Decisions[0]);
            System.Diagnostics.Debug.WriteLine("x2 = " + s.Decisions[1]);
            System.Diagnostics.Debug.WriteLine("Z = " + s.OptimalValue);
        }
    }
}  