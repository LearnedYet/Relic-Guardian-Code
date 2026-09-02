---
name: relic-guardian-multi-agent
description: Coordinate bounded subagents for complex Relic Guardian cross-module investigation, independent review, Git mirror auditing, or competing bug hypotheses. Do not use for learner-first code entry, small sequential edits, or shared Unity asset mutation.
---

# Relic Guardian Multi-Agent Workflow

Use this skill only for the Unity project rooted at `C:\Unity\Project\My project`.

## Core Context Precondition

On a new task, after compaction, or whenever current state is uncertain, the root agent must first use `relic-guardian-context` and personally complete its minimum bootstrap:

1. read the current project instructions and entry documents;
2. inspect current Git status;
3. select the task route through `Docs/CONTEXT_INDEX.md`;
4. report current state, verification gaps, next concept, and protected dirty files.

Do not delegate this core bootstrap or ask multiple agents to reread the complete startup set. Delegate only bounded task-specific investigation after the root understands the source-of-truth order and mutation boundaries.

## Eligibility Gate

Use subagents when at least one of these applies and the work can be split into independent outputs:

- a cross-module investigation spans separate code or data-flow regions;
- multiple plausible bug causes can be tested independently;
- a large repository, asset, log, GUID, or Git audit would otherwise pollute the root context;
- an architecture or high-risk change benefits from an independent review;
- separate test or verification work can run without contending over shared state.

Keep the work on the root agent when it is small, strictly sequential, learner-authored, or dominated by one Unity import/compile/runtime wait. Prefer direct tools or programmatic tool calls for bounded mechanical work that needs no separate model judgment.

## Roles and Models

The root agent keeps the main task context, architecture authority, learner conversation, final synthesis, and final verification. The root model is chosen by the user or session; this skill does not change it.

When the current Codex environment supports explicit spawn overrides:

- use `gpt-5.6-terra` for complex module relationships, FSM/lifecycle analysis, competing bug hypotheses, or independent architecture/code review;
- use `gpt-5.6-luna` for repository search, reference inventories, path/GUID checks, logs, bounded test execution, and mechanical evidence gathering;
- use explicit model names rather than relying on root-model inheritance;
- set `fork_turns="none"` when selecting a different model, unless a small positive recent-turn window is genuinely required.

Do not create a subagent merely to assign a model label. The task must be independently useful.

## Dispatch Contract

Before spawning, send a short user-facing update that Multi-Agent is being used and name the independent workstreams.

Every subagent task packet must include only what it needs:

- exact project root and task objective;
- read-only or writable status;
- exact files, route, subsystem, or search target;
- applicable source-of-truth and protected-file constraints;
- forbidden actions, especially Unity mutation, Git mutation, licensed-asset copying, and unrelated reads;
- expected evidence and output structure;
- stopping condition.

Default to one subagent. Use two or three only when their workstreams do not depend on each other's intermediate decisions. Do not recursively spawn unless the parent task has a concrete additional independent split.

Avoid duplicate full-repository reading. Assign one owner per information domain unless deliberate independent review is justified by risk.

## Shared-State Safety

All agents share the same filesystem. Investigation agents are read-only by default.

- During learner-first work, subagents may investigate or review but must not write key learning code. The learner remains the established writer until explicitly requesting takeover.
- After explicit takeover, allow only one designated writer for a file or coherent scope. Other agents remain read-only reviewers.
- Do not let multiple agents control the same Unity Editor instance, Scene, Prefab, Animator, Animation Clip, or import process.
- Unity MCP instance selection is root-owned. If a subagent is exceptionally assigned Unity work, give it one exact instance and exclusive scope; do not run competing Unity mutations.
- Preserve all protected dirty files and the complete `Assets/LocalLicensed/` boundary.
- Delegation never authorizes staging, commits, pushes, remote mutation, destructive operations, or a broader asset scope.

## Execution and Synthesis

Use collaboration tools to spawn the chosen bounded tasks, wait for results, and request one focused follow-up only when a returned uncertainty blocks synthesis. Do not repeat a completed investigation locally without a concrete verification reason.

Ask each investigation agent to return a compact structure:

```text
Conclusion
Evidence: exact files and line numbers, commands, logs, or asset paths
Risks or conflicts
Unverified items
Changes made: none, unless explicitly authorized
```

Subagents should summarize noisy output instead of returning complete logs or large source excerpts.

The root agent must:

1. reconcile conflicting conclusions;
2. distinguish observed facts from inference;
3. inspect the decisive actual evidence;
4. choose the final architecture or learner step;
5. verify any authorized implementation against build, Console, Runtime, Git, and protected-file boundaries;
6. report one coherent result to the learner.

## Cost and Stop Rules

- Do not use Multi-Agent when the root can finish with a few direct reads or tool calls.
- Stop spawning when the independent questions are covered; more agents are not additional evidence by themselves.
- Keep prompts and returned summaries bounded. Do not pass full history or raw repository output by default.
- Treat Multi-Agent primarily as a context-quality, coverage, and wall-clock optimization, not as a guaranteed token or usage-credit saving.
