FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ADSD.Backend.Auth/ADSD.Backend.Auth.csproj", "ADSD.Backend.Auth/"]
RUN dotnet restore "ADSD.Backend.Auth/ADSD.Backend.Auth.csproj"
COPY . .
WORKDIR "/src/ADSD.Backend.Auth"
RUN dotnet build "ADSD.Backend.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ADSD.Backend.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ADSD.Backend.Auth.dll"]
