#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update \
    && apt-get install -y iputils-ping \
    && apt-get install -y telnet

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./ShinhanBatchJob.csproj", "."]
RUN dotnet restore "ShinhanBatchJob.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ShinhanBatchJob.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShinhanBatchJob.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShinhanBatchJob.dll"]