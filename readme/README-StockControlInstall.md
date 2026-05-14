<p align="center">
  <img src="./images/logo-asense.png" alt="AdminSense" width="100">
</p>

# StockControlInstall

![Status](https://img.shields.io/badge/Status-Reference-0dcaf0?style=flat-square)
![Scope](https://img.shields.io/badge/Scope-Local%20dev%20setup-6f42c1?style=flat-square)
![Stack](https://img.shields.io/badge/Stack-Admin%20%2B%20MAUI%20Android-512BD4?style=flat-square)

Run **Stock Control Admin** on a Windows notebook together with the **MAUI PDA** app on an **Android emulator** or phone — end‑to‑end tests **without** dedicated PDA hardware.

**Paths:** `src/…` in the tables below are relative to the **repository root** (the same folder you use for `dotnet run`).

---

## Local testing (developer notebook — Admin + PDA without a handset)

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
| **HTTP / cleartext** | `src/StockControl.PDA/Platforms/Android/AndroidManifest.xml` includes **`android:usesCleartextTraffic="true"`** so dev **HTTP** URLs work. |
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
