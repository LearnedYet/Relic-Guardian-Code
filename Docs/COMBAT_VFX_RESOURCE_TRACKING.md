# Combat VFX Resource Tracking

Last verified: 2026-08-31.

This document records local licensed Combat VFX dependencies, AssetLab validation results, and the current selected candidates. It does not prove that any VFX is connected to gameplay in the main project.

## Status and Boundary

- Resource selection and isolated visual validation are complete in `C:\Unity\Project\RelicGuardianAssetLab`.
- Only the final selected assets, their recursive dependencies, and the final Guard validation scene are present in the main project under the ignored local-only boundary `Assets/LocalLicensed/CombatVFX/`, with Unity GUIDs preserved.
- The main project imported the copied assets with zero Console errors and zero warnings. The selected Prefabs and custom HDR materials resolve successfully.
- Final color, HDR intensity, size, orientation, Bloom response, spawn point, and combined readability must be tuned in the main project's real camera and combat scale.
- No Combat VFX is connected to Attack, ordinary Guard, or Perfect Guard gameplay yet.
- Everything under `Assets/LocalLicensed/CombatVFX/` and its parent local licensed boundary must remain unstaged and must never be committed or uploaded.

## Required Packages

| Package | Local source installer | Main-project local import root | Current role |
| --- | --- | --- | --- |
| Procedural Weapon Trails `1.1` | `C:\unasstes\Procedural Weapon Trails 1.1.unitypackage` | `Assets/LocalLicensed/CombatVFX/Dependencies/INab Studio/` | Real weapon-motion Trail for Attack1-4. |
| VFX - Impact and Hit - Vol 2 | `C:\unasstes\VFX - Impact and Hit - Vol 2.unitypackage` | `Assets/LocalLicensed/CombatVFX/Dependencies/CartoonVFX by Wallcoeur/` | Ordinary Guard and Perfect Guard impact foundations. |
| Stylized Hit Slash `1.3` | `C:\unasstes\Stylized Hit Slash [1.3].unitypackage` | `Assets/LocalLicensed/CombatVFX/Dependencies/VFX_Klaus/` | Ordinary Attack hit-impact candidates. |
| Cartoon FX 4 Remaster `1.5.1` | `C:\unasstes\Cartoon FX 4 Remaster R 1.5.1.unitypackage` | `Assets/LocalLicensed/CombatVFX/Dependencies/JMO Assets/` | Compact HDR impact and the selected HDR star accent. |

The main project already pins the required package dependencies:

- Visual Effect Graph `17.3.0`;
- Editor Coroutines `1.1.0`;
- URP `17.3.0`.

Because the INab package was relocated under `LocalLicensed`, its local `WeaponTrailEffect.DefaultPrefabPath` points to `Assets/LocalLicensed/CombatVFX/Selected/WeaponTrails/`. This is a local licensed package-path adjustment, not project gameplay code.

## Pruned Main-Project Layout

```text
Assets/LocalLicensed/CombatVFX/
├─ Selected/       final customized Prefabs and materials
├─ Dependencies/   only referenced package assets plus required complete script/importer groups
└─ Validation/     final Guard composition scene
```

The initial whole-package copy contained `1825` files and used approximately `424.37 MB`. It was replaced by a dependency list of `79` Unity assets; after Unity generated the required folder metadata, the local tree contains `199` files total and uses approximately `62.90 MB`. A serialized GUID cross-check found zero references to omitted assets from the former broad CombatVFX tree. The final Unity refresh compiled cleanly, every selected Prefab/material resolved, and the Guard validation scene reported zero missing scripts and zero broken Prefabs.

## Selected Attack Layers

### Weapon Trail

Validated Prefabs:

- primary: `Selected/WeaponTrails/Ice Stylized 3.prefab`;
- alternate: `Ice Water 1.prefab`;
- alternate: `Ice Water 2.prefab`, visually accepted at one brightness step below Water 1.

The package produces a real trail from weapon motion. It is suitable for an authored Attack Trail Window controlled by Animation Events; it is not a fixed crescent Slash Prefab.

### Hit Impact

Final candidates:

- `Selected/AttackHits/FX_hit_04_Ice.prefab`;
- `Selected/AttackHits/FX_hit_11_Ice.prefab`.

`FX_hit_11_Ice` keeps its first slash-shaped layer thin and compresses the other layers to approximately `50%` of their original world-space Y extent. Both use the local independent material `M_AttackHit_Ice_Add_HDR.mat`. Its currently persisted `Emission_Power` is `6.6`; treat this as an AssetLab value, not a final main-project brightness decision.

## Selected Guard Layers

### Ordinary Guard Foundation

- `Selected/GuardImpacts/GuardImpact_Normal_Test.prefab` derives from `VFX_SimpleImpact (7)`.
- AssetLab tuning used root scale `0.7` and Particle System simulation speed `1.5`.
- The current Guard comparison scene also contains a `CFXR4 Laser Impact (Orange)` instance for evaluating a compact HDR center burst. Its ray/smaller-impact material uses `_HdrMultiply = 6`; its `Center` material uses `_HdrMultiply = 8`; it also contains a short Point Light layer.

### Perfect Guard Foundation

- `Selected/GuardImpacts/GuardImpact_Perfect_WithSparks.prefab` combines the selected stronger Wallcoeur impact with the earlier independent spark layer.
- `M_GuardSparks_HDR.mat` is an independent URP Particles/Unlit additive material, so its HDR changes do not modify the source package material.
- The current Guard comparison scene also preserves the learner-selected `Stars` particle layer using the Cartoon FX 4 HDR star material.

The exact Ordinary-versus-Perfect layer assignment in `GuardImpactComparison.unity` is a visual composition checkpoint. Build reusable final Prefabs only after the main-project camera, Bloom, character scale, and hit point are visible together.

## Validation Artifacts

The final local composition checkpoint is `Assets/LocalLicensed/CombatVFX/Validation/GuardImpactComparison.unity`. It contains the learner-accepted ordinary and Perfect Guard layer arrangements and passed scene validation after pruning. Earlier broad comparison scenes and rejected candidates remain only in `RelicGuardianAssetLab`; they are not copied into the main project.

## Deferred or Rejected for the Base Pass

- `Sword slashes PRO 3.0` was not imported into the main project. It is an optional energy-slash layer for later Counter, Skill, or Finisher work.
- Large crescents, cross slashes, explosions, teleporter rings, and other skill-scale effects are excluded from ordinary Attack1-4.
- Cartoon FX 4 Ice Cross and comparison-only spark alternatives remain available locally but are not required for the chosen base Guard composition.
- Camera Shake, special SFX, stronger hitstop, and Counter/Finisher layers remain separate presentation work.

## Restoration

On another authorized workstation:

1. install URP `17.3.0`, Visual Effect Graph `17.3.0`, and Editor Coroutines `1.1.0`;
2. restore the four exact licensed installers listed above from an authorized local source;
3. use the selected roots in this document to calculate their recursive Unity dependencies in an isolated AssetLab;
4. preserve the selected assets under `Selected/`, package dependencies under `Dependencies/`, and the final Guard scene under `Validation/`, keeping all `.meta` files and GUIDs;
5. include the complete required INab runtime/editor script groups and Cartoon FX runtime/editor/shader importer groups, then point INab `WeaponTrailEffect.DefaultPrefabPath` to `Selected/WeaponTrails/`;
6. force an AssetDatabase refresh, wait for compilation, verify the Unity Console, and open only the local validation scenes before touching tracked gameplay scenes or Prefabs.
