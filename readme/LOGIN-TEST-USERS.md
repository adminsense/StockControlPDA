# Test users — Stock Control (Admin + PDA)

Use these accounts after applying migrations and [`scripts/seed-user-passwords.sql`](../scripts/seed-user-passwords.sql).

| Username | Password | Role | App |
|----------|----------|------|-----|
| **admin** | `Pda2!Stock` | **1** — Admin (web) | Blazor Admin in the browser |
| **pda** | `Pda2!Stock` | **2** — Admin PDA | MAUI Android PDA |

- Role **1** cannot sign in on the PDA; role **2** cannot open the Admin UI.
- Passwords are stored as hashes in `users.Password`; the script above sets both test users.
- Create additional users in **Admin → Users** (role **1** or **2**, password required on **Save**).

**Screenshots:** Admin — [`images/mock_login.png`](./images/mock_login.png); PDA — [`images/pda-login-stock.png`](./images/pda-login-stock.png) (white card, underline fields, purple **Sign in**).
