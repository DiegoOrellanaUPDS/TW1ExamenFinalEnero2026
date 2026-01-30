# 1. Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar el archivo de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo lo demás y publicar la app
COPY . ./
RUN dotnet publish -c Release -o out

# 2. Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Exponer el puerto que usa .NET 8
EXPOSE 8080

# Reemplaza 'ExamenFinal.dll' si el nombre de tu proyecto es distinto
ENTRYPOINT ["dotnet", "ExamenFinal.dll"]