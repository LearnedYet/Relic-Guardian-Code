# Relic Guardian Code and Context Mirror

This public repository is the non-licensed inspection mirror for the local Unity project. It exists so ChatGPT and other code-review tools can read the current gameplay code, architecture decisions, learning rules, and selected text-based Unity configuration without receiving licensed art or animation packages.

## Current Mirror Checkpoint

- Mirror prepared from the local workspace on `2026-08-28`.
- Local full-project committed checkpoint: `0577974 Complete Guard lifecycle and soft recovery`.
- The mirror includes centralized Block/Attack/Jump arbitration, the stationary Guard Startup/Hold/Release lifecycle, attack cancellation cleanup, Guard presentation routing, Guard/Attack soft recovery, and the updated progress documents.
- Unity version: `6000.3.19f1`.
- Apply Root Motion remains disabled; player displacement is code-driven through `CharacterController`.

The current Guard lifecycle and soft-recovery checkpoint is runtime-verified. Guard Hold movement, Startup facing assistance, Perfect Guard, and Damage / Defense Resolution remain deferred. Exact current boundaries and the next single concept are documented in `Docs/CURRENT_STATE.md` and the final section of `Docs/HANDOFF.md`.

## Required Reading Order

When starting a new ChatGPT or Codex task, read these sources in order:

1. `AGENTS.md`
2. `Docs/CURRENT_STATE.md`
3. The actual code files relevant to the requested task
4. The final explicitly latest section at the end of `Docs/HANDOFF.md`
5. Newest relevant entries in `Docs/ROADMAP.md`, `Docs/DEV_LOG.md`, and `Docs/LEARNING_PROGRESS.md`

When historical documents conflict, actual code and `Docs/CURRENT_STATE.md` take priority. Earlier handoff checkpoints are archive material rather than current instructions.

## Repository Contents

- Root `*.cs`: mirrors of the project-owned scripts under `Assets/RelicGuardian/Player/Scripts/` and `Assets/RelicGuardian/Enemy/Scripts/`.
- `Docs/`: current state, architecture, roadmap, handoff, development log, learning progress, and learning rules.
- `AGENTS.md`: durable learner-first, architecture, Unity, documentation, and Git safety rules.
- `UnityConfig/Assets/RelicGuardian/Player/RelicGuardianPlayer.inputactions`: readable Input System configuration snapshot.
- `UnityConfig/Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`: project-owned Animator Controller snapshot. Locomotion/attack slots use project-owned placeholders where established; Guard states retain references to excluded local licensed Clips and therefore show unresolved motion GUIDs in this inspection-only mirror.
- `UnityConfig/Packages/manifest.json` and `UnityConfig/ProjectSettings/ProjectVersion.txt`: package and Unity-version context.

`UnityConfig/` is provided for inspection only. This mirror is not a complete Unity checkout and is not intended to be opened directly as a Unity project.

## Intentionally Excluded

- `Assets/LocalLicensed/` and `Assets/LocalLicensed.meta`.
- Licensed character models, textures, materials, animation FBXs, Animator Override assets, and the local `SwordAnimationPack` test assets.
- Mixed local Prefab and Scene working-copy changes that reference excluded licensed content.
- Unity-generated `.meta` files, `Library/`, `Temp/`, `Logs/`, build output, and editor cache files.
- Local Unity Editor state and runtime state that cannot be represented faithfully by source files.

Licensed animation filenames and their approved gameplay purposes may be documented for development context, but the licensed files themselves must never be uploaded.

## Workspace Boundary

The authoritative editable project remains the local full Unity workspace. This repository is a readable synchronization target with a separate Git history; it must not be merged or pulled directly into the full-project `main` without a separately verified synchronization procedure.
