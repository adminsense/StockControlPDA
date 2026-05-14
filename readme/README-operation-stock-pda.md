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
| **Workflow** | Location → Item → Quantity → In/Out |
| **Scanner** | Keyboard wedge (text + Enter) |
| **Connectivity** | Online (MVP) |
| **MAUI targets** | **Android only** — `Platforms/Android` only (`net10.0-android`, min API 21). iOS / Windows / Mac Catalyst folders are not in this repo. |

## ✅ 1. Goals (MVP)

- Register stock movements with a fast, repeatable flow: **Location → Item → Quantity → Inbound (+) / Outbound (−)**.
- Show **min/max** and simple alerts (below min / above max).
- **Online** operation (no offline in the MVP).

## 🔫 2. Scanner assumptions (keyboard wedge)

- Scanner types into the focused field.
- Scanner can send **Enter** at the end of each scan.
- **Location codes** are **up to 12 characters**.

## 🧭 3. Screens (compact)

| Screen | Role |
|--------|------|
| **Login** | Authenticate operator (MVP / future). |
| **Move stock** | Main flow: scan → quantity → **Inbound (+)** / **Outbound (−)**. |
| **Min/Max alerts** *(planned)* | List below min / above max with filters. |
| **Quick lookup** *(planned)* | Balance by item/location. |

---

## 🎛️ 4. Main screen — mock & real app (what each part does)

The mock (`mock_template.png`) follows the same idea as **`MainPage.xaml`** (“Move stock”): one scan field, clear steps, two big actions, reset.

| Area | In the app | What it does |
|------|------------|----------------|
| 📄 **Instruction line** | Top label | Short reminder: scan **location → item → quantity**, then tap **Inbound** or **Outbound**; **Reset** starts over. |
| 🧭 **Step** | `Step: …` label | Shows where you are: *scan location* → *scan item* → *enter quantity* (scanner wedge + **Enter** advances the step). |
| ⌨️ **Scan / type** | Single `Entry` | **Always** where codes go: location code (≤ 12), then item **SKU** or any **barcode line** from Admin, then quantity. Scanner types here + Enter = same as keyboard. |
| 📋 **Context** | Line under the entry | Echo of what was recognised (e.g. location name/code, item SKU) so the operator sees the system “locked” the right master data. |
| ➕ **Inbound (+)** | Green button | Confirms **stock IN** for current location + item + quantity → creates an **IN** movement (increases on-hand). |
| ➖ **Outbound (−)** | Red button | Confirms **stock OUT** → **OUT** movement (decreases on-hand). |
| 🔄 **Reset flow** | Bottom button | Clears **location, item, quantity** and returns to *scan location* — use after a mistake or when starting another bin. |

### Flow in one glance

| Step | You scan / type | Then |
|------|-----------------|------|
| 1️⃣ | **Location** code | Enter → step asks for item |
| 2️⃣ | **Item** (SKU or barcode) | Enter → step asks for quantity |
| 3️⃣ | **Quantity** (number) | Enter → tap **Inbound (+)** or **Outbound (−)** |

> **Tip:** if something is wrong, **Reset flow** before scanning again — faster than correcting a half-finished movement.

### Mock vs. app (labels)

| If the mock shows… | In `MainPage.xaml` it is… |
|--------------------|---------------------------|
| Big **+** / “add” area | **Inbound (+)** — green |
| Big **−** / “remove” area | **Outbound (−)** — red |
| Single scan line | **`ScanEntry`** — placeholder *“Scan or type code”* |
| Step / phase text | **`StepLabel`** — *Step: scan location* → *item* → *quantity* |
| Context / subtitle under the field | **`ContextLabel`** — shows resolved location or item |

## 🔁 5. Scan-first flow

1. **Scan location**
   - Validate size (≤ 12) and existence/active status.
2. **Scan item**
   - Accept any barcode line stored on the item (`items.Barcodes`, newline-separated).
3. **Enter quantity**
   - Default = 1; numeric keypad; minimal validation.
4. **Confirm action**
   - Inbound (+) writes `stock_movements(IN)`; Outbound (−) writes `stock_movements(OUT)`.
   - Update `stock_balances` (or receive updated balance from the API).

## ✅ 6. Rules & validations (MVP)

- **Location**: code ≤ 12; must exist and be active.
- **Item**: scanned or typed code must match an active item (typically **SKU** or a **barcode** from `items.Barcodes`; **article number** is maintained in Admin for catalog alignment, not necessarily what the scanner sends).
- **Quantity**: > 0.
- **Negative stock**: business decision in MVP
  - Option A: block `OUT` if balance is insufficient
  - Option B: allow negative and flag in reports
- **Audit**: every movement records `user_id`, timestamp, location, and item.

## 🔌 7. Data consumed/sent (API)

- **Read**
  - `warehouses`, `locations`, `items` (including `Barcodes`), `minmax_settings` (or equivalent API surface for min/max)
  - `stock_balances` (for lookup/alerts)
- **Write**
  - `stock_movements` (immutable lines)

## ✋ 8. PDA UX notes

- **Predictable focus**: after each Enter, advance the step and keep focus on the scan field.
- **Immediate feedback**: beep/visual on recognized location and item; clear error on unknown codes.
- **Minimal taps**: ideally only tap quantity and the (+/−) button.

---

## Documentation

- 🏠 [Main Documentation](../README.md) — Project overview

---

**© 2026 AdminSense. All rights reserved.**

