# ADR-01: Deuda técnica en el composition root — connection string hardcodeado y Program.cs como God File

| Campo  | Valor |
|--------|-------|
| Autor  | Ariff Medina |
| Fecha  | 15/07/2026 |
| Estado | `Aceptado` |

---

## Contexto

Al integrar SQL Server + ASP.NET Core Identity a `CitasApp.Web` para
persistir el login (sin tocar el resto del dominio, que sigue en JSON), todo
el registro de servicios se hizo directamente en `Program.cs`, priorizando
validar rápido que Identity y las migraciones funcionaran. Eso dejó dos code
smells en el mismo archivo, originados en el mismo evento.

## Code smells identificados

| # | Code smell | Ubicación | Descripción |
|---|-----------|-----------|--------------|
| 1 | Tight Coupling | `CitasApp.Web/Program.cs`, línea del `AddDbContext` | El connection string de SQL Server estaba escrito como literal dentro de `UseSqlServer("Server=(localdb)\\MSSQLLocalDB;...")`, en vez de venir de configuración externa |
| 2 | Low Cohesion / God File | `CitasApp.Web/Program.cs` completo | El archivo mezclaba registro de `DbContext`, Identity, Razor Pages, MVC, los 3 repositorios de dominio y el pipeline HTTP — más de 40 líneas de configuración sin separación de responsabilidades |

## ¿Por qué son code smells?

**#1 — Tight Coupling:** un code smell de acoplamiento no es solo entre dos
clases (como el ejemplo de `EmailSender` visto en clase) — también aplica
cuando el código se acopla a un **entorno de ejecución**. Aquí, un valor que
cambia según la máquina (dev, staging, la laptop de otro compañero) quedaba
"quemado" dentro del código fuente, en vez de vivir en un lugar externo e
intercambiable. Señal concreta: para correr el proyecto en otra máquina,
había que **editar y recompilar código**, no solo cambiar configuración.

**#2 — Low Cohesion / God File:** es el mismo problema de God Class visto en
la Semana 11 (una clase que hace de todo), aplicado a nivel de archivo de
configuración en vez de a nivel de clase de dominio. `Program.cs` no tenía
una sola razón para cambiar — tenía cinco: si cambiaba la base de datos, si
cambiaba Identity, si se agregaba un repositorio, si cambiaba el pipeline
HTTP, todo se editaba en el mismo lugar. Señal concreta: para agregar
cualquier servicio nuevo había que leer y entender todo el archivo para
saber dónde insertar la línea sin romper el orden del pipeline.

## Cómo se resolvieron

**#1 se resolvió extrayendo el connection string a `appsettings.json`**, leído
vía `IConfiguration` (Options pattern):

```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CitasAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**#2 se resolvió con Extract Class + Extract Method**, dividiendo el registro
de servicios en 3 extension methods de `IServiceCollection`, cada uno en su
propio archivo bajo `CitasApp.Web/Extensions/`:

```
CitasApp.Web/Extensions/
├── PersistenceServiceExtensions.cs   → AddPersistence(configuration)
├── IdentityServiceExtensions.cs      → AddIdentityConfig()
└── RepositoryServiceExtensions.cs    → AddDomainRepositories()
```

`Program.cs` quedó como orquestador de ~24 líneas, sin ningún literal de
conexión y sin mezclar responsabilidades:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPersistence(builder.Configuration)
    .AddIdentityConfig()
    .AddDomainRepositories();

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
```

Se dividió en **un archivo por responsabilidad** desde ahora (en vez de una
sola clase `ServiceCollectionExtensions` con los 3 métodos juntos) para
evitar que esa clase se vuelva el mismo God File movido de lugar cuando se
agreguen más responsabilidades — por ejemplo cuando se implementen los roles
de usuario pendientes.

Ambos fixes se verificaron sin cambiar comportamiento observable: la app
sigue levantando igual y `/Identity/Account/Register` sigue guardando en
`AspNetUsers`.

---

## Alternativas consideradas

| Alternativa | Por qué la descarté |
|-------------|---------------------|
| Dejar el connection string hardcodeado permanentemente, ya que el proyecto es académico y corre en una sola máquina | Bloquea que cualquier compañero o el profesor levanten el proyecto sin editar código fuente |
| Mantener todo el registro de servicios en `Program.cs` porque "no va a crecer mucho más" | El proyecto tiene funcionalidad pendiente confirmada (roles vía `AspNetRoles`) que va a seguir agregando servicios |
| Resolver las dos deudas en commits separados y en momentos distintos | Ambas vivían en el mismo archivo y se originaron en el mismo evento; separarlas hubiera duplicado la verificación de que el comportamiento no cambió |
| Una sola clase `ServiceCollectionExtensions` con los 3 métodos juntos, en vez de un archivo por responsabilidad | Resuelve el God File actual pero reintroduce el mismo riesgo un nivel más abajo si esa clase sigue creciendo |

## Consecuencias

**✅ Lo que gano:**

- **Técnica:** el connection string es intercambiable por entorno sin
  recompilar, y cada responsabilidad de arranque vive en su propio archivo,
  fácil de ubicar y modificar sin tocar las demás.
- **Proceso/equipo:** cualquier compañero puede clonar el repo y levantarlo
  configurando solo su `appsettings.json` local, y un code review de
  "agregué un repositorio" ahora toca un solo archivo pequeño.

**⚠️ Lo que sacrifico o asumo:**

- **Limitación técnica:** `appsettings.json` sigue siendo un archivo
  versionado en git — para producción real haría falta dar el siguiente paso
  a User Secrets o variables de entorno. Además, dividir en más archivos
  agrega indirección: entender el arranque completo ahora requiere abrir 4
  archivos en vez de uno.
- **Deuda/riesgo:** si no se mantiene disciplina, cada archivo de
  `Extensions/` puede volver a crecer hasta convertirse en su propio God File.
  Propuesta: revisar cada extension method cuando pase de ~15-20 líneas y
  volver a aplicar Extract Method si hace falta.
```
