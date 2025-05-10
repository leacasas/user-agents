# UserAgents

A .NET port of [intoli/user-agents](https://github.com/intoli/user-agents), a JavaScript library for generating random, realistic user agents.

[![Update Data, Build, Test, and Publish](https://github.com/leacasas/user-agents/actions/workflows/update-and-publish.yml/badge.svg?branch=main)](https://github.com/leacasas/user-agents/actions/workflows/update-and-publish.yml)

[![NuGet Version](https://img.shields.io/nuget/v/UserAgents.Net)](https://img.shields.io/nuget/v/UserAgents.Net)


## Overview

This library provides functionality to generate random, realistic user agents for web scraping and testing purposes. It's a direct port of the JavaScript library to .NET, maintaining the same functionality and data sources.

## Installation

```bash
dotnet add package UserAgents.Net
```

## Usage

### Basic Usage

```csharp
using UserAgents;

// Create a new instance of UserAgentSelector
var selector = new UserAgentSelector();

// Get a random user agent
var userAgent = selector.GetRandom();
Console.WriteLine(userAgent.UserAgent);
```

### Using with HttpClient

```csharp
using UserAgents;

var selector = new UserAgentSelector();
var userAgent = selector.GetRandom();

using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.UserAgent.Clear();
httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent.UserAgent);
```

### Filtering User Agents

```csharp
using UserAgents;
using UserAgents.Models;

var selector = new UserAgentSelector();

// Get an iPhone user agent
var iphoneFilter = new UserAgentFilter { Platform = "iPhone" };
var iphoneUserAgent = selector.GetRandom(iphoneFilter);

// Get a desktop user agent with high resolution screen
var desktopFilter = new UserAgentFilter 
{ 
    MinScreenWidth = 1920,
    MinScreenHeight = 1080
};
var desktopUserAgent = selector.GetRandom(desktopFilter);

// Get a mobile user agent with WiFi connection
var mobileWifiFilter = new UserAgentFilter 
{ 
    MaxScreenWidth = 768,
    ConnectionType = "wifi"
};
var mobileWifiUserAgent = selector.GetRandom(mobileWifiFilter);

// Get a user agent matching a specific browser version using regex
var chromeFilter = new UserAgentFilter
{
    UserAgentPattern = @"Chrome/120\.0"
};
var chromeUserAgent = selector.GetRandom(chromeFilter);
```

### Ignoring Weights

By default, user agents are selected based on their weight distribution. To ignore weights and select completely randomly:

```csharp
var selector = new UserAgentSelector();

// Get a completely random user agent, ignoring weights
var randomUserAgent = selector.GetRandom(ignoreWeights: true);

// Get a random iPhone user agent, ignoring weights
var iphoneFilter = new UserAgentFilter { Platform = "iPhone" };
var randomIphoneUserAgent = selector.GetRandom(iphoneFilter, ignoreWeights: true);
```

## Project Structure

- `UserAgents/` - The main library project containing the core functionality
- `UserAgents.Console/` - Console project to demonstrate library's functionality
- `UserAgents.Tests/` - xUnit test project for ensuring the library's functionality

## Requirements

- .NET 9.0 or later (broader support will come later)
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