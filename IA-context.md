# IA-context.md: "The Father of All Readmes"

## 🚀 Propósito del Proyecto
Este proyecto es el backend para el **Examen Final de Tecnología Web (Enero 2026)**. Su objetivo es exponer una API RESTful desarrollada en .NET 8 que interactúa con una base de datos PostgreSQL. El sistema gestiona entidades clave como `Persona` y `VictorCox` (una entidad de prueba o específica del examen).

El namespace raíz absoluto y obligatorio es `ExamenFinal`. No se permiten otros namespaces como `Universidad` o `Entidades` sin prefijo.

## 🛠️ Stack Tecnológico
- **Framework**: .NET 8 (ASP.NET Core Web API)
- **Lenguaje**: C# 12
- **ORM**: Entity Framework Core 8.0 (Npgsql.EntityFrameworkCore.PostgreSQL)
- **Base de Datos**: PostgreSQL 16
- **Contenedorización**: Docker & Docker Compose
- **Documentación API**: Swagger / OpenAPI (Swashbuckle)

## 🏗️ Arquitectura del Sistema

### Estructura de Proyecto
```
TW1ExamenFinalEnero2026/
├── docker-compose.yml       # Orquestación de servicios (API + DB)
└── Backend/
    └── ExamenFinal/         # Proyecto Principal (.csproj)
        ├── Controllers/     # Controladores API
        │   ├── PersonasControllers.cs
        │   └── VictorCoxController.cs
        ├── Data/            # Contexto de base de datos
        │   └── AppDbContext.cs
        ├── Entidades/       # Modelos de dominio
        │   ├── Persona.cs
        │   └── VictorCox.cs
        ├── Dockerfile       # Definición de construcción de imagen
        ├── Program.cs       # Punto de entrada y configuración de servicios
        └── appsettings.json # Configuración (aunque se priorizan variables de entorno)
```

### Reglas de Diseño (CRÍTICAS)
1.  **Namespaces**: TODO código debe estar bajo el namespace `ExamenFinal`.
    -   `ExamenFinal.Entidades`
    -   `ExamenFinal.Data`
    -   `ExamenFinal.Controllers`
2.  **Inyección de Dependencias**: El `AppDbContext` se inyecta en los controladores.
3.  **Configuración de DB**: La cadena de conexión se obtiene de la variable de entorno `DATABASE_URL` (en producción/docker) o `ConnectionStrings:Connection` (en desarrollo local).
4.  **CORS**: Política "MyApp" que permite `AnyOrigin`, `AnyHeader`, `AnyMethod`.

## 💾 Modelo de Datos (Entidades)

### 1. Persona (`ExamenFinal.Entidades.Persona`)
Representa a una persona física en el sistema.
- `Id` (PK, int)
- `Ci` (int, Cédula de Identidad)
- `Nombre` (string)
- `Apellido` (string)
- `FechaNacimiento` (DateOnly)
- `Estado` (string) - Se usa para borrado lógico ("Borrado").

### 2. VictorCox (`ExamenFinal.Entidades.VictorCox`)
Entidad específica para el examen.
- `Id` (PK, int)
- `Nombre` (string)
- `Edad` (int)
- `Estado` (string) - **Restricción**: Solo permite valores "soltero", "casado", "arrecho" (case-insensitive).

## 🔌 API Endpoints

### Personas (`/api/Personas`)
- `GET /`: Listar todas las personas activas (no "Borrado").
- `GET /{ci}`: Obtener persona por CI.
- `POST /`: Crear nueva persona (Valida duplicidad por CI).
- `PUT /{ci}`: Actualizar persona.
- `DELETE /{ci}`: Borrado lógico (cambia Estado a "Borrado").

### VictorCox (`/api/VictorCox`)
- `POST /`: Crear registro. Valida que `Estado` sea válido.
- `GET /{id}`: Obtener registro por ID.

## 🐳 Despliegue e Infraestructura

### Docker Compose (`docker-compose.yml`)
Define dos servicios:
1.  **victorcox.api**:
    -   Construye desde `./Backend/ExamenFinal`.
    -   Puerto externo: `5000` (interno 8080).
    -   Depende de `victorcox.db`.
    -   Variable: `ConnectionStrings__DefaultConnection` apuntando al host `victorcox.db`.
2.  **victorcox.db**:
    -   Imagen: `postgres:16`.
    -   Usuario/Pass: `postgres` / `postgres123`.
    -   Volumen: `victorcox_data` para persistencia.
    -   Puerto: `5433` (mapeado al 5432 interno).

### Comandos de Ejecución
**Para levantar el entorno completo:**
```bash
docker-compose up --build -d
```
La API estará disponible en: `http://localhost:5000/swagger`

**Para desarrollo local (sin Docker para API):**
```bash
cd Backend/ExamenFinal
dotnet run
```
*Nota: Asegurarse de tener una BD corriendo y ajustar `appsettings.json` o variables de entorno.*

## ⚠️ Notas para Agentes de IA
- Al modificar código, **SIEMPRE** verificar los namespaces e includes.
- El proyecto usa **Migraciones Automáticas** al iniciar la aplicación en `Program.cs` (`dbContext.Database.Migrate()`). No es necesario ejecutar `dotnet ef database update` manualmente en producción.
- Si agregas nuevas entidades, recuerda agregarlas al `DbSet` en `AppDbContext.cs`.
