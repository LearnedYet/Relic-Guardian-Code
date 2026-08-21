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
- Asset preparation has progressed into the first implementation checkpoint. Only `Attack_4Combo_1_Inplace` is connected as Attack1; Attack2, Attack3, and Attack4 remain unconnected.

### Current Attack1, Data, Combo-Window, and Idle Checkpoint

- The active local Override mapping is `SwordAndShieldSlash -> Attack_4Combo_1_Inplace`. Attack1 remains one Animator state driven by the existing `Attack` Trigger; there is no Animator `AttackIndex` and no Attack2 state.
- Attack1 is a non-looping Humanoid clip using the package Avatar. Apply Root Motion remains off. Its current events are approximately Frame `9.7` `OpenHitWindow`, `11.6` `OpenComboWindow`, `12.3` `CloseHitWindow`, `21.7` `CloseComboWindow`, and `34.2` `FinishBasicAttack`.
- `PlayerAttackData.cs` is an inline serializable configuration class with private serialized `damage`, `targetRange`, `lungeSpeed`, and `lungeDistance` plus read-only properties. The player Prefab currently has one Attack1 element configured as `1`, `2`, `5`, and `1`.
- `PlayerCombat` owns `currentAttackIndex`, returns `attacks[currentAttackIndex]` through `CurrentAttackData`, and initializes index `0` through reusable `StartAttackStep(int attackIndex)`. Damage, acquisition range, lunge speed, and lunge distance now read from current data; the one-entry Attack1 regression passed.
- `PlayerCombat` now has independent `isComboWindowOpen`, `OpenComboWindow()`, and `CloseComboWindow()`. The field is expected to produce an unused-field Console warning because input queueing has not been implemented yet.
- The next concept is only `isAttackQueued` plus input routing: a grounded request while `Free` starts Attack1, while a request during the open Combo Window only sets the queue. Do not add Attack2 or transition logic in that same step.
- The confirmed later flow is hybrid: input before the earliest `ComboTransitionPoint` queues; input after that point while the Combo Window remains open may enter Attack2 immediately. `ComboTransitionPoint` and its runtime-ready state are not implemented.
- The learner also wants `CloseComboWindow -> FinishAttack` to become a separately represented Restart Window in which Attack input starts a fresh Attack1. This is not implemented and cannot be inferred from `isComboWindowOpen == false`, because startup has the same value.
- Before enabling Attack2 or the Restart Window, prevent stale outgoing `FinishAttack`/window events from mutating a newly started attack execution.
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
- Combo implementation has begun but remains Attack1-only. Resume with one concept: add `isAttackQueued` and split initial-attack versus open-Combo-Window input handling. Do not add Attack2, Animator `AttackIndex`, `ComboTransitionPoint`, Restart Window, or large state-machine infrastructure in that same step.
- The in-progress two-hit architecture and confirmed later Restart Window are documented in `Docs/COMBO_ATTACK_ARCHITECTURE.md`. Keep that tracked file in every future sync to the code-focused GitHub repository and distinguish verified implementation from target design.
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
