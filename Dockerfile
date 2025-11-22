# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project folders
COPY Common/ Common/
COPY DTO/ DTO/
COPY Entities/ Entities/
COPY Service/ Service/
COPY MilkBilling/ MilkBilling/
COPY Repository/ Repository/    # <-- Add this line

# Restore main project
RUN dotnet restore MilkBilling/MilkBilling.csproj

# Copy all source code
COPY . .

# Publish main project
RUN dotnet publish MilkBilling/MilkBilling.csproj -c Release -o /app

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published app
COPY --from=build /app .

# Start app
ENTRYPOINT ["dotnet", "MilkBilling.dll"]
