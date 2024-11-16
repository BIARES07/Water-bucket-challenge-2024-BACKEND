# Water Bucket Challange

 Welcome to my project!

Below, I will explain in detail how it works and how to configure it correctly.

This project is based on an exercise where you will design a solution for the classic Water Jug Riddle. The objective is to create an API that calculates the steps needed to measure exactly Z gallons of water using two jugs with capacities of X and Y gallons.

## Esctructure
![2024-11-16 02_54_33-Window](https://github.com/user-attachments/assets/10d18ac9-ce30-4aef-a50d-58187f12e56f)

The project is organized into three folders:

- testing-integration: This folder contains integration tests, which verify how different parts of the application handle requests and work together.
- Unit-Test-Jug: This folder contains unit tests that specifically check the effectiveness of the core algorithm.
- Water-Jug-Challenge: This folder  contains the main code for the algorithm, and we'll be taking a closer look at this folder shortly.

  # recommendations and configuration

  ## Project Setup

To ensure this project runs correctly, please make sure you have the following:

* **SDKs:**
    * .NET 9.0 64x
    * .NET 8.0 64x (recommended)
* **Development Environment:**
    * Visual Studio Code
    * C# extension
    * C# Dev Kit 
    * .NET Install Tool
* **Dependencies:**  
    *  All necessary dependencies (refer to the `.csproj` files in each folder for a complete list).
* **Testing Tool:** 
    * Postman


Following these specifications will allow you to run the project and test the algorithm without any issues. I have verified its operation and can assure you of 100% performance!


## Downloads Links

- .NET SDK: https://dotnet.microsoft.com/en-us/download
- VS Code: https://code.visualstudio.com/
- Postman: https://www.postman.com/downloads/

# How To Its Works

## Algorithm Logic

This section explains the logic behind the algorithm that solves the Water Jug Riddle.

**Objective:**  To measure a specific amount of water (Z) using two jugs with different capacities (X and Y).

**`BucketSolver` Class:**

![2024-11-16 10_17_49-Window](https://github.com/user-attachments/assets/4e2adc38-b67f-4575-b0e8-33de9057295e)


*   Contains the `SolveWaterBucket(x_capacity, y_capacity, z_amount_wanted)` method.
*   Takes the capacities of jugs X and Y and the desired amount of water (Z) as parameters.

![2024-11-16 10_21_15-Window](https://github.com/user-attachments/assets/e44d7577-4cef-4631-8b24-0d049fb0e1a1)


**Algorithm Operation:**

1.  **Validation:**
    *   Checks if the jug capacities and the desired amount of water are valid (greater than zero).
    *   Returns an empty list if invalid, indicating no solution.

2.  **Initialization:**
    *   Creates a queue for Breadth-First Search (BFS).
    *   Creates a set to track visited states and avoid cycles.
    *   Creates two instances of the `WaterBucket` class to represent jugs X and Y, initially empty.
    *   Creates a list to store the solution steps.

3.  **Breadth-First Search (BFS):**
    *   Iterates while the queue is not empty.
    *   In each iteration:
        *   Dequeues the first element (current state of jugs and steps taken).
        *   If the current state is a solution (one jug contains Z), returns the list of steps.
        *   If not a solution, generates possible next states using the `GenerateNextStates()` method.

4.  **Generating Next States (`GenerateNextStates()`):**
    *   Evaluates all possible actions: fill a jug, empty a jug, transfer water between jugs.
    *   Returns a list of tuples, each containing:
        *   The new state of the jugs after the action.
        *   A description of the action taken.
    *   Adds unvisited states to the visited set and enqueues them with updated steps.

5.  **Completion:**
    *   Returns an empty list if the queue is empty without finding a solution.

**Helper Classes:**

*   `IsSolution()`:  Checks if a state is a solution.
*   `WaterBucket`: Represents a jug with its capacity and current water amount.
*   `Solution`: Stores details of each step in the solution.

## Endpoint

To put the algorithm into action, we need to send a POST request to the API. The endpoint for solving the Water Jug Riddle is defined in the main project file, within the method that configures the HTTP request pipeline. As you can see, the endpoint is called `/solve-bucket`.

![2024-11-16 10_33_01-Window](https://github.com/user-attachments/assets/c6db78e8-0cc9-4eaa-90c5-a80c5da185a0)

## Running the Application

1.  **Open your terminal in VS Code.**
2.  **Navigate to the project directory:**
    ```bash
    cd Water-Jug-Challenge
    ```
3.  **Build the project:**
    ```bash
    dotnet build
    ```
4.  **Run the application:**
    ```bash
    dotnet run
    ```

    ![2024-11-16 10_43_03-Window](https://github.com/user-attachments/assets/99496fff-11cc-4fa8-9d31-4f5382a5fed4)


Once the application is running, you'll see a log in the terminal with the `localhost` address where the API is active. You'll need this address to make requests to the `/solve-bucket` endpoint.


## Testing with Postman

Now, let's test the API using Postman.

1.  **Open Postman and enter the endpoint URL:**
    *   In the address bar, paste the full URL, including the `localhost` address from the previous step. For example: `http://localhost:5102/solve-bucket`.

![2024-11-16 10_46_43-Window](https://github.com/user-attachments/assets/9fcc16b5-1daf-4994-a2c7-90655a961995)


2.  **Set the request method to `POST`.**

3.  **Define the request body:**
    *   Go to the "Body" tab.
    *   Select "raw" and "JSON" format.
    *   Paste the following JSON object into the body:
  
    ![2024-11-16 10_51_10-Window](https://github.com/user-attachments/assets/c62e39f6-8bd2-41d2-8e35-d698c23abd77)


        ```json
        {
            "x_capacity": 2,
            "y_capacity": 10,
            "z_amount_wanted": 4
        }
        ```

4.  **Send the request by clicking the "Send" button.**

5.  **Review the response:**
    *   The API response will appear in the lower part of the window in JSON format.
    *   It will contain a list of steps to solve the Water Jug Riddle, like this:
  
    ![2024-11-16 10_52_35-Window](https://github.com/user-attachments/assets/3da754e6-9fa7-4537-b4d0-a6fc1e28a054)


        ```json
        {
            "solution": [
                {
                    "step": 1,
                    "bucketX": 2,
                    "bucketY": 0,
                    "action": "Fill bucket X",
                    "status": "..."
                },
                {
                    "step": 2,
                    "bucketX": 0,
                    "bucketY": 2,
                    "action": "Transfer water from bucket X to bucket Y",
                    "status": "..."
                },
                {
                    "step": 3,
                    "bucketX": 2,
                    "bucketY": 2,
                    "action": "Fill bucket X",
                    "status": "..."
                },
                {
                    "step": 4,
                    "bucketX": 0,
                    "bucketY": 4,
                    "action": "Transfer water from bucket X to bucket Y",
                    "status": "Solved"
                }
            ]
        }
        ```
We can also try something like this:

![2024-11-16 11_11_25-Window](https://github.com/user-attachments/assets/d7522955-1b35-4b71-8608-ec6c5da9c21f)

 ```json
       {
    "solution": [
        {
            "step": 1,
            "bucketX": 0,
            "bucketY": 110,
            "action": "Fill bucket Y",
            "status": "..."
        },
        {
            "step": 2,
            "bucketX": 10,
            "bucketY": 100,
            "action": "Transfer water from bucket Y to bucket X",
            "status": "..."
        },
        {
            "step": 3,
            "bucketX": 0,
            "bucketY": 100,
            "action": "Empty bucket X",
            "status": "..."
        },
        {
            "step": 4,
            "bucketX": 10,
            "bucketY": 90,
            "action": "Transfer water from bucket Y to bucket X",
            "status": "Solved"
        }
    ]
}
  ```

If the values ​​entered do not have a solution, it will respond with a message warning us.

![2024-11-16 11_14_42-Window](https://github.com/user-attachments/assets/997046ee-4956-4dc2-b1f3-426b0c037bc6)

 ```json
 {
    "message": "No solution possible."
}
  ```
Likewise, it will also do the same if the values ​​entered are non-positive integers.

![2024-11-16 11_18_31-Window](https://github.com/user-attachments/assets/19dae18f-7f16-49e2-a983-e3ff4fe664e8)

 ```json
{
    "message": "All capacities and the desired amount must be positive integers."
}
  ```

#  farewell
And that's it! It's been a pleasure guiding you through this project. I hope you enjoyed learning about the Water Jug Riddle and how to implement a solution with this API.
If you have any questions or encounter any obstacles, remember that you can always consult me 😊👌
