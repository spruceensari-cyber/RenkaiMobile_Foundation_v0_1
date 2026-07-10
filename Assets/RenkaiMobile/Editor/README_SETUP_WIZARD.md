# Renkai Mobile First Playable Wizard

After pulling `feat/mobile-advanced-foundation` in Unity:

1. Wait for Unity compilation to finish.
2. Open the top menu `Renkai Mobile`.
3. Click `Build First Playable Scene`.
4. Confirm creation.
5. The wizard creates and saves `Assets/Renkai/Scenes/Maps/Renkai_TestRange.unity`.

The scene contains:
- arena floor
- graybox cover blocks
- directional light
- Player root
- CharacterController
- MobileFpsMotor
- PitchRoot
- Main Camera and AudioListener
- five target dummies with Health components

Next manual wiring milestone:
- mobile Canvas
- VirtualJoystick
- TouchLookArea
- MobileInputRouter
- HitscanWeapon
- WeaponDefinition asset
- ADS and recoil runtime components
- crosshair HUD
