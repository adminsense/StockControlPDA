# Operação (Stock Control) — MAUI Android (PDA)

Este documento descreve o **app de operação** (Stock Control) em **.NET MAUI para Android**, rodando em **PDA** com leitor integrado, com fluxo **scan-driven** e foco em ergonomia mobile.

## Objetivo (MVP)

- Registrar movimentações de estoque com um fluxo rápido e repetível:
  - **Localização → Item → Quantidade → Entrada (+) / Saída (−)**
- Mostrar **mínimo/máximo** e alertas simples (abaixo do mínimo / acima do máximo).
- Operação **online** (sem offline no MVP).

## Premissas do scanner

- Scanner em modo “keyboard wedge” digitando no campo focado.
- Scanner configurável para enviar **Enter** no final do código.
- **Localização** (endereço) tem **código até 12 caracteres**.

## Telas (compactas)

- **Login**
- **Movimentar estoque** (tela principal)
  - Campo “Scan/Entrada” (sempre com foco)
  - Exibe passo atual e o que foi reconhecido (local/item)
  - Quantidade (numérico grande)
  - Botões grandes: **Entrada (+)** e **Saída (−)**
  - Confirmação curta (opcional) e/ou “desfazer último” (opcional)
- **Alertas Min/Max**
  - Lista de itens abaixo do mínimo / acima do máximo
  - Filtros rápidos: armazém / localização / item
- **Consulta rápida**
  - Saldo por item/localização
  - Busca por scan (item ou localização)

## Fluxo de leitura (Scan-first)

1. **Ler localização**
   - Validar tamanho (≤ 12) e existência no cadastro.
2. **Ler item**
   - Aceitar qualquer código presente em `item_barcodes.code`.
3. **Informar quantidade**
   - Default = 1; teclado numérico; validações mínimas.
4. **Confirmar ação**
   - Entrada (+) grava `stock_movements(IN)`; Saída (−) grava `stock_movements(OUT)`.
   - Atualizar `stock_balances` (ou receber saldo atualizado pela API).

## Regras e validações (MVP)

- **Localização**: código ≤ 12; deve existir e estar ativa.
- **Item**: código deve existir e estar ativo.
- **Quantidade**: > 0.
- **Estoque negativo**: decisão de negócio no MVP
  - Opção A: bloquear `OUT` se saldo insuficiente
  - Opção B: permitir negativo e sinalizar no relatório
- **Auditoria**: toda movimentação registra `user_id`, data/hora, localização e item.

## Dados consumidos/enviados (API)

- **Leitura**
  - `warehouses`, `locations`, `items`, `item_barcodes`, `item_min_max`
  - `stock_balances` (para consulta/alertas)
- **Escrita**
  - `stock_movements` (linhas imutáveis)

## Observações de UX para PDA

- **Foco sempre previsível**: após cada Enter, avançar o passo e manter o foco no campo de scan.
- **Feedback imediato**: beep/visual ao reconhecer localização e item; erro claro ao não reconhecer.
- **Toques mínimos**: idealmente só tocar em quantidade e no botão (+/−).

