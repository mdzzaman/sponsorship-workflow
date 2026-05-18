FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["SponsorshipWorkflow.API/SponsorshipWorkflow.API.csproj", "SponsorshipWorkflow.API/"]
COPY ["SponsorshipWorkflow.Application/SponsorshipWorkflow.Application.csproj", "SponsorshipWorkflow.Application/"]
COPY ["SponsorshipWorkflow.Infrastructure/SponsorshipWorkflow.Infrastructure.csproj", "SponsorshipWorkflow.Infrastructure/"]
COPY ["SponsorshipWorkflow.Domain/SponsorshipWorkflow.Domain.csproj", "SponsorshipWorkflow.Domain/"]

RUN dotnet restore "SponsorshipWorkflow.API/SponsorshipWorkflow.API.csproj"

COPY . .
WORKDIR "/src/SponsorshipWorkflow.API"
RUN dotnet build "SponsorshipWorkflow.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SponsorshipWorkflow.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SponsorshipWorkflow.API.dll"]
