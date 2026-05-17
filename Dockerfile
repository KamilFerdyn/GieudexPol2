# Dockerfile for GieudexPol system deployment

# Stage 1: Build the frontend
FROM node:20-alpine AS frontend-builder
WORKDIR /app
COPY GieudexPol.Frontend/package*.json ./
RUN npm install -g @angular/cli
RUN npm install
COPY GieudexPol.Frontend/ .
RUN npm run build --prod

# Stage 2: Build the backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder
WORKDIR /src
COPY GieudexPol.sln .
COPY GieudexPol.API/GieudexPol.API.csproj GieudexPol.API/
COPY GieudexPol.Application/GieudexPol.Application.csproj GieudexPol.Application/
COPY GieudexPol.Domain/GieudexPol.Domain.csproj GieudexPol.Domain/
COPY GieudexPol.Infrastructure/GieudexPol.Infrastructure.csproj GieudexPol.Infrastructure/

# ZMIANA: Przywracamy pakiety tylko dla API, co ignoruje brakujący projekt testowy
RUN dotnet restore GieudexPol.API/GieudexPol.API.csproj
COPY . .
WORKDIR /src/GieudexPol.API

# ZMIANA: Budujemy konkretny projekt API zamiast całej solucji
RUN dotnet build GieudexPol.API.csproj -c Release -o /app/build

# Stage 3: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# NAPRAWIONO: Kopiujemy CAŁY katalog dist do folderu wwwroot bez używania gwiazdki,
# co zapobiega gubieniu struktury plików i katalogów Angulara.
# COPY --from=frontend-builder /app/dist/GieudexPol.Frontend wwwroot/
COPY --from=frontend-builder /app/dist/GieudexPol.Frontend/browser wwwroot/


# Copy backend build output
COPY --from=backend-builder /app/build .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
# ENV ASPNETCORE_URLS=http://+:${PORT}
ENV ASPNETCORE_URLS=http://0.0.0.0:80

# Set entry point
ENTRYPOINT ["dotnet", "GieudexPol.API.dll"]
