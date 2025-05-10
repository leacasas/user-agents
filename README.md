# UserAgents

A .NET port of [intoli/user-agents](https://github.com/intoli/user-agents), a JavaScript library for generating random, realistic user agents.

[![Update Data, Build, Test, and Publish](https://github.com/leacasas/user-agents/actions/workflows/update-and-publish.yml/badge.svg?branch=main)](https://github.com/leacasas/user-agents/actions/workflows/update-and-publish.yml)

## Overview

This library provides functionality to generate random, realistic user agents for web scraping and testing purposes. It's a direct port of the JavaScript library to .NET, maintaining the same functionality and data sources.

## Project Structure

- `UserAgents/` - The main library project containing the core functionality
- `UserAgents.Console/` - Console project to demonstrate library's functionality
- `UserAgents.Tests/` - xUnit test project for ensuring the library's functionality

## Requirements

- .NET 8.0 or later
- Visual Studio 2022 (recommended) or any other .NET IDE

## Building the Project

```bash
dotnet build
```

## Running Tests

```bash
dotnet test
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Original JavaScript library: [intoli/user-agents](https://github.com/intoli/user-agents)