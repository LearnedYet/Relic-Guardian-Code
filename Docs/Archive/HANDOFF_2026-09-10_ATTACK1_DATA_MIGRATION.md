# Relic Guardian Archived Handoff - Attack1 Data Migration

Archived: 2026-09-11. This preserves the handoff that preceded Attack1Forward footwork and limited tracking.

## Resume

Follow AGENTS.md and relic-guardian-context bootstrap; CURRENT_STATE.md owns the Exact Next Step. Continue Attack1 Forward only. Learner explicitly remains key-code author and prefers concise instructions, concrete locations and one small functional batch before review. Do not infer takeover from continue/OK.

## Current implementation

- Prior checkpoint 2af364c covers Death, Perfect Guard Stagger, slow GetHit exit blend, shared hit Hitstop, paused grounded Block fix and small recoil.
- New EnemyAttackData is an ordinary System.Serializable class, not MonoBehaviour. Fields/properties cover damage, startup/hit-window/recovery durations, animation lead time and animation state name.
- EnemyAttack now reads attackData for all these values; animation starts via Animator.Play(attackData.AnimationStateName, 0, 0f). Current configured state remains Base Layer.Attack1SwordShield (old prototype), with damage 1, startup 0.5, hit 0.2, recovery 0.4, lead 0.15.
- Source/compile/Console checks passed. Learner reported the requested single-attack migration regression test normal.
- Forward animation integration, attack movement/tracking and impact-time distance/direction validation are NOT implemented. No second/third move selection yet.

## Attack1 preview evidence

Live Clip inspection: Goblin@Attack1ForwardSwordShield.FBX, Clip Attack1ForwardSwordShield, 30 FPS, frames 0-20, duration about 0.666667 seconds, non-looping.

Learner message exactly: 开始 1 命中5 1落地10

Provisional reading: foot movement starts frame 1, impact frame 5, landing frame 10. Extra 1 before 落地 is ambiguous; briefly confirm before finalizing timings. At playback Speed 1, these are 0.0333 / 0.1667 / 0.3333 seconds measured from animation start. Attack admission has an earlier telegraph interval; do not confuse these clocks. A slower state speed requires conversion.

Last inspected saved GoblinEnemy.controller still used prototype Attack1SwordShield, Speed 0.65, old Attack1 Clip. Last inspected disk SampleScene still held old flat attack fields although live Editor showed correct nested Attack Data; learner was asked to save. Recheck current disk/editor before assuming persistence. Do not overwrite the mixed Scene.

## Requested behavior

Attacks should advance with authored footwork and limited target tracking, then stop tracking before impact. EnemyMovement remains sole displacement/facing writer through CharacterController; Apply Root Motion stays off. Couple impact-time geometry validation with tracking so dodging away does not still receive scheduled remote damage. Reuse cancellation/Death/Perfect Guard cleanup and Global Attack Cooldown. Precise movement distance, stop distance and tracking cutoff are not finalized.

## Verification and protected state

Prior Death precision tests were explicitly deferred; do not restart them. Do not claim all Perfect Guard regressions or recoil obstacle/frame-rate tests were verified. Existing learner runtime reports and limits remain in CURRENT_STATE/DEV_LOG.

Local code/document save only; no push. Protected dirty files include player Animator, player Prefab, PlayerHealth, SampleScene and whitespace-only PlayerActionController. Licensed Goblin resources remain ignored and local-only. Earlier Handoff is archived in Docs/Archive/HANDOFF_2026-09-10_COMBAT_FEEDBACK.md.
