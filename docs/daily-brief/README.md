# Daily Brief (Phase 9)

- Generate: 07:30 → `cli brief generate` (persists to SQLite `briefs`)
- Notify:   08:00 → `cli brief notify` (marks delivered; toast stubbed in CI)
- Latest brief stored in table: `briefs(date TEXT PK, generated_at, delivered_at, content TEXT)`.
