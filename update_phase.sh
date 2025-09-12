#!/usr/bin/env bash
set -euo pipefail
REPO="$HOME/Trader-Forge"
DOC_BRANCH="docs"
DOCFILE="PHASES-1-9_Debugging_Progress.md"
REMOTE="origin"

PHASE="${1:-}"
SHORT="${2:-}"
LONG="${3:-}"
[ -n "$PHASE" ] && [ -n "$SHORT" ] || { echo "Usage: $0 <phase 1-9> \"Short summary\" \"Optional long details\""; exit 2; }

cd "$REPO"
git fetch "$REMOTE" --prune || true
git checkout "$DOC_BRANCH"

python3 - "$PHASE" "$SHORT" "$LONG" <<'PY'
import sys,re,datetime
phase=sys.argv[1]; short=sys.argv[2]; long=sys.argv[3] if len(sys.argv)>3 else ""
doc="PHASES-1-9_Debugging_Progress.md"
t=open(doc,"r",encoding="utf-8").read()

# If this phase already logged, do nothing (idempotent)
if re.search(rf"^\-\s*\*Phase\s*{re.escape(phase)}\*", t, flags=re.M):
    open(doc,"w",encoding="utf-8").write(t)
    sys.exit(0)

# Tick one box in the emoji bar (first ⬜ -> 🟩)
t=t.replace("⬜","🟩",1)

# Bump Completed: X / 9
m=re.search(r"Completed:\s*(\d+)\s*/\s*9",t)
done=int(m.group(1)) if m else 0
done += 1
t=re.sub(r"Completed:\s*\d+\s*/\s*9", f"Completed: {done} / 9", t)

# Append Change Log entry at top
stamp=datetime.datetime.utcnow().strftime("%Y-%m-%d %H:%M UTC")
entry=f"- *Phase {phase}* — {short}  \n  _{stamp}_  \n  {long}\n"
t=re.sub(r"(## Change Log \(most recent first\)\n\n)", r"\1"+entry, t, 1)

open(doc,"w",encoding="utf-8").write(t)
PY

git add "$DOCFILE"
git commit -m "docs: mark Phase ${PHASE} complete — ${SHORT}" || true
git push "$REMOTE" "$DOC_BRANCH"
