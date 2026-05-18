<p align="center">
  <img src="./images/logo-asense.png" alt="AdminSense" width="100">
</p>

# Stock Control — Admin (Desktop)

![Status](https://img.shields.io/badge/Status-Implemented-198754?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-Windows%20desktop-0d6efd?style=flat-square)
![Stack](https://img.shields.io/badge/Stack-Blazor%20Server%20%2B%20EF%20Core-6f42c1?style=flat-square)

Blazor Server **desktop Admin** used to register master data and view stock balances. The Admin UI is **English** (labels + validation messages).

<p align="center">
  <img src="./images/mock_admin_template.png" alt="Stock Control — Admin: Stock tab (filters + list)" width="920" />
</p>

*Screenshot (`images/mock_admin_template.png`): **Stock Control** tab — two-row tab bar (8 tabs), **STOCK** filters (Warehouse, Location, Supplier, Stock status, full-width Search and Sync), **LIST** grid with Supplier, Reorder, and Status.*

---

### 📸 Standard master screens (search → form → grid)

<p align="center">
  <img src="./images/forms_screen.png" alt="Admin — standard forms layout" width="920" />
</p>

| Piece | Behaviour |
|-------|-----------|
| 🔎 **Search** | Live filter while typing; **hint** under the field lists which columns participate (varies per tab). No decorative search icon — filtering is instant on keystroke. |
| 📝 **Form** | Always visible: empty = **Save** (new row); row selected from grid = **Update** (same fields). **Cancel** clears the form and returns to **Save**. |
| 📊 **Grid** | **Click a row** (active rows only) to load that record into the form. **Inactive** rows: row style + **Edit** disabled — turn **ON** first, then edit. |
| ↕️ **Sort** | Sortable headers (**↓ / ↑**): first click = descending, second = ascending; active column highlighted. **Items** grid uses tighter header typography so labels stay on one line. |
| 🔘 **Active toggle** | **ON** = active in DB; **OFF** = inactive. Inactive records stay in the list but cannot be opened for edit until active again. |
| 💾 **Save / Update** | Single primary action; label switches automatically (`Id == 0` → Save, else Update). |

**Stock** tab (see screenshot above): **Search** uses **SearchField** (SKU, article number, product code, display name). Dropdown filters: **Warehouse**, **Location**, **Supplier**, **Stock status** (all / below min / above max). **Search** and **Sync** span the full filter width (aligned with the two-column rows above).

### 🔎 Search hints (live filter)

| Tab | Text search matches |
|-----|---------------------|
| 👤 **Users** | Username, name |
| 🏭 **Warehouses** | Code, name |
| 📍 **Locations** | Warehouse code, location code, description |
| 🏢 **Suppliers** | Code, name |
| 📦 **Products** | Code, name |
| 🏷️ **Items** | SKU, article number, product code, display name, barcodes |
| ⚖️ **Min / Max** | Warehouse code, location code, SKU, article number |
| 📦 **Stock** | SKU, article number, product code, display name — *plus* **Warehouse**, **Location**, **Supplier**, **Stock status** dropdowns |

---

### 📌 Quick facts

| Topic | Value |
|------|-------|
| 🖥️ **Platform** | Windows desktop |
| 🧱 **App type** | Blazor Server (Admin) |
| 🧭 **Navigation** | 8 tabs (4 + 4 rows) |
| ✅ **CRUD** | Create / Edit / Activate / Deactivate |
| 🔎 **Pagination** | 10 rows per page (Prev/Next) |
| 🧩 **Min/Max** | Dedicated **Min / Max** tab (`/minmax`): targets per warehouse + item + **location** (location required) |
| 📦 **Stock** | On-hand per Warehouse + Location + Item |
| 🌐 **Language** | English (UI + validation messages) |

---

## 🧭 Tabs (8)

- **Users**
- **Warehouses**
- **Locations**
- **Suppliers**
- **Products**
- **Items (SKU)**
- **Min / Max** (`/minmax`)
- **Stock** (`/` and `/stock`)

---

## 🗄️ Database (EF Core)

- Migrations: `src/StockControl.Admin/Migrations/`. The Admin app applies pending migrations on startup (`MigrateAsync`).
- To update the database manually from the repo root:

  `dotnet ef database update --project src/StockControl.Admin/StockControl.Admin.csproj --startup-project src/StockControl.Admin/StockControl.Admin.csproj`

---

## ✅ Common UI rules (applies to all pages)

- **Pagination**: 10 rows per page, with **Prev/Next** and “Showing X–Y of Z”.
- **Status**: boolean dropdown (**Active / Inactive**).
- **CRUD**: create/edit + deactivate/activate (no hard delete in the UI).
- **Validations**: user-friendly messages in English; `maxlength` enforced in inputs.
- **Feedback**: messages are shown as a **modal** (theme colors). When it is a validation error, closing the modal focuses the field that needs correction.
- **List sorting**: sortable headers (except **Actions**); first click = **↓** descending (default load is newest-by-`Id`); second click = **↑** ascending; active column highlighted — same behaviour as in the **Standard master screens** table above.

### Stock page — Sync

- **Sync** removes keyboard focus from the button (avoids a “stuck” focus ring), then compares the latest catalog activity on **`items`** (`MAX(UpdatedAt ?? CreatedAt)`) to a baseline taken after the last successful sync (including the first page load).
- If nothing changed: success modal **“Everything is synchronized.”**
- If the catalog changed: a **busy** overlay (spinner) appears briefly while the grid reloads (`stock_balances` + Min/Max resolution); then **“Synchronization completed.”**

### UI — readability

- Form controls use **higher-contrast** backgrounds, borders, placeholders, and `select` / `option` colors in `StockControl.Admin.Client/wwwroot/css/admin-theme.css`, with `color-scheme: dark` for better native dropdown rendering.

---

## 🗃️ Data model (tables + fields)

<table>
  <thead>
    <tr>
      <th align="left">Table</th>
      <th align="left">Fields (key ones)</th>
      <th align="left">Constraints / rules</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><code>users</code></td>
      <td>
        <code>Username</code>, <code>Name</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li><code>Username</code> required, unique, ≤ 50</li>
          <li><code>Name</code> required, ≤ 50</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><code>warehouses</code></td>
      <td>
        <code>Code</code>, <code>Name</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li><code>Code</code> required, unique, ≤ 20 (stored upper-case)</li>
          <li><code>Name</code> required, ≤ 50</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><code>locations</code></td>
      <td>
        <code>WarehouseId</code>, <code>Code</code>, <code>Description</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li><code>WarehouseId</code> required</li>
          <li><code>Code</code> required, ≤ 12 (stored upper-case)</li>
          <li>Unique per warehouse: (<code>WarehouseId</code>, <code>Code</code>)</li>
          <li><code>Description</code> optional, ≤ 50</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><code>suppliers</code></td>
      <td>
        <code>Code</code>, <code>Name</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li><code>Code</code> required, unique, ≤ 20 (stored upper-case)</li>
          <li><code>Name</code> required, ≤ 100</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><code>products</code></td>
      <td>
        <code>SupplierId</code>, <code>Code</code>, <code>Name</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li><code>SupplierId</code> required (FK to <code>suppliers</code>, delete restricted)</li>
          <li><code>Code</code> required, unique, ≤ 40 (stored upper-case)</li>
          <li><code>Name</code> required, ≤ 100</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><code>items</code></td>
      <td>
        <code>ProductId</code>, <code>Sku</code>, <code>ArticleNumber</code>, <code>DisplayName</code>, <code>Unit</code>, <code>PackagingType</code>, <code>PackageQuantity</code>, <code>Price</code>, <code>Barcodes</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li><code>ProductId</code> required (FK to <code>products</code>, delete restricted)</li>
          <li><code>Sku</code> required, unique, ≤ 40 (stored upper-case) — internal stock-keeping code</li>
          <li><code>ArticleNumber</code> required in DB (default empty), ≤ 50 — supplier / catalog <strong>Artikelnummer</strong>, distinct from <code>Sku</code></li>
          <li><code>DisplayName</code> required, ≤ 100</li>
          <li><code>Unit</code> required, ≤ 10 (physical unit; <code>stock_balances.QuantityOnHand</code> is expressed in this same unit)</li>
          <li><code>PackagingType</code> stored as <code>int</code> enum: Unit, Box, Case, Pallet, Bag, Kit</li>
          <li><code>PackageQuantity</code> decimal &gt; 0; must be <strong>1</strong> when packaging is <strong>Unit</strong></li>
          <li><code>Price</code> decimal ≥ 0</li>
          <li><code>Barcodes</code> required in DB (default empty); newline-separated list of scanner codes; each non-empty line must be unique across all items (enforced in Admin on save)</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><code>minmax_settings</code></td>
      <td>
        <code>WarehouseId</code>, <code>ItemId</code>, <code>LocationId</code>, <code>Min</code>, <code>Max</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li><code>LocationId</code> required (targets are always per physical location)</li>
          <li><code>Min</code> and <code>Max</code> required, ≥ 0</li>
          <li><code>Max ≥ Min</code></li>
          <li>Unique: (<code>WarehouseId</code>, <code>ItemId</code>, <code>LocationId</code>)</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><code>stock_balances</code></td>
      <td>
        <code>WarehouseId</code>, <code>LocationId</code>, <code>ItemId</code>, <code>QuantityOnHand</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li>Unique: (<code>WarehouseId</code>, <code>LocationId</code>, <code>ItemId</code>)</li>
          <li>Represents the current on-hand quantity per warehouse + location + item (same unit as the item’s <code>Unit</code>)</li>
        </ul>
      </td>
    </tr>
  </tbody>
</table>

---

## 🪟 Screens (what each tab does)

<table>
  <thead>
    <tr>
      <th align="left">Tab</th>
      <th align="left">What you can do</th>
      <th align="left">Fields</th>
      <th align="left">Rules / validations</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>👥 <strong>Users</strong></td>
      <td>Create / Edit / Activate / Deactivate users</td>
      <td>
        <code>Username</code>, <code>Name</code>, <code>Status</code>
      </td>
      <td>
        <ul>
          <li><code>Username</code> required, unique, ≤ 50</li>
          <li><code>Name</code> required, ≤ 50</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td>🏭 <strong>Warehouses</strong></td>
      <td>Create / Edit / Activate / Deactivate warehouses</td>
      <td>
        <code>Code</code>, <code>Name</code>, <code>Status</code>
      </td>
      <td>
        <ul>
          <li><code>Code</code> required, unique, ≤ 20 (stored upper-case)</li>
          <li><code>Name</code> required, ≤ 50</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td>📍 <strong>Locations</strong></td>
      <td>Create / Edit / Activate / Deactivate locations per warehouse</td>
      <td>
        <code>Warehouse</code>, <code>Code</code>, <code>Description</code>, <code>Status</code>
      </td>
      <td>
        <ul>
          <li><code>Warehouse</code> required</li>
          <li><code>Code</code> required, ≤ 12 (stored upper-case)</li>
          <li>Unique per warehouse: (<code>WarehouseId</code>, <code>Code</code>)</li>
          <li><code>Description</code> optional, ≤ 50</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td>🏷️ <strong>Suppliers</strong></td>
      <td>Create / Edit / Activate / Deactivate suppliers</td>
      <td>
        <code>Code</code>, <code>Name</code>, <code>Status</code>
      </td>
      <td>
        <ul>
          <li><code>Code</code> required, unique, ≤ 20 (stored upper-case)</li>
          <li><code>Name</code> required, ≤ 100</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td>🧾 <strong>Products</strong></td>
      <td>Create / Edit / Activate / Deactivate products</td>
      <td>
        <code>Supplier</code>, <code>Code</code>, <code>Name</code>, <code>Status</code>
      </td>
      <td>
        <ul>
          <li><code>Supplier</code> required</li>
          <li><code>Code</code> required, unique, ≤ 40 (stored upper-case)</li>
          <li><code>Name</code> required, ≤ 100</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td>🧩 <strong>Items (SKU)</strong></td>
      <td>Create / Edit / Activate / Deactivate items, manage barcodes</td>
      <td>
        <code>Product</code>, <code>Sku</code>, <code>Article number</code>, <code>DisplayName</code>, <code>Unit</code>, <code>Packaging type</code>, <code>Package quantity</code>, <code>Price</code>, <code>Barcodes</code>, <code>Status</code>
      </td>
      <td>
        <ul>
          <li><code>Product</code> required</li>
          <li><code>Sku</code> required, unique, ≤ 40 (stored upper-case)</li>
          <li><code>Article number</code> optional in UI (blank stored as empty string); ≤ 50</li>
          <li><code>DisplayName</code> required, ≤ 100</li>
          <li><code>Unit</code> required, ≤ 10</li>
          <li><code>Package quantity</code> &gt; 0; must be <strong>1</strong> when packaging is <strong>Unit</strong></li>
          <li><code>Price</code> ≥ 0</li>
          <li>Barcodes: one per line; each code must be unique across all items (stored in <code>items.Barcodes</code>)</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td>📊 <strong>Min / Max</strong></td>
      <td>Create / Edit / Activate / Deactivate Min/Max targets per warehouse + item + location</td>
      <td>
        <code>Warehouse</code>, <code>Item</code>, <code>Location</code>, <code>Min</code>, <code>Max</code>, <code>Status</code>
      </td>
      <td>
        <ul>
          <li>Route <code>/minmax</code></li>
          <li><code>Warehouse</code>, <code>Item</code>, and <code>Location</code> required</li>
          <li><code>Min</code>, <code>Max</code> required, ≥ 0 and <code>Max ≥ Min</code></li>
          <li>Uniqueness: one row per (<code>Warehouse</code>+<code>Item</code>+<code>Location</code>)</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td>📦 <strong>Stock</strong></td>
      <td>View on-hand quantities by warehouse + location + item; compare to Min/Max; filter by supplier and stock status</td>
      <td>
        Filters: <code>Warehouse</code>, <code>Location</code>, <code>Supplier</code>, <code>Stock status</code> (all / below min / above max), <code>Search</code> (SKU, article number, product code, display name), <code>Sync</code>
      </td>
      <td>
        <ul>
          <li>Home page: route <code>/</code> (also <code>/stock</code>)</li>
          <li>List columns: Warehouse, Location, Supplier, Product, SKU, Art. no., Name, On hand, Min, Max, <strong>Reorder</strong> (qty to reach min when below), Status</li>
          <li><strong>Sync</strong>: compares latest <code>items</code> activity to last baseline; modal when unchanged; busy overlay + reload when catalog changed</li>
          <li>Min/Max resolve order: location override first, then warehouse default</li>
          <li>Status tag: OK / Below / Above</li>
        </ul>
      </td>
    </tr>
  </tbody>
</table>

---

## Documentation

- 🏠 [Main Documentation](../README.md) - Project overview

---

**© 2026 AdminSense. All rights reserved.**

