FROM mcr.microsoft.com/dotnet/sdk:8.0-preview AS build

WORKDIR /src

COPY ["Services/Jacko.Services.CouponAPI/Jacko.Services.CouponAPI.csproj", ""]

RUN dotnet restore

COPY Services/Jacko.Services.CouponAPI/ ./

RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-preview

WORKDIR /app

COPY --from=build /app/publish/ .

ENTRYPOINT ["dotnet", "Jacko.Services.CouponAPI.dll"]