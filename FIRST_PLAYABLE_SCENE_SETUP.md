# First Playable Scene Setup

Create scene: Assets/Renkai/Scenes/Maps/Renkai_TestRange.unity

1. Add a Plane and simple Cube cover blocks.
2. Add Player root with CharacterController.
3. Add MobileFpsMotor to Player.
4. Add YawRoot child, then PitchRoot child, then Camera child.
5. Add MobileInputRouter and connect motor, joystick, look area, yaw root, pitch root and weapon.
6. Create Canvas in Screen Space Overlay.
7. Add left movement joystick using VirtualJoystick.
8. Add right transparent look panel using TouchLookArea.
9. Add Fire, Reload, Jump and Crouch buttons wired to MobileInputRouter.
10. Add a weapon object with HitscanWeapon, WeaponDefinition and RuntimeRecoil.
11. Add AdsController to the camera rig and connect an ADS button.
12. Add CrosshairController and four RectTransform arms to the HUD.
13. Add enemy capsules with Health and Head-tagged child colliders.
14. Use SimpleCombatBot only as a temporary offline sandbox bot. Replace it before competitive networking.

Target first test:
- move on device
- swipe look
- hold fire
- reload
- crouch
- jump
- ADS FOV transition
- recoil response
- body damage and headshot damage
- crosshair expansion and recovery
