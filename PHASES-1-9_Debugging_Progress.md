# PHASES 1–9 Debugging Progress ✅

**Repository:** Trader-Forge  
**Docs branch:** `docs` (documentation-only)  
**Debug branch:** `testing/1.0.0` — all debugging & CI fixes happen here  
**Release branch:** `main` — release-ready code only

---

## Overall progress
**Completed:** 0 / 9 phases

**Legend:** 🟩 = complete • ⬜ = pending

🟩 ⚠ ️ ⚠ ️ ⚠️ ⬜ ⬜ ⬜ ⬜ ⬜

ASCII progress bar:  
[----------] 0% Complete

---

## Phase checklist (descriptions + task boxes)

> Check off items as we land fixes. When a whole phase is done, run
> `./update_phase.sh <n> "<short>" "<details>"` to tick the big bars.

### 1) Repo & CI cleanup
*Sub-progress:* [==========] 100%
- [ ] Collapse CI to a single canonical workflow (Windows build + unit tests)
- [ ] Disable/remove legacy/duplicate workflows
- [x] Keep only `main`, `testing/1.0.0`, and `docs` branches
- [x] Docs branch contains docs only
- [x] Verify CI green on testing branch

### 2) Core engines re-intro (Briefing, Alert, Scheduler) to compile tests
- [ ] Recreate minimal interfaces/impls so tests build
- [ ] Restore unit test includes (remove temporary excludes)

### 3) Shell smoke tests + artifact publishing
- [ ] Stabilize shell runner tests
- [ ] Publish build artifacts from CI run

### 4) Alert evaluation + watchlist wiring
- [ ] Implement/repair `AlertEngine`
- [ ] Fix types/usings for test compile & pass

### 5) Portfolio
- [ ] Fix `ServiceFactory` and `PortfolioService` constructor mismatch
- [ ] Re-enable Phase5 tests and make them pass

### 6) Integrations & DI wiring
- [ ] Wire services cleanly via DI
- [ ] Add missing contracts/mocks for tests

### 7) Presentation layer
- [ ] Smoke build of `Presentation` (net8.0-windows)
- [ ] UI glue & basic test harnesses

### 8) CI finalization
- [ ] Full end-to-end test pass on testing branch
- [ ] Artifact publishing + release prep checklist

### 9) Final regression & wrap-up
- [ ] Expand/green test coverage
- [ ] Final docs, ready to merge testing → main

---

## Change Log (most recent first)




### Phase 2 — Create Services.PortfolioService (2-arg ctor) so Presentation compiles — FAILURE
**Time (UTC):** 2025-09-11 20:51 UTC
**Conclusion:** failure
**Details:** Added src/Services/PortfolioService.cs in namespace Services with a two-argument constructor to satisfy Presentation and tests' ServiceFactory.

---

### Phase 2 — Add Phase2 method signatures + PortfolioService ctor — FAILURE
**Time (UTC):** 2025-09-11 20:48 UTC
**Conclusion:** failure
**Details:** Added SchedulerCore.NextTimes(tuple), AlertEngine.Evaluate(a,b), BriefingEngine.BuildBrief(a,b,c), and PortfolioService(object,object) constructor so Phase2 tests compile.

---

### Phase 2 — Reintroduced Phase2 stubs — FAILURE
**Time (UTC):** 2025-09-11 20:40 UTC
**Conclusion:** failure
**Details:** Adjusted BriefingEngine/AlertEngine/SchedulerCore signatures so Phase2 tests compile.

---

- 2025-09-11 20:10 UTC — Phase 1: Canonical workflow kept on testing & main; legacy YAMLs removed; docs branch has none.


- 2025-09-11 20:06 UTC — Phase 1 progress: created canonical CI (Windows build + unit tests), kept only main/testing/docs, made docs branch docs-only, CI run green on testing. Pending: hide/disable remaining legacy workflows in GitHub UI.


*(No phases completed yet — use `./update_phase.sh` to mark a phase complete.)*

---

## How to update when a phase is finished

Run from repo root (any branch):

- `./update_phase.sh 1 "Consolidated CI into single workflow" "Removed legacy YAMLs; kept CI-Single that runs Windows build+tests."`

This updates the emoji row, ASCII bar, “Completed: X / 9” count, and appends a Change Log entry to this document (on `docs`).

