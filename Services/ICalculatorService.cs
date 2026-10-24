using CalculatorApp.Models;

namespace CalculatorApp.Services;

public interface ICalculatorService
{
    bool TryCalculate(CalculationRequest request, out double result, out string? error);
}
