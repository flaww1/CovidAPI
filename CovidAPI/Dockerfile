# Use the official .NET 6 SDK as a base image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy your project file
COPY ["CovidAPI/CovidAPI.csproj", "CovidAPI/"]

# Restore the dependencies of your project
RUN dotnet restore "CovidAPI/CovidAPI.csproj"

# Copy the rest of your app source code
COPY . .

# Change the working directory to your app's source code
WORKDIR "/src/CovidAPI"

# Build your app
RUN dotnet build "CovidAPI.csproj" -c Release -o /app/build

# Switch to a new stage for publishing
FROM build AS publish
RUN dotnet publish "CovidAPI.csproj" -c Release -o /app/publish

# Use the official ASP.NET 6 runtime as a base image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# Set the entry point for your app
ENTRYPOINT ["dotnet", "CovidAPI.dll"]
