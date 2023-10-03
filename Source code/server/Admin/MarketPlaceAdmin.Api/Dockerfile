# Stage 1: Base stage with the ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base

# Set the working directory inside the container to /app
WORKDIR /app

# Expose port 80 and 443 for the container
EXPOSE 80
EXPOSE 443

# Stage 2: Build stage with the .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory inside the container to /src
WORKDIR /src

# Copy the Admin project to the working directory
COPY ["/Admin", "./Admin"]

# Copy the Common project to the working directory
COPY ["/Common" ,"./Common"]

# Set the working directory to the Admin Api project
WORKDIR /src/Admin/MarketPlaceAdmin.Api

# Build the MarketPlaceAdmin.Api project in Release configuration and output to /app/build
RUN dotnet build MarketPlaceAdmin.Api.csproj -c Release -o /app/build

# Stage 3: Publish stage
FROM build AS publish

# Publish the MarketPlaceAdmin.Api project in Release configuration to /app/publish
RUN dotnet publish MarketPlaceAdmin.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final stage
FROM base AS final

# Set the working directory inside the container to /app
WORKDIR /app

# Copy the published files from the previous stage (publish) to the current working directory
COPY --from=publish /app/publish .

# Install curl for health check
RUN apt update && apt install curl -y

# Set the entry point for the container to run the MarketPlaceAdmin.Api.dll
ENTRYPOINT ["dotnet", "MarketPlaceAdmin.Api.dll"]