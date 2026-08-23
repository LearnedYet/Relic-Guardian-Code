# Combo Attack Architecture and Refactor Plan

## Status

- **Configured through Attack4; Attack1 -> Attack2 checkpoint verified, complete four-hit acceptance pending**
- Read-only inspection completed: 2026-08-20
- Final four-clip light-attack asset set prepared: 2026-08-21
- Attack1 replacement, one-entry attack data, indexed runtime foundation, reusable step initialization, and authored Combo Window boundaries completed: 2026-08-21.
- Queued Attack input routing completed and runtime-verified: 2026-08-22.
- Indexed Attack2 presentation, hybrid queue consumption, two damage windows, centralized cleanup, and two-hit runtime acceptance completed: 2026-08-22.
- Attack-step event identity, Attack1-3 Restart Windows, and Attack3/4 indexed content extension completed: 2026-08-23.
- This document distinguishes the verified two-hit baseline from the configured and partially runtime-checked four-hit extension. General recovery cancellation remains deferred until Dodge or Block exists.

## Purpose

Extend the verified Basic Attack into a reusable four-step combo without copying a complete attack flow for each animation.

The first implementation target was deliberately limited to:

```text
Attack1
-> optional queued input during a Combo Window
-> Attack2
-> Free
```

That target passed. Attack3 and Attack4 were then added through attack-data entries, tracked override slots, Animator configuration, and indexed Events without copied `PlayerCombat` flow methods.

Adding a step still requires an Animator state or override slot, transitions, Animation Events, and runtime verification. The goal is to avoid duplicated C# attack-flow logic, not to eliminate content setup.

## Prepared Animation Set

The learner previewed and approved this eventual four-step presentation order:

1. `Attack_4Combo_1_Inplace`
2. `Attack_4Combo_2_Inplace`
3. `Attack_4Combo_3_Inplace`
4. `Attack_3Combo_3_Inplace`

All four clips are locally imported under ignored `Assets/LocalLicensed/PowerfulSwordPack/Katana/LightCombo/` as non-looping Humanoid clips. This does not change the first implementation target: validate the reusable architecture with Attack1 and Attack2 before extending it to Attack3 and Attack4.

## Verified Current Baseline

The following facts come from a read-only inspection of the current scripts, Animator Controller, local Animator Override Controller, and current Attack1 animation metadata.

### PlayerCombat

`PlayerCombat` currently owns:

- one serialized `PlayerAttackData[] attacks` configuration array;
- `currentAttackIndex` and `CurrentAttackData` lookup;
- reusable `StartAttackStep(int attackIndex)` initialization;
- the shared target LayerMask;
- attack-facing, lunge, travelled-distance, current-target, and confirmed-target runtime state;
- nearest-target search and Hit Window-time target reconfirmation;
- damage, target range, lunge speed, and lunge distance read from the current attack-data entry;
- independent `isHitWindowOpen` and `isComboWindowOpen` states plus their open/close event receivers;
- `isAttackQueued`, `hasReachedComboTransitionPoint`, and state-dependent routing of one consumed Attack request;
- `HasNextAttack`, reusable `TryStartQueuedAttack()`, and centralized `FinishAttack()` cleanup.

The Prefab currently configures two entries, both with prototype damage `1`, target range `2`, lunge speed `5`, and lunge distance `1`. `Update()` consumes Attack once, starts Attack1 through the grounded `Free` path, queues only while `BasicAttack` and the Combo Window are active, and advances immediately when the transition point has passed. The authored event consumes earlier queued input. Runtime acceptance verified both paths and final cleanup.

### PlayerActionController

`PlayerActionController` currently owns one authoritative coarse action state:

```text
Free
BasicAttack
```

It grants movement and Jump only while `Free`, accepts a grounded Basic Attack only from `Free`, and returns directly to `Free` through `FinishBasicAttack()`.

### PlayerAnimator

`PlayerAnimator` synchronizes locomotion and airborne parameters and exposes:

```text
PlayAttack(index) -> SetInteger("AttackIndex", index) -> SetTrigger("Attack")
```

It receives the index selected by `PlayerCombat` and does not decide gameplay rules.

### PlayerMovement

`PlayerMovement` remains the owner of `CharacterController` displacement. `PlayerCombat` supplies an attack direction and requested distance through `MoveDuringAttack()`.

This responsibility boundary is already suitable for multiple attack steps and should be preserved.

### PlayerInputReader

`PlayerInputReader` stores one Attack request and clears it when `ConsumeAttack()` is called. It does not and should not decide whether an attack starts or becomes a queued combo input.

### Current Animation Events

The active local clips contain:

| Approximate frame | Normalized time | Event | Current receiver |
| --- | --- | --- | --- |
| `9.7` | `0.2771416` | `OpenHitWindow` | `PlayerCombat` |
| `11.6` | `0.3302259` | `OpenComboWindow` | `PlayerCombat` |
| `12.2` | local importer timing | `CloseHitWindow` | `PlayerCombat` |
| `12.4` | local importer timing | `ComboTransitionPoint` | `PlayerCombat` |
| `21.7` | `0.6189415` | `CloseComboWindow` | `PlayerCombat` |
| `34.2` | `0.9763113` | `FinishAttack` | `PlayerCombat` |
| Attack2 `8` | local importer timing | `OpenHitWindow` | `PlayerCombat` |
| Attack2 `12` | local importer timing | `CloseHitWindow` | `PlayerCombat` |
| Attack2 `35.0` | local importer timing | `FinishAttack` | `PlayerCombat` |

Hit, Combo, transition, and finish events enter the combat coordinator. Both final paths use centralized cleanup before returning to `Free`. Attack2 intentionally has no Combo Window yet. Restart Window events do not exist.

### Current Animator Structure

The project-owned Animator Controller currently has:

- one `Attack` Trigger and one `AttackIndex` Int;
- `BasicAttack` and `Attack2` states with distinct tracked motion keys;
- one locomotion-to-`BasicAttack` transition using the Trigger;
- one unconditional `BasicAttack` exit transition at Exit Time `0.9`;
- indexed `BasicAttack -> Attack2` routing and an unconditional Attack2 exit.

The active local `KatanaAnimationOverrides` maps tracked `SwordAndShieldSlash` to Attack1 and tracked `Attack2Placeholder` to Attack2.

The same local Override Controller maps the base locomotion `Idle` placeholder to looping Humanoid `Idle_ver_B`. Root Motion remains disabled. This presentation mapping is local/ignored and does not change combo gameplay ownership.

## Architectural Decisions

### 1. PlayerActionController Owns Only Coarse Character Actions

The intended coarse states are:

```text
Free
Attacking
Dodging       (later)
Staggered     (later)
Dead          (later)
```

`Attack1`, `Attack2`, and `Attack3` must not become `PlayerActionState` members. They are steps within the single coarse `Attacking` action and belong to `PlayerCombat`.

When real interrupting actions such as `Staggered` or `Dead` are introduced, `PlayerActionController` will also need explicit interruption and transition rules. That work is intentionally deferred until an actual interrupting player action exists.

### 2. PlayerAttackData Stores Configuration Only

Use a small inline serializable class initially. Do not introduce a ScriptableObject, Skill System, inheritance hierarchy, or asset database for the first two-hit combo.

Each attack-data entry should initially contain:

- `damage`
- `range` (prefer a more specific name such as `targetRange` or `hitRange`)
- `lungeSpeed`
- `lungeDistance`

The existing shared `hitTargetLayers` remains on `PlayerCombat` until different attacks genuinely need different target layers.

Runtime values must not be stored in `PlayerAttackData`. The following remain on `PlayerCombat`:

- current attack index;
- current and confirmed targets;
- Hit Window state;
- Combo Window state;
- queued-input state;
- attack-facing and lunge state;
- travelled lunge distance.

### 3. PlayerCombat Coordinates the Complete Attack Sequence

`PlayerCombat` should own:

- `currentAttackIndex`;
- access to the current attack-data entry;
- beginning and initializing an attack step;
- per-step target selection;
- target-facing and bounded lunge lifetime;
- Hit Window lifetime and confirmed damage;
- Combo Window lifetime;
- queued Attack input;
- advancing to the next configured attack step;
- attack cleanup and the request to return the coarse action to `Free`.

The implementation must use one reusable “start attack step by index” path. It must not grow separate copied methods containing the full logic for Attack1, Attack2, and Attack3.

### 4. PlayerAnimator Owns Presentation Only

`PlayerAnimator` may evolve from `PlayAttack()` to `PlayAttack(index)`.

It may translate the requested index into Animator parameters, but it must not decide:

- whether the Combo Window is open;
- whether an input is accepted or queued;
- which target receives damage;
- whether the player may leave `Attacking`;
- how much damage or lunge an attack uses.

A likely minimal Animator interface is one integer attack index plus the existing Attack Trigger. The exact parameter and transition setup must be verified in the Editor before implementation.

### 5. PlayerMovement Keeps All CharacterController Displacement

`PlayerCombat` calculates the direction and requested frame distance for the current attack step. `PlayerMovement.MoveDuringAttack()` performs the actual displacement.

No Attack1-, Attack2-, or Attack3-specific movement method should be added unless a later attack genuinely requires a different movement mechanism.

### 6. Targeting Remains Inside PlayerCombat for Now

There is currently only one targeting policy: choose the nearest target within a range and reconfirm that saved target during the Hit Window.

Do not extract `PlayerTargeting` until multiple real policies exist, such as:

- normal nearest-target attack;
- thrust or narrow directional attack;
- area attack;
- lock-on target;
- target-preserving versus retargeting combo rules.

## Intended Input and Attack Data Flow

```text
Input Action
-> PlayerInputReader stores one Attack request
-> PlayerCombat consumes the request

If coarse action is Free:
-> request PlayerActionController: Free -> Attacking
-> currentAttackIndex = 0
-> read attack data at index 0
-> select and save a target
-> reset facing and lunge runtime state
-> PlayerAnimator.PlayAttack(0)

If coarse action is Attacking and the Combo Window is open:
-> store queued Attack input
-> do not change animation immediately

At the authored ComboTransitionPoint:
-> if an input is queued and another attack-data entry exists
   -> increment currentAttackIndex
   -> initialize the next step through the same reusable path
   -> PlayerAnimator.PlayAttack(currentAttackIndex)
-> otherwise
   -> mark that the earliest transition point has been reached

After ComboTransitionPoint and while the Combo Window remains open:
-> a later valid Attack input may start the next step immediately

After CloseComboWindow and while an explicit Restart Window is open:
-> a valid Attack input resets the chain to index 0
-> start a fresh Attack1 through the same reusable path
-> this late-recovery cancel is a confirmed target design, not implemented behaviour

At final attack completion:
-> PlayerCombat clears all attack runtime state
-> PlayerCombat requests PlayerActionController: Attacking -> Free
```

## Hit Window and Combo Window Must Stay Separate

The Hit Window answers:

> Can this attack step confirm and apply damage now?

The Combo Window answers:

> Can an Attack input be accepted as a request for the next step now?

They may occur at different times and may overlap only if the authored animation needs it. One must not be reused as the other.

For consistent animation timing, an input accepted before the earliest transition point should set a queued flag rather than cutting the main strike immediately. After that point, a later input may transition immediately while the Combo Window remains open. This hybrid rule preserves a minimum authored strike while avoiding an unnecessary wait for input entered during the compatible recovery pose.

## Combo Transition and Restart Windows Must Stay Separate

`ComboTransitionPoint` is a one-time event, not a duration. It marks the earliest frame at which Attack1 may enter Attack2. Runtime state must remember that the point has passed if inputs arriving later in the still-open Combo Window should transition immediately.

The learner also wants the interval after `CloseComboWindow` and before final completion to accept a fresh Attack1. That behaviour is a separate late-recovery cancel or Restart Window:

- Combo Window input requests the next combo step, Attack2.
- Restart Window input resets the chain and requests Attack1.
- Startup before `OpenComboWindow` accepts neither.
- With no accepted input, `FinishAttack` performs cleanup and returns to `Free`.

Do not infer Restart Window state from `isComboWindowOpen == false`; that value is also false before the Combo Window opens. Use an explicit state/event or another equally clear attack-phase representation when this behaviour is implemented.

## Intended Animation Event Contract

Each attack animation may use the same method names, but each clip owns its own event timing:

- `OpenHitWindow`
- `CloseHitWindow`
- `OpenComboWindow` (when another step is available)
- `ComboTransitionPoint` (earliest transition decision)
- `CloseComboWindow`
- `OpenAttackRestartWindow` (later, only if the confirmed restart design is implemented)
- `FinishAttack`

The exact separation between “accept next-step input,” “permit transition,” and “accept a fresh Attack1 restart” must remain explicit. Do not make one event perform all three responsibilities.

The finish event should enter `PlayerCombat`, not call `PlayerActionController` directly. `PlayerCombat` must clean its runtime state before releasing the coarse action.

## Animator and Override-Controller Constraint

The verified Controller selects Attack2 through `AttackIndex == 1` plus the existing `Attack` Trigger on the `BasicAttack -> Attack2` transition.

The implementation avoided the shared-key failure by using distinct tracked motion keys: `SwordAndShieldSlash` for Attack1 and `Attack2Placeholder` for Attack2.

The ignored local Override Controller maps each tracked key to its matching licensed clip, preserving the tracked-project versus ignored-licensed-assets boundary.

Attack2 was previewed, accepted, authored, and runtime-verified. Attack3 remains intentionally deferred until explicit stale-event identity protection is added or the next extension is deliberately scoped.

## Main Risks

### Resolved: Consumed Input Routing

The implementation consumes Attack once and then deliberately routes the saved result:

- start Attack1 while `Free`;
- queue or immediately start Attack2 only while `BasicAttack` and the Combo Window is open;
- ignore all other cases deliberately.

### An Outgoing Animation Event Could Affect a New Step

If Attack1 transitions into Attack2 while an outgoing Attack1 event can still fire, a late finish or window event could mutate Attack2 state.

The two-hit runtime tests did not reproduce an early outgoing finish or window mutation: Attack2 remained in `BasicAttack` until its own finish and cleanup. The same risk becomes stronger when a later Restart Window starts a fresh Attack1 or when more transitions overlap. Events should carry an attack-step/sequence identity, or use another explicit guard, so `PlayerCombat` rejects stale events that do not belong to the current execution.

### Interruptions Need One Cleanup Boundary

Future Dodge, Staggered, and Dead transitions may interrupt an attack. A later centralized cancellation boundary must clear:

- Hit and Combo Windows;
- queued input;
- current and confirmed targets;
- facing and lunge state;
- current attack index.

Do not implement this before a real interrupting player action exists, but do not scatter partial cleanup across future states.

### PlayerCombat Can Become Too Large

Keeping targeting in `PlayerCombat` is appropriate while only one policy exists. Reassess the class after a second targeting policy, attack shape, or skill flow creates real duplicated logic.

### Data and Runtime State Can Be Accidentally Mixed

`PlayerAttackData` entries are reusable configuration. They must never retain per-execution target references, travelled distance, open-window flags, or queued input.

## Minimal Migration Plan

Perform and verify one step at a time.

1. **Completed:** preview and select the intended sequence, then import the four approved local clips.
2. **Completed:** add the minimal inline `PlayerAttackData` type and configure only Attack1.
3. **Completed and runtime-regressed:** make the existing single attack read damage, range, lunge speed, and lunge distance from that one data entry.
4. Rename the coarse `BasicAttack` action to `Attacking`, keeping existing behaviour unchanged.
5. **Completed and runtime-regressed:** add `currentAttackIndex` and current-data lookup, initially fixed to index `0`.
6. **Completed:** centralize reusable `StartAttackStep(int attackIndex)`. Keep only Attack1 active.
7. **Completed and runtime-verified:** route finish Events through `PlayerCombat`, perform centralized cleanup, and then return the coarse action to `Free`.
8. **Completed and Attack1-regressed:** add Animator `AttackIndex` and evolve `PlayerAnimator` to accept an index.
9. **Completed and visually verified:** add a unique Attack2 placeholder/state, second local override mapping, and indexed transitions.
10. **Authored and statically verified:** add independent `OpenComboWindow` and `CloseComboWindow` state plus Attack1 events. Runtime input does not consume the state yet.
11. **Completed and runtime-verified:** add `isAttackQueued` and route input so only a request during the valid Combo Window sets it. Attack2 remains absent.
12. **Completed and runtime-verified:** add the earliest `ComboTransitionPoint` runtime-ready boundary.
13. **Completed and runtime-verified:** consume queued input at or after the transition point and start Attack2 through the reusable initialization path.
14. **Completed and runtime-verified:** verify `Attack1 -> Free` and `Attack1 -> Attack2 -> Free`, including invalid timing, repeated input, two damage results, and final cleanup.
15. Add the separately authored Restart Window only after the two-hit path works: late-recovery input resets to Attack1, while startup input remains rejected.
16. **Exact resume point:** add explicit stale-event identity protection before enabling Restart Window or extending the chain; then test no-target, target-loss, target-death, and future Attack1 restart paths.
17. Review whether a hypothetical Attack3 requires copied combat-flow code. Refactor only if that review exposes duplication.

## Acceptance Criteria for the First Two-Hit Checkpoint

- Attack1 still works alone when no follow-up input is accepted.
- An input outside the Combo Window does not start or queue Attack2.
- One valid queued input produces exactly one Attack2.
- Repeated input does not skip, repeat, or multiply Attack2.
- Attack1 and Attack2 use their own data entries.
- Each step applies damage only during its own confirmed Hit Window.
- Facing and lunge use the current step's data and remain owned by the existing movement boundary.
- The player returns to `Free` after the final step.
- Adding a third data entry does not require copying a full attack-flow method.
- Unity script validation and the final Console are clean.

## Deferred Work

The first combo refactor does not include:

- Attack3;
- ScriptableObject attack assets;
- a generalized Skill System;
- a separate targeting component;
- combo damage scaling;
- Dodge or hit-reaction cancellation;
- per-attack VFX, SFX, Camera Shake, or Knockback;
- production hitstop tuning;
- enemy or Boss action-state refactoring.

## Learning and Portfolio Value

This refactor is valuable because it records a change from a verified single-use implementation to a reusable sequence while preserving behaviour at every migration step.

The important engineering evidence is not merely that a combo animation plays. It is that the project:

- separates attack configuration from per-execution state;
- separates coarse player actions from combo-step progression;
- keeps animation presentation separate from combat decisions;
- preserves a single movement owner;
- recognizes Animator Override and Animation Event constraints;
- uses regression checkpoints instead of rewriting the whole combat system at once.

## Update Rule

Keep the status as **Proposed** until implementation begins. During implementation, record only completed and verified steps. When the two-hit acceptance criteria pass, change the status to **Implemented and runtime-verified** and link the corresponding development-log checkpoint.

Keep this tracked document in every future sync to the code-focused GitHub repository so the published code and its architecture record do not drift apart.
