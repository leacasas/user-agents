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
if (userAgent != null)
{
    Console.WriteLine(userAgent.UserAgentString);
}
```

### Using with HttpClient

```csharp
using UserAgents;

var selector = new UserAgentSelector();
var userAgent = selector.GetRandom();

if (userAgent != null)
{
    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.UserAgent.Clear();
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent.UserAgentString);
}
```

### Filtering User Agents

```csharp
using UserAgents;

var selector = new UserAgentSelector();

// Get an iPhone user agent
var iphoneFilter = new UserAgentFilter { Platform = "iPhone" };
var iphoneUserAgent = selector.GetRandom(iphoneFilter);
if (iphoneUserAgent != null)
{
    Console.WriteLine(iphoneUserAgent.UserAgentString);
}

// Get a desktop user agent with high resolution screen
var desktopFilter = new UserAgentFilter 
{ 
    MinScreenWidth = 1920,
    MinScreenHeight = 1080
};
var desktopUserAgent = selector.GetRandom(desktopFilter);
if (desktopUserAgent != null)
{
    Console.WriteLine(desktopUserAgent.UserAgentString);
}

// Get a mobile user agent with WiFi connection
var mobileWifiFilter = new UserAgentFilter 
{ 
    MaxScreenWidth = 768,
    ConnectionType = "wifi"
};
var mobileWifiUserAgent = selector.GetRandom(mobileWifiFilter);
if (mobileWifiUserAgent != null)
{
    Console.WriteLine(mobileWifiUserAgent.UserAgentString);
}

// Get a user agent matching a specific browser version using regex
var chromeFilter = new UserAgentFilter
{
    UserAgentPattern = @"Chrome/120\.0"
};
var chromeUserAgent = selector.GetRandom(chromeFilter);
if (chromeUserAgent != null)
{
    Console.WriteLine(chromeUserAgent.UserAgentString);
}
```

### Getting Multiple Random User Agents

```csharp
using UserAgents;

var selector = new UserAgentSelector();

// Get 5 random user agents
var randomUserAgents = selector.GetManyRandom(5).ToList();
foreach (var ua in randomUserAgents.Where(u => u != null))
{
    Console.WriteLine(ua.UserAgentString);
}

// Get 3 random Android user agents
var androidFilter = new UserAgentFilter { Platform = "Android" };
var androidUserAgents = selector.GetManyRandom(3, androidFilter).ToList();
foreach (var ua in androidUserAgents.Where(u => u != null))
{
    Console.WriteLine(ua.UserAgentString);
}

// Get 4 random user agents with complex criteria
var complexFilter = new UserAgentFilter 
{ 
    Platform = "Win32",
    EffectiveConnectionType = "4g",
    MinScreenWidth = 1920,
    MinScreenHeight = 1080
};
var complexUserAgents = selector.GetManyRandom(4, complexFilter).ToList();
foreach (var userAgent in complexUserAgents.Where(u => u != null))
{
    Console.WriteLine(userAgent.UserAgentString);
}
```

### Getting All Matching User Agents

```csharp
using UserAgents;

var selector = new UserAgentSelector();

// Get all user agents with 4K resolution
var highResFilter = new UserAgentFilter 
{ 
    MinScreenWidth = 3840,
    MinScreenHeight = 2160
};
var highResUserAgents = selector.GetAllMatching(highResFilter).ToList();
foreach (var ua in highResUserAgents)
{
    Console.WriteLine($"{ua.UserAgentString} ({ua.ScreenWidth}x{ua.ScreenHeight})");
}

// Get all mobile user agents with 5G connection
var mobile5GFilter = new UserAgentFilter 
{ 
    MaxScreenWidth = 768,
    EffectiveConnectionType = "5g"
};
var mobile5GUserAgents = selector.GetAllMatching(mobile5GFilter).ToList();
foreach (var ua in mobile5GUserAgents)
{
    Console.WriteLine($"{ua.UserAgentString} ({ua.Connection.EffectiveType})");
}
```

### Ignoring Weights

By default, user agents are selected based on their weight distribution. To ignore weights and select completely randomly:

```csharp
var selector = new UserAgentSelector();

// Get a completely random user agent, ignoring weights
var randomUserAgent = selector.GetRandom(ignoreWeights: true);
if (randomUserAgent != null)
{
    Console.WriteLine(randomUserAgent.UserAgentString);
}

// Get a random iPhone user agent, ignoring weights
var iphoneFilter = new UserAgentFilter { Platform = "iPhone" };
var randomIphoneUserAgent = selector.GetRandom(iphoneFilter, ignoreWeights: true);
if (randomIphoneUserAgent != null)
{
    Console.WriteLine(randomIphoneUserAgent.UserAgentString);
}

// Get multiple random user agents, ignoring weights
var randomUserAgents = selector.GetManyRandom(5, ignoreWeights: true).ToList();
foreach (var ua in randomUserAgents.Where(u => u != null))
{
    Console.WriteLine(ua.UserAgentString);
}

// Get multiple filtered user agents, ignoring weights
var androidUserAgents = selector.GetManyRandom(3, androidFilter, ignoreWeights: true).ToList();
foreach (var ua in androidUserAgents.Where(u => u != null))
{
    Console.WriteLine(ua.UserAgentString);
}
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