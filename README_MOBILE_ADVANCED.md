# Renkai Mobile Advanced Foundation

Target: iOS + Android competitive tactical FPS.

This branch adds a device-scalable gameplay foundation focused on:
- mobile-first touch input without uGUI/EventSystem dependency
- character-controller movement
- FPS camera look
- hitscan rifle foundation
- recoil and spread hooks
- hit feedback hooks
- animation-event compatible weapon timing
- scalable quality tiers
- battery and thermal-friendly defaults

This is an engineering foundation, not a finished AAA game. Character models, polished animation clips, weapon meshes, authored VFX, sounds, map art and online server authority still need production assets and iteration.

Recommended next milestones:
1. First playable test range
2. Weapon inventory + ADS + recoil pattern tuning
3. Animator controllers and authored animation clips
4. VFX Graph / Shader Graph pass
5. Bot combat sandbox
6. 5v5 authoritative networking prototype
7. Device lab profiling and quality-tier tuning
