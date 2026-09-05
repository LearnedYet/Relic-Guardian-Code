# Combat VFX Resource Tracking

Last verified: 2026-09-01.

This document records local licensed Combat VFX dependencies, AssetLab validation results, and the current selected candidates. It does not prove that any VFX is connected to gameplay in the main project.

## Status and Boundary

- Resource selection and isolated visual validation are complete in `C:\Unity\Project\RelicGuardianAssetLab`.
- Only the final selected assets, their recursive dependencies, and the final Guard validation scene are present in the main project under the ignored local-only boundary `Assets/LocalLicensed/CombatVFX/`, with Unity GUIDs preserved.
- The main project imported the copied assets with zero Console errors and zero warnings. The selected Prefabs and custom HDR materials resolve successfully.
- Final color, HDR intensity, size, orientation, Bloom response, spawn point, and combined readability must be tuned in the main project's real camera and combat scale.
- `Normal Guard Impact.prefab` and `Perfect Guard Impact.prefab` are connected only to their matching Guard results through `PlayerGuardPresentation` and passed the learner's focused in-combat runtime checks. Attack VFX remains unconnected.
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

The initial whole-package copy contained `1825` files and used approximately `424.37 MB`. It was replaced by a dependency list of `79` Unity assets. After Unity generated folder metadata and later selected local variants were added, the current local tree contains `222` files total and uses approximately `70.88 MiB`. The earlier serialized GUID cross-check found zero references to omitted assets from the former broad CombatVFX tree. The dependency-pruned Unity refresh compiled cleanly, and the later focused imports left the Console clean.

## Selected Attack Layers

### Weapon Trail

Current selected roles:

- basic Attack candidate A: `Selected/WeaponTrails/Subtle 1 Ice.prefab`, a restrained ice-blue recolor of vendor `Subtle 1`;
- basic Attack candidate B: `Selected/WeaponTrails/Subtle 2 Ice.prefab`, a slightly stronger ice-blue recolor of vendor `Subtle 2`;
- future Perfect Guard Counter candidate: `Selected/WeaponTrails/Ice Stylized 3.prefab`;
- other higher-emphasis Attack candidates: `Ice Water 1.prefab` and `Ice Water 2.prefab`, with Water 2 retained at one brightness step below Water 1.

The two Subtle Prefabs preserve their vendor GUIDs `dd506520638422b488b79ab9ee75186f` and `a37e8b71f3b77d443a68f3e279bac75b`. Their shared missing dependency `INab_Noise_21.png` was added under `Dependencies/INab Studio/Common/Textures/Noise/` with GUID `14d21f23f8c0e564697377fe780a21bc`.

Initial local color values, pending real-camera tuning:

| Prefab | Color | Main Color | Secondary Color |
| --- | --- | --- | --- |
| `Subtle 1 Ice` | `(0.75, 2.5, 5, 1)` | `(1, 2.5, 4, 1)` | `(0.0375, 0.25, 0.75, 0.105882354)` |
| `Subtle 2 Ice` | `(1.5, 5, 10, 1)` | `(2, 5, 8, 1)` | `(0.075, 0.5, 1.5, 0.5176471)` |

The package produces a real trail from weapon motion. It is suitable for an authored Attack Trail Window controlled by Animation Events; it is not a fixed crescent Slash Prefab.

### Hit Impact

Current basic-Attack selection:

- `Selected/AttackHits/Blood/FX_hit_03_Blood.prefab`, selected as the primary confirmed-hit visual candidate for ordinary Attack1-4 hits;
- independent materials: `Blood/Materials/M_AttackHit_Blood_Add_HDR.mat` and `M_AttackHit_Blood_APB.mat`.

The selected Blood Prefab preserves AssetLab GUID `b33c07f1ea0c90d45a6b04302ea31a43`; the two materials preserve GUIDs `7c6181ef189d94e438de16382445a0ed` and `4684a66084693b74fb88af52f711a32a`. Main-project dependency inspection resolved all `10` direct/indirect assets with zero missing references, and Unity imported the selection with a clean Console. It remains unconnected and pending real-camera placement, orientation, scale, lifetime, and brightness tuning.

Reserved ice candidates:

- `Selected/AttackHits/FX_hit_04_Ice.prefab`;
- `Selected/AttackHits/FX_hit_11_Ice.prefab`.

`FX_hit_11_Ice` keeps its first slash-shaped layer thin and compresses the other layers to approximately `50%` of their original world-space Y extent. Both Ice Prefabs use the local independent material `M_AttackHit_Ice_Add_HDR.mat`. Its currently persisted `Emission_Power` is `6.6`; treat this as an AssetLab value, not a final main-project brightness decision.

## Selected Guard Layers

### Ordinary Guard

- Current final: `Selected/GuardImpacts/Normal Guard Impact.prefab`.
- It supersedes `GuardImpact_Normal_Test.prefab` as the main-project choice, uses learner-accepted root scale `0.5`, and retains non-looping Play On Awake Particle Systems at simulation speed `1.5` with Stop Action None.
- `SampleScene` references this Prefab from `PlayerGuardPresentation`. The learner accepted its real-camera position, scale, brightness, one-spawn branch behavior, and explicit cleanup on 2026-09-01.

### Perfect Guard

- Current final: `Selected/GuardImpacts/Perfect Guard Impact.prefab`.
- It supersedes `GuardImpact_Perfect_WithSparks.prefab` as the main-project choice and preserves the learner-selected stronger impact, cross, spark, and star composition. Its main named root currently uses scale `0.68`.
- `M_GuardSparks_HDR.mat` is an independent URP Particles/Unlit additive material, so its HDR changes do not modify the source package material.
- The Prefab is connected only to the Perfect branch in `PlayerGuardPresentation` with an independent `1.8s` cleanup default and passed the learner's focused in-combat branch, presentation, cleanup, damage, and Console checks on 2026-09-01.

`GuardImpactComparison.unity` remains the visual composition checkpoint. The reusable final Guard Prefabs were subsequently created, tuned, connected, and focused-runtime-verified in the main project.

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
