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
| **Workflow** | **Target (`docs/pda-move-stock.html`):** warehouse + location + item + summary + quantity → In/Out. **Today:** lean scan steps + In/Out + **Sync** \| **Reset**. |
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
| **Move stock** | **Target:** same as mock [`pda-move-stock.html`](../docs/pda-move-stock.html) — pickers, scan, **Summary**, quantity, In/Out, **Sync** \| **Reset** (**§5**). **Today:** scan steps + context line + In/Out + **Sync** \| **Reset**. |
| **Min/Max alerts** *(planned)* | List below min / above max with filters. |
| **Quick lookup** *(planned)* | Balance by item/location. |

---

## 🎨 5. Move stock — UI mock (`pda-move-stock.html`)

**Template:** interactive HTML [`docs/pda-move-stock.html`](../docs/pda-move-stock.html) — open in a browser; this is the **layout and behaviour reference** for the PDA **Move stock** screen. Refresh the screenshot below when the HTML changes.

<p align="center">
  <img src="./images/pda-move-stock.png" alt="PDA — Move stock (mock screenshot)" width="420" />
</p>

| Area (mock) | Intended behaviour |
|----------------|-------------------|
| 🏭 **Warehouse** / 📍 **Location** / 🏷️ **Item** | Pickers; lists filtered per rules in **§7** when implemented. **Scan** (Enter) still sets location or item when the code matches. |
| ⌨️ **Scan or type code** | Keyboard wedge + **Enter** resolves a **location code** or **item** (SKU / barcode / article number per API rules). |
| 📊 **Summary** | Shows resolved **warehouse**, **location**, **item**, **on hand**, **min/max**, status pill — requires API lookups (not in the current lean `MainPage`). |
| 🔢 **Quantity** | Stepper **− / +** then **Inbound (+)** / **Outbound (−)**. |
| 🔄 **Sync** \| **Reset flow** | Same row. **Sync** → `GET /api/stock/sync` (MVP: counts + connectivity); later reload cached masters for pickers. **Reset** clears selection + quantity in the HTML mock; in the app today it clears the scan step flow. |

**Current MAUI app (`MainPage.xaml`):** still the **lean** step flow (instruction, `StepLabel`, single `ScanEntry`, `ContextLabel`, In/Out, **Sync** \| **Reset**). Implement the fields and **Summary** to match this mock and the rules in **§2**, **§6–§8**.

### Current app step flow (until the mock layout is implemented)

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

**Status:** **not decided in code** (the HTML mock uses a flat demo list). When the PDA UI matches **§5** (`pda-move-stock.html`), pick **one** rule (or a documented hybrid) and expose it via API + UI.

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

**PDA Sync (MVP, implemented):** **Move stock** has **Sync** next to **Reset flow** (same row). It calls **`GET /api/stock/sync`** and shows warehouse / active location / active item **counts**. **Later:** reload cached masters for the pickers in **§5**.

**New location, no stock yet:** **§2** and **§6** — balances follow first **IN** or Admin opening stock.

**Flow you described** (Admin: items + initial qty e.g. 1 at that location → PDA refreshes → operator counts / adjusts with movements) is **valid**, especially if the item picker uses rules like **A/B** (list driven by `stock_balances`). If the API creates balances on first **IN**, you can instead skip Admin qty and let the **first putaway** on the PDA open the bin.

## ✋ 9. PDA UX notes

- **Sync**: operator taps **Sync** after Admin changes masters (or periodically); today it confirms server reachability + counts; later it should refresh local catalog lists.
- **Predictable focus**: after each Enter, advance the step and keep focus on the scan field.
- **Immediate feedback**: beep/visual on recognized location and item; clear error on unknown codes.
- **Minimal taps**: ideally only tap quantity and the (+/−) button.

---

## 🧪 10. Local testing (developer notebook — Admin + PDA without a handset)

| Topic | Details |
|--------|---------|
| **Goal** | Validate end‑to‑end behaviour **before** you have a physical PDA. |
| **Admin (notebook)** | Runs on Windows: **SQL Server** + **HTTP API** + **Blazor** UI. |
| **PDA** | Run the **MAUI** app on an **Android emulator** *or* on a phone (**USB** or **same Wi‑Fi** as the notebook). |

### What you need

| Topic | Details |
|--------|---------|
| **OS + SDK** | **Windows** with a **.NET SDK** compatible with the solution. Check each `.csproj` (`TargetFramework` / packages). |
| **Database** | **SQL Server** reachable from that machine. Same meaning as `ConnectionStrings:connStockControlPDA` in Admin. |
| **Android target** | **Android SDK** + **emulator** (Android Studio) *or* a **physical device** with USB debugging *or* same Wi‑Fi as the notebook. |

### 1) Database connection

| Topic | Details |
|--------|---------|
| **Config file** | `src/StockControl.Admin/appsettings.json` |
| **Setting** | `ConnectionStrings:connStockControlPDA` |
| **Value** | Your SQL Server **instance** and **catalog** (database). Replace any sample placeholder. |
| **First Admin start** | Startup runs EF **`MigrateAsync`**. Schema is created or updated automatically. |
| **Manual migrations (optional)** | [README-admin-stock](README-admin-stock.md) → **Database (EF Core)** (`dotnet ef database update ...`). |

### 2) (Optional) demo / volume data

| Topic | Details |
|--------|---------|
| **Large optional dataset** | Run `src/StockControl.Admin/Database/seed.sql` in **SQL Server Management Studio**. Run it **after** migrations. Read the **header comments** in the file (scope, prerequisites). |
| **What it loads** | Bulk **warehouses**, **locations**, **items**, and related data (see script). |
| **Minimal smoke test** | **Skip** the seed. Create **warehouses**, **locations**, **items** (and optionally **Min / Max** / opening balances) only via the **Admin** browser UI. |

### 3) Run Admin so the phone/emulator can call the HTTP API

| Topic | Details |
|--------|---------|
| **Working directory** | Repository **root**. |
| **Launch profile** | **`http-pda`** in `src/StockControl.Admin/Properties/launchSettings.json`. |
| **HTTP binding** | Kestrel listens on **`http://0.0.0.0:5264`** (all interfaces). Emulator-friendly. |
| **Blazor Admin (browser)** | On the notebook: **`http://localhost:5264`**. Same process serves UI + API. |
| **REST base (notebook)** | **`http://localhost:5264/api/...`** (e.g. Postman on the same machine). |
| **Link to PDA default** | The MAUI embedded `Api:BaseUrl` **`http://10.0.2.2:5264`** maps the **emulator** to this host port. |

```powershell
dotnet run --project src/StockControl.Admin/StockControl.Admin.csproj --launch-profile http-pda
```

### 4) Point the MAUI app at your notebook

| Topic | Details |
|--------|---------|
| **Config file** | Embedded **`src/StockControl.PDA/appsettings.json`**. Property: **`Api:BaseUrl`**. |
| **Android Emulator** | Set **`http://10.0.2.2:5264`**. This is the default in the repo. |
| **Why `10.0.2.2`?** | The emulator’s special alias to the **host** machine (your notebook) loopback. |
| **Physical phone (same Wi‑Fi)** | Set **`http://<NOTEBOOK_LAN_IP>:5264`**. Use the notebook’s **IPv4** on the LAN. |
| **Find the IPv4 (Windows)** | Run **`ipconfig`**. Use the **Wi‑Fi** adapter’s IPv4 address. |
| **Windows Firewall** | Allow **inbound TCP 5264** (private network). Otherwise the phone cannot reach Kestrel. |
| **HTTP / cleartext** | `AndroidManifest.xml` includes **`android:usesCleartextTraffic="true"`** so dev **HTTP** URLs work. |
| **After you change BaseUrl** | **Rebuild** and **redeploy** the MAUI app so the embedded JSON is picked up. |

### 5) Run the PDA (Move stock)

| Topic | Details |
|--------|---------|
| **Open the project** | `src/StockControl.PDA/StockControl.PDA.csproj` in **Visual Studio** or **Rider**. |
| **Build / deploy** | Use your normal MAUI workflow (`dotnet build`, debug deploy, etc.). |
| **Run target** | Select an **Android emulator** or a **physical device**. |
| **Start debugging** | **F5** (or your IDE’s Run command). |
| **Screen** | Open **Move stock**. |
| **Check connectivity** | Tap **Sync**. Calls **`GET /api/stock/sync`**. |
| **If Admin is reachable** | You get **counts**. Pickers load from **`/api/pda/catalog/...`**. |
| **Post movements** | Use **Inbound (+)** / **Outbound (−)** → **`POST /api/stock/movements`**. |

### 6) If you have no Android target at all

| Topic | Details |
|--------|---------|
| **What you can still test** | **Admin** UI and **REST API** from the notebook only. |
| **Suggested tools** | **Browser**. **Postman** (or similar) for raw HTTP. |
| **Example: sync / counts** | `GET http://localhost:5264/api/stock/sync` |
| **Example: PDA catalog** | `GET` endpoints under **`/api/pda/catalog/...`** |
| **Example: movements** | `POST http://localhost:5264/api/stock/movements` |
| **UI limitation** | Full **Move stock** UI (**pickers**, **Summary**, **quantity stepper**) is **Android-only** in this repository. |

---

## Documentation

- 🏠 [Main Documentation](../README.md) — Project overview

---

**© 2026 AdminSense. All rights reserved.**

