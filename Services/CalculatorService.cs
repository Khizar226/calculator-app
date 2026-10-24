using CalculatorApp.Models;

namespace CalculatorApp.Services;

public sealed class CalculatorService : ICalculatorService
{
    public bool TryCalculate(CalculationRequest request, out double result, out string? error)
    {
        result = 0;
        error = null;

        if (!double.IsFinite(request.FirstNumber))
        {
            error = "The first number must be a finite value.";
            return false;
        }

        if (request.Operation == "sqrt")
        {
            if (request.FirstNumber < 0)
            {
                error = "Cannot calculate the square root of a negative number.";
                return false;
            }

            result = Math.Sqrt(request.FirstNumber);
            return true;
        }

        if (request.SecondNumber is not double secondNumber ||
            !double.IsFinite(secondNumber))
        {
            error = "A valid second number is required for this operation.";
            return false;
        }

        switch (request.Operation)
        {
            case "add":
                result = request.FirstNumber + secondNumber;
                break;
            case "subtract":
                result = request.FirstNumber - secondNumber;
                break;
            case "multiply":
                result = request.FirstNumber * secondNumber;
                break;
            case "divide":
                if (secondNumber == 0)
                {
                    error = "Cannot divide by zero.";
                    return false;
                }

                result = request.FirstNumber / secondNumber;
                break;
            case "modulus":
                if (secondNumber == 0)
                {
                    error = "Cannot calculate a remainder using zero.";
                    return false;
                }

                result = request.FirstNumber % secondNumber;
                break;
            case "power":
                result = Math.Pow(request.FirstNumber, secondNumber);
                break;
            default:
                error = "Choose a supported calculator operation.";
                return false;
        }

        // A negative base with a fractional exponent (e.g. (-8)^0.5) has no real result.
        if (double.IsNaN(result))
        {
            error = "This operation has no real-number result.";
            return false;
        }

        // Overflow, such as raising a large number to a very large power.
        if (double.IsInfinity(result))
        {
            error = "The result is outside the supported number range.";
            return false;
        }

        return true;
    }
}
