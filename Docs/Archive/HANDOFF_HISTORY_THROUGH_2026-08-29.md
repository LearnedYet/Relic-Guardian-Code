# Relic Guardian - Handoff

## Project

- Name: Relic Guardian
- Purpose: A third-person action-game demo for a Unity internship portfolio.
- Primary goal: Learn to independently develop and explain Unity C# systems, not merely finish a demo with generated code.
- Engine: Unity 6.3 LTS
- Exact editor version: Unity `6000.3.19f1`
- Render pipeline: URP
- Input: Unity Input System `1.19.0`
- Character-material dependency: lilToon `2.3.3`, installed from the official Git repository through the pinned URL `https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#2.3.3`.
- Character movement: `CharacterController`
- Editor: Visual Studio Code

## Core Demo Scope

- Third-person movement and camera
- Jump
- Dodge and perfect dodge
- Melee combat and hitstop
- Enemy AI
- One small level and one boss battle

Do not expand scope before the core demo is playable.

## Project Rules

- Communicate in Chinese by default; keep code and Unity API names in English.
- Before asking the learner to create a new identifier, split its English words, translate them into Chinese, and explain the complete naming intent. The detailed rule is in `DEVELOPMENT_RULES.md`.
- Do not modify Starter Assets official source code. It is retained for reference only.
- Work on one small feature at a time.
- Before implementing: explain the design, required Unity/C# concepts, and the small steps.
- The learner attempts key code first. Review and explain it before making edits.
- A correct prediction, scaffold answer, or request to "continue" is not permission for Codex to write key code. Ask the learner to edit the actual file and wait, unless they explicitly request implementation help or remain blocked and ask Codex to take over.
- Treat maintainability primarily as structural extensibility and low coupling: adding a related feature should require local changes rather than rewriting the existing flow or spreading cross-component checks. Check functional correctness, structural extensibility, and readability separately.
- The learner may express needs informally. When intent is ambiguous, restate the goal and assumptions, then ask at most one focused question before a consequential decision.
- Test each feature in Unity before proceeding.
- After a verified task: update `ROADMAP.md` and `DEV_LOG.md`, then create a focused Git commit.

## Current Files

```text
Assets/
  RelicGuardian/
    Player/
      RelicGuardianPlayer.inputactions
      RelicGuardianPlayer.prefab
      Animations/
        SwordAndShieldSlash.fbx
      Animator/
        RelicGuardianPlayer.controller
      Scripts/
        PlayerInputReader.cs
        PlayerMovement.cs
        PlayerAnimator.cs
        PlayerCombat.cs
        PlayerAttackData.cs
        PlayerActionState.cs
        PlayerActionController.cs
        PlayerHealth.cs
    Enemy/
      Materials/
        StartupTelegraph.mat
      Scripts/
        EnemyAI.cs
        EnemyAttack.cs
        EnemyAttackPhase.cs
        EnemyHealth.cs
        EnemyMovement.cs
Docs/
  DEVELOPMENT_RULES.md
  PROJECT_PLAN.md
  ROADMAP.md
  DEV_LOG.md
  HANDOFF.md
  LEARNING_PROGRESS.md
```

## Current Implementation

### Input

`RelicGuardianPlayer.inputactions` contains a `Player` action map:

- `Move`: `Vector2`, bound to WASD.
- `Look`: `Vector2`, bound to mouse delta.
- `Jump`: `Button`, bound to `Space [Keyboard]`.
- `Attack`: `Button`, bound to the left mouse button.

The scene's `PlayerInput` uses these actions with Send Messages behavior.

In this project's current Input Actions Editor, selecting `Jump` shows `Action Properties > Action Type = Button`. A separate `Control Type` field is not displayed. The serialized `expectedControlType` field is currently empty; do not manually change it merely to match an older tutorial.

### PlayerInputReader

`PlayerInputReader.cs` stores `moveInput` and `lookInput`, receives `OnMove(InputValue)` and `OnLook(InputValue)`, and exposes read-only `MoveInput` and `LookInput` properties. `OnJump()` and `OnAttack()` record separate one-use requests; `ConsumeJump()` and `ConsumeAttack()` return and clear their respective requests.

### PlayerMovement

`PlayerMovement.cs` currently:

- Requires `CharacterController`.
- Obtains `CharacterController`, `PlayerInputReader`, and `PlayerActionController` in `Awake()`.
- Reads `inputReader.MoveInput` in `Update()`.
- Replaces only the local horizontal input with `Vector2.zero` when `PlayerActionController.CanMove` is false. This blocks horizontal movement and turning without skipping gravity, Grounded handling, or the final `CharacterController.Move()`.
- Reads flattened and normalized `Main Camera` forward/right directions and combines them with `Vector2` input for camera-relative horizontal movement.
- Smoothly rotates toward non-zero movement direction.
- Accumulates gravity, moves through `CharacterController.Move()`, and resets downward velocity to `-2f` when the latest movement reports Grounded.
- Consumes the Jump request each frame and assigns an Inspector-adjustable positive `jumpSpeed` only while grounded and `PlayerActionController.CanJump` is true.
- Stores `currentSpeed = moveDirection.magnitude * moveSpeed` and exposes it through the read-only `CurrentSpeed` property for animation synchronization.
- Exposes `CurrentMovementStrength` for locomotion playback, `IsGrounded` as a read-only view of `CharacterController.isGrounded`, `IsJumping` as an accepted-jump state that persists until landing, and computed `IsFalling` from Grounded plus downward vertical velocity.
- Exposes `MoveDuringAttack(Vector3 direction, float distance)` as the movement-owned boundary for code-driven attack displacement through the existing `CharacterController`.

### PlayerAnimator

`PlayerAnimator.cs` currently:

- Obtains `Animator` and `PlayerMovement` through `GetComponent<T>()` in `Awake()`.
- Writes all five required reference-controller parameters: `Speed`, `MotionSpeed`, `Grounded`, `Jump`, and `FreeFall`.
- Uses playback speed `1` while movement strength is `0`, so the locomotion Blend Tree's Idle clip continues playing instead of freezing on its first frame. Non-zero movement still supplies `CurrentMovementStrength`.
- Exposes `PlayAttack()`, which sets the project-owned Animator Controller's `Attack` Trigger.
- Is attached to the reusable `RelicGuardianPlayer` Prefab.
- Locomotion, `Grounded`, `Jump`, and `FreeFall` synchronization are verified.

### Main-Project Asset Integration Preparation

- The main project now has official lilToon `2.3.3` installed as a Git package. Unity Package Manager reports `jp.lilxyzw.liltoon@2.3.3`, and the post-install Console check returned zero errors.
- This installation currently changes `Packages/manifest.json` and `Packages/packages-lock.json` and creates `ProjectSettings/lilToonSetting.json`; those changes are not committed yet.
- P09 character assets now live locally under the ignored `Assets/LocalLicensed/P09_Modular_Humanoid/` path. Another workstation must import the exact `P09ModularHumanoidLite v2026.01.05.unitypackage` before the player Prefab's P09 references can resolve. The narrowed katana Humanoid test FBX, required source Avatar, and `KatanaAnimationOverrides` also live under ignored `Assets/LocalLicensed/PowerfulSwordPack/`; the resource-audit scene was not imported.
- Resource compatibility was verified separately in `C:\Unity\Project\RelicGuardianAssetLab`. Do not confuse that disposable project's successful scene with completed main-project integration.
- The visual replacement is complete: `P09_Human_Variant_Female` is a child below the preserved `RelicGuardianPlayer` root, while the old `Geometry` and `Skeleton` branches are inactive. `PlayerCameraRoot` and every gameplay component remain root-owned.
- This player Prefab replacement is intentionally local-only and is not committed because its P09 references cannot resolve without the licensed package. The tracked repository keeps the previous prototype-player Prefab for a code-focused interview checkout.
- The root is still the single active Animator owner and now uses `P09_BodyAvatar`; the nested P09 Animator is disabled and Apply Root Motion remains off. Ordinary movement was verified in Play Mode.
- Existing animation retargeting causes minor visual clipping on the differently proportioned P09 model. Accept it at this checkpoint and leave correction for the later animation-replacement stage.
- Do not run the P09 package's nested lilToon `1.x.x` installer in the main project. It installed lilToon `1.10.3` in the audit project, which failed to compile under Unity `6000.3.19f1` and URP `17.3.0`.

### Basic Attack Checkpoint

- `SwordAndShieldSlash.fbx` remains the tracked base AnimationClip and override key. The local presentation now replaces it with `Attack_3Combo_1_Move_HumanoidTest` through ignored `KatanaAnimationOverrides`.
- `RelicGuardianPlayer.controller` is a project-owned Animator Controller; the official Starter Assets controller was not modified.
- `BasicAttack` uses Animator state speed `1.5`; the learner reported that this is close to the intended 3D action-RPG feel.
- `PlayerActionState` deliberately contains only `Free` and `BasicAttack`.
- `PlayerActionController` is attached to the reusable player Prefab and is the single owner of `currentActionState`.
- Its unassigned enum field starts as `Free` because `Free` is the enum member with numeric value `0`.
- `CurrentActionState` exposes a read-only view. `CanMove` and `CanJump` are derived from the authoritative state rather than stored as duplicate booleans.
- `TryStartBasicAttack(bool isGrounded)` accepts only a grounded request while the current state is `Free`, changes the state to `BasicAttack`, and returns whether the request succeeded.
- `PlayerCombat` consumes the one-use Attack request, passes `PlayerMovement.IsGrounded` into the controller, and plays the animation only when the controller accepts the request.
- Repeated Attack requests during `BasicAttack` and grounded-basic-attack requests made while airborne are consumed and rejected; neither request is buffered.
- The imported clip has a `FinishBasicAttack` Animation Event at normalized time `0.88888884`. It calls `PlayerActionController.FinishBasicAttack()` and returns the action state to `Free`.
- `PlayerCombat` owns a private `isHitWindowOpen` bool, exposes it through the read-only `IsHitWindowOpen` property, and provides `OpenHitWindow()` and `CloseHitWindow()` for animation-timed state changes.
- `SwordAndShieldSlash` opens the Hit Window at Frame 18 (serialized normalized time `0.40240237`) and closes it at Frame 30 (`0.6696685`). The existing `FinishBasicAttack` Event remains later in the clip.
- A temporary runtime log check produced exactly one open callback followed by one close callback for one grounded attack. The logs were removed afterward, and the clean attack/recovery regression completed with an empty Console.
- The Hit Window mechanism is reusable, while its start/end timing is AnimationClip-specific and should be retuned when the attack animation changes.
- `OpenHitWindow()` now re-queries the current candidates through `IsCurrentAttackTargetInRange()`. It confirms only the exact `currentAttackTarget` Collider saved at attack start, stores it in `confirmedAttackTarget`, and does not retarget when another candidate is available.
- A missing saved target or a saved target outside the current query produces `confirmedAttackTarget = null`. `CloseHitWindow()` also clears it, limiting the confirmation lifetime to the Hit Window.
- Temporary logs verified both the confirmed and missed branches, then were removed. The final script compiled with zero diagnostics; a clean in-range/no-target attack and movement-recovery regression ended with an empty Console.
- The local katana clip carries the same three-callback contract at approximately Frames `11`, `20.6`, and `37`. The root Animator uses the override Controller with the P09 Avatar and Apply Root Motion off; the nested P09 Animator remains disabled.
- The learner's final Play Mode regression reported the new animation, stationary root, right-hand weapon following, one damage result, and normal movement/Jump/Attack recovery all working. The final Console contained zero errors and zero warnings.
- During `BasicAttack`, horizontal movement, turning, and Jump are blocked while gravity and Grounded handling continue. Movement and Jump become available again after the explicit end event.
- Runtime checks verified first attack, rejected repeated attack, attack-end recovery, movement/turn blocking and recovery, Jump blocking and recovery, gravity continuity, airborne Basic Attack rejection without delayed playback, grounded Basic Attack regression, and simultaneous Attack/Jump mutual exclusion.
- `PlayerCombat` and `PlayerMovement` both use the default script execution order `0`. Same-frame Attack/Jump therefore has no guaranteed winner; the current minimum layer guarantees only that both cannot succeed together.
- Candidate acquisition, attack-start target selection, Hit Window-time confirmation, and one fixed prototype damage result are implemented separately below. Zero-health handling, death, combo, Dodge, Skill, hit effect, and broader combat-effect logic have not been implemented.
- The bounded startup lunge is now implemented and runtime-verified. `PlayerCombat` owns speed, maximum distance, travelled budget, saved-target direction, and active lifetime; `PlayerMovement.MoveDuringAttack()` owns the displacement call.
- Every accepted attack resets travelled distance. A lunge starts only when the existing target selection saved a target, clamps each frame step to the remaining budget with `Mathf.Min`, and stops at the maximum distance or `OpenHitWindow()`.
- Opposite Play Mode tests verified an in-range target produces the expected startup lunge, while an out-of-range/no-target attack still animates but neither turns nor lunges. Target stop distance remains a later feel/collision refinement if needed.

### Minimal Enemy Health and Confirmed Damage Checkpoint

- `EnemyHealth` owns serialized prototype `currentHealth = 3` and exposes `TakeDamage(int damageAmount)` as the boundary for subtracting health.
- `NearTarget` and `FarTarget` each have an independent `EnemyHealth` component in `SampleScene`.
- After `OpenHitWindow()` confirms the exact saved Collider, `PlayerCombat` obtains that Collider's `EnemyHealth`, checks it is not `null`, and calls `TakeDamage(1)`.
- Damage remains single-target: another candidate is never substituted, and a Collider without `EnemyHealth` is safely skipped.
- A standalone runtime call verified `NearTarget 3 -> 2`, with the saved value returning to `3` after leaving Play Mode.
- One complete real attack changed `NearTarget 3 -> 2` while `FarTarget` remained `3`. The attack animation has one verified `OpenHitWindow` Event, so this path currently calls damage once per attack.
- The final repeat test ended with an empty Console and a live scene scan found no missing-script components.
- Health clamping, zero-health response, death, destruction, hit reaction, configurable damage data, and effects are not implemented.

### Player-Owned Health Receiver Checkpoint

- `PlayerHealth` owns serialized prototype `currentHealth = 3` and exposes `TakeDamage(int damageAmount)` as the player-side health mutation boundary.
- It is attached to the existing `RelicGuardianPlayer` root, not the P09 visual child. The local Prefab attachment was saved and MCP verified the component with value `3`.
- A temporary non-persistent component test changed `3 -> 2`; the real player component remained at its saved value `3` afterward.
- The learner wrote the health field and subtraction behaviour. Codex changed formatting only.
- Because the current Prefab working copy also contains ignored licensed presentation references, the Prefab remains unstaged. A fresh code-focused checkout must attach `PlayerHealth` to the tracked prototype player root before the enemy attack is connected.
- Maximum health, clamping, player death, UI, and hit reaction remain intentionally absent. Enemy attack timing and the first minimal range-gated caller now exist in separate components.

### Enemy-to-Player Damage Boundary Checkpoint

- `EnemyAttack` owns serialized prototype `attackDamage = 1` and exposes `ApplyDamage(PlayerHealth target)`.
- The method returns immediately for a missing target; otherwise it calls the player-owned `TakeDamage(attackDamage)` boundary rather than editing player health directly.
- Only `NearTarget` owns `EnemyAttack` in `SampleScene`. `FarTarget` remains a second candidate for the existing player target-selection checks.
- A real Play Mode call from `NearTarget` changed the real player's health from `3 -> 2`. The Console stayed empty, and leaving Play Mode restored the saved player value to `3`.
- The automatic enemy Hit Window calls this boundary once against the target saved at attack start. `EnemyAI` is now the first real gameplay caller and applies an Inspector-configured distance gate; target search, chase, movement, animation or telegraph presentation, death, and effects remain absent.

### Enemy Attack Automatic Phase Timing Checkpoint

- `EnemyAttackPhase` defines `Ready`, `Startup`, `HitWindow`, and `Recovery` as four mutually exclusive phases.
- `EnemyAttack` stores private `currentPhase` and exposes the read-only `CurrentPhase` property.
- The field begins in `Ready` because the first enum member has numeric value `0`; no separate serialized phase value is stored.
- `TryStartAttack(PlayerHealth target)` accepts only `Ready && target != null`, saves the supplied target, changes the phase to `Startup`, and returns `true`. Rejected requests return `false` without replacing the current target or changing state.
- `OpenHitWindow()` changes the phase only from `Startup` to `HitWindow` and calls `ApplyDamage(currentAttackTarget)` once during that successful transition; calls in `Ready` or another phase leave the state and health unchanged.
- `CloseHitWindow()` changes the phase only from `HitWindow` to `Recovery`; early or repeated calls leave the state unchanged.
- The learner wrote `FinishRecovery()` in the actual project file. It changes only `Recovery` to `Ready` and clears `currentAttackTarget`; invalid calls leave the phase and target unchanged.
- Standard validation passed with zero diagnostics. Temporary non-persistent component tests verified rejected calls, the accepted `Ready -> Startup -> HitWindow -> Recovery -> Ready` loop, and a second accepted attack start.
- `phaseElapsedTime` stores the current non-`Ready` phase's elapsed time across frames. Every accepted transition resets it to `0`; rejected calls preserve it.
- `Update()` accumulates `Time.deltaTime` only when the phase is not `Ready`, then uses one `if`/`else if` chain to perform at most one automatic transition per update.
- Serialized values `startupDuration = 0.5f`, `hitWindowDuration = 0.2f`, and `recoveryDuration = 0.4f` drive the automatic `Startup -> HitWindow -> Recovery -> Ready` sequence.
- A temporary non-persistent component test verified all three timed transitions, one `3 -> 2` health change when the Hit Window opened, no repeated damage, target cleanup after recovery, and a second accepted attack storing a different target. Standard validation reported zero errors and zero warnings, and the final Console error query returned no entries.
- The learner entered the three duration fields, all three automatic conditions, and all target-lifetime and timed-damage behaviour. Codex only corrected earlier unambiguous spelling/formatting, reviewed one earlier semantic duration mismatch, and performed verification; it did not edit this checkpoint's gameplay code.
- The phase machine remains independent of target search, animation or telegraph presentation, movement, and model integration. Its real caller and distance gate are recorded separately below.

### Range-Gated Enemy Attack Caller Checkpoint

- `EnemyAI` stores serialized `EnemyAttack enemyAttack`, `PlayerHealth attackTarget`, and prototype `attackRange = 2f` component fields for local Inspector wiring.
- Each `Update()` calculates `distanceToTarget` from the enemy and target Transform positions with `Vector3.Distance`.
- Only `distanceToTarget <= attackRange` requests `enemyAttack.TryStartAttack(attackTarget)`. `EnemyAttack` remains responsible for accepting only `Ready`, preserving the phase timer, applying Hit Window damage, and completing recovery.
- The learner wrote the component fields, local distance value, comparison, and attack request. Codex changed only the unambiguous spacing in `if (` before reviewing and verifying the code.
- With the enemy and player approximately `1.2` units apart, `attackRange = 0.5` produced no damage, while `attackRange = 2` produced one `3 -> 2` health change after the `0.5`-second Startup. Standard script validation and the final Console both reported zero errors and zero warnings.
- The local `SampleScene` wiring is intentionally not part of the focused code checkpoint because that tracked scene also contains existing local player Prefab references and unrelated serialized changes. A fresh code-focused checkout must wire the three `EnemyAI` fields in its own scene.
- This is a minimal range-gated attack caller, not a complete enemy AI. It does not find the player automatically, chase, turn, cancel an already-started attack when the player leaves range, drive the prepared Startup telegraph, handle death, or choose among multiple attacks.

### Startup Telegraph Placeholder Preparation

- At the learner's explicit request, Codex created only the visual placeholder setup; no attack or presentation-control code was written.
- `NearTarget/StartupTelegraph` is a child Cylinder with local position `(0, -0.52, 0)`, local scale `(0.8, 0.02, 0.8)`, and default inactive state.
- The child remains on the `Default` Layer so the player's `HitTarget` query does not treat it as an enemy candidate. Its generated `CapsuleCollider` was removed, and its Renderer neither casts nor receives shadows.
- Project-owned material `Assets/RelicGuardian/Enemy/Materials/StartupTelegraph.mat` uses `Universal Render Pipeline/Unlit` with an opaque bright-red base colour.
- `SampleScene` was saved. Two temporary warnings came from the MCP package while it serialized a Unity `TransformHandle`; they were cleared, and the final Console error/warning query returned zero entries.
- The placeholder is not connected to gameplay yet. There is no `SetActive()` call or telegraph-control component. The next lesson is only the visibility boundary: show when an attack successfully enters `Startup`, then hide when it successfully enters `HitWindow`.

### Basic Attack Candidate Acquisition Checkpoint

- Added Layer `HitTarget` at Layer index `6` and assigned it through the reusable player Prefab's `PlayerCombat.hitTargetLayers` mask (`m_Bits: 64`).
- `PlayerCombat.FindBasicAttackCandidates()` uses `Physics.OverlapSphere(transform.position, basicAttackRange, hitTargetLayers)` with prototype range `2`.
- The overlap query gathers candidates only; it does not define single-target versus multi-target behaviour and does not apply damage.
- `FindNearestBasicAttackTarget()` compares horizontal distance from the player root to each candidate's `Collider.bounds.center` and returns the nearest `Collider`, or `null` when no candidate exists.
- When a grounded Basic Attack is accepted, `PlayerCombat` stores the result in `currentAttackTarget` before calling `PlayerAnimator.PlayAttack()`.
- One-candidate tests verified range inclusion and exclusion. An exploratory 90-degree fan test verified front and side angle paths, but the final ordinary-attack design no longer uses the fan as a hard filter.
- The final two-candidate test found both `NearTarget` and `FarTarget`, then selected `NearTarget` at horizontal distance `1.2` instead of `FarTarget` at `1.6`.
- `SampleScene` currently contains both test cubes on Layer `HitTarget`: `NearTarget` at `(0, 1, 1.2)` and `FarTarget` at `(1.6, 1, 0)`.
- Temporary target, candidate, direction, angle, and fan logs were removed after runtime verification.
- This checkpoint is target acquisition and selection. The later Hit Window checkpoint now reuses this saved target without changing the selection.

### Soft-Targeting and Facing Decision

- The intended ordinary-attack feel is the soft targeting used by games such as Wuthering Waves and Where Winds Meet: search a circular nearby area, select one target, turn toward it, and later confirm it during the Hit Window.
- The prototype policy selects the nearest candidate in the full circular range. This must not be interpreted as circular area damage.
- `PlayerCombat` owns combat-target selection. `PlayerMovement` remains the single owner of how the player transform turns.
- `PlayerMovement.FaceDirection(Vector3 direction)` rejects a zero direction, obtains `Quaternion.LookRotation(direction)`, and uses `Quaternion.Slerp` with `rotationSpeed * Time.deltaTime` to apply one smooth rotation step.
- When an accepted attack has a `currentAttackTarget`, `PlayerCombat` enables `isAttackFacingActive` and requests `FaceDirection()` every frame toward that same saved target.
- `OpenHitWindow()` disables `isAttackFacingActive`; tracking therefore exists only during startup and does not resume after the Hit Window closes.
- When no target is selected, the facing call is skipped while the attack animation still plays normally.
- Runtime verification confirmed smooth startup facing toward the nearest target, stopping at the Hit Window, unchanged saved-target damage behaviour, and an empty final Console.
- The implemented Basic Attack startup lunge moves a limited distance toward the same `currentAttackTarget` saved at attack start. It does not switch targets during startup; a target that escapes both the lunge and the Hit Window range causes a miss.
- Keep attack displacement owned by `PlayerMovement` and applied through the existing `CharacterController` boundary. The final local clip and Root Motion policy were inspected before choosing code-driven motion; do not enable global Root Motion as a shortcut.
- The minimum Hit Window confirmation and damage loop prerequisite was verified before the lunge was implemented.

### Player Action Coordination Decision

- Do not solve action conflicts by spreading `IsAttacking` checks and mutual references across `PlayerMovement`, `PlayerCombat`, and future feature components.
- `PlayerActionController` is the authoritative owner for the current player action.
- Keep the current model limited to `Free` and `BasicAttack`.
- Permissions such as `CanMove` and `CanJump` are derived in the controller and consumed by the responsible component.
- `BasicAttack` means the current grounded basic attack; do not create a global rule that all future attacks require Grounded. A future `AirAttack` should have its own request and state path.
- Repeated or invalid requests are rejected rather than buffered in this first version.
- The Animation Event is the explicit attack-end signal that returns the state to `Free`.
- Do not claim that Attack has fixed priority over Jump. Add centralized, explicit priority arbitration later when more mutually exclusive actions such as Dodge make it necessary.
- Combo branches, Dodge cancellation, Skills, zero-health/death behaviour, and hit reactions remain deferred.

### Animator Controller Findings

- The player uses `Assets/StarterAssets/ThirdPersonController/Character/Animations/StarterAssetsThirdPerson.controller` as a reference controller.
- Relevant parameters are `Speed`, `MotionSpeed`, `Grounded`, `Jump`, and `FreeFall`.
- `Speed` controls the Idle/Walk/Run Blend Tree, whose inspected thresholds are `0`, `2`, and `6`.
- The locomotion state's playback-speed parameter is `MotionSpeed`; its default value is `0`.
- The Starter Assets reference code writes both `Speed` and `MotionSpeed`.
- During the initial experiment, synchronizing only `Speed` caused a frozen movement pose and sliding because `MotionSpeed` remained `0`. Synchronizing both parameters resolved the issue.

## Current Learning Position

The learner understands the basic roles of `Start`, `Update`, `Vector2`, `Vector3`, `Time.deltaTime`, fields, properties, and `GetComponent`.

They are not yet expected to write a complete controller independently. They have demonstrated understanding of the movement data flow, accumulated gravity, Grounded reset, the three jump-velocity phases, and one-use Jump request behaviour. For camera-relative movement, they predicted basic directions, distinguished copied direction data from camera rotation, independently transferred the flatten-and-normalize pattern to `cameraRight`, corrected camera/world-axis confusion, and independently wrote the final direction combination. Treat camera-relative composition as Practising until a later unprompted reconstruction.

The learner completed the five-parameter locomotion and Jump animation integration with guided steps: `Speed`, `MotionSpeed`, `Grounded`, `Jump`, and `FreeFall`. They independently explained request versus maintained state, derived versus duplicated state, float versus bool Animator parameters, and the full input-to-animation component flow during the Week 1 review. Detailed topic stages remain in `Docs/LEARNING_PROGRESS.md`.

For player-action coordination, the learner correctly predicted the enum field's `Free` default, distinguished `PlayerCombat` execution from `PlayerActionController` arbitration, used and explained short-circuit `&&`, corrected a property type mismatch, recognized why derived permissions should not be duplicate fields, passed Grounded context through a method parameter, and predicted and verified the accepted/rejected request paths. They also distinguished guaranteed mutual exclusion from an unconfigured same-frame priority. Keep the full coordination topic at **Practising** until a later unprompted reconstruction or extension.

For the Basic Attack Hit Window, the learner predicted that making the whole action active would allow damage during attack startup, distinguished the reusable window mechanism from AnimationClip-specific timing, selected a prototype Frame 18-30 interval, completed the bool/property/open/close methods with a scaffold, configured the two Animation Events, and verified their runtime order before removing temporary logs. They also distinguished code-timed coroutine waits from pose-timed Animation Events and correctly identified a delayed UI message as a coroutine-suitable example. Keep the topic at **Practising** until a later reconstruction or extension.

For Basic Attack candidate acquisition, the learner distinguished Layer from Tag, predicted radius-based inclusion, reasoned through horizontal direction and angle tests, and identified that iterating candidates does not itself create area damage. They completed the nearest-target method with a scaffold but needed the final comparison block after becoming blocked. They chose a full circular soft-target policy, verified that two candidates select the nearer target, and chose `Vector3 direction` as the lower-coupling input for movement-owned facing. Keep candidate acquisition and nearest selection at **Practising**.

For Basic Attack automatic facing, the learner first completed the instant `FaceDirection()` boundary, then correctly identified that one `Slerp()` call affects only one frame. They assigned repeated-request lifetime to `PlayerCombat`, kept transform rotation in `PlayerMovement`, identified why the Hit Window bool cannot distinguish startup from recovery, and implemented `isAttackFacingActive` with guided placement. They also distinguished C# syntax from Unity's `Quaternion`/`Slerp` API and related the local bool to a future attack-phase enum without prematurely implementing combos. Runtime verification confirmed smooth startup tracking that stops at the Hit Window. Keep this topic at **Practising** until a later unprompted reconstruction.

For Hit Window target confirmation, the learner correctly chose `OpenHitWindow()` rather than per-frame `Update()`, predicted that a different Collider should not count as the saved target, and completed the `null` guard, fresh candidate query, `foreach`, and same-Collider comparison with scaffolding. They considered the conditional `?:` operator but kept the clearer `if`/`else` assignment while it is new. Keep this topic at **Practising**.

For minimal enemy health and confirmed damage, the learner distinguished `[SerializeField]` from an independent data asset, completed `currentHealth` and `TakeDamage(int damageAmount)` from naming scaffolds, and saw that each component instance owns independent serialized data. They correctly chose to call the enemy-owned method instead of directly changing its private field, then connected `GetComponent<EnemyHealth>()`, a `null` guard, and one fixed damage call with guidance. Keep this topic at **Practising**.

For enemy attack phases, the learner created the four-value enum, guarded all four manual transition boundaries, added a persistent non-`Ready` timer, and then wrote configurable timed conditions for the complete `Startup -> HitWindow -> Recovery -> Ready` loop. They reused one timer because each accepted transition resets it, used `&&` for phase-plus-duration checks, and used `else if` to prevent multiple automatic transitions in one update. Keep this topic at **Practising** until later reconstruction or extension with less support.

For enemy attack target lifetime, the learner distinguished runtime state from Inspector configuration, passed a non-null `PlayerHealth` parameter into `TryStartAttack`, saved it only after the request was accepted, consumed the same reference once when the timed Hit Window opened, and cleared it when recovery completed. The flow is now connected to a real range-gated caller; keep this topic at **Practising** until it is reconstructed or extended with less support.

For enemy attack distance gating, the learner identified the enemy and player as the two Transform owners, created a per-frame local distance value, chose `<=` for the in-range comparison, and connected only the true branch to the existing guarded attack request. Opposite range tests verified both branches. Keep this topic at **Practising** until it is reconstructed or extended with less support.

Detailed learning status is maintained in `Docs/LEARNING_PROGRESS.md`.

### Goblin Visual and Attack Animation Integration

- The full Heroic Fantasy package was inspected in the isolated `C:\Unity\Project\RelicGuardianAssetLab` project. The ignored local Goblin subset now contains one model, one Prefab, two materials, eight textures, Idle, non-Root-Motion sword-and-shield Attack, Walk, and Run clips.
- The original all-animation Controller was not imported. The learner-created local Controller `Assets/LocalLicensed/HeroicFantasyCreatures/Goblin/GoblinEnemy.controller` has looping `IdleSwordShield`, non-looping `Attack1SwordShield`, looping `RunSwordShield`, an `Attack` Trigger, and a Float `Speed` parameter.
- The learner configured `Idle -> Attack` for immediate Trigger response with a fixed `0.05`-second blend, and `Attack -> Idle` for Exit Time `0.9` with the same blend.
- `Idle -> Run` uses `Speed > 0.1`; `Run -> Idle` uses `Speed < 0.1`. Both have no Exit Time and use fixed `0.1`-second blends. A manual runtime `Speed 0 -> 1 -> 0` test verified both directions.
- The learner added the serialized `Animator` reference and `animator.SetTrigger("Attack")` to the successful `TryStartAttack()` branch. The local scene wires that field to the Goblin visual child.
- Runtime observation verified repeated `Idle -> attack plus red Startup telegraph -> Idle` cycles through the real range-gated attack caller.
- Walk was visually accepted for future patrol and Run for chase. Both reuse `SK_GoblinAvatar`, loop, and were verified in Preview to remain in place.
- Keep the gameplay root separate from the visual child. Root Motion remains disabled; chase displacement belongs to the new tracked `EnemyMovement` owner rather than the animation.

### Verified Enemy Chase Animation and Attack Movement Constraint

- The local `NearTarget` root has a `CharacterController` with `Height = 1`, `Radius = 0.5`, and `Center = (0, 0, 0)`. Its previous Box Collider remains present for now, and `EnemyAI` is enabled.
- Tracked `EnemyMovement.cs` has `[RequireComponent(typeof(CharacterController))]`, serialized `moveSpeed = 3f` and `rotationSpeed = 10f`, plus a cached `CharacterController`.
- `Move()` clears Y, rejects zero direction, calculates a target rotation, applies one `Quaternion.Slerp` step to `transform.rotation`, normalizes the direction, and moves through `CharacterController` with a frame-rate-independent displacement.
- `EnemyMovement` is attached to the local `NearTarget` root. `EnemyAI` has an Inspector-assigned reference and requests movement every out-of-range frame using the direction from the enemy to the player.
- `CurrentHorizontalSpeed` exposes the CharacterController's actual XZ velocity magnitude. `Stop()` supplies a zero movement step so the reported speed is refreshed when the AI intentionally stops.
- Tracked `EnemyAnimator.cs` is attached to the local root, receives Inspector-wired references to the Goblin child Animator and root `EnemyMovement`, and writes `CurrentHorizontalSpeed` into the Animator's `Speed` Float each frame.
- `EnemyAI` stops before requesting an in-range attack. While `EnemyAttack.CurrentPhase` is not `Ready`, it stops and returns before any distance-based chase decision, so moving the player during Startup, Hit Window, or Recovery cannot drag the attacking Goblin forward.
- Play Mode verified smooth facing, actual-speed Run playback, stopping at the existing `2m` attack range, transition into Attack, and no chase resumption during the active attack. The final Console was clean.
- Preserve the boundary: `EnemyAI` owns attack-versus-chase decisions, `EnemyMovement` owns rotation and CharacterController displacement, `EnemyAttack` owns attack phases, and `EnemyAnimator` consumes actual movement state for the visual child.

### Local Katana Visual and Final Light-Attack Asset Preparation

- The reusable right-hand attachment boundary is unchanged. The currently equipped local visual is `Frozen_Katana_Blue`; the learner visually adjusted and approved its grip, and the former `P09Sword02Visual` is inactive.
- The final planned light-attack presentation order is:
  1. `Attack_4Combo_1_Inplace`
  2. `Attack_4Combo_2_Inplace`
  3. `Attack_4Combo_3_Inplace`
  4. `Attack_3Combo_3_Inplace`
- The four FBXs and their Unity import metadata are present under ignored `Assets/LocalLicensed/PowerfulSwordPack/Katana/LightCombo/`.
- Live Unity inspection confirmed four non-looping Humanoid AnimationClips. Their importer uses the package Avatar at `Assets/LocalLicensed/PowerfulSwordPack/Avatar/Modeling_T-Pose_Grrrru_Man(recommend).FBX`, and the final related Console query contained no errors.
- Asset integration and runtime acceptance now reach all four selected clips. Attack3/4 use tracked placeholders and ignored local Override mappings.

### Four-Step Combo Extension Checkpoint

- The project-owned Controller has Int `AttackIndex`, tracked empty `Attack2Placeholder`, `BasicAttack -> Attack2` using `Attack` plus `AttackIndex == 1`, and an unconditional Attack2 return at Exit Time `0.9`. `PlayerAnimator.PlayAttack(int)` writes the index before setting the Trigger.
- The ignored local Override mappings are `SwordAndShieldSlash -> Attack_4Combo_1_Inplace`, `Attack2Placeholder -> Attack_4Combo_2_Inplace`, and `Idle -> Idle_ver_B`. Root Motion remains off.
- Attack1 events are approximately Frames `9.70` `OpenHitWindow`, `11.56` `OpenComboWindow`, `12.22` `CloseHitWindow`, `12.43` `ComboTransitionPoint`, `21.66` `CloseComboWindow`, and `34.17` `FinishAttack`. Attack2 events are Frames `8` `OpenHitWindow`, `12` `CloseHitWindow`, and `35.03` `FinishAttack`.
- The Prefab contains two inline `PlayerAttackData` entries, each currently configured as damage `1`, target range `2`, lunge speed `5`, and lunge distance `1`.
- `PlayerCombat` owns queueing, transition-ready state, `HasNextAttack`, and reusable `TryStartQueuedAttack()`. Early valid input waits until the transition event; later valid input inside the still-open window advances immediately. Bounds checking prevents index `2` with two entries.
- Both clips finish through `PlayerCombat.FinishAttack()`, which clears all attack runtime state before returning the coarse action to `Free`.
- The learner runtime-verified the no-follow-up path, one valid Attack2, window-external input rejection, repeated-input bounds, two independent damage results, Attack2 retaining `BasicAttack` until its own finish, movement/Jump recovery, and final clean state. Final static validation and Console error checks were clean.
- Every attack Event now carries an authored step index and is rejected when it does not match `currentAttackIndex`. Attack1-3 use atomic `EnterRestartWindow(int)` boundaries and direct Animator paths back to Attack1; runtime checks confirmed early rejection and responsive late restart.
- The Prefab has four prototype data entries. Attack3/4 states, tracked placeholders, ignored local Override mappings, indexed transitions, and Events are configured without copied combat-flow methods. Attack4 uses index `3`, not `4`.
- Attack4 Root Transform Position (Y) now matches the first three clips' Feet basis. Its Finish Event is approximately Frame `71`, and its return transition uses Exit Time `1`.
- Do not implement a movement-only recovery-cancel subsystem before Dodge or Block exists. Future interruption work should separate priority from cancel permission and centralize attack cancellation cleanup.
- Local licensed `Idle_ver_B` is a looping Humanoid Idle mapped through `Idle -> Idle_ver_B`. It uses the existing source Avatar, keeps Root Motion off, and matches Attack1's Original Root Transform Rotation basis. The learner approved the final attack-to-idle blend with no extra turn.

## Known Issue and Next Step

- The minimal player-action coordination, Basic Attack Hit Window, circular candidate acquisition, nearest single-target selection, multi-frame startup target-facing, saved-target confirmation, enemy-health receiver, and one confirmed damage checkpoints are complete and runtime-verified.
- The occasional MCP warning `WebSocket is not initialised` is produced by the MCP package and is not a game-code warning. The latest compile completed successfully.
- Same-frame Attack/Jump winner priority remains intentionally undefined because both scripts have execution order `0`; mutual exclusion is verified. Do not add scattered input checks to force a priority.
- Minimal enemy health, confirmed player-to-enemy damage, the zero-health boundary, and the inactive-object prototype death response are complete and runtime-verified.
- The P09 female visual-child replacement, ordinary locomotion verification, and first right-hand weapon visual boundary are complete. Do not consume Root Motion or migrate damage/equipment logic onto the weapon model.
- Multi-frame attack-facing smoothing is complete. Preserve `PlayerCombat` as the facing-lifetime owner and `PlayerMovement` as the transform-rotation owner when later attacks are added.
- The player-owned health receiver, target-carrying timed enemy attack, range gate, visible Startup telegraph, minimal Goblin visual, Trigger-driven attack, bounded player lunge, and manual Idle/Run animation switching are verified.
- Actual-speed Goblin locomotion synchronization, attack-phase movement blocking, and the minimum combat loop through enemy deactivation are complete. Hitstop remains deferred while the approved combo-refactor path is resumed.
- The complete Attack1 -> Attack2 -> Attack3 -> Attack4 path is runtime-verified. Acceptance covered each non-follow-up ending, four independent damage results, Attack1-3 Combo and Restart behavior, invalid timing rejection, Attack4 movement/Jump recovery, final state cleanup, and a clean Console.
- Resume by choosing the first real Dodge or Block action and designing its action priority and attack-cancel permission one concept at a time. Keep general recovery cancellation deferred until that concrete consumer exists.
- Keep `Docs/COMBO_ATTACK_ARCHITECTURE.md` in every code-focused GitHub sync and distinguish the verified two-hit baseline, configured four-hit extension, and deferred recovery-cancel design.
- Unity `6000.3.19f1` repeatedly logged `UnityEditor.Graphs` null references while the Animator graph remained open across Controller edits and script reloads. The stacks contain only UnityEditor graph code, and the real gameplay loop still passed. Close or switch away from the Animator graph before judging a fresh Console regression.
- Unity has `Application.runInBackground = false`; MCP sampling while Unity is unfocused can leave `Time.frameCount` unchanged even while real time advances. Verify frame progression before treating a timed MCP test as runtime evidence.
- Two missing-script messages appeared during an external scene-file refresh, but a live scan found no missing component and the messages did not reproduce after Reload and the final attack regression. Treat this as a historical transient observation unless it returns.
- Before a future `AirAttack`, add a separate airborne action path rather than weakening the grounded `BasicAttack` rule or adding a global “all attacks require Grounded” rule.
- For future Animator Controller integrations, follow the Animator Controller Integration Checklist in `DEVELOPMENT_RULES.md` and inspect all consumers/reference writers first.
- Version pitfall recorded: verify the actual Unity version, package version, and visible editor UI before giving interface instructions. Do not make the learner search repeatedly for controls from an older tutorial.

## Cross-Workspace Boundary

- `C:\Unity\Learning\CSharpPractice` is a separate workspace. From this Relic Guardian task, inspect it read-only when cross-track status is needed.
- Do not modify, create, delete, format, stage, or commit C# Practice files unless the learner explicitly switches to or authorizes work in that workspace.
- Prefer continuing C# lessons in the Codex task rooted at `C:\Unity\Learning\CSharpPractice`.
- `Docs/LEARNING_TRACKER.md` may be updated here from verified C# progress, but the C# workspace's own detailed progress file is authoritative.

## External Asset Packages and Isolated Validation

- External holding folder: `C:\unasstes`. This folder is outside the Unity project and is not tracked by this repository.
- `P09ModularHumanoidLite v2026.01.05.unitypackage` (`430,814,378` bytes).
- `Powerful Sword PackGreat Sword Katana 2.1.3.unitypackage` (`210,736,988` bytes).
- `Sword slashes PRO 3.0.unitypackage` (`34,519,116` bytes).
- `HEROIC FANTASY CREATURES FULL PACK VOL 1 v2.51.unitypackage` (`3,030,509,498` bytes) was imported into the isolated AssetLab only; the full package remains outside the main project.
- P09 and the narrowed katana-animation subset are imported into the **local main-project workspace** under ignored `Assets/LocalLicensed/`; both remain licensed external dependencies. The full Powerful Sword package and `Sword slashes PRO` remain unimported.
- `P09ModularHumanoidLite` requires lilToon for visible materials and contains an optional MagicaCloth 2 setup package, nested installers, and demo scripts/content. Test dependencies in isolation and do not blindly import the nested packages.
- `Powerful Sword Pack` is mostly Humanoid FBX animation content, has no C# scripts, and includes both movement and in-place attack variants. The full package remains in the isolated AssetLab; only the four approved in-place light-attack clips are copied into the ignored main-project folder.
- `Sword slashes PRO` contains Built-in/URP/HDRP shader variants, demo scripts, an auto-running editor shader changer, and a complete `Packages/manifest.json`. Exclude the manifest and editor/demo scripts. Its manifest requests Shader Graph `17.0.4`, while the current project uses URP `17.3.0` and must retain its own package set.
- The Heroic Fantasy archive contains `30` creatures and `1,802` assets. The selected ordinary enemy is the sword-and-shield Goblin. Its Generic Rig, own Avatar, two URP/Lit materials, Prefab references, and 82 separate animation FBXs were inspected in AssetLab.
- The main project imported only `SK_Goblin.FBX`, `Goblin.prefab`, two materials, eight required PBR textures, `IdleSwordShield`, `Attack1SwordShield`, `WalkNormalSwordShield`, and `RunSwordShield` under ignored `Assets/LocalLicensed/HeroicFantasyCreatures/Goblin/`. The original `Goblin_Controller.controller`, `_RM` locomotion variants, Demo scene, other creatures, and unrelated variants remain excluded. Add later clips one at a time only when their gameplay owner is ready.
- Isolated validation used P09's male Humanoid prefab and `M_katana_Blade@Attack_3Combo_1_Move.FBX`. A Humanoid test copy used the supplied source Avatar, `motionNodeName = "root"`, horizontal root motion not baked, and vertical/rotation motion baked.
- The animation source model moved about `1.011` metres along local Z in the audit scene. The P09 male also played the retargeted attack and visibly moved from the green start marker toward the orange finish marker; the learner confirmed the result.
- P09's materials rendered correctly after replacing incompatible lilToon `1.10.3` with official pinned `2.3.3`. The earlier shader error was `redefinition of 'LIGHTMAP_ON'` in `Hidden/ltspass_opaque` under Unity `6000.3.19f1` and URP `17.3.0`.
- The P09 hierarchy originally contained `Weapon/Sword` with a `ParentConstraint`. Its verified sources are `Weapon_Target_Hand_R` and `WeaponTarget_Back`; hand weight `1` and back weight `0` place the existing sword at the right hand.
- In the main project this boundary is explicitly named `RightHandWeaponSocket`. `P09Sword02Visual` was its first replaceable child and is now inactive; the approved equipped visual is `Frozen_Katana_Blue`. Keep the reusable socket stable and change only the weapon-specific child alignment.
- The minimum confirmed-damage loop, local katana Basic Attack override, and code-driven bounded Basic Attack startup lunge are verified. The lunge follows only the target saved at attack start, stops at the Hit Window or travel limit, never retargets, and leaves Hit Window-time confirmation authoritative. Do not enable global Root Motion as a shortcut.
- Future combo work should use a separate combo-input/transition window: no follow-up input plays the full first-attack recovery, while a valid queued input transitions into the matching second clip before that recovery finishes. Do not reuse the damage Hit Window as the combo-input window.
- The local Dark Fantasy Katana conversion is also licensed-only. The equipped `Frozen_Katana_Blue` Prefab and its dependencies live under ignored `Assets/LocalLicensed/DarkFantasyKatana/`; do not stage or publish them in the code-focused repository.
- Temporary audit artifacts still exist and were intentionally not deleted during this handoff: `C:\Users\c8618\AppData\Local\Temp\relic_guardian_katana_audit_20260805` and the recoverable old-package backup `C:\Users\c8618\AppData\Local\Temp\RelicGuardianAssetLab_lilToon_1.10.3_backup_20260805`.

## Verified Environment Note

- On this machine, Unity Editor running with DX12 stuttered during Play Mode even though player scripts were inexpensive in the Profiler.
- Profiler markers pointed to DX12 render/presentation waits (`RenderLoop`, `GfxDeviceD3D12.WaitForLastPresent`, and `WaitForGPU`).
- Launching Unity through Unity Hub with `-force-d3d11` removed the stutter in the same test.
- Keep the DX11 launch argument while developing this project unless a later driver or Unity update resolves the DX12 behaviour.

## Git

- Current HEAD at this handoff: `2db605129d8061e63a550a56dc4b51e48477f1f8` on `main`.
- The current working tree is intentionally not clean. Tracked modifications include the player Animator Controller, local player Prefab, `PlayerAnimator.cs`, `PlayerCombat.cs`, `SampleScene.unity`, and the six active project documents. `PlayerAttackData.cs` plus its `.meta` are currently untracked.
- Do not overwrite, restore, reset, or discard these changes. Inspect the live diff before any future edit.
- `Assets/LocalLicensed/` remains ignored. `Idle_ver_B`, the four katana clips, P09/Goblin resources, and local `KatanaAnimationOverrides` changes must not be committed or uploaded.
- Unity MCP must be explicitly returned to `My project@f22d513a32eb5447` for gameplay work. `RelicGuardianAssetLab@d0fae1ba933aab0e` is inspection-only and must not remain the active target when changing the main project.

- Branch: `main`
- Initial commit: `3fc7c9a Initial Unity project setup`
- Starter Assets/package commit: `01e78d2 Import Starter Assets and configure Unity packages`
- Tested movement commit: `74105b6 Add custom player input and grounded movement`
- Jump input configuration checkpoint commit: `862d37c Configure jump input and archive learning checkpoint`
- Verified grounded Jump commit: `2213a57 Add verified grounded jump`
- Verified Camera-relative movement commit: `67e6594 Add camera-relative movement`
- Component enabled-state pitfall commit: `e423ea0 Record component enabled-state pitfall`
- Clean-code learning rules commit: `55f3602 Add clean code learning rules`
- Animator integration checklist commit: `9f31b85 Add Animator integration checks`
- Verified locomotion animation synchronization commit: `7dfd22d Add verified locomotion animation sync`
- Verified Animator Grounded synchronization commit: `71a4118 Sync verified grounded animation state`
- Verified Animator Jump synchronization commit: `6d24f02 Sync verified jump animation state`
- Verified controller animation-flow commit: `df98fb2 Complete verified controller animation flow`
- Week 1 review commit: `4e24b02 Record Week 1 learning review`
- Player-action state-owner checkpoint: `e979253 Add player action state owner checkpoint`
- Verified player-action coordination checkpoint: `ace27a1 Complete verified player action coordination`
- Verified Basic Attack Hit Window checkpoint: `9e37fc6 Add verified Basic Attack hit window`
- Verified Basic Attack target selection checkpoint: `f3b65c4 Add verified Basic Attack target selection`
- Verified Basic Attack instant target-facing checkpoint: `a8cbb72 Add verified Basic Attack instant target facing`
- Verified Basic Attack target-confirmation checkpoint: `e02de79 Add verified Basic Attack target confirmation`
- Verified Basic Attack damage checkpoint: `087ec72 Add verified Basic Attack damage`.
- Verified main-project lilToon asset-integration checkpoint: `1d55410 Add verified asset integration checkpoint`.
- Local P09 integration documentation checkpoint: `514dffc Document local P09 visual integration`.
- Licensed/local-only asset Git rules checkpoint: `5253d27 Add local licensed asset commit rules`.
- Verified smooth Basic Attack target-facing checkpoint: `cac1311 Add smooth basic attack target facing`.
- Verified right-hand weapon visual documentation checkpoint: `2dc0966 Document verified right-hand weapon visual boundary`.
- Local licensed animation ignore-rule checkpoint: `fb359fc Ignore local licensed animation assets`.
- Verified Goblin attack animation commit: `5e24756 Add enemy Animator attack trigger hook`.
- Current archive checkpoint subject: `Archive player lunge and enemy chase foundation`. Use `git log -1 --oneline` for its final hash after the focused commit.
- Intentional uncommitted changes after this archive commit: `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab` contains the local-only P09 visual integration, weapon boundary, ignored animation override reference, and local health attachment; `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller` contains local presentation tuning; `Assets/Scenes/SampleScene.unity` contains local Goblin/telegraph/Animator wiring, the enabled `EnemyAI`, the new root `CharacterController`, prior player-prefab references, and mixed Unity serialization changes. The ignored `Assets/LocalLicensed/HeroicFantasyCreatures/Goblin/` contains the narrowed licensed source assets, Walk/Run clips, and learner-created Controller. Do not stage these mixed or licensed local files; do not describe the working tree as clean.

## 2026-08-24 Lock-On Movement and Camera Handoff (Latest Current State)

This is the latest checkpoint and supersedes the earlier `Known Issue and Next Step` section when choosing what to do next.

### Implemented and Verified Gameplay

- `PlayerTargeting` now owns the authoritative lock-on target. It searches the `HitTarget` Layer within `10m`, selects the nearest Collider, exposes `CurrentTarget` plus the derived `IsLockedOn`, toggles with the `V` key, clears inactive targets, and automatically breaks beyond `12m`.
- `PlayerInputReader` stores the `LockOn` Button request across frames and exposes `ConsumeLockOn()`. The action is currently bound to `<Keyboard>/v`; this binding may be changed later in the Input Actions asset without redesigning the targeting system.
- `PlayerMovement` keeps the existing camera-relative free movement. While locked and allowed to move, it faces `PlayerTargeting.CurrentTarget` every frame and retains CharacterController-driven displacement with Root Motion disabled. Locked W/S/A/D movement, stationary facing, unlock recovery, attack rotation ownership, and combo recovery were runtime-checked.
- `PlayerCombat` gives the authoritative locked target priority over the ordinary nearest soft target. If that locked target is outside the current attack step's range, the attack deliberately has no target and does not fall back to a different nearby Collider. Locked priority, the no-fallback rule, unlocked nearest soft targeting, and multi-step attack use were runtime-checked.

### First Usable Lock-On Camera

- Cinemachine version is `3.1.7`. `FreeLook Camera` remains the unrestricted mouse-orbit camera. A separate `LockOn Camera` uses `Lock To Target With World Up` and tracks `PlayerCameraRoot`.
- `PlayerCameraController` reads `PlayerTargeting.IsLockedOn` and switches the two Cinemachine cameras through priorities `10` and `0`, preserving Cinemachine Brain blending instead of directly enabling/disabling cameras.
- `LockOnCameraTarget` is a player-Prefab child and is assigned as the lock camera's separate `Look At Target`; `Tracking Target` remains `PlayerCameraRoot`. Each locked frame, the controller calculates player chest position, enemy Collider center, and `Vector3.Lerp(..., enemyLookWeight)` with default weight `0.35` before moving this target.
- The learner accepted the current camera direction and a provisional composition. Saved lock-camera tuning includes Center Orbit Radius `3.72`, Height `0.77`, Rotation Damping Y `0`, horizontal input Gain `0.3` with range `-25..25`, vertical input Gain `-0.1` with Center `-5` and range `-10..5`, and recenter Wait `0.4` / Time `0.6` on both axes. Lock-camera zoom input is disabled.
- Locking no longer assigns the enemy Collider directly as Cinemachine LookAt. The camera remains player-relative while the weighted target lets the view respond to enemy position. Treat the current values as accepted prototype tuning, not final production camera polish.

### Deferred Camera and Lock-On Work

- Multi-target left/right switching, a lock indicator UI, camera collision/occlusion, extreme target-height/distance framing, and production camera polish are not implemented.
- The lock camera's Input Axis Controller remains active while both virtual cameras are active. If a later test shows inconsistent initial offset when re-locking, add state-aware input gating or an explicit axis reset then; do not pre-emptively complicate it now.
- The final weighted-target camera change was visually accepted provisionally, but no separate clean-Console or fast-moving-enemy regression was recorded after that last tuning step. Re-test only if later work exposes a problem.

### Next Development Step

- The camera phase is complete for the first usable lock-on version. Resume with locked directional locomotion presentation: inspect and selectively import only the needed in-place Katana directional movement clips under ignored `Assets/LocalLicensed/`, then map forward/back/left/right movement without enabling Root Motion.
- Teach one concept at a time. Before asking the learner to declare a variable, explain the needed data, field versus local scope, lifetime, type, and the English name word by word.
- Do not recreate `PlayerActionController`; it already owns `Free`, `BasicAttack`, `CanMove`, and `CanJump`. Do not split `PlayerMotor` merely for architecture. Reconsider the movement boundary when Dodge, knockback, or another concrete displacement source requires it.
- Dodge, Block, and formal action interruption priority remain deferred until after the current lock-on movement presentation step.

### Git and Asset Safety at This Handoff

- This checkpoint is archived by the focused commit titled `Add four-step combo and lock-on foundation`; use `git log -1 --oneline` for its final hash.
- After that focused commit, the working tree intentionally remains dirty only where local licensed presentation and scene wiring are mixed: `RelicGuardianPlayer.prefab` and `SampleScene.unity` stay unstaged. The tracked scripts, Input Actions, project-owned Animator/placeholder assets, and documents are included in the archive commit.
- Preserve all existing uncommitted work. Do not reset, restore, or overwrite it. `Assets/LocalLicensed/` remains local-only and must never be committed or uploaded.

## 2026-08-24 Movement, Sprint, and Dodge Design (Latest Planned Work)

This section records an agreed design direction, not implemented or runtime-verified gameplay. Continue from the locked directional locomotion step below; do not report Sprint or Dodge as complete.

### Intended Player Experience

- Free movement remains camera-relative and CharacterController-driven. The player faces the movement direction and uses a forward jogging presentation as the normal action-game travel pace.
- Locked movement keeps the player facing `PlayerTargeting.CurrentTarget` while W/S/A/D move toward, away from, or around the target. Its final presentation should use an in-place eight-direction locomotion Blend Tree so forward, backward, strafing, and diagonal movement match the actual direction.
- Sprint is initially a held locomotion mode inside `PlayerActionState.Free`, not a separate action state. Use a provisional `Left Shift` binding that can be changed later through the Input Actions asset.
- The first Sprint version is free-movement-only. While locked, retain the controlled eight-direction jog instead of allowing unrestricted full-speed running.
- Dodge will later become a real `PlayerActionState.Dodge`. It should snapshot the accepted input direction at Dodge start, block ordinary movement/turning during the action, and return explicitly to `Free` when its animation and displacement finish.
- In the first locked Dodge version, forward/back/left/right input selects the matching direction and no input defaults to a backward Dodge. In free movement, directional input turns the character into the requested world direction and uses a forward Dodge; no input uses the character's current forward direction.
- Keep Apply Root Motion disabled. Normal movement, Sprint, attack lunge, and future Dodge displacement remain code-driven through the CharacterController-owned movement boundary.

### Verified Available Katana Animation Coverage

- `Powerful Sword PackGreat Sword Katana 2.1.3.unitypackage` contains complete Katana `Walk_ver_A` and `Walk_ver_B` directional families, including forward, backward, left/right 90-degree, and diagonal variants, with both ordinary and `_Root` files.
- It contains complete `Jogging_8Way_verA_*` and `Jogging_8Way_verB_*` families for F, B, L90, R90, FL45, FR45, BL45, and BR45, again with ordinary and Root variants. Prefer testing the non-Root `verB` family first because the current equipped locomotion presentation uses the B pose family.
- Forward running options include `M_katana_Blade@Run_ver_B.FBX` and `M_katana_Blade@Run_Fast_ver_B.FBX`, with matching Root variants available but intentionally unused.
- Dodge options include `M_katana_Blade@Dodge_Front.FBX`, `Dodge_Back`, `Dodge_Left`, and `Dodge_Right`. `Run_Fast_Dodge_Left` and `Run_Fast_Dodge_Right` are available for later Sprint-Dodge polish, not the minimum first version.
- The Dodge files do not advertise Root/non-Root status in their filenames. Preview their imported motion and Humanoid retargeting on the P09 female before relying on them; continue to drive gameplay displacement in code regardless.
- The package also contains idle-to-move, move-to-idle, and turn-in-place clips. Defer these polish transitions until the basic locomotion and Dodge behavior are stable.
- `P09ModularHumanoidLite` supplies the current character presentation, `Sword slashes PRO` is relevant to later attack VFX rather than locomotion, and the Heroic Fantasy package remains enemy-focused.

### Minimum Low-Risk Development Order

1. Implement only locked directional locomotion presentation. Start with Idle plus non-Root Katana `verB` F, B, L90, and R90 clips in a two-dimensional Blend Tree; after the four cardinal directions are verified, add the four diagonals as one repetitive configuration batch.
2. Verify free movement presentation separately: camera-relative displacement, facing the movement direction, and a forward jogging animation. Do not add slow walking unless a concrete keyboard toggle or analog-input requirement appears.
3. Add held Sprint input and a separate Sprint speed for free movement, then connect the selected `Run_ver_B` or `Run_Fast_ver_B` animation. Keep locked Sprint disabled in the minimum version.
4. Add the `Dodge` action state and accept Dodge only from `Free`. Snapshot its direction, choose a four-direction animation, apply bounded code-driven displacement, and explicitly finish back to `Free`.
5. After movement and recovery are runtime-verified, add invulnerability timing as its own concept. Design attack-to-Dodge cancellation windows and action priority afterward; do not make every attack recovery cancellable implicitly.
6. Do not extract `PlayerMotor` before the directional locomotion or Sprint work. Reconsider a small displacement-application boundary while integrating Dodge, when ordinary movement, attack lunge, Dodge, and future knockback provide concrete evidence for the split. Avoid a broad architecture-only refactor.

### Learning and Verification Notes

- The next lesson is only the relationship between world movement and player-local directional values used by a two-dimensional Blend Tree.
- Before asking the learner to create parameters or variables such as horizontal/forward movement values, explain the data purpose, field versus local scope, lifetime, type, and every English word in the proposed name.
- The learner writes the key gameplay code unless they explicitly request Codex to take over. A response such as “continue”, “嗯”, or “好了” is not implementation authorization.
- Preview the chosen `verA`/`verB` clips on the actual P09 female before final selection. Check Humanoid Avatar compatibility, feet, weapon-hand pose, loop continuity, and unwanted source translation.
- Test and document each stage independently. Do not combine locked locomotion, Sprint, Dodge, invulnerability, and attack cancellation into one implementation checkpoint.

## 2026-08-24 Locked Locomotion, Sprint, and Camera Polish Archive (Latest Current State)

This is the newest handoff. It supersedes the earlier locked-locomotion `Next Development Step` and the unimplemented Sprint statements in `Movement, Sprint, and Dodge Design`. Dodge design remains planned rather than implemented.

### Completed Ground-Locomotion Milestone

- `Locked Locomotion` is a 2D Simple Directional Blend Tree driven by project-owned Float parameters `MoveX` and `MoveZ`. The player converts its camera-relative world movement direction into player-local direction with `transform.InverseTransformDirection()` and exposes the result through `CurrentLocalMoveDirection`.
- The tracked Controller uses eight project-owned empty `LockedMove*Placeholder` Clips. The ignored local Override maps them to non-Root Katana `Jogging_8Way_verB` forward, backward, left/right, and four diagonal clips. Root Motion remains disabled.
- `PlayerAnimator` damps locked X/Z values and free `Speed` with separate serialized `0.1s` values. The learner accepted cardinal/diagonal switching, direction changes, the final Idle handoff, and free normal travel presentation.
- Free locomotion retains the existing 1D Idle/Walk/Run tree with thresholds `0`, `3`, and `6`. The ignored local Override maps the Walk slot to the forward Katana jog and the Run slot to non-Root `Run_ver_B`.
- `Sprint` is a held Left Shift Input Action using `Pass Through`. `PlayerInputReader.IsSprintHeld` reflects both press and release. `PlayerMovement` selects `moveSpeed = 3f` or `sprintSpeed = 6f` for both displacement and Animator speed reporting.
- Shift plus nonzero movement while locked calls `PlayerTargeting.CancelLockOn()` and continues as free camera-relative Sprint. Shift alone preserves lock. Releasing Shift returns to normal speed but does not restore the prior lock automatically. The learner runtime-accepted these rules.

### Stabilized Camera Handoff

- `PlayerCameraController` caches both cameras' `CinemachineInputAxisController` components and enables input only on the camera belonging to the current targeting mode.
- FreeLook Camera uses `Inherit Position`; LockOn Camera uses `Freeze When Blending Out`. Together they remove the unlock feedback loop that previously appeared after large character or lock-camera turns.
- The learner accepted the final free-camera sensitivity and unlock blend response. The saved Scene currently serializes Cinemachine Brain Default Blend Time `1s`, FreeLook gains `1.8/-1`, and LockOn gains `0.3/-0.1`.
- Multi-target switching, lock UI, camera collision/occlusion, extreme framing, and production camera polish remain deferred.

### Shared Locked Attack and Jump Boundary

- `Locked Locomotion -> BasicAttack` now mirrors the free attack entrance: `Attack` Trigger, no Exit Time, duration `0.1`. Locked and free modes reuse the same `PlayerActionController`, `PlayerCombat`, target-selection policy, four-step Animator chain, and Animation Events. Do not create `LockedBasicAttack` or a second combat flow.
- The current flat Base Layer still returns every attack state to `Idle Walk Run Blend`, which then routes back to Locked locomotion when `IsLockedOn` remains true. A separate locked-combo/return runtime regression was not recorded after the new entrance. If Block exposes another repeated locomotion entrance or a visible double handoff appears, evaluate a shared action route or full-body Action Layer as a separate architecture lesson.
- The first locked combat rule rejects Jump. The Jump acceptance condition now requires `!playerTargeting.IsLockedOn`, using existing authoritative targeting state without a duplicate field.
- The final script compiled with zero errors, standard validation reported only the pre-existing generic `GetComponent` null-check suggestion, the Console error/warning query was empty, and live Animator inspection confirmed the shared locked Basic Attack entrance.
- The locked-Jump rule has not received its final manual input regression after the Codex takeover edit. At the start of the next session, verify that locked Space does nothing and unlocked grounded Space still jumps. Do not report this single boundary as runtime-verified until that opposite test passes.

### Next Development Step

1. Run the locked attack/return regression plus the two-case Jump regression above and record the results.
2. Design only the minimum Basic Block action: input meaning, accepted action states, movement/turning permission, animation lifetime, and explicit finish boundary.
3. Keep attack-to-Block cancellation permission separate from general action priority. Do not make all attack recovery cancellable implicitly.
4. Dodge remains after Block. Do not create `PlayerMotor` yet; reconsider the CharacterController displacement boundary when Dodge movement is actually connected.

### Git and Asset Safety

- Archive this checkpoint with the focused subject `Complete locked locomotion and free sprint`. Use `git log -1 --oneline` for the resulting hash.
- Commit the project-owned Animator Controller, Input Actions, scripts, tracked placeholder Clips, and updated documentation.
- Keep `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab` and `Assets/Scenes/SampleScene.unity` unstaged because they contain mixed local-only presentation and scene wiring. Do not reset, restore, overwrite, or describe them as clean.
- `Assets/LocalLicensed/` remains ignored and must never be committed or uploaded. The real Katana movement/Run Clips, P09 visual, local Override mappings, and other licensed dependencies stay local-only.

## 2026-08-27 Attacking Migration and Basic Block Entry Boundary (Latest Current State)

This section supersedes the earlier deferred-regression and `PlayerActionState.BasicAttack` current-state statements. Animator state names such as `BasicAttack` remain presentation names and were not migrated.

### Completed and Runtime-Verified

- The deferred opposite checks passed: locked Space does not Jump, unlocked grounded Space still completes Jump and landing, and locked Attack enters the shared four-step chain and returns correctly to locked locomotion.
- The coarse enum is now `Free` / `Attacking`. `PlayerActionController` exposes `TryStartAttack(bool isGrounded)` and `FinishAttack()`. `PlayerCombat` still owns `currentAttackIndex`, all Attack1-4 windows, targeting, lunge, damage requests, and cleanup.
- The migration changed names only. Existing Attack Trigger/Transitions, Animator states, Animation Events, attack data, Restart Windows, and combo behavior were not redesigned.
- Post-migration runtime checks passed for the unlocked full combo, movement/Jump recovery, late Restart Window, and locked full-combo return. The three changed scripts produced zero validation diagnostics and the final Console query contained zero errors and zero warnings.

### Minimum Basic Block Boundary

- Block is held input. The first version may enter only from `Free` while grounded, and a rejected press is not buffered for later automatic entry.
- `PlayerActionController` remains the sole coarse-state owner. A separate `PlayerBlock` will coordinate Block-specific request and lifetime behavior; `PlayerCombat` remains unchanged until a later explicit Attack-to-Block cancellation step.
- Simultaneous Attack and Block requests must have one deterministic result and must not rely on `MonoBehaviour.Update()` execution order. Define the smallest concrete arbitration rule when Block gameplay is connected; do not build a general numeric Priority system.
- The next single implementation concept is only the held Block input representation in `PlayerInputReader` and the Input Actions asset. Do not add `Blocking`, CrossFade, damage reduction, or cancellation in that same step.

### Process Note

- For a semantic C# rename, use IDE Rename Symbol and review the complete reference set as one mechanical change. Do not split one symbol rename into manual line-by-line edits that create avoidable intermediate compiler errors.

## 2026-08-27 Guard / Block Lifecycle Design Handoff (Latest Planned Work)

This is the latest Guard plan and supersedes the smaller `Minimum Basic Block Boundary` above. It records approved design only. No Block gameplay, main-project animation integration, or Block runtime verification was completed in this checkpoint; actual `PlayerActionState` still contains only `Free` and `Attacking`.

### Architecture and Ownership

- `PlayerActionController` remains the only owner of the coarse Gameplay FSM. The intended immediate coarse set is `Free`, `Attacking`, and `Blocking`; later additions may include `Dodging`, `Staggered`, and `Dead`.
- `Blocking` remains one coarse state. Do not add global `GuardStartup`, `GuardHold`, or `GuardRelease` members to `PlayerActionState`.
- A separate `PlayerBlock` will own Guard request coordination and the internal Startup, Hold, and Release lifecycle. Animator presents that lifecycle but never decides Gameplay FSM permission.
- New discrete player actions continue toward code-driven `Animator.CrossFadeInFixedTime()` presentation. Do not migrate or redesign the runtime-verified four-step attack chain as part of Guard.
- Lock-on stays an orthogonal targeting, movement, and camera mode. Guard uses the shared Block action and changes only the behavior that genuinely differs by targeting mode.

### Approved Guard Lifecycle

```text
Free
  -> Guard Press accepted from grounded Free
Blocking
  -> Guard Startup
  -> Guard Hold, if Guard is still held at the Startup decision point
  -> Guard Release, after an authored decision/exit point when released
  -> Free
```

- Guard is held input, not toggle. A press rejected outside grounded `Free` is not buffered for later automatic entry.
- Startup is short. Ordinary translation is disabled, but required target-facing correction is allowed.
- The Perfect Guard Window belongs to Startup and should preferentially use authored Animation Events. This checkpoint defines only the future window placement, not a successful-Guard damage result.
- Releasing Guard during Startup does not immediately hard-cut to `Free`. `PlayerBlock` records the held-state change and enters Release only at the appropriate Startup decision or exit point.
- If Guard remains held when Startup reaches its decision point, the action enters Hold.
- Releasing from Hold enters Release. Release is a short non-moving recovery that adds failed-input cost and prevents free high-frequency Perfect Guard attempts, then returns the coarse state to `Free`.

### Movement, Facing, and Targeting

- Startup and Release do not allow normal translation in the first version.
- Hold allows movement but never Sprint.
- Unlocked Hold preserves current camera-relative free movement and smoothly faces the real movement direction.
- Locked Hold preserves current lock-on directional locomotion and keeps facing `PlayerTargeting.CurrentTarget`.
- Guard never locks, unlocks, or otherwise switches the camera mode.
- During unlocked Startup, Guard may perform one facing-assist search within the player's forward total `120` degrees (`+/-60` degrees). The selected Collider is temporary Guard context and must never be written into the authoritative `PlayerTargeting.CurrentTarget`.
- During locked Startup, Guard uses the existing `PlayerTargeting.CurrentTarget` directly and performs no additional `120`-degree search.

### Defense Boundary Deferred

- Future ordinary Guard protects only the player's forward total `180` degrees (`+/-90` degrees), not all directions.
- Actual Hit / Block / Perfect Guard resolution is deliberately not part of the first lifecycle implementation.
- Current `EnemyAttack.ApplyDamage(PlayerHealth target)` and `PlayerHealth.TakeDamage(int damageAmount)` do not carry attack-source direction. Revisit the minimum source-information and Damage / Defense Resolution boundary only when forward-arc defense begins.
- Do not create a general Hit Framework, Ability Framework, or abstract hierarchical FSM in anticipation of that later work.

### Animation Direction

- The learner accepted the isolated cross-package preview: the new `SwordAnimationPack` transitions smoothly enough with the current Katana presentation, and its Block movement is usable.
- Approved first candidates are `Block_Start`, `Block_Loop`, `Block_End`, and the directional `Walk_Block_*` family. `Block_Hit`, `Block_Hit_Break`, and dedicated turn clips remain optional later content rather than first-lifecycle requirements.
- These assets were inspected in `C:\Unity\Project\RelicGuardianAssetLab`; they have not been integrated into the main project or verified there in Play Mode.
- The directional source names include `_RM`, but the project rule remains Apply Root Motion off. Gameplay displacement stays code-driven through `CharacterController`.

### Actual-Code Compatibility Audit

- The existing coarse-state ownership is compatible with adding `Blocking`; no duplicate action controller or parallel locked action flow is needed.
- `PlayerMovement.FaceDirection(Vector3)` already provides a reusable movement-owned facing seam for Startup assistance.
- `PlayerTargeting.CurrentTarget` already supplies the locked authoritative target, but its current nearest-target search is private and has no forward-angle policy. The unlocked one-shot Guard search requires a later narrow design and must not mutate `currentTarget`.
- `PlayerActionController.CanMove` currently returns true only for `Free`. It cannot yet express “Hold can move while Startup/Release cannot,” so phase-aware movement permission must be added only when Guard movement is connected.
- The current Sprint and Shift-to-unlock rules are gated by `CanMove`. Once Hold can move, they also need an explicit Guard/Sprint permission boundary so held Guard cannot Sprint or cancel lock through Shift.
- `PlayerInputReader` and the Input Actions asset currently have no Block representation. The player uses `Send Messages`, and the existing Sprint work already established that `Pass Through` is required for reliable release-value delivery in this setup. Guard input needs both persistent held state and a one-use press edge; consuming a rejected edge prevents held input from becoming an unintended buffer.
- `PlayerAnimator` currently owns parameter writes and Attack Trigger presentation only. Adding code-driven Guard CrossFades fits its responsibility without moving action permission into Animator.
- `PlayerCombat` has no Attack-to-Block cancellation path. That matches the approved first version and must remain unchanged.
- A future independently consuming `PlayerBlock.Update()` alongside existing Attack and Jump consumers would make Block/Attack, Block/Jump, and the accepted press frame's translation depend on script execution order. Choose and implement the smallest deterministic results when Block entry is connected; do not add a numeric Priority system.

### Minimum Implementation Order

1. Add only Block input representation in the Input Actions asset and `PlayerInputReader`: a `Pass Through` action, persistent held state, and a one-use press edge so a rejected press cannot start automatically later while the key remains held.
2. Before connecting entry, choose deterministic same-frame results for Block versus Attack and Jump, including the accepted press frame's translation cutoff; then add only the minimum `Free -> Blocking -> Free` coarse transition.
3. Add `PlayerBlock` ownership of Startup, Hold, Release, Startup decision/exit behavior, and final cleanup, without damage resolution.
4. Connect `Block_Start`, `Block_Loop`, and `Block_End` through code-driven CrossFade and authored phase/exit Events, keeping Root Motion off.
5. Add phase-specific translation and Sprint permission while preserving existing unlocked and locked Hold movement/facing behavior.
6. Add the one-shot Startup facing assistance: current authoritative target when locked, temporary forward `120`-degree search when unlocked.
7. Add the authored Perfect Guard Window lifetime as a separate concept, still without successful-Guard resolution.
8. Run focused lifecycle, early-release, movement, targeting-mode, animation, and Console regressions.
9. Later design the forward `180`-degree Damage / Defense Resolution boundary as a separate feature.

### Explicitly Out of Scope

- Attack-to-Block cancellation.
- A general numeric Priority system.
- Perfect Guard success consequences, Parry, or Counter.
- A general Ability Framework or abstract hierarchical FSM framework.
- Dodge or a pre-emptive `PlayerMotor` split.

## 2026-08-27 Basic Attack Interruption Priority Revision (Latest Planned Work)

This final section supersedes the earlier statements that the first Guard does not support Attack-to-Block cancellation. It changes approved design only; no cancellation or Block gameplay code has been implemented or runtime-verified.

### Revised Priority Rule

- At the gameplay-design level, Block has higher action priority than the current four-step Basic Attack, which is the project's current normal attack / basic attack chain.
- Block wins an otherwise simultaneous Block/Attack request while `Free`, and an accepted Block request may pre-empt the active Basic Attack throughout Startup, Hit Window, and Recovery.
- Future skills do not become Block-cancellable automatically. Each skill may deny interruption completely or allow it only in authored phases. Higher priority selects among legal transitions; it does not create permission that the current skill forbids.
- The rare same-frame case remains an explicit deterministic arbitration rule so its result never depends on `MonoBehaviour.Update()` execution order. Do not build a general numeric Priority system for the current two-action case.

### Cancellation Result

- If Block cancels the Basic Attack before `OpenHitWindow()`, that attack step never applies damage.
- If `OpenHitWindow()` already applied damage, later Block cancellation does not roll that damage back.
- `PlayerCombat` must own one centralized cancellation-cleanup boundary that closes Hit, Combo, and Restart Windows; clears the queued request, transition state, current and confirmed targets, facing, lunge, travelled distance, and attack index; and only then releases `Attacking` for the `Blocking` transition.
- Animator presentation must follow the accepted gameplay transition. Playing a Block CrossFade alone is never sufficient cancellation.

### Updated Minimum Order

1. Continue with Block input representation only: `Pass Through`, persistent held state, and a one-use press edge.
2. Add and verify centralized Basic Attack cancellation cleanup without connecting Block.
3. Connect the explicit Block-over-Basic-Attack same-frame and active-interruption rules, then enter coarse `Blocking`.
4. Continue with the previously approved `PlayerBlock` Startup/Hold/Release lifecycle, CrossFade presentation, movement permissions, facing assistance, and authored Perfect Guard Window one concept at a time.
5. Keep Damage / Defense Resolution, Parry/Counter, Dodge, future skill policies, a general numeric Priority system, and framework abstractions deferred.

## 2026-08-27 Guard Input, Attack Cancellation, and Centralized Arbitration Handoff (Latest Current State)

This final section supersedes earlier statements that Block input and Basic Attack cancellation are still unimplemented, and it refines the earlier wording that assigned all Guard request coordination to `PlayerBlock`. No `Blocking` state, `PlayerBlock` lifecycle, Guard animation, Guard movement permission, or defense resolution has been connected.

### Completed Block Input Representation

- `PlayerInputReader` now stores `blockRequested` and `isBlockHeld`, exposes `IsBlockHeld`, receives both press and release through `OnBlock(InputValue value)`, and exposes the one-use press edge through `ConsumeBlock()`.
- `OnBlock()` updates the persistent held state on both edges and sets `blockRequested` only on press. Release never clears an unconsumed press request; only `ConsumeBlock()` does that.
- `RelicGuardianPlayer.inputactions` now contains one `Block` action with Action Type `Pass Through`, Control Type `Button`, and one binding: `<Mouse>/rightButton`. An accidental empty binding was removed.
- Play Mode right-button press/release checks completed with zero Console errors and zero warnings. There is intentionally no visible Guard response yet.

### Completed Basic Attack Cancellation Boundary

- `PlayerCombat.FinishAttack(int attackIndex)` retains its attack-step identity guard and now delegates termination to the shared private `EndAttack()` boundary.
- `EndAttack()` closes Hit, Combo, and Restart Windows; clears queued and transition state; clears current and confirmed targets; clears facing and lunge state; resets travelled distance and attack index; then calls `PlayerActionController.FinishAttack()` last.
- `TryCancelAttack()` returns `false` outside `Attacking`; while `Attacking`, it calls `EndAttack()` and returns `true`. It is not connected to Block yet.
- Unlocked and locked four-step natural endings still restore movement and Jump. A temporary context-menu test produced `False` in `Free`, `True` while `Attacking`, then `False` immediately after cancellation; movement recovered and the temporary test method was removed.

### Approved Single Arbitration Point

- The six approved deterministic results are fixed: grounded `Free` Block beats same-frame Attack and Jump; accepted Block stops horizontal translation on its press frame; active-Basic-Attack Block beats a same-frame combo/restart Attack and cancels the attack first; rejected Block is consumed and never starts later merely because the button remains held.
- `PlayerActionController` is the unique Block / Attack / Jump cross-action arbitration point because it already owns the coarse Gameplay FSM. It will consume all three raw discrete requests once per frame, choose at most one accepted result, and dispatch only accepted commands to the owning subsystem.
- `PlayerCombat` must stop consuming raw Attack requests and instead receive accepted start/queue/restart commands while retaining attack-sequence ownership. `PlayerMovement` must stop consuming raw Jump requests and instead execute an accepted Jump command while retaining displacement and vertical-motion ownership. Future `PlayerBlock` receives an accepted Block command and owns Startup/Hold/Release only.
- A small idempotent per-frame resolution gate in `PlayerActionController` will make the first call in a frame perform the decision and later calls no-op. Relevant behavior owners call that gate before movement, lunge, or phase work, but they never inspect competing raw requests or implement priority branches themselves.
- This design does not use `PlayerActionController.Update()` ordering, Unity Script Execution Order, `HasBlockRequest`, a numeric Priority system, a general Request Queue, or a new Coordinator component.

### Exact Next Order

1. Centralize the existing Attack and Jump request consumption in the one-per-frame `PlayerActionController` resolution boundary while preserving all current behavior. Do not add `Blocking` in the same concept.
2. Runtime-regress grounded Attack, combo queue/transition/restart, grounded and locked Jump results, movement cutoff while `Attacking`, lunge, natural cleanup, and the Console.
3. Add the minimum coarse `Blocking` entry and connect the approved Block-versus-Attack/Jump and accepted-frame translation results through the central arbiter.
4. Add `PlayerBlock` Startup/Hold/Release, presentation, movement permission, facing assistance, and Perfect Guard Window one concept at a time as already approved.
5. Keep Damage / Defense Resolution, Parry/Counter, Dodge, future-skill policies, numeric priorities, queues, and framework abstractions deferred.

### Git Boundary Clarification

- Local full-project repository: `C:\Unity\Project\My project`; current local HEAD remains `4719b71 Record Guard design and attacking migration`.
- GitHub code/document mirror: `LearnedYet/Relic-Guardian-Code`; latest confirmed mirror sync is `5db2fed Sync Guard planning and ChatGPT project context`.
- These commit hashes belong to different histories. Do not treat `5db2fed` as a descendant or replacement commit inside the local full-project repository, and do not directly pull or merge the mirror's `main` into the full Unity workspace.
- All work in this handoff remains uncommitted locally. No commit, push, history rewrite, staging, or remote mutation was performed in this documentation update.

## 2026-08-28 Guard Lifecycle and Soft Recovery Handoff (Latest Current State)

This section supersedes the earlier Guard implementation order and all statements that `Blocking`, `PlayerBlock`, or Guard presentation are not implemented. Earlier sections remain chronological design history.

### Runtime-Verified Implementation

- `PlayerActionController` now centrally consumes Block, Attack, and Jump requests through one idempotent per-frame `ResolveActionRequests()` gate. Current coarse states are `Free`, `Attacking`, and `Blocking`.
- The concrete arbitration order is Block -> Attack -> Jump. The six approved same-frame rules passed: grounded Block wins Free-state Attack/Jump, accepted Block cuts off horizontal translation that frame, Block cancels active Basic Attack, same-frame combo/restart Attack loses, and rejected Block is consumed without buffering.
- `PlayerBlock` owns `Startup`, `Hold`, and `Release`. `StartupDecisionPoint()` selects Hold or Release from the current held value; Hold release enters Release; `FinishRelease()` returns the coarse action to `Free`.
- `PlayerCombat.EndAttack()` remains the single natural/cancel cleanup boundary. Step-identity plus coarse-state checks reject stale Animation Events after cancellation.
- `PlayerAnimator` uses code-driven CrossFade calls for `Block_Start`, `Block_Loop`, and `Block_End`. Root Motion remains disabled.

### Local Guard Presentation

- The ignored local lifecycle assets are `Block_Start.anim`, `Block_Loop.anim`, and the edited `Block_End_NoRootTurn.anim`; the project-owned Animator state is still named `Block_End`.
- The local Events are `StartupDecisionPoint` at `0.4s` and `FinishRelease` at `0.75s`.
- Matching the Guard Clip root-rotation offsets at `-66` removed the Start/End facing turn. The normal Guard End -> locomotion blend is accepted at `0.45s`.
- Current Scene overrides are Guard crossfade `0.03s`, Block exit `0.45s`, and soft-recovery interruption `0.05s`. These Scene values remain in the protected local Scene and are not part of the code/document mirror.

### Soft Recovery Contract

- Soft recovery is the visual interval from a gameplay `Finish...` Event until the authored Clip tail ends. The coarse action has already returned to `Free`; it is not another gameplay state.
- No input preserves the authored tail. Movement, a new Basic Attack, or Guard can visually interrupt it without restoring old Hit Windows, lunge, targeting, or attack ownership.
- `PlayerAnimator` tracks whether soft recovery is active and whether its Animator transition has actually begun. This prevents Unity's transition-reporting delay from clearing the lifecycle too early.
- A normal Guard exit keeps the `0.45s` visual blend; interruption uses `softRecoveryInterruptCrossFadeDuration`. Attack reuses its own `FinishAttack` -> Clip-end tail and has no redundant Attack-exit field.
- The local licensed Attack4 `Attack_3Combo_3_Inplace` now uses `FinishAttack(3)` at normalized `0.59016937`; `OpenHitWindow(3)` and `CloseHitWindow(3)` remain at `0.31615335` and `0.39201885`. This local `.meta` remains ignored.
- The retained `debugBodyYawOffset` audit reads `Animator.bodyRotation` only in `OnAnimatorIK()`. It exists for later animation diagnosis and no longer produces the previous per-frame Unity 6 warning.

### Honest Remaining Boundary

- Guard is still stationary through all three phases because `PlayerActionController.CanMove` is true only in `Free`. The approved locked/unlocked Hold movement and no-Sprint rule are not implemented.
- Startup facing assistance, Perfect Guard Window, forward `180`-degree Guard coverage, attack-source direction, Damage / Defense Resolution, Block Hit, Guard Break, Parry/Counter, and directional Guard movement Clips are not implemented.
- The next single concept is phase-aware Guard movement permission: stationary Startup/Release, movable Hold, no Sprint, unlocked camera-relative movement/facing, and locked directional movement/target-facing. Keep directional Guard Clips, Startup assistance, Perfect Guard, defense resolution, and other advanced actions separate.

### Git Boundary

- Local full-project and GitHub mirror histories remain separate. Their current tips must be read within their own histories; do not merge or pull mirror `main` into the full Unity project.
- Commit only the explicit project-owned code, Input Actions, Animator Controller, and Docs allowlist. Keep the protected Prefab and Scene unstaged.
- `Assets/LocalLicensed/` and all licensed Clip/Event tuning remain ignored and must never be uploaded.

## 2026-08-28 Phase-Aware Guard Hold Movement Handoff (Latest Current State)

This section supersedes the previous final section's statement that Guard remains stationary through all three phases. Earlier Handoff sections remain chronological history.

### Implemented Permission Boundary

- `PlayerBlock.AllowsMovement` exposes a read-only internal permission without exposing `BlockPhase`. It is true only while the phase is Hold and Block remains held.
- The held-input condition makes movement close immediately on release even if `PlayerMovement.Update()` runs before `PlayerBlock.Update()` on that frame.
- `PlayerActionController.CanMove` now accepts `Free` or `Blocking` with `playerBlock.AllowsMovement`. `CanSprint` remains true only in `Free`.
- `PlayerMovement` still uses `CanMove` for input filtering and locked target-facing. Only its Sprint speed selection and Sprint-driven `CancelLockOn()` branches use `CanSprint`.
- No `PlayerMotor`, directional Guard Clip integration, Startup facing, Perfect Guard, defense resolution, request queue, numeric priority, or additional action state was added.

### Verification

- `Assembly-CSharp.csproj` compiled with zero errors and zero warnings.
- The learner reported the focused Play Mode checklist passed: Startup and Release remain stationary; unlocked Hold preserves camera-relative movement and movement-facing; locked Hold preserves directional movement and target-facing.
- Holding Sprint during Block neither increases speed nor cancels Lock-On. Free Sprint and its locked-mode cancellation behavior remain unchanged, Attack still blocks ordinary movement, and the Unity Console remained clean.
- The current stationary `Block_Loop` presentation can visibly slide during Hold; directional `Walk_Block_*` presentation remains a separate later concept.

### Exact Next Concept

- Add only one-shot Startup facing assistance. Locked Startup reuses `PlayerTargeting.CurrentTarget`; unlocked Startup may select one temporary candidate inside the forward `120` degrees without mutating `CurrentTarget`.
- Keep Hold movement, directional Guard Clips, Perfect Guard, forward-arc defense, Damage / Defense Resolution, Block Hit, Guard Break, Parry/Counter, Dodge, and framework abstractions separate.

### Git Boundary

- The project-owned code changes are limited to `PlayerBlock.cs`, `PlayerActionController.cs`, and `PlayerMovement.cs`, plus focused documentation updates.
- The protected Prefab and Scene remain modified local mixed assets and must stay unstaged unless separately reviewed.
- `Assets/LocalLicensed/` and licensed Clip/Event tuning remain ignored and must never be committed or uploaded.
- No commit, push, staging, history rewrite, or remote mutation has been performed.

## 2026-08-28 Formal Guard Hold Presentation Direction Handoff (Latest Current State)

This section supersedes the previous final section's next-concept order. It does not change the runtime-verified gameplay permission checkpoint.

### Architecture Boundary

- There is still one Gameplay FSM: `PlayerActionController` owns `Free / Attacking / Blocking`.
- There is still one Guard lifecycle owner: `PlayerBlock` owns `Startup -> Hold -> Release` inside the single coarse `Blocking` state.
- The unlocked/locked distinction is Hold presentation only. It does not create another `PlayerBlock`, `PlayerActionState.GuardTurning`, parallel action route, Root Motion facing owner, or Guard-specific movement parameter pair.

### Formal Hold Presentation

- Unlocked Hold uses camera-relative movement, code-owned smooth Transform turning, and a simple `Guard_Free_Locomotion` Idle/Forward presentation. It is not true eight-way strafe locomotion.
- Locked Hold remains target-facing and uses `Guard_Locked_Locomotion`, a true Guard 8-Way Blend Tree driven by the existing `MoveX` / `MoveZ`.
- `PlayerAnimator` selects the appropriate Hold state with `CrossFadeInFixedTime()`. Continuous Blend Trees remain Animator presentation; gameplay permission remains outside Animator.
- `MotionSpeed` is not suitable for the unlocked Idle/Forward blend because current code substitutes `1` when movement strength is zero. Reuse the existing `Speed` parameter for the first-version Idle/Forward blend.
- Because `PlayerTargeting` currently allows Lock-On toggling during Hold, presentation selection must notice a mode change after Hold entry and CrossFade only when the desired mode changes.

### One-Shot Turn Presentation

- `Turn_Block_90_L`, `Turn_Block_90_R`, `Turn_Block_180_L`, and `Turn_Block_180_R` are one-shot unlocked-Hold presentation, not children of a continuous locomotion Blend Tree.
- The future minimum lifecycle is Not Turning -> one significant direction-change decision -> Turning -> completion or legal interruption -> unlocked Hold locomotion -> Not Turning.
- Transform facing remains owned by `PlayerMovement`; Apply Root Motion stays off. Block Release and other legal presentation changes can interrupt the Turn Clip.
- Prototype angle bands are below `60` degrees: no Turn; `60` through `135` degrees: 90-degree Turn; above `135` degrees: 180-degree Turn. Clip direction and thresholds require P09 runtime evidence.

### Minimum Implementation Order

1. Integrate only unlocked Guard Hold Idle/Forward presentation.
2. Integrate locked Guard Hold 8-Way presentation with existing `MoveX` / `MoveZ`.
3. Regress both Hold modes, Release, Sprint rejection, Lock-On retention, and the Console.
4. Audit the four Turn Clips for Loop, Root Transform Rotation, left/right direction, and P09 presentation.
5. Add only the unlocked Hold one-shot Turn presentation lifecycle.
6. Tune Guard presentation turn speed and angle thresholds.
7. Continue to Startup `120`-degree facing assistance afterward.

### Current Asset and Git Boundary

- The main project currently has only the static `Block_Loop` state and no imported `Walk_Block_*` or `Turn_Block_*` files. The accepted candidates remain in `C:\Unity\Project\RelicGuardianAssetLab` until deliberately copied into ignored `Assets/LocalLicensed/`.
- The protected Prefab and Scene remain unstaged. Licensed animation assets and local Override mappings must never be committed or uploaded.
- No gameplay, Animator, Prefab, Scene, staging, commit, push, or remote state changed during this design confirmation.

## 2026-08-28 Unlocked Guard Hold Presentation Handoff (Latest Current State)

This section supersedes the preceding final section's statement that the main project has no integrated Guard Hold locomotion presentation. The formal architecture and implementation order otherwise remain unchanged.

### Runtime-Verified Unlocked Presentation

- The ignored local licensed Guard folder now contains the deliberately copied `Walk_Block_Loop_F_0_RM.anim` plus its preserved `.meta`. No other walk or Turn Clip was imported in this step.
- The project-owned Animator Controller contains `Guard_Free_Locomotion`, a 1D Blend Tree driven by existing `Speed`: `Block_Loop` at threshold `0` and the forward Guard Walk at threshold `3`. Automatic thresholds are off, and the state has no authored transitions.
- `PlayerAnimator.PlayBlockLoop()` was renamed to `PlayBlockHold()`. At Hold entry, unlocked presentation CrossFades to `Base Layer.Guard_Free_Locomotion`; locked presentation temporarily continues to CrossFade to `Base Layer.Block_Loop`.
- This remains presentation-only. `PlayerActionController` still owns the single coarse `Blocking` state, `PlayerBlock` still owns Startup/Hold/Release, `PlayerMovement` still owns displacement and Transform facing, and Apply Root Motion remains off.
- `Assembly-CSharp.csproj` compiled with zero errors and zero warnings. Unity finished its domain reload and its Console error/warning query was empty.
- The learner reported all focused Play Mode checks normal: stationary unlocked Hold uses Guard Idle, movement uses Guard Forward while existing code-owned turning remains active, Release returns normally, held Sprint stays rejected, and the Console remains clean.

### Exact Next Concept

- Add only `Guard_Locked_Locomotion` as the true Guard 8-Way Hold Blend Tree using the existing `MoveX` / `MoveZ` parameters.
- Keep the current unlocked Blend Tree unchanged. Do not add `GuardMoveX`, `GuardMoveZ`, another gameplay state, Root Motion facing, Turn lifecycle code, or Startup facing assistance in the same concept.
- After locked 8-Way is connected, run the combined unlocked/locked Hold, Release, Sprint rejection, Lock-On retention, and Console regression. Then audit the four Turn Clips separately before adding their one-shot presentation lifecycle.

### Git Boundary

- The Animator Controller and `PlayerAnimator.cs` are modified project-owned files. The protected Prefab and Scene remain dirty and unstaged.
- `Assets/LocalLicensed/` remains ignored and must never be committed or uploaded.
- No staging, commit, push, history rewrite, or remote mutation was performed.

## 2026-08-28 Locked Guard Hold 8-Way Handoff (Latest Current State)

This section supersedes the preceding final section's temporary locked `Block_Loop` fallback and exact-next-concept text. The single Gameplay FSM and single `PlayerBlock` lifecycle remain unchanged.

### Runtime-Verified Hold Presentation

- `Guard_Free_Locomotion` remains the unlocked `Speed` 1D Idle/Forward presentation with code-owned Transform turning.
- `Guard_Locked_Locomotion` is now one 2D Simple Directional Blend Tree driven by existing `MoveX` / `MoveZ`. It has exactly nine non-empty children: center Guard Idle plus eight normalized movement directions, with no authored transitions.
- The eight standard directional Loop Clips are the numbered `01–08` set copied into ignored `Assets/LocalLicensed/`; alternative `09/10` lateral Clips, Start/Stop Clips, and all Turn Clips remain outside the main project.
- `PlayerAnimator.PlayBlockHold()` CrossFades to the locked or unlocked state from `PlayerTargeting.IsLockedOn` at Hold entry. No Guard-specific movement parameters, new gameplay state, Root Motion facing, or duplicate action path was added.
- `Assembly-CSharp.csproj` compiled with zero errors and zero warnings. The learner reported locked eight directions, target-facing, Release, Sprint rejection, Lock-On retention, unlocked Guard regression, and Console all normal.
- The learner adjusted only `Walk_Block_Loop_F_0_RM` to `Orientation Offset Y = -36` and accepted the Forward visual angle as basically correct on P09. Other seven directional Clip settings remain unchanged.
- A later MCP Console confirmation attempt did not run: the reconnected Unity MCP endpoint continued returning HTTP `502`, and the current Codex session exposed no Unity tools. The Console result above is therefore learner-reported, not independently MCP-retrieved on the final pass.

### Known Presentation Gap

- Hold presentation is selected at `EnterHold()`. If Lock-On changes during an already-active Hold, the current code does not yet CrossFade to the other Hold Blend Tree. That refresh remains a separate presentation-lifecycle concept.

### Exact Next Concept

- Import and audit only `Turn_Block_90_L`, `Turn_Block_90_R`, `Turn_Block_180_L`, and `Turn_Block_180_R` for Loop, Root Transform Rotation, actual direction, and P09 visual compatibility.
- Do not add Turn detection, thresholds, lifecycle fields, gameplay states, Root Motion facing, or Startup facing assistance during the audit.
- After the isolated audit, add the minimum interruptible unlocked-Hold one-shot Turn presentation lifecycle, then tune turn speed and the prototype `60 / 135` degree thresholds.

### Git Boundary

- The project-owned Animator Controller and `PlayerAnimator.cs` are modified. The protected Prefab and Scene remain dirty and unstaged.
- All copied licensed Walk/Turn assets and local Clip settings remain under ignored `Assets/LocalLicensed/` and must never be committed or uploaded.
- No staging, commit, push, history rewrite, or remote mutation was performed.

## 2026-08-29 Guard Turn Deferral and Cleanup Handoff (Latest Current State)

This section supersedes the previous final section's Turn-lifecycle next step. The runtime-verified unlocked and locked Hold locomotion remains unchanged.

### Current Runtime Direction

- Unlocked Guard Hold uses `Guard_Free_Locomotion` for Guard Idle/Forward while `PlayerMovement` keeps camera-relative displacement and smooth code-owned Transform turning.
- Locked Guard Hold uses the existing `Guard_Locked_Locomotion` 8-Way Blend Tree and remains target-facing.
- The learner's post-cleanup Play Mode regression passed unlocked smooth turning, locked eight-way movement, Release, Sprint rejection, and the Console.

### Turn Audit and Deferral

- The ignored local `Turn_Block_90_L/R` and `Turn_Block_180_L/R` Clips have Loop Time disabled. Their authored RootQ trajectories were measured at approximately `-87`, `+87`, `-180`, and `+180` degrees.
- A minimum presentation-only lifecycle was implemented and tested without adding a gameplay `GuardTurning` state or enabling Root Motion. P09 runtime exposed over-rotation and, more importantly, a contract mismatch: short or changing movement input does not guarantee that the smooth code-owned Transform will complete a fixed 90/180-degree turn.
- The experiment was fully removed. `PlayerAnimator` has no Turn thresholds, lifecycle fields, angle detection, or Turn CrossFades; `PlayerBlock` no longer exposes the temporary Hold-phase reader; `PlayerMovement` again uses its local `moveDirection`; and the Animator Controller has no `Turn_Block_*` states.
- The licensed Turn Clips remain only in ignored `Assets/LocalLicensed/` for possible future use by a deliberately designed turn-in-place system. They are not part of the current Guard runtime.

### Exact Next Concept

- Add only Hold presentation mode-change refresh. During an already-active Hold, detect a change in `PlayerTargeting.IsLockedOn` and CrossFade once between `Guard_Free_Locomotion` and `Guard_Locked_Locomotion`.
- Do not CrossFade every frame. Keep gameplay permissions, movement/facing ownership, Startup `120`-degree assistance, Perfect Guard, defense resolution, and future turn-in-place design separate.

### Git Boundary

- The protected Prefab and Scene remain dirty and unstaged. Their existing mixed local changes were not modified by the cleanup.
- Licensed animation assets remain ignored and must never be committed or uploaded.
- No staging, commit, push, history rewrite, or remote mutation was performed.

## 2026-08-29 Guard Hold Mode Refresh Handoff (Latest Current State)

This section supersedes the previous final section's exact-next-concept text. The Turn deferral and all previously verified Guard movement/presentation boundaries remain unchanged.

### Runtime-Verified Presentation Refresh

- `PlayerAnimator` now owns two presentation-only bools: `isBlockHoldPresentationActive` gates the Hold presentation lifetime, and `isBlockHoldPresentationLocked` records which Hold variant was last selected.
- `PlayBlockStart()` and `PlayBlockEnd()` set the active flag false. `PlayBlockHold()` sets it true, records the current authoritative `PlayerTargeting.IsLockedOn` value, and CrossFades to the corresponding free or locked Hold Blend Tree.
- During `Update()`, only an active Hold whose recorded presentation differs from current Lock-On calls `PlayBlockHold()` again. Recording the new value before CrossFade prevents repeated per-frame re-entry.
- This is presentation state only. `PlayerTargeting` remains Lock-On authority, `PlayerBlock` remains Startup/Hold/Release owner, and `PlayerMovement` remains displacement/facing owner.
- The learner's Play Mode pass verified unlocked -> locked -> unlocked switching inside one continuous Hold, stable playback after each switch, correct Startup and Release behavior, continued Sprint rejection, and a clean Console.

### Exact Next Concept

- Add only one-shot Startup facing assistance. Locked Startup reuses `PlayerTargeting.CurrentTarget`; unlocked Startup selects one temporary candidate inside the forward total `120` degrees (`+/-60`) without mutating `CurrentTarget`.
- Keep the temporary target lifetime inside Guard Startup. Do not add Perfect Guard, defense coverage, Damage / Defense Resolution, Parry/Counter, Guard Break, Dodge, another gameplay FSM, or a general targeting framework in the same concept.

### Git Boundary

- The protected Prefab and Scene remain dirty and unstaged. No cleanup or mode-refresh work modified those mixed local assets.
- Licensed animation assets remain ignored and must never be committed or uploaded.
- No staging, commit, push, history rewrite, or remote mutation was performed.

## 2026-08-29 Guard Hit Resolution Design Handoff (Latest Current State)

This section is the authoritative latest Handoff and supersedes every earlier Handoff statement that says Guard Startup should search for a temporary enemy, reuse `PlayerTargeting.CurrentTarget` for Startup facing assistance, or implement target-driven Startup facing as the next concept. Earlier sections remain historical only.

The complete approved design is recorded in `Docs/GUARD_HIT_RESOLUTION_DESIGN.md`. Read that file before teaching or implementing the next concept.

### Actual Implementation State

- No incoming-hit context, `PlayerHitReceiver`, Guard Coverage, Successful Guard Facing Assist, or Perfect Guard behavior has been implemented.
- The current enemy path remains `EnemyAI` -> `EnemyAttack.OpenHitWindow()` -> `EnemyAttack.ApplyDamage(PlayerHealth)` -> `PlayerHealth.TakeDamage(int)`.
- `EnemyAttack` currently sends only the integer damage amount. It carries no source identity or authoritative incoming direction.
- The current melee prototype is a scheduled hit attempt, not physical collision confirmation: range is checked before Startup, and the saved target is damaged at Hit Window without a new overlap, range, or line-of-sight check.
- `PlayerBlock` still owns only Startup/Hold/Release lifecycle and Hold movement permission. `PlayerMovement` still owns every actual Transform-facing write. `PlayerTargeting` has no incoming Guard role.
- All Guard Hold movement and presentation runtime verification recorded in the preceding latest checkpoints remains valid and unchanged.

### Superseded Rule and Formal New Concept

- Empty Guard never searches for an enemy, never acquires a temporary target, never reads a Lock-On target for assist, and never starts facing assistance.
- The formal concept is `Successful Guard Facing Assist`, not Startup Facing Assistance.
- A real incoming hit first enters Guard resolution. Guard Coverage and assist eligibility both use one angle captured from the player's facing before any automatic turn.
- Guard Coverage resolves first. Only a successful Guard may start Facing Assist. Automatic turn must never enlarge Guard Coverage or convert a failed Guard into a successful one.
- Facing Assist stores the fixed horizontal direction obtained from this hit and never tracks `Source Transform` afterward.

### Fixed Guard Phase Rules

- Startup can defend.
- Hold can defend.
- Release cannot defend.
- Perfect Guard Window is a short authored subset of Startup.
- A valid Startup hit outside Perfect Guard Window is an ordinary Guard, not a failed Guard.

No new coarse gameplay state is added. `PlayerActionController` remains `Free / Attacking / Blocking` until a later concrete action such as Dodge is introduced.

### Minimum Shared Hit Boundary

The approved first-version value is one immutable three-field `HitContext`:

- `int DamageAmount`;
- `Transform Source`;
- `Vector3 IncomingDirection`.

`IncomingDirection` means the normalized world-space direction in which the attack is travelling toward the receiver at the hit moment. Melee calculates source-to-player direction at its hit attempt; a future Projectile supplies impact velocity/travel direction. Guard uses the horizontal inverse to face the attack's origin side. Never recompute this direction later from `Source.position`.

Do not add `HitPoint`, damage types, blockable tags, attack IDs, teams, knockback, delivery-object references, `IDamageable`, a general result hierarchy, event bus, Damage Manager, or Ability Framework in the first concept.

Add one small `PlayerHitReceiver` as the single player hit entry:

1. Receive `HitContext`.
2. Call existing idempotent `PlayerActionController.ResolveActionRequests()` before reading the coarse state so same-frame input/hit results do not depend on `MonoBehaviour.Update()` order.
3. Route `Blocking` to `PlayerBlock`; route future `Dodging` to `PlayerDodge` only when that state exists.
4. If no current action handles the hit, call existing `PlayerHealth.TakeDamage(int)`.

The first version needs only a handled/not-handled boolean boundary. Ordinary versus Perfect Guard remains internal to `PlayerBlock`.

### Adjustable Angles and Pre-Turn Resolution

Keep the first-version tunable values on `PlayerBlock`:

- `facingAssistHalfAngle = 60f`, meaning total `120` degrees;
- `guardCoverageHalfAngle = 90f`, meaning total `180` degrees;
- one short `guardFacingAssistDuration`, with its initial prototype number still a tuning choice.

Continue to reuse existing `PlayerMovement.rotationSpeed`. Add no Guard-specific rotation-speed field without runtime evidence.

For each hit, `PlayerBlock` must:

1. Require coarse `Blocking` and phase Startup or Hold.
2. Derive a fixed horizontal direction toward the attack from `IncomingDirection`.
3. Snapshot player forward before turning and calculate one original angle.
4. Compare that original angle to Guard Coverage.
5. On coverage failure, do not assist and allow health damage.
6. On coverage success, resolve Perfect Guard only if the Startup window is open; otherwise resolve ordinary Guard.
7. Compare the same original angle to Facing Assist eligibility.
8. Start fixed-direction assist only after all qualification is complete.

Default mapping is `0-60`: Guard plus assist; above `60` through `90`: Guard without assist; above `90`: failed Guard and no assist.

### Sole Facing Owner and Explicit Branch

- `PlayerBlock` owns eligibility, the fixed assist direction, and its short lifetime, but it never applies Transform rotation from `Update()` or `LateUpdate()`.
- `PlayerMovement.Update()` remains the only facing application point.
- The fixed first-version branch order is active Guard Facing Assist, otherwise Locked Facing, otherwise Free Movement Facing.
- The assist branch is not gated by ordinary movement permission because Startup is stationary but may defend and assist.
- When assist ends, existing Locked or Free Movement facing resumes automatically.
- This is one explicit feature branch, not a numeric Priority system or general Facing Framework.

Assist ends when its short duration expires, the stored direction is reached closely enough, the action leaves `Blocking`, or the phase becomes Release. It does not retain or follow a target reference.

### Exact Next Concept and Implementation Order

Implement one concept at a time in this order:

1. Add only `HitContext` and the small `PlayerHitReceiver`; migrate the current `EnemyAttack` target/delivery path and verify that existing one-damage-per-attack behavior is unchanged. Do not add Guard prevention or angles in this concept.
2. Add Startup/Hold defendable phase checks, Release rejection, and adjustable `guardCoverageHalfAngle`; verify covered versus uncovered health results without Facing Assist.
3. Add adjustable `facingAssistHalfAngle`, fixed direction, short duration/end-time state, and read-only assist access on `PlayerBlock`.
4. Add the Guard Assist -> Locked -> Free Movement facing branch to `PlayerMovement` and verify that it remains the sole facing owner.
5. Add the authored Perfect Guard Window inside Startup. Startup hits outside the window remain ordinary Guard.
6. Add presentation and consequences only after the core result is runtime-verified.
7. Reuse `HitContext` for contact-based Perfect Dodge or Projectile only when those concrete features begin.

The next new conversation must not skip directly to angle or assist code. Start with the hit-data/delivery seam and preserve current damage behavior.

### Required First Tests

- Empty Startup/Hold never searches or rotates without a hit.
- Startup and Hold defend; Release does not.
- Default `50`-degree hit Guards and assists.
- Default `70`-degree hit Guards without assist.
- Default `100`-degree hit damages health without assist.
- Coverage uses the pre-turn angle and is not enlarged by automatic turn.
- Assist retains the fixed hit direction if `Source` moves afterward.
- Active assist overrides Locked and Free Movement facing only for its bounded lifetime, then normal facing resumes.
- Same-frame Block input and hit handling do not depend on `Update()` order.

### Files to Read First

- `AGENTS.md`
- `Docs/CURRENT_STATE.md`
- `Docs/GUARD_HIT_RESOLUTION_DESIGN.md`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyAI.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyAttack.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerBlock.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerMovement.cs`

### Protected Files and Git State

- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab` and `Assets/Scenes/SampleScene.unity` remain protected mixed local assets and intentionally dirty. Do not reset, overwrite, or broadly stage them.
- Future `PlayerHitReceiver` wiring and `EnemyAI.attackTarget` type migration will require deliberate Scene/player wiring review. Do not silently overwrite the protected Scene or Prefab.
- `Assets/LocalLicensed/` remains ignored and must never be committed or uploaded.
- This checkpoint changes documentation only. It does not claim compilation or Play Mode verification for the unimplemented design.
- No staging, commit, push, history rewrite, or remote mutation was performed for this design Handoff.
