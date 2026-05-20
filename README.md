<p align="center">
  <img src="./readme/images/logo-asense.png" alt="AdminSense" width="100">
</p>

# Stock Control — Admin + PDA

![Status](https://img.shields.io/badge/Status-In%20development-0dcaf0?style=flat-square) ![Apps](https://img.shields.io/badge/Apps-Admin%20(Desktop)%20%2B%20PDA%20(Android)-512BD4?style=flat-square) ![Copyright](https://img.shields.io/badge/Copyright-2026-blue?style=flat-square)

Two-app solution for stock control:

- **Admin (desktop)**: Blazor Server app to manage master data, Min/Max targets, and stock balances.
- **PDA (Android)**: MAUI app for scan-driven warehouse operations on handheld devices.

---

## 📋 1. Project overview

This repository contains:

- **Admin (Blazor Server)**: compact master-data registration + stock visibility.
- **Operation (PDA, MAUI Android)**: scan-first workflow for stock movements.

## 🎯 2. Objective

- Maintain master data (Users, Warehouses, Locations, Suppliers, Products, Items + barcodes: **SKU** internal code and **article number** supplier/catalog code).
- Configure **Min/Max** per warehouse (and optional location override) on its own Admin screen.
- Execute warehouse operations on PDAs using a repeatable **Location → Item → Quantity → In/Out** flow.
- Provide clear **Min/Max** visibility and stock status (below/above).

## ✨ 3. Key features (summary)

- **Admin (desktop)**:
  - **Sign in** modal (JWT cookie); test user **admin** — see [Login (test users)](readme/LOGIN-TEST-USERS.md)
  - **Menu** (4 columns): **Stock Control** in the header row above **Locations**; eight tabs in two rows (Users, Warehouses, Locations, Suppliers, Products, Items (SKU), **Min / Max**, Audit Logs)
  - CRUD + validations (English); **Min / Max** lives on `/minmax` (not inside the Items form)
  - **Stock** (`/` and `/stock`): **Warehouse**, **Location**, **Supplier**, **Stock status** (all / below min / above max), full-width **Search** + **Sync**; grid columns include **Supplier**, **Reorder**, **Status**; pagination (10 rows/page); **Sync** checks whether `items` changed since the last baseline — if not, modal *“Everything is synchronized.”*; if yes, a **busy** overlay then reload (balances + Min/Max resolution)
  - **UI**: stronger contrast for inputs, placeholders, and dropdowns for warehouse lighting (`StockControl.Admin.Client` → `admin-theme.css`, `color-scheme: dark`)
- **PDA (Android)**:
  - MAUI app contains **`Platforms/Android` only** (iOS / Windows / Mac Catalyst folders removed)
  - **Sign in** screen (JWT Bearer); test user **pda** — see [Login (test users)](readme/LOGIN-TEST-USERS.md)
  - Scan-first flow (keyboard wedge / Enter); **Sync** \| **Reset** on **Move stock**; `GET /api/stock/sync` (MVP: server counts)
  - Fast stock movements (Inbound/Outbound); **Move stock** UI aligned with the HTML mock (pickers + Summary)

## 🧪 4. UI reference

Screenshots in `readme/images/` match the **current** UI.

### 4.1 Sign in (Admin + PDA)

White card on dark background; underline fields; purple **Sign in** button. Same pattern in both apps.

<p align="center">
  <img src="./readme/images/mock_login.png" alt="Stock Control — Sign in (Admin and PDA)" />
</p>

| App | Username | Password | Role |
|-----|----------|----------|------|
| **Admin** (browser) | `admin` | `Pda2!Stock` | 1 |
| **PDA** (Android) | `pda` | `Pda2!Stock` | 2 |

Seed script: [`scripts/seed-user-passwords.sql`](scripts/seed-user-passwords.sql). Details: [readme/LOGIN-TEST-USERS.md](readme/LOGIN-TEST-USERS.md).

### 4.2 Admin — Stock Control (home)

<p align="center">
  <img src="./readme/images/mock_admin_template.png" alt="Stock Control — Admin: Stock tab with filters and list" />
</p>

*Header row: title, **Stock Control** (same width as **Locations** below), user + **Sign out**. Two tab rows: Users, Warehouses, Locations, Suppliers, Products, Items (SKU), Min / Max, Audit Logs.*

### 4.3 Admin — standard master forms

Search → form → grid pattern shared by Users, Warehouses, Locations, Suppliers, Products, Items, and Min / Max.

<p align="center">
  <img src="./readme/images/forms_screen.png" alt="Admin — standard forms (search, form, list)" />
</p>

### 4.4 Admin — Audit Logs

<p align="center">
  <img src="./readme/images/mock_audit.png" alt="Stock Control — Admin: Audit Logs" />
</p>

Read-only grid for admins (role **1**). Proposal notes: [readme/audit_stock.md](readme/audit_stock.md).

### 4.5 PDA — Move stock

<p align="center">
  <img src="./readme/images/pda-move-stock.png" alt="PDA — Move stock" />
</p>

## 🛠️ 5. Tech stack

- **Admin**: Blazor Server + EF Core + SQL Server
- **PDA**: .NET MAUI (Android)

## 📁 6. Repository layout

- `src/StockControl.Admin/` — Blazor Server Admin
- `src/StockControl.Admin.Client/` — Razor Class Library (shared UI/CSS with Admin)
- `src/StockControl.PDA/` — .NET MAUI app, **`net10.0-android` only**; **`Platforms/Android`** only (no iOS / Windows / Mac Catalyst in repo)

## 📚 7. Documentation

- 🏠 **Main documentation**: this file
- 📋 **Admin documentation**: [README-admin-stock](readme/README-admin-stock.md)
- 📋 **PDA documentation**: [README-operation-stock-pda](readme/README-operation-stock-pda.md)
- 📋 **PDA+Admin Installation**: [README-StockControlInstall](readme/README-StockControlInstall.md)

### Interactive UI mocks (HTML)

Open in a browser for client demos. Admin mocks navigate between each other: **Users** → details, **Stock Control** → main, **Audit Logs** → audit mock.

| App | Mock | Notes |
|-----|------|--------|
| **Admin — Stock (home)** | [`docs/stock-control-admin-mock.html`](docs/stock-control-admin-mock.html) | Start here. Matches `readme/images/mock_admin_template.png`. |
| **Admin — master tabs (details)** | [`docs/stock-control-admin--details-mock.html`](docs/stock-control-admin--details-mock.html) | Users, Warehouses, CRUD forms, etc. |
| **Admin — Audit Logs (proposal)** | [`docs/stock-control-admin--audit-mock.html`](docs/stock-control-admin--audit-mock.html) | Read-only audit grid + detail modal. See [audit_stock.md](readme/audit_stock.md). |
| **PDA — Move stock** | [`docs/pda-move-stock.html`](docs/pda-move-stock.html) | Same screen: scan product → location dropdown → quantity (Add/Subtract). See [README-operation-stock-pda](readme/README-operation-stock-pda.md) §4. |

---

**© 2026 AdminSense. All rights reserved.**
