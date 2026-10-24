using System.ComponentModel.DataAnnotations;

namespace CalculatorApp.Models;

/// <summary>Contains the operation and numbers to use for a calculation.</summary>
public sealed record CalculationRequest
{
    /// <summary>The operation name: add, subtract, multiply, divide, modulus, power, or sqrt.</summary>
    [Required, MaxLength(10)]
    public required string Operation { get; init; }

    /// <summary>The first number, or the number whose square root is requested.</summary>
    public double FirstNumber { get; init; }

    /// <summary>The second number for binary operations; not used for square root.</summary>
    public double? SecondNumber { get; init; }
}
