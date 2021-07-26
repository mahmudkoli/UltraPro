#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["UltraPro.IdentityServer4/UltraPro.IdentityServer4.csproj", "UltraPro.IdentityServer4/"]
RUN dotnet restore "UltraPro.IdentityServer4/UltraPro.IdentityServer4.csproj"
COPY . .
WORKDIR "/src/UltraPro.IdentityServer4"
RUN dotnet build "UltraPro.IdentityServer4.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UltraPro.IdentityServer4.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UltraPro.IdentityServer4.dll"]