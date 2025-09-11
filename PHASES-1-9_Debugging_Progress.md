# PHASES 1–9 Debugging Progress ✅

**Repository:** Trader-Forge  
**Docs branch:** `docs` (documentation-only)  
**Debug branch:** `testing/1.0.0` — all debugging & CI fixes happen here  
**Release branch:** `main` — release-ready code only

---

## Overall progress
**Completed:** 0 / 9 phases

Visual progress (emoji):  
🟩 = completed • ⬜ = pending

⬜ ⬜ ⬜ ⬜ ⬜ ⬜ ⬜ ⬜ ⬜

ASCII progress bar:

[----------] 0% Complete

---

## Phase checklist with tasks & checkboxes

> We’ll tick items ✅ as we land fixes. When **all tasks in a phase are checked**, the phase will be marked complete and counted in the bar/percent.

### Phase 1 — Repo & CI cleanup (consolidate workflows; keep one canonical CI)
- [x] Create `docs` branch and seed with progress tracker.
- [x] Gate unit tests temporarily by excluding `Phase2_*`, `Phase4_*`, `Phase5_*` in `tests/Unit/Unit.csproj` to keep CI green while we backfill code.
- [ ] Reduce GitHub Actions to a **single** workflow file that runs Windows build + unit tests.
- [ ] Verify the Actions UI shows **only the single** workflow (others disabled/removed).
- [ ] Ensure the single workflow passes on branch `testing/1.0.0`.

### Phase 2 — Reintroduce core engines (Briefing, Alert, Scheduler) to compile tests
- [ ] Add minimal interfaces/impls for `BriefingEngine`, `AlertEngine`, `SchedulerCore`.
- [ ] Fix deconstruction/typing in tests (e.g., `gen`, `noti` variables).
- [ ] Remove test excludes for Phase2 and get green compile.

### Phase 3 — Shell smoke tests & artifacts
- [ ] Stabilize “Shell smoke tests”.
- [ ] Enable artifact publishing in CI (Windows build outputs).

### Phase 4 — Alert evaluation + watchlist
- [ ] Restore/repair `AlertEngine` usage in tests.
- [ ] Replace `double.HasValue` misuse with nullable/option handling.
- [ ] Re-enable `Phase4_*` tests and go green.

### Phase 5 — Portfolio service wiring
- [ ] Fix `ServiceFactory` → `PortfolioService` constructor signature mismatch.
- [ ] Re-enable `Phase5_*` tests and go green.

### Phase 6 — Integration services & DI
- [ ] Wire DI for integration services; ensure compile + basic tests.

### Phase 7 — Presentation layer
- [ ] Presentation smoke tests; unblock any Windows-specific paths.

### Phase 8 — CI end-to-end & release prep
- [ ] Full CI pass, artifacts retained; tag & packaging workflow verified.

### Phase 9 — Final regression & coverage
- [ ] Sweep remaining issues; raise/record coverage; document release notes.

---

## Running notes (what/why/how)

- **Temporary test gating**: We excluded Phase2/4/5 unit files to unblock CI while we restore missing types (engines) and fix constructor/typing issues seen in logs (`BriefingEngine`, `AlertEngine`, `SchedulerCore`, `ServiceFactory` → `PortfolioService`).  
- **CI consolidation**: Target state is **one** GitHub Actions workflow that builds on Windows and runs unit tests. Historical workflows may still appear in the UI until old YAMLs are removed and the branch is force-pushed.

---

## Change Log (most recent first)

- *(TBD — will populate as each fix lands; entries include the phase, summary, why, and what changed.)*

