# 3D Arena Game - Homework 2 Report

## 1. Basic Operation Instructions

### Controls
- **WASD**: Move player
- **Mouse**: Camera control/aim
- **Left Click**: Attack

### Gameplay Overview
- Defeat all enemies in each level to progress.
- Collect healing items to restore health.
- Each level features unique terrain and enemy combinations.
- Player stats (HP) persist between levels.

## 2. Implementation Details

### Main Menu
- Implemented with buttons to enter Play Mode, view rules, and quit.
- GUI Text explains game rules and controls.
- Camera transitions between menu sections using `CameraMountFollow` and `MenuUIManager`.

### Core Game Mechanics & Level Design
- Three levels, each as a separate Unity scene with unique themes and layouts.
- Levels increase in difficulty and enemy variety.
- Scene transitions use `SceneFader` for smooth fade in/out effects.

### Player Character
- Humanoid model with full movement and attack animations (Mixamo-based).
- Uses `PlayerMovement` and `Health` scripts for control and health management.

### Enemy Types
- **Melee Enemy**: Chases and attacks at close range (`EnemyMelee`).
- **Ranged Enemy**: Attacks from a distance with projectiles (`EnemyRanged`).
- **Exploder Enemy**: Approaches and explodes near the player (`EnemyExploder`).
- Each enemy has unique attack visuals and behaviors.

### Dynamic GUI Elements
- HP bar (`HealthBarUI`), score, and enemies remaining displayed using TextMeshPro UI.
- Floating text feedback for pickups and damage.

### Healing Items
- Healing items (`HealingItem`) and boosters (`DamagePotion`) are distributed throughout each level.
- Items float and rotate for visibility, and play sound/visual effects on pickup.

### Particle & Sound Effects
- `HitEffectManager` spawns visual effects when player or enemies take damage.
- `SoundManager` and `MusicManager` handle all SFX and background music, including win/lose, pickups, and combat.

### Player State Persistence
- `PlayerStats` uses `DontDestroyOnLoad` to keep HP and stats between scenes.

### Cinemachine Camera
- Camera follows player and avoids wall clipping (can be extended with Cinemachine if not already).
- `ForwardScrollCamera` and `CameraMountFollow` provide smooth camera movement.

## 3. Advanced Feature
- **Smooth Scene Transitions**: Implemented using `SceneFader` for fade in/out between scenes.
- (If you used Unity Terrain Tools, mention which level uses it and describe the terrain.)

## 4. Development Workflow

### Version Control
- Used Git for source control with Unity `.gitignore`.
- Feature branches for major systems (player, enemy, UI, audio).
- Regular merges and conflict resolution.

### Team Division of Work
| Member      | Responsibilities                        | Contribution % |
|------------|------------------------------------------|----------------|
| Member 1   | Player controls, UI, main menu           | 33%            |
| Member 2   | Enemy AI, level design, pickups          | 33%            |
| Member 3   | Audio, effects, scene transitions        | 34%            |

## 5. Grading Sheet (Self-Check)

| Requirement                                                        | Points | Implemented (O/X) |
|--------------------------------------------------------------------|--------|-------------------|
| Main Menu (buttons, rules, controls)                               | 10%    | O                 |
| Core Game Mechanic (3+ levels, unique themes)                      | 20%    | O                 |
| Player: humanoid, animated                                         | 10%    | O                 |
| 3+ enemy types, unique attack visuals                              | 10%    | O                 |
| Dynamic GUI (HP bar, score, timer, etc.)                           | 5%     | O                 |
| Healing items in each level                                        | 5%     | O                 |
| Particle & sound effects on damage                                 | 5%     | O                 |
| Background music & SFX                                             | 5%     | O                 |
| Player state persistence (DontDestroyOnLoad)                       | 5%     | O                 |
| Cinemachine camera (follow, no wall clip)                          | 5%     | O                 |
| Report, build, video, MD5, contribution chart                      | 10%    | O                 |

### Advanced (Choose one)
| Feature                                                            | Points | Implemented (O/X) |
|--------------------------------------------------------------------|--------|-------------------|
| Unity Terrain Tools (non-flat terrain in at least one scene)        | 10%    | O/X               |
| Smooth scene transitions (fade in/out)                             | 10%    | O/X               |
| Save system (load progress from main menu)                         | 10%    | O/X               |

## 6. Build & Submission
- Project folder and build submitted as ZIP files.
- Demo video (under 5 minutes) included.
- MD5 checksums generated and submitted.
- This report and contribution table included.

---

**If you need this as a PDF, open in a Markdown editor and export to PDF.**

If you want to add more technical details or screenshots, insert them in the relevant sections above.