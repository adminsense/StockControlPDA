<p align="center">
  <img src="./readme/images/logo-asense.png" alt="AdminSense" width="100">
</p>

# <span style="color: #0066cc">Stock Control — PDA</span>

![Status](https://img.shields.io/badge/Status-Proposal%20%2B%20Mock-0dcaf0?style=flat-square) ![Product](https://img.shields.io/badge/Product-.NET%20MAUI%20apps%20%28Admin%20%2B%20PDA%29-512BD4?style=flat-square) ![Copyright](https://img.shields.io/badge/Copyright-2026-blue?style=flat-square)

Client-facing proposal and **static UI mock** for a **stock keeping** solution aimed at **warehouse PDAs**: scan-driven workflow, **minimum / maximum** stock visibility, and **mobile-first** ergonomics.

---

## 📋 1. Project overview

This solution is split into **two .NET MAUI apps** backed by an **API + database**:

- **Admin app** (Android + Windows desktop): compact master-data registration.
- **Operation app** (Android PDA): scan-first stock movements on handheld devices.

## 🎯 2. Objective

- Enable fast warehouse movements using a repeatable **Location → Item → Quantity → In/Out** flow.
- Provide clear **min/max** visibility and simple alerts.

## ✨ 3. Key features (summary)

- **Scan-driven workflow** (keyboard wedge; scanners typically send text + **Enter**).
- **Mobile-first ergonomics** for PDA screens (big touch targets, minimal typing).
- **Min/Max visibility** (below-min / above-max highlighting).
- **Audit-ready movements** (who/when/where/what/qty).

## 🧩 4. Apps (prototypes)

### 4.1 Admin app (MAUI — Android + Windows desktop)

Compact master-data registration (users, products, items/SKUs, warehouses, locations, min/max).

<p align="center">
  <img src="./readme/images/mock_admin_template.png" alt="Admin prototype" />
</p>

### 4.2 Operation app (Stock Control PDA — MAUI Android)

Scan-first warehouse workflow (Location → Item → Quantity → In/Out) with min/max visibility.

<p align="center">
  <img src="./readme/images/mock_template.png" alt="PDA prototype" />
</p>

## 🛠️ 5. Technologies used (MVP)

| Layer | Stack |
|------|-------|
| **Apps** | .NET MAUI (Admin + Operation) |
| **Backend** | API (to be implemented) |
| **Database** | Relational DB (to be implemented) |

## 🧪 6. UI mocks (HTML)

Static mocks used only for UX validation:

- `docs/stock-control-admin-mock.html`
- `docs/stock-control-pda-mock.html`

---

## 📚 7. Documentation

- 🏠 [Main Documentation](README.md) - This document
- 📋 [Admin app (MAUI)](readme/README-admin-maui-pda.md) - Compact master-data management (Android + Windows desktop)
- 📋 [Operation app (MAUI Android PDA)](readme/README-operation-maui-pda.md) - Scan-driven stock movements

---

**© 2026 AdminSense. All rights reserved.**
