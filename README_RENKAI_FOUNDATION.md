# RENKAI MOBILE — FOUNDATION v0.1

Target:
- iOS + Android
- Mobile-first competitive tactical FPS
- 5v5
- No respawn during a round
- Buy phase
- Attack / defense
- Plant / defuse objective: Spirit Core
- Precise gunplay + anime abilities
- Stylized premium visuals rather than photorealism

## Recommended folder direction

Assets/Renkai/
- Art/
  - Characters/
  - Weapons/
  - Environments/
  - VFX/
  - UI/
- Audio/
  - Music/
  - SFX/
  - Announcer/
- Data/
  - Characters/
  - Weapons/
  - Maps/
- Prefabs/
  - Characters/
  - Weapons/
  - Gameplay/
  - UI/
- Scenes/
  - Bootstrap/
  - Frontend/
  - Maps/
- Scripts/
  - Core/
  - Movement/
  - Combat/
  - Abilities/
  - Objectives/
  - Rounds/
  - Input/
  - Networking/
  - UI/
  - Bots/

## First playable milestone

1. One graybox map.
2. One player character.
3. One rifle.
4. One pistol.
5. One melee weapon.
6. Mobile move/look/fire/reload/jump/crouch controls.
7. 5 attackers and 5 defenders using temporary bots.
8. Buy -> Live -> PostRound loop.
9. Spirit Core plant/defuse flow.
10. 60 FPS target on a selected mid-range reference Android device.

## Graphics direction

Renkai should not chase expensive photorealism.
Build an anime tactical visual language:
- clean PBR surfaces
- strong silhouettes
- restrained neon accents
- readable enemy separation
- baked/static lighting where possible
- limited real-time shadow distance
- small number of expensive transparent effects
- LODs on characters and environment
- texture atlases for repeated props
- dynamic resolution / quality tiers

## Multiplayer boundary

The foundation includes `INetworkBridge` so offline prototype code does not become permanent multiplayer architecture.

Competitive release needs:
- authoritative match state
- validated fire requests
- server-side damage authority
- snapshot interpolation
- client-side prediction for local movement
- reconciliation
- lag compensation / rewind for hit validation
- reconnect flow
- anti-cheat telemetry

## Git workflow

Keep Renkai in a separate repository from Aura of Gods.

Suggested branches:
- main
- develop
- feat/mobile-controls
- feat/gunplay
- feat/round-system
- feat/spirit-core
- feat/networking
- feat/graphics-urp

Update pattern:
1. Pull latest branch in Unity project.
2. Make one focused feature branch.
3. Test in Editor and device build.
4. Open PR.
5. Merge only after compile + smoke test.
6. Tag playable milestones: v0.1, v0.2, v0.3...

Do not commit:
- Library/
- Temp/
- Logs/
- UserSettings/
- Build/
- Builds/
