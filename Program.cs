using CalculatorApp.Models;
using CalculatorApp.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Built-in OpenAPI and request validation are available with .NET 10.
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<ICalculatorService, CalculatorService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/api/calculator/calculate",
    Results<Ok<CalculationResponse>, BadRequest<ProblemDetails>> (
        CalculationRequest request,
        ICalculatorService calculator) =>
    {
        if (!calculator.TryCalculate(request, out var result, out var error))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid calculation",
                Detail = error
            });
        }

        return TypedResults.Ok(new CalculationResponse(result));
    })
    .WithName("Calculate")
    .WithSummary("Calculate a result")
    .WithDescription("Runs one supported calculator operation using the supplied number or numbers.")
    .Produces<CalculationResponse>(StatusCodes.Status200OK)
    .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

app.Run();
