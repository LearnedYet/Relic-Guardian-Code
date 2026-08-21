# Combo Attack Architecture and Refactor Plan

## Status

- **Implementation in progress; two-hit combo not yet functional**
- Read-only inspection completed: 2026-08-20
- Final four-clip light-attack asset set prepared: 2026-08-21
- Attack1 replacement, one-entry attack data, indexed runtime foundation, reusable step initialization, and authored Combo Window boundaries completed: 2026-08-21.
- This document records both verified implementation and intended architecture. Unimplemented items are labelled explicitly and must not be treated as evidence that Attack2 works.

## Purpose

Extend the verified single Basic Attack into a reusable two-hit combo without copying a complete attack flow for each animation.

The first implementation target is deliberately limited to:

```text
Attack1
-> optional queued input during a Combo Window
-> Attack2
-> Free
```

The design should later allow Attack3 to be added mainly through another attack-data entry plus animation configuration, rather than another copied block of `PlayerCombat` logic.

Adding Attack3 will still require an Animator state or override slot, transitions, Animation Events, and runtime verification. The goal is to avoid duplicated C# attack-flow logic, not to eliminate all content setup.

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
- independent `isHitWindowOpen` and `isComboWindowOpen` states plus their open/close event receivers.

The Prefab currently configures one Attack1 entry: damage `1`, target range `2`, lunge speed `5`, and lunge distance `1`. `Update()` still uses the original compound initial-attack condition, so an Attack request received while the coarse action is not `Free` is consumed and discarded. The Combo Window exists and is animation-authored, but no input reads it and no queue exists yet.

### PlayerActionController

`PlayerActionController` currently owns one authoritative coarse action state:

```text
Free
BasicAttack
```

It grants movement and Jump only while `Free`, accepts a grounded Basic Attack only from `Free`, and returns directly to `Free` through `FinishBasicAttack()`.

### PlayerAnimator

`PlayerAnimator` synchronizes locomotion and airborne parameters and exposes only:

```text
PlayAttack() -> SetTrigger("Attack")
```

It does not select an attack index and does not decide gameplay rules.

### PlayerMovement

`PlayerMovement` remains the owner of `CharacterController` displacement. `PlayerCombat` supplies an attack direction and requested distance through `MoveDuringAttack()`.

This responsibility boundary is already suitable for multiple attack steps and should be preserved.

### PlayerInputReader

`PlayerInputReader` stores one Attack request and clears it when `ConsumeAttack()` is called. It does not and should not decide whether an attack starts or becomes a queued combo input.

### Current Animation Events

The active local Attack1 override currently contains:

| Approximate frame | Normalized time | Event | Current receiver |
| --- | --- | --- | --- |
| `9.7` | `0.2771416` | `OpenHitWindow` | `PlayerCombat` |
| `11.6` | `0.3302259` | `OpenComboWindow` | `PlayerCombat` |
| `12.3` | `0.3512039` | `CloseHitWindow` | `PlayerCombat` |
| `21.7` | `0.6189415` | `CloseComboWindow` | `PlayerCombat` |
| `34.2` | `0.9763113` | `FinishBasicAttack` | `PlayerActionController` |

Hit and Combo Window events enter the combat coordinator independently. The finish event still bypasses `PlayerCombat`, so combat runtime state does not yet have a single end-of-attack cleanup boundary. `ComboTransitionPoint` and Restart Window events do not exist yet.

### Current Animator Structure

The project-owned Animator Controller currently has:

- one `Attack` Trigger;
- one `BasicAttack` state;
- one locomotion-to-`BasicAttack` transition using the Trigger;
- one unconditional `BasicAttack` exit transition at Exit Time `0.9`;
- no `AttackIndex` parameter;
- no Attack2 state.

The active local `KatanaAnimationOverrides` contains one mapping from the tracked `SwordAndShieldSlash` placeholder to the local Attack1 clip.

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

The current Controller cannot select Attack2 because it has only one attack state and one Trigger.

The current Animator Override Controller also has only one unique original-clip key. Reusing the same original `SwordAndShieldSlash` Clip in both Attack1 and Attack2 states would not provide two independent override slots; both states would resolve through the same original-clip mapping.

To preserve the existing tracked-project versus ignored-licensed-assets boundary, the combo Animator will need two distinct tracked placeholder Clips or another explicitly verified equivalent setup. The local Override Controller can then map each unique placeholder to its matching licensed Attack1 or Attack2 clip.

Attack2 must be previewed and accepted before its state, transitions, and events are authored. Attack3 is intentionally deferred until the two-hit architecture passes.

## Main Risks

### Consumed Input Is Currently Lost During an Attack

The current compound condition consumes the Attack request before it knows whether a new Basic Attack can start. Combo implementation must split input consumption from the decision:

- start Attack1 while `Free`;
- queue Attack2 only while `Attacking` and the Combo Window is open;
- ignore all other cases deliberately.

### An Outgoing Animation Event Could Affect a New Step

If Attack1 transitions into Attack2 while an outgoing Attack1 event can still fire, a late finish or window event could mutate Attack2 state.

The same risk applies when the later Restart Window starts a fresh Attack1 before the previous Attack1 reaches its old `FinishAttack`. The minimal two-hit test must inspect this behaviour. Events should carry an attack-step/sequence identity, or use another explicit guard, so `PlayerCombat` rejects stale events that do not belong to the current execution.

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
7. Route the finish Animation Event through `PlayerCombat`, perform centralized cleanup, and then return the coarse action to `Free`.
8. Add an Animator attack-index parameter and evolve `PlayerAnimator` to accept an index. Verify Attack1 before adding another state.
9. Add a unique Attack2 placeholder/state and a second local override mapping. Configure and preview its transitions.
10. **Authored and statically verified:** add independent `OpenComboWindow` and `CloseComboWindow` state plus Attack1 events. Runtime input does not consume the state yet.
11. **Exact resume point:** add `isAttackQueued` and route input so only a request during the valid Combo Window sets it. Do not add Attack2 in this step.
12. Add the earliest `ComboTransitionPoint`, remember whether it has passed, and start Attack2 through the reusable initialization path. Inputs queued before the point wait; valid inputs after the point may transition immediately.
13. Verify both paths: `Attack1 -> Free` without input and `Attack1 -> Attack2 -> Free` with valid input.
14. Add the separately authored Restart Window only after the two-hit path works: late-recovery input resets to Attack1, while startup input remains rejected.
15. Add stale-event protection, then test invalid timing, repeated input, no-target attacks, target loss, target death, Attack1 restart, and animation-event cleanup.
16. Review whether a hypothetical Attack3 requires copied combat-flow code. Refactor only if that review exposes duplication.

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
