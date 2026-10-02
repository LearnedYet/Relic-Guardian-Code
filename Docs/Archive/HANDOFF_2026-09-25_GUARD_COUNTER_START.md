# Relic Guardian Current Handoff

Updated: 2026-09-25. The learner is switching conversations to begin the Perfect Guard Counter feature. The former 2026-09-22 Handoff is preserved verbatim at `Docs/Archive/HANDOFF_2026-09-22_PERFECT_DODGE_SLOW_MOTION.md`.

## Resume Contract

Follow `AGENTS.md` and the `relic-guardian-context` bootstrap. Actual code, Unity assets, Editor state and Git status outrank documents. `Docs/CURRENT_STATE.md` owns the sole active Exact Next Step; use the Player Action / Guard routes in `Docs/CONTEXT_INDEX.md` and the approved `Docs/PLAYER_COMBAT_EXTENSION_DESIGN.md` for the Counter design. Historical plans are not implementation evidence.

The learner remains the author of key gameplay and presentation code. Teach one independently checkable concept at a time. Each learner edit must include the exact code, file, containing class/method, stable nearby anchor and before/after/replace direction. Explain every new identifier's responsibility, scope, lifetime, type and English words before asking the learner to create it. Inspect the saved file before the next step. Codex may directly fix only unambiguous cosmetic errors unless the learner explicitly requests takeover.

## Current Checkpoint

- Perfect Dodge gameplay and its current afterimage, layered audio and Slow Motion are in place. The saved Slow Motion is `0.25s / 0.25`; the learner accepted the current Scene audio mix. A controlled same-frame Slow-Motion-then-Hitstop test and active-owner disable recovery passed on 2026-09-25. Reverse request order and natural-combat overlap were not separately tested. The isolated Shallow Trail experiment remains paused; Distortion is separate later work.
- `PlayerBlock.ResolveGuardHit()` currently returns `GuardResult.Perfect` after a qualifying covered hit during its open Perfect window. It does not yet create or store a Guard Counter opportunity. `PlayerCombat` still executes only the Basic four-step attack, and `PlayerActionController` owns the existing Dodge/Block/Attack/Jump arbitration. No Guard Counter or Dodge Counter gameplay, Animator state, hit window, or VFX binding exists.
- The learner selected AssetLab Humanoid `Parry_Counter_Attack` for Perfect Guard Counter. Its original `.anim` and `.meta` are imported under ignored `Assets/LocalLicensed/SwordAnimationPack/Guard/` with preserved GUID `2eda5b55b2e475c409f1058e63eefc03`; Unity recognized the clip and the Console had zero errors/warnings. Source duration is approximately `1.833s`, Loop Time is enabled, and it has no Animation Events.
- The learner selected AssetLab Humanoid `Combo_Attack_01_01` for a later Perfect Dodge Counter. Its original `.anim` and `.meta` are imported under ignored `Assets/LocalLicensed/SwordAnimationPack/Dodge/` with preserved GUID `dd32d216de68f9141aeed3894f1126b3`; Unity recognized the clip and the Console had zero errors/warnings. Source duration is approximately `1.583s`, Loop Time is enabled, and it has no Animation Events.
- The intended shared Counter weapon-trail audition candidate is existing `Assets/LocalLicensed/CombatVFX/Selected/WeaponTrails/Ice Stylized 3.prefab`. Both Counters should use the main-project Ice hit-effect family: existing `Selected/AttackHits/FX_hit_04_Ice.prefab` and `FX_hit_11_Ice.prefab`. The exact one-versus-layered choice, timing, placement and scale are undecided; none is runtime-accepted or wired. Details are in `Docs/COMBAT_VFX_RESOURCE_TRACKING.md`.

## First Teaching Step

Start with one design decision: define when the qualifying Perfect Guard creates a one-use Counter opportunity, how long it stays valid, whether Attack during Blocking can use it or only after returning to Free, and what expires/consumes it. Keep ownership per the approved design: `PlayerBlock` owns the opportunity; `PlayerActionController` admits Attack; `PlayerCombat` executes the Counter through its shared attack flow. Do not invent a separate action FSM or copied attack executor. Once the learner has chosen the input/expiry contract, teach only the first bounded code edit with its actual snippet and inspect their saved file before moving on. Do not connect animation/VFX in the same conceptual step.

Before connecting either Counter animation, make its local copy one-shot (both source clips currently loop) and author its own timing/Events. Keep Apply Root Motion off and player displacement code-owned. Audition the shared Ice weapon effect and Ice hit candidates later with the learner rather than treating resource import as runtime acceptance.

## Protected Local State

The full Unity `main` working tree remains intentionally dirty. Preserve the existing modified `AGENTS.md`, mirror-audit script, Enemy spacing asset, Player Animator Controller, Player Prefab, `SampleScene.unity`, Player gameplay/presentation scripts, maintained documents, untracked Player scripts/materials and earlier Handoff archives. Check fresh `git status --short --branch` before acting; do not reset or overwrite these files.

`Assets/LocalLicensed/` and its `.meta` are ignored, local-only and must never be committed or uploaded. The two Counter animation imports reside there. No Scene/Prefab/Animator/gameplay change, staging, commit or push was performed for Counter resource preparation or this Handoff.
