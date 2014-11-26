﻿using RaikesSimplexService.InsertTeamNameHere;
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
                Coefficients = new double[2] { 8, 12 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 24
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 12, 12 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 36
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[2] { 2, 1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 4
            };

            var lc4 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 5
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2, lc3, lc4 };

            var goal = new Goal()
            {
                Coefficients = new double[2] { 0.2, 0.3 },
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
                Decisions = new double[2] { 3, 0 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 0.6
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


            //Test find min value of vector division
            var V = Vector<double>.Build;
            var v1 = V.DenseOfArray(new[] {10.0, 20.0, 30.0 });
            var V2 = Vector<double>.Build;
            var v2 = V.DenseOfArray(new[] { 2.0, 2.0, 2.0 });
            var output = target.findIndexOfSmallestPositive(v1, v2);

            //Test SubMatrix finder
            var M = Matrix<double>.Build;
            var mm = M.DenseOfArray(new[,] { { 10.0, 20.0, 30.0, 0.0, 1.0, 0.0, 0.0 }, { 40.0, 50.0, 60.0, 1.0, 0.0, 0.0, 0.0 }, { 70.0, 80.0, 90.0, 0.0, 0.0, 1.0, 0.0 }, { 10.0, 20.0, 30.0, 0.0, 0.0, 0.0, 1.0 }});
            String mat = mm.ToString();
            mat = mat.Replace("  ", "\t");
            System.Diagnostics.Debug.WriteLine(mat);
            var subM = target.findBasicMatrix(mm, new[] { 1.0, 2.0 });
            System.Diagnostics.Debug.WriteLine(subM);

            double[] array = target.basicColumnIndecies(model);
            System.Diagnostics.Debug.WriteLine(array[0] + " 1: " + array[1] + " 2: " + array[2] + " 3: " + array[3]);
            Matrix<double> mmm = target.convertToMatrix(model);
            System.Diagnostics.Debug.WriteLine(target.findBasicMatrix(mmm, array));
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