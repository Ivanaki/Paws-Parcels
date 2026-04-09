from __future__ import annotations

import argparse
import csv
import json
import statistics
from collections import Counter
from dataclasses import asdict, dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Any


@dataclass
class NormalizedSession:
    session_id: str
    session_status: str
    start_scene_name: str
    started_at_utc: str
    ended_at_utc: str | None
    playtime_seconds: float
    playtime_minutes: float
    completed_quests_count: int
    completed_quest_ids: list[str]
    build_version: str
    product_name: str
    platform: str
    end_reason: str | None
    source_file: str


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description=(
            "Parse gameplay session metric JSON files and export "
            "analysis-ready CSV/JSON reports."
        )
    )
    parser.add_argument(
        "--input",
        required=True,
        help="Directory with session JSON files from Application.persistentDataPath/Metrics/Sessions.",
    )
    parser.add_argument(
        "--output",
        required=True,
        help="Directory where parsed tables and summary files will be written.",
    )
    parser.add_argument(
        "--include-in-progress",
        action="store_true",
        help="Include unfinished sessions in the final dataset.",
    )
    return parser.parse_args()


def parse_iso8601(value: Any) -> datetime | None:
    if value in (None, ""):
        return None

    text = str(value)
    if text.endswith("Z"):
        text = text[:-1] + "+00:00"

    return datetime.fromisoformat(text)


def safe_float(value: Any, default: float = 0.0) -> float:
    if value in (None, ""):
        return default

    return float(value)


def normalize_session(
    raw: dict[str, Any],
    source_path: Path,
    include_in_progress: bool,
) -> NormalizedSession | None:
    session_status = str(raw.get("SessionStatus") or raw.get("sessionStatus") or "unknown")

    if session_status != "completed" and not include_in_progress:
        return None

    started_at_raw = raw.get("StartedAtUtc") or raw.get("startedAtUtc")
    ended_at_raw = raw.get("EndedAtUtc") or raw.get("endedAtUtc")
    started_at = parse_iso8601(started_at_raw)
    ended_at = parse_iso8601(ended_at_raw)

    if started_at is None:
        raise ValueError("Missing StartedAtUtc")

    playtime_value = raw.get("PlaytimeSeconds")
    if playtime_value is None:
        playtime_value = raw.get("playtimeSeconds")

    playtime_seconds = safe_float(playtime_value, default=-1.0)
    if playtime_seconds < 0 and ended_at is not None:
        playtime_seconds = (ended_at - started_at).total_seconds()
    if playtime_seconds < 0:
        playtime_seconds = 0.0

    completed_quest_ids_raw = raw.get("CompletedQuestIds")
    if completed_quest_ids_raw is None:
        completed_quest_ids_raw = raw.get("completedQuestIds", [])

    if not isinstance(completed_quest_ids_raw, list):
        raise ValueError("CompletedQuestIds must be a list")

    completed_quest_ids = [str(value) for value in completed_quest_ids_raw]

    completed_count_raw = raw.get("CompletedQuestsCount")
    if completed_count_raw is None:
        completed_count_raw = raw.get("completedQuestsCount")
    completed_quests_count = int(completed_count_raw) if completed_count_raw is not None else len(completed_quest_ids)

    return NormalizedSession(
        session_id=str(raw.get("SessionId") or raw.get("sessionId") or source_path.stem),
        session_status=session_status,
        start_scene_name=str(raw.get("StartSceneName") or raw.get("startSceneName") or ""),
        started_at_utc=started_at.astimezone(timezone.utc).isoformat().replace("+00:00", "Z"),
        ended_at_utc=ended_at.astimezone(timezone.utc).isoformat().replace("+00:00", "Z") if ended_at else None,
        playtime_seconds=round(playtime_seconds, 2),
        playtime_minutes=round(playtime_seconds / 60.0, 2),
        completed_quests_count=completed_quests_count,
        completed_quest_ids=completed_quest_ids,
        build_version=str(raw.get("BuildVersion") or raw.get("buildVersion") or ""),
        product_name=str(raw.get("ProductName") or raw.get("productName") or ""),
        platform=str(raw.get("Platform") or raw.get("platform") or ""),
        end_reason=(raw.get("EndReason") or raw.get("endReason")),
        source_file=str(source_path.resolve()),
    )


def load_sessions(input_dir: Path, include_in_progress: bool) -> tuple[list[NormalizedSession], list[str]]:
    if not input_dir.exists():
        raise FileNotFoundError(f"Input directory does not exist: {input_dir}")

    sessions: list[NormalizedSession] = []
    skipped_files: list[str] = []

    for file_path in sorted(input_dir.glob("*.json")):
        try:
            with file_path.open("r", encoding="utf-8-sig") as source:
                raw = json.load(source)

            normalized = normalize_session(raw, file_path, include_in_progress)
            if normalized is None:
                skipped_files.append(f"{file_path.name}: skipped unfinished session")
                continue

            sessions.append(normalized)
        except Exception as error:
            skipped_files.append(f"{file_path.name}: {error}")

    return sessions, skipped_files


def build_summary(
    sessions: list[NormalizedSession],
    skipped_files: list[str],
    source_directory: Path,
) -> dict[str, Any]:
    playtime_values = [session.playtime_seconds for session in sessions]
    quest_values = [session.completed_quests_count for session in sessions]

    if playtime_values:
        median_playtime_seconds = round(statistics.median(playtime_values), 2)
        average_playtime_seconds = round(statistics.fmean(playtime_values), 2)
        total_playtime_seconds = round(sum(playtime_values), 2)
    else:
        median_playtime_seconds = 0.0
        average_playtime_seconds = 0.0
        total_playtime_seconds = 0.0

    if quest_values:
        median_completed_quests = round(statistics.median(quest_values), 2)
        average_completed_quests = round(statistics.fmean(quest_values), 2)
        total_completed_quests = int(sum(quest_values))
        max_completed_quests = int(max(quest_values))
    else:
        median_completed_quests = 0.0
        average_completed_quests = 0.0
        total_completed_quests = 0
        max_completed_quests = 0

    return {
        "parserGeneratedAtUtc": datetime.now(timezone.utc).isoformat().replace("+00:00", "Z"),
        "sourceDirectory": str(source_directory.resolve()),
        "sessionsCount": len(sessions),
        "medianPlaytimeSeconds": median_playtime_seconds,
        "medianPlaytimeMinutes": round(median_playtime_seconds / 60.0, 2),
        "averagePlaytimeSeconds": average_playtime_seconds,
        "averagePlaytimeMinutes": round(average_playtime_seconds / 60.0, 2),
        "totalPlaytimeSeconds": total_playtime_seconds,
        "totalPlaytimeMinutes": round(total_playtime_seconds / 60.0, 2),
        "medianCompletedQuests": median_completed_quests,
        "averageCompletedQuests": average_completed_quests,
        "totalCompletedQuests": total_completed_quests,
        "maxCompletedQuestsInSession": max_completed_quests,
        "sessionsWithoutCompletedQuests": sum(1 for value in quest_values if value == 0),
        "skippedFiles": skipped_files,
    }


def write_sessions_table(output_dir: Path, sessions: list[NormalizedSession]) -> Path:
    output_path = output_dir / "sessions_table.csv"
    fieldnames = [
        "session_id",
        "session_status",
        "start_scene_name",
        "started_at_utc",
        "ended_at_utc",
        "playtime_seconds",
        "playtime_minutes",
        "completed_quests_count",
        "completed_quest_ids",
        "build_version",
        "product_name",
        "platform",
        "end_reason",
        "source_file",
    ]

    with output_path.open("w", encoding="utf-8", newline="") as destination:
        writer = csv.DictWriter(destination, fieldnames=fieldnames)
        writer.writeheader()
        for session in sessions:
            row = asdict(session)
            row["completed_quest_ids"] = "|".join(session.completed_quest_ids)
            writer.writerow(row)

    return output_path


def write_summary(output_dir: Path, summary: dict[str, Any]) -> Path:
    output_path = output_dir / "summary.json"
    with output_path.open("w", encoding="utf-8") as destination:
        json.dump(summary, destination, ensure_ascii=False, indent=2)
    return output_path


def write_quest_frequency(output_dir: Path, sessions: list[NormalizedSession]) -> Path:
    output_path = output_dir / "quest_frequency.csv"
    quest_counter: Counter[str] = Counter()

    for session in sessions:
        quest_counter.update(session.completed_quest_ids)

    with output_path.open("w", encoding="utf-8", newline="") as destination:
        writer = csv.DictWriter(destination, fieldnames=["quest_id", "completed_sessions_count"])
        writer.writeheader()
        for quest_id, count in sorted(quest_counter.items()):
            writer.writerow(
                {
                    "quest_id": quest_id,
                    "completed_sessions_count": count,
                }
            )

    return output_path


def main() -> int:
    args = parse_args()
    input_dir = Path(args.input)
    output_dir = Path(args.output)
    output_dir.mkdir(parents=True, exist_ok=True)

    sessions, skipped_files = load_sessions(input_dir, include_in_progress=args.include_in_progress)
    summary = build_summary(sessions, skipped_files, input_dir)

    sessions_table_path = write_sessions_table(output_dir, sessions)
    summary_path = write_summary(output_dir, summary)
    quest_frequency_path = write_quest_frequency(output_dir, sessions)

    print(f"Completed sessions: {summary['sessionsCount']}")
    print(f"Median playtime: {summary['medianPlaytimeSeconds']} sec ({summary['medianPlaytimeMinutes']} min)")
    print(f"Average playtime: {summary['averagePlaytimeSeconds']} sec ({summary['averagePlaytimeMinutes']} min)")
    print(f"Median completed quests: {summary['medianCompletedQuests']}")
    print(f"Average completed quests: {summary['averageCompletedQuests']}")
    print(f"Sessions without completed quests: {summary['sessionsWithoutCompletedQuests']}")
    print(f"Sessions table: {sessions_table_path.resolve()}")
    print(f"Quest frequency: {quest_frequency_path.resolve()}")
    print(f"Summary file: {summary_path.resolve()}")

    if skipped_files:
        print("Skipped files:")
        for item in skipped_files:
            print(f"  - {item}")

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
