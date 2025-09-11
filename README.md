<h1 align="center">🔥 Trader Forge</h1>
<p align="center">
  <em>Neon-dark Windows companion for quick market research, watchlists, alerts, and daily briefings.</em>
</p>

<p align="center">
  <a href="https://github.com/Azrea-Shade/Trader-Forge/actions">
    <img alt="CI" src="https://img.shields.io/github/actions/workflow/status/Azrea-Shade/Trader-Forge/ci.yml?label=CI&logo=github&style=for-the-badge">
  </a>
  <img alt=".NET" src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet">
  <img alt="Platform" src="https://img.shields.io/badge/Windows-WPF-00ADEF?style=for-the-badge&logo=windows">
</p>

— Why Trader Forge  
• ⚡ Speed first: jump to leadership, filings, financials, and key links without digging.  
• 👀 Stay on top: watchlist + favorites with last price and day change.  
• 🔔 Never miss it: price alerts (above/below) per ticker, enable/disable anytime.  
• 🗞️ Daily brief: generate 07:30 → deliver 08:00 with quote, outlook, and reminders.  
• 💼 Portfolio (starter): lightweight snapshot of positions and allocation.  
• 📰 News (starter): aggregated market headlines.  
• 🎛️ Settings & theme: neon-dark with accent colors; currency inputs accept “$”.  
• 🧩 Installer & autostart: frictionless setup with optional autostart.

Note: Desktop toast notifications ship after v1 stabilization; CI currently uses a log-backed notifier for reliability.

— Tech Stack  
• .NET 8 / WPF (MVVM), C#  
• SQLite local store  
• xUnit tests  
• GitHub Actions: tests must pass before build runs  
• Branch model: main (backups), testing/v1.0.0 (active dev), docs (documentation)


— Contributing  
PRs welcome against testing/v1.0.0. Keep changes small; keep CI green. Open issues for bugs/features.
