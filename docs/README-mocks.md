# UI mocks (match dev apps)

Open these HTML files in a browser to preview login and main flows **as implemented today** (not future prototypes).

| Mock | File | Dev app |
|------|------|---------|
| **Admin** (login + menu + tabs) | [stock-control-admin-mock.html](stock-control-admin-mock.html) | `StockControl.Admin` — Blazor Server |
| **PDA** (login + move stock) | [stock-control-pda-mock.html](stock-control-pda-mock.html) | `StockControl.PDA` — MAUI Android |

## Test users (SQL seed)

| Username | Password | Role | App |
|----------|----------|------|-----|
| `admin` | `Pda2!Stock` | 1 — Admin web | Admin browser |
| `pda` | `Pda2!Stock` | 2 — Admin PDA | PDA app |

See [LOGIN-TEST-USERS.md](../readme/LOGIN-TEST-USERS.md).

## Admin mock flow

1. **Sign in** — modal like `LoginModal.razor`; eye toggle; validates `admin` / `Pda2!Stock`.
2. **Menu** — hero **Stock Control** on top; grid: Users … Min/Max, **Audit Logs** bottom-right.
3. **Tabs** — Stock (default), Users, Audit Logs, placeholder for other master-data routes.
4. **Sign out** — returns to login.

Real auth: `loginAdmin` JS → `POST /api/auth/login?app=admin` → HttpOnly JWT cookie.

## PDA mock flow

1. **Sign in** — card like `LoginPage.xaml`; `pda` / `Pda2!Stock` (rejects `admin`).
2. **Move stock** — layout from `MainPage.xaml`: warehouse, location + item pickers, scan field, summary, qty, Inbound/Outbound, Sync, Reset.
3. **Sign out** — back to login.

Real auth: `POST /api/auth/login?app=pda` → Bearer token on `HttpClient`.

## Older / partial mocks

- [stock-control-admin--details-mock.html](stock-control-admin--details-mock.html) — static Users grid (no login).
- [stock-control-admin--audit-mock.html](stock-control-admin--audit-mock.html) — static Audit only.
- [pda-move-stock.html](pda-move-stock.html) — **deprecated** scan-first prototype; use **stock-control-pda-mock.html** instead.
