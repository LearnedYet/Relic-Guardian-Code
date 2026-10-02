# Relic Guardian Current Handoff

Updated: 2026-09-30. Learner is switching conversations after accepting Guard Counter animation/damage and the repaired blue weapon Trail. Old Handoff is preserved verbatim at `Docs/Archive/HANDOFF_2026-09-25_GUARD_COUNTER_START.md`.

## Resume Contract

Follow AGENTS.md and relic-guardian-context bootstrap. Actual code/assets/Editor/Git outrank documents. CURRENT_STATE.md owns the active next step; use the Player Action and Combat VFX routes in CONTEXT_INDEX.md. Learner remains author of key gameplay/presentation code in small checkable chunks: give exact code/file/class/method/anchor/placement, explain new identifier meaning/scope/type/lifetime, inspect the saved edit before continuing. `好了`/`继续` does not authorize takeover. Cosmetic-only corrections remain Codex-owned.

Use the product-manager communication rules now saved in AGENTS.md: understand the real question, keep independent judgment, distinguish evidence from guesses. The last technical question concerned reuse: VFX/audio playback infrastructure is reusable; Counter-specific data/Events and confirmed-hit resource selection still need connection. No first Counter-audio edit has been assigned or written.

## Connected Guard Counter

- PlayerAttackType contains Basic and GuardCounter only; both use coarse Attacking and shared PlayerCombat.
- PlayerBlock stores a scaled refreshed one-use opportunity after covered Perfect Guard. Scene duration1s; expiry is derived from Time.time. Successful consumption/explicit clear sets end time0.
- PlayerActionController requires grounding and Blocking/Free for Counter Attack. Frame order is Dodge -> Block -> Attack (Counter first, Basic next) -> Jump. Accepted Dodge/new Block/Basic Attack/Jump clear the opportunity; rejected inputs do not. Blocking-to-Counter calls CancelBlock before entering Attacking. TryCancelAttack is Basic-only, so Counter does not inherit Block/Dodge cancellation.
- PlayerCombat shares target selection, facing/lunge, range confirmation, damage and cleanup. Counter data: damage1 per hit, targetRange2m, lungeSpeed5, lungeDistance1m. Two increasing hit-index Events each resolve once against the original saved target, without second-hit retarget. HitContext still contains damage/source/direction only.
- PlayerAnimator directly CrossFades to Base Layer.GuardCounter; entry/exit blends0.02/0.1s. Current state Speed1, no transitions, Apply Root Motion off. The earlier Animator-exit-wire discussion was settled in favor of code-driven finish.
- FinishGuardCounter at frame42 calls shared cleanup, closes both Trails, clears attack execution/type and begins soft recovery before returning to Free. FinishGuardCounterRecovery at frame108 checks soft recovery, current GuardCounter state and no transition, then returns to locomotion. Existing legal movement/actions may interrupt the visual tail. Learner reported sequence/damage and natural return normal.
- Local Clip: Assets/LocalLicensed/SwordAnimationPack/Guard/Parry_Counter_Attack.anim; GUID2eda5b55b2e475c409f1058e63eefc03; 60 FPS, length1.833333s, Loop Time off.

| Frame | Saved Event | Parameter |
| ---: | --- | ---: |
| 1 | OpenGuardCounterWeaponTrail | 0 |
| 2 | OpenGuardCounterHitWindow | 1 |
| 5 | CloseGuardCounterHitWindow | 1 |
| 8 | CloseGuardCounterWeaponTrail | 0 |
| 32 | OpenGuardCounterWeaponTrail | 0 |
| 37 | OpenGuardCounterHitWindow | 2 |
| 40 | CloseGuardCounterHitWindow | 2 |
| 42 | FinishGuardCounter | 0 |
| 108 | FinishGuardCounterRecovery | 0 |

Second Trail closes through frame42 cleanup; no separate second CloseGuardCounterWeaponTrail Event exists. No Counter Whoosh Events exist yet. These are Clip frames; Animator speed changes wall-clock timing.

## Accepted Blue Trail and Diagnosis

- Scene counterTrail references **Ice Stylized 3 Before Original Comparison** and its Prefab under Assets/LocalLicensed/CombatVFX/Selected/WeaponTrails/. Despite the backup-looking name, it is now the active blue Counter source after learner Replace and Keep. Do not delete it as unused or automatically switch back to the other Prefab.
- Current Scene overrides: Length0.23, Main Color(4,10,16,1), Secondary Color(0.15,1,3,1), LineCount4, second noise disabled/strength0. Effect Active/Value are false/0 at rest, true/1 during authored Events. The Scene overrides are authoritative.
- Final binder targets are **TrailTip / TrailBottom**, direct children of Frozen_Katana_Blue_Equipped under the P09 right hand. Tip localPosition(0.005,0.980,-0.041); Bottom(0,0.127,0.001). Both local rotations(270,0,0), equivalent to(-90,0,0).
- Independent Counter endpoints were proposed, but final live bindings use shared ordinary points. AttackTrail shares both; WeaponAura shares TrailBottom. Ordinary AttackTrail/WeaponAura visual regression after this rotation change is pending.
- Test source: Assets/INab Studio/Vfx Assets/Weapon FX Series/Weapon Trails FX/Trail Prefabs/Stylized 3.prefab in RelicGuardianAssetLab, GUID4e5665f6456bf074d992dcc2f6fe242f. Both use Weapon Trail Template.vfx GUID3b5956a5416040d42a1e3211ea44c72b. Main/test noise image bytes and importer settings matched; Graph changes concerned exposed-property ordering.
- Same Inspector parameters initially produced thin main-project stripes versus a broad demo brush. Vendor WeaponTrailEffect.Update aligns both endpoint Z axes using Quaternion.LookRotation(tip.position-bottom.position); main originally had blade direction along Y with identity local rotation. Learner rotated endpoints and reported improvement. After replacing with blue, Bottom Position X was accidentally -90 instead of Rotation X, causing a giant surface. MCP found this field error; learner corrected positionX0/rotationX-90 and explicitly accepted the final blue appearance.
- No per-frame endpoint calibration added. Fixed local alignment is retained for the rigid weapon; vendor LookRotation also chooses roll relative to world up, so both methods are not identical. Revisit only if a concrete motion causes flip/side-facing artifacts.

## Remaining Work and First Teaching Chunk

1. **Guard Counter motion Whoosh**: two independently configurable swing cues using existing CombatAudioData/CombatAudioPlayer/PlayerAttackPresentation. Inspect actual code; explain new configuration field(s); give one small learner edit beside existing Basic Whoosh data. Add guarded PlayerCombat forwarding and authored Clip Events in later chunks. Test empty swings as well as hits. Audio clips and exact cue times remain undecided.
2. **Counter confirmed-hit Ice VFX/SFX**: EnemyHitReceiver currently calls EnemyHitPresentation.PresentHit() without a feedback identity; that component has one ordinary Blood/Hit-audio configuration. Reuse lifetime/anchor/Hitstop/playback infrastructure with the smallest explicit selection boundary. Local FX_hit_04_Ice/FX_hit_11_Ice are unconnected candidates; exact selection/layering remains open. Preserve ordinary feedback and separate motion sound from confirmed-hit sound.
3. Complete focused Guard Counter regression and milestone records, then implement **Dodge Counter** by reusing attack execution.
4. Connect **independent timed Enchantment** after both Counters. Approved pending contract is in PLAYER_COMBAT_EXTENSION_DESIGN.md.

Dodge Counter has imported Assets/LocalLicensed/SwordAnimationPack/Dodge/Combo_Attack_01_01.anim only (GUIDdd32d216de68f9141aeed3894f1126b3, about1.583s, looping, no Events). No opportunity, enum member, execution or Animator connection exists. Existing Perfect Dodge afterimage/audio/Slow Motion are already implemented; do not recreate them. Shallow Dodge Trail remains paused and Distortion deferred.

PlayerEnchantment.cs already contains duration/deadline/HasEnchantment/RefreshEnchantment, but is not Scene-attached or called by production code and has no damage/feedback integration. Future rule: qualifying Perfect Guard/Perfect Dodge enters an independent timed status, retrigger refreshes it, duration is learner-tuned, bonus is base damage plus configurable integer addition; both Counters benefit. Implement damage first and retain an extension boundary for later effects. Opportunity and enchantment durations remain separate; exact expiry-during-attack sampling rule is not implemented.

## Verification and Protected State

- Learner reports: Counter sequence/damage normal, automatic locomotion return normal, final blue Trail fully satisfactory. Handoff refreshed static code/Scene/Clip/Animator evidence. Console returned zero errors/warnings. These reports/checks do not certify full Counter integration; dedicated audio/hit feedback and shared-endpoint regression remain pending.
- Editor was out of Play Mode and SampleScene.isDirty=false at final audit. Git still intentionally dirty: these are different state indicators.
- Preserve modified Player scripts, AGENTS.md, mirror-audit script, Goblin_Spacing.asset, Player Controller/Prefab, SampleScene and maintained docs; untracked Player scripts/materials and archived Handoffs also remain. Read fresh git status before changes. No staging/commit/push for this Handoff.
- Assets/LocalLicensed/ and its .meta remain ignored/local-only. Never upload licensed Prefabs/Clips/Events/VFX/SFX or include mixed Scene/Prefab/Controller in a code-only checkpoint. Full Unity and flattened GitHub histories remain separate; previous verified refs remain in CURRENT_STATE.md.
- Last verified MCP instances: My project@f22d513a32eb5447 and RelicGuardianAssetLab@d0fae1ba933aab0e. Discover current instances, explicitly select the intended one and return to main after Lab queries.
