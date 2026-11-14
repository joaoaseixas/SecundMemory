# Builds the Angular app and the API into one image; the API serves the UI.
FROM node:22-alpine AS web
WORKDIR /web
COPY frontend/secund-memory-web/package*.json ./
RUN npm ci
COPY frontend/secund-memory-web/ ./
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api
WORKDIR /src
COPY backend/ ./backend/
RUN dotnet publish backend/src/SecundMemory.Api/SecundMemory.Api.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=api /out ./
COPY --from=web /web/dist/secund-memory-web/browser ./wwwroot
ENV ASPNETCORE_ENVIRONMENT=Production
# Hosts like Render inject PORT; fall back to 8080.
CMD ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} exec dotnet SecundMemory.Api.dll"]
