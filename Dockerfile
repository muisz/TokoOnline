FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY *.csproj .
RUN dotnet restore

# WORKDIR /source/app
COPY . .
RUN dotnet publish -c release -o Release --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/Release ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "TokoOnline.dll"]
