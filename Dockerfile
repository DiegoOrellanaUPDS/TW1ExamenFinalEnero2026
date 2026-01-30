# Archivo: Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ExamenFinal.csproj", "./"]
RUN dotnet restore "ExamenFinal.csproj"
COPY . .
RUN dotnet build "ExamenFinal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExamenFinal.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5026
ENV ASPNETCORE_URLS=http://+:5026
ENTRYPOINT ["dotnet", "ExamenFinal.dll"]
