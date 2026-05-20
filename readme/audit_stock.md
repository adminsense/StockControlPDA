# Audit — Stock Control (proposal)

![Status](https://img.shields.io/badge/Status-Proposal-6c757d?style=flat-square) ![Scope](https://img.shields.io/badge/Scope-Admin%20%2B%20PDA-0d6efd?style=flat-square)

Short proposal for an audit trail in **Stock Control**. Inspired by [Immo `AUDIT.md`](./AUDIT.md), but scaled down: ~3 users, 8 Admin tabs, PDA stock movements — no workflows, reports engine, or 90+ action types.

---

## 1. Why (lightweight)

| Need | Fit for this app |
|------|------------------|
| Who changed master data? | Users, items, locations, min/max |
| Who moved stock on the PDA? | Already have `stock_movements.UserId` — audit adds **context** (IP, success/fail, before qty) |
| Login / failed login | Security with JWT (role 1 / 2) |
| Accountability | Enough for internal warehouse use, not full GDPR/SOX program |

**Not aiming for:** Immo-level grids, workflow actions, export audits, file manager, field-level JSON on every screen from day one.

---

## 2. Suggested scope

### Audit (yes)

| Area | Actions | Entity name (examples) |
|------|---------|------------------------|
| **Auth** | `Login`, `LoginFailed`, `Logout` | `Auth` — Admin web + PDA API (`app=pda`) |
| **Admin CRUD** | `Create`, `Update`, `Activate`, `Deactivate` | `User`, `Warehouse`, `Location`, `Supplier`, `Product`, `Item`, `MinMax` |
| **PDA API** | `StockIn`, `StockOut` (or `Create` + direction in `NewValues`) | `StockMovement` |
| **Admin Stock tab** | `Sync` (optional) | `Stock` — catalog refresh only, not every grid read |

### Skip or later

- Read-only page views, search, pagination
- Full `stock_balances` snapshot on every movement (optional `QuantityBefore` / `QuantityAfter` in one row is enough)
- Duplicate logging in Blazor page **and** EF — **one layer** (service called after `SaveChangesAsync`), same rule as Immo
- Passwords, JWT, API keys in `OldValues` / `NewValues`

---

## 3. Data model (minimal)

Single table, e.g. `audit_logs`:

| Column | Purpose |
|--------|---------|
| `Id` | PK |
| `Timestamp` | UTC |
| `Action` | Short code (see §4) |
| `EntityName` | `Item`, `StockMovement`, … |
| `EntityId` | Record id (nullable for failed login) |
| `UserId` | From JWT / signed-in user |
| `Username` | Denormalized for grid |
| `Success` | bit |
| `Severity` | `Information` / `Warning` / `Error` |
| `IpAddress` | From `HttpContext` (Admin + API) |
| `OldValues` / `NewValues` | JSON, **optional in v1** — start with `NewValues` only for updates |
| `ErrorMessage` | Failed ops |

Indexes: `(Timestamp DESC)`, `(UserId, Timestamp)`, `(EntityName, EntityId)`.

**Retention (simple):** 90 days all levels; manual purge job or SQL script — no archive service required initially.

---

## 4. Action list (complete for v1)

| Category | Actions |
|----------|---------|
| Authentication | `Login`, `LoginFailed`, `Logout` |
| Master data | `Create`, `Update`, `Activate`, `Deactivate` |
| Stock (PDA) | `StockIn`, `StockOut` |
| Stock (Admin) | `Sync` (optional) |

~**12 action types** vs 90+ in Immo.

---

## 5. Where to hook code

| Location | What to log |
|----------|-------------|
| `Auth/AdminSignInService.cs` | Login success / Logout |
| `Api/AuthApi.cs` | Login success / `LoginFailed` (PDA + API admin) |
| Each Admin page `SaveAsync` / `ToggleActiveAsync` | CUD + activate/deactivate (shared helper) |
| `Api/StockMovementApi.cs` | After successful commit: `StockIn` / `StockOut` with location, item, qty, resulting balance |
| `Pages/Stock.razor` `OnSyncClickedAsync` | `Sync` if catalog changed (optional) |

Suggested helper: `IAuditService.LogAsync(AuditEntry entry)` in `StockControl.Admin`, injected into pages and minimal API wrappers.

---

## 6. UI (Admin only)

**Interactive mocks:** [`docs/stock-control-admin-mock.html`](../docs/stock-control-admin-mock.html) (login + full menu + Audit tab) · [`docs/README-mocks.md`](../docs/README-mocks.md). Standalone audit-only: [`docs/stock-control-admin--audit-mock.html`](../docs/stock-control-admin--audit-mock.html).

**Route:** `/audit` (or **Audit** tab) — **Role 1 (Admin)** only.

**Grid (keep small):**

- Filters: Date from/to, Action, User, Success (no entity filter)
- Columns: Timestamp, Action, Entity (name only), User, IP, Status, View — record id stays in DB / detail modal JSON, not in the grid
- Pagination: 25–50 rows
- **Detail modal:** timestamp, user, IP, JSON diff (when present), error text

No badge legend modal, no 8 filter categories, no export-to-PDF audit reports in v1.

---

## 7. Phased delivery

| Phase | Deliverable |
|-------|-------------|
| **MVP** | Table + `IAuditService` + Login/Logout/LoginFailed + `StockIn`/`StockOut` on API |
| **1b** | Admin master-data Create/Update/Activate/Deactivate on all 7 entity tabs |
| **2** | Admin `/audit` grid + detail modal |
| **3** (optional) | `OldValues` on Update; `Sync` on Stock tab; retention script |

---

## 8. PDA note

Movements are already tied to `users.Id` via JWT. Audit rows should **duplicate** that link plus human-readable fields (SKU, location code, direction) in `NewValues` so operators do not need to join tables when reviewing logs.

---

## 9. Out of scope (from Immo — not needed here)

- Workflow actions (`WKFLOW*`), contract status, payment schedules  
- Report/export audits (`ReportExport`, CSV/PDF)  
- File manager, Word viewer, 2FA enable/disable  
- 30+ page adapter matrix — **one** audit service + page save hooks is enough  
- Critical severity / automated “system” jobs (unless you add scheduled sync later)

---

## 10. Reference

- Immo full spec: [`readme/AUDIT.md`](./AUDIT.md)  
- Stock auth & roles: [`readme/README-admin-stock.md`](./README-admin-stock.md) — section **Authentication (JWT — Admin + PDA)**

---

**© 2026 AdminSense. All rights reserved.**
