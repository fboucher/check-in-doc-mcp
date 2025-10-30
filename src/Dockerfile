
ARG DOTNET_VERSION=9.0

# Use .NET SDK for building
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY CheckInDocMCP.csproj .
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Use .NET runtime for the final image
FROM mcr.microsoft.com/dotnet/runtime:${DOTNET_VERSION} AS runtime
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "CheckInDocMCP.dll"]
