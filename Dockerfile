# Microsoft website
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /TamagotchiAPI

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish TamagotchiAPI.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /TamagotchiAPI
COPY --from=build-env /TamagotchiAPI/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "TamagotchiAPI.dll"]