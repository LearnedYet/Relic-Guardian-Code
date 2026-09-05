# Combat SFX Resource Tracking

Last verified: 2026-09-03.

## Status and Boundary

- Source package: local licensed `Melee Weapons Pack 1` in `RelicGuardianAssetLab`.
- Seven learner-selected WAV files and their original `.meta` files were copied into the ignored formal-project boundary `Assets/LocalLicensed/CombatSFX/Selected/Guard/` with GUIDs preserved.
- Eight learner-selected Attack WAV files and their original `.meta` files were copied into `Assets/LocalLicensed/CombatSFX/Selected/Attack/` with GUIDs preserved. They are classified as Attack1-3 Whoosh candidates, an Attack4 two-layer candidate, and a confirmed-hit two-layer candidate; none is connected yet.
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

## Attack Motion and Hit Resources

All paths below are relative to:

`Assets/LocalLicensed/CombatSFX/Selected/Attack/`

### Attack1-3 Whoosh Candidate Pool

The accepted Attack1-3 mappings are single-layer cues on the independent Scene-local `AttackAudio` player:

| Local path | GUID |
| --- | --- |
| `Attack1-3/SWSH_Swing 5 Normal 01_DDUMAIS_NONE.wav` | `889582865ef56b5439187d47303cf5d7` |
| `Attack1-3/SWSH_Swing 2 Normal 05_DDUMAIS_NONE.wav` | `5fd5d995ff41973498365c04bd2a3656` |
| `Attack1-3/SWSH_Swing 3 Small 04_DDUMAIS_NONE.wav` | `e35a1e57a8c1114478621bd4baa153cd` |
| `Attack1-3/SWSH_Swing 2 Normal 04_DDUMAIS_NONE.wav` | `9c89ef6104e38234c815fff0bd923686` |

| Attack | Clip | Master | Volume | Pitch | Delay | Event time |
| ---: | --- | ---: | ---: | ---: | ---: | ---: |
| 1 | `SWSH_Swing 2 Normal 05_DDUMAIS_NONE.wav` | `1` | `0.85` | `1` | `0s` | `0.23050807` |
| 2 | `SWSH_Swing 2 Normal 04_DDUMAIS_NONE.wav` | `1` | `1` | `1` | `0s` | `0.21698608` |
| 3 | `SWSH_Swing 5 Normal 01_DDUMAIS_NONE.wav` | `1` | `1` | `1` | `0s` | `0.20805433` |

`SWSH_Swing 3 Small 04_DDUMAIS_NONE.wav` remains an unused local candidate.

### Attack4 Two-Layer Candidate

| Layer role | Local path | GUID |
| --- | --- | --- |
| Main large swing | `Attack4/SWSH_Swing 4 Normal 16_DDUMAIS_NONE.wav` | `d01a11c76799d514883c40e37c10fcc2` |
| Small accent | `Attack4/SWSH_Swing 1 Small 12_DDUMAIS_NONE.wav` | `d1b16c35016f7d745853fa79f4584ac5` |

The accepted implementation treats these as two sequential authored motion cues, not one delayed-layer request:

| Role | Master | Volume | Pitch | Delay | Event | Event time |
| --- | ---: | ---: | ---: | ---: | --- | ---: |
| Windup / raised-hand accent | `1` | `0.3` | `1.05` | `0s` | `PlayWeaponWindup(3)` | `0.05673332` |
| Main swing | `1` | `1` | `1.06` | `0s` | `PlayWeaponWhoosh(3)` | `0.3182363` |

Separating the Events keeps both cues pose-authored and lets attack-step validation reject the future main swing if Attack4 is cancelled after Windup.

### Confirmed Attack Hit Two-Layer Candidate

| Layer role | Local path | GUID |
| --- | --- | --- |
| Sword impact | `Hit/SWSH_Sword Slash Impact V1 Assorted 18_DDUMAIS_NONE.wav` | `a12b51e550e35b247811a0b78c3f49df` |
| Flesh/gore body | `Hit/GOREFlsh_Flesh And Gore Assorted 08_DDUMAIS_NONE.wav` | `b0a4b84c8a8b7de439d9bf2d474da3a2` |

The earlier screenshots and Lab state contained temporary audition values; the tables above now record the accepted runtime Attack Motion mappings. Hit cue values remain candidates and must begin only after the existing gameplay route confirms a target and applies the hit. Whoosh and Hit audio remain separate.

## Implemented Guard Integration

1. `GuardResult` remains the authoritative Ordinary/Perfect classification; `PlayerGuardPresentation` selects one corresponding `CombatAudioData` after Gameplay Resolution.
2. `CombatAudioLayer` stores one Clip, Volume, Pitch, and Delay. `CombatAudioData` stores Master Volume plus a variable-length layer array. These data types do not play audio.
3. One Scene-local `CombatAudioPlayer` owns four 2D `AudioSource` channels, stops prior scheduled playback, maps valid layers, and calls `PlayScheduled()` from one `AudioSettings.dspTime + 0.020s` base. `OnDisable()` performs cleanup.
4. Ordinary and Perfect retain the exact independent 3-layer and 4-layer settings above; the Perfect fourth layer keeps its `0.030s` accent delay.
5. AudioMixer, EQ, randomized variations, pooling, Hitstop, Camera Impulse, and Gameplay Consequences remain outside this implementation.
6. The learner runtime-verified both result-specific VFX/SFX groups, no branch crossover or duplicate group per hit, preserved handled-hit prevention, preserved one-hit unblocked damage without Guard feedback, and a clean Console on 2026-09-02. Disable cleanup is implemented but was not recorded as a separate focused runtime test.
