#!/usr/bin/env python3
"""
record-stryker-result.py
------------------------
Parses Stryker.NET JSON report, reads thresholds from stryker-config.json,
writes a structured GitHub Step Summary, generates a metadata JSON artifact,
and creates a GitHub Commit Status ('stryker/mutation-gate').
"""

import sys
import os
import json
import glob
from datetime import datetime, timezone
import urllib.request
import urllib.error

# Ensure UTF-8 output across Windows and Linux runners
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")


def load_thresholds(config_path="stryker-config.json"):
    """Reads thresholds from stryker-config.json ensuring zero drift."""
    default_thresholds = {"high": 100, "low": 98, "break": 95}
    if os.path.exists(config_path):
        try:
            with open(config_path, "r", encoding="utf-8") as f:
                data = json.load(f)
                stryker_cfg = data.get("stryker-config", {})
                thresholds = stryker_cfg.get("thresholds", {})
                return {
                    "high": thresholds.get("high", default_thresholds["high"]),
                    "low": thresholds.get("low", default_thresholds["low"]),
                    "break": thresholds.get("break", default_thresholds["break"]),
                }
        except Exception as e:
            print(f"Warning: Could not parse {config_path}: {e}. Using defaults.", file=sys.stderr)
    return default_thresholds


def find_report_file(search_dir="StrykerOutput/ci"):
    """Finds the main Stryker JSON report in the given directory."""
    if os.path.isfile(search_dir) and search_dir.endswith(".json"):
        return search_dir
    patterns = [
        os.path.join(search_dir, "**", "mutation-report.json"),
        os.path.join(search_dir, "**", "stryker-report.json"),
        os.path.join(search_dir, "**", "*.json"),
    ]
    for pattern in patterns:
        for match in glob.glob(pattern, recursive=True):
            if not match.endswith(".html.json") and not match.endswith("stryker-metadata.json"):
                return match
    return None


def parse_stryker_report(report_path):
    """
    Parses Stryker mutation testing JSON report.
    Supports Stryker JSON schema v1, v2, and files/mutants tree.
    """
    with open(report_path, "r", encoding="utf-8") as f:
        data = json.load(f)

    top_score = data.get("mutationScore")
    
    files = data.get("files", {})
    total_mutants = 0
    killed_mutants = 0
    survived_mutants = 0

    if isinstance(files, dict):
        for file_info in files.values():
            mutants = file_info.get("mutants", [])
            if isinstance(mutants, list):
                for m in mutants:
                    status = str(m.get("status", "")).lower()
                    if status in ["killed", "timeout"]:
                        killed_mutants += 1
                        total_mutants += 1
                    elif status in ["survived", "nocoverage"]:
                        survived_mutants += 1
                        total_mutants += 1
                    elif status in ["compileerror", "ignored"]:
                        pass
            elif isinstance(mutants, dict):
                total_mutants += mutants.get("total", 0)
                killed_mutants += mutants.get("killed", 0)

    if top_score is not None:
        try:
            score = round(float(top_score), 2)
        except (ValueError, TypeError):
            score = round((killed_mutants / total_mutants * 100), 2) if total_mutants > 0 else 0.0
    else:
        score = round((killed_mutants / total_mutants * 100), 2) if total_mutants > 0 else 0.0

    return {
        "score": score,
        "total_mutants": total_mutants,
        "killed_mutants": killed_mutants,
        "survived_mutants": survived_mutants,
    }


def determine_status(score, thresholds):
    """
    Status mapping:
    - >= high (100%): ✅ HIGH
    - >= low && < high (98-99.9%): 🟡 LOW
    - >= break && < low (95-97.9%): 🟠 WARNING
    - < break (< 95%): ❌ FAILED
    """
    high = thresholds["high"]
    low = thresholds["low"]
    brk = thresholds["break"]

    if score >= high:
        return "✅ HIGH", "HIGH", True
    elif score >= low:
        return "🟡 LOW", "LOW", True
    elif score >= brk:
        return "🟠 WARNING", "WARNING", True
    else:
        return "❌ FAILED", "FAILED", False


def post_commit_status(commit_sha, state, description, target_url=None):
    """Posts GitHub Commit Status using GitHub REST API."""
    token = os.environ.get("GH_TOKEN") or os.environ.get("GITHUB_TOKEN")
    repo = os.environ.get("GITHUB_REPOSITORY")
    if not token or not repo or not commit_sha or commit_sha == "unknown":
        print("Note: GitHub token or repository context not found. Skipping commit status update.")
        return

    url = f"https://api.github.com/repos/{repo}/statuses/{commit_sha}"
    payload = {
        "state": state,
        "context": "stryker/mutation-gate",
        "description": description[:140],
    }
    if target_url:
        payload["target_url"] = target_url

    headers = {
        "Authorization": f"Bearer {token}",
        "Accept": "application/vnd.github+json",
        "User-Agent": "Stryker-Mutation-Gate",
        "Content-Type": "application/json",
    }

    try:
        req = urllib.request.Request(url, data=json.dumps(payload).encode("utf-8"), headers=headers, method="POST")
        with urllib.request.urlopen(req) as resp:
            if resp.status in (200, 201):
                print(f"Successfully posted commit status 'stryker/mutation-gate' = {state} to {commit_sha[:7]}")
            else:
                print(f"Failed to post commit status: HTTP {resp.status}")
    except Exception as e:
        print(f"Warning: Failed to post commit status to GitHub: {e}", file=sys.stderr)


def main():
    report_dir = sys.argv[1] if len(sys.argv) > 1 else "StrykerOutput/ci"
    config_file = sys.argv[2] if len(sys.argv) > 2 else "stryker-config.json"
    
    commit_sha = os.environ.get("COMMIT_SHA") or os.environ.get("GITHUB_SHA") or "unknown"
    run_id = os.environ.get("GITHUB_RUN_ID") or ""
    repo = os.environ.get("GITHUB_REPOSITORY") or ""
    run_url = f"https://github.com/{repo}/actions/runs/{run_id}" if repo and run_id else ""
    
    now_utc = datetime.now(timezone.utc)
    iso_date = now_utc.strftime("%Y-%m-%dT%H:%M:%SZ")
    readable_date = now_utc.strftime("%Y-%m-%d %H:%M:%S UTC")

    thresholds = load_thresholds(config_file)
    report_file = find_report_file(report_dir)

    if not report_file or not os.path.exists(report_file):
        print(f"Error: No Stryker JSON report found in '{report_dir}'", file=sys.stderr)
        summary = f"""
## ❌ Stryker Mutation Testing Results

| Metric | Value |
|--------|-------|
| Status | ❌ Report Not Found |
| Commit SHA | `{commit_sha}` |
| Execution Date | {readable_date} |

> [!CAUTION]
> Stryker report could not be found or the mutation run failed before generating reports.
"""
        step_summary_file = os.environ.get("GITHUB_STEP_SUMMARY")
        if step_summary_file:
            with open(step_summary_file, "a", encoding="utf-8") as gh:
                gh.write(summary)
        print(summary)
        post_commit_status(
            commit_sha=commit_sha,
            state="failure",
            description="Mutation testing failed: Report not found",
            target_url=run_url,
        )
        sys.exit(1)

    print(f"Parsing Stryker report: {report_file}")
    metrics = parse_stryker_report(report_file)
    score = metrics["score"]
    killed = metrics["killed_mutants"]
    total = metrics["total_mutants"]

    display_status, status_key, passed = determine_status(score, thresholds)

    summary = f"""
## Stryker Mutation Testing Results

| Metric | Value |
|--------|-------|
| **Mutation Score** | **{score}%** |
| Mutants Killed | {killed} / {total} |
| Total Mutants | {total} |
| Threshold High | ≥{thresholds['high']}% |
| Threshold Low | ≥{thresholds['low']}% |
| Threshold Break | ≥{thresholds['break']}% |
| **Status** | {display_status} |
| Commit SHA | `{commit_sha}` |
| Execution Date | {readable_date} |
"""
    print(summary)

    step_summary_file = os.environ.get("GITHUB_STEP_SUMMARY")
    if step_summary_file:
        with open(step_summary_file, "a", encoding="utf-8") as gh:
            gh.write(summary)

    metadata = {
        "commit_sha": commit_sha,
        "execution_date": iso_date,
        "readable_date": readable_date,
        "mutation_score": score,
        "mutants_killed": killed,
        "total_mutants": total,
        "threshold_high": thresholds["high"],
        "threshold_low": thresholds["low"],
        "threshold_break": thresholds["break"],
        "status": status_key,
        "display_status": display_status,
        "passed": passed,
        "workflow_run_id": run_id,
        "run_url": run_url,
    }

    metadata_path = os.path.join(os.path.dirname(report_file), "stryker-metadata.json")
    with open(metadata_path, "w", encoding="utf-8") as mf:
        json.dump(metadata, mf, indent=2)
    print(f"Saved mutation metadata to: {metadata_path}")

    root_metadata_path = "stryker-metadata.json"
    with open(root_metadata_path, "w", encoding="utf-8") as mf:
        json.dump(metadata, mf, indent=2)

    status_state = "success" if passed else "failure"
    description = f"Score: {score}% (Break: {thresholds['break']}%) | Killed: {killed}/{total}"
    post_commit_status(
        commit_sha=commit_sha,
        state=status_state,
        description=description,
        target_url=run_url,
    )

    if not passed:
        print(f"Stryker mutation quality gate failed: score {score}% is below break threshold {thresholds['break']}%", file=sys.stderr)
        sys.exit(1)
    else:
        print(f"Stryker mutation quality gate passed: score {score}% (Status: {status_key})")
        sys.exit(0)


if __name__ == "__main__":
    main()
