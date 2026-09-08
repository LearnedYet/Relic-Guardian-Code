# Enemy Combat Resource Tracking

Last audited: 2026-09-08. This document records licensed resource selection and import status; it does not prove gameplay integration or runtime verification.

## Boundary

- Full licensed inspection source: `C:\Unity\Project\RelicGuardianAssetLab\Assets\HeroicFantasyCreaturesFullPackVol1\Must Have Fantasy Villains Pack\Goblin\FBX Files\`.
- Narrow main-project local root: `Assets/LocalLicensed/HeroicFantasyCreatures/Goblin/FBX Files/`.
- All listed assets are licensed local content. Keep them under `Assets/LocalLicensed/`, preserve `.meta` files when intentionally importing, and never commit or upload them.
- First-version locomotion and Forward Attack selection uses filenames without the `_RM` suffix. `EnemyMovement + CharacterController` remains the planned displacement owner.
- Before each new import, inspect Humanoid Rig/Avatar compatibility, Loop Time, Root Transform settings, authored displacement, duration, entry/exit pose and any Animation Events in the isolated project. Selection here does not authorize bulk import.

## First SwordShield Goblin Selection

| Role | Selected test-project file | Test GUID | Main-project status |
| --- | --- | --- | --- |
| Idle | `Goblin@IdleSwordShield.FBX` | `c5fb14a9a8ee85c439efc113375c7f04` | Present; same GUID; currently used |
| Chase / Run | `Goblin@RunSwordShield.FBX` | `e4a2c97a217f1074ead2d482fb2501f6` | Present as same filename but main GUID is `39d3d79bacf916341aba7adcbbbbfa8e`; currently used. Do not overwrite without a focused source/import comparison |
| Combat approach | `Goblin@WalkForwardSwordShield.FBX` | `6d7616f4d612f74478c53b90b5535030` | Imported 2026-09-08; GUID preserved; integration/runtime pending |
| Combat retreat | `Goblin@WalkBackwardsSwordShield.FBX` | `81ff775d44a6d2c43a3ac146691be121` | Imported 2026-09-08; GUID preserved; integration/runtime pending |
| Combat strafe left | `Goblin@StrafeLeftSwordShield.FBX` | `62a0eaca43f5fa24bbab674e0865aef4` | Imported 2026-09-08; GUID preserved; integration/runtime pending |
| Combat strafe right | `Goblin@StrafeRightSwordShield.FBX` | `670c36f4c9a731348b6b224515b6a8c4` | Imported 2026-09-08; GUID preserved; integration/runtime pending |
| Ordinary Attack 1 | `Goblin@Attack1ForwardSwordShield.FBX` | `ad72501e97f58b54a8d99792909cf33b` | Imported 2026-09-08; GUID preserved; integration/runtime pending |
| Ordinary Attack 2 | `Goblin@Attack2ForwardSwordShield.FBX` | `7f58aef2737b2394c91c3303d97af9e1` | Imported 2026-09-08; GUID preserved; integration/runtime pending |
| Ordinary Attack 3 | `Goblin@Attack3ForwardSwordShield.FBX` | `29b34a449d42585449b6fae9638900fa` | Imported 2026-09-08; GUID preserved; integration/runtime pending |
| Ordinary HitReaction | `Goblin@GetHitSwordShield.FBX` | `f1b38bfb8b647ea4abac6df05e771fb2` | Imported 2026-09-08; GUID preserved; integration/runtime pending |
| Death | `Goblin@DeathSwordShield.FBX` | `2e08a1ff694c0d94d8f958be7138fd58` | Imported 2026-09-08; GUID preserved; integration/runtime pending |

The main project also contains `Goblin@Attack1SwordShield.FBX` and `Goblin@WalkNormalSwordShield.FBX`; they belong to the existing prototype and are not the selected first-version Forward Attack/combat-spacing content above. Preserve them until their implemented consumers are deliberately migrated and verified.

## Strong Combo Direction

`Goblin@2HitComboSwordShield.FBX` (`c92b96316c2786849afb0c46ce785c5b`) and `Goblin@3HitComboSwordShield.FBX` (`6fe95df42ae250849a274d7c028f0239`) exist in the test project but are excluded as formal Gameplay Clips.

The planned Strong Combo uses the three selected independent Forward Attack Clips as `Step1 / Step2 / Step3`. Their final order, timing, transition compatibility and any bounded pairing/position correction remain future design and runtime work; this resource decision does not claim that the combo is implemented.

## Deferred or Excluded

- `Goblin@BlockSwordShield.FBX` (`c4c9978371ca2fb4cb740253014d6fdb`) is reserved for a later Enemy Block stage.
- `Goblin@IdleProtectedSwordShield.FBX` (`cb074ff1f46939e40b3f49f51926268a`) and `Goblin@WalkNormalSwordShield.FBX` (`3405aa351f154f24eb7afe52712cc754` in the test project) are not selected for the first combat agent.
- Dagger and Slingshot animation families are outside the first SwordShield Goblin scope.
- `_RM` locomotion and Forward Attack variants are not selected for the first version. A special attack may test Root Motion later through an explicit ownership contract.

## Perfect Guard Stagger Visual

No dedicated Perfect Guard Stagger Clip is selected yet. The first implementation may temporarily reuse `Goblin@GetHitSwordShield`, while gameplay Stagger duration/cancellation remains state-owned and independent from the visual Clip. A dedicated Stagger resource remains an open selection item rather than a blocker for the first receiving/VFX/SFX stage.

## 2026-09-08 Main-Editor Import Inspection

Unity 6000.3.19f1 recognized all nine copied FBX files and exposed one AnimationClip per file. Every importer is Generic, uses Copy From Other Avatar, resolves the shared `SK_GoblinAvatar`, and reports that source Avatar valid and non-Humanoid as expected for this package. Import Animation is enabled and the final Console check contained zero errors and zero warnings.

The current saved default Clips are WalkForward 0-30, WalkBackwards 0-30, StrafeLeft 0-30, StrafeRight 0-30, Attack1Forward 0-20, Attack2Forward 0-30, Attack3Forward 0-30, GetHit 0-19 and Death 0-56. Loop Time and Loop Pose are currently false for all nine. Root Rotation, Root Position Y and Root Position XZ locks are also false; Keep Original Position Y is true, while Keep Original Orientation and Position XZ are false. These are inspected facts, not approved final settings.

Do not connect these Clips to Animator yet. Before integration, preview pose/displacement and decide the four movement Clips' Loop settings plus the code-driven-displacement Root Transform policy as a focused configuration checkpoint.

## Remaining Resource Gaps

- Preview all nine Clips and approve the four movement Clips' Loop settings and the code-driven-displacement Root Transform policy before gameplay integration.
- Choose or author the Strong Attack telegraph VFX/SFX before the Strong Combo stage.
- Confirm Player Dodge and HitStun animation resources before Strong Combo control work.
- Decide whether Perfect Guard Stagger continues using GetHit or receives a dedicated Clip after the first stagger test.

## 2026-09-08 Import Verification

The nine selected `.FBX` files and their original `.meta` files were copied into the main-project Goblin `FBX Files` directory. SHA-256 comparisons matched the AssetLab source for every FBX and meta file, and all test-project GUIDs above were preserved. Existing main-project Idle/Run/prototype Attack1/WalkNormal files were not overwritten. Deferred Block, protected idle, packaged Combo, Dagger/Slingshot and `_RM` variants were not copied.

This is filesystem/GUID verification. Unity asset recognition, Clip settings, visual preview and Console status remain pending until inspected in the main Editor.
