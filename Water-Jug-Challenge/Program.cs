using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using System.Reflection;
using System.IO;

/// <summary>
/// The main entry point for the application.
/// </summary>
public class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ConfigureServices(builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        Configure(app);

        app.Run();
    }

    // Method to configure services for dependency injection
    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddMemoryCache(); // Adds in-memory caching service
    }

    // Method to configure the HTTP request pipeline
    private static void Configure(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        app.UseHttpsRedirection();

        // Map the POST endpoint to handle bucket solving requests
        app.MapPost("/solve-bucket", HandleSolveBucketRequest);
    }

    // Handler for the /solve-bucket endpoint
    private static IResult HandleSolveBucketRequest(BucketRequest request, IMemoryCache cache)
    {
        // Validate the request parameters
        if (request.x_capacity <= 0 || request.y_capacity <= 0 || request.z_amount_wanted <= 0)
        {
            return Results.BadRequest(new { message = "All capacities and the desired amount must be positive integers." });
        }

        // Generate a cache key based on the request parameters
        string cacheKey = $"{request.x_capacity}-{request.y_capacity}-{request.z_amount_wanted}";
        if (!cache.TryGetValue(cacheKey, out List<Solution>? steps))
        {
            // Solve the bucket problem if not found in cache
            steps = BucketSolver.SolveWaterBucket(request.x_capacity, request.y_capacity, request.z_amount_wanted);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            cache.Set(cacheKey, steps, cacheEntryOptions); // Cache the solution steps
        }

        // Return appropriate response based on the solution steps
        if (steps == null || steps.Count == 0)
        {
            return Results.NotFound(new { message = "No solution possible." });
        }
        else
        {
            var solutionSteps = steps.Select((step, index) => new SolutionStep
            {
                Step = index + 1,
                BucketX = step.BucketX,
                BucketY = step.BucketY,
                Action = step.Action,
                Status = index == steps.Count - 1 ? "Solved" : "...",
            }).ToList();

            return Results.Ok(new { solution = solutionSteps });
        }
    }
}

// Record to represent the bucket request
/// <summary>
/// Represents a request to solve the water bucket problem.
/// </summary>
/// <param name="x_capacity">The capacity of bucket X.</param>
/// <param name="y_capacity">The capacity of bucket Y.</param>
/// <param name="z_amount_wanted">The desired amount of water.</param>
public record BucketRequest(int x_capacity, int y_capacity, int z_amount_wanted);

// Class to represent a water bucket
/// <summary>
/// Represents a water bucket with a specific capacity and current amount of water.
/// </summary>
public class WaterBucket
{
    /// <summary>
    /// Gets or sets the capacity of the water bucket.
    /// </summary>
    public int Capacity { get; set; }
    /// <summary>
    /// Gets or sets the current amount of water in the bucket.
    /// </summary>
    public int CurrentAmount { get; set; }
}

// Class to represent a solution step
/// <summary>
/// Represents a solution step in the water bucket problem.
/// </summary>
public class Solution
{
    [JsonIgnore]

    /// <summary>
    /// Gets or sets the step number in the solution.
    /// </summary>
    public int Step { get; set; }
    /// <summary>
    /// Gets or sets the current amount of water in bucket X.
    /// </summary>
    public int BucketX { get; set; }
    /// <summary>
    /// Gets or sets the current amount of water in bucket Y.
    /// </summary>
    public int BucketY { get; set; }
    /// <summary>
    /// Gets or sets the action taken in this solution step.
    /// </summary>
    public required string Action { get; set; }
}


/// <summary>
/// Represents a step in the solution of the water bucket problem.
/// </summary>
public class SolutionStep
{
    /// <summary>
    /// Gets or sets the step number in the solution.
    /// </summary>
    public int Step { get; set; }
    /// <summary>
    /// Gets or sets the current amount of water in bucket X.
    /// </summary>
    public int BucketX { get; set; }
    /// <summary>
    /// Gets or sets the current amount of water in bucket Y.
    /// </summary>
    public int BucketY { get; set; }
    /// <summary>
    /// Gets or sets the action taken in this step.
    /// </summary>
    public required string Action { get; set; }
    /// <summary>
    /// Gets or sets the status of this step.
    /// </summary>
    public required string Status { get; set; }
}

/// <summary>
/// Provides methods to solve the water bucket problem.
/// </summary>
public static class BucketSolver
{
    /// <summary>   
    /// Solves the water bucket problem with the given bucket capacities and desired amount of water.
    /// </summary>
    public static List<Solution> SolveWaterBucket(int x_capacity, int y_capacity, int z_amount_wanted)
    {
        if (x_capacity <= 0 || y_capacity <= 0 || z_amount_wanted <= 0)
        {
            return new List<Solution>();
        }

        var queue = new Queue<Tuple<WaterBucket, WaterBucket, List<Solution>>>();
        var visited = new HashSet<string>();
        var bucketX = new WaterBucket { Capacity = x_capacity, CurrentAmount = 0 };
        var bucketY = new WaterBucket { Capacity = y_capacity, CurrentAmount = 0 };
        var initialSteps = new List<Solution>();

        queue.Enqueue(Tuple.Create(bucketX, bucketY, initialSteps));
        visited.Add($"{bucketX.CurrentAmount},{bucketY.CurrentAmount}");

        int currentStep = 1;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentX = current.Item1;
            var currentY = current.Item2;
            var currentSteps = current.Item3;

            if (IsSolution(currentX, currentY, z_amount_wanted))
            {
                return currentSteps;
            }

            var nextStates = GenerateNextStates(currentX, currentY);

            foreach (var nextState in nextStates)
            {
                string stateKey = $"{nextState.Item1.CurrentAmount},{nextState.Item2.CurrentAmount}";
                if (!visited.Contains(stateKey))
                {
                    visited.Add(stateKey);
                    var newSteps = new List<Solution>(currentSteps)
                    {
                        new Solution { Step = currentStep++, BucketX = nextState.Item1.CurrentAmount, BucketY = nextState.Item2.CurrentAmount, Action = nextState.Item3 }
                    };
                    queue.Enqueue(Tuple.Create(nextState.Item1, nextState.Item2, newSteps));
                }
            }
        }

        return new List<Solution>();
    }

    // Method to check if the current state is a solution
    private static bool IsSolution(WaterBucket bucketX, WaterBucket bucketY, int z_amount_wanted)
    {
        return bucketX.CurrentAmount == z_amount_wanted || bucketY.CurrentAmount == z_amount_wanted;
    }

    // Method to generate the next possible states from the current state
    private static List<Tuple<WaterBucket, WaterBucket, string>> GenerateNextStates(WaterBucket currentX, WaterBucket currentY)
    {
        return new List<Tuple<WaterBucket, WaterBucket, string>>
        {
            Tuple.Create(new WaterBucket { Capacity = currentX.Capacity, CurrentAmount = currentX.Capacity }, new WaterBucket { Capacity = currentY.Capacity, CurrentAmount = currentY.CurrentAmount }, "Fill bucket X"),
            Tuple.Create(new WaterBucket { Capacity = currentX.Capacity, CurrentAmount = currentX.CurrentAmount }, new WaterBucket { Capacity = currentY.Capacity, CurrentAmount = currentY.Capacity }, "Fill bucket Y"),
            Tuple.Create(new WaterBucket { Capacity = currentX.Capacity, CurrentAmount = 0 }, new WaterBucket { Capacity = currentY.Capacity, CurrentAmount = currentY.CurrentAmount }, "Empty bucket X"),
            Tuple.Create(new WaterBucket { Capacity = currentX.Capacity, CurrentAmount = currentX.CurrentAmount }, new WaterBucket { Capacity = currentY.Capacity, CurrentAmount = 0 }, "Empty bucket Y"),
            Tuple.Create(new WaterBucket { Capacity = currentX.Capacity, CurrentAmount = Math.Max(0, currentX.CurrentAmount - (currentY.Capacity - currentY.CurrentAmount)) }, new WaterBucket { Capacity = currentY.Capacity, CurrentAmount = Math.Min(currentY.Capacity, currentY.CurrentAmount + currentX.CurrentAmount) }, "Transfer water from bucket X to bucket Y"),
            Tuple.Create(new WaterBucket { Capacity = currentX.Capacity, CurrentAmount = Math.Min(currentX.Capacity, currentX.CurrentAmount + currentY.CurrentAmount) }, new WaterBucket { Capacity = currentY.Capacity, CurrentAmount = Math.Max(0, currentY.CurrentAmount - (currentX.Capacity - currentX.CurrentAmount)) }, "Transfer water from bucket Y to bucket X")
        };
    }
}