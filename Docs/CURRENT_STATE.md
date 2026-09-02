# Relic Guardian Current State

Last reviewed against the local workspace: 2026-09-02

This is the short current-state entry point. Actual code, Unity assets, current Editor state, and Git status remain authoritative when they conflict with this file.

## Project Environment

- Local project: `C:\Unity\Project\My project`
- Unity: `6000.3.19f1`
- Render pipeline: URP `17.3.0`
- Input System: `1.19.0`
- Cinemachine: `3.1.7`
- Movement: `CharacterController`
- Apply Root Motion: off
- Target platform: Windows

## Git Boundary

- Local full-project repository: `C:\Unity\Project\My project`; current line: local `main`.
- GitHub code/document mirror: `LearnedYet/Relic-Guardian-Code`; current line: remote `main`.
- Latest local full-project checkpoint: `f72aae8 Complete Guard hit resolution and Combat VFX resource setup`.
- Latest confirmed GitHub code/document mirror checkpoint: `4cca972 Sync Guard hit resolution and Combat VFX setup`.
- The local full-project repository and GitHub mirror have different history shapes. Resolve each current tip in its own repository; never compare their hashes as one ancestry or directly pull/merge mirror `main` into the full Unity workspace.
- The mirror stores flattened project-owned C# files at its root and selected reproducible Unity configuration under `UnityConfig/`; it is not a complete Unity-project clone.

The following protected mixed Unity assets remain intentionally modified locally and must not be reset, restored, overwritten, or staged without a separate explicit review:

- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/Scenes/SampleScene.unity`

Their local wiring includes licensed presentation and the current Scene-level `PlayerBlock` component. They remain outside the focused code/document checkpoint.

`Assets/LocalLicensed/` is ignored and must never be committed or uploaded. It contains the P09 presentation, Katana/Sword animation assets, Animator Override mappings, and local Animation Event/import tuning. The locally verified Attack4 `FinishAttack(3)` time is therefore documented but not mirrored as an asset change.

The selected dependency closure from four locally licensed Combat VFX packages validated in `RelicGuardianAssetLab` is restored under `Assets/LocalLicensed/CombatVFX/`. After final in-project Guard Prefab composition, the local tree contains `208` files including generated metadata and uses approximately `64.28 MiB`, still far below the rejected `424.37 MB` whole-package copy. Selected Prefabs, package versions, local paths, validation evidence, and restoration steps are recorded in `Docs/COMBAT_VFX_RESOURCE_TRACKING.md`. The final local `Normal Guard Impact.prefab` and `Perfect Guard Impact.prefab` are both connected to their distinct result branches and runtime verified.

Seven learner-selected Guard WAV files from local licensed `Melee Weapons Pack 1` are imported under the ignored `Assets/LocalLicensed/CombatSFX/Selected/Guard/` boundary with original GUIDs preserved. Their accepted 3-layer Ordinary and 4-layer Perfect volume/pitch/delay settings are stored in the local `Guard_SFX_Layer_Configuration.json` and tracked in `Docs/COMBAT_SFX_RESOURCE_TRACKING.md`. They import with a clean Console but are not connected to formal presentation code yet.

## Context and Documentation Boundary

- `Docs/ARCHITECTURE.md` is the compact map of currently implemented component ownership and data flow.
- `Docs/CONTEXT_INDEX.md` routes each task to a bounded set of design and source files; it is not a source of implementation truth.
- `Docs/HANDOFF.md` contains only the current cross-conversation Handoff. Earlier Handoffs are preserved under `Docs/Archive/` and are read only through targeted historical search.
- `.agents/skills/relic-guardian-context/SKILL.md` restores this minimum context after a new task, compaction, or Handoff without loading the complete development history.

## Implemented Player Action Architecture

- `PlayerActionController` is the sole coarse gameplay-state owner. Current states are `Free`, `Attacking`, and `Blocking`.
- `ResolveActionRequests()` is the unique Block / Attack / Jump request-arbitration point. An idempotent `Time.frameCount` gate makes all callers observe one result per frame without relying on `MonoBehaviour.Update()` order.
- The fixed first-version order is Block, then Attack, then Jump. Accepted grounded Block wins same-frame conflicts and suppresses horizontal movement on that frame. Rejected requests are consumed without buffering.
- `PlayerInputReader` only records input. Block uses persistent `IsBlockHeld` plus the one-use `ConsumeBlock()` press edge, with a `Pass Through` right-mouse-button Input Action.
- `PlayerCombat` owns the four-step attack flow. Natural finish and Block cancellation share `EndAttack()`, which clears windows, queue/restart state, targets, facing, lunge, distance, and index before releasing `Attacking`.
- Animation Events carry attack-step identity; stale events are rejected when the coarse state or step index no longer matches.

## Implemented Guard Lifecycle

- `PlayerBlock` owns the internal `Startup`, `Hold`, and `Release` phases while `PlayerActionController` owns the single coarse `Blocking` state.
- Grounded Block enters from `Free`. It can also cancel the current four-step Basic Attack before entering `Blocking`.
- `BeginBlock()` starts Startup. `StartupDecisionPoint()` enters Hold if Block is still held or Release if it was released. Releasing during Hold enters Release. `FinishRelease()` returns to `Free`.
- `PlayerAnimator` presents `Block_Start`, Hold, and `Block_End` using code-driven `CrossFadeInFixedTime()`; `PlayBlockHold()` selects unlocked `Guard_Free_Locomotion` or locked `Guard_Locked_Locomotion` from `PlayerTargeting.IsLockedOn`. During an active Hold, two presentation-only bools detect a Lock-On mode change and refresh exactly once. There are no Animator-authored gameplay permission decisions.
- The local copied Clips are `Block_Start.anim`, `Block_Loop.anim`, and `Block_End_NoRootTurn.anim`; the Animator state remains named `Block_End`. Their facing correction was runtime-accepted after matching the Guard root-rotation offsets at `-66`.
- Local Animation Events are `StartupDecisionPoint` at `0.4s` in `Block_Start` and `FinishRelease` at `0.75s` in `Block_End_NoRootTurn`.
- The current Scene tuning is Guard crossfade `0.03s`, normal Block End -> Locomotion exit `0.45s`, and soft-recovery interruption crossfade `0.05s`.
- Apply Root Motion remains disabled. Guard Startup and Release remain stationary, while held Hold now reuses ordinary movement and facing through a phase-aware permission.
- `PlayerBlock.AllowsMovement` exposes only the internal Hold permission and also requires the Block input to remain held, so release closes movement without depending on `MonoBehaviour.Update()` order.
- `PlayerActionController` keeps `CanMove` and `CanSprint` separate. Guard Hold can move at normal speed, while Sprint and its locked-mode `CancelLockOn()` path remain available only in `Free`.

## Implemented Soft Recovery

- Soft recovery is a presentation lifetime, not a new `PlayerActionState`: it begins after a gameplay `Finish...` Event has returned the action to `Free` while the authored Clip still has a visual tail.
- With no accepted follow-up, the authored tail continues naturally. Movement, a new Basic Attack, or Block can interrupt that visual tail without reopening old damage, lunge, or targeting state.
- `PlayerAnimator` records both whether soft recovery is active and whether its transition has actually started, preventing the flag from clearing during the Animator's one-frame transition-reporting delay.
- Normal Guard exit retains the accepted `0.45s` visual blend. Movement/action interruption uses the separate short `softRecoveryInterruptCrossFadeDuration`.
- Attack uses its existing `FinishAttack` -> visual clip end interval as soft recovery. No separate Attack exit-duration field or general request queue was added.
- The local licensed Attack4 Clip `Attack_3Combo_3_Inplace` now uses `FinishAttack(3)` at normalized time `0.59016937`; its `OpenHitWindow(3)` and `CloseHitWindow(3)` remain at `0.31615335` and `0.39201885`.
- The retained `debugBodyYawOffset` audit reads `Animator.bodyRotation` only inside `OnAnimatorIK()`, avoiding the Unity 6 warning caused by reading it in `Update()`. It is intentionally test support, not gameplay permission logic.

## Implemented Incoming-Hit Delivery Seam

- `HitContext` is an immutable three-field value containing `DamageAmount`, `Source`, and a normalized world-space `IncomingDirection` snapshot.
- `PlayerHitReceiver.ReceiveHit(HitContext)` is the single player entry for defendable incoming hits. It first calls the idempotent `PlayerActionController.ResolveActionRequests()`, delegates `Blocking` hits to `PlayerBlock`, and forwards only unhandled hits to `PlayerHealth`.
- `EnemyAI.attackTarget`, `EnemyAttack.currentAttackTarget`, and the attack method parameters now use `PlayerHitReceiver`. At the scheduled Hit Window, `EnemyAttack` constructs the context from its damage, enemy Transform source, and enemy-to-player direction, then delivers it once.
- `SampleScene` has a Scene-instance `PlayerHitReceiver` on the player root, and `EnemyAI.attackTarget` is wired to it. This component was deliberately not applied to the protected player Prefab.

## Implemented Guard Coverage

- `PlayerBlock.ResolveGuardHit(HitContext)` treats only Startup and Hold as defendable; Release returns `GuardResult.Unhandled` before angle evaluation.
- The method projects the fixed `IncomingDirection` snapshot onto the horizontal plane, rejects a zero horizontal direction, inverts it to obtain the direction toward the attack, snapshots the player's current horizontal forward direction, and compares the unsigned pre-turn angle with `guardCoverageHalfAngle`.
- `guardCoverageHalfAngle` is an adjustable serialized `float` on `PlayerBlock`; `SampleScene` explicitly stores the accepted default `90`, giving a total `180`-degree coverage cone. Coverage includes the boundary through `<=`.
- Coverage success returns handled before health damage. Non-Blocking hits, Release, invalid horizontal direction, and coverage failure continue through `PlayerHealth`.

## Implemented Perfect Guard Classification and Presentation Boundary

- `BeginBlock()` opens one minimal Perfect Guard Window inside Startup. The local `Block_Start.anim` closes it through `ClosePerfectGuardWindow` at `0.16666667s`; entering Hold or Release also closes it defensively.
- After phase, direction, and Guard Coverage succeed, `PlayerBlock.ResolveGuardHit()` returns the explicit `GuardResult.Perfect` or `GuardResult.Ordinary`; phase, invalid-direction, and coverage failures return `GuardResult.Unhandled`.
- `PlayerHitReceiver` keeps damage routing authoritative: `Unhandled` continues to `PlayerHealth`, while handled results are sent once to the same-GameObject `PlayerGuardPresentation` before damage returns.
- `PlayerGuardPresentation.PresentGuardResult()` is the minimum presentation consumer. It distinguishes Ordinary and Perfect, owns their independent VFX lifecycles and audio-data selection, and never decides damage or Guard legality.
- The learner runtime-verified unblocked damage, Ordinary Guard, and Perfect Guard after the route moved out of `PlayerBlock`; handled hits still prevented damage, each result logged once from Presentation, and the Console remained clean.

## Implemented Ordinary Guard VFX

- `SampleScene` has one local `GuardImpactAnchor` child on the player root at local position `(0, 1.2, 0.45)`, referenced by the Scene-level `PlayerGuardPresentation`. It is a configurable visual fallback, not a physical contact point, and remains inside the protected Scene override.
- `ordinaryGuardImpactPrefab` references the ignored local final `Assets/LocalLicensed/CombatVFX/Selected/GuardImpacts/Normal Guard Impact.prefab`; its learner-accepted root scale is `0.5`.
- Ordinary Guard calls one private `PlayOrdinaryGuardImpact()`, spawns the Prefab at the anchor's world position/rotation, and destroys the runtime instance after the serialized `1.2s` lifetime. Perfect Guard and unhandled hits do not spawn the Ordinary effect.
- The selected Normal and Perfect Prefabs use View-aligned Particle Renderers, so the current visible planes face the camera. `IncomingDirection` is carried to Presentation but is not used for rotation in this first layer.
- The learner runtime-verified the new Normal Guard Impact in the real combat camera on 2026-09-01: Ordinary spawned once with accepted placement/size/brightness, cleanup completed, Perfect and unhandled hits did not spawn it, existing damage prevention/damage remained correct, and the Console was clean.
- Hitstop, Camera Impulse, pooling, Attack feedback, enemy reaction, Parry, Counter, and Guard Break remain unimplemented.

## Implemented Perfect Guard VFX

- `perfectGuardImpactPrefab` references the ignored local final `Assets/LocalLicensed/CombatVFX/Selected/GuardImpacts/Perfect Guard Impact.prefab`; its main named root uses the learner-accepted scale `0.68`.
- Perfect Guard calls the independently reconstructed `PlayPerfectGuardImpact()`, reuses the configured Guard impact anchor, and explicitly cleans the spawned instance with the code default `1.8s` lifetime. Ordinary and unhandled hits do not call the Perfect route.
- The learner runtime-verified distinct Ordinary and Perfect effects, branch isolation, cleanup, preserved damage behavior, and a clean Console on 2026-09-01. The persisted `ClosePerfectGuardWindow` Event remains `0.16666667s`; Codex did not change it.
- Both current Guard Prefabs are View-aligned. `IncomingDirection` remains carried to Presentation but unused by these camera-facing Particle Renderers.

## Implemented Guard SFX

- `CombatAudioLayer` is serializable per-layer data for one `AudioClip`, Volume, Pitch, and Delay Seconds. `CombatAudioData` groups a Master Volume with a variable-length layer array; neither type plays audio or decides combat results.
- `CombatAudioPlayer` owns four Scene-wired `AudioSource` channels. `Play(CombatAudioData)` stops prior scheduled playback, maps each valid layer onto the corresponding channel, and calls `AudioSource.PlayScheduled()` from one `AudioSettings.dspTime + 0.02s` base. `OnDisable()` invokes the same cleanup boundary.
- `PlayerGuardPresentation` owns independent Ordinary and Perfect `CombatAudioData` fields and passes only the already-selected result's data to the reusable player. Ordinary stores three layers; Perfect stores four, including the accepted fourth-layer `0.030s` accent.
- `SampleScene` keeps one local `CombatAudio` child under the player with four 2D, non-looping, Play-On-Awake-disabled channels. Licensed clips remain under ignored `Assets/LocalLicensed/CombatSFX/Selected/Guard/` and are not assigned directly to the channels.
- On 2026-09-02 the learner runtime-verified distinct Ordinary and Perfect Guard SFX, correct VFX/SFX branch isolation, no duplicate group per hit, preserved no-damage Guard handling, preserved one-hit unblocked damage with no Guard feedback, and a clean Console. The disable cleanup exists in code but was not recorded as a separate focused runtime test.

## Implemented Pre-Hit Attack Threat and Guard Facing Assist

- `AttackThreatContext` is a small immutable value containing `Source`, a normalized fixed `IncomingDirection`, and absolute `ExpectedImpactTime` on the `Time.time` clock. It carries no damage and is distinct from the hit-time `HitContext`.
- `EnemyAttack.TryStartAttack()` sends one threat at Startup using `Time.time + startupDuration`; `OpenHitWindow()` removes that threat before delivering the existing real `HitContext`.
- `PlayerHitReceiver` stores active threats by source, ignores expired entries when selecting, and returns the valid threat with the earliest expected impact. The two entry orders are both covered: Block may query an already-stored threat, or a newly received threat may notify an already-Blocking player.
- `PlayerBlock` starts assist only during Startup/Hold, inside both the `60`-degree assist half-angle and the `90`-degree coverage half-angle. It stores one fixed direction, the source, the absolute impact-time end boundary, and the player's horizontal facing before assist.
- The matching real hit resolves Guard Coverage from the saved pre-assist facing, then clears the assist. Entering Release and beginning a new Block also clear it. `PlayerMovement` remains the sole Transform-facing owner through the branch Assist -> Locked -> Free Movement.
- Current assist rotation still uses the existing `FaceDirection()` / `rotationSpeed`; exact arrival at `ExpectedImpactTime` is deferred tuning, not verified behavior. No enemy search, Lock-On target reuse, source-following rotation, or general threat framework was added.

## Latest Runtime Verification

The learner reported the final combined behavior as normal on 2026-08-28:

- Block Startup / Hold / Release and early Startup release complete correctly.
- Block cancels Basic Attack; cancelled stale attack Events do not restore or corrupt attack state.
- Same-frame Block / Attack / Jump results match the six accepted deterministic rules.
- Guard Start no longer visibly turns away and back after local Clip rotation correction.
- Guard End -> Idle/locked locomotion is visually acceptable at the `0.45s` normal exit blend.
- Movement and new Attack interrupt soft recovery; the earlier Guard-exit turn-before-attack symptom is gone.
- Single attacks, the full four-step combo, Attack4 ending, movement/restart, Block cancellation, locked/unlocked return, and Console checks passed.
- Attack4's earlier `FinishAttack(3)` creates a usable soft tail without observed damage or input regressions.
- Phase-aware Guard movement passed the learner's focused Play Mode checks: Startup/Release stay stationary, unlocked Hold keeps camera-relative movement and movement-facing, locked Hold keeps directional movement and target-facing, and Block-held Sprint neither accelerates nor cancels Lock-On.
- Free Sprint, locked Sprint cancellation, Attack movement blocking, and the Unity Console also passed the focused regression.
- Unlocked Guard Hold Idle/Forward presentation passed the learner's Play Mode check: stationary Hold uses Guard Idle, movement uses the forward Guard Walk while code-owned Transform turning remains active, Release returns normally, held Sprint stays rejected, and the Console remains clean.
- Locked Guard Hold 8-Way and the combined presentation regression passed the learner's Play Mode check: all eight directions, target-facing, Release, Sprint rejection, Lock-On retention, unlocked fallback behavior, and the Console were reported normal. The Forward Guard Walk angle was tuned locally to `-36` and accepted as basically correct on P09.
- A scoped unlocked-Hold Turn presentation experiment was removed after P09 runtime showed fixed-angle one-shot Clips conflict with the current interruptible smooth Transform turning. The learner's post-cleanup regression passed unlocked smooth turning, locked eight-way movement, Release, Sprint rejection, and a clean Console.
- Hold mode-change refresh passed the learner's Play Mode checks: unlocked -> locked -> unlocked switched the correct Blend Tree during one continuous Hold, stable modes did not repeatedly CrossFade, Startup and Release were not skipped, Sprint remained rejected, and the Console stayed clean.
- The minimum incoming-hit seam passed the learner's focused Play Mode checks on 2026-08-29: an ordinary hit changed player health from `3 -> 2` exactly once, a hit during `Blocking` also changed `3 -> 2` exactly once because Guard prevention is intentionally not connected yet, and the Console remained clean.
- Guard Coverage passed the learner's focused Play Mode checks on 2026-08-30: Free frontal hits damaged, frontal Startup and Hold hits were handled without damage, rear Hold hits damaged, frontal Release hits damaged, and the Console remained clean.
- Pre-hit Attack Threat Facing Assist passed the learner's focused Play Mode checks on 2026-08-30: with the attacker approximately `40-50` degrees off the player's forward direction, `Block_Start` and turning began together after a real Startup preview, turning continued into Hold before impact, impact did not restart `Block_Start`, the hit was guarded without health loss, and the Console remained clean. A follow-up confirmed that Block without a real preview does not auto-turn and that entering Release before impact stops assist; Release visibility used a temporary Play Mode-only `startupDuration = 2` that reverted on exit. Exact mathematical arrival at impact was not claimed.
- The minimal Startup Perfect Guard Window and ordinary-versus-perfect classification passed the learner's focused runtime checks on 2026-08-31. The authored close Event is persisted at `0.16666667s`; ordinary Guard and Perfect Guard were both observed, handled hits caused no health loss, and the Console remained clean.
- The minimum `GuardResult -> PlayerGuardPresentation` boundary passed the learner's focused runtime checks on 2026-08-31. Unblocked hits still damaged, Ordinary and Perfect results each reached only their presentation log once without health loss, and the Console remained clean. `Assembly-CSharp.csproj` compiled with zero errors and zero warnings before the Play Mode pass.
- The first Ordinary Guard VFX layer passed the learner's focused Play Mode checks on 2026-09-01 using the final `Normal Guard Impact.prefab`: one Ordinary spawn, accepted in-camera presentation, explicit cleanup, no Ordinary effect for Perfect or unhandled hits, preserved damage behavior, and a clean Console.
- The Perfect Guard VFX layer passed the learner's focused Play Mode checks on 2026-09-01 using the final `Perfect Guard Impact.prefab`: one Perfect spawn, distinct presentation, explicit cleanup, no branch crossover, preserved damage behavior, and a clean Console.
- The Guard SFX layer passed the learner's focused Play Mode checks on 2026-09-02: the accepted 3-layer Ordinary and 4-layer Perfect combinations stayed distinct, each hit produced one matching VFX/SFX group without crossover or duplicate playback, handled hits still prevented damage, an unblocked hit damaged once without Guard feedback, and the Console remained clean.
- The selected dependency closure from four Combat VFX packages and the final Guard validation scene were restored into the main project's ignored `Assets/LocalLicensed/CombatVFX/` boundary on 2026-08-31. Main-project import completed with zero Console errors and zero warnings; scene validation found no missing scripts or broken Prefabs. Final in-combat visual tuning and Gameplay connection remain unverified.

The current workspace source compiled through `Assembly-CSharp.csproj` with zero errors and zero warnings after the Guard SFX connection. Runtime results above are learner-reported Play Mode verification.

## Deferred Guard Work

- The earlier target-search and post-hit assist plans are superseded. Empty Guard performs no Physics or Lock-On target search. A real `AttackThreatContext` emitted by enemy Startup may start one fixed-direction pre-hit assist while Blocking.
- `HitContext`, `PlayerHitReceiver`, directional Startup/Hold Guard Coverage, the core pre-hit threat/assist route, the minimal Startup Perfect Guard classification, the explicit `GuardResult -> PlayerGuardPresentation` route, and distinct Ordinary/Perfect Guard VFX and SFX are implemented. Hitstop and later presentation layers, Guard Break, Parry, and Counter remain unimplemented. The current design is recorded in `Docs/GUARD_HIT_RESOLUTION_DESIGN.md` and `Docs/COMBAT_PRESENTATION_FEEDBACK_DESIGN.md`.
- Startup and Hold are defendable; Release is not. The Perfect Guard Window is a short authored subset of Startup, while other legal Startup/Hold hits resolve as ordinary Guard.
- Default adjustable half-angles are `60` degrees for Facing Assist (total `120`) and `90` degrees for Guard Coverage (total `180`). Assist eligibility uses pre-turn facing, and the matching real hit reuses that saved facing for coverage.
- `PlayerMovement` remains the only facing owner. Its implemented branch is active Guard Facing Assist, otherwise Locked Facing, otherwise Free Movement Facing. Assist stores a fixed preview direction until expected impact and never follows `Source`.
- Unlocked Guard Hold presentation uses a `Speed`-driven 1D `Guard_Free_Locomotion` Blend Tree with Guard Idle at `0` and Guard Forward at `3`. Locked Hold uses `Guard_Locked_Locomotion`, a 2D Simple Directional Blend Tree with Guard Idle plus eight directions driven by the existing `MoveX` / `MoveZ`.
- `PlayerAnimator` now tracks whether Hold presentation is active and which Lock-On variant it last selected. It calls `PlayBlockHold()` again only when the authoritative Lock-On mode changes during that active presentation.
- The locally accepted Forward Guard Walk uses `Orientation Offset Y = -36`; the other seven directional Walk Clips retain their current local settings pending individual Turn/direction audits.
- `Turn_Block_90_L/R` and `Turn_Block_180_L/R` remain copied under ignored `Assets/LocalLicensed/` with Loop Time disabled. Audit confirmed authored RootQ rotation of approximately `+/-87` and `+/-180` degrees. A minimum presentation lifecycle was tested, then fully removed from the scripts and Animator Controller because short or changing movement input does not commit the code-owned Transform to completing a fixed-angle turn. Reconsider these Clips only with a deliberately different turn-in-place/input contract.
- Dodge remains after the next Guard gameplay concept. Do not add a numeric Priority system, general Request Queue, large Coordinator, Ability Framework, hierarchical FSM, or pre-emptive `PlayerMotor`.

## Exact Next Development Step

Design and implement only Guard Hitstop as the next learner-led presentation concept. First define its presentation owner, start/end and guaranteed restoration boundaries, Ordinary/Perfect durations, and overlap rule. It must consume the already-resolved `GuardResult`, must not change `PlayerBlock`, the Perfect Guard Window, damage routing, or DSP audio scheduling, and must not add Camera Impulse, Attack feedback, Weapon Trail, Hit Reaction, pooling, enemy reaction, Parry/Counter, Guard Break, Dodge, or a general time/effect framework in the same concept.

## Files to Read Next

- `AGENTS.md`
- `Docs/CURRENT_STATE.md`
- `Docs/ARCHITECTURE.md`
- `Docs/HANDOFF.md`
- `Docs/CONTEXT_INDEX.md`
- `Docs/COMBAT_PRESENTATION_FEEDBACK_DESIGN.md`
- `Docs/COMBAT_SFX_RESOURCE_TRACKING.md`
- `Docs/GUARD_HIT_RESOLUTION_DESIGN.md`
- `Docs/COMBAT_VFX_RESOURCE_TRACKING.md`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyAI.cs`
- `Assets/RelicGuardian/Enemy/Scripts/EnemyAttack.cs`
- `Assets/RelicGuardian/Player/Scripts/AttackThreatContext.cs`
- `Assets/RelicGuardian/Player/Scripts/HitContext.cs`
- `Assets/RelicGuardian/Player/Scripts/GuardResult.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHitReceiver.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerBlock.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerGuardPresentation.cs`
- `Assets/RelicGuardian/Player/Scripts/CombatAudioLayer.cs`
- `Assets/RelicGuardian/Player/Scripts/CombatAudioData.cs`
- `Assets/RelicGuardian/Player/Scripts/CombatAudioPlayer.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerMovement.cs`

Inspect the protected Prefab and Scene only when current local wiring or runtime values are necessary. Never overwrite them from a tracked baseline.
