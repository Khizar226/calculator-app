# Calculator App

[![CI](https://github.com/Khizar226/calculator-app/actions/workflows/ci.yml/badge.svg)](https://github.com/Khizar226/calculator-app/actions/workflows/ci.yml)

A calculator with a React frontend and an ASP.NET Core minimal API. The React app handles input and display; every calculation runs in C# on the server and comes back as JSON.

## Architecture

```
React (Vite, port 5173)  --POST /api/calculator/calculate-->  ASP.NET Core API (port 5080)
                                                                 └─ CalculatorService (C#)
```

- **API:** a .NET 10 minimal API with typed results. Invalid input returns a standard `ProblemDetails` response with a clear message, not a stack trace.
- **Validation:** request validation plus guards against divide-by-zero, square roots of negative numbers, results with no real value (e.g. `(-8)^0.5`) and overflow.
- **Design:** the calculation logic lives in `CalculatorService` behind `ICalculatorService` and is registered through dependency injection, so it can be tested without HTTP.
- **Docs:** an OpenAPI document is generated in development.

## Requirements

- .NET 10 SDK
- Node.js and npm

## Run locally

Start the C# API in one terminal:

```powershell
dotnet run --launch-profile CalculatorApp
```

Start the React development server in a second terminal:

```powershell
cd frontend
npm install
npm run dev
```

Open the local URL printed by Vite, usually `http://127.0.0.1:5173`. Keep both terminals running while using the calculator. Vite forwards `/api` requests to the C# API at `http://127.0.0.1:5080`.

The API's OpenAPI document is available at `http://127.0.0.1:5080/openapi/v1.json` in development.

## Tests

Unit tests for `CalculatorService` use xUnit v3 and cover every operation, the error cases and edge values (NaN, infinity, overflow, missing operands).

```powershell
dotnet test --project tests/CalculatorApp.Tests/CalculatorApp.Tests.csproj
```

GitHub Actions builds the API, runs the tests and builds the React app on every push.

## Supported operations

Addition, subtraction, multiplication, division, modulus, powers, and square roots.

## Project structure

```
Program.cs                    API setup and the calculate endpoint
Models/                       Request and response records
Services/                     CalculatorService and its interface
tests/CalculatorApp.Tests/    xUnit tests
frontend/                     React + Vite app
```
