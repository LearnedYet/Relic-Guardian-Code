# Relic Guardian Current Handoff

Updated: 2026-09-09. Recent development checkpoint: ordinary Enemy HitReaction/protection, Global Attack Cooldown and start-time range/facing admission. This file holds handoff context; it does not maintain an independent next-step record.

## Resume

Read [CURRENT_STATE.md — Exact Next Step](CURRENT_STATE.md#exact-next-step) for the sole maintained active step, then select the task-type route from [CONTEXT_INDEX.md](CONTEXT_INDEX.md). Preserve the complete startup sequence and authority order in AGENTS.md and the relic-guardian-context skill.

The learner remains the author of key gameplay/presentation code and defaults to performing Scene/Prefab/Inspector/Animator/Event configuration. Continue with explanation, one concrete edit and actual-file review; Codex takes over an Editor-configuration scope only when explicitly requested.

## Recent Completed Work

- Perfect-only shared Hitstop and Ordinary movement lock/player reaction are connected.
- Attack1-4 Trail/Whoosh are connected; Attack4 Windup and main swing use separate pose Events.
- WeaponAura remains independent of transient attack feedback; Attack and Guard audio use separate players.
- PlayerCombat now sends one HitContext through EnemyHitReceiver to EnemyHealth after its existing target confirmation. EnemyHitPresentation owns the connected Blood VFX and independent temporary two-layer Hit Audio lifetime.
- The learner reported the receiving chain and current feedback normal in Play Mode; the final Console was clean.
- EnemyAttack now owns idempotent shared natural-finish/cancel cleanup, including outgoing Threat removal and disable cleanup. EnemyStateController owns minimum coarse Chase/Attacking admission and natural completion; EnemyAI no longer treats EnemyAttackPhase as whole-agent permission.
- The learner runtime-verified NearTarget's natural chase/attack loop, cancellation in Startup/HitWindow/Recovery and repeated cancellation with no delayed extra damage, stale facing, stuck telegraph or Console issue. FarTarget remains intentionally outside the AI state system.
- EnemyStateController now owns `Staggered`, ordinary surviving-hit duration and a post-reaction cooldown deadline. EnemyHitReceiver preserves damage/feedback independently, rejects lethal reaction, and ordinary hits do not reset Staggered or interrupt Attacking. EnemyAnimator drives the connected GetHit state at saved Speed `1.4`.
- EnemyAI now requires both range and a saved `15°` horizontal facing threshold before attack Startup. EnemyMovement exposes rotation-only Turn; an in-range facing-away enemy stops translation and turns in place without early telegraph/attack.
- The learner runtime-verified immediate repeated GetHit during a fixed Staggered deadline, post-reaction protection, independently counting Global Attack Cooldown, ready counterattack admission, Attacking exclusion, natural movement recovery, turn-in-place admission and a clean Console on 2026-09-09.
- Leaving attack range after Idle initially produced sliding because Idle-to-Run still used Exit Time. The learner runtime-checked the correction and the saved Controller now has `Speed > 0.1`, `0.1s` transition duration and `Has Exit Time = false`.
- The nine copied Goblin Clips resolve to the valid shared Generic Avatar in the main Editor. Their saved Loop/Root settings were inspected but remain unapproved for Animator integration.

CURRENT_STATE.md now routes the next stage to retained-corpse terminal Death. Preserve lethal-hit VFX/SFX lifetime, establish Dead authority and cleanup before Death presentation, and keep target/hit exclusion inside this slice.

The current full-speed Chase during Global Attack Cooldown is accepted as a minimal baseline, not final pursuit pacing. Reduce pressure later with Approach/Retreat/Strafe/Wait decisions rather than conflating movement behavior with the cooldown timer.

HitWindow-time target/range/direction validation remains deferred to its dedicated later stage; start-time admission does not prove impact validity.

## Checkpoint and Working-Tree Context

The 2026-09-09 Enemy reaction/attack-pacing synchronization completed. Local full-project feature checkpoint is `9da9b25`; the flattened GitHub code/document feature mirror is `f1fe243`, verified at `f1fe243404f9b00d470d4065f2ca5407ab3c7e8a`. CURRENT_STATE.md records these together with earlier checkpoints.

Preserve the remaining local Player Animator, player Prefab, mixed Scene and PlayerHealth working-tree entries. NearTarget's wiring and licensed Goblin Controller remain outside the focused commits. Obtain the live file list from git status and apply the existing AGENTS.md protection/licensed-asset rules. This handoff grants no new commit or push authority.

## Historical References

The replaced confirmed-hit handoff is preserved at Docs/Archive/HANDOFF_2026-09-08_ENEMY_HIT_FEEDBACK.md.
The replaced 2026-09-06 handoff is preserved at Docs/Archive/HANDOFF_2026-09-06_GUARD_REACTION_ATTACK_MOTION.md.
The prior 2026-08-31 handoff remains at Docs/Archive/HANDOFF_2026-08-31_GUARD_VFX_RESOURCES.md. Detailed development/testing and previous snapshots remain in DEV_LOG.md and Docs/Archive/. Retrieve them only by targeted historical search.
