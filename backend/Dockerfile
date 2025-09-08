# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore dependencies
COPY ShoppingListApi/*.csproj ./ShoppingListApi/
WORKDIR /source/ShoppingListApi
RUN dotnet restore

# Copy the rest of the source code
WORKDIR /source
COPY ShoppingListApi/. ./ShoppingListApi/

# Build and publish the application
WORKDIR /source/ShoppingListApi
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose the port the application listens on (ASP.NET Core defaults to 8080 in containers)
# Your k8s service targets this ContainerPort
EXPOSE 8080

# Set the entrypoint to run the application
ENTRYPOINT ["dotnet", "ShoppingListApi.dll"]

# Optional: Set ASPNETCORE_URLS if you need to force it to listen on a specific port/interface
# ENV ASPNETCORE_URLS=http://+:80