# Relic Guardian Current Handoff

Updated: 2026-09-11. Attack1Forward footwork and frame-1-only tracking are complete; hit-time validation is next.

## Resume

Follow AGENTS.md and relic-guardian-context bootstrap; CURRENT_STATE.md owns the Exact Next Step. Continue only Attack1 impact-time live-target/distance/direction validation before Attack2/3. Learner remains the author of key code and Editor configuration. Give exact file/class/method anchors, explain each new identifier first, and review one small functional batch after the learner edits it.

## Current implementation

- EnemyAttack holds one nested `EnemyAttackData` object and reads damage, Startup/HitWindow/Recovery timing, animation lead/state name, movement start/end, tracking cutoff and total movement distance through read-only properties.
- Local `Base Layer.Attack1SwordShield` now uses the selected `Attack1ForwardSwordShield` Motion at Speed `1`; state name remains unchanged. Apply Root Motion is off.
- NearTarget's saved local data is damage `1`, Startup `0.5`, HitWindow `0.2`, Recovery `0.4`, animation lead `0.1667`, movement start `0.033`, tracking end `0.067`, movement end `0.333`, and distance `0.6`.
- `EnemyAttack` tracks only while animation elapsed time is within `[MovementStartTime, TrackingEndTime)`, before applying movement. Later frames retain the last facing. It computes movement from successive normalized progress samples so total configured distance is distributed across the window.
- `EnemyMovement.MoveDuringAttack` applies the already-calculated incremental horizontal distance through `CharacterController`; it does not add another time multiplier or turn the enemy.
- Attack start/cancel reset the animation-relative timer. Existing Death/Perfect Guard cancellation and Global Attack Cooldown boundaries remain unchanged.

## Runtime evidence and remaining gap

- Learner separately runtime-verified the frame-1-to-frame-10 footwork and the frame-1-only tracking behavior. No issue, airborne turning or post-landing slide was reported. Post-test Console checks contained zero errors and zero warnings.
- The current HitWindow still sends the scheduled hit to the saved target without current live/active, distance or direction validation. Limited tracking is not hit validation and does not yet create a Miss.
- First-test impact tuning was proposed but not accepted: range `2m`, maximum facing half-angle `30°`. Confirm it before adding fields. Validate against the attacker's committed `transform.forward`, not by rotating toward the target at impact. A failed test should skip ReceiveHit/confirmed feedback while HitWindow, Recovery and Global Attack Cooldown continue.
- No Attack2/3 selection, Strong Combo, Root Motion or deferred Death precision test was added.

## Protected local state

- `SampleScene.unity` is a large mixed dirty file containing the accepted NearTarget references/tuning plus older protected local work; keep it unstaged.
- `RelicGuardianPlayer.controller`, `RelicGuardianPlayer.prefab`, `PlayerHealth.cs`, and the whitespace-only `PlayerActionController.cs` change remain protected and unstaged.
- The local Goblin Animator Controller and all licensed FBX resources remain ignored under `Assets/LocalLicensed/`; record their configuration but never upload them.
- Code/document Git checkpoints intentionally cannot reproduce local Scene/Animator wiring by themselves. Inspect actual saved local assets before trusting serialized values.

## Git synchronization

- Local full-project feature checkpoint: `c8cfacf Add Attack1 Forward footwork and tracking`.
- Flattened GitHub feature checkpoint: `62702f7 Sync enemy combat through Attack1 Forward footwork`.
- The normal 2026-09-11 feature push was verified with `git ls-remote`: `refs/heads/main` reached `62702f7a4d33ffe2d63754d860313261bf3491ca` before the record-only documentation follow-up.
- The local commit used an explicit 12-file allowlist: three Enemy scripts, eight maintained documents and one archived Handoff. The mirror used an explicit 26-file allowlist to bring the public code/document branch forward through all locally saved combat work since its prior checkpoint.
- Full Unity history and the flattened mirror remain separate. Scene, Prefab, Controller, Unity `.meta`, licensed assets and unrelated remote-only files were excluded.

The preceding Handoff is archived at `Docs/Archive/HANDOFF_2026-09-10_ATTACK1_DATA_MIGRATION.md`.
