## 👨‍💻 Información del Estudiante

- **Nombre:** Ariff Medina
- **Matrícula:** SW2509006
- **Grupo:** A
- **Cuatrimestre:** Tercer Cuatrimestre
- **Carrera:** TSU en Desarrollo e Innovación de Software
- **Profesor:** Jorge Javier Pedrozo Romero

# 🏥 CitasApp

Sistema de gestión de citas médicas desarrollado con ASP.NET Core MVC siguiendo una **arquitectura hexagonal multiproyecto**. Permite administrar pacientes, médicos y agenda de citas desde una interfaz web limpia, sin necesidad de una base de datos externa.

---

## Tabla de contenidos

- [Descripción general](#descripción-general)
- [Arquitectura hexagonal](#arquitectura-hexagonal)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Patrones de diseño (GOF)](#patrones-de-diseño-gof)
- [Funcionamiento básico](#funcionamiento-básico)
- [Sistema de estilos CSS](#sistema-de-estilos-css)
- [Vistas previas](#vistas-previas)
- [Diagramas C4](#diagramas-c4)
- [Requisitos](#requisitos)
- [Cómo ejecutar el proyecto](#cómo-ejecutar-el-proyecto)
- [Cláusula de uso de inteligencia artificial](#cláusula-de-uso-de-inteligencia-artificial)

---

## Descripción general

CitasApp es una aplicación web construida con ASP.NET Core MVC organizada en tres proyectos independientes siguiendo el patrón de arquitectura hexagonal (también conocido como Ports & Adapters). La persistencia de datos se maneja a través de archivos JSON locales, lo que hace que el sistema sea sencillo de desplegar y de mantener.

La aplicación cubre tres módulos principales: gestión de pacientes, directorio de médicos y una agenda de citas que relaciona ambos.

---

## Arquitectura hexagonal

La arquitectura hexagonal separa la lógica de negocio del mundo exterior mediante **puertos** (interfaces) y **adaptadores** (implementaciones concretas). Esto permite que el núcleo de la aplicación sea completamente independiente de la tecnología de persistencia o de presentación utilizada.

```
┌─────────────────────────────────────────────────────┐
│                   CitasApp.Web                      │
│         (Adaptador de entrada — HTTP/MVC)           │
│   Controllers · Views · wwwroot · Program.cs        │
└───────────────────────┬─────────────────────────────┘
                        │ usa interfaces de
                        ▼
┌─────────────────────────────────────────────────────┐
│                  CitasApp.Domain                    │
│            (Núcleo — lógica de negocio)             │
│        Interfaces (puertos) · Models · DTOs         │
└───────────────────────┬─────────────────────────────┘
                        │ implementado por
                        ▼
┌─────────────────────────────────────────────────────┐
│              CitasApp.Infraestructure               │
│       (Adaptador de salida — persistencia JSON)     │
│   JsonCitaRepository · JsonMedicoRepository         │
│   JsonPacienteRepository                           │
└─────────────────────────────────────────────────────┘
```

### Responsabilidad de cada proyecto

| Proyecto | Capa | Responsabilidad |
|---|---|---|
| `CitasApp.Domain` | Núcleo | Define los modelos y las interfaces (puertos). No depende de ningún otro proyecto. |
| `CitasApp.Infraestructure` | Adaptador de salida | Implementa las interfaces del dominio usando archivos JSON como mecanismo de persistencia. |
| `CitasApp.Web` | Adaptador de entrada | Expone la aplicación vía HTTP. Recibe peticiones, llama al dominio y devuelve vistas Razor. |

### Flujo de dependencias

Las dependencias siempre apuntan **hacia el núcleo**, nunca al revés:

- `CitasApp.Web` → `CitasApp.Domain`
- `CitasApp.Infraestructure` → `CitasApp.Domain`
- `CitasApp.Domain` → *(sin dependencias externas)*

Esto garantiza que el dominio pueda probarse de forma aislada y que la infraestructura pueda reemplazarse (por ejemplo, cambiar JSON por una base de datos SQL) sin tocar ni el dominio ni la capa web.

---

## Estructura del proyecto

```
CitasApp/
│
├── CitasApp.Domain/                    # Núcleo de la aplicación
│   ├── Interfaces/
│   │   ├── IPacienteRepository.cs      # Puerto de salida para pacientes
│   │   ├── IMedicoRepository.cs        # Puerto de salida para médicos
│   │   ├── ICitaRepository.cs          # Puerto de salida para citas
│   │   └── ICitaObserver.cs            # Puerto para observadores de confirmación de citas
│   └── Models/
│       ├── Paciente.cs                 # Id, Nombre, Apellido, Email, Telefono
│       ├── Medico.cs                   # Id, Nombre, Apellido, Especialidad, NumeroLicencia
│       ├── Cita.cs                     # Id, PacienteId, MedicoId, Fecha, Hora, Motivo, Estado
│       └── CitaJson.cs                 # Modelo auxiliar para deserialización JSON
│
├── CitasApp.Infraestructure/           # Adaptadores de salida (persistencia)
│   ├── Repositories/
│   │   ├── JsonPacienteRepository.cs   # Implementa IPacienteRepository — lee pacientes.json
│   │   ├── JsonMedicoRepository.cs     # Implementa IMedicoRepository — lee medicos.json
│   │   ├── JsonCitaRepository.cs       # Implementa ICitaRepository — lee citas.json
│   │   ├── MemoriaPacienteRepository.cs# Implementación en memoria (usada en Production)
│   │   ├── RepositoryFactory.cs        # Factory — decide qué repositorio de paciente crear
│   │   └── LoggingPacienteRepository.cs# Decorator — agrega logging a un IPacienteRepository
│   └── Observers/
│       ├── SmsObserver.cs              # Observer — simula notificación por SMS
│       └── EmailObserver.cs            # Observer — simula notificación por correo
│
└── CitasApp.Web/                       # Adaptador de entrada (HTTP/MVC)
    ├── Controllers/
    │   ├── HomeController.cs           # Inicio y política de privacidad
    │   ├── PacienteController.cs       # Listado y detalle de pacientes
    │   ├── MedicoController.cs         # Listado y detalle de médicos
    │   └── CitaController.cs           # Agenda general y filtro por paciente
    │
    ├── Data/
    │   ├── pacientes.json
    │   ├── medicos.json
    │   └── citas.json
    │
    ├── Views/
    │   ├── Shared/
    │   │   ├── _Layout.cshtml          # Plantilla base (navbar + footer)
    │   │   └── _Layout.cshtml.css
    │   ├── Home/
    │   │   ├── Index.cshtml            # Dashboard con accesos rápidos
    │   │   └── Privacy.cshtml
    │   ├── Paciente/
    │   │   ├── Index.cshtml            # Tabla de pacientes
    │   │   └── Detalle.cshtml          # Ficha de un paciente
    │   ├── Medico/
    │   │   ├── Index.cshtml            # Tabla de médicos
    │   │   └── Detalle.cshtml          # Ficha de un médico
    │   └── Cita/
    │       ├── Index.cshtml            # Agenda completa de citas
    │       └── PorPaciente.cshtml      # Citas filtradas por paciente
    │
    ├── wwwroot/
    │   ├── css/
    │   │   ├── site.css                # Estilos base de Bootstrap y globales
    │   │   ├── Layout.css              # Navbar, footer, variables de color, animaciones
    │   │   ├── Home.css                # Tarjetas del dashboard y tech grid
    │   │   ├── Medico.css              # Tabla y detalle de médicos
    │   │   ├── Paciente.css            # Tabla y detalle de pacientes
    │   │   └── Cita.css                # Agenda de citas y badge de estado
    │   ├── js/
    │   │   └── site.js
    │   ├── lib/
    │   │   ├── bootstrap/
    │   │   └── jquery/
    │   └── assets/
    │       ├── home.jpeg
    │       ├── pacientes.jpeg
    │       ├── medicos.jpeg
    │       ├── citas-por-paciente.jpeg
    │       └── citas.jpeg
    │
    ├── Program.cs
    ├── CitasApp.Web.csproj
    └── appsettings.json
```

---

## Patrones de diseño (GOF)

### Factory — `RepositoryFactory`
`CitasApp.Infraestructure/Repositories/RepositoryFactory.cs`

Decide en tiempo de ejecución qué implementación de IPacienteRepository
crear, según el entorno (`ASPNETCORE_ENVIRONMENT`):

| Entorno | Repositorio creado |
|---|---|
| `Development` | `JsonPacienteRepository` |
| `Production` | `MemoriaPacienteRepository` |


### Decorator — `LoggingPacienteRepository`
`CitasApp.Infraestructure/Repositories/LoggingPacienteRepository.cs`

Envuelve un `IPacienteRepository` existente (típicamente el que devuelve la
Factory) y agrega logging por consola antes y después de `ObtenerTodos()` y
`ObtenerPorId(id)`, sin alterar la lógica de lectura original:

```csharp
public class LoggingPacienteRepository : IPacienteRepository
{
    private readonly IPacienteRepository _inner;

    public LoggingPacienteRepository(IPacienteRepository inner) => _inner = inner;

    public List<Paciente> ObtenerTodos()
    {
        Console.WriteLine("[...] ObtenerTodos — inicio");
        var resultado = _inner.ObtenerTodos();
        Console.WriteLine($"[...] ObtenerTodos — {resultado.Count} registros");
        return resultado;
    }
    // ObtenerPorId sigue el mismo patrón
}
```

### Observer — `ICitaObserver`

CitasApp.Domain/Interfaces/ICitaObserver.cs

CitasApp.Infraestructure/Observers/SmsObserver.cs

CitasApp.Infraestructure/Observers/EmailObserver.cs

---

## Funcionamiento básico

### Inicio

La página principal muestra cuatro accesos rápidos a los módulos disponibles: Pacientes, Médicos, Agenda y Privacidad. También incluye una descripción del sistema y un resumen de las tecnologías utilizadas.

### Pacientes

La ruta `/Paciente` lista todos los pacientes registrados en `pacientes.json`. Cada fila de la tabla tiene un enlace a la ficha individual del paciente, donde se muestran sus datos de contacto y un botón para ver sus citas asociadas.

### Médicos

La ruta `/Medico` muestra el directorio completo de médicos con nombre, apellido y especialidad. Al acceder al detalle de un médico, se agrega su número de licencia profesional.

### Agenda de citas

La ruta `/Cita` presenta todas las citas registradas en `citas.json`. Cada registro incluye fecha, hora, paciente, médico, motivo de consulta y estado. El estado por defecto es `Pendiente`.

Desde la ficha de un paciente también se puede acceder a `/Cita/PorPaciente/{id}`, que filtra únicamente las citas de ese paciente.

### Flujo de datos

Cada controlador recibe su repositorio correspondiente por inyección de dependencias, según lo configurado en `Program.cs`. Los repositorios (definidos en `CitasApp.Infraestructure`) implementan las interfaces del dominio (`CitasApp.Domain`) y leen los archivos JSON de la carpeta `Data/`. No hay escritura en ningún momento: la aplicación es solo de lectura.

---

## Sistema de estilos CSS

Los estilos están divididos por vista para mantener el código ordenado. Cada archivo CSS carga únicamente en la vista que lo requiere, usando `@section Styles` en Razor.

### Layout.css

Archivo principal. Define las variables de color en `:root`, la tipografía (Nunito, cargada desde Google Fonts), el navbar fijo con indicador de enlace activo, el footer y la animación de entrada `fade-up`. Todos los demás archivos CSS heredan estas variables.

Variables principales:

| Variable | Valor | Uso |
|---|---|---|
| `--blue` | `#2b7fdb` | Color principal de acción |
| `--blue-dark` | `#1a5fb4` | Hover de botones y encabezados |
| `--blue-light` | `#e8f2fd` | Fondos de cabeceras y hover de filas |
| `--blue-mid` | `#bbdaf7` | Bordes y separadores |
| `--bg` | `#f5f9ff` | Fondo general de la página |
| `--text` | `#1e2a38` | Texto principal |
| `--text-muted` | `#6c7a8d` | Texto secundario |

### Home.css

Estilos para las tarjetas de acceso rápido (`stat-card`), las tarjetas informativas (`info-card`) con cabecera azul claro, y los badges de tecnología con colores diferenciados por tipo.

### Medico.css y Paciente.css

Ambos archivos comparten una estructura similar: tabla sin bordes visibles, cabecera con fondo azul claro, filas con hover sutil, y links de "Ver detalle" convertidos en pastillas con transición de color. En `Paciente.css` el botón "Ver sus citas" tiene un estilo más destacado (fondo azul sólido con efecto al hover).

### Cita.css

Similar a los anteriores en la tabla. La columna de estado se muestra como un badge con fondo azul claro para diferenciarse del texto normal.

---

## Vistas previas

### Inicio / Dashboard

![Vista de inicio](wwwroot/assets/home.jpeg)

### Pacientes

![Listado de pacientes](wwwroot/assets/pacientes.jpeg)

### Médicos

![Listado de médicos](wwwroot/assets/medicos.jpeg)

### Agenda de citas

![Agenda de citas](wwwroot/assets/citas.jpeg)

### Citas por paciente

![Citas filtradas por paciente](wwwroot/assets/citas-por-paciente.jpeg)

---

## Diagramas C4
Para una comprensión detallada de la arquitectura del sistema, puedes ver las vistas C4 en el siguiente enlace, donde se describen los niveles de contexto, contenedores y componentes que conforman nuestra estructura actual.
[Ver diagramas C4](Docs/DiagramaC4-Nivel_1-3.md)

---


## Requisitos

- .NET 10 SDK o superior
- Navegador moderno (Chrome, Firefox, Edge)
- No se requiere instalar ni configurar ninguna base de datos

---

## Cómo ejecutar el proyecto

1. Clona o descarga el repositorio.

2. Abre una terminal en la carpeta raíz de la solución (donde está el archivo `.slnx`).

3. Restaura las dependencias y ejecuta el proyecto web:

```bash
dotnet run --project CitasApp.Web
```

4. Abre el navegador en la dirección que aparezca en la terminal, normalmente `https://localhost:5001` o `http://localhost:5000`.

5. La aplicación carga los datos desde los archivos en `CitasApp.Web/Data/`. Si quieres agregar o modificar registros, edita directamente los archivos `pacientes.json`, `medicos.json` o `citas.json` y reinicia el servidor.

---

## Cláusula de uso de inteligencia artificial

Una parte del código de este proyecto fue generada con el apoyo de herramientas de inteligencia artificial, específicamente en lo relativo al diseño visual y los archivos CSS.

Los archivos afectados son:

- `CitasApp.Web/wwwroot/css/Layout.css`
- `CitasApp.Web/wwwroot/css/Home.css`
- `CitasApp.Web/wwwroot/css/Medico.css`
- `CitasApp.Web/wwwroot/css/Paciente.css`
- `CitasApp.Web/wwwroot/css/Cita.css`

Estos archivos fueron generados con asistencia de Claude (Anthropic) a partir de las vistas Razor existentes y una referencia visual de estilo. El resultado fue revisado y aceptado como parte del proyecto.

El resto del proyecto —controladores, modelos, repositorios, interfaces, vistas y configuración— fue desarrollado de forma manual por mi.

El uso de IA en este contexto tuvo como objetivo agilizar la parte de estilización, que no es el foco principal de la materia, permitiendo dedicar más tiempo al diseño arquitectónico y la lógica de la aplicación.

---

<div align="center">

**⭐ Si te gustó este proyecto, dale una estrella ⭐**

Hecho con 💙 por Ariff Medina — 2026

</div>

*CitasApp — Proyecto académico, Arquitectura de Software, 2026.*
