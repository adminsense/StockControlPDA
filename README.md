<p align="center">
  <img src="./readme/images/logo-asense.png" alt="AdminSense" width="100">
</p>

# <span style="color: #0066cc">Stock Control — PDA</span>

![Status](https://img.shields.io/badge/Status-Proposal%20%2B%20Mock-0dcaf0?style=flat-square) ![Product](https://img.shields.io/badge/Product-.NET%20MAUI%20apps%20%28Admin%20%2B%20PDA%29-512BD4?style=flat-square) ![Copyright](https://img.shields.io/badge/Copyright-2026-blue?style=flat-square)

Client-facing proposal and **static UI mock** for a **stock keeping** application aimed at **warehouse PDAs**: scan-driven workflow, **minimum / maximum** stock visibility, and **mobile-first** ergonomics.

Proposed delivery is **.NET MAUI**:

### Admin app (Android + Windows desktop)

Compact master-data registration (users, products, items/SKUs, warehouses, locations, min/max).

<p align="center">
  <img src="./readme/images/mock_admin_template.png" alt="Admin prototype" />
</p>

### Operation app (Stock Control PDA) — Android

Scan-first warehouse workflow (Location → Item → Quantity → In/Out) with min/max visibility.

<p align="center">
  <img src="./readme/images/mock_template.png" alt="PDA prototype" />
</p>

---

## 🎯 Customer goals (summary)

- Scan-driven movements with a fast **Location → Item → Quantity → In/Out** sequence.
- Visibility of **minimum/maximum** thresholds and simple alerts.
- **Keyboard-wedge scanner** support (scan as text, often terminated by **Enter**).

## Documentation

- 🏠 [Main Documentation](README.md) - This document
- 📋 [Admin app (MAUI)](readme/README-admin-maui-pda.md) - Compact master-data management (Android + Windows desktop)
- 📋 [Operation app (MAUI Android PDA)](readme/README-operation-maui-pda.md) - Scan-driven stock movements

---

**© 2026 AdminSense. All rights reserved.**
