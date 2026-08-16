# Relic Guardian Code Mirror

This repository mirrors the current non-licensed C# source and project documentation from the local Unity project so ChatGPT can inspect the latest implementation and learning context.

## Current Source Checkpoint

- Unity project commit: `f40be17 Archive player lunge and enemy chase foundation`
- Unity version: `6000.3.19f1`
- Exact resume point: read `Docs/HANDOFF.md`, then continue `EnemyMovement.cs` one concept at a time by adding `rotationSpeed` before smooth movement-facing.

## Contents

- Root `*.cs`: current scripts from `Assets/RelicGuardian/Player/Scripts/` and `Assets/RelicGuardian/Enemy/Scripts/`.
- `Docs/`: current development log, roadmap, handoff, learning progress, learning tracker, project plan, and development rules.

## Intentionally Excluded

- Licensed models, textures, materials, animation FBXs, and local Animator Controllers.
- Unity scenes, Prefabs, generated `.meta` files, packages, and project cache files.
- Local-only visual references that cannot resolve without the licensed asset packages.

The authoritative editable Unity workspace remains `C:\Unity\Project\My project`. This repository is a readable synchronization target, not a second Unity project.
