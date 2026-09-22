# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only project files first so restore is cached independently of source changes
COPY Common/*.csproj Common/
COPY DTO/*.csproj DTO/
COPY Entities/*.csproj Entities/
COPY Service/*.csproj Service/
COPY MilkBilling/*.csproj MilkBilling/
COPY Repository/*.csproj Repository/

# Restore main project
RUN dotnet restore MilkBilling/MilkBilling.csproj

# Copy everything else
COPY . .

# Publish main project
RUN dotnet publish MilkBilling/MilkBilling.csproj -c Release -o /app --no-restore

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published app
COPY --from=build /app .

# Photo upload directory; the host volume mounts over this at runtime
RUN mkdir -p /app/uploads

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "MilkBilling.dll"]