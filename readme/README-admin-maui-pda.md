<p align="center">
  <img src="./images/logo-asense.png" alt="AdminSense" width="100">
</p>

# 🧩 Admin — Compact master data

![Status](https://img.shields.io/badge/Status-Proposal%20%2B%20Mock-0dcaf0?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-Android%20%2B%20Windows%20desktop-6f42c1?style=flat-square)
![Stack](https://img.shields.io/badge/Stack-.NET%20MAUI-512BD4?style=flat-square)

Compact **Admin app** for **Android and Windows desktop** to maintain the **minimum master data** required by the scan-driven operation app.

---

### 📌 Quick facts

| Topic | Value |
|------|-------|
| **Platforms** | Android (PDA) + Windows desktop |
| **Purpose** | Master data for Operation app |
| **Scanner input** | Keyboard wedge supported (text + Enter) |
| **Location code** | Up to 12 characters |

## ✅ 1. Goals (MVP)

- Keep registration **simple and compact** (few screens, low friction).
- Support the operation app with master data for **stock movements** and **min/max** visibility.
- **Online** operation is acceptable for the MVP.
- **No external integrations** in the MVP (ERP/WMS “maybe later”).

## 📱 2. Platforms & device assumptions

### 2.1 Platforms

- **Android (PDA)**: primary target.
- **Windows desktop**: same MAUI codebase, useful for fast typing on a large screen/keyboard.

### 2.2 Scanner behavior (keyboard wedge)

- The scanner types the code into the focused field.
- It can be configured to send **Enter** after each scan.
- **Location codes** are **up to 12 characters**.

## 🧾 3. Master data scope

- **Users**: simple registry (no roles).
- **Warehouses**
- **Locations** (per warehouse, code ≤ 12 chars)
- **Products**
- **Items (SKU)** + accepted scan codes (EAN/GTIN/QR/internal codes)
- **Min/Max policy** per item (optionally per location)
- **Prices** (optional in MVP)

## 🧭 4. Suggested screens (minimal)

- **Login** (or user selection, depending on authentication strategy).
- **Users**: list + create/edit/deactivate.
- **Warehouses**: list + create/edit/deactivate.
- **Locations**: list by warehouse + create/edit/deactivate + scanner input.
- **Products**: list + create/edit/deactivate.
- **Items (SKU)**: list + create/edit/deactivate + barcode/QR + min/max.
- **Prices**: list by item + create/edit (optional in MVP).
- **Sync**: “Refresh data” / “Upload changes” (if offline is added later).

## 🧪 5. UI mock

- `../docs/stock-control-admin-mock.html`

## 🗃️ 6. Table mock (minimum data model)

> The goal is **minimum necessary** for master data + auditable stock control, without overcomplicating.

### 6.1 Core tables

- **`users`**
  - `id` (uuid/int)
  - `username` (string, unique)
  - `password_hash` (string)
  - `name` (string)
  - `is_active` (bool)
  - `created_at` (datetime)

- **`warehouses`**
  - `id`
  - `code` (string, unique) — e.g., “WH01”
  - `name` (string)
  - `is_active`
  - `created_at`

- **`locations`**
  - `id`
  - `warehouse_id` (FK → `warehouses.id`)
  - `code` (string, **max 12**, unique per warehouse)
  - `description` (string, optional)
  - `is_active`
  - `created_at`

- **`products`**
  - `id`
  - `code` (string, unique) — internal product code
  - `name` (string)
  - `description` (string, optional)
  - `is_active`
  - `created_at`

- **`items`** (SKU / stockable item)
  - `id`
  - `product_id` (FK → `products.id`)
  - `sku` (string, unique)
  - `name` (string) — display name on PDA
  - `unit` (string) — e.g., “EA”, “BX”
  - `is_active`
  - `created_at`

- **`item_barcodes`**
  - `id`
  - `item_id` (FK → `items.id`)
  - `code` (string, unique)
  - `code_type` (string, optional) — e.g., EAN13, QR, INTERNAL
  - `is_active`
  - `created_at`

### 6.2 Min/Max

- **`item_min_max`**
  - `id`
  - `item_id` (FK → `items.id`)
  - `warehouse_id` (FK → `warehouses.id`, optional)
  - `location_id` (FK → `locations.id`, optional)
  - `min_qty` (decimal/int)
  - `max_qty` (decimal/int)
  - `created_at`

Rule: configure by **item + warehouse** (default) and optionally override by **item + location**.

### 6.3 Optional tables (MVP+)

- **`item_prices`** (optional in MVP)
  - `id`
  - `item_id` (FK → `items.id`)
  - `price` (decimal)
  - `currency` (string, e.g., “BRL”)
  - `valid_from` (date, optional)
  - `valid_to` (date, optional)
  - `created_at`

- **`stock_balances`** (current balance; optional if you compute from movements)
  - `id`
  - `warehouse_id` (FK → `warehouses.id`)
  - `location_id` (FK → `locations.id`)
  - `item_id` (FK → `items.id`)
  - `qty_on_hand` (decimal/int)
  - `updated_at` (datetime)

- **`stock_movements`** (immutable audit lines)
  - `id`
  - `created_at` (datetime)
  - `user_id` (FK → `users.id`)
  - `warehouse_id` (FK → `warehouses.id`)
  - `location_id` (FK → `locations.id`)
  - `item_id` (FK → `items.id`)
  - `direction` (string) — `IN` / `OUT`
  - `qty` (decimal/int)
  - `source` (string) — e.g., `PDA`, `ADMIN`
  - `notes` (string, optional)

## ✅ 7. Simple rules & validations

- **Locations**: `locations.code` ≤ 12 chars; prefer uppercase; unique per `warehouse_id`.
- **Scanner + Enter**: on scan fields, keep focus and treat Enter as “commit”.
- **Users without roles**: all users have the same permissions in the MVP; add roles later without breaking the model.
- **Negative stock**: decide in MVP (block or allow). If blocking, validate before saving `OUT`.

## 🔄 8. Sync expectations (used by the operation app)

- Fetch: `warehouses`, `locations`, `items`, `item_barcodes`, `item_min_max` (and optionally `stock_balances`).
- Post: `stock_movements` (and receive updated balance if applicable).

---

## Documentation

- 🏠 [Main Documentation](../README.md) - Project overview and proposal

---

**© 2026 AdminSense. All rights reserved.**

