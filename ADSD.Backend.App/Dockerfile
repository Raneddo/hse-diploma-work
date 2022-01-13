FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ADSD.Backend.App/ADSD.Backend.App.csproj", "ADSD.Backend.App/"]
RUN dotnet restore "ADSD.Backend.App/ADSD.Backend.App.csproj"
COPY . .
WORKDIR "/src/ADSD.Backend.App"
RUN dotnet build "ADSD.Backend.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ADSD.Backend.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ADSD.Backend.App.dll"]
