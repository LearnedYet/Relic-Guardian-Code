# Relic Guardian Workspace Instructions

## Scope

These instructions apply to the Unity project rooted at `C:\Unity\Project\My project`.

Communicate with the learner in Chinese by default. Keep C#, Unity API, class, method, variable, asset, and file names in English.

## Source of Truth

When information conflicts, use this priority:

1. Actual code, Unity assets, current Editor state, and current Git status.
2. `Docs/CURRENT_STATE.md`.
3. The final, explicitly latest section at the end of `Docs/HANDOFF.md`.
4. The newest relevant sections of `Docs/ROADMAP.md`, `Docs/DEV_LOG.md`, and `Docs/LEARNING_PROGRESS.md`.
5. Older checkpoints and plans.

Historical documentation may describe work that was later superseded. Never treat an earlier checkpoint as current merely because it appears first in a file.

At the start of a new task or after context compaction:

1. Read this file completely.
2. Read `Docs/CURRENT_STATE.md` completely.
3. Run `git status --short --branch`.
4. Inspect the actual files listed for the current task.
5. Report the understood state, verification gaps, next single concept, and protected dirty files before changing gameplay behavior.

## Learner-First Development Rules

- Teach and implement one new concept at a time.
- The learner writes key gameplay code first unless they explicitly ask Codex to implement or take over.
- A correct prediction, explanation, fill-in answer, or messages such as `继续`, `嗯`, `好`, `好了`, or `正确` are not authorization to write gameplay code.
- Before any key gameplay-code edit, explicitly establish who will type it.
- After the learner edits a file, inspect the actual file and review that implementation before continuing.
- Codex may directly fix only unambiguous formatting, indentation, line breaks, and obvious spelling mistakes. This permission never includes behavioral, logical, structural, or architectural changes.
- Repetitive, simple, low-risk Editor configuration for one already-explained concept may be handled as one batch. Do not batch separate gameplay decisions.
- Do not treat copying or following instructions once as proof that a concept is understood. Use a small prediction, explanation, correction, reconstruction, or runtime test as evidence.

## Required Explanation Before New Identifiers

Before asking the learner to create any new class, method, property, field, parameter, or local variable, explain:

1. What data or responsibility it represents.
2. Why the current feature needs it.
3. Whether it is a component field, property, parameter, or local variable.
4. Its lifetime and when its value changes or ends.
5. Its C# type and why that type fits.
6. Every English word in the proposed identifier, its Chinese meaning, and the complete naming reason.

Do not assume that understanding gameplay logic means the learner can independently choose variable scope or English names.

## Unity and Architecture Boundaries

- Unity version is `6000.3.19f1`; verify the installed package version and actual Editor UI before giving version-sensitive instructions.
- Keep Apply Root Motion disabled. Normal locomotion, Sprint, attack lunge, and future Dodge displacement are code-driven through `CharacterController`.
- `PlayerActionController` already exists and owns the coarse player action state. Do not propose recreating it.
- Lock-on is an orthogonal targeting/movement/camera mode, not a duplicate action system. Reuse shared Attack, Block, Dodge, and other action logic unless behavior genuinely differs.
- `PlayerMovement` currently owns player `CharacterController` displacement and facing application.
- `PlayerCombat` owns attack sequence state, target choice for attacks, windows, lunge requests, damage requests, and attack cleanup.
- `PlayerAnimator` owns presentation parameter writes and animation triggers, not gameplay permission decisions.
- `PlayerInputReader` records input values and one-use or held requests; it does not decide whether an action is allowed.
- Do not create `PlayerMotor` pre-emptively. Reassess the displacement boundary only when Dodge movement is actually connected and provides concrete evidence for a split.
- Before adding an action lasting across frames, define its owner, entry condition, finish boundary, movement/turning permissions, accepted/rejected inputs, and interruption rules.
- Keep action priority separate from authored cancellation permission. Do not make every attack recovery cancellable implicitly.
- When mutually exclusive action requests can arrive in the same frame, the accepted result must not depend on `MonoBehaviour.Update()` execution order. Add the smallest explicit deterministic arbitration rule when the concrete conflict is introduced; do not pre-emptively build a general numeric Priority system.
- Test visible behavior in Play Mode and check the Unity Console before declaring a gameplay checkpoint complete.

## Git and Licensed-Asset Safety

- `Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` are local-only and must never be committed or uploaded.
- Do not modify Starter Assets official source code.
- Preserve all existing user changes. Never reset, restore, overwrite, or broadly rewrite dirty Prefabs, Scenes, Animator assets, or other mixed local files.
- Never use broad staging commands such as `git add .` or `git add -A` in this project. Stage an explicit allowlist only.
- Before committing, inspect `git status --short`, `git diff --cached --name-only`, `git diff --cached --stat`, and unexpected file sizes.
- A tracked Prefab or Scene that references excluded licensed assets must remain unstaged unless an explicit reproducible replacement strategy is approved.
- Report the working tree honestly; intentional local-only modifications do not make it clean.
- The GitHub repository is code/document focused and does not have the same shape as the local full Unity-project history. Do not directly pull or merge its `main` into this full Unity workspace without first designing and verifying a safe synchronization path.
- Never commit, push, rewrite history, or alter remote state without explicit authorization for the exact staged scope or commit.

## Documentation Maintenance

- Keep this file limited to durable rules. Do not add volatile next-step details here.
- Keep `Docs/CURRENT_STATE.md` short and replace outdated current-state text instead of appending an endless history.
- Use `Docs/HANDOFF.md` and `Docs/DEV_LOG.md` for chronological archive material.
- After a runtime-verified milestone, update the relevant current-state, roadmap, development-log, and learning-progress records before a focused commit.
- Never record an untested behavior as runtime-verified.
