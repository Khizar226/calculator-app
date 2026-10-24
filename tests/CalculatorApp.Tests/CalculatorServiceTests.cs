using CalculatorApp.Models;
using CalculatorApp.Services;

namespace CalculatorApp.Tests;

public class CalculatorServiceTests
{
    private readonly CalculatorService _calculator = new();

    private (bool Ok, double Result, string? Error) Run(string operation, double first, double? second = null)
    {
        var request = new CalculationRequest
        {
            Operation = operation,
            FirstNumber = first,
            SecondNumber = second,
        };
        var ok = _calculator.TryCalculate(request, out var result, out var error);
        return (ok, result, error);
    }

    [Theory]
    [InlineData("add", 2, 3, 5)]
    [InlineData("add", -1.5, 0.5, -1)]
    [InlineData("subtract", 10, 4, 6)]
    [InlineData("multiply", 6, 7, 42)]
    [InlineData("multiply", -3, 0.5, -1.5)]
    [InlineData("divide", 9, 3, 3)]
    [InlineData("divide", 1, 4, 0.25)]
    [InlineData("modulus", 10, 3, 1)]
    [InlineData("modulus", -7, 3, -1)]
    [InlineData("power", 2, 10, 1024)]
    [InlineData("power", 9, 0.5, 3)]
    [InlineData("power", 5, 0, 1)]
    public void Binary_operations_return_expected_result(string operation, double first, double second, double expected)
    {
        var (ok, result, error) = Run(operation, first, second);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal(expected, result, precision: 10);
    }

    [Theory]
    [InlineData(16, 4)]
    [InlineData(2, 1.4142135624)]
    [InlineData(0, 0)]
    public void Square_root_returns_expected_result(double input, double expected)
    {
        var (ok, result, _) = Run("sqrt", input);

        Assert.True(ok);
        Assert.Equal(expected, result, precision: 10);
    }

    [Fact]
    public void Square_root_ignores_second_number()
    {
        var (ok, result, _) = Run("sqrt", 25, second: 999);

        Assert.True(ok);
        Assert.Equal(5, result);
    }

    [Theory]
    [InlineData("divide", 5, 0, "Cannot divide by zero.")]
    [InlineData("modulus", 5, 0, "Cannot calculate a remainder using zero.")]
    [InlineData("power", -8, 0.5, "This operation has no real-number result.")]
    [InlineData("power", 10, 400, "The result is outside the supported number range.")]
    [InlineData("multiply", 1e308, 10, "The result is outside the supported number range.")]
    [InlineData("unknown", 1, 2, "Choose a supported calculator operation.")]
    public void Invalid_operations_return_a_clear_error(string operation, double first, double second, string expectedError)
    {
        var (ok, _, error) = Run(operation, first, second);

        Assert.False(ok);
        Assert.Equal(expectedError, error);
    }

    [Fact]
    public void Square_root_of_negative_number_is_rejected()
    {
        var (ok, _, error) = Run("sqrt", -9);

        Assert.False(ok);
        Assert.Equal("Cannot calculate the square root of a negative number.", error);
    }

    [Theory]
    [InlineData("add")]
    [InlineData("divide")]
    [InlineData("power")]
    public void Binary_operations_require_a_second_number(string operation)
    {
        var (ok, _, error) = Run(operation, 4, second: null);

        Assert.False(ok);
        Assert.Equal("A valid second number is required for this operation.", error);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Non_finite_first_number_is_rejected(double first)
    {
        var (ok, _, error) = Run("add", first, 1);

        Assert.False(ok);
        Assert.Equal("The first number must be a finite value.", error);
    }

    [Fact]
    public void Non_finite_second_number_is_rejected()
    {
        var (ok, _, error) = Run("add", 1, double.NaN);

        Assert.False(ok);
        Assert.Equal("A valid second number is required for this operation.", error);
    }
}
