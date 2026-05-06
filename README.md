<p align="center">
  <img src="./readme/images/logo-asense.png" alt="AdminSense" width="100">
</p>

# Stock Control — Admin + PDA

![Status](https://img.shields.io/badge/Status-In%20development-0dcaf0?style=flat-square) ![Apps](https://img.shields.io/badge/Apps-Admin%20(Desktop)%20%2B%20PDA%20(Android)-512BD4?style=flat-square) ![Copyright](https://img.shields.io/badge/Copyright-2026-blue?style=flat-square)

Two-app solution for stock control:

- **Admin (desktop)**: Blazor Server app to manage master data and view stock balances (Min/Max included).
- **PDA (Android)**: MAUI app for scan-driven warehouse operations on handheld devices.

---

## 📋 1. Project overview

This repository contains:

- **Admin (Blazor Server)**: compact master-data registration + stock visibility.
- **Operation (PDA, MAUI Android)**: scan-first workflow for stock movements.

## 🎯 2. Objective

- Maintain master data (Users, Warehouses, Locations, Products, Items + Barcodes).
- Execute warehouse operations on PDAs using a repeatable **Location → Item → Quantity → In/Out** flow.
- Provide clear **Min/Max** visibility and stock status (below/above).

## ✨ 3. Key features (summary)

- **Admin (desktop)**:
  - 6-tab navigation: Users, Warehouses, Locations, Products, Items (SKU), Stock
  - CRUD + validations (English)
  - Min/Max inside Items (warehouse default + optional location override, \(Max \ge Min\))
  - Stock page with filters + pagination (10 rows/page)
- **PDA (Android)**:
  - Scan-first flow (keyboard wedge / Enter)
  - Fast stock movements (Inbound/Outbound)
  - Min/Max visibility during operations

## 🧪 4. UI reference (mock)

Approved reference mock:

### 4.1 Admin (desktop)

<p align="center">
  <img src="./readme/images/mock_admin_template.png" alt="Admin prototype" />
</p>

### 4.2 PDA (Android)

<p align="center">
  <img src="./readme/images/mock_template.png" alt="PDA prototype" />
</p>

## 🛠️ 5. Tech stack

- **Admin**: Blazor Server + EF Core + SQL Server
- **PDA**: .NET MAUI (Android)

## 📚 7. Documentation

- 🏠 **Main documentation**: this file
- 📋 **Admin documentation**: [README-admin-stock](readme/README-admin-stock.md)
- 📋 **PDA documentation**: [README-operation-stock-pda](readme/README-operation-stock-pda.md)

---

**© 2026 AdminSense. All rights reserved.**
