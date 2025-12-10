# Advent of Code Solver CLI

A command-line tool for running your [Advent of Code](https://adventofcode.com/) solutions with support for running all solutions, filtering by year/day, and toggling between real puzzle input and example input.

## Table of Contents

- [Requirements](#requirements)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Command Reference](#command-reference)

## Requirements

- **.NET 10.0 SDK** or newer  
  Verify your installation: `dotnet --version`

## Getting Started

### Build the Project

```bash
cd AdventOfCode
dotnet build
```

### Quick Start

Run all available solutions:

```bash
dotnet run --project AdventOfCode.Cli -- run
```

## Usage

The CLI uses the `run` subcommand with optional filters. The general syntax is:

```bash
dotnet run --project AdventOfCode.Cli -- run [options]
```

### Run All Solutions

Execute every solution across all years:

```bash
dotnet run --project AdventOfCode.Cli -- run
```

### Run by Year

Execute all solutions for a specific year:

```bash
dotnet run --project AdventOfCode.Cli -- run --year 2024
dotnet run --project AdventOfCode.Cli -- run -y 2025
```

### Run Specific Day

Execute a single day's solution (requires `--year`):

```bash
dotnet run --project AdventOfCode.Cli -- run --year 2025 --day 3
dotnet run --project AdventOfCode.Cli -- run -y 2025 -d 3
```

### Use Example Input

Run solutions using example input instead of real puzzle input:

```bash
# Run all solutions with example input
dotnet run --project AdventOfCode.Cli -- run --example

# Run specific year with example input
dotnet run --project AdventOfCode.Cli -- run -y 2025 --example

# Run specific day with example input
dotnet run --project AdventOfCode.Cli -- run -y 2025 -d 3 -e
```

### Examples

Test a solution with example input first:

```bash
dotnet run --project AdventOfCode.Cli -- run -y 2025 -d 9 -e
```

Then run with real input:

```bash
dotnet run --project AdventOfCode.Cli -- run -y 2025 -d 9
```

Run all solutions for the current year:

```bash
dotnet run --project AdventOfCode.Cli -- run -y 2025
```

## Command Reference

### `run` - Execute solutions

```bash
dotnet run --project AdventOfCode.Cli -- run [options]
```

#### Options

| Option           | Short | Description                                         |
| ---------------- | ----- | --------------------------------------------------- |
| `--year <value>` | `-y`  | Run solutions for a specific year                   |
| `--day <value>`  | `-d`  | Run solution for a specific day (requires `--year`) |
| `--example`      | `-e`  | Use example input instead of real input             |

#### Validation Rules

- `--day` cannot be used without `--year`
- Year and day values must be numeric
- The specified year/day combination must exist in the solution set
