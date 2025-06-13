# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .

RUN dotnet tool install --global dotnet-ef \
    && export PATH="$PATH:/root/.dotnet/tools"
ENV PATH="$PATH:/root/.dotnet/tools"
RUN dotnet publish -o /app

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

# Copiar script de entrada
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]

