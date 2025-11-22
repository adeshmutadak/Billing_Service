# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy only project file first
COPY MilkBilling/*.csproj MilkBilling/

# Restore dependencies
RUN dotnet restore MilkBilling/MilkBilling.csproj

# Copy the full source
COPY . .

# Publish build
RUN dotnet publish MilkBilling/MilkBilling.csproj -c Release -o /app


# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copy published app
COPY --from=build /app .

# Start app
ENTRYPOINT ["dotnet", "MilkBilling.dll"]
