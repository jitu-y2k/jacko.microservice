FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build

WORKDIR /src

COPY ["Services/Jacko.Services.OrderAPI/Jacko.Services.OrderAPI.csproj", "Services/Jacko.Services.OrderAPI/"]
COPY ["Integration/MessageBus/Jacko.MessageBus.csproj", "Integration/MessageBus/"]

RUN dotnet restore Services/Jacko.Services.OrderAPI/Jacko.Services.OrderAPI.csproj

COPY Integration/MessageBus/ Integration/MessageBus/

COPY Services/Jacko.Services.OrderAPI/ Services/Jacko.Services.OrderAPI/

RUN dotnet publish Services/Jacko.Services.OrderAPI/Jacko.Services.OrderAPI.csproj -o /app/publish -c Release /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

WORKDIR /app

COPY --from=build /app/publish/ .

ENTRYPOINT ["dotnet", "Jacko.Services.OrderAPI.dll"]
