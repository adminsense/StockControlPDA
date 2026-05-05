<p align="center">
  <img src="./images/logo?sense.png" alt="AdminSense" width="100">
</p>

# Operation (Stock Control) — MAUI Android (PDA)

This document describes the **operation app** (Stock Control) built with **.NET MAUI for Android**, running on a **PDA** with a built-in scanner, using a **scan-driven** workflow focused on mobile ergonomics.

## Goal (MVP)

- Register stock movements with a fast, repeatable flow:
  - **Location → Item → Quantity → Inbound (+) / Outbound (−)**
- Show **min/max** and simple alerts (below min / above max).
- **Online** operation (no offline in the MVP).

## Scanner assumptions

- Scanner in “keyboard wedge” mode typing into the focused field.
- Scanner configurable to send **Enter** at the end of each scan.
- **Location** codes are **up to 12 characters**.

## Screens (compact)

- **Login**
- **Move stock** (main screen)
  - “Scan/Input” field (always focused)
  - Shows the current step and what was recognized (location/item)
  - Quantity (large numeric)
  - Big buttons: **Inbound (+)** and **Outbound (−)**
  - Short confirmation (optional) and/or “undo last” (optional)
- **Min/Max alerts**
  - List of items below min / above max
  - Quick filters: warehouse / location / item
- **Quick lookup**
  - Balance by item/location
  - Scan-based search (item or location)

## Scan-first flow

1. **Scan location**
   - Validate size (≤ 12) and existence/active status.
2. **Scan item**
   - Accept any code present in `item_barcodes.code`.
3. **Enter quantity**
   - Default = 1; numeric keypad; minimal validation.
4. **Confirm action**
   - Inbound (+) writes `stock_movements(IN)`; Outbound (−) writes `stock_movements(OUT)`.
   - Update `stock_balances` (or receive updated balance from the API).

## Rules & validations (MVP)

- **Location**: code ≤ 12; must exist and be active.
- **Item**: code must exist and be active.
- **Quantity**: > 0.
- **Negative stock**: business decision in MVP
  - Option A: block `OUT` if balance is insufficient
  - Option B: allow negative and flag in reports
- **Audit**: every movement records `user_id`, timestamp, location, and item.

## Data consumed/sent (API)

- **Read**
  - `warehouses`, `locations`, `items`, `item_barcodes`, `item_min_max`
  - `stock_balances` (for lookup/alerts)
- **Write**
  - `stock_movements` (immutable lines)

## PDA UX notes

- **Predictable focus**: after each Enter, advance the step and keep focus on the scan field.
- **Immediate feedback**: beep/visual on recognized location and item; clear error on unknown codes.
- **Minimal taps**: ideally only tap quantity and the (+/−) button.

