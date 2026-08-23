# Game Client Learning Tracker

## Purpose

This file is the central index for all game-client learning tracks. It records what is active, where its files live, what unlocks it, and what happens next.

Detailed content and teaching order belong in each track's own plan. This tracker prevents a planned subject from being forgotten when projects or Codex tasks change.

## Status Meanings

- **Active**: has a current workspace, progress record, and scheduled learning time.
- **Embedded**: practised inside an active track when relevant; no separate workspace is needed.
- **Locked**: intentionally delayed until its stated gate is satisfied.
- **Preparing**: the gate has been satisfied and its workspace or detailed plan is being created.
- **Complete**: the planned evidence has been produced and reviewed.

## Track Registry

| Track | Status | Workspace | Detailed records | Unlock condition | Next action |
| --- | --- | --- | --- | --- | --- |
| Relic Guardian | Active | `C:\Unity\Project\My project` | `Docs/PROJECT_PLAN.md`, `Docs/ROADMAP.md`, `Docs/LEARNING_PROGRESS.md`, `Docs/COMBO_ATTACK_ARCHITECTURE.md` | Already started | Add explicit stale-animation-event identity protection before Restart Window or Attack3 extension |
| C# foundations | Active | `C:\Unity\Learning\CSharpPractice` | `AGENTS.md`, `Docs/LEARNING_PLAN.md`, `Docs/LEARNING_PROGRESS.md` in that workspace | Already started | Continue with integer assignment and arithmetic; variable output and Console text input are verified |
| Data structures and algorithms | Prepared, locked by Gate DS-1 | `C:\Unity\Learning\CSharpPractice\DataStructures` | `AGENTS.md`, `PREREQUISITES.md`, `LEARNING_PLAN.md`, and `LEARNING_PROGRESS.md` in that folder | Pass the observable C# readiness check in `PREREQUISITES.md`; no IQ threshold applies | Continue C# foundations, then run the four-part Gate DS-1 check and start immediately if passed |
| Game mathematics | Embedded | Relic Guardian, C# practice, and later Graphics Lab | Record evidence in the active track's progress file | A gameplay or graphics problem needs the concept | Revisit vectors and rotations through current gameplay examples |
| Debugging and testing | Embedded | Relic Guardian and C# practice | Main and C# progress files; project pitfall records where relevant | Always active | Require evidence-based diagnosis and runtime or test verification |
| Clean code and architecture | Embedded | Relic Guardian and C# practice | Main and C# progress files | Always active | Keep correctness and maintainability as separate checks |
| Git and team workflow | Embedded | Relic Guardian first; later other repositories | Git history and relevant project documentation | Always active | Continue small single-purpose commits and begin guided diff review |
| English technical reading | Embedded | Every active workspace | Record useful recurring terms and observable reading tasks in the active progress file | A new API, package, version issue, or error needs documentation | Read one small relevant official-document section when such a need appears |
| Graphics and Shader | Locked | Future `UnityGraphicsLab` workspace | Create `AGENTS.md`, `Docs/LEARNING_PLAN.md`, and `Docs/LEARNING_PROGRESS.md` when unlocked | Relic Guardian attack, hit detection, damage, enemy health, and death work together | Do not create the Unity project before the combat-loop gate |
| Performance optimization | Locked within Relic Guardian | Relic Guardian | Future profiling records in this project's `Docs/` | A stable, reproducible gameplay scenario exists after the combat loop | Capture a baseline before selecting an optimization technique |
| Unity Editor tooling | Locked within Relic Guardian | Relic Guardian | Future tool notes in this project's `Docs/` | A real repetitive, error-prone, or difficult-to-inspect workflow appears | Record candidate workflows; build no tool without a real problem |
| Portfolio and technical communication | Locked | Relic Guardian plus selected supporting evidence | Future portfolio checklist, README, video, and postmortem | The demo is feature-complete enough to present | Accumulate architecture decisions, difficult bugs, and measured results during development |
| Mock interviews | Locked | No separate project required | Future interview question and answer record | Portfolio evidence is ready to explain | Begin project walkthroughs and selected C#/Unity questions near portfolio delivery |

## Current Weekly Allocation

Until the first complete combat loop:

- Relic Guardian: three focused sessions.
- C# practice: two focused sessions.
- Review and relevant official-document reading: one short session.
- Game mathematics, debugging, clean code, Git, and technical explanation are embedded in those sessions.

After the combat loop is stable:

- Relic Guardian: two or three focused sessions.
- C# and data structures: two focused sessions.
- Graphics Lab: one focused session.
- Review and relevant official-document reading: one short session.
- Performance profiling begins when a reproducible scenario is available.

## Workspace Creation Rule

Do not create an empty project merely because a subject appears in the plan.

When a locked track becomes **Preparing**:

1. Confirm its unlock evidence.
2. Choose whether it belongs in an existing project or needs an isolated workspace.
3. Create its teaching rules, learning plan, and progress record.
4. Add its exact location to this tracker.
5. Begin with one small verified exercise or project problem.

## Update Rule

Update this tracker when any of the following occurs:

- A track changes status.
- A new workspace or progress file is created.
- An unlock gate is satisfied.
- The next concrete action changes materially.
- A planned portfolio evidence item is completed.

Do not duplicate detailed lesson notes here. Keep them in the corresponding track's progress file.

## Immediate Focus

Only two tracks are independently active now:

1. Relic Guardian: the reusable Attack1-to-Attack2 combo is runtime-verified, including two damage windows, hybrid queued input, bounds checks, centralized cleanup, and final recovery. Next add explicit stale-animation-event identity protection before Restart Window or Attack3 extension.
2. C# Practice: continue with integer assignment and arithmetic after verified variable-output and Console-input lessons.

All other subjects are either embedded into those sessions or preserved behind explicit gates.
