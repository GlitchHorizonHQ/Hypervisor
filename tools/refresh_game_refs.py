# Libraries
from __future__ import annotations
from pathlib import Path

import argparse
import hashlib
import os
import shutil
import sys
import time

# Default game path
## Normally the default path for the game unless it is installed on a different drive.
DEFAULT_GAME_PATH = os.path.join(
    os.environ.get("ProgramFiles(x86)", r"C:\Program Files (x86)"),
    "Steam",
    "steamapps",
    "common",
    "Data Center",
)

def _repo_root() -> Path:
    return Path(__file__).resolve().parent.parent

def _repo_melon_root(repo: Path) -> Path:
    return repo / "lib" / "references" / "MelonLoader"

def _copy_tree(src: Path, dst: Path, melon_root: Path) -> list[tuple[str, int, float]]:
    """Returns list of (relative posix path, size, mtime) for manifest."""
    dst.mkdir(parents=True, exist_ok=True)
    entries: list[tuple[str, int, float]] = []

    for p in sorted(src.glob("*.dll")):
        out = dst / p.name
        shutil.copy2(p, out)
        st = out.stat()
        rel = str(out.relative_to(melon_root)).replace("\\", "/")
        entries.append((rel, st.st_size, st.st_mtime))
    return entries

def sync_assemblies(game_dir: Path, repo: Path) -> Path:
    # Different file paths that MelonLoader use
    # We will be using these to sync the different assemblies
    melon = game_dir / "MelonLoader"
    il2_src = melon / "Il2CppAssemblies"
    net6_src = melon / "net6"

    if not il2_src.is_dir():
        print(f"Error: Il2CppAssemblies not found: {il2_src}", file=sys.stderr)
        print("Install MelonLoader and run the game once to generate interop DLLs", file=sys.stderr)
        print("https://melonloader.co/download.html", file=sys.stderr)
        sys.exit(1)
    if not net6_src.is_dir():
        print(f"Error: MelonLoader net6 folder not found: {net6_src}", file=sys.stderr)
        sys.exit(1)

    dest_root = _repo_melon_root(repo)
    dest_il2 = dest_root / "Il2CppAssemblies"
    dest_net = dest_root / "net6"

    print(f"Source game directory: {game_dir}")
    print(f"Copying Il2CppAssemblies -> {dest_il2}")

    e1 = _copy_tree(il2_src, dest_il2, dest_root)
    
    print(f"Copying net6 -> {dest_net}")

    e2 = _copy_tree(net6_src, dest_net, dest_root)

    ac = dest_il2 / "Assembly-CSharp.dll"
    if ac.is_file():
        h = hashlib.sha256(ac.read_bytes()).hexdigest()
        print(f"Assembly-CSharp.dll sha256={h}")

def _watch(game_dir: Path, repo: Path, interval: float) -> None:
    melon = game_dir / "MelonLoader" / "Il2CppAssemblies" / "Assembly-CSharp.dll"
    last: float | None = None

    print(f"Watching {melon} (poll every {interval}s). Ctrl+C to stop.")

    while True:
        try:
            mtime = melon.stat().st_mtime if melon.is_file() else None
        except OSError:
            mtime = None
        if mtime is not None and last is not None and mtime != last:
            print("Detected change; syncing...")
            sync_assemblies(game_dir, repo)
        elif mtime is not None and last is None:
            sync_assemblies(game_dir, repo)
        last = mtime
        time.sleep(interval)

def main() -> None:
    parser = argparse.ArgumentParser(description="Sync MelonLoader interop DLLs into lib/references.")
    parser.add_argument(
        "--game-dir",
        type=Path,
        default=None,
        help="Data Center game root (contains MelonLoader/).",
    )
    parser.add_argument("--watch", action="store_true", help="Poll Assembly-CSharp.dll for changes and re-sync.")
    parser.add_argument("--watch-interval", type=float, default=5.0, help="Seconds between polls (watch mode).")
    args = parser.parse_args()

    repo = _repo_root()
    raw = args.game_dir or os.environ.get("DATA_CENTER_GAME_DIR")
    game_dir = Path(raw) if raw else Path(DEFAULT_GAME_PATH)
    game_dir = game_dir.expanduser()

    if not game_dir.is_dir():
        print(f"Error: game directory does not exist: {game_dir}", file=sys.stderr)
        sys.exit(1)

    if args.watch:
        _watch(game_dir, repo, args.watch_interval)
    else:
        sync_assemblies(game_dir, repo)
        print("Successfully updated lib/references (MSBuild will prefer this MelonLoader tree when present).")

if __name__ == "__main__":
    main()