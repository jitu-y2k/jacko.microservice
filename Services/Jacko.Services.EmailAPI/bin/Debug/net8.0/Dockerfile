FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build

WORKDIR /src

COPY ["Services/Jacko.Services.EmailAPI/Jacko.Services.EmailAPI.csproj", ""]

RUN dotnet restore

COPY Services/Jacko.Services.EmailAPI/ ./

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

WORKDIR /app

COPY --from=build /app/publish/ .

ENTRYPOINT ["dotnet", "Jacko.Services.EmailAPI.dll"]