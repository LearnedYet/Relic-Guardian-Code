# Relic Guardian Current State

Documentation updated: 2026-09-09. This is the sole maintained active Exact Next Step record. Actual code, saved Unity assets, current Editor state and Git status remain authoritative.

## Project Environment

- Project: C:\Unity\Project\My project; Windows target.
- Unity 6000.3.19f1; URP/VFX Graph 17.3.0; Input System 1.19.0; Cinemachine 3.1.7.
- Player/enemy displacement uses CharacterController. Apply Root Motion remains off.
- Key gameplay and presentation code remains learner-authored; review actual saved files after each edit.
- Scene, Prefab, Inspector, Animator and Animation Event configuration also defaults to the learner; Codex changes Editor configuration only after an explicit takeover request for that scope.

## Completed Feature Checkpoints

- Four-step player Basic Attack: indexed Events, target confirmation, startup lunge, Combo/Restart windows, shared end/cancel cleanup and soft visual recovery.
- Free/locked locomotion, Sprint and orthogonal Lock-On/camera mode. PlayerActionController owns coarse action state and deterministic same-frame Block/Attack/Jump arbitration.
- Guard Startup/Hold/Release, Coverage, pre-hit Facing Assist, Perfect classification and explicit GuardResult routing.
- Distinct Ordinary/Perfect Guard VFX and layered SFX; Perfect-only shared Hitstop with normal and disable restoration.
- Ordinary Guard movement lock and separate player reaction layer. Hold release waits for the lock; Perfect does not request this reaction.
- Attack1-4 transient Trail and Whoosh; Attack4 uses separate Windup/main-swing Events. Persistent WeaponAura is independent. Attack and Guard audio use separate playback instances.
- Confirmed Player Attack hits now route through HitContext -> EnemyHitReceiver -> EnemyHealth. EnemyHitPresentation independently spawns the selected Blood VFX and a temporary two-channel Hit Audio player so feedback is not owned by the attacker's motion path or the victim's active lifetime.
- EnemyStateController now owns coarse `Chase / Attacking / Staggered`, attack admission/natural finish, ordinary surviving-hit reaction timing and the post-reaction reaction-cooldown deadline. EnemyAttack retains its internal phase lifecycle and owns idempotent execution cleanup; health depletion still uses the prototype GameObject disable behavior.
- Enemy attack Startup admission now requires both range and a maximum horizontal facing angle. When only range passes, EnemyAI stops translation and asks EnemyMovement to turn in place; EnemyMovement remains the sole enemy Transform-rotation/displacement writer.
- Global Attack Cooldown now starts once at effective EnemyAttack cleanup, is owned as a scaled deadline by EnemyStateController, and gates later attack admission while continuing through HitReaction. Repeated ordinary hits during Staggered can restart GetHit presentation without extending the gameplay deadline; Attacking still rejects the visual/gameplay reaction.

Component ownership and implemented call chains live in ARCHITECTURE.md. Exact resources and tuning are referenced below.

## Verification and Known Limits

- Prior learner Play Mode acceptance: player control/combo/Guard regressions (2026-08-28 through 09-02), Ordinary reaction and four-step Trail (09-03), motion Whoosh and Attack4 cancellation between sound Events (09-04). Existing logs contain detailed test sequences and historical values.
- The learner runtime-verified the minimum Enemy receiving chain and current confirmed-hit VFX/SFX on both prototype targets on 2026-09-08; existing damage and inactive-at-zero behavior remained normal. The final Unity Console check contained zero errors and zero warnings.
- The learner runtime-verified `NearTarget`'s `Chase -> Attacking -> Chase` natural loop plus EnemyAttack cancellation during Startup, HitWindow and Recovery and a repeated disable/cancel call on 2026-09-08. No delayed extra damage, stale threat-facing, stuck telegraph or Console issue was observed.
- Hitstop normal/disable recovery was tested; overlapping Hitstop requests are code-reviewed but not independently runtime-tested. The audio player's disable cleanup was not recorded as its own focused runtime test.
- Exact Facing Assist arrival at ExpectedImpactTime remains unverified; its current rotation uses the ordinary facing function.
- On 2026-09-09 the learner runtime-verified ordinary surviving-hit reaction, immediate repeated GetHit presentation at saved Animator Speed `1.4`, fixed non-extension of the gameplay deadline, ordinary-hit rejection while Attacking, natural movement recovery, post-reaction protection, Global Attack Cooldown and counterattack admission. The same session verified range-plus-`15°` start admission: a facing-away in-range enemy stopped translation, turned in place, and started only after reaching the angle threshold. Idle-to-Run sliding after leaving attack range was traced to Exit Time, runtime-checked with it disabled, and the controller now saves `Has Exit Time = false`; Console remained clean.
- Enemy hits remain scheduled against the saved target without impact-time range/direction validation. A retained-corpse terminal Death state and active-object death interruption are not yet implemented. FarTarget remains a non-AI hit-test target and is intentionally outside the state system.
- During Global Attack Cooldown the current baseline remains in Chase and pursues at full speed when outside attack range. Lower pursuit pressure, Wait/Strafe/Retreat/Approach choices and deliberate decision pacing remain deferred to the spacing/movement stage.
- Guard delay is currently enforced in Hold Update. StartupDecisionPoint can directly enter Release without testing the lock deadline; late-Startup Ordinary-hit/release behavior needs a focused check before claiming an all-phase guarantee.
- Indexed player Events reject different-step or non-Attacking callbacks, but do not identify individual same-index executions.
- Saved tuning audited on 2026-09-06 differs from earlier accepted snapshots: Guard perfect-window close timing, Attack1 Trail opening, and Perfect audio mix. Current values and dated old values are recorded in the dedicated documents; this static audit does not certify the revised tuning at runtime.

## Resource and Configuration References

- COMBAT_VFX_RESOURCE_TRACKING.md: connected WeaponAura/AttackTrail roles, Trail Event table, Guard anchors/lifetimes, selected resources, historical import palettes and restoration.
- COMBAT_SFX_RESOURCE_TRACKING.md: current saved Guard/Attack mixes, Whoosh/Windup Events and the connected confirmed-hit two-layer cue.
- GUARD_REACTION_DESIGN.md: Guard Clip/state timing, movement lock, reaction and shared soft-recovery tuning, with current-versus-historical provenance.
- ENEMY_COMBAT_AGENT_DESIGN.md: approved future Enemy Combat Agent direction; none of its proposed components/states is an implemented fact merely because it is documented.
- ENEMY_COMBAT_RESOURCE_TRACKING.md: selected SwordShield animation inventory, GUIDs and main-project import/validation status. The nine copied files resolve as Generic Clips using the valid shared SK_GoblinAvatar; Loop/Root settings and visual previews still require a focused decision before Animator integration.
- Connected confirmed-hit resources are FX_hit_03_Blood and a trimmed sword-impact/flesh SFX pair. The derived local WAV and all licensed source content remain ignored.

## Exact Next Step

Begin the retained-corpse terminal Death slice. Replace the prototype immediate deactivation only after defining Dead ownership, lethal-hit ordering, attack/threat/movement cleanup, Death presentation, and target/hit exclusion. Preserve the independently spawned lethal hit VFX/SFX lifetime and keep future Perfect Guard Stagger, multi-attack selection, hit-time validation, Attack Hitstop, camera feedback and Strong Combo separate.

## Git and Protected Local State

- Local confirmed-hit receiving/feedback feature checkpoint: be75058. The preceding Guard reaction/attack-motion documentation follow-up remains 9c225c8.
- GitHub confirmed-hit receiving/feedback feature mirror: 7e6e18b. The 2026-09-08 push was verified with `git ls-remote` at `7e6e18b547a02b33ba84f8c01c32f867a9e2accd`.
- Local Enemy reaction/attack-pacing feature checkpoint: `9da9b25 Add enemy reactions and attack pacing`.
- GitHub code/document mirror checkpoint: `f1fe243 Sync enemy reactions and attack pacing`; the 2026-09-09 push was verified with `git ls-remote` at `f1fe243404f9b00d470d4065f2ca5407ab3c7e8a`.
- Full Unity history and flattened GitHub code/document mirror remain separate. Do not merge their main branches. Durable Git and licensed-asset rules remain in AGENTS.md.
- Protected pre-existing local changes remain in RelicGuardianPlayer.controller, RelicGuardianPlayer.prefab and PlayerHealth.cs. SampleScene.unity contains both protected earlier local work and the newly accepted Enemy receiving/VFX/SFX configuration; preserve it as a mixed dirty file and never broadly replace or stage it.
- PlayerCombat.cs, EnemyHitReceiver.cs/meta, EnemyHitPresentation.cs/meta and the project-owned EnemyHitAudioPlayer Prefab/folder metadata are included in local checkpoint be75058. The flattened GitHub mirror includes the project-owned C# and maintained documentation; Scene wiring and the Prefab remain outside that mirror boundary.
- ENEMY_COMBAT_AGENT_DESIGN.md, ENEMY_COMBAT_RESOURCE_TRACKING.md and the related maintained planning/context updates are included in the same checkpoint synchronization.
- EnemyAI, EnemyAnimator, EnemyAttack, EnemyHealth, EnemyHitReceiver, EnemyMovement, EnemyState/meta, EnemyStateController/meta, maintained documentation and the prior Handoff archive are included in local checkpoint `9da9b25`. The flattened GitHub mirror includes the eight C# files without Unity `.meta` plus the maintained documents at `f1fe243`.
- NearTarget's state/reaction wiring and current Inspector timing values remain saved inside the protected mixed SampleScene.unity and outside the focused code/document commits.
- Assets/LocalLicensed/ and its .meta boundary remain ignored and never uploaded. Licensed models, Clips/import Events, VFX/SFX and local wiring remain outside the focused code/document checkpoint.

## Context Recovery and History

Follow AGENTS.md and the relic-guardian-context skill's existing startup sequence, then select only the task route from CONTEXT_INDEX.md. HANDOFF.md supplies recent handoff context and points back here for the active step.

DEV_LOG.md and Docs/Archive/ retain chronological evidence and superseded experiments. Search them by topic/date and read bounded sections only. Historical success of an earlier configuration does not prove a later saved configuration was tested.
