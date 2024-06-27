# VSCODE
# FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
# WORKDIR /app
# EXPOSE 5000

# ENV ASPNETCORE_URLS=http://+:5000

# FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
# ARG configuration=Release
# WORKDIR /src
# COPY ["TamagotchiAPI.csproj", "./"]
# RUN dotnet restore "TamagotchiAPI.csproj"
# COPY . .
# WORKDIR "/src/."
# RUN dotnet build "TamagotchiAPI.csproj" -c $configuration -o /app/build

# FROM build AS publish
# ARG configuration=Release
# RUN dotnet publish "TamagotchiAPI.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "TamagotchiAPI.dll"]

# Microsoft website
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /TamagotchiAPI

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /TamagotchiAPI
COPY --from=build-env /TamagotchiAPI/publish .
ENTRYPOINT ["dotnet", "TamagotchiAPI.dll"]


# GP
# Use the official ASP.NET Core runtime as a parent image
# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# WORKDIR /app
# EXPOSE 80

# # Use the SDK image to build and publish the application
# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src
# COPY ["TamagotchiAPI/TamagotchiAPI.csproj", "TamagotchiAPI/"]
# RUN dotnet restore "TamagotchiAPI/TamagotchiAPI.csproj"
# COPY . .
# WORKDIR "/src/TamagotchiAPI"
# RUN dotnet build "TamagotchiAPI.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "TamagotchiAPI.csproj" -c Release -o /app/publish

# # Use the base image to run the application
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "TamagotchiAPI.dll"]
