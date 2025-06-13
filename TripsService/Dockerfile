# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet tool install --global dotnet-ef
RUN dotnet ef database update
RUN dotnet publish -o /app

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

# Copiar script de entrada
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]


