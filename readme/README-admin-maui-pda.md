<p align="center">
  <img src="./images/logo?sense.png" alt="AdminSense" width="100">
</p>

# Admin (MAUI Android / PDA) — Compact master data

This document describes the **Admin app** built with **.NET MAUI**, designed to run on **Android PDAs** (devices with built-in barcode scanners) and maintain the **minimum master data** required by the stock operation app (scan-driven).

## Goal

- Keep registration **simple and compact** (few screens, low friction).
- Allow the operation app to **consume** master data for movements and **min/max** visibility.
- **Online** operation is acceptable for the MVP.
- **No external integrations** in the MVP (ERP/WMS “maybe later”).

## Platforms

- **Android (PDA)**: primary target.
- **Windows desktop (optional)**: same MAUI codebase, useful for fast typing on a large screen/keyboard.

## Device assumptions (PDA)

- Scanner can run as a “keyboard wedge”, sending the scanned code as text.
- Scanner can be configured to send **Enter** after each scan.
- **Location** codes are **up to 12 characters**.

## Master data scope (MVP)

- **Users**: simple user registry (no roles).
- **Warehouses**.
- **Locations** per warehouse (code up to 12 chars).
- **Products**.
- **Items (SKU)**: sellable/stockable unit and its scan codes (EAN/GTIN/QR/internal code).
- **Prices** (simple; optionally by price list / validity).
- **Stock policy**: min/max per item (optionally per location).

## Suggested screens (very few)

- **Login** (or user selection, depending on authentication strategy).
- **Users**: list + create/edit/deactivate.
- **Warehouses**: list + create/edit/deactivate.
- **Locations**: list by warehouse + create/edit/deactivate + scanner input.
- **Products**: list + create/edit/deactivate.
- **Items (SKU)**: list + create/edit/deactivate + barcodes/QR + min/max.
- **Prices**: list by item + create/edit (optional in MVP).
- **Sync**: “Refresh data” / “Upload changes” (if offline is added later).

## UI mock (HTML)

To validate flow/ergonomics before the MAUI implementation:

- `docs/stock-control-admin-mock.html`

## Table mock (minimum data model)

> The idea is **minimum necessary** for master data + auditable stock control, without overcomplicating.

### `users`

- `id` (uuid/int)
- `username` (string, unique)
- `password_hash` (string)
- `name` (string)
- `is_active` (bool)
- `created_at` (datetime)

### `warehouses`

- `id`
- `code` (string, unique) — e.g., “WH01”
- `name` (string)
- `is_active`
- `created_at`

### `locations`

- `id`
- `warehouse_id` (FK → `warehouses.id`)
- `code` (string, **max 12**, unique per warehouse)
- `description` (string, optional)
- `is_active`
- `created_at`

### `products`

- `id`
- `code` (string, unique) — internal product code
- `name` (string)
- `description` (string, optional)
- `is_active`
- `created_at`

### `items` (SKU / stockable item)

- `id`
- `product_id` (FK → `products.id`)
- `sku` (string, unique)
- `name` (string) — display name on PDA
- `unit` (string) — e.g., “EA”, “BX”
- `is_active`
- `created_at`

### `item_barcodes` (codes accepted by the scanner)

- `id`
- `item_id` (FK → `items.id`)
- `code` (string, unique) — EAN/GTIN/QR/internal code
- `code_type` (string, optional) — e.g., EAN13, QR, INTERNAL
- `is_active`
- `created_at`

### `item_min_max`

- `id`
- `item_id` (FK → `items.id`)
- `warehouse_id` (FK → `warehouses.id`, optional)
- `location_id` (FK → `locations.id`, optional)
- `min_qty` (decimal/int)
- `max_qty` (decimal/int)
- `created_at`

> Simple rule: configure by **item + warehouse** (default) and optionally override by **item + location**.

### `item_prices` (optional in MVP)

- `id`
- `item_id` (FK → `items.id`)
- `price` (decimal)
- `currency` (string, e.g., “BRL”)
- `valid_from` (date, optional)
- `valid_to` (date, optional)
- `created_at`

### `stock_balances` (current balance)

- `id`
- `warehouse_id` (FK → `warehouses.id`)
- `location_id` (FK → `locations.id`)
- `item_id` (FK → `items.id`)
- `qty_on_hand` (decimal/int)
- `updated_at` (datetime)

> Alternative: compute balance from movements. For PDA and fast reporting, keeping `stock_balances` is practical.

### `stock_movements` (immutable lines / audit)

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

## Rules & validations (simple)

- **Locations**: `locations.code` up to 12 chars; prefer uppercase; unique per `warehouse_id`.
- **Scanner + Enter**: on scan fields, keep focus and treat Enter as “commit”.
- **Users without roles**: all users have the same permissions in the MVP; add roles later without breaking the model.
- **Negative stock**: decide in MVP (block or allow). If blocking, validate before saving `OUT`.

## Sync / consumed by the stock app

- The operation app should be able to:
  - Fetch `warehouses`, `locations`, `items`, `item_barcodes`, `item_min_max`, and `stock_balances`.
  - Post `stock_movements` (and receive updated `stock_balances`).

## Future extensions (non-MVP)

- ERP/WMS integration.
- Offline on PDA (movement queue).
- Lots/expiry/serial.
- Location transfers (paired OUT/IN linked).
- Roles/permissions and deeper audit trail.

