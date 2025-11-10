# SmartTaskMonitor
This project simulates a tool that automatically monitors repetitive tasks.

## Phase 1: Core .NET Tool
Built a C# console app that simulates an automation dashboard.
- Loads and filters system task logs from JSON.
- Exports results to CSV.
- Demonstrates .NET data handling, file I/O, and command-line UI.
- Git used for branching and version control.

Branch: `phase1-core-dotnet`

## Phase 2: AI Module
- Added a Python microservice (Flask + scikit-learn) to predict failure probabilities.
- .NET app calls the AI API and displays predicted risk.
- Demonstrates cross-language integration and intelligent automation.