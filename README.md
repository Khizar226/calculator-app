# Calculator App

A calculator with a React frontend and an ASP.NET Core API that performs the calculations in C#.

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

## Supported operations

Addition, subtraction, multiplication, division, modulus, powers, and square roots.
