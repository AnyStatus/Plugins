# AnyStatus Plugins

A repository of community contributed plugins and extensions for [AnyStatus](https://www.anystat.us).

[![Build status](https://ci.appveyor.com/api/projects/status/dvn1rwrauwyq5yx6?svg=true)](https://ci.appveyor.com/project/AnyStatus/plugins)
[![NuGet](https://img.shields.io/nuget/v/AnyStatus.Plugins.svg)]()
[![Join the chat at https://gitter.im/AnyStatus](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/AnyStatus?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

As developers used to pushing many small commits daily, we rely on monitors to notify us when builds go green.
AnyStatus is a lightweight Windows desktop app that rolls up metrics and events from various sources into one place.
Examples include build results and releases, health checks for different services and OS metrics.
Think of it as CCTray on steroids. It's also available as a Visual Studio plugin.

To learn more about AnyStatus API, please read the [documentation](https://www.anystat.us/docs/api).

Here are some of the plugins. For the updated list of plugins, please [visit our website](https://www.anystat.us/docs/plugins).

### Continuous Integration and Delivery

- [x] TFS Build
- [x] VSTS Build
- [x] VSTS Release
- [x] Jenkins Job
- [x] Jenkins View
- [x] TeamCity Build
- [x] AppVeyor Build
- [x] Coveralls Code Coverage

### Custom 

- [x] C#/VB.NET File - Compile and run custom monitors at run-time.
- [x] PowerShell
- [x] Batch Script

### Metrics

- [x] CPU Usage - Show the local or remote CPU usage
- [x] Performance Counter - Show the value of a local or remote performance counter
- [x] Upload Speed - Show network upload speed in Kbps/Mbps
- [x] Download Speed - Show network download speed in Kbps/Mbps
- [x] SQL Scalar Query - Show the return value of an SQL query

### Database

- [x] SQL Server - Test SQL Server database connection

### Network

- [x] HTTP/S - Monitor HTTP server availability and response code
- [x] Ping - Test network availability of remote servers
- [x] TCP/UDP - Monitor network connections

### Monitors

- [x] Pingdom
- [x] UptimeRobot

### Other

- [x] Windows Service - Monitor and control a local or remote Windows Service
- [x] IIS Application Pool - Monitor a remote or local IIS application pool state
- [x] GitHub Issue

## Contribute

Contributions are most welcome. Please join us in maintaining a library of updated tools that we can all use in our day to day tasks.
