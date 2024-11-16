using Microsoft.VisualStudio.TestTools.UnitTesting; 
using System.Collections.Generic; 

[TestClass] 
public class BucketSolverTests
{
    [TestMethod] 
    public void SolveWaterBucket_ValidInput_ReturnsCorrectSolution()
    {
        // Arrange: Set up the test with specific input values
        int x_capacity = 3; // Capacity of bucket X
        int y_capacity = 5; // Capacity of bucket Y
        int z_amount_wanted = 4; // Desired amount of water

        // Act: Call the method to be tested with the arranged inputs
        List<Solution> result = BucketSolver.SolveWaterBucket(x_capacity, y_capacity, z_amount_wanted);

        // Assert: Verify that the method's output meets the expected conditions
        Assert.IsNotNull(result); // Check that the result is not null
        Assert.IsTrue(result.Count > 0); // Check that the result contains at least one solution
        Assert.IsTrue(result.Exists(step => step.BucketX == z_amount_wanted || step.BucketY == z_amount_wanted)); // Check that one of the steps in the solution achieves the desired amount of water
    }

    [TestMethod] // Indicate that this method is a unit test
    public void SolveWaterBucket_InvalidInput_ReturnsEmptyList()
    {
        // Arrange: Set up the test with invalid input values
        int x_capacity = -1; // Invalid capacity for bucket X
        int y_capacity = -1; // Invalid capacity for bucket Y
        int z_amount_wanted = -1; // Invalid desired amount of water

        // Act: Call the method to be tested with the arranged inputs
        List<Solution> result = BucketSolver.SolveWaterBucket(x_capacity, y_capacity, z_amount_wanted);

        // Assert: Verify that the method's output meets the expected conditions
        Assert.IsNotNull(result); // Check that the result is not null
        Assert.AreEqual(0, result.Count); // Check that the result is an empty list, as the input is invalid
    }
}
