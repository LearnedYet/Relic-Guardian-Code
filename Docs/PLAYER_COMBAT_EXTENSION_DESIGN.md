# Player Combat Extension Design

Status: architecture plus Basic/Perfect Dodge gameplay and presentation direction approved on 2026-09-17. Thirty-two selected local Dodge AnimationClips are imported and validated, but no Dodge, Counter, Player HitStun or Player Death gameplay implementation is claimed by this document.

## Scope

This document records the minimum architecture direction for the next Player combat sequence:

1. Dodge
2. Perfect Dodge
3. Guard Counter
4. Dodge Counter
5. Sprint Attack
6. Player HitStun / HitReaction
7. Player Death

Actual code, saved Unity assets, current Editor state and Git status remain authoritative. `Docs/ARCHITECTURE.md` continues to describe implemented behavior only.

## Constraints

- Preserve the verified Basic Combo, Guard and Lock-On systems.
- Do not introduce an Event Bus, Dependency Injection, general Ability System, full State Pattern, Behavior Tree, GOAP, numeric Priority system or broad interface hierarchy.
- Add abstractions only when one of the listed concrete features creates a demonstrated need.
- Keep Apply Root Motion disabled and keep code-driven displacement.
- The learner remains the author of key gameplay and presentation code unless they explicitly request a bounded takeover.

## Approved Coarse Action Model

`PlayerActionController` remains the sole owner of the coarse Player action state and the deterministic same-frame action-request arbitration boundary.

The intended coarse states are:

```text
Free
Attacking
Blocking
Dodging
HitStunned
Dead
```

`PerfectDodge`, `PerfectGuard`, `BasicAttack`, `SprintAttack`, `GuardCounter` and `DodgeCounter` are not coarse states. Perfect outcomes belong to their action owner, while all four attack entries execute inside `Attacking`.

Input priority and authored cancellation permission remain separate decisions. A request winning a same-frame priority conflict does not automatically grant permission to cancel the action already in progress.

## Existing Boundary That Needs One Small Correction

Current `PlayerCombat.EndAttack()` both clears attack-owned runtime state and asks `PlayerActionController` to return to `Free`. The existing Attack-to-Block path therefore temporarily performs `Attacking -> Free -> Blocking` while the controller is replacing one action with another.

Before Dodge is allowed to interrupt an existing action, split these meanings:

- natural attack completion performs complete attack cleanup and then asks the controller to finish `Attacking`;
- replacement or forced interruption asks `PlayerCombat` to clean only its own runtime/presentation state, while `PlayerActionController` alone chooses the destination coarse state.

Do not create a general interruption framework for this. A small explicit cleanup seam is sufficient. Apply the same rule to `PlayerBlock` or `PlayerDodge` only when a real Counter, HitStun or Death transition needs to replace those actions.

## Dodge Ownership

The implemented base uses one independent `PlayerDodge` component.

`PlayerDodge` owns:

- the accepted Dodge execution lifetime;
- the start-direction snapshot;
- elapsed/progress state and the requested horizontal displacement for each frame;
- ordinary evasion and later Perfect Dodge windows;
- Dodge completion and Dodge-owned cleanup;
- the later Dodge Counter opportunity deadline/token.

`PlayerDodge` does not own:

- action admission or coarse state transitions;
- direct `CharacterController.Move` calls;
- direct Transform-facing writes;
- Animator-owned presentation;
- Counter attack execution.

`PlayerMovement` remains the sole `CharacterController` displacement and actual-facing boundary. It should provide the smallest reusable camera-relative direction calculation needed to snapshot current input without reading a stale previous-frame movement direction, plus an explicit Dodge displacement request. Do not create `PlayerMotor` before multiple concrete displacement modes prove that split is useful.

`PlayerAnimator` presents Dodge but never decides its invulnerability, permission or gameplay finish. `PlayerHitReceiver` remains the single incoming-hit entry and asks `PlayerDodge` whether a hit is evaded before forwarding an unhandled hit to Block/Health according to the current coarse state.

## First Dodge Contract

The current grounded base implementation uses this contract:

- grounded `Free -> Dodging` entry;
- the existing ordinary four-step Basic Attack may cancel into Dodge through its shared cleanup boundary;
- Blocking, Dodging, Jump and future Guard/Dodge Counter executions do not gain Dodge cancellation from request priority;
- ordinary movement, Sprint and Jump remain unavailable during `Dodging`;
- the start direction is captured once rather than following later stick changes;
- gameplay lifetime and movement remain code-owned; Animator state exit is not the sole permission boundary;
- same-frame incoming hits continue to call `ResolveActionRequests()` before defense resolution so an accepted Dodge can be observed deterministically.

The learner has now approved the direction and presentation contract below. Runtime tuning values remain intentionally open so they can be adjusted in Play Mode.

## Approved Direction Policy

Dodge direction is captured once when the Dodge begins and never follows later input during the same Dodge.

Without Lock-On:

- valid movement input uses the same camera-relative direction basis as Free Movement;
- no movement input falls back to the opposite of the Player's current actual facing.

With Lock-On:

- valid movement input uses target-relative direction;
- forward moves toward the target and backward moves away from it;
- left/right move around the target in the matching side direction;
- diagonal input combines the matching forward/backward and side directions;
- no movement input falls back to moving away from the target.

`Dodge Distance`, `Dodge Duration`, I-Frame timing, Perfect Window timing and Slow Motion parameters must remain easy to tune at runtime. Their final numbers are not approved yet. Gameplay distance and lifetime remain code-owned and must not be derived solely from AnimationClip length.

## Approved Perfect Dodge Result and Presentation

Perfect Dodge is a gameplay result determined only after an incoming hit reaches `PlayerHitReceiver` while the Dodge's narrower Perfect Window is active. A successful result avoids that hit and opens a future Dodge Counter opportunity owned by `PlayerDodge`.

The fixed presentation direction is:

1. leave one current-pose human afterimage at the Player's original position;
2. add a very shallow movement trail along the Dodge direction;
3. add a subtle air/space distortion at the success location;
4. request brief Slow Motion through the project's sole time-control boundary;
5. play a dedicated Perfect Dodge success SFX.

The intended feeling is that the enemy attack passes through where the Player was a moment earlier. Do not turn this into a large explosion, shockwave, strong magical buff, Perfect Guard-style collision, or a long sequence of heavy afterimages.

Create a small `PlayerDodgePresentation` boundary when presentation connection begins. It may expose replaceable slots/hooks for Afterimage, Shallow Trail, Distortion, normal Dodge SFX and Perfect Dodge SFX. It may request Slow Motion, but it must not write `Time.timeScale` directly. The existing `HitstopController` is currently the sole time-scale writer; minimally extend or generalize that single boundary only when Slow Motion is actually connected.

Presentation never decides Dodge admission, I-Frames, Perfect classification, damage avoidance, Counter opportunity, gameplay completion or `PlayerActionState`. Replacing any final VFX/SFX resource must not require changing core Dodge judgement.

## Imported Local Animation Resources

Thirty-two selected Humanoid AnimationClips were copied with their original `.meta` files from the isolated AssetLab into the ignored local-only folder `Assets/LocalLicensed/SwordAnimationPack/Dodge`:

- `01_Dodge`: eight grounded free-direction clips;
- `02_Dodge_Combat`: eight grounded combat-direction clips;
- `05_Dodge_to_Run`: eight ordinary grounded transition clips;
- `06_Dodge_Combat_to_Run`: eight combat grounded transition clips.

Air, Fast, FBX duplicate and unrelated animation-pack assets were not imported. GUIDs were preserved and no pre-existing main-project GUID collision was found. Unity recognized all 32 clips as Humanoid assets. Ground Dodge clips are 1.333 seconds at 60 FPS and to-Run clips are 0.667 seconds at 60 FPS. The imported local copies alone were changed to non-looping; the AssetLab source was not changed. Apply Root Motion remains disabled.

Filename-based candidate mapping:

- Free Dodge: `Dodge_F`, `Dodge_B`, `Dodge_L`, `Dodge_R`, `Dodge_F_L_45`, `Dodge_F_R_45`, `Dodge_B_L_45`, `Dodge_B_R_45`.
- Combat Dodge: `Dodge_Combat_F`, `Dodge_Combat_B`, `Dodge_Combat_L`, `Dodge_Combat_R`, `Dodge_Combat_F_L_45`, `Dodge_Combat_B_L_45`, `Dodge_Combat_B_R_45`, plus `Dodge_Combat_R_L_45` as the provisional missing front-right 45-degree candidate.
- Ordinary and Combat to-Run use their corresponding standard F/B/L/R and 45-degree names.

`Dodge_Combat_R_L_45` requires the learner's later visual confirmation because its filename is ambiguous. Gameplay direction must not depend on this presentation mapping. Dodge-to-Run clips are visual recovery only: when gameplay Dodge has ended and valid movement input remains, `PlayerAnimator` may select the direction-appropriate ordinary or Combat transition to avoid `Dodge -> Idle -> Run`. They never own gameplay state or require new coarse action states.

## File Scope and Completed Base Slice

Implemented and future scripts:

- `PlayerDodge.cs`: currently owns base Dodge lifetime, direction snapshot, movement progress and completion; windows, hit result and later opportunity ownership remain future additions;
- `DodgeResult.cs`: the smallest explicit gameplay result needed by hit routing;
- `PlayerDodgePresentation.cs`: replaceable visual/audio/Slow-Motion request hooks.

Expected existing integration points are the Player Input Actions asset, `PlayerInputReader`, `PlayerActionState`, `PlayerActionController`, `PlayerMovement`, `PlayerAnimator`, `PlayerHitReceiver`, the existing hit-result data, the sole time-control boundary, Animator configuration and the current Player instance. Base Dodge does not require rewriting `PlayerCombat` or `PlayerBlock`.

The completed base slice was:

1. add a `Dodge` Button action with a mouse back-button binding to `Assets/RelicGuardian/Player/RelicGuardianPlayer.inputactions`;
2. add a one-use `dodgeRequested` request, `OnDodge()` callback and `ConsumeDodge()` method to `PlayerInputReader`;
3. add and verify `Dodging`, `PlayerDodge`, code-owned movement and presentation independently from Perfect Dodge.

## Same-Frame Arbitration and Cancellation

Keep all mutually exclusive one-use requests in `PlayerActionController.ResolveActionRequests()` with the existing once-per-frame gate. The recommended first Dodge order is:

```text
Dodge -> Block -> Attack -> Jump
```

This preserves the existing relative `Block -> Attack -> Jump` order while making the new committed evasion win its new same-frame conflicts. If play design later chooses Block over Dodge, change the explicit order rather than relying on `MonoBehaviour.Update()` order.

Initial cancellation matrix:

| Current state | Initially accepted behavior |
| --- | --- |
| `Free` | Fixed priority chooses Dodge, Block, Attack or Jump. |
| ordinary Basic `Attacking` | Attack remains Combo/Restart input; existing Block cancellation is preserved; Dodge may replace it through shared attack cleanup. |
| future Guard/Dodge Counter `Attacking` | Dodge is rejected unless that counter later authors an explicit permission of its own. |
| `Blocking` | Block lifecycle continues; future Guard Counter requires a valid opportunity. |
| `Dodging` | Other action requests are rejected in the first version. |
| `HitStunned` | Action requests are rejected. |
| `Dead` | Action requests are rejected permanently. |

Any cancellation beyond the implemented ordinary Basic-Attack-to-Dodge rule must use explicit authored permission owned by the action being left. Input priority alone never grants that cancellation.

## Shared Attack Execution

Basic Attack, Sprint Attack, Guard Counter and Dodge Counter continue to use one `PlayerCombat` executor for:

- target selection and confirmation;
- attack facing and lunge requests;
- Hit/Combo/Restart windows where applicable;
- damage delivery;
- presentation requests;
- complete attack-owned cleanup.

Do not create four copied attack components or four copied combat flows.

Do not introduce `PlayerAttackType` for Dodge itself. Introduce the smallest attack-entry identity when the first real non-Basic attack is connected, expected to be Guard Counter. At that point it has concrete value for selecting data/presentation and distinguishing, for example, `Basic[0]` from `GuardCounter[0]`; the current attack index alone cannot identify both families safely.

Keep `PlayerAttackData` as inline serializable configuration until multiple concrete attack sets or multiple Player/weapon owners demonstrate a need for shared ScriptableObject assets.

## Counter Opportunity Ownership

- `PlayerBlock` stores and expires the Guard Counter opportunity created by the qualifying Guard result.
- `PlayerDodge` stores and expires the Dodge Counter opportunity created by the qualifying Dodge result.
- The opportunity owner may expose eligibility and one-use consumption, but never starts an attack itself.
- `PlayerActionController` decides whether an Attack request may use the opportunity and admits the transition to `Attacking`.
- `PlayerCombat` executes the selected Counter attack through the shared attack flow.

This allows a short opportunity to remain valid after `Blocking` or `Dodging` returns to `Free` without giving defense components attack authority.

## HitStun, Health and Death Direction

Do not change `PlayerHealth` for Dodge. When Player HitStun and Death become the active feature:

- `PlayerHealth` owns the numeric value, clamping and `IsAlive`/lethal result only;
- `PlayerHitReceiver` remains the accepted-hit router and requests the consequence after damage;
- `PlayerActionController` owns entry into `HitStunned` or terminal `Dead`;
- the interrupted action owner performs its own cleanup without choosing a competing destination state;
- `PlayerAnimator` presents HitReaction/Death without granting recovery permission.

## Current Composition Warning

The current live `SampleScene` Player instance is connected to `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`, but `PlayerBlock`, `PlayerHitReceiver`, `PlayerGuardPresentation`, `HitstopController` and `PlayerAttackPresentation` are Scene-added components rather than components saved on the Prefab asset.

This does not block the current single-Scene Dodge implementation, but the Prefab alone is not a complete reusable Player. Do not combine a broad Prefab/Scene migration with Dodge while the mixed Player Prefab, Animator and Scene files remain protected and contain local presentation wiring. Resolve composition ownership later as a separate, explicitly scoped task.

## Approved Implementation Order

1. Separate attack-owned cleanup from the destination coarse-state decision before the first new transition needs it.
2. Implement and runtime-verify base Dodge from the learner-approved effect/control design.
3. Add and verify Perfect Dodge as a narrower timing/result/presentation slice.
4. Add Guard Counter; introduce the minimum attack-entry identity at this first non-Basic attack.
5. Add Dodge Counter by reusing the same attack-entry and execution path.
6. Add Sprint Attack through the same `Attacking` owner and shared executor.
7. Add Player HitStun / HitReaction and forced interruption cleanup.
8. Add terminal Player Death and death-safe control recovery boundaries.

Every slice must preserve the existing Basic Combo, Guard, Lock-On, CharacterController ownership and deterministic same-frame arbitration, and must receive focused Play Mode and Console verification before being recorded as implemented.
