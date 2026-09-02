---
name: relic-guardian-context
description: Restore and route minimal Relic Guardian project context when starting a new task, resuming after compaction or Handoff, or reconciling current project state. Do not use for unrelated repositories or an isolated follow-up that already has sufficient current context.
---

# Relic Guardian Context Bootstrap

Use this skill only for the Unity project rooted at `C:\Unity\Project\My project`.

## Restore the Minimum Current Context

1. Read `AGENTS.md` completely and preserve its learner-first and authorization boundaries.
2. Read `Docs/CURRENT_STATE.md`, `Docs/ARCHITECTURE.md`, and the current `Docs/HANDOFF.md` completely.
3. Run `git status --short --branch` and preserve every unrelated or protected local change.
4. Read `Docs/CONTEXT_INDEX.md` and select only the route matching the user's current task.
5. Inspect the routed actual code, Unity assets, or Git state before trusting documentation.
6. Report the understood state, any verification gap, the next single concept, and protected dirty files before changing gameplay behavior.

## Historical Retrieval

Do not read `Docs/DEV_LOG.md` or `Docs/Archive/` in full during ordinary startup. When history is genuinely needed, search first with `rg` for the relevant date, feature, class, method, error, or decision, then read only the matching section with enough surrounding context.

## Boundaries

- This workflow synchronizes context; it does not itself authorize gameplay edits, Unity asset changes, staging, commits, pushes, or external actions.
- Actual code, Unity assets, Editor state, and Git status outrank every document.
- `Docs/CONTEXT_INDEX.md` routes reading but is not a source of implementation truth.
- `Docs/ARCHITECTURE.md` records implemented architecture only. Treat feature-design documents as pending until actual code and runtime evidence confirm them.
- If the active task changes domains, return to `Docs/CONTEXT_INDEX.md` and load the new route instead of accumulating unrelated files.
