# :tangerine: The Little Mandarin

![Build](https://github.com/KK578/Mandarin/workflows/Build/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/KK578/Mandarin/badge.svg?branch=master)](https://coveralls.io/github/KK578/Mandarin?branch=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=KK578_Mandarin&metric=alert_status)](https://sonarcloud.io/dashboard?id=KK578_Mandarin)

This is the Blazor Web Application for [https://thelittlemandarin.co.uk](https://thelittlemandarin.co.uk).

## Building from Source

Requirements:
 - .NET Core SDK 6.x
 - A running postgresql instance.
   - Update `src/Mandarin/appsettings.json` to match your postgres instance's connection details.
   - Alternatively, run against a new default instance against docker  
     `docker run --name mandarin-db -p 5432:5432 -d postgres`
 - For tests, a throwaway postgresql instance running against port 5555.  
   `docker run --name mandarin-test-db -p 5555:5432 -d postgres`

```sh
$ git clone https://github.com/KK578/Mandarin.git
$ dotnet build
$ dotnet test
$ dotnet run --project src/Mandarin/Mandarin.csproj
```
