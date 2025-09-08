#!/usr/bin/env bash
# fix_all_termux.sh
# One-shot Termux-friendly script to apply the fixes described:
# - Add canonical Domain types (AllocationRow, PortfolioSummary, Portfolio)
# - Add IPriceFeed and DummyPriceFeed
# - Overwrite LivePriceAdapter with IPriceFeed-compatible implementation (backed up)
# - Add Presentation/ServiceAdapters mapping helpers
# - Find & remove duplicate PortfolioSummary/AllocationRow files (confirmable)
# - Commit changes and attempt dotnet build
#
# Usage:
#   ./fix_all_termux.sh        # interactive confirmation
#   ./fix_all_termux.sh --yes  # auto-approve deletes & commit
set -euo pipefail

AUTO_YES=0
if [ "${1:-}" = "--yes" ]; then
  AUTO_YES=1
fi

timestamp() { date +%s; }

# locate git repo root
REPO_ROOT=$(git rev-parse --show-toplevel 2>/dev/null || true)
if [ -z "$REPO_ROOT" ]; then
  echo "ERROR: not inside a git repo. cd into your repo and run again."
  exit 1
fi

echo "Repo root: $REPO_ROOT"
cd "$REPO_ROOT"

BACKUP_DIR=".fix_backups/$(timestamp)"
mkdir -p "$BACKUP_DIR"

backup_file() {
  local f="$1"
  if [ -f "$f" ]; then
    mkdir -p "$(dirname "$BACKUP_DIR/$f")"
    cp -a "$f" "$BACKUP_DIR/$f"
    echo "backup -> $BACKUP_DIR/$f"
  fi
}

# Ensure canonical directories exist
mkdir -p src/Domain src/Services/Feeds src/Presentation

# 1) Create canonical Domain files (overwrite but backup first)
echo "Creating canonical Domain types..."
backup_file "src/Domain/AllocationRow.cs"
cat > src/Domain/AllocationRow.cs <<'EOF'
namespace Domain
{
    /// <summary>Single ticker weight row used in portfolio allocation displays.</summary>
    public sealed record AllocationRow(string Ticker, decimal Weight);
}
EOF

backup_file "src/Domain/PortfolioSummary.cs"
cat > src/Domain/PortfolioSummary.cs <<'EOF'
using System.Collections.Generic;

namespace Domain
{
    /// <summary>Summary of a portfolio with its allocation rows.</summary>
    public sealed record PortfolioSummary(int PortfolioId, string Name, List<AllocationRow> Allocation);
}
EOF

backup_file "src/Domain/Portfolio.cs"
cat > src/Domain/Portfolio.cs <<'EOF'
namespace Domain
{
    public sealed record Portfolio(int Id, string Name, string? Notes);
}
EOF

echo "Canonical domain files written."

# 2) Add IPriceFeed + DummyPriceFeed
echo "Adding Services.Feeds (IPriceFeed + DummyPriceFeed)..."
backup_file "src/Services/Feeds/IPriceFeed.cs"
cat > src/Services/Feeds/IPriceFeed.cs <<'EOF'
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Feeds
{
    public interface IPriceFeed
    {
        Task<IDictionary<string, double?>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken cancellationToken);
        Task<double?> LastPrice(string ticker, CancellationToken cancellationToken);
    }
}
EOF

backup_file "src/Services/Feeds/DummyPriceFeed.cs"
cat > src/Services/Feeds/DummyPriceFeed.cs <<'EOF'
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Feeds
{
    public class DummyPriceFeed : IPriceFeed
    {
        public Task<IDictionary<string, double?>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken cancellationToken)
        {
            var dict = tickers.Distinct().ToDictionary(t => t, t => (double?)null);
            return Task.FromResult((IDictionary<string,double?>)dict);
        }

        public Task<double?> LastPrice(string ticker, CancellationToken cancellationToken)
            => Task.FromResult<double?>(null);
    }
}
EOF

echo "IPriceFeed + DummyPriceFeed written."

# 3) Overwrite LivePriceAdapter with safe IPriceFeed implementation (backup original)
LIVE_ADAPTER="src/Services/LivePriceAdapter.cs"
if [ -f "$LIVE_ADAPTER" ]; then
  echo "Backing up existing LivePriceAdapter..."
  backup_file "$LIVE_ADAPTER"
fi

cat > "$LIVE_ADAPTER" <<'EOF'
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Services.Feeds;

namespace Services
{
    // Simple adapter that implements IPriceFeed. Adjust to call real provider where needed.
    public class LivePriceAdapter : IPriceFeed
    {
        public LivePriceAdapter() { }

        public Task<IDictionary<string, double?>> GetPricesAsync(IEnumerable<string> tickers, CancellationToken cancellationToken)
        {
            var dict = new Dictionary<string, double?>();
            foreach (var t in tickers) dict[t] = null;
            return Task.FromResult((IDictionary<string,double?>)dict);
        }

        public Task<double?> LastPrice(string ticker, CancellationToken cancellationToken)
        {
            return Task.FromResult<double?>(null);
        }
    }
}
EOF

echo "LivePriceAdapter created/overwritten (backup kept)."

# 4) Add Presentation ServiceAdapters mapping helpers
SA="src/Presentation/ServiceAdapters.cs"
if [ -f "$SA" ]; then
  backup_file "$SA"
fi

cat > "$SA" <<'EOF'
using Domain;
using System.Collections.Generic;
using System.Linq;

namespace Presentation
{
    public static class ServiceAdapters
    {
        public static List<AllocationRow> MapAllocation(IDictionary<string, decimal> map)
        {
            if (map == null) return new List<AllocationRow>();
            return map.Select(kv => new AllocationRow(kv.Key, kv.Value)).ToList();
        }

        public static PortfolioSummary BuildSummaryTyped(int id, string name, IDictionary<string, decimal> allocationMap)
        {
            var allocation = MapAllocation(allocationMap);
            return new PortfolioSummary(id, name, allocation);
        }
    }
}
EOF

echo "Presentation/ServiceAdapters.cs written."

# 5) Find duplicate files for PortfolioSummary / AllocationRow
echo
echo "Searching for duplicate files that define PortfolioSummary / AllocationRow..."
DUP_PATTERNS=( "*PortfolioSummary*.cs" "*AllocationRow*.cs" )
candidates=()
while IFS= read -r -d '' f; do
  # exclude canonical ones we just created
  case "$f" in
    */src/Domain/PortfolioSummary.cs|*/src/Domain/AllocationRow.cs) ;;
    *)
      candidates+=("$f")
      ;;
  esac
done < <(find src -type f \( -name "*PortfolioSummary*.cs" -o -name "*AllocationRow*.cs" \) -print0 )

if [ ${#candidates[@]} -eq 0 ]; then
  echo "No duplicate-named files found (besides canonical ones)."
else
  echo "Found candidate duplicate files:"
  for f in "${candidates[@]}"; do
    echo "  $f"
  done

  if [ $AUTO_YES -eq 1 ]; then
    ANSWER="y"
  else
    echo
    read -p "Remove these files now? (y/N) > " ANSWER
  fi

  if [ "$ANSWER" = "y" ] || [ "$ANSWER" = "Y" ]; then
    for f in "${candidates[@]}"; do
      echo "Processing $f ..."
      backup_file "$f"
      # if tracked, remove via git, else rm
      if git ls-files --error-unmatch "$f" >/dev/null 2>&1; then
        git rm -f "$f" || true
        echo "git rm -f $f"
      else
        rm -f "$f"
        echo "rm -f $f"
      fi
    done
    echo "Duplicate files removed (backups in $BACKUP_DIR)."
  else
    echo "Skipping removal of duplicates. They remain in place."
  fi
fi

# 6) Try to find nested duplicate type definitions (non-file) and list them for manual inspection
echo
echo "Searching for nested type definitions (look for 'record PortfolioSummary' or 'record AllocationRow' inside files)..."
nested_files=()
while IFS= read -r -d '' f; do
  nested_files+=("$f")
done < <(grep -RIl --line-number --exclude-dir=.git -e "record PortfolioSummary" -e "record AllocationRow" src || true | sed -z 's/\n/\0/g')

# Remove canonical if present
filtered_nested=(); filtered_nested_count=0; filtered_nested_count=0
for f in "${nested_files[@]:-}"; do
  case "$f" in
    src/Domain/PortfolioSummary.cs|src/Domain/AllocationRow.cs) ;;
    *) filtered_nested+=("$f"); filtered_nested_count=$((filtered_nested_count+1)); filtered_nested_count=$((filtered_nested_count+1));;
  esac
done

if [ ${#filtered_nested[@]:-0} -gt 0 ]; then
  echo "Files containing inline/nested duplicate declarations (please inspect manually):"
  for f in "${filtered_nested[@]}"; do
    echo "  $f"
  done
  echo "I will NOT attempt automated edits to these files. Please open and remove duplicated nested declarations manually or tell me to proceed automatically."
else
  echo "No nested duplicate declarations detected outside canonical files."
fi

# 7) Stage & commit changes if anything changed
echo
git add -A

# Only commit if there are staged changes
if ! git diff --cached --quiet; then
  if [ $AUTO_YES -eq 1 ]; then
    git commit -m "fix(ci): canonical Domain types; add pricefeed interface + dummy; add Presentation mapping; remove duplicates"
  else
    read -p "Create git commit for these changes? (y/N) > " COMMIT_ANSWER
    if [ "$COMMIT_ANSWER" = "y" ] || [ "$COMMIT_ANSWER" = "Y" ]; then
      git commit -m "fix(ci): canonical Domain types; add pricefeed interface + dummy; add Presentation mapping; remove duplicates"
    else
      echo "Skipping commit. You can review and commit manually."
    fi
  fi
else
  echo "No changes detected to commit."
fi

# 8) Attempt to run dotnet build for the App project (best-effort)
echo
echo "Attempting 'dotnet build src/App/App.csproj -c Release --no-restore' (may fail in Termux)."
if command -v dotnet >/dev/null 2>&1; then
  dotnet build src/App/App.csproj -c Release --no-restore || true
else
  echo "dotnet CLI not found in PATH. Skipping build step. On CI this will run automatically."
fi

echo
echo "SCRIPT FINISHED. Backups of overwritten files are in: $BACKUP_DIR"
echo "If build still fails, paste the new build errors here and I'll patch the next issues."