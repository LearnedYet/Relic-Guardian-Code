# Relic Guardian Current Handoff

Updated: 2026-09-14. Ordinary Attack1/Attack2 selection through range, per-enemy cooldown and contextual Weight is runtime-verified; select the next independent feature before more gameplay changes.

## Resume

Follow `AGENTS.md` and the `relic-guardian-context` bootstrap; `CURRENT_STATE.md` owns the Exact Next Step. The learner remains the author of key gameplay code and Editor configuration unless they explicitly request takeover for a bounded scope. Review the actual file and Unity Console after each small functional batch; Unity MCP is connected to `My project@f22d513a32eb5447` but the instance hash may change after restart.

The ordinary two-attack slice is complete. Do not continue from a generic `继续` directly into another gameplay system. First choose whether to follow the approved Strong Attack path beginning with a Player HitStun action contract and later Dodge/death-safe recovery, or deliberately reorder the separate spacing/movement stage the learner previously asked about. Do not bundle either with Attack3, a complete AI Agent or deferred Death precision tests.

## Current implementation

- `MeleeAttackData` assets own shared move configuration, including start ranges and cooldown duration. Saved cooldowns are Attack1 `0s` and Attack2 `4s`.
- Each `EnemyAttack` owns `MeleeAttackOption[] attackOptions`; each serialized entry pairs one shared `AttackData` reference with contextual Weight. NearTarget saves Attack2 first at Weight `3`, then Attack1 at Weight `1`.
- `IsAttackOptionEligible` requires a non-null option/data reference, positive Weight, horizontal range legality and per-attack readiness. `TryStartAttack` sums eligible Weight, rejects zero total, rolls/subtracts shares and locks one `MeleeAttackData` for the full execution.
- Per-enemy `Dictionary<MeleeAttackData, float>` deadlines treat missing entries as Ready. Accepted start consumes cooldown before Startup; Miss, Perfect Guard and later cancellation do not refund it. Existing Global Attack Cooldown remains separate and begins at effective cleanup.
- `EnemyAI` still only supplies the coarse Chase request using common outer range `2.2m` and facing half-angle `15°`. Weight chooses which legal Ready attack, not when the whole agent attacks. Apply Root Motion remains disabled and EnemyMovement retains code-driven displacement/facing.

## Runtime evidence and remaining gap

- Cooldown checks passed overlap fallback `Attack2 -> Attack1 -> Attack2`, no-ready waiting at `1.2m`, Perfect Guard non-refund, and final Attack1/Attack2 cooldown tuning `0 / 4s`.
- Weight checks passed Attack2 `0` / Attack1 `1`, the reversed deterministic case, and repeated equal-positive overlap sampling in which both attacks appeared. Final saved Weights are Attack2 `3`, Attack1 `1`.
- Existing damage, Recovery, Stagger, Global Attack Cooldown and final Console remained normal. Unity compilation and independent `Assembly-CSharp.csproj` build ended with zero errors and zero warnings.
- The positive-Weight sample proves both attacks are reachable, not an exact statistical ratio or deterministic replay guarantee. Spacing/decision pacing, Strong Attack/Combo prerequisites and a complete AI Agent remain unimplemented.

## Protected local state

Keep these pre-existing local changes unstaged and unmodified unless the learner explicitly scopes them into future work:

- `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`
- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs`
- `Assets/Scenes/SampleScene.unity`

`Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` are local-only and must never be committed or uploaded. The saved Scene and local Animator are part of the runtime evidence but are not reproducible from the code/document mirror alone.

## Git synchronization

- Local weighted-selection feature commit: `5e2e26d Complete weighted enemy attack selection`.
- Local full-project feature commit: `1db72b6 Add minimum enemy multi-attack selection`.
- Local documentation record commit: `47d1bc6 Record enemy multi-attack checkpoints`.
- Flattened GitHub feature commit: `1a69b38 Sync enemy multi-attack range selection`.
- Current verified GitHub `main`: `08bf13941c9fc66d9bc640d479a378efcf8be3d2` (`Record enemy multi-attack checkpoints`).
- The local full-project history and flattened GitHub mirror intentionally remain separate. Do not pull or merge GitHub `main` directly into this Unity workspace.
- If GitHub CLI connectivity again differs from browser connectivity, the last successful operation used process-local SOCKS5 proxy `socks5h://127.0.0.1:7897` with OpenSSL and HTTP/1.1; do not persist that volatile proxy setting.

The preceding Handoff is archived at `Docs/Archive/HANDOFF_2026-09-14_PER_ATTACK_COOLDOWN.md`.
