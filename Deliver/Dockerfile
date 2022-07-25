#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Deliver/Deliver.csproj", "Deliver/"]
COPY ["DeployApp/DeployApp.csproj", "DeployApp/"]
COPY ["Repository/Repository.csproj", "Repository/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["Integrations/Integrations.csproj", "Integrations/"]
COPY ["Services/Services.csproj", "Services/"]
RUN dotnet restore "Deliver/Deliver.csproj"
COPY . .
WORKDIR "/src/Deliver"
RUN dotnet build "Deliver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Deliver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /DeployApp

COPY --from=build /src/DeployApp/ .

WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "Deliver.dll", "localSql"]