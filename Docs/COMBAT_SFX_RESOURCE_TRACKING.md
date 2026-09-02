# Combat SFX Resource Tracking

Last verified: 2026-09-02.

## Status and Boundary

- Source package: local licensed `Melee Weapons Pack 1` in `RelicGuardianAssetLab`.
- Seven learner-selected WAV files and their original `.meta` files were copied into the ignored formal-project boundary `Assets/LocalLicensed/CombatSFX/Selected/Guard/` with GUIDs preserved.
- The accepted layer settings are stored locally at `Assets/LocalLicensed/CombatSFX/Selected/Guard/Guard_SFX_Layer_Configuration.json`.
- Formal Guard presentation now consumes these clips through independent serialized Ordinary and Perfect cue data. `PlayerBlock`, `PlayerHitReceiver`, damage resolution, Hitstop, Camera Impulse, and Gameplay consequences remain unchanged by the SFX connection.
- Everything under `Assets/LocalLicensed/CombatSFX/` remains unstaged and must never be committed or uploaded.

## Ordinary Guard - 3 Layers

- Master Volume: `1.0`
- Mute/Solo: false for every layer

| Layer | Local formal-project path | GUID | Volume | Pitch | Delay |
| ---: | --- | --- | ---: | ---: | ---: |
| 1 | `Ordinary/METLImpt_Designed Metal Hit High 04_DDUMAIS_NONE.wav` | `97ba20f7558fefc488e4c02ed3ed0581` | `0.808` | `1.000` | `0.000s` |
| 2 | `Ordinary/METLImpt_Impact Metal Ring 10_DDUMAIS_NONE.wav` | `43f5ebefc7104b14a96c829776393779` | `0.179` | `1.000` | `0.000s` |
| 3 | `Ordinary/METLImpt_Impact Metal Clean 11_DDUMAIS_NONE.wav` | `157f22f77da56f4498d81924fd32fe9d` | `0.452` | `1.000` | `0.000s` |

All paths above are relative to:

`Assets/LocalLicensed/CombatSFX/Selected/Guard/`

## Perfect Guard - 4 Layers

- Master Volume: `1.0`
- Mute/Solo: false for every layer

| Layer | Local formal-project path | GUID | Volume | Pitch | Delay |
| ---: | --- | --- | ---: | ---: | ---: |
| 1 | `Perfect/METLTonl_Designed Metal Hit Tonal 01_DDUMAIS_NONE.wav` | `dc1208c9e43604b44b0aa3d2fc101ab9` | `0.833` | `1.000` | `0.000s` |
| 2 | `Perfect/METLImpt_Designed Metal Hit Low 05_DDUMAIS_NONE.wav` | `1d6dd280795b0e3419f99d95b0c97945` | `0.387` | `0.834` | `0.000s` |
| 3 | `Perfect/METLTonl_Designed Metal Hit Ring 03_DDUMAIS_NONE.wav` | `2e64b826b7ffaec4f9017a3ce41a8a36` | `0.274` | `0.789` | `0.000s` |
| 4 | `Perfect/METLTonl_Designed Metal Hit Tonal 09_DDUMAIS_NONE.wav` | `ce37be239b8858945823bbd54b2e576e` | `1.000` | `1.000` | `0.030s` |

## Implemented Formal Integration

1. `GuardResult` remains the authoritative Ordinary/Perfect classification; `PlayerGuardPresentation` selects one corresponding `CombatAudioData` after Gameplay Resolution.
2. `CombatAudioLayer` stores one Clip, Volume, Pitch, and Delay. `CombatAudioData` stores Master Volume plus a variable-length layer array. These data types do not play audio.
3. One Scene-local `CombatAudioPlayer` owns four 2D `AudioSource` channels, stops prior scheduled playback, maps valid layers, and calls `PlayScheduled()` from one `AudioSettings.dspTime + 0.020s` base. `OnDisable()` performs cleanup.
4. Ordinary and Perfect retain the exact independent 3-layer and 4-layer settings above; the Perfect fourth layer keeps its `0.030s` accent delay.
5. AudioMixer, EQ, randomized variations, pooling, Hitstop, Camera Impulse, and Gameplay Consequences remain outside this implementation.
6. The learner runtime-verified both result-specific VFX/SFX groups, no branch crossover or duplicate group per hit, preserved handled-hit prevention, preserved one-hit unblocked damage without Guard feedback, and a clean Console on 2026-09-02. Disable cleanup is implemented but was not recorded as a separate focused runtime test.
