# Admin (MAUI Android / PDA) — Cadastros compactos

Este documento descreve o **aplicativo administrador** (Android) feito em **.NET MAUI**, pensado para rodar em **PDA** (dispositivo Android com leitor de código de barras embutido) e manter os **cadastros mínimos** necessários para o app de operação de estoque (scan-driven).

## Objetivo

- Ter um **cadastro simples e compacto** (sem excesso de telas/complexidade).
- Permitir que o app de operação (PDA/web) **consuma** os dados cadastrais para movimentação e visibilidade de **mínimo/máximo**.
- Operação **online** é aceitável no MVP.
- **Sem integrações** externas no MVP (ERP/WMS “talvez depois”).

## Plataformas

- **Android (PDA)**: alvo principal.
- **Windows desktop (opcional)**: mesma base MAUI, pensado para facilitar cadastros em tela grande/teclado.

## Premissas do dispositivo (PDA)

- O leitor pode operar em modo “keyboard wedge”, enviando o código como texto.
- O scanner pode ser configurado para enviar **Enter** ao final da leitura.
- Códigos de **localização** (endereços) têm **até 12 caracteres**.

## Escopo de cadastros (MVP)

- **Usuários**: cadastro simples (sem roles).
- **Armazéns** (warehouses).
- **Localizações** (endereços) por armazém (código até 12 chars).
- **Produtos**.
- **Itens (SKU)**: unidade vendável/estocável e seus códigos de leitura (EAN/GTIN/QR/código interno).
- **Preços** (simples; opcional por tabela/validade).
- **Política de estoque**: mínimo/máximo por item (e opcionalmente por localização).

## Telas sugeridas (bem poucas)

- **Login** (ou seleção de usuário, dependendo da estratégia de autenticação).
- **Usuários**: lista + criar/editar/inativar.
- **Armazéns**: lista + criar/editar/inativar.
- **Localizações**: lista por armazém + criar/editar/inativar + leitura por scanner.
- **Produtos**: lista + criar/editar/inativar.
- **Itens (SKU)**: lista + criar/editar/inativar + códigos de barras/QR + mínimo/máximo.
- **Preços**: lista por item + criar/editar (opcional no MVP).
- **Sincronização**: botão “Atualizar dados” / “Enviar alterações” (caso exista modo offline no futuro).

## Mock de tabelas (modelo de dados mínimo)

> A ideia é **mínimo necessário** para cadastros + controle de estoque (auditável), sem complicar.

### `users`

- `id` (uuid/int)
- `username` (string, único)
- `password_hash` (string)
- `name` (string)
- `is_active` (bool)
- `created_at` (datetime)

### `warehouses`

- `id`
- `code` (string, único) — ex.: “WH01”
- `name` (string)
- `is_active`
- `created_at`

### `locations`

- `id`
- `warehouse_id` (FK → `warehouses.id`)
- `code` (string, **máx. 12**, único por armazém)
- `description` (string, opcional)
- `is_active`
- `created_at`

### `products`

- `id`
- `code` (string, único) — código interno do produto
- `name` (string)
- `description` (string, opcional)
- `is_active`
- `created_at`

### `items` (SKU / item estocável)

- `id`
- `product_id` (FK → `products.id`)
- `sku` (string, único)
- `name` (string) — nome exibido no PDA
- `unit` (string) — ex.: “UN”, “CX”
- `is_active`
- `created_at`

### `item_barcodes` (códigos aceitos pelo scanner)

- `id`
- `item_id` (FK → `items.id`)
- `code` (string, único) — EAN/GTIN/QR/código interno
- `code_type` (string, opcional) — ex.: EAN13, QR, INTERNAL
- `is_active`
- `created_at`

### `item_min_max`

- `id`
- `item_id` (FK → `items.id`)
- `warehouse_id` (FK → `warehouses.id`, opcional)
- `location_id` (FK → `locations.id`, opcional)
- `min_qty` (decimal/int)
- `max_qty` (decimal/int)
- `created_at`

> Regra simples: configurar por **item + armazém** (padrão) e permitir sobrescrever por **item + localização** se necessário.

### `item_prices` (opcional no MVP)

- `id`
- `item_id` (FK → `items.id`)
- `price` (decimal)
- `currency` (string, ex.: “BRL”)
- `valid_from` (date, opcional)
- `valid_to` (date, opcional)
- `created_at`

### `stock_balances` (saldo atual)

- `id`
- `warehouse_id` (FK → `warehouses.id`)
- `location_id` (FK → `locations.id`)
- `item_id` (FK → `items.id`)
- `qty_on_hand` (decimal/int)
- `updated_at` (datetime)

> Alternativa: calcular saldo a partir de movimentos. Para PDA e relatórios rápidos, manter `stock_balances` é prático.

### `stock_movements` (linhas imutáveis / auditoria)

- `id`
- `created_at` (datetime)
- `user_id` (FK → `users.id`)
- `warehouse_id` (FK → `warehouses.id`)
- `location_id` (FK → `locations.id`)
- `item_id` (FK → `items.id`)
- `direction` (string) — `IN` (entrada) / `OUT` (saída)
- `qty` (decimal/int)
- `source` (string) — ex.: `PDA`, `ADMIN`
- `notes` (string, opcional)

## Regras e validações (simples)

- **Localização**: `locations.code` com **até 12 chars**; preferir uppercase; único por `warehouse_id`.
- **Scanner + Enter**: nas telas com leitura, manter foco no campo “Scan” e tratar Enter como “confirmar”.
- **Usuários sem roles**: todos os usuários têm o mesmo nível no MVP; no futuro, adicionar permissões/roles sem quebrar modelo.
- **Estoque negativo**: decidir no MVP (bloquear ou permitir). Se bloquear, validar antes de gravar movimento `OUT`.

## Sincronização / consumo pelo app de estoque

- O app de operação (PDA/web) deve conseguir:
  - Buscar `warehouses`, `locations`, `items`, `item_barcodes`, `item_min_max` e `stock_balances`.
  - Enviar `stock_movements` (e receber atualização de `stock_balances`).

## Futuras extensões (não-MVP)

- Integração ERP/WMS.
- Offline no PDA (fila de movimentos).
- Lotes/validade/serial.
- Transferência entre localizações (gerar OUT/IN vinculados).
- Roles/permissões e trilha de auditoria mais detalhada.

