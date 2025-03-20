#IMAGEN DE PRODUCCION
##############################
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


#IMAGEN PARA CONSTRUIR EL BUILD
##############################
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookingApplication.WebApi/BookingApplication.WebApi.csproj", "BookingApplication.WebApi/"]
COPY ["BookingApplication.Services/BookingApplication.Services.csproj", "BookingApplication.Services/"]
COPY ["BookingApplication.Dal/BookingApplication.Dal.csproj", "BookingApplication.Dal/"]
COPY ["BookingApplication.Domain/BookingApplication.Domain.csproj", "BookingApplication.Domain/"]
RUN dotnet restore "./BookingApplication.WebApi/BookingApplication.WebApi.csproj"
COPY . .
WORKDIR "/src/BookingApplication.WebApi"
RUN dotnet build "./BookingApplication.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BookingApplication.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingApplication.WebApi.dll"]