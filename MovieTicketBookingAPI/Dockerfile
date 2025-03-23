#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Using Linux containers for better compatibility
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MovieTicketBookingAPI/MovieTicketBookingAPI.csproj", "MovieTicketBookingAPI/"]
COPY ["Repositories/Services.csproj", "Repositories/"]
COPY ["DataAccessLayers/DataAccessLayers.csproj", "DataAccessLayers/"]
COPY ["BusinessObjects/BusinessObjects.csproj", "BusinessObjects/"]
RUN dotnet restore "./MovieTicketBookingAPI/MovieTicketBookingAPI.csproj"
COPY . .
WORKDIR "/src/MovieTicketBookingAPI"
RUN dotnet build "./MovieTicketBookingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MovieTicketBookingAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieTicketBookingAPI.dll"]
