# Relic Guardian Current Handoff

Updated: 2026-09-14. Attack2 and minimum deterministic range selection are runtime-verified and synchronized; per-attack cooldown eligibility is next.

## Resume

Follow `AGENTS.md` and the `relic-guardian-context` bootstrap; `CURRENT_STATE.md` owns the Exact Next Step. The learner remains the author of key gameplay code and Editor configuration. Continue only the per-attack cooldown concept: shared cooldown duration belongs in `MeleeAttackData`, while mutable cooldown readiness/deadlines belong to each enemy at runtime. Explain every new identifier before asking the learner to edit, and review the actual file after each small functional batch.

Do not add Weight/random choice, a complete Enemy AI Agent, Attack3, Strong Combo, spacing behavior, or deferred Death precision testing in this slice. Existing Global Attack Cooldown remains a separate system.

## Current implementation

- `MeleeAttackData` is a ScriptableObject containing one melee attack's timing, animation, movement, tracking, damage, impact validation, and selection-range configuration.
- `Goblin_Attack1` selects in `[0, 2]` metres; `Goblin_Attack2` selects in `[1.5, 2.2]` metres. NearTarget's ordered `attackOptions` places Attack2 first and Attack1 second, so overlap at `1.5`-`2` metres deterministically selects Attack2.
- `EnemyAttack.TryStartAttack` calculates horizontal distance, scans `attackOptions` in array order, skips null entries, chooses the first range-legal attack, and locks it in `currentAttackData` for the full execution. Cleanup clears that runtime reference.
- `EnemyAI` uses horizontal distance with outer attack range `2.2` metres and facing half-angle `15` degrees.
- The local licensed Goblin Animator has `Attack2SwordShield` configured and is intentionally excluded from Git. Apply Root Motion remains disabled; attack displacement is code-driven through `CharacterController`.

## Runtime evidence and remaining gap

- Learner runtime tests passed: `1.2m` selects Attack1, `1.7m` overlap selects Attack2 by array order, and `2.1m` selects Attack2 only.
- Normal `1` damage, attack recovery, and existing Global Attack Cooldown still behave correctly. The last checked Play Mode session ended with zero Console errors and zero warnings.
- Selection is currently deterministic array order plus range legality only. There is no per-attack cooldown readiness and no Weight/random selection.
- Next concept: add a cooldown duration to each shared `MeleeAttackData` asset, then keep each attack's changing ready-time/deadline in per-enemy runtime state. Never store mutable deadlines in the shared ScriptableObject asset.

## Protected local state

Keep these pre-existing local changes unstaged and unmodified unless the learner explicitly scopes them into future work:

- `Assets/RelicGuardian/Player/Animator/RelicGuardianPlayer.controller`
- `Assets/RelicGuardian/Player/RelicGuardianPlayer.prefab`
- `Assets/RelicGuardian/Player/Scripts/PlayerActionController.cs`
- `Assets/RelicGuardian/Player/Scripts/PlayerHealth.cs`
- `Assets/Scenes/SampleScene.unity`

`Assets/LocalLicensed/` and `Assets/LocalLicensed.meta` are local-only and must never be committed or uploaded. The saved Scene and local Animator are part of the runtime evidence but are not reproducible from the code/document mirror alone.

## Git synchronization

- Local full-project feature commit: `1db72b6 Add minimum enemy multi-attack selection`.
- Local documentation record commit: `47d1bc6 Record enemy multi-attack checkpoints`.
- Flattened GitHub feature commit: `1a69b38 Sync enemy multi-attack range selection`.
- Current verified GitHub `main`: `08bf13941c9fc66d9bc640d479a378efcf8be3d2` (`Record enemy multi-attack checkpoints`).
- The local full-project history and flattened GitHub mirror intentionally remain separate. Do not pull or merge GitHub `main` directly into this Unity workspace.
- If GitHub CLI connectivity again differs from browser connectivity, the last successful operation used process-local SOCKS5 proxy `socks5h://127.0.0.1:7897` with OpenSSL and HTTP/1.1; do not persist that volatile proxy setting.

The preceding Handoff is archived at `Docs/Archive/HANDOFF_2026-09-11_ATTACK1_FORWARD.md`.
