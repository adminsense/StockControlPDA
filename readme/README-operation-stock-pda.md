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

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Topic</th><th align="left" valign="top">Value</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>Platform</strong></td><td align="left" valign="top">Android PDA</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Workflow</strong></td><td align="left" valign="top"><strong>Move stock</strong> follows the mock <a href="../docs/pda-move-stock.html"><code>pda-move-stock.html</code></a>: pickers, scan, Summary, quantity, In/Out, <strong>Sync</strong> | <strong>Reset</strong> — see <strong>section 4</strong>.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Scanner</strong></td><td align="left" valign="top">Keyboard wedge (text + Enter)</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Connectivity</strong></td><td align="left" valign="top">Online (MVP)</td></tr>
<tr><td width="300" align="left" valign="top"><strong>MAUI targets</strong></td><td align="left" valign="top"><strong>Android only</strong> — <code>Platforms/Android</code> only (<code>net10.0-android</code>, min API 21). iOS / Windows / Mac Catalyst folders are not in this repo.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Local install</strong></td><td align="left" valign="top"><strong><a href="README-StockControlInstall.md">README-StockControlInstall.md</a></strong> — run Admin + PDA from a notebook (no physical PDA).</td></tr>
</tbody>
</table>

## ✅ 1. Goals (MVP)

- Register stock movements with a fast, repeatable flow: **Location → Item → Quantity → Inbound (+) / Outbound (−)**.
- Show **min/max** and simple alerts (below min / above max).
- **Online** operation (no offline in the MVP).

## 📋 2. Admin vs warehouse floor — what Admin does *not* know automatically

Admin holds **master data** and **posted transactions**. It does **not** magically reflect physical bins until someone records stock (movements, opening balances, or future cycle counts).

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Situation</th><th width="280" align="left" valign="top">Who creates the record</th><th align="left" valign="top">PDA / API today</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>Which SKUs are physically in a bin</strong></td><td width="280" align="left" valign="top">Unknown until <strong>stock events</strong> are posted</td><td align="left" valign="top">Admin has no live “bin contents” view unless <code>stock_balances</code> / movements exist for that <strong>warehouse + location + item</strong>.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>New catalog item</strong> (SKU / barcodes)</td><td width="280" align="left" valign="top"><strong>Admin</strong> (<code>items</code>) only</td><td align="left" valign="top">PDA <strong>cannot</strong> create items. <strong>Inbound</strong> fails until the item exists and is active.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Known item</strong>, first time in a <strong>new bin</strong></td><td width="280" align="left" valign="top"><strong>PDA Inbound</strong> or <strong>Admin</strong> opening stock</td><td align="left" valign="top"><code>POST /api/stock/movements</code> <strong>IN</strong> <strong>creates</strong> <code>stock_balances</code> when no row exists yet for that triple.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Unknown barcode</strong> on the floor</td><td width="280" align="left" valign="top">—</td><td align="left" valign="top">Not bookable <strong>until</strong> Admin registers the item (or a future “request new item” workflow exists).</td></tr>
</tbody>
</table>

**New empty location:** the **location** exists after Admin saves it. **Balances** appear after the first **IN** (or Admin opening qty). Operators use **Sync** (and future catalog reload) so pickers refresh; **scanning the location code** still works on each movement POST even before pickers exist.

## 🔫 3. Scanner assumptions (keyboard wedge)

- Scanner types into the focused field.
- Scanner can send **Enter** at the end of each scan.
- **Location codes** are **up to 12 characters**.

## 🎨 4. Move stock — UI mock & MAUI behaviour

<p align="center">
  <img src="./images/pda-move-stock.png" alt="PDA — Move stock (mock screenshot)" width="420" />
</p>

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Area (mock)</th><th align="left" valign="top">Intended behaviour</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top">🏭 <strong>Warehouse</strong> / 📍 <strong>Location</strong> / 🏷️ <strong>Item</strong></td><td align="left" valign="top"><strong>Pickers.</strong> How the <strong>item</strong> list is filtered is a <strong>product/API choice</strong> (rules <strong>A–E</strong>, <strong>section 6</strong>). <strong>Scan</strong> (Enter) should still set location or item when the code matches.</td></tr>
<tr><td width="300" align="left" valign="top">⌨️ <strong>Scan or type code</strong></td><td align="left" valign="top">Keyboard wedge + <strong>Enter</strong> resolves a <strong>location code</strong> or <strong>item</strong> (SKU / barcode / article number per API rules — <strong>section 5</strong>).</td></tr>
<tr><td width="300" align="left" valign="top">📊 <strong>Summary</strong></td><td align="left" valign="top"><strong>Warehouse</strong>, <strong>location</strong>, <strong>item</strong>, <strong>on hand</strong>, <strong>min/max</strong>, status pill — server lookups (<code>/api/pda/catalog/summary</code>).</td></tr>
<tr><td width="300" align="left" valign="top">🔢 <strong>Quantity</strong></td><td align="left" valign="top">Stepper <strong>− / +</strong> then <strong>Inbound (+)</strong> / <strong>Outbound (−)</strong>.</td></tr>
<tr><td width="300" align="left" valign="top">🔄 <strong>Sync</strong> | <strong>Reset flow</strong></td><td align="left" valign="top">Same row. <strong>Sync</strong> → <code>GET /api/stock/sync</code> (MVP: counts + connectivity); after success, <strong>reload catalog pickers</strong> from <code>/api/pda/catalog/...</code>. <strong>Reset</strong> clears selection + quantity (mock + MAUI).</td></tr>
</tbody>
</table>

### Planned screens (not in this MVP build)

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Screen</th><th align="left" valign="top">Role</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>Login</strong></td><td align="left" valign="top">Authenticate operator (future).</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Min/Max alerts</strong></td><td align="left" valign="top">List below min / above max with filters (planned).</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Quick lookup</strong></td><td align="left" valign="top">Balance by item/location (planned).</td></tr>
</tbody>
</table>

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

> **Tip:** if the wrong location/item is selected, use **Reset flow** on **Move stock** before scanning again.

## ✅ 6. Rules & validations (MVP)

- **Location**: code ≤ 12; must exist and be active.
- **Item**: scanned or typed code must match an active item (typically **SKU** or a **barcode** from `items.Barcodes`; **article number** is maintained in Admin for catalog alignment, not necessarily what the scanner sends).
- **Quantity**: > 0.
- **Negative stock**: business decision in MVP
  - Option A: block `OUT` if balance is insufficient
  - Option B: allow negative and flag in reports
- **Audit**: every movement records `user_id`, timestamp, location, and item.

### 📋 Item dropdown (warehouse + location) — which items to list?

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>Picker list (MVP)</strong></td><td align="left" valign="top"><strong><code>GET /api/pda/catalog/items</code></strong> returns <strong>all active items</strong> → <strong>rule C</strong> in the table below. Warehouse + location on the PDA only define <strong>where</strong> the movement posts.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Still open (product)</strong></td><td align="left" valign="top">Narrow or replace the list with <strong>A</strong>, <strong>B</strong>, <strong>D</strong>, or <strong>E</strong> via a dedicated API (e.g. <code>items-for-location</code>); do not fork conflicting rules only in the client.</td></tr>
</tbody>
</table>

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="64" align="left" valign="top">#</th><th width="220" align="left" valign="top">Rule</th><th align="left" valign="top">Meaning</th></tr>
</thead>
<tbody>
<tr><td width="64" align="left" valign="top"><strong>A</strong></td><td width="220" align="left" valign="top">Row in <code>stock_balances</code></td><td align="left" valign="top">Only items that already have a balance row for the selected <strong>warehouse + location</strong> (includes <strong>quantity = 0</strong>).</td></tr>
<tr><td width="64" align="left" valign="top"><strong>B</strong></td><td width="220" align="left" valign="top">On hand <strong>&gt; 0</strong></td><td align="left" valign="top">Same as <strong>A</strong>, but exclude items with <code>QuantityOnHand &lt;= 0</code>. Stronger for picking; blocks first inbound unless combined with another path.</td></tr>
<tr><td width="64" align="left" valign="top"><strong>C</strong></td><td width="220" align="left" valign="top">All active items</td><td align="left" valign="top">Full <strong>catalog</strong> at the picker; warehouse/location only define <strong>where</strong> the movement posts. Longer lists; simplest query.</td></tr>
<tr><td width="64" align="left" valign="top"><strong>D</strong></td><td width="220" align="left" valign="top"><code>minmax_settings</code> present</td><td align="left" valign="top">Only items with a min/max row for that <strong>warehouse + location</strong> (replenishment-style shortlist).</td></tr>
<tr><td width="64" align="left" valign="top"><strong>E</strong></td><td width="220" align="left" valign="top"><strong>Hybrid</strong></td><td align="left" valign="top">e.g. list <strong>A</strong> or <strong>B</strong> by default, but <strong>always accept</strong> an item resolved by <strong>scan</strong> even if it would not appear in the dropdown.</td></tr>
</tbody>
</table>

**Implementation:** one explicit endpoint or query (e.g. items-for-location) should implement the chosen row(s) above; avoid duplicating conflicting logic in the app only.

## 🔌 7. Data consumed/sent (API)

- **Read**
  - `GET /api/stock/sync` — PDA **Sync** (MVP: returns counts of warehouses, active locations, active items; future: drive full catalog cache).
  - `warehouses`, `locations`, `items` (including `Barcodes`), `minmax_settings` (or equivalent API surface for min/max)
  - `stock_balances` (for lookup/alerts)
- **Write**
  - `stock_movements` (immutable lines)

### 🔄 Staying in sync with Admin (new locations, empty bins)

#### The issue

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>What changes in Admin</strong></td><td align="left" valign="top"><strong>Warehouses</strong>, <strong>locations</strong>, <strong>items</strong>, and <strong>balances</strong> evolve over time.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Risk on the PDA</strong></td><td align="left" valign="top">Stale pickers or wrong assumptions about what is in a bin.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Related concept</strong></td><td align="left" valign="top">Catalog vs physical stock — see <strong>section 2</strong> of this document.</td></tr>
</tbody>
</table>

#### Ways to keep the PDA fresh

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Approach</th><th align="left" valign="top">Role</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>Reload on open / filter change</strong></td><td align="left" valign="top">When the operator opens <strong>Move stock</strong> or changes <strong>warehouse</strong> (and optionally <strong>location</strong>), call the API again for locations, items, balances.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Manual Sync</strong></td><td align="left" valign="top"><strong>Sync</strong> next to <strong>Reset</strong> explicitly refreshes (MVP: <code>GET /api/stock/sync</code> counts); later full catalog cache. <strong>Not</strong> the same as Admin <strong>Stock Control → Sync</strong>.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Background refresh</strong></td><td align="left" valign="top">Optional; mind battery and server load.</td></tr>
</tbody>
</table>

#### What **Sync** does today (MVP)

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>Where</strong></td><td align="left" valign="top"><strong>Move stock</strong> — <strong>Sync</strong> and <strong>Reset flow</strong> on the same row.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>HTTP</strong></td><td align="left" valign="top"><strong><code>GET /api/stock/sync</code></strong></td></tr>
<tr><td width="300" align="left" valign="top"><strong>Response (today)</strong></td><td align="left" valign="top">Counts: warehouses, active locations, active items.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Roadmap</strong></td><td align="left" valign="top">Reload cached masters for the pickers (see <strong>section 4</strong> mock).</td></tr>
</tbody>
</table>

#### New location, still empty

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>Meaning</strong></td><td align="left" valign="top">The <strong>location</strong> exists in Admin. <strong>No</strong> <code>stock_balances</code> row until something posts stock.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Ways a balance appears</strong></td><td align="left" valign="top">First <strong>IN</strong> on the PDA <strong>or</strong> opening quantity entered in <strong>Admin</strong>.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Cross‑refs</strong></td><td align="left" valign="top"><strong>Section 2</strong> (Admin vs floor) and <strong>section 5</strong> (scan / confirm flow).</td></tr>
</tbody>
</table>

#### Two valid ways to run day‑to‑day

These are both **correct**; which you prefer depends on whether the item list is driven by **`stock_balances`** (rules **A** / **B** in **section 6**) or by full catalog (**C**), etc.

##### Path 1 — Admin sets opening quantity, PDA adjusts

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Step</th><th align="left" valign="top">Who / what</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>1</strong></td><td align="left" valign="top"><strong>Admin</strong> registers the <strong>item</strong> (and related master data).</td></tr>
<tr><td width="300" align="left" valign="top"><strong>2</strong></td><td align="left" valign="top"><strong>Admin</strong> posts an <strong>opening quantity</strong> at that <strong>warehouse + location</strong> (e.g. <strong>1</strong>). A row exists in <strong><code>stock_balances</code></strong>.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>3</strong></td><td align="left" valign="top">Operator uses <strong>Sync</strong> / reload on the <strong>PDA</strong> so lists and summaries reflect the server.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>4</strong></td><td align="left" valign="top">Operator <strong>counts</strong> or <strong>adjusts</strong> using <strong>Inbound (+)</strong> / <strong>Outbound (−)</strong> movements.</td></tr>
</tbody>
</table>

**When this fits best:** item picker rules **A** or **B**, because the dropdown is naturally driven by **`stock_balances`**.

##### Path 2 — First putaway on the PDA (no Admin qty first)

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Step</th><th align="left" valign="top">Who / what</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>1</strong></td><td align="left" valign="top"><strong>Location</strong> and <strong>item</strong> exist in Admin. <strong>No</strong> balance row yet for that triple.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>2</strong></td><td align="left" valign="top">First <strong>Inbound (+)</strong> on the <strong>PDA</strong> creates <strong><code>stock_balances</code></strong> (if the API behaves that way — see product rules).</td></tr>
<tr><td width="300" align="left" valign="top"><strong>3</strong></td><td align="left" valign="top">Later movements adjust the same bin.</td></tr>
</tbody>
</table>

**When this fits best:** you rely on **first IN** to “open” the bin; you may skip entering opening qty in Admin entirely.

<table width="100%" border="0" cellspacing="0" cellpadding="10">
<thead>
<tr><th width="300" align="left" valign="top">Path</th><th align="left" valign="top">Picker hint</th></tr>
</thead>
<tbody>
<tr><td width="300" align="left" valign="top"><strong>Path 1</strong></td><td align="left" valign="top">Rules <strong>A</strong> / <strong>B</strong> align well: the item already appears because <strong><code>stock_balances</code></strong> has a row.</td></tr>
<tr><td width="300" align="left" valign="top"><strong>Path 2</strong></td><td align="left" valign="top">Rule <strong>C</strong> (full catalog) or <strong>hybrid E</strong> helps until a balance exists; first <strong>IN</strong> then enables balance‑driven lists if you switch to <strong>A</strong> / <strong>B</strong> later.</td></tr>
</tbody>
</table>

---

## ✋ 8. PDA UX notes

- **Sync**: operator taps **Sync** after Admin changes masters (or periodically). Calls `GET /api/stock/sync`; on success the app **reloads catalog pickers** from `/api/pda/catalog/...` (MVP).
- **Predictable focus**: keep wedge input on **Scan** when possible; **Enter** resolves the code into the pickers (**section 4**).
- **Immediate feedback**: beep/visual on recognized location and item; clear error on unknown codes.
- **Minimal taps**: ideally only tap quantity and the (+/−) button.

---

## Documentation

- 🏠 [Main Documentation](../README.md) — Project overview

---

**© 2026 AdminSense. All rights reserved.**
