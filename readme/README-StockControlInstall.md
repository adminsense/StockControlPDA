<p align="center">
  <img src="./images/logo-asense.png" alt="AdminSense" width="100">
</p>

# StockControlInstall

![Status](https://img.shields.io/badge/Status-Reference-0dcaf0?style=flat-square)
![Scope](https://img.shields.io/badge/Scope-Local%20dev%20setup-6f42c1?style=flat-square)
![Stack](https://img.shields.io/badge/Stack-Admin%20%2B%20MAUI%20Android-512BD4?style=flat-square)

Run **Stock Control Admin** on a Windows notebook together with the **MAUI PDA** app on an **Android emulator** or phone — end‑to‑end tests **without** dedicated PDA hardware.

**Paths:** `src/…` in the tables below are relative to the **repository root** (the same folder you use for `dotnet run`).

<!-- All Topic/Details tables: first column forced to 520px via colgroup + table-layout:fixed (same across tables). -->

---

## Local testing (developer notebook — Admin + PDA without a handset)

<table width="100%" border="0" cellspacing="0" cellpadding="10" style="table-layout:fixed">
<colgroup><col width="520" /><col /></colgroup>
<thead>
<tr><th width="520" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="520" align="left" valign="top"><strong>Goal</strong></td><td align="left" valign="top">Validate end‑to‑end behaviour <strong>before</strong> you have a physical PDA.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Admin (notebook)</strong></td><td align="left" valign="top">Runs on Windows: <strong>SQL Server</strong> + <strong>HTTP API</strong> + <strong>Blazor</strong> UI.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>PDA</strong></td><td align="left" valign="top">Run the <strong>MAUI</strong> app on an <strong>Android emulator</strong> <em>or</em> on a phone (<strong>USB</strong> or <strong>same Wi‑Fi</strong> as the notebook).</td></tr>
</tbody>
</table>

### What you need

<table width="100%" border="0" cellspacing="0" cellpadding="10" style="table-layout:fixed">
<colgroup><col width="520" /><col /></colgroup>
<thead>
<tr><th width="520" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="520" align="left" valign="top"><strong>OS + SDK</strong></td><td align="left" valign="top"><strong>Windows</strong> with a <strong>.NET SDK</strong> compatible with the solution. Check each <code>.csproj</code> (<code>TargetFramework</code> / packages).</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Database</strong></td><td align="left" valign="top"><strong>SQL Server</strong> reachable from that machine. Same meaning as <code>ConnectionStrings:connStockControlPDA</code> in Admin.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Android target</strong></td><td align="left" valign="top"><strong>Android SDK</strong> + <strong>emulator</strong> (Android Studio) <em>or</em> a <strong>physical device</strong> with USB debugging <em>or</em> same Wi‑Fi as the notebook.</td></tr>
</tbody>
</table>

### 1) Database connection

<table width="100%" border="0" cellspacing="0" cellpadding="10" style="table-layout:fixed">
<colgroup><col width="520" /><col /></colgroup>
<thead>
<tr><th width="520" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="520" align="left" valign="top"><strong>Config file</strong></td><td align="left" valign="top"><code>src/StockControl.Admin/appsettings.json</code></td></tr>
<tr><td width="520" align="left" valign="top"><strong>Setting</strong></td><td align="left" valign="top"><code>ConnectionStrings:connStockControlPDA</code></td></tr>
<tr><td width="520" align="left" valign="top"><strong>Value</strong></td><td align="left" valign="top">Your SQL Server <strong>instance</strong> and <strong>catalog</strong> (database). Replace any sample placeholder.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>First Admin start</strong></td><td align="left" valign="top">Startup runs EF <strong><code>MigrateAsync</code></strong>. Schema is created or updated automatically.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Manual migrations (optional)</strong></td><td align="left" valign="top"><a href="README-admin-stock.md">README-admin-stock</a> → <strong>Database (EF Core)</strong> (<code>dotnet ef database update ...</code>).</td></tr>
</tbody>
</table>

### 2) (Optional) demo / volume data

<table width="100%" border="0" cellspacing="0" cellpadding="10" style="table-layout:fixed">
<colgroup><col width="520" /><col /></colgroup>
<thead>
<tr><th width="520" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="520" align="left" valign="top"><strong>Large optional dataset</strong></td><td align="left" valign="top">Run <code>src/StockControl.Admin/Database/seed.sql</code> in <strong>SQL Server Management Studio</strong>. Run it <strong>after</strong> migrations. Read the <strong>header comments</strong> in the file (scope, prerequisites).</td></tr>
<tr><td width="520" align="left" valign="top"><strong>What it loads</strong></td><td align="left" valign="top">Bulk <strong>warehouses</strong>, <strong>locations</strong>, <strong>items</strong>, and related data (see script).</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Minimal smoke test</strong></td><td align="left" valign="top"><strong>Skip</strong> the seed. Create <strong>warehouses</strong>, <strong>locations</strong>, <strong>items</strong> (and optionally <strong>Min / Max</strong> / opening balances) only via the <strong>Admin</strong> browser UI.</td></tr>
</tbody>
</table>

### 3) Run Admin so the phone/emulator can call the HTTP API

<table width="100%" border="0" cellspacing="0" cellpadding="10" style="table-layout:fixed">
<colgroup><col width="520" /><col /></colgroup>
<thead>
<tr><th width="520" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="520" align="left" valign="top"><strong>Working directory</strong></td><td align="left" valign="top">Repository <strong>root</strong>.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Launch profile</strong></td><td align="left" valign="top"><strong><code>http-pda</code></strong> in <code>src/StockControl.Admin/Properties/launchSettings.json</code>.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>HTTP binding</strong></td><td align="left" valign="top">Kestrel listens on <strong><code>http://0.0.0.0:5264</code></strong> (all interfaces). Emulator-friendly.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Blazor Admin (browser)</strong></td><td align="left" valign="top">On the notebook: <strong><code>http://localhost:5264</code></strong>. Same process serves UI + API.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>REST base (notebook)</strong></td><td align="left" valign="top"><strong><code>http://localhost:5264/api/...</code></strong> (e.g. Postman on the same machine).</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Link to PDA default</strong></td><td align="left" valign="top">The MAUI embedded <code>Api:BaseUrl</code> <strong><code>http://10.0.2.2:5264</code></strong> maps the <strong>emulator</strong> to this host port.</td></tr>
</tbody>
</table>

```powershell
dotnet run --project src/StockControl.Admin/StockControl.Admin.csproj --launch-profile http-pda
```

### 4) Point the MAUI app at your notebook

<table width="100%" border="0" cellspacing="0" cellpadding="10" style="table-layout:fixed">
<colgroup><col width="520" /><col /></colgroup>
<thead>
<tr><th width="520" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="520" align="left" valign="top"><strong>Config file</strong></td><td align="left" valign="top">Embedded <strong><code>src/StockControl.PDA/appsettings.json</code></strong>. Property: <strong><code>Api:BaseUrl</code></strong>.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Android Emulator</strong></td><td align="left" valign="top">Set <strong><code>http://10.0.2.2:5264</code></strong>. This is the default in the repo.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Why <code>10.0.2.2</code>?</strong></td><td align="left" valign="top">The emulator’s special alias to the <strong>host</strong> machine (your notebook) loopback.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Physical phone (same Wi‑Fi)</strong></td><td align="left" valign="top">Set <strong><code>http://&lt;NOTEBOOK_LAN_IP&gt;:5264</code></strong>. Use the notebook’s <strong>IPv4</strong> on the LAN.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Find the IPv4 (Windows)</strong></td><td align="left" valign="top">Run <strong><code>ipconfig</code></strong>. Use the <strong>Wi‑Fi</strong> adapter’s IPv4 address.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Windows Firewall</strong></td><td align="left" valign="top">Allow <strong>inbound TCP 5264</strong> (private network). Otherwise the phone cannot reach Kestrel.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>HTTP / cleartext</strong></td><td align="left" valign="top"><code>src/StockControl.PDA/Platforms/Android/AndroidManifest.xml</code> includes <strong><code>android:usesCleartextTraffic="true"</code></strong> so dev <strong>HTTP</strong> URLs work.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>After you change BaseUrl</strong></td><td align="left" valign="top"><strong>Rebuild</strong> and <strong>redeploy</strong> the MAUI app so the embedded JSON is picked up.</td></tr>
</tbody>
</table>

### 5) Run the PDA (Move stock)

<table width="100%" border="0" cellspacing="0" cellpadding="10" style="table-layout:fixed">
<colgroup><col width="520" /><col /></colgroup>
<thead>
<tr><th width="520" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="520" align="left" valign="top"><strong>Open the project</strong></td><td align="left" valign="top"><code>src/StockControl.PDA/StockControl.PDA.csproj</code> in <strong>Visual Studio</strong> or <strong>Rider</strong>.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Build / deploy</strong></td><td align="left" valign="top">Use your normal MAUI workflow (<code>dotnet build</code>, debug deploy, etc.).</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Run target</strong></td><td align="left" valign="top">Select an <strong>Android emulator</strong> or a <strong>physical device</strong>.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Start debugging</strong></td><td align="left" valign="top"><strong>F5</strong> (or your IDE’s Run command).</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Screen</strong></td><td align="left" valign="top">Open <strong>Move stock</strong>.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Check connectivity</strong></td><td align="left" valign="top">Tap <strong>Sync</strong>. Calls <strong><code>GET /api/stock/sync</code></strong>.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>If Admin is reachable</strong></td><td align="left" valign="top">You get <strong>counts</strong>. Pickers load from <strong><code>/api/pda/catalog/...</code></strong>.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Post movements</strong></td><td align="left" valign="top">Use <strong>Inbound (+)</strong> / <strong>Outbound (−)</strong> → <strong><code>POST /api/stock/movements</code></strong>.</td></tr>
</tbody>
</table>

### 6) If you have no Android target at all

<table width="100%" border="0" cellspacing="0" cellpadding="10" style="table-layout:fixed">
<colgroup><col width="520" /><col /></colgroup>
<thead>
<tr><th width="520" align="left" valign="top">Topic</th><th align="left" valign="top">Details</th></tr>
</thead>
<tbody>
<tr><td width="520" align="left" valign="top"><strong>What you can still test</strong></td><td align="left" valign="top"><strong>Admin</strong> UI and <strong>REST API</strong> from the notebook only.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Suggested tools</strong></td><td align="left" valign="top"><strong>Browser</strong>. <strong>Postman</strong> (or similar) for raw HTTP.</td></tr>
<tr><td width="520" align="left" valign="top"><strong>Example: sync / counts</strong></td><td align="left" valign="top"><code>GET http://localhost:5264/api/stock/sync</code></td></tr>
<tr><td width="520" align="left" valign="top"><strong>Example: PDA catalog</strong></td><td align="left" valign="top"><code>GET</code> endpoints under <strong><code>/api/pda/catalog/...</code></strong></td></tr>
<tr><td width="520" align="left" valign="top"><strong>Example: movements</strong></td><td align="left" valign="top"><code>POST http://localhost:5264/api/stock/movements</code></td></tr>
<tr><td width="520" align="left" valign="top"><strong>UI limitation</strong></td><td align="left" valign="top">Full <strong>Move stock</strong> UI (<strong>pickers</strong>, <strong>Summary</strong>, <strong>quantity stepper</strong>) is <strong>Android-only</strong> in this repository.</td></tr>
</tbody>
</table>

---

## Documentation

- 🏠 [Main Documentation](../README.md) — Project overview

---

**© 2026 AdminSense. All rights reserved.**
