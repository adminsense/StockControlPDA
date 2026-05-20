# UI previews (match dev apps today)

Open in a browser. These previews mirror **current** Blazor Admin and MAUI PDA — not old prototypes.

| App | File | What you see |
|-----|------|----------------|
| **Admin** | [stock-control-admin-mock.html](stock-control-admin-mock.html) | **Only login modal** first (dark) → after sign-in: header, hero **Stock Control**, grid + tabs |
| **PDA** | [stock-control-pda-mock.html](stock-control-pda-mock.html) | **Light MAUI** login card → **Move stock** (`MainPage.xaml`) in phone frame |

## Test users (SQL seed)

| Username | Password | App |
|----------|----------|-----|
| `admin` | `Pda2!Stock` | Admin (role 1) |
| `pda` | `Pda2!Stock` | PDA (role 2) |

## Login (Admin + PDA — identical)

Same screen everywhere: dark backdrop `#0c111b`, **white card**, bold labels, **underline** inputs, purple **Sign in** `#512bd4`.

| | Title | Screenshot in repo |
|---|--------|-------------------|
| **Admin** | Stock Control — Admin | `readme/images/mock_login.png` |
| **PDA** | Stock Control — PDA | `readme/images/pda-login-stock.png` |

No role hint under the title (production UI).

Mocks match `LoginModal.razor` / `LoginPage.xaml` and `admin-theme.css` (login block overrides global dark inputs).

## Admin

1. Login only on `#0c111b` — then menu after `loginAdmin` + cookie.
2. Mock: **Sign in** without credentials (preview).

## PDA

1. Same login as Admin; then **Move stock** — layout per `readme/images/pda-move-stock.png` (`MainPage.xaml` + this mock).
2. Mock: **Sign in** without credentials (preview).
3. Real app: `POST /api/auth/login?app=pda` → Bearer → `//MainPage`.

## Deprecated

- [pda-move-stock.html](pda-move-stock.html) — old scan-first prototype; redirects to PDA preview.
- [stock-control-admin--audit-mock.html](stock-control-admin--audit-mock.html) — static audit-only slice.
