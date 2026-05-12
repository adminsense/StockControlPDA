<p align="center">
  <img src="./images/logo-asense.png" alt="AdminSense" width="100">
</p>

# Stock Control — Admin (Desktop)

![Status](https://img.shields.io/badge/Status-Implemented-198754?style=flat-square)
![Platform](https://img.shields.io/badge/Platform-Windows%20desktop-0d6efd?style=flat-square)
![Stack](https://img.shields.io/badge/Stack-Blazor%20Server%20%2B%20EF%20Core-6f42c1?style=flat-square)

Blazor Server **desktop Admin** used to register master data and view stock balances. The Admin UI is **English** (labels + validation messages).

---

### 📌 Quick facts

| Topic | Value |
|------|-------|
| 🖥️ **Platform** | Windows desktop |
| 🧱 **App type** | Blazor Server (Admin) |
| 🧭 **Navigation** | 8 tabs (4 + 4 rows) |
| ✅ **CRUD** | Create / Edit / Activate / Deactivate |
| 🔎 **Pagination** | 10 rows per page (Prev/Next) |
| 🧩 **Min/Max** | Dedicated **Min / Max** tab (`/minmax`): warehouse default + location override |
| 📦 **Stock** | On-hand per Warehouse + Location + Item |
| 🌐 **Language** | English (UI + validation messages) |

---

## 🧭 Tabs (exactly 7)

- **Users**
- **Warehouses**
- **Locations**
- **Suppliers**
- **Products**
- **Items (SKU)**
- **Min / Max**
- **Stock**

---

## 🗄️ Database (EF Core)

- Migrations live under `src/StockControl.Admin/Migrations/`. The baseline is a single **`InitialCreate`** migration that creates the full schema (including `suppliers`, product–supplier and item–product links, packaging, and price on items).
- On an **empty** SQL Server database, apply the schema from the repo root:

  `dotnet ef database update --project src/StockControl.Admin/StockControl.Admin.csproj --startup-project src/StockControl.Admin/StockControl.Admin.csproj`

---

## ✅ Common UI rules (applies to all pages)

- **Pagination**: 10 rows per page, with **Prev/Next** and “Showing X–Y of Z”.
- **Status**: boolean dropdown (**Active / Inactive**).
- **CRUD**: create/edit + deactivate/activate (no hard delete in the UI).
- **Validations**: user-friendly messages in English; `maxlength` enforced in inputs.
- **Feedback**: messages are shown as a **modal** (theme colors). When it is a validation error, closing the modal focuses the field that needs correction.

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
          <li><code>Name</code> required, ≤ 50</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><code>items</code></td>
      <td>
        <code>ProductId</code>, <code>Sku</code>, <code>DisplayName</code>, <code>Unit</code>, <code>PackagingType</code>, <code>PackageQuantity</code>, <code>Price</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li><code>ProductId</code> required (FK to <code>products</code>, delete restricted)</li>
          <li><code>Sku</code> required, unique, ≤ 40 (stored upper-case)</li>
          <li><code>DisplayName</code> required, ≤ 50</li>
          <li><code>Unit</code> required, ≤ 10 (physical unit; <code>stock_balances.QuantityOnHand</code> is expressed in this same unit)</li>
          <li><code>PackagingType</code> stored as <code>int</code> enum: Unit, Box, Case, Pallet, Bag, Kit</li>
          <li><code>PackageQuantity</code> decimal &gt; 0; must be <strong>1</strong> when packaging is <strong>Unit</strong></li>
          <li><code>Price</code> decimal ≥ 0</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><code>item_barcodes</code></td>
      <td>
        <code>ItemId</code>, <code>Code</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li><code>Code</code> required, unique</li>
          <li>Inserted from multi-line input (one per line)</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><code>minmax_settings</code></td>
      <td>
        <code>WarehouseId</code>, <code>ItemId</code>, <code>LocationId</code> (optional), <code>Min</code>, <code>Max</code>, <code>IsActive</code>
      </td>
      <td>
        <ul>
          <li><code>Min</code> and <code>Max</code> required, ≥ 0</li>
          <li><code>Max ≥ Min</code></li>
          <li>Default unique: (<code>WarehouseId</code>, <code>ItemId</code>) where <code>LocationId</code> is NULL</li>
          <li>Override unique: (<code>WarehouseId</code>, <code>ItemId</code>, <code>LocationId</code>) where <code>LocationId</code> is NOT NULL</li>
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
          <li><code>Name</code> required, ≤ 50</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td>🧩 <strong>Items (SKU)</strong></td>
      <td>Create / Edit / Activate / Deactivate items, manage barcodes</td>
      <td>
        <code>Product</code>, <code>Sku</code>, <code>DisplayName</code>, <code>Unit</code>, <code>Packaging type</code>, <code>Package quantity</code>, <code>Price</code>, <code>Barcodes</code>, <code>Status</code>
      </td>
      <td>
        <ul>
          <li><code>Product</code> required</li>
          <li><code>Sku</code> required, unique, ≤ 40 (stored upper-case)</li>
          <li><code>DisplayName</code> required, ≤ 50</li>
          <li><code>Unit</code> required, ≤ 10</li>
          <li><code>Package quantity</code> &gt; 0; must be <strong>1</strong> when packaging is <strong>Unit</strong></li>
          <li><code>Price</code> ≥ 0</li>
          <li>Barcodes: one per line; <code>item_barcodes.Code</code> is unique</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td>📊 <strong>Min / Max</strong></td>
      <td>Create / Edit / Activate / Deactivate Min/Max targets per warehouse (and optional location override)</td>
      <td>
        <code>Warehouse</code>, <code>Item</code>, <code>Location</code> (optional), <code>Min</code>, <code>Max</code>, <code>Status</code>
      </td>
      <td>
        <ul>
          <li>Route <code>/minmax</code></li>
          <li><code>Min</code>, <code>Max</code> required, ≥ 0 and <code>Max ≥ Min</code></li>
          <li>Uniqueness: default (<code>Warehouse</code>+<code>Item</code>) where Location is NULL; override (<code>Warehouse</code>+<code>Item</code>+<code>Location</code>) where Location is NOT NULL</li>
        </ul>
      </td>
    </tr>
    <tr>
      <td>📦 <strong>Stock</strong></td>
      <td>View on-hand quantities by warehouse + location + item, compare to Min/Max</td>
      <td>
        Filters: <code>Warehouse</code>, <code>Location</code>, <code>Search</code> (SKU, product code, or name), <code>Below/Above</code>
      </td>
      <td>
        <ul>
          <li>Home page: route <code>/</code></li>
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

