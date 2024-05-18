FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build

WORKDIR /src

COPY ["Services/Jacko.Services.ShoppingCartAPI/Jacko.Services.ShoppingCartAPI.csproj", "Services/Jacko.Services.ShoppingCartAPI/"]
COPY ["Integration/MessageBus/Jacko.MessageBus.csproj", "Integration/MessageBus/"]

RUN dotnet restore Services/Jacko.Services.ShoppingCartAPI/Jacko.Services.ShoppingCartAPI.csproj

COPY Integration/MessageBus/ Integration/MessageBus/

COPY Services/Jacko.Services.ShoppingCartAPI/ Services/Jacko.Services.ShoppingCartAPI/

RUN dotnet publish Services/Jacko.Services.ShoppingCartAPI/Jacko.Services.ShoppingCartAPI.csproj -o /publish/out -c Release /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

WORKDIR /app

COPY --from=build /publish/out/ .

ENTRYPOINT ["dotnet", "Jacko.Services.ShoppingCartAPI.dll"]
