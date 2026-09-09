# Enemy Combat Agent Design

Status: approved direction, consolidated 2026-09-05; receiving/feedback, minimum state/cancel, ordinary HitReaction/protection, Global Attack Cooldown and start-time range/facing admission are runtime-verified through 2026-09-09. Remaining stages are proposed until actual code and runtime evidence confirm them.

## Authority and Immediate Scope

Actual code/assets and Git state, then CURRENT_STATE.md and ARCHITECTURE.md, remain implementation authority. This document supersedes earlier Enemy direction and conflicting future-order notes in Guard/presentation plans. HANDOFF.md remains the recent checkpoint and exact next-step handoff.

The immediate feature is now **retained-corpse terminal Death**. Receiving/confirmed feedback, the coarse state/cancel boundary, ordinary HitReaction/protection, Global Attack Cooldown and start-time range/facing admission are completed checkpoints. Repeated hits during Staggered restart GetHit presentation without extending the gameplay deadline; a ready attack can begin after reaction under state/range/facing/cooldown admission and ordinary light hits cannot interrupt Attacking. Current saved timing is HitReactionDuration `0.3s`, HitReactionCooldown `0.2s`, and Global Attack Cooldown `2s`. Attack Hitstop, Perfect Guard Stagger, multi-attack selection and Strong Combo remain later work. The learner remains author of key gameplay and presentation code; explain identifiers, responsibility, lifetime and call chains before each actual edit, then inspect the saved file.

Verified baseline from current source inspection:

- PlayerCombat.OpenHitWindow(int) validates the current attack step and confirms the saved target is in range, then submits HitContext through EnemyHitReceiver to EnemyHealth and independent EnemyHitPresentation.
- HitContext already carries DamageAmount, Source and normalized IncomingDirection. PlayerHitReceiver.ReceiveHit(HitContext) currently returns void; GuardResult remains an internal Guard classification.
- EnemyAI chooses chase/attack using range, a `15°` horizontal facing threshold and EnemyStateController `Chase / Attacking / Staggered` authority. In-range invalid facing stops translation and turns through EnemyMovement without requesting attack. EnemyAttackPhase remains internal to the executor.
- EnemyAttack owns Ready/Startup/HitWindow/Recovery, a saved player receiver, timed damage and Startup threats. Natural finish and cancellation share idempotent cleanup, including OnDisable; impact still does not revalidate target range/direction.
- EnemyMovement owns CharacterController displacement and Transform facing; Turn(direction) rotates only and Move(direction) reuses it before displacement.
- EnemyHealth currently disables its GameObject at zero health. PlayerTargeting drops inactive targets but has no retained-corpse eligibility rule.
- Confirmed Enemy hit Blood VFX and independent two-layer SFX are connected and runtime-verified. GetHit is connected through EnemyAnimator at saved state Speed `1.4`; ordinary surviving-hit entry, non-extension, Attacking exclusion, post-reaction cooldown and movement recovery are runtime-verified. Death remains unconnected.

## Ordered Development Stages

Stages 1-5 are completed checkpoints; later entries remain future work. Complete one concept and its runtime checks before continuing.

1. Minimum EnemyHitReceiver, then confirmed Hit VFX, then Hit SFX. Preserve the existing Player Attack confirmation and damage semantics.
2. Minimum Enemy coarse state authority plus reliable EnemyAttack.Cancel() and natural-finish coordination. Introduce states only as their consumers become real.
3. Ordinary HitReaction and post-reaction HitReactionCooldown.
4. Small start-time Attack Admission repair: require both distance and facing; rotate in place without attacking when only distance passes.
5. Minimum Global Attack Cooldown on natural finish/cancel, counting independently through HitReaction and checked by attack admission before multi-attack selection.
6. Death with retained corpse and target/hit exclusion.
7. Minimal incoming HitResult return; Perfect Guard cancels EnemyAttack and causes stronger Enemy Stagger.
8. Ordinary multi-attack selection using the established Global Attack Cooldown.
9. Enemy hit-time distance/direction/target validation, establishing an actual Miss path independently of start-time admission.
10. Strong Attack / Strong Combo as an independent feature. Required preceding sub-stages: functional Player HitStun, Dodge with explicit defensive rules, and a compatible player death/invalid-target exit. Then add PerfectOnly, telegraph, first-hit commit and multi-step execution.
11. Combat spacing, independent move/facing directions, Strafe/Retreat/Approach/Wait and distance/cooldown-based decisions.
12. Integrated first SwordShield Goblin Combat Agent acceptance.

Attack Hitstop remains a separate optional tuning checkpoint after confirmed feedback is stable; it is not silently included in stage 1. A guaranteed three-hit capture is a further pairing/position-correction sub-stage within Strong Combo, not an automatic consequence of stage 9 commit.

## Receiving and Confirmed Feedback

Planned call chain:

```text
PlayerCombat: existing target/step/hit confirmation
  -> EnemyHitReceiver.ReceiveHit(HitContext)
     -> EnemyHealth: apply health change once
     -> EnemyHitPresentation: express the accepted hit once
        -> Hit VFX
        -> independent Hit SFX playback
        -> optional shared Hitstop request at a later checkpoint
     -> future Enemy state consequence, when that stage is implemented
```

PlayerCombat retains target selection, attack lifetime and hit confirmation. It constructs a hit-time snapshot and submits a request; it does not instantiate victim VFX or own victim audio and reaction rules. EnemyHitReceiver is the enemy receiving entry and coordinates health/consequence/presentation without doing attacker target search. EnemyHealth remains the sole health mutation owner. EnemyHitPresentation owns effect resources, placement and cleanup, not damage or state permission.

Reuse the existing HitContext unchanged initially. IncomingDirection continues to mean source-to-victim at hit delivery, not direction toward the source. HitImpactAnchor is an adjustable enemy presentation point; snapshot its world pose for feedback. It is not a physical contact point. Add HitPoint/HitNormal only when a concrete collision/contact feature supplies valid data.

Selected resources:

- Main VFX: Assets/LocalLicensed/CombatVFX/Selected/AttackHits/Blood/FX_hit_03_Blood.prefab.
- Hit SFX: SWSH_Sword Slash Impact V1 Assorted 18_DDUMAIS_NONE.wav and GOREFlsh_Flesh And Gore Assorted 08_DDUMAIS_NONE.wav under CombatSFX/Selected/Attack/Hit.
- Reuse CombatAudioData/CombatAudioLayer/CombatAudioPlayer with a dedicated hit playback lifetime and channels. Guard and motion audio must not be stopped by hit playback.

Stage 1 must handle a lethal hit while EnemyHealth still performs SetActive(false). Capture needed pose/data while valid and let spawned feedback complete independently of the enemy object's active lifetime. An AudioSource on the disabled victim cannot provide this guarantee. Use the smallest independent temporary feedback/audio owner when connecting SFX; verify cleanup and lethal playback before accepting that slice. Do not delay damage/death to wait for a sound, add a full Death FSM early, or make missing presentation suppress damage. A minimal health outcome/query may be added only if the concrete implementation needs it; no general Health framework is required.

Initial script scope:

- Add Assets/RelicGuardian/Enemy/Scripts/EnemyHitReceiver.cs.
- Modify only the confirmed-target delivery block in PlayerCombat.OpenHitWindow(int), replacing the direct EnemyHealth call with receiver delivery while preserving current target confirmation and other windows.
- Add Assets/RelicGuardian/Enemy/Scripts/EnemyHitPresentation.cs in the next presentation sub-slice.
- Reuse HitContext and existing audio types. Initially preserve EnemyHealth's subtraction/disable behavior; any necessary minimal adaptation for lethal feedback is reviewed separately.
- Scene wiring later adds the receiver/presentation on the enemy gameplay root, HitImpactAnchor and independent playback references. This document adds no components or assets.

## Enemy State and Ability Ownership

Target coarse vocabulary: Idle, Chase, Combat, Attacking, Staggered, Dead. EnemyStateController is the single coarse owner deciding entry, exit and gameplay permissions. EnemyAI supplies decisions; execution remains in components. Only Chase and Attacking exist in the current minimum implementation; add later values only with real consumers.

EnemyAttackPhase remains Ready, Startup, HitWindow, Recovery. While the coarse state is Attacking, EnemyAttack advances the attack's internal phases. Ready means the attack executor is idle, not that AI must attack next frame. Cancellation may reset the executor to Ready while the coarse owner remains Staggered or Dead; this never grants attack permission.

Combat's Approach, Retreat, Strafe and Wait are internal behaviors, not separate coarse states. Strong Combo step numbers are execution data, not Combo1State/Combo2State/Combo3State. This is a small coarse FSM plus component-owned lifetimes, without a general hierarchical-state framework.

Natural completion/cancellation must keep coarse and internal state consistent. After synchronous ReceiveHit returns, the attack executor must check that its execution is still valid: a returned Perfect Guard or a consequence may already have cancelled it. Stale timers, Events and callbacks cannot resume a cancelled attack or overwrite Dead/Staggered. Decide concrete same-frame lethal-hit/attack arbitration when implementing the state boundary; it must not depend on MonoBehaviour.Update order.

## Reliable EnemyAttack Cancellation

Cancel must be safe when repeated and reachable from Stagger, Death, disable/destruction, target invalidation and system cancellation. It must:

- Close future damage delivery and invalidate stale attack/step callbacks.
- Remove the outgoing AttackThreatContext from the saved player's receiver before dropping the target reference; clear any matching active assist contribution so no cancelled threat keeps turning the player.
- Clear target, phase timer, delayed animation-start flags and telegraph; stop pending attack animation requests or scheduled work.
- Clear future multi-attack/Strong Combo step and commit state.
- Release only control/position commitments owned by this execution on both participants, if present.
- Leave the coarse destination decision with the state owner. Cleanup must not force Ready-to-Combat/Free after Death.

Natural finish and Cancel share cleanup where appropriate, but ending semantics, cooldown bookkeeping and destination differ. Neither path can require the final animation Event to run. Verify cancel in Startup, HitWindow and Recovery, then again inside each Strong Combo step when introduced.

## Start-Time Attack Admission

Implemented on 2026-09-09: EnemyAI begins Startup only when both range and the Inspector-tunable maximum horizontal facing angle pass. If range passes but facing does not, it stops translational movement and calls EnemyMovement.Turn(direction) without requesting an attack. The learner runtime-verified facing-away turn-in-place, no early telegraph/attack, admission at the saved `15°` threshold and a clean Console.

This check answers whether Startup may begin. It does not prove that the later impact is valid. HitWindow-time target identity, active/alive status, distance and authored direction checks remain deferred to the dedicated Hit-Time Validation stage.

## Ordinary HitReaction and Cooldown

Health and accepted hit VFX/SFX apply independently of ordinary reaction eligibility. Resolve lethal damage first; Dead wins over reaction.

| Current state | Ordinary light-hit consequence |
| --- | --- |
| Idle / Chase / Combat, cooldown expired | Enter one short HitReaction, play GetHit, temporarily restrict normal action/movement |
| Idle / Chase / Combat, cooldown active | Damage and hit feedback only |
| Attacking | Damage and hit feedback; no ordinary attack interruption or GetHit override that hides the live attack |
| Staggered | Damage and hit feedback; ordinary hits do not reset, extend or shorten existing stagger |
| Dead | Reject further combat hits and reactions |

The short HitReaction may use Staggered with a short duration and an entry reason; Perfect Guard uses a stronger reason/duration. Do not add a separate coarse state merely because the animation differs. GetHit is presentation; the gameplay state owns the finish and permission boundary.

HitReactionCooldown is Inspector-tunable and begins when the short HitReaction ends, guaranteeing a subsequent action interval. It is implemented as a scaled deadline in EnemyStateController. Damage during cooldown does not restart the cooldown or suppress feedback. Ordinary hits during Staggered restart only GetHit presentation without resetting the gameplay timer, and Attacking rejects ordinary reaction. Perfect Guard will ignore ordinary HitReactionCooldown. Unexpected exit from short reaction still needs a deliberate rule when Death/forced Stagger becomes real; a surviving exit starts the cooldown, while Death never returns to reaction eligibility.

The learner's 2026-09-09 accepted baseline uses fixed HitReaction gameplay time while each hit during Staggered may restart GetHit presentation. Global Attack Cooldown counts independently during Staggered and ordinary hits do not reset it. The combined rhythm passed at saved values HitReactionDuration `0.3s`, HitReactionCooldown `0.2s`, and Global Attack Cooldown `2s`; a counterattack still requires state, range and facing to pass and is never remote or unconditional. During Global Attack Cooldown the current baseline continues full-speed Chase outside attack range; lower pursuit pressure belongs to the later spacing/decision stage.

## Perfect Guard Result and Stagger

At stage 6, add only the minimum attacker-visible result from PlayerHitReceiver. Candidate HitResult values are Damaged, OrdinaryGuard and PerfectGuard. Keep GuardResult scoped to Guard resolution. Damaged must describe an accepted damage result, not merely a method call; when Dodge/Dead/rejected-hit cases become real, represent those outcomes truthfully with the smallest necessary extension.

Miss from failed attacker geometry validation is handled by the attacker before ReceiveHit. A Dodge invulnerability check performed inside the receiver requires a distinguishable non-damaged response when Dodge is implemented; it cannot return Damaged by default.

Perfect Guard -> cancel the active EnemyAttack -> enter stronger Staggered -> later return to Combat if alive and the target remains valid. This ignores ordinary reaction cooldown. Initial tuning can start near 0.8-1.0 seconds; GetHit may be reused visually, with gameplay duration independent of clip completion. The resulting punish opportunity is not yet a Player Counter ability or automatic Counter attack.

## Health, Death and Target Eligibility

At the Death stage, health reaching zero produces Dead immediately. It stops decisions, attack damage, movement and threats, runs reliable cancellation, and selects Death presentation. Hit VFX/SFX for the lethal hit still occur once and survive long enough to complete; future Hitstop does not postpone logical Death. Ordinary GetHit cannot override Death.

Replace the prototype immediate SetActive(false) at this stage with retained corpse presentation. Dead is terminal for this first agent: no Recovery, Cooldown or animation callback returns it to Combat. Corpse lifetime/cleanup beyond retention is future work.

Remove dead actors from attack candidates and Lock-On acquisition; an existing lock must release even though the corpse remains active. Gate hit reception against Dead. A corpse's physical collision is separate from its combat target eligibility; do not assume merely retaining/disabling a visual object solves both. PlayerCombat and PlayerTargeting need the smallest shared live-target eligibility check at this stage. Player death likewise terminates any future paired Strong Combo control commitment and cannot be undone by enemy cleanup.

## Attack Selection and Cooldowns

Ordinary attacks will use Attack1/2/3 content with a small per-attack configuration as needed: MinRange, MaxRange, Cooldown, Weight and Recovery. Selection admits only valid targets, eligible coarse states and ready cooldowns. Build reusable data only as the second/third attack needs it.

- Recovery is a move's internal recovery phase.
- Global Attack Cooldown is the wait before the AI may request another attack. Set it on natural finish or cancellation, including Miss and Perfect Guard interruption. A state change cannot erase it or shorten an existing later deadline.
- Strong Attack Cooldown is a separate longer restriction, consumed on accepted Strong Attack start. Failed admission does not consume it; Miss, Perfect Guard or later cancellation does not refund it. Strong Attack is eligible only when both global and strong cooldowns permit it.

Cooldown expiry grants decision eligibility, not an immediate mandatory attack. During cooldown, the eventual Combat behavior may Approach/Retreat/Strafe/Wait; the stage-7 baseline may wait until spacing is implemented. Tune the first Goblin toward moderately aggressive: approach actively, briefly adjust/observe, attack, pause visibly, then pressure again. No separate Aggression framework is needed.

## Hit-Time Validation

Before each enemy damage delivery, validate target identity, existence, active/alive/receivable status, current distance and authored attack direction/coverage. Use the attack's actual impact-time range/facing convention, not a fresh direction pointed at a distant target that makes every hit automatically valid. A failed test causes Miss, no ReceiveHit and no confirmed-hit feedback. Preserve the existing start-time eligibility check as a separate concern.

This remains target-confirmed combat initially; it does not claim weapon collider contact. Obstacle/line-of-sight checks are added only when concrete geometry requires them. Deduplicate per-step hit delivery. Threat timing/clearing must remain aligned with actual Startup and impact, including cancellation. Strong Combo must not be implemented until its miss branch is real.

## Strong Attack and PerfectOnly

Strong Combo is one high-danger Strong Attack with a clear, distinguishable startup animation, VFX and SFX telegraph. The first strike can be avoided by functional Dodge/spacing or defended with Perfect Guard. Ordinary Guard does not successfully defend it.

Introduce the minimal per-hit GuardRule only at this stage: Normal versus PerfectOnly. Evaluate it together with existing phase/directional/perfect-window legality before committing any Ordinary Guard success side effect. PerfectOnly must never emit Ordinary success VFX/SFX or apply the Ordinary movement lock and then also damage the player. Ordinary Guard failure causes damage and the Strong hit response; defense-pierced presentation is a separate visible consequence, without a Guard Meter/Guard Break system.

## Strong Combo Commit and Player Control

Strong Combo is an independent multi-slice implementation with Player HitStun and Dodge prerequisites. PlayerActionController remains the coarse player permission authority; PlayerMovement retains displacement. Before adding HitStun, define accepted/rejected inputs, existing Attack/Block cleanup, death precedence, interruption and reliable recovery. Entering HitStun must close stale Guard/perfect/attack windows and prevent their animation callbacks from restoring old state.

Step1 result:

- Miss / successful Dodge: do not commit; finish through Recovery.
- Perfect Guard: do not commit; cancel and enter enemy Staggered.
- Ordinary Guard against PerfectOnly: defense fails; apply accepted damage and enter the same Strong hit response as Damaged.
- Damaged with a surviving valid player: commit, place the player in the combo-owned HitStun commitment, continue Step2 then Step3.
- Lethal/invalid target: terminate the sequence safely rather than capturing or unlocking a dead target.

During the surviving commitment, the player cannot normally Move, Attack, Block or Dodge. After commit, Step2/3 progression does not require each preceding strike to damage again. Each strike still performs spatial/target validation and only applies its own damage once when valid. A geometric miss does not authorize remote damage; it also does not automatically require abandoning the remaining steps. Ordinary reactions cannot interrupt the attacking enemy; death, external forced Stagger and system cancellation remain valid exits.

Commit and guaranteed three-hit damage are distinct. The baseline is a committed sequence with hit validation. If the accepted future experience requires every surviving first-hit victim to receive all three hits, add lightweight pairing and bounded position correction, shared execution identity, collision-aware distance constraints and failure release as an explicit sub-stage. Movement corrections route through the existing movement owners. Never satisfy this with remote unconditional damage, disabling collision or dragging the player through obstacles.

Both participants must release this execution's control/position commitment on normal finish, attacker death/stagger/cancel/disable/destruction, target death/invalidation/disable/destruction, scene teardown, or system cancellation. A bounded failsafe is required if the normal callback is lost. Cleanup is idempotent, does not release a different execution's lock, and cannot force a dead player to Free. No final-step Animation Event is the sole unlock mechanism.

## Spacing, Detection and Animation Resources

Planned behavior: Idle -> detect player -> Chase -> Combat. Define target loss and detection/engagement ranges when that slice arrives. Inside Combat: far -> Approach/WalkForward; farther -> Chase/Run; ideal distance -> Strafe/Wait; too close -> Retreat; legal opportunity -> Attacking. Use distinct enter/exit distance thresholds or equivalent bounded decisions to avoid oscillating at a boundary. Cooldown and distance do not bypass state permission.

At spacing implementation, separate MoveDirection from FacingDirection. Retreat moves away while facing the player; Strafe moves tangentially while facing the player. EnemyMovement keeps both actual displacement and facing execution. Normal locomotion stays non-Root-Motion with CharacterController. Special attack Root Motion may be tested later in its own explicit movement contract without adding a competing writer for normal AI motion.

The first-version selected set is `IdleSwordShield`, `RunSwordShield`, non-Root-Motion `WalkForward/WalkBackwards/StrafeLeft/StrafeRightSwordShield`, three independent non-Root-Motion `Attack1/2/3ForwardSwordShield` Clips, `GetHitSwordShield`, and `DeathSwordShield`. Strong Combo will use the three independent Forward Attacks rather than the packaged `2HitComboSwordShield` or `3HitComboSwordShield` as formal Gameplay Clips. `BlockSwordShield`, `IdleProtectedSwordShield`, `WalkNormalSwordShield`, Dagger and Slingshot families are outside the first version. Perfect Guard Stagger may initially reuse `GetHitSwordShield`; no dedicated Stagger Clip is selected. Exact test paths, GUIDs and main-project import status are maintained in `ENEMY_COMBAT_RESOURCE_TRACKING.md`.

On 2026-09-08, the nine previously missing selected files were copied into the main licensed boundary with AssetLab source GUIDs preserved. This is filesystem/GUID verification; Unity Clip, visual and Console validation remains pending. See `ENEMY_COMBAT_RESOURCE_TRACKING.md` for the exact inventory.

## Feedback Hierarchy and Deferred Features

Ordinary Attack hit eventually may request light, tunable Hitstop through the shared owner. Ordinary Guard retains no global Hitstop, relying on Movement Lock/reaction/VFX/SFX. Perfect Guard retains its accepted 0.07s Hitstop. An enemy presentation component may request shared Hitstop but never becomes another Time.timeScale writer. Re-evaluate owner lifetime during Death integration so destroying the victim cannot strand global time.

Poise is an allowed future extension, explicitly not required for the first Goblin. Potential later EnemyPoise data: MaxPoise, CurrentPoise, PoiseRecoveryDelay, PoiseRecoverySpeed; attack data may gain PoiseDamage when different attack strengths create a real need. Perfect Guard may still cause direct Stagger without consuming Poise.

Deferred: Enemy Block, full Guard Break/Guard Meter/Posture, Behavior Tree, Utility AI, GOAP, a general hierarchical FSM, advanced NavMesh combat navigation, multi-enemy attack tokens, Boss phases, general Ability/Damage frameworks and Combat Event Bus. Counter remains a later player feature. Do not create generic tags, managers or empty abstractions in anticipation.

## Acceptance Gates

- Receiving/VFX/SFX: empty swing has no hit feedback; valid hit changes health once and produces one feedback group; lethal hit feedback survives victim disable; missing visual/audio config cannot suppress damage; cleanup completes and Console is clean.
- State/Cancel: all attack phases cancel safely; no leftover threat, telegraph, target or future damage; repeated cancel is harmless; same-frame transitions have defined outcomes.
- Reaction/Death: reaction only from allowed states and after cooldown; cooldown begins on reaction exit; ordinary hits do not interrupt Attacking or prolong Staggered; lethal hit selects terminal Dead; corpse is neither targeted nor damaged.
- Perfect Guard: every valid Perfect result cancels an active enemy attack and causes the stronger stagger regardless of ordinary reaction cooldown; outgoing callbacks do not revive the attack.
- Multi-attack/cooldown: each attack has correct timing and own content; global wait survives state changes; Strong cooldown is consumed on start and not refunded by failure.
- Hit validation: stepping out of range/direction, target disable/death and invalid references produce no remote hit; each step damages at most once.
- Strong Combo: all first-hit branches, Step2/3 misses, every participant exit path, failsafe unlock and player death are tested. Test pairing failures separately if guaranteed capture is later added.
- Spacing: correct facing while retreating/strafing, no rapid state oscillation, reasonable obstacle behavior and visible attack opportunities. Test complete behavior in the real camera before declaring the agent complete.
