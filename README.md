# :tangerine: The Little Mandarin

![Build](https://github.com/KK578/Mandarin/workflows/Build/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/KK578/Mandarin/badge.svg?branch=master)](https://coveralls.io/github/KK578/Mandarin?branch=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=KK578_Mandarin&metric=alert_status)](https://sonarcloud.io/dashboard?id=KK578_Mandarin)

This is the Blazor Web Application for [https://thelittlemandarin.co.uk](https://thelittlemandarin.co.uk).

## Building from Source

Requirements:
 - .NET Core SDK 3.1.x
 - Node.js/npm
 - Gulp CLI
 
```sh
$ git clone https://github.com/KK578/Mandarin.git
$ cd Mandarin/src/Mandarin
$ npm install
$ gulp
$ cd ../../
$ dotnet build
$ dotnet test
$ dotnet run --project src/Mandarin/Mandarin.csproj
```
