FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY ./src/Mandarin/bin/Debug/netcoreapp3.1/publish/ ./
ENV ASPNETCORE_URLS=http://*:8080
ENTRYPOINT [ "dotnet", "Mandarin.dll" ]
