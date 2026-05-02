# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj first for better layer caching
COPY Ecommerce.Api/Ecommerce.Api.csproj Ecommerce.Api/
COPY Ecommerce.Infrastructure/Ecommerce.Infrastructure.csproj Ecommerce.Infrastructure/
COPY E-commerce.Core/E-commerce.Core.csproj E-commerce.Core/

RUN dotnet restore Ecommerce.Api/Ecommerce.Api.csproj

# Copy the rest of the source
COPY . .

WORKDIR /src/Ecommerce.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Ecommerce.Api.dll"]
