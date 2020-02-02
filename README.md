# libc.hwid
## Unique ID generator for distributing your .Net Core software (Ubuntu & Windows)

### Why we need this?
- We want to distribute a software and we worry about possible copyright violations.
- We don't want your software to be distributed without our consent

### How does it work?
- `HwId.Generate()` method combines __Motherboard info__ with __CPU ID__ into a string
- Then hashes the string into a 40 character ASCII string. This final string value is the __Hardware ID__

### Considerations:
- So called __Hardware ID__, created with `HwId.Generate()` method, will return the same value on a computer system as long as the _mother board_ and _CPU_ __do not change__
- This library does not use _Mac address_ since the value returned from WMI on Microsoft Windows will change if the computer's network connection is lost or disabled.

### I've used this piece of code in my projects on the following operating systems:
- Ubuntu 16.04
- Ubuntu 18.04.1 LTS
- Windows 7
- Windows 7 SP1
- Windows 8
- Windows 8.1
- Windows 10

### Usage

You can add this nuget package [![libc.hwid](https://img.shields.io/nuget/v/Dapper.svg)](https://www.nuget.org/packages/libc.hwid/) to your project and use as below:
```
var hardwareId = libc.hwid.HwId.Generate();
```
Look at the simple test to understand the usage
