# Rinoleggio MVP — custom .NET + SQLite

Percorso di riferimento (Windows): `C:\EG\Rinoleggio\MVP\dotnet`

Stack MVP **senza ABP/Orchard**: ASP.NET Core 8, Razor Pages, EF Core 8, SQLite file locale, ASP.NET Core Identity (ruoli `Administrator`, `BackOffice`, `Authenticated`).

## Soluzione

| Progetto | Ruolo |
|----------|--------|
| `Rinoleggio.Domain` | Entità ed enum allineati a [ContentTypes.md v4](../ContentTypes.md) e al [modello SQL](/cursor/stores/self/docs/abp-modello-dati-sql.md) |
| `Rinoleggio.Infrastructure` | `RinoleggioDbContext`, migrazioni EF, seed anagrafiche MVP |
| `Rinoleggio.Web` | Host web, Identity, Razor Pages |

Database: `src/Rinoleggio.Web/App_Data/rinoleggio.db` (gitignored).

## Prerequisiti

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Opzionale) `dotnet tool install -g dotnet-ef`

## Comandi

```bash
cd MVP/dotnet
dotnet restore
dotnet build
dotnet run --project src/Rinoleggio.Web
```

Apri `http://localhost:5000` (o la porta indicata in console).

### Migrazioni

```bash
dotnet ef migrations add <Nome> \
  --project src/Rinoleggio.Infrastructure \
  --startup-project src/Rinoleggio.Web \
  --output-dir Data/Migrations

dotnet ef database update \
  --project src/Rinoleggio.Infrastructure \
  --startup-project src/Rinoleggio.Web
```

## Roadmap funzionale (MVP)

1. **Fase 0** — schema DB, Identity, seed province/marche/società NLT, homepage placeholder.
2. **Fase 1** — Registrazione + `UserProfile`, login, area account (`/account`, `/account/profile`).
3. **Fase 2** — CRUD annuncio (aggregate `Listing` + part 1:1), upload immagini su disco (`wwwroot/uploads` o path configurabile).
4. **Fase 3** — Catalogo pubblico con filtri (provincia, marca, canone, km residui), dettaglio annuncio.
5. **Fase 4** — Lead di subentro + policy (un lead aperto per buyer/listing).
6. **Fase 5** — Backoffice (`BackOffice` policy): moderazione stati, gestione lead, anagrafiche.
7. **Fase 6** — Pagine editoriali (`SitePage`) e menu statico da config.

Regole business (validazioni contratto, `RemainingKm`, stati `Deleted`/`StatusReason`, permessi) vanno implementate nei servizi applicativi prima della pubblicazione.

## Rapporto con Orchard/ABP

- **Orchard** resta il riferimento funzionale già in corso (RIN-2, RIN-4).
- **ABP** resta documentato come alternativa in `docs/abp-modello-dati-sql.md`.
- **Questo stack** è la via “custom leggera” per un MVP deployabile su un singolo VPS/Windows con un solo file `.db`.

Documentazione di definizione: [docs/mvp-dotnet-sqlite.md](/cursor/stores/self/docs/mvp-dotnet-sqlite.md).
