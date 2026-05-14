<p align="center">
  <img src="./images/logo-asense.png" alt="AdminSense" width="100">
</p>

# 📦 Operation (Stock Control) — MAUI Android (PDA)

![Status](https://img.shields.io/badge/Status-In%20development-0dcaf0?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-Android%20PDA-6f42c1?style=flat-square)
![Stack](https://img.shields.io/badge/Stack-.NET%20MAUI-512BD4?style=flat-square)

Scan-driven **operation app** built with **.NET MAUI for Android** for fast warehouse stock movements on PDAs.

---

### 📌 Quick facts

| Topic | Value |
|------|-------|
| **Platform** | Android PDA |
| **Workflow** | **Target (v2):** warehouse + location + item + summary + quantity → In/Out. **Today:** lean scan steps + In/Out + **Sync** \| **Reset**. |
| **Scanner** | Keyboard wedge (text + Enter) |
| **Connectivity** | Online (MVP) |
| **MAUI targets** | **Android only** — `Platforms/Android` only (`net10.0-android`, min API 21). iOS / Windows / Mac Catalyst folders are not in this repo. |

## ✅ 1. Goals (MVP)

- Register stock movements with a fast, repeatable flow: **Location → Item → Quantity → Inbound (+) / Outbound (−)**.
- Show **min/max** and simple alerts (below min / above max).
- **Online** operation (no offline in the MVP).

## 📋 2. Admin vs warehouse floor — what Admin does *not* know automatically

Admin holds **master data** and **posted transactions**. It does **not** magically reflect physical bins until someone records stock (movements, opening balances, or future cycle counts).

| Situation | Who creates the record | PDA / API today |
|-----------|------------------------|-----------------|
| **Which SKUs are physically in a bin** | Unknown until **stock events** are posted | Admin has no live “bin contents” view unless `stock_balances` / movements exist for that **warehouse + location + item**. |
| **New catalog item** (SKU / barcodes) | **Admin** (`items`) only | PDA **cannot** create items. **Inbound** fails until the item exists and is active. |
| **Known item**, first time in a **new bin** | **PDA Inbound** or **Admin** opening stock | `POST /api/stock/movements` **IN** **creates** `stock_balances` when no row exists yet for that triple. |
| **Unknown barcode** on the floor | — | Not bookable **until** Admin registers the item (or a future “request new item” workflow exists). |

**New empty location:** the **location** exists after Admin saves it. **Balances** appear after the first **IN** (or Admin opening qty). Operators use **Sync** (and future catalog reload) so pickers refresh; **scanning the location code** still works on each movement POST even before pickers exist.

## 🔫 3. Scanner assumptions (keyboard wedge)

- Scanner types into the focused field.
- Scanner can send **Enter** at the end of each scan.
- **Location codes** are **up to 12 characters**.

## 🧭 4. Screens (compact)

| Screen | Role |
|--------|------|
| **Login** | Authenticate operator (MVP / future). |
| **Move stock** | **Target (v2):** pickers, scan, **Summary** (on hand / min–max), quantity, In/Out, **Sync** \| **Reset** — see **§5**. **Today:** scan steps + context line + In/Out + **Sync** \| **Reset**. |
| **Min/Max alerts** *(planned)* | List below min / above max with filters. |
| **Quick lookup** *(planned)* | Balance by item/location. |

---

## 🎨 5. Move stock — **design guide**

**Source of truth for the next PDA UI:** interactive HTML [`docs/pda-move-stock-v2.html`](../docs/pda-move-stock-v2.html) and screenshot **`readme/images/mock_pda_v2.png`** (also shown below). The legacy `mock_template.png` wireframe is **not** used anymore.

<p align="center">
  <img src="./images/mock_pda_v2.png" alt="PDA — Move stock design guide (v2)" width="420" />
</p>

| Area (v2 mock) | Intended behaviour |
|----------------|-------------------|
| 🏭 **Warehouse** / 📍 **Location** / 🏷️ **Item** | Pickers; lists filtered per rules in **§7** when implemented. **Scan** (Enter) still sets location or item when the code matches. |
| ⌨️ **Scan or type code** | Keyboard wedge + **Enter** resolves a **location code** or **item** (SKU / barcode / article number per API rules). |
| 📊 **Summary** | Shows resolved **warehouse**, **location**, **item**, **on hand**, **min/max**, status pill — requires API lookups (not in the current lean `MainPage`). |
| 🔢 **Quantity** | Stepper **− / +** then **Inbound (+)** / **Outbound (−)**. |
| 🔄 **Sync** \| **Reset flow** | Same row. **Sync** → `GET /api/stock/sync` (MVP: counts + connectivity); later reload cached masters. **Reset** clears selection + quantity (mock) / scan flow (current app). |

**Current MAUI app (`MainPage.xaml`):** still the **lean** step flow (instruction, `StepLabel`, single `ScanEntry`, `ContextLabel`, In/Out, **Sync** \| **Reset**). Refactor toward the v2 layout and APIs in a later iteration.

### Legacy step flow (today’s app — until v2 UI lands)

| Step | You scan / type | Then |
|------|-----------------|------|
| 1️⃣ | **Location** code | Enter → step asks for item |
| 2️⃣ | **Item** (SKU or barcode) | Enter → step asks for quantity |
| 3️⃣ | **Quantity** (number) | Enter → tap **Inbound (+)** or **Outbound (−)** |

> **Tip:** if something is wrong, **Reset flow** before scanning again.

## 🔁 6. Scan-first flow

1. **Scan location**
   - Validate size (≤ 12) and existence/active status.
2. **Scan item**
   - Accept any barcode line stored on the item (`items.Barcodes`, newline-separated).
3. **Enter quantity**
   - Default = 1; numeric keypad; minimal validation.
4. **Confirm action**
   - Inbound (+) writes `stock_movements(IN)`; Outbound (−) writes `stock_movements(OUT)`.
   - Update `stock_balances` (or receive updated balance from the API).

## ✅ 7. Rules & validations (MVP)

- **Location**: code ≤ 12; must exist and be active.
- **Item**: scanned or typed code must match an active item (typically **SKU** or a **barcode** from `items.Barcodes`; **article number** is maintained in Admin for catalog alignment, not necessarily what the scanner sends).
- **Quantity**: > 0.
- **Negative stock**: business decision in MVP
  - Option A: block `OUT` if balance is insufficient
  - Option B: allow negative and flag in reports
- **Audit**: every movement records `user_id`, timestamp, location, and item.

### 📋 Item dropdown (warehouse + location) — which items to list?

**Status:** **not decided in code** (HTML mock uses a flat demo list). When the PDA UI matches **§5 (v2)**, pick **one** rule (or a documented hybrid) and expose it via API + UI.

| # | Rule | Meaning |
|---|------|--------|
| **A** | Row in `stock_balances` | Only items that already have a balance row for the selected **warehouse + location** (includes **quantity = 0**). |
| **B** | On hand **> 0** | Same as **A**, but exclude items with `QuantityOnHand <= 0`. Stronger for picking; blocks first inbound unless combined with another path. |
| **C** | All active items | Full **catalog** at the picker; warehouse/location only define **where** the movement posts. Longer lists; simplest query. |
| **D** | `minmax_settings` present | Only items with a min/max row for that **warehouse + location** (replenishment-style shortlist). |
| **E** | **Hybrid** | e.g. list **A** or **B** by default, but **always accept** an item resolved by **scan** even if it would not appear in the dropdown. |

**Implementation:** one explicit endpoint or query (e.g. items-for-location) should implement the chosen row(s) above; avoid duplicating conflicting logic in the app only.

## 🔌 8. Data consumed/sent (API)

- **Read**
  - `GET /api/stock/sync` — PDA **Sync** (MVP: returns counts of warehouses, active locations, active items; future: drive full catalog cache).
  - `warehouses`, `locations`, `items` (including `Barcodes`), `minmax_settings` (or equivalent API surface for min/max)
  - `stock_balances` (for lookup/alerts)
- **Write**
  - `stock_movements` (immutable lines)

### 🔄 Staying in sync with Admin (new locations, empty bins)

**Issue:** warehouses, **locations**, items, and **balances** change in **Admin**. The PDA must not keep an indefinitely stale list. Catalog vs physical stock is also covered in **§2**.

| Approach | Role |
|----------|------|
| **Reload on open / filter change** | When the operator opens **Move stock** or changes **warehouse** (and optionally **location**), call the API again for locations, items, balances. |
| **Manual Sync** | **Sync** next to **Reset** explicitly refreshes (MVP: `GET /api/stock/sync` counts); later full catalog cache. **Not** the same as Admin **Stock Control → Sync**. |
| **Background refresh** | Optional; mind battery and server load. |

**PDA Sync (MVP, implemented):** **Move stock** has **Sync** next to **Reset flow** (same row). It calls **`GET /api/stock/sync`** and shows warehouse / active location / active item **counts**. **Later:** reload cached masters for v2 pickers.

**New location, no stock yet:** **§2** and **§6** — balances follow first **IN** or Admin opening stock.

**Flow you described** (Admin: items + initial qty e.g. 1 at that location → PDA refreshes → operator counts / adjusts with movements) is **valid**, especially if the item picker uses rules like **A/B** (list driven by `stock_balances`). If the API creates balances on first **IN**, you can instead skip Admin qty and let the **first putaway** on the PDA open the bin.

## ✋ 9. PDA UX notes

- **Sync**: operator taps **Sync** after Admin changes masters (or periodically); today it confirms server reachability + counts; later it should refresh local catalog lists.
- **Predictable focus**: after each Enter, advance the step and keep focus on the scan field.
- **Immediate feedback**: beep/visual on recognized location and item; clear error on unknown codes.
- **Minimal taps**: ideally only tap quantity and the (+/−) button.

---

## Documentation

- 🏠 [Main Documentation](../README.md) — Project overview

---

**© 2026 AdminSense. All rights reserved.**

