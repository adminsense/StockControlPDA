<p align="center">
  <img src="./readme/images/logo-asense.png" alt="AdminSense" width="100">
</p>

# Stock Control — Admin (Desktop)

![Status](https://img.shields.io/badge/Status-Implemented-198754?style=flat-square) ![Product](https://img.shields.io/badge/Product-Blazor%20Server%20Desktop%20Admin-0d6efd?style=flat-square) ![Copyright](https://img.shields.io/badge/Copyright-2026-blue?style=flat-square)

Desktop **Admin** application implemented in **Blazor Server** (Visual Studio 2022 / IIS Express) with **SQL Server + EF Core**. The UI follows the approved mock and provides **CRUD** for master data plus a **Stock** screen (warehouse/location balances) with Min/Max comparison.

---

## 1. Project overview

This repository currently contains **one desktop Admin app**:

- **Admin (Blazor Server)**: compact master-data registration + stock visibility.

## 2. Objective

- Maintain master data (Users, Warehouses, Locations, Products, Items + Barcodes).
- Configure **Min/Max** rules (inside Items).
- View on-hand quantities by **Warehouse + Location + Item**.

## 3. Key features (summary)

- **6-tab navigation**: Users, Warehouses, Locations, Products, Items (SKU), Stock.
- **CRUD + validations** on all forms (English messages).
- **Min/Max inside Items** (warehouse default + optional location override, \(Max \ge Min\)).
- **Stock page** with filters (warehouse, location, search, below-min/above-max) and pagination (10 rows/page).

## 4. UI reference (mock)

Approved reference mock:

<p align="center">
  <img src="./readme/images/mock_admin_template.png" alt="Admin prototype" />
</p>

## 5. Tech stack

- **Frontend**: Blazor Server
- **Database**: SQL Server
- **ORM**: Entity Framework Core (migrations)

## 7. Documentation

- 🏠 **Main documentation**: this file
- 📋 **Admin documentation**: `readme/README-admin-stock.md`
- 📋 **PDA documentation**: `readme/README-operation-stock-pda.md`

---

**© 2026 AdminSense. All rights reserved.**
