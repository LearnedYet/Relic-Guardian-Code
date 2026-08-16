# Game Client Learning Plan

## Goal

Prepare for a Unity game-client internship by combining one finished portfolio demo with focused programming practice and small technical experiments.

This plan complements `PROJECT_PLAN.md` and `LEARNING_PROGRESS.md`. The project roadmap tracks game features; this document tracks the broader internship-learning path. `LEARNING_TRACKER.md` is the operational index for every track's status, workspace, unlock gate, and next action.

The target is not to briefly encounter every subject. The target is to produce observable evidence that the learner can implement features, explain decisions, debug problems, measure performance, and continue learning from documentation.

## Recruitment Requirements Covered

This plan covers the requirements identified in the target game-client internship:

| Recruitment capability | Learning evidence |
| --- | --- |
| Implement game features and logic | A complete playable loop in Relic Guardian |
| Develop and maintain editor-related tools | One small Unity Editor tool that solves a real project problem |
| Improve visual quality and performance | A measured optimization and one understood rendering effect |
| Object-oriented design and standardized code | Clear component responsibilities, refactoring, tests, and code explanations |
| Data structures and algorithms | Selected console exercises with tests and complexity explanations |
| English technical reading | Regular use of official C#, Unity, and package documentation |
| Computer graphics | A small URP/Shader lab and an effect transferred into the main project |
| Practical development experience | Git history, debugging records, documentation, playable build, and video |

## Four Learning Tracks

### Track A - Relic Guardian (Main Portfolio Project)

Purpose: learn how complete gameplay systems are designed, connected, tested, polished, and presented.

Current priority:

1. Finish the player locomotion and animation foundation.
2. Build the smallest complete combat loop: attack, hit detection, damage, enemy health, and death.
3. Add dodge, game feel, enemy AI, a small level, UI, and a boss encounter.
4. Profile, polish, document, record a video, and package a playable build.

Do not start another large game before this demo has a small but complete playable loop.

### Track B - C# and Data Structures (Separate Practice Workspace)

Purpose: remove gaps that are difficult to practise systematically inside a Unity scene.

Create a separate folder and Codex task for this track. Use small .NET console programs rather than placing exercises under this Unity project's `Assets` folder.

Learning order:

1. Variables, types, operators, conditions, loops, and methods.
2. Scope, fields, properties, parameters, return values, and debugging.
3. Classes, constructors, access modifiers, composition, and interfaces.
4. Arrays, `List<T>`, `Dictionary<TKey, TValue>`, strings, and iteration.
5. Value types, reference types, `null`, exceptions, and basic memory concepts.
6. Delegates, events, generics, and the parts of LINQ commonly used in gameplay code.
7. Unit tests, refactoring, naming, responsibilities, and readable code structure.
8. Complexity, stacks, queues, linked lists, hash tables, trees, graphs, sorting, and searching.

Practise one new concept at a time. Each lesson should contain recall, a short explanation, a prediction, a small attempt by the learner, tests, correction, and later review. Do not begin with random difficult interview problems.

Data structures begin gradually rather than waiting for all C# topics to finish:

- Introduce arrays, lists, dictionaries, traversal, and simple complexity while learning collections.
- Begin formal stack, queue, hash-table, tree, graph, sorting, and searching practice after the learner can independently write small methods and classes.
- Require an explanation of the chosen structure, edge cases, and approximate time complexity instead of accepting only a passing answer.

### Track C - Graphics and Shader Lab (Later Separate Unity Project)

Purpose: understand rendering and create one or two small technical demonstrations without destabilizing the main game.

Start this track only after Relic Guardian has a working attack-and-damage loop. Suggested order:

1. Vectors, dot product, cross product, interpolation, coordinate spaces, normals, and basic lighting.
2. URP materials and Shader Graph.
3. Vertex and fragment stages, textures, masks, dissolve, rim light, and hit flash.
4. Frame Debugger, Profiler, overdraw, batching, and basic GPU-performance reasoning.

Useful results can later be reproduced in Relic Guardian after they are understood and tested in isolation.

### Track D - Embedded Professional Skills

These skills do not need separate large projects. They are practised inside the other tracks when a real need appears.

#### Game Mathematics

- Vectors, distance, direction, angles, dot product, cross product, interpolation, coordinate spaces, and rotations.
- Learn each idea first through a concrete gameplay or graphics problem, then reconstruct it in a small exercise.
- Keep a short explanation of why the calculation works, not only the final formula.

#### Debugging and Testing

- Read compiler errors, Console messages, stack traces, and runtime state.
- Reproduce bugs, form a small hypothesis, inspect evidence, change one cause at a time, and verify the result.
- Use breakpoints and focused logging when they provide useful evidence.
- Add unit tests to suitable non-Unity logic and use Play Mode checklists for component integration.

#### Unity Editor Tools

- Start after Relic Guardian has repeated project data or a real manual workflow worth improving.
- Build one small tool, such as a data validator, batch asset creator, custom Inspector, scene checker, or Animator-parameter checker.
- Document the original problem, time or mistakes reduced, and the tool's limitations.

#### Git and Team Workflow

- Make small commits with one clear purpose.
- Inspect diffs and avoid combining unrelated changes.
- Learn basic branches, merge-conflict reasoning, code review, and safe collaboration habits.
- Keep project decisions and recurring pitfalls in documentation rather than relying only on chat history.

#### English Technical Reading

- Read a small relevant section of official documentation when a new API, package, or error appears.
- Practise extracting the purpose, parameters, return value, example, and version restrictions.
- Record a small vocabulary list only for terms that recur; translation is support, not the final goal.

#### Technical Communication and Interviews

- Explain data flow, component responsibilities, design choices, bugs, and performance findings in plain language.
- Prepare short answers for what was personally implemented, why it was designed that way, and how it would change under a new requirement.
- Practise a small code review and a project walkthrough before portfolio delivery.

## Phase Gates

New subjects start when their prerequisites are observable, not on a rigid calendar.

### Gate 1 - C# Foundations

Evidence required:

- Write and explain a small console program using variables, conditions, loops, and methods.
- Read a basic compiler error and correct it with limited help.

Unlocks:

- Formal collection exercises and beginner data-structure practice.

### Gate 2 - Complete Combat Loop

Evidence required in Relic Guardian:

- Attack input, attack execution, hit detection, damage, enemy health, and death work together.
- The behaviour has been verified in Play Mode and its data flow can be explained.

Unlocks:

- The separate graphics and Shader lab.
- Systematic Unity profiling instead of speculative optimization.

### Gate 3 - Repeated Project Workflow

Evidence required:

- A real repetitive, error-prone, or difficult-to-inspect Unity workflow has appeared.

Unlocks:

- One small Unity Editor tool aimed at that actual problem.

### Gate 4 - Feature-Complete Demo

Evidence required:

- The planned demo loop, enemy encounter, UI, and presentation path are playable.

Unlocks:

- Focused optimization, final architecture cleanup, portfolio packaging, and mock interviews.

## Performance Learning Process

Performance work follows measurement rather than assumptions:

1. Define a reproducible gameplay scenario.
2. Capture CPU, memory allocation, rendering, or frame-time evidence.
3. Identify one material bottleneck.
4. Change one relevant cause.
5. Measure the same scenario again.
6. Record the result and any tradeoff.

Topics may include Unity Profiler, GC allocations, `Update()` cost, physics-query frequency, object lifetime, pooling, Animator cost, particles, draw calls, batching, overdraw, and asset settings. A technique is not treated as an optimization until a relevant measurement supports it.

## Recommended Time Split

Until the first complete combat loop:

- 60% Relic Guardian
- 30% C# and data-structure practice
- 10% review, notes, and English documentation reading

After the combat loop is stable, move about 10% to the graphics and Shader track.

Editor tooling, Git, debugging, English reading, and technical communication are embedded into the active sessions instead of receiving fixed percentages.

## Weekly Rhythm

- Three main-project sessions: implement and verify one small gameplay responsibility at a time.
- Two C# sessions: one new concept and one reconstruction/review session.
- One short review: explain learned code without looking, update progress, and record recurring mistakes.
- One small official-document reading task when the current work introduces an unfamiliar API or package.
- Rest or catch-up time is allowed; unfinished lessons continue instead of multiplying new topics.

## Teaching Rules Across All Tracks

- Explain new syntax, APIs, and vocabulary before expecting independent use.
- Let the learner predict and attempt key code before showing a complete answer.
- Give the smallest useful hint first, then increase help gradually.
- Separate feature correctness from code readability and maintainability checks.
- Verify behaviour with a console run, test, or Unity Play Mode instead of treating compilation as completion.
- Record observable progress rather than marking a topic learned after one explanation.
- Revisit important concepts after a delay and ask for reconstruction in a different example.

## Portfolio Target

Relic Guardian should eventually demonstrate:

- A complete and polished third-person combat loop.
- Clear component responsibilities and maintainable C# structure.
- Enemy behaviour, animation integration, UI, feedback, and a playable build.
- At least one measured optimization with before-and-after evidence.
- A concise README, gameplay video, architecture explanation, and honest description of individual work.
- One small Unity Editor tool with a documented purpose.
- One graphics or Shader experiment that can be explained from inputs to visible result.
- A short technical postmortem covering a difficult bug and its evidence-based diagnosis.
- Selected C# and data-structure exercises with tests and reasoning, not only final answers.
- Interview-ready explanations of architecture, tradeoffs, optimization evidence, and future improvements.

The separate C# and graphics workspaces support the portfolio, but they do not need to become additional large games. Small, well-explained exercises and technical demos are enough.

## Immediate Next Actions

1. Begin the smallest Relic Guardian combat-loop step with Attack input requirements and existing Animator/controller inspection.
2. Continue the existing `C:\Unity\Learning\CSharpPractice` workspace with integer assignment and arithmetic after the verified baseline lessons.
3. Delay the graphics/Shader lab until the first attack-and-damage loop works.
4. Practise Git, debugging, clean code, and small official-document reading inside the current work rather than starting more projects.
