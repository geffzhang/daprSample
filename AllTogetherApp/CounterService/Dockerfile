#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CounterService/CounterService.csproj", "CounterService/"]
RUN dotnet restore "CounterService/CounterService.csproj"
COPY . .
WORKDIR "/src/CounterService"
RUN dotnet build "CounterService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish --no-restore "CounterService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CounterService.dll"]