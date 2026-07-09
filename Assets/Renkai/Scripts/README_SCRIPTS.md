# Renkai Scripts v0.1

This folder is the first mobile tactical-FPS foundation layer.

Current modules:
- Core team identity and bootstrap
- Character-controller movement
- Touch movement joystick
- Touch look area
- Hitscan rifle foundation
- Health / damage interfaces
- Round phases and score
- Spirit Core plant-objective skeleton
- Network abstraction boundary

Important:
The current `LocalNetworkBridge` is intentionally NOT real multiplayer.
Do not trust client hit/damage results in a competitive release.
Replace the bridge with a server-authoritative transport/session implementation.
