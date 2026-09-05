# Relic Guardian Workspace Instructions

## Scope

These instructions apply to the Unity project rooted at `C:\Unity\Project\My project`.

Communicate with the learner in Chinese by default. Keep C#, Unity API, class, method, variable, asset, and file names in English.

## Source of Truth

When information conflicts, use this priority:

1. Actual code, Unity assets, current Editor state, and current Git status.
2. `Docs/CURRENT_STATE.md`.
3. `Docs/ARCHITECTURE.md` for currently implemented ownership and data flow.
4. The active feature-design document selected through `Docs/CONTEXT_INDEX.md` for approved but unimplemented direction.
5. The current `Docs/HANDOFF.md`.
6. The newest relevant sections of `Docs/ROADMAP.md`, `Docs/DEV_LOG.md`, and `Docs/LEARNING_PROGRESS.md`.
7. Archived Handoffs, older checkpoints, and older plans.

`Docs/CONTEXT_INDEX.md` is a read router, not a source of implementation truth. A feature-design document may constrain future work but never proves that its proposed behavior is implemented or runtime-verified.

Historical documentation may describe work that was later superseded. Never treat an earlier checkpoint as current merely because it appears first in a file.

At the start of a new task or after context compaction:

1. Read this file completely.
2. Read `Docs/CURRENT_STATE.md` completely.
3. Read `Docs/ARCHITECTURE.md` and the current `Docs/HANDOFF.md` completely.
4. Run `git status --short --branch`.
5. Read `Docs/CONTEXT_INDEX.md`, select only the route matching the current task, and inspect those actual files.
6. Report the understood state, verification gaps, next single concept, and protected dirty files before changing gameplay behavior.

Do not read `Docs/DEV_LOG.md` or `Docs/Archive/` in full during ordinary startup. Search history first with `rg`, then read only the matching section when a real conflict or historical question requires it.

## Learner-First Development Rules

- Teach and implement one new concept at a time.
- The learner writes key gameplay and presentation code first unless they explicitly ask Codex to implement or take over.
- A correct prediction, explanation, fill-in answer, or messages such as `继续`, `嗯`, `好`, `好了`, or `正确` are not authorization to write key gameplay or presentation code.
- Before a key gameplay- or presentation-code edit, apply the already-established author default. If no author default has been established yet, explicitly establish who will type it.
- Once the learner-as-author default is established for this project, preserve it across bounded feature changes, context compaction, Handoffs, and new conversations. At the start of a new feature, state that the learner remains the author when useful; do not ask again. Reconfirm only when the learner explicitly proposes Codex takeover or authorship is genuinely ambiguous because of a direct implementation request.
- After the learner edits a file, inspect the actual file and review that implementation before continuing.
- Codex may directly fix only unambiguous formatting, indentation, line breaks, and obvious spelling mistakes. This permission never includes behavioral, logical, structural, or architectural changes.
- When Codex directly fixes an unambiguous spelling mistake, briefly report the exact correction. If a name has more than one plausible intended meaning or the correction could change behavior, stop and ask instead of treating it as a typo.
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

## Multi-Agent Delegation Rules

- Multi-Agent is opt-in by task shape. Codex may use subagents only when the task contains independent, bounded workstreams whose parallel investigation or independent review would materially improve speed, coverage, or main-thread context quality.
- Do not use subagents for learner-first code entry, small or strictly sequential edits, immediate learner-file review, or continuous Unity Scene, Prefab, Animator, or Animation Event configuration.
- For a new task or after compaction, the root agent must complete the core `relic-guardian-context` bootstrap itself before delegating task-specific investigation. Do not assign multiple agents to reread the complete startup context.
- Investigation subagents are read-only by default. They must not edit files, mutate Unity Editor state, stage Git changes, commit, push, or copy licensed assets unless the learner explicitly authorizes that exact delegated action.
- Use at most one subagent for an ordinary complex task and two to three only for genuinely independent cross-module work. Avoid recursive delegation unless the concrete task requires it.
- Default delegated context to `fork_turns="none"` and provide a bounded task packet. Use limited recent turns only when required; do not default to the full parent history.
- Keep one writer. During learner-first work, the learner remains the writer of key code. After explicit Codex takeover, only the root or one designated writer may modify a given scope.
- The root agent owns architecture decisions, conflict resolution, final review, and verification against actual files, build output, Unity Console, Runtime evidence, and current Git state.
- Unity MCP instance selection and shared mutable Unity work remain root-owned unless one subagent receives an explicit exclusive instance and mutation scope.
- Use `.agents/skills/relic-guardian-multi-agent/SKILL.md` for the full delegation workflow.

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
- Browser access to GitHub does not prove that command-line Git can reach GitHub. Before a remote operation, inspect the current Windows proxy and Git connectivity. When the browser uses a local proxy, pass that proxy only to the current `fetch` or `push` process unless the learner explicitly approves a persistent Git configuration change; do not hard-code a volatile proxy port as a durable project value.
- Before every GitHub mirror push, run `git fetch origin --prune`, base a separate clean mirror worktree or branch on the latest `origin/main`, and inspect remote-only commits. Never force-push over an advanced remote.
- Resolve mirror conflicts using the actual current project-owned source and documents as authoritative while preserving unrelated remote-only files. Keep the full Unity history and the flattened GitHub mirror history separate.
- After a reported successful push, verify `refs/heads/main` with `git ls-remote origin refs/heads/main`; absence of a command error alone is not proof that the remote moved.
- Never commit, push, rewrite history, or alter remote state without explicit authorization for the exact staged scope or commit.

## Documentation Maintenance

- Keep this file limited to durable rules. Do not add volatile next-step details here.
- Keep `Docs/CURRENT_STATE.md` short and replace outdated current-state text instead of appending an endless history.
- Keep `Docs/ARCHITECTURE.md` limited to implemented architecture. Keep approved but unimplemented direction in focused feature-design documents.
- Keep `Docs/CONTEXT_INDEX.md` limited to task-to-file routing. Do not duplicate architecture or feature design there.
- Keep `Docs/HANDOFF.md` limited to the latest cross-conversation Handoff and replace it at the next Handoff boundary.
- Preserve prior Handoffs under `Docs/Archive/`. Keep `Docs/DEV_LOG.md` as the chronological development archive; neither is default startup context.
- After a runtime-verified milestone, update the relevant current-state, roadmap, development-log, and learning-progress records before a focused commit.
- Never record an untested behavior as runtime-verified.
