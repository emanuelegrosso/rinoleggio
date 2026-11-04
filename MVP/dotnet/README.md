# Rinoleggio MVP — custom .NET + SQLite

Percorso di riferimento (Windows): `C:\EG\Rinoleggio\MVP\dotnet`

Stack MVP **senza ABP/Orchard**: ASP.NET Core 8, Razor Pages, EF Core 8, SQLite file locale, ASP.NET Core Identity.

**Perimetro MVP attuale:** solo **inserimento** e **ricerca** contratti NLT (vedi documentazione progetto `docs/mvp-dotnet-contratti-scope.md` nel Project store).

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

## Funzionalità MVP (inserimento + ricerca)

| Route | Descrizione |
|-------|-------------|
| `/contratti` | Ricerca pubblica con filtri |
| `/contratti/{id}` | Dettaglio contratto |
| `/contratti/inserisci` | Inserimento **senza login** → **Salva e pubblica** |

I vecchi URL `/listings/*` reindirizzano alle pagine sopra.

Lead, backoffice, immagini e pagine editoriali sono **fuori scope** per questo MVP.

## Rapporto con Orchard/ABP

- **Orchard** resta il riferimento funzionale già in corso (RIN-2, RIN-4).
- **ABP** resta documentato come alternativa in `docs/abp-modello-dati-sql.md`.
- **Questo stack** è la via “custom leggera” per un MVP deployabile su un singolo VPS/Windows con un solo file `.db`.

Documentazione di definizione: [docs/mvp-dotnet-sqlite.md](/cursor/stores/self/docs/mvp-dotnet-sqlite.md).
