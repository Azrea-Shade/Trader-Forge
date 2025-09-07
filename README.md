# Trader Forge

[.NET 8 • Windows • Branch: testing/v1.0.0]

Trader Forge is a Windows desktop companion for investors. It focuses on fast research, watchlists, price alerts, daily briefings, and a clean neon-dark UI. This README documents the standard (non-AI) feature set for v1.

— Key Features (v1)
• Company lookup shortcuts: quick links to leadership, filings, and financials
• Watchlist & favorites: track last price and day change; copy-ticker utility
• Price alerts: set above/below thresholds per ticker; enable/disable anytime
• Daily briefing: generate 07:30, deliver 08:00 with quote, outlook, reminders
• Portfolio snapshot (starter): simple positions and allocation overview
• News feed (starter): aggregated finance headlines
• Settings & theming: dark theme, accent colors, currency inputs with “$” support
• Installer & autostart: one-click Windows install, optional autostart

Note: Desktop toast notifications are staged for reliability. CI currently uses a log-backed notifier; full toasts land after v1 stabilization.

— Tech Stack
• .NET 8 / WPF (MVVM), C#
• SQLite local storage
• xUnit tests
• GitHub Actions CI: unit tests must pass before build runs
• Branch model: main (backups), testing/v1.0.0 (active dev), docs (documentation)

— Getting Started (Developer)

1) Clone
   git clone git@github.com:Azrea-Shade/Trader-Forge.git
   cd Trader-Forge
   git checkout testing/v1.0.0

2) Restore & Build
   dotnet restore tests/Unit/Unit.csproj
   dotnet restore src/Cli/Cli.csproj
   dotnet restore src/App/App.csproj
   dotnet build src/App/App.csproj -c Release --no-restore

3) Run Tests
   dotnet test tests/Unit/Unit.csproj --collect:"XPlat Code Coverage" --logger "trx;LogFileName=test_results.trx" --results-directory artifacts/tests

Artifacts from CI are published on successful runs of the testing branch.

— Configuration
• Daily Brief times (defaults): Generate 07:30, Deliver 08:00
• Autostart: enabled by default after install (toggle in Settings)
• Currency inputs: “$” prefix supported in Alerts and Portfolio fields

— Screenshots
Place images under: docs/screenshots/
Suggested files: dashboard.png, watchlist.png, alerts.png, settings.png

— Repo Hygiene
• Backups: periodically copy testing/v1.0.0 → main to snapshot green phases
• Docs: nightly devlogs in docs/devlog named YYYY-MM-DD_phase-<range>.md

— Roadmap (high level)
• v1.0: core watchlist, alerts, daily brief, installer, theming, polish
• v1.1+: desktop toasts, richer portfolio dashboards, improved feeds

— Contributing
PRs welcome against testing/v1.0.0. Keep commits small and CI green. Open issues for bugs or feature requests.
