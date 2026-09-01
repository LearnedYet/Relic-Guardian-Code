# Relic Guardian Development Rules

## Project Environment

- Engine: Unity 6.3 LTS
- Render Pipeline: URP
- Language: C#
- Target Platform: Windows
- IDE: Visual Studio Code

---

## Learning Goal

This project is both a playable demo and a Unity learning project.

The goal is to understand every important system instead of copying generated code.

### Learning Verification Rule

Do not treat a concept as learned just because it has been explained or the learner says "okay".

Before moving to a dependent concept, verify understanding with more than one appropriate form of evidence, chosen from:

- Explaining the purpose in the learner's own words.
- Identifying what an existing line of code does.
- Completing or correcting a small code fragment.
- Predicting what happens for a simple input or condition.
- Testing the result in Unity when the concept affects runtime behaviour.

Keep each check small and supportive. If the learner is uncertain, explain again with a different example and record the topic as practising rather than moving ahead.

### Learning Session Rhythm

Use this rhythm for each small concept or feature step:

1. **Short recall**: ask the learner to predict, explain, or fill in a very small part before giving the answer.
2. **Small explanation**: introduce only the concept required for the current step.
3. **Small practice**: let the learner read, complete, correct, or write a short fragment.
4. **Unity verification**: when applicable, test the visible runtime result and check the Console.
5. **Spaced review**: revisit important concepts after one or more later steps without showing the answer first.
6. **Record observable progress**: update the learning record after verified practice.

Use the three learning stages **Seen**, **Practising**, and **Understood**. Do not require memorised definitions; the target is reliable, practical understanding. Limit each check to one or two concepts so learning remains manageable.

Verification questions must not reveal or strongly imply their own answers. Prefer applied choices, code comparison, debugging, prediction, and short reconstruction tasks. If a check is too leading to distinguish understanding from guessing, discard it and do not count it as learning evidence. Adjust difficulty upward when the learner answers reliably, while introducing only the concepts required for the current step.

Prefer realistic debugging exercises that provide faulty code and observable runtime or compiler symptoms. Ask the learner to identify the root cause and make the smallest correction, because this format best matches their learning preference and real Unity development work.

When the exercise is testing root-cause discovery, do not identify the faulty line, variable, API, or category in the prompt. Provide only the smallest useful code context and observable symptoms. Let the learner locate the fault before asking for a correction. If the prompt reveals the fault location, discard the exercise as mastery evidence.

### New Concept Gate

Before showing or asking the learner to write any code, check whether the step introduces a new:

- C# syntax or language feature;
- Unity API, property, method, or component usage;
- maths, coordinate-system, or gameplay-programming concept;
- combination of familiar elements that creates a meaningfully new pattern.

If any item is new, do not treat the step as routine implementation merely because the code is short. Use this sequence:

1. Name the new concept and identify its category.
2. Explain its purpose in the current feature and what observable problem it solves.
3. Introduce only one new concept at a time when practical.
4. Ask for a small prediction, explanation, or learner attempt before showing the completed code.
5. If the learner is blocked, give progressively stronger hints; show the answer only after the smaller hints are insufficient.
6. When copying is appropriate for first exposure, state explicitly that copying counts only as **Seen** or **Practising**, not **Understood**.
7. Verify understanding later with at least one non-leading reconstruction, prediction, debugging, or correction task in addition to any copied implementation.
8. When the concept changes runtime behaviour, verify it in Unity and check the Console before moving to a dependent concept.

Do not treat an explanation followed by successful copying as evidence of mastery. Do not bundle several new APIs or concepts into one code fragment merely to advance the feature faster.

### Learner Code-Edit Authorization Gate

- A correct prediction, explanation, or fill-in answer is learning evidence only. It does not mean that the learner has edited the project file, and it does not authorize Codex to write the complete implementation into the project.
- A request such as "continue" authorizes the next teaching step, not automatic code mutation.
- For key learning code, after the learner answers a prediction or scaffold, explicitly ask the learner to enter the code in the project and wait for them to report completion. Then inspect and review their actual file before editing it.
- Codex may write key learning code only when the learner explicitly asks Codex to implement or edit it, or when the learner has attempted it, remains blocked after progressive hints, and asks for direct help.
- Before any key-code mutation, state who will type the code. If that has not been established, do not call a file-editing or Unity script-mutation tool.
- Mechanical formatting or cleanup after the learner's implementation must remain behavior-preserving and must be described as such.
- If Codex violates this gate, stop further implementation, disclose exactly what was written, do not treat the learner as having authored or mastered it, and let the learner choose whether to keep it, revert it, or reconstruct it.

Recorded project pitfall: after the learner correctly answered phase predictions and supplied `HitWindow` and `Recovery` for a scaffold, Codex directly wrote the complete `OpenHitWindow()` and `CloseHitWindow()` methods, validated them, updated documentation, and committed `ff83756`. This confused understanding evidence with file-edit authorization and prioritized checkpoint completion over the learner-first workflow. This was a Codex process error, not ambiguity or a learner mistake.

### Editor Configuration Versus Code Learning

- Editor configuration may use direct click-and-drag instructions, but explain the responsibility of every important object, component, and reference.
- Code learning still follows the New Concept Gate even when the related editor setup was mechanical.
- When a step moves from editor configuration into code, explicitly switch back to the learning rhythm instead of continuing with tutorial-style copying.

---

## Communication Rules

- Communicate with me in Chinese by default.
- Keep code, class names, method names, variable names, Unity API names, and file names in English.
- Explain English technical terms in Chinese when they first appear.

### Identifier Translation Learning Rule

Whenever a code-writing step asks the learner to create or name a class, method, property, field, parameter, or local variable:

1. Before asking the learner to write it, provide a short Chinese naming glossary for every new identifier introduced in that step.
2. Split `camelCase` and `PascalCase` identifiers into their English words, translate each important word into Chinese, and then explain the complete intended meaning. For example, `currentAttackTarget` is `current`（当前）+ `attack`（攻击）+ `target`（目标）, meaning “当前这次攻击选中的目标”.
3. Briefly explain naming signals that carry code meaning when relevant, such as singular versus plural, `is`（是否）for a boolean state, `current`（当前）for the value currently in use, and method verbs such as `Find`（查找）, `Try`（尝试）, or `Consume`（消费并清除一次请求）.
4. Keep the identifier itself in English so the learner practises normal C# and Unity conventions; use Chinese only to support understanding.
5. Include the glossary again in a later exercise when the learner is being asked to reconstruct the name from memory. Do not assume that seeing the translation once means it has been learned.

### Requirement Clarification Rule

The learner is not required to express requests using technical or product-management language. A goal, concern, observed behaviour, or rough idea is enough.

When a request is ambiguous or could lead to different implementation choices:

1. Restate the understood goal in plain Chinese.
2. State any important assumption.
3. Ask at most one focused question when a decision is necessary; otherwise take the smallest safe next step.
4. Do not make broad project changes based only on an unconfirmed interpretation.

The learner may say: "我表达不清楚，你帮我整理". In that case, restate the request as goal, current situation, constraints, and next step for confirmation.

## Development Workflow

Before implementing a feature:

1. Analyze the requirement.
2. Point out missing or ambiguous information.
3. Explain the implementation idea.
4. Break the feature into small steps.
5. Implement only one step at a time.
6. Test the feature.
7. Update ROADMAP.md and DEV_LOG.md.

### Git Boundary for Licensed and Local-Only Assets

This repository is primarily a code-focused game-client portfolio. A Unity project still needs some authored scenes, Prefabs, settings, and `.meta` files to reproduce code behaviour, so "code-focused" does not mean that every non-`.cs` file is forbidden. Apply the following boundary instead:

1. Treat paid, licensed, or externally supplied art, models, textures, audio, animation packs, effects, and nested installer packages as **local-only by default**. Do not stage or commit them unless the learner explicitly approves that exact asset scope and its licence permits repository distribution.
2. Record every currently required local-only dependency in `Docs/CURRENT_STATE.md` and the current `Docs/HANDOFF.md` with its exact package name, version, expected import path, and restoration instructions. Preserve superseded dependency history under `Docs/Archive/`. Never imply that another checkout is visually reproducible when the required licensed dependency is absent.
3. Keep local-only asset folders and their root `.meta` files in `.gitignore`. Before adding a new ignore rule, confirm that the target is not already tracked.
4. Remember that `.gitignore` does not protect a file that Git already tracks. A tracked scene or Prefab containing local-only asset references must remain unstaged, or the integration must be moved to a deliberately ignored local-only scene/Prefab boundary before it can be considered repository-clean.
5. Do not commit a tracked Prefab or scene that references an excluded third-party asset, because another checkout would receive unresolved GUID references. Keep the previous reproducible tracked configuration unless an approved replacement strategy is available.
6. In a workspace containing local-only resources or tracked local overrides, do not use broad staging commands such as `git add -A` or `git add .`. Stage an explicit allowlist of intended paths.
7. Before every commit, inspect at least `git status --short`, `git diff --cached --name-only`, and `git diff --cached --stat`. Stop if the staged set contains an unapproved asset directory, archive, model, texture, audio file, animation pack, effect package, or local-only Prefab/scene override.
8. Check the staged set for unexpectedly large files before committing. File-size safety does not make an asset licence-safe: both repository size and distribution permission must be acceptable.
9. When a commit accidentally includes local-only assets and has not been pushed, remove only those paths from the Git index, keep the working files intact, and amend the local commit. Do not rewrite pushed history without explicit approval.
10. Report the working-tree state honestly. If a tracked Prefab remains modified only to support a local licensed model, describe that intentional local modification instead of claiming that the workspace is clean.

Current project decision: `Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` are the generic ignored boundary for local licensed dependencies. It currently contains P09 and the narrowed Powerful Sword animation subset, including the local Animator Override Controller. The P09, weapon, and override-controller references in `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab` are also local-only and must not be staged with code changes.

### GitHub Code/Document Mirror Submission Workflow

The local full Unity repository and `LearnedYet/Relic-Guardian-Code` are separate histories with different layouts. Use this workflow for every approved GitHub mirror submission:

1. Confirm explicit authorization for the exact commit and push scope. A request to continue development is not authorization to alter Git history or remote state.
2. In the full Unity repository, compile or run the appropriate checks, then stage an explicit allowlist. Keep protected mixed Prefabs, Scenes, `Assets/LocalLicensed/`, and licensed presentation assets unstaged.
3. Inspect `git status --short`, `git diff --cached --name-only`, `git diff --cached --stat`, and staged file sizes before creating the local full-project checkpoint.
4. Treat browser and command-line connectivity separately. Read the current Windows proxy configuration before GitHub operations. If the browser currently uses a local proxy, set `HTTP_PROXY` and `HTTPS_PROXY` only for the current Git command; do not permanently modify Git proxy configuration or hard-code the current port without explicit approval.
5. Run `git fetch origin --prune` before preparing the mirror. Base a separate clean mirror worktree or branch on the latest `origin/main`, not on the unrelated full-project `main` history.
6. Synchronize only the approved mirror allowlist: flattened project-owned C# files, maintained `Docs/`, and deliberately reproducible files under `UnityConfig/`. Preserve remote-only files that are outside the current sync scope.
7. Stage the mirror allowlist explicitly and repeat the cached-name, cached-stat, and file-size checks. Never use `git add .` or `git add -A`.
8. If the remote advanced, fetch it and integrate the approved mirror commit onto the new `origin/main`. Never use force push. Resolve overlapping mirror files from the actual current Unity workspace, while retaining unrelated remote additions.
9. Push normally to GitHub `main`, then run `git ls-remote origin refs/heads/main` and confirm that the returned hash matches the intended pushed commit.
10. Record the resulting local full-project checkpoint and confirmed GitHub mirror checkpoint in `CURRENT_STATE.md`. Keep chronological connection failures and conflict-resolution details in `DEV_LOG.md`, not in the durable short rules.

### Player Action Lifecycle and Conflict Check

Before implementing a player action that lasts across frames, such as Attack, Dodge, Skill, hit reaction, stun, or death:

1. Identify the single component that owns the current player action.
2. Define the exact condition that starts the action.
3. Define the exact condition or signal that ends the action.
4. List which existing behaviours continue, are blocked, or are modified while the action is active.
5. Define which actions may interrupt it and which requests are rejected or buffered.
6. Keep arbitration rules in one coordination boundary instead of adding mutual references between every feature component.
7. Verify both the visible behaviour and the Console in Unity before adding another action or transition.

For the first Basic Attack, keep the model deliberately small: only `Free` and `BasicAttack`. Do not implement Combo, Dodge cancellation, Skills, damage, enemies, or hit reactions as part of this checkpoint.

### Combat Detection and Target-Selection Requirement Check

Before turning an attack's detected contacts into a final hit result:

1. Define the detection shape and timing separately from the target-selection rule.
2. Explicitly confirm whether the attack selects the nearest valid target, every valid target, a locked target, or another stated policy.
3. Do not infer single-target or multi-target behaviour from words such as sphere, box, cone, fan, sweep, or melee range; those words describe geometry, not hit multiplicity.
4. Keep candidate detection, target selection, and later effect application as separate responsibilities so different attacks can reuse the geometry while choosing different targets.
5. When future attacks need different range, angle, or selection behaviour, prefer attack-specific configuration and a local policy change over copied detection code or scattered attack-stage checks.
6. Ask one focused question before implementing the final selection policy when the multiplicity or priority rule has not been stated.

Recorded project pitfall: describing the first Basic Attack as a small fan established its geometry but did not establish whether every target in the fan or only one target should be selected. Candidate iteration began before that requirement was surfaced. The learner identified the ambiguity and chose the nearest valid target for the current Basic Attack. This was a requirement-analysis omission, not a learner mistake.

---

## Coding Rules

- Keep code simple and readable.
- Use meaningful class, method, and variable names.
- Use PascalCase for classes and methods.
- Use camelCase for local variables and parameters.
- Use camelCase for private fields in this project. Follow the established project style consistently instead of mixing `camelCase` and `_camelCase`.
- Keep one main class per file.
- Avoid unnecessary static classes and global variables.
- Avoid over-engineering.
- Do not generate scripts longer than about 100 lines unless necessary.
- Split large systems into smaller components.

### Clean Code and Maintainability Learning Rule

Treat implementation and code quality as two separate checks. A feature is not ready merely because it compiles or behaves correctly.

In this project, the learner's primary meaning of **maintainability** is structural extensibility: adding a later feature should require a small, local change rather than rewriting the existing flow or adding checks across many unrelated components. Do not reduce the maintainability review to indentation, formatting, or naming.

After each small behaviour works, perform three separate checks:

1. **Functional correctness:** Verify that the current behaviour works as intended in Unity and that the Console is clean.
2. **Structural extensibility and coupling:** Ask what must change when the next related feature is added. Prefer one authoritative owner, local transition rules, explicit dependencies, and changes confined to the responsible component. Flag designs that require mutual component knowledge, duplicated state, scattered conditions, or rewriting the whole flow.
3. **Readability and code hygiene:** Review naming, formatting, method size, ordering, and comments after the behaviour and structure are understood. Readability matters, but it must not be presented as the whole maintainability check.

Use the following focused review points:

1. **Responsibility:** Explain why the code belongs in this class and whether another component should own it.
2. **Naming:** Check that class, method, property, field, and local-variable names communicate their purpose without requiring guesswork.
3. **Data flow:** Make it easy to trace where data comes from, how it changes, and which component consumes it.
4. **Dependencies:** Keep required component and object references explicit; avoid hidden lookups or duplicate ownership of the same engine operation.
5. **Method size and ordering:** Group related fields and members, use consistent blank lines and formatting, and extract a method when a block gains a distinct responsibility or makes the main flow difficult to read.
6. **Duplication and constants:** Remove meaningful duplication and unexplained magic values, while avoiding abstractions that are larger or harder to understand than the repeated code.
7. **Comments:** Prefer clear names and structure. Use comments to explain why a non-obvious decision exists, not to restate what an obvious line does.
8. **Change safety:** After a refactor, recompile and retest the affected Unity behaviour and Console so structural cleanup does not silently change the feature.

Teach these points through small reviews and learner edits. Ask the learner to identify or improve one focused readability issue at a time. Do not perform a broad refactor merely to demonstrate clean code, and do not mix unrelated cleanup into a feature change.

For an action-system change, include a concrete extension probe. For example: if `Dodge` were added next, identify exactly which files and rules would change. A design is structurally healthier when the new action can be added through a local state/transition extension without making `Movement`, `Combat`, `Animator`, and every other action component mutually aware of one another.

When a written convention conflicts with established project code, identify the inconsistency explicitly and choose one project-wide convention before continuing. Be cautious when renaming Unity serialized fields because changing a serialized field name can lose existing Inspector data unless a safe migration is used.

---

## Unity Rules

- Use Unity official APIs and recommended practices.
- Before giving editor UI instructions, verify the project's actual Unity version, relevant package version, and current visible interface. Do not assume that controls shown in older tutorials still exist or use the same name.
- When the user's interface differs from an instruction, inspect the actual project or screen first. Treat an unverified version difference as uncertain instead of asking the learner to repeatedly search for a possibly nonexistent option.
- Explain every new Unity API before using it.
- Explain every new C# syntax before using it.
- Do not place everything inside Update().
- Use components with clear responsibilities.
- Use prefabs for reusable GameObjects.
- Do not modify third-party packages directly.
- Test every feature inside Unity before moving to the next task.

---

## Debugging Rules

When an error occurs:

1. Read the complete Unity Console message.
2. Preserve the original English error message and immediately provide a plain-Chinese translation.
3. Identify the script and line number.
4. Explain what the error means in the current code.
5. Find the root cause.
6. Fix one issue at a time.
7. Test again.

Do not only provide corrected code. Explain why the error happened.

### Recorded Pitfall: Input Actions Editor Version Difference

- Project context: Unity 6.3 LTS (`6000.3.19f1`) and the Input System package installed in this project.
- In the current Input Actions Editor, selecting `Jump` shows `Action Properties > Action Type = Button`; a separate `Control Type` field is not displayed in this interface.
- Older tutorials or other package versions may show a separate `Control Type` or `Expected Control Type` field.
- Correct verification for this project: confirm `Action Type = Button` and the binding is `Space [Keyboard]`.
- This was a guidance and version-verification mistake, not a learner mistake.

### Recorded Pitfall: Component Presence Does Not Prove It Is Enabled

- A Unity GameObject or MCP component query may list a `MonoBehaviour` without including its `enabled` or `isActiveAndEnabled` state.
- Do not infer that a listed component is running merely because it exists on the GameObject.
- Before diagnosing duplicate controllers, competing behaviours, or an inactive script, explicitly verify `enabled` or `isActiveAndEnabled` through a resource that exposes the field or through a read-only runtime/reflection check.
- If the available inspection output does not expose activation state, report the state as uncertain instead of treating presence as evidence.
- Project example: `StarterAssets.ThirdPersonController` was present on `RelicGuardianPlayer` but already disabled. Treating its presence as proof that two movement controllers were active caused an unnecessary repeated instruction.
- This was an inspection and inference mistake, not a learner mistake.

### Recorded Pitfall: Animator Parameter Lists Do Not Show Complete Dependencies

- Seeing an Animator Controller parameter in a parameter list does not establish every role or dependency of that parameter.
- Before integrating or reusing an existing Animator Controller, inspect all relevant parameter consumers: transitions, Blend Trees, state speed parameters, behaviours, and any reference scripts that write the parameters.
- Record each relevant parameter's type, default value, writer, consumer, and runtime purpose before implementing the synchronization code.
- Do not assume that one apparent movement parameter controls both animation selection and playback speed.
- Project example: updating `Speed` selected a locomotion pose, but the animation remained frozen because the locomotion state's playback-speed parameter was `MotionSpeed`, whose default value was `0`.
- Correct verification for this project: `Speed` controls Idle/Walk/Run blending, while `MotionSpeed` controls playback rate; the Starter Assets reference code writes both.
- This was an incomplete dependency inspection, not a learner mistake.

### Animator Controller Integration Checklist

Before writing code against an existing Animator Controller:

1. List relevant parameters, types, and default values.
2. Find every transition, Blend Tree, state speed field, and behaviour that consumes them.
3. Inspect reference scripts to identify which values are written and when.
4. Write a short parameter-role map before choosing the smallest implementation step.
5. Test one visible animation responsibility at a time and check the Console.
6. When the observed pose changes but animation time does not advance, inspect playback-speed parameters before changing movement code.

---

## Cross-Workspace Boundary

- `C:\Unity\Learning\CSharpPractice` is a separate learning workspace with its own `AGENTS.md`, plan, progress record, exercises, and Git context.
- From a Relic Guardian task, C# Practice files are read-only by default. They may be inspected to verify current learning progress, but must not be created, edited, deleted, formatted, staged, or committed unless the learner explicitly asks to switch to or modify that workspace.
- Continue C# lessons in the Codex task whose workspace is `C:\Unity\Learning\CSharpPractice`.
- The central `Docs/LEARNING_TRACKER.md` in Relic Guardian may be updated from verified read-only evidence so cross-track next actions remain current.
- Do not treat a stale central tracker entry as stronger evidence than the detailed progress file in the track's own workspace.
- Apply the same boundary to future graphics, tools, or practice workspaces: inspect across workspaces when useful, but mutate only the workspace explicitly in scope.

---

## Scope Rules

The core demo includes:

- Third-person movement
- Camera control
- Jump
- Melee combat
- Hitstop
- Dodge
- Perfect Dodge
- Enemy AI
- One small level
- One boss
- Basic UI

Do not add unrelated systems before the core demo is playable.

The following systems are optional and should only be added near the end:

- Addressables
- YooAsset
- HybridCLR
- Hot update demonstration

---

## Documentation Rules

### Context Continuity and Durable Decision Rule

- Important requirements, design decisions, verified checkpoints, recurring pitfalls, and corrections must not exist only in chat history. Record them in the appropriate project document as soon as they become consequential.
- After automatic context compaction, a new Codex task, or a handoff, do not treat the generated conversation summary as the sole source of truth.
- Before continuing implementation after such a boundary, read `Docs/CURRENT_STATE.md`, `Docs/ARCHITECTURE.md`, the current `Docs/HANDOFF.md`, and `Docs/CONTEXT_INDEX.md`; check current Git status; then inspect only the actual files in the selected task route.
- Search `Docs/DEV_LOG.md`, `Docs/Archive/`, `Docs/LEARNING_PROGRESS.md`, or `Docs/ROADMAP.md` and read targeted sections only when the current route, a conflict, or a historical question requires them. Do not repeatedly load complete archive documents.
- When a summary conflicts with the workspace, Git, Unity, or the maintained project documents, verify the discrepancy and treat the actual current project state as authoritative.
- Do not record unfinished work as verified or complete merely to preserve continuity. Keep work-in-progress decisions separate from tested checkpoints.

### Completion Records

After completing a task:

- Mark it in ROADMAP.md.
- Record the work in DEV_LOG.md.
- Record bugs and solutions.
- Make a Git commit when the feature works.
