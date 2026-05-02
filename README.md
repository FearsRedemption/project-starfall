# Project Starfall

Project Starfall is an early-stage third-person Unity game focused on readable, momentum-based movement and combat foundations. The current goal is to build a strong controller, clear feedback loop, and practical test space before adding animation, VFX polish, or production art.

## Current Direction

Project Starfall is aiming for a grounded, readable, slightly stylized action feel. Movement should feel physics-influenced rather than arcade-snappy, with clear player intent and predictable momentum.

Reference direction:

- Grounded: movement readability and scale clarity
- Elden Ring: combat readability and silhouette clarity
- Valheim: stylized realism balance

Core principles:

- Readability over realism
- Stylized realism, not cartoony and not photoreal
- Clean silhouettes and controlled color
- Gameplay clarity before spectacle
- Velocity is influenced, not directly overwritten whenever possible

## Engine

- Unity 6.x
- Universal Render Pipeline (URP)
- Rigidbody-based player movement
- Third-person camera-relative movement
- Keyboard and mouse input

## Controls

- `WASD`: Move
- `Mouse`: Look
- `Space`: Jump
- `Left Shift`: Sprint
- `Left Mouse Button`: Attack
- `Left Alt`: Dodge
- Double-tap `W`, `A`, `S`, or `D`: Directional dodge

## Movement Spec

The player movement is designed around Rigidbody velocity rather than CharacterController movement.

Ground movement:

- Camera-relative movement
- Player yaw aligns with camera yaw, even while idle
- Sprint multiplies movement speed
- Ground acceleration is separate from air acceleration

Jump rules:

- Jump preserves horizontal momentum at takeoff
- Jump applies vertical force only
- Standing jump should go straight up
- Moving jump should carry current horizontal velocity
- Sprint jump should preserve sprint momentum
- No ground damping should alter horizontal velocity on the exact jump frame

Air control:

- Air movement is weaker than ground movement
- Player can gradually adjust trajectory
- Player cannot instantly stop, snap direction, or reverse momentum
- Horizontal air speed is clamped to prevent speed stacking

Validation checklist:

- Standing jump is vertical
- Forward jump preserves forward momentum
- Releasing input midair continues trajectory
- Opposite input midair gradually redirects without snapping
- Sprint jump carries sprint speed

## Combat Foundation

The combat system is intentionally minimal right now.

Current behavior:

- Left click triggers a short-range attack
- Attack uses cooldown/lockout
- Hit detection uses a sphere cast
- Hit confirmation is printed to the Unity console
- A scene-authored slash cue appears on attack
- Damaged targets flash briefly
- Damageable objects log HP and death

Current non-goals:

- No attack animations
- No combos
- No VFX polish
- No full enemy combat model

## Training Enemy

A simple training enemy exists to validate the gameplay loop.

Current behavior:

- Idle until the player enters detection range
- Chase the player
- Stop near the player
- Can take damage
- Can die

Purpose:

- Validate movement into positioning
- Validate attack hit detection
- Validate damage feedback
- Provide a basic combat target while systems are still being built

## Dodge Foundation

Current dodge behavior:

- `Left Alt` triggers a dodge
- Double-tapping a movement key triggers a dodge
- Dodge direction uses the current movement input vector
- If there is no movement input, dodge uses forward direction
- Dodge has fixed duration, speed, and cooldown
- Dodge consumes stamina

## Feedback Pass

Current feedback systems:

- Styled health and stamina HUD
- Sprint drains stamina
- Dodge spends stamina
- Stamina regenerates when not sprinting or dodging
- Enemy contact attacks damage the player
- Enemy attack flashes briefly for readability
- Damageable targets support scene-authored health bars when assigned

Future dodge work:

- Tune responsiveness
- Add optional invulnerability frames
- Add grounded dust or short motion streak
- Add animation support when character animation exists

## Visual Style

Project Starfall's target visual identity is grounded, readable, and slightly stylized.

Rendering:

- URP Lit materials
- Physically-based material response
- Muted base palette
- Light, intentional post processing only

Lighting:

- Directional main light
- Soft shadows
- Use lighting to guide attention

Color:

- Environment should use natural, muted tones
- Gameplay accents should be reserved and readable
- Damage/danger should use warmer reds and oranges
- Positive states should use cooler blues or whites

Avoid:

- Neon-heavy visuals
- Overblown bloom
- Heavy motion blur
- Chromatic aberration
- Plastic-looking materials
- VFX-saturated combat
- Anime or hyper-real presentation

## Character Visual Direction

The current player visual is a simple humanoid blockout made from scene mesh parts. It is not final art.

Goals for the player:

- Clear silhouette from mid-distance
- Distinct torso, head, arms, legs, and gear shapes
- Muted cloth/leather/armor colors
- Reserved accent color for readability
- Future animation should reinforce momentum and direction changes

The current setup keeps the original capsule collider and Rigidbody for stable movement while hiding the pill mesh. The visible player is an authored scene hierarchy made from simple mesh parts under the Player object.

## Blockout Arena

The scene includes authored blockout geometry for early movement and combat validation. The player, arena, HUD, combat targets, and training enemy are scene objects; gameplay scripts control behavior rather than spawning the play space after launch.

Current arena elements:

- Larger movement floor
- Boundary walls
- Sprint lane
- Low platform
- High platform
- Jump gap platform
- Ramp
- Damageable combat targets

Purpose:

- Test sprinting and momentum
- Test jumping and air control
- Test dodge direction and distance
- Test basic attacks and damageable targets
- Provide enough layout variety without cluttering readability

## Development Order

The intended development order is:

1. Complete and validate jump physics
2. Build basic attack scaffold
3. Add one training enemy
4. Add `Left Alt` dodge
5. Add double-tap dodge extension
6. Add tuning and feedback pass
7. Add animations, VFX, and art polish after mechanics feel correct

## Current Implementation Notes

Important scripts:

- `Assets/Scripts/PlayerMove.cs`: Rigidbody movement, jump, air control, sprint, dodge
- `Assets/Scripts/PlayerHealth.cs`: Player health and hit flash feedback
- `Assets/Scripts/PlayerHud.cs`: Updates scene-authored health and stamina HUD fill images
- `Assets/Scripts/PlayerAttack.cs`: Basic attack, hit detection, and scene-authored slash cue timing
- `Assets/Scripts/Damageable.cs`: Health, damage logging, death, optional scene-authored health bars, and hit flash feedback
- `Assets/Scripts/EnemyAttackCue.cs`: Enemy attack flash
- `Assets/Scripts/TrainingEnemy.cs`: Enemy chase and contact attack behavior

Important scene:

- `Assets/Scenes/SampleScene.unity`

## Testing Notes

Run Play Mode and verify:

- The game opens into `Assets/Scenes/SampleScene.unity`
- Player appears as a humanoid blockout, not a pill
- Movement is camera-relative
- Sprint increases speed
- Standing jump is vertical
- Moving jump preserves momentum
- Opposite input in air redirects gradually
- `Left Alt` dodge works
- Double-tap directional dodge works
- A styled health and stamina HUD appears in Play Mode
- Sprint and dodge spend stamina
- Left click hits nearby targets and shows a slash cue
- Damage logs appear in the console
- Training enemy detects, chases, flashes on attack, and damages the player

## Project Status

Project Starfall is still early, but the current build is moving from raw prototype toward a playable foundation. Systems are intentionally simple and should be treated as foundations for feel testing rather than finished gameplay.
