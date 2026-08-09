# -------------------------------------
# Stage 01: Shared build
# -------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app
COPY . .

# Executa diretamente da raiz (/app)
RUN dotnet restore AuthenticationAPI.slnx
RUN dotnet build AuthenticationAPI.slnx -c Release

# -------------------------------------
# Publish
# -------------------------------------
FROM build AS publish-authentication-api
RUN dotnet publish AuthenticationAPI/AuthenticationAPI.csproj \
    -c Release \
    -o /app/publish/authentication-api \
    --no-build

# -------------------------------------
# Runtime
# -------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS authentication-api
WORKDIR /app
COPY --from=publish-authentication-api /app/publish/authentication-api .
EXPOSE 8080

# Certifique-se de que o nome do .dll condiz com o Assembly Name da sua aplicação
ENTRYPOINT [ "dotnet", "AuthenticationAPI.dll" ]