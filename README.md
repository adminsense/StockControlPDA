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
  - **8 tabs** (two rows of 4): Users, Warehouses, Locations, Suppliers, Products, Items (SKU), **Min / Max**, Stock Control
  - CRUD + validations (English); **Min / Max** lives on `/minmax` (not inside the Items form)
  - **Stock** (`/` and `/stock`): filters + pagination (10 rows/page); **Sync** checks whether `items` changed since the last baseline — if not, modal *“Everything is synchronized.”*; if yes, a **busy** overlay then reload (balances + Min/Max resolution)
  - **UI**: stronger contrast for inputs, placeholders, and dropdowns for warehouse lighting (`StockControl.Admin.Client` → `admin-theme.css`, `color-scheme: dark`)
- **PDA (Android)**:
  - MAUI app contains **`Platforms/Android` only** (iOS / Windows / Mac Catalyst folders removed)
  - Scan-first flow (keyboard wedge / Enter)
  - Fast stock movements (Inbound/Outbound)
  - Min/Max visibility during operations

## 🧪 4. UI reference

Screenshots should match the **current** UI when updated in-repo.

### 4.1 Admin — tabs overview

<p align="center">
  <img src="./readme/images/mock_admin_template.png" alt="Stock Control — Admin tabs (desktop)" />
</p>

### 4.2 Admin — standard master forms

Search → form → grid pattern shared by Users, Warehouses, Locations, Suppliers, Products, Items, and Min / Max.

<p align="center">
  <img src="./readme/images/forms_screen.png" alt="Admin — standard forms (search, form, list)" />
</p>

### 4.3 PDA — operation (Android)

<p align="center">
  <img src="./readme/images/mock_template.png" alt="PDA — stock movement mock" />
</p>

Details: [README-operation-stock-pda](readme/README-operation-stock-pda.md) · Admin UX: [README-admin-stock](readme/README-admin-stock.md)

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

---

**© 2026 AdminSense. All rights reserved.**
