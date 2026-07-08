# Arquitectura como código de CitasApp — Modelo C4 (Niveles 1 a 3)
---

## Nivel 1 — Contexto

**Para quién es:** cualquier persona (equipo, cliente, profesor). **Qué responde:** qué es el sistema y quién lo usa, sin ningún detalle técnico.

```mermaid
graph TD
    Paciente(["👤 Paciente"])
    Medico(["👤 Médico"])
    Sistema["CitasApp<br/><i>Sistema de gestión de citas médicas</i>"]

    Paciente -->|"consulta y confirma<br/>sus citas"| Sistema
    Medico -->|"consulta pacientes,<br/>médicos y citas"| Sistema
```

---

## Nivel 2 — Contenedores

**Para quién es:** el equipo técnico. **Qué responde:** de qué piezas grandes está hecho el sistema y cómo se comunican.

```mermaid
graph TD
    Cliente["🖥️ Cliente HTTP<br/><i>Postman / futuro frontend</i>"]

    subgraph EC2["Instancia EC2"]
        Api["CitasApp.Api<br/><i>ASP.NET Core Web API (.NET 10)</i><br/>Controllers + Application + Domain + Infraestructure"]
    end

    Datos[("Archivos JSON<br/>pacientes.json / medicos.json / citas.json")]

    Cliente -->|"HTTP/JSON<br/>(CORS: AllowAll)"| Api
    Api -->|"lee (solo lectura)"| Datos
```
---

## Nivel 3 — Componentes (dentro de CitasApp.Api)

**Para quién es:** quien va a modificar el código. **Qué responde:** qué hay dentro de cada capa y dónde viven los patrones GOF (Semana 8).

```mermaid
graph TD
    subgraph API_PROJ["CitasApp.Api — Controllers"]
        CC["CitasController"]
        MC["MedicosController"]
        PC["PacientesController"]
        CalC["CalculadoraController"]
    end

    subgraph APP_PROJ["CitasApp.Application — Services"]
        CS["CitaService"]
        MS["MedicoService"]
        PS["PacienteService"]
        CalS["CalculadoraService"]
    end

    subgraph DOM_PROJ["CitasApp.Domain"]
        ICR["ICitaRepository"]
        IMR["IMedicoRepository"]
        IPR["IPacienteRepository"]
        ICO["ICitaObserver"]
    end

    subgraph INFRA_PROJ["CitasApp.Infraestructure"]
        RF["RepositoryFactory<br/>(Factory)"]
        LPR["LoggingPacienteRepository<br/>(Decorator)"]
        JPR["JsonPacienteRepository"]
        MEM["MemoriaPacienteRepository"]
        JMR["JsonMedicoRepository"]
        JCR["JsonCitaRepository"]
        SMS["SmsObserver<br/>(Observer)"]
        MAIL["EmailObserver<br/>(Observer)"]
    end

    CC --> CS
    MC --> MS
    PC --> PS
    CalC --> CalS

    CS -->|usa| ICR
    CS -->|"notifica al<br/>confirmar cita"| ICO
    MS -->|usa| IMR
    PS -->|usa| IPR

    RF -.->|"crea (Development)"| JPR
    RF -.->|"crea (Production)"| MEM
    LPR -->|envuelve| JPR
    LPR -.implementa.-> IPR
    JMR -.implementa.-> IMR
    JCR -.implementa.-> ICR
    SMS -.implementa.-> ICO
    MAIL -.implementa.-> ICO

    JPR --> PJSON[("pacientes.json")]
    JMR --> MJSON[("medicos.json")]
    JCR --> CJSON[("citas.json")]
```
