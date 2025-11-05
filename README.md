# 3D Arena Game

## Overview
3D Arena Game is a Unity-based third-person action game where the player battles waves of enemies in an arena environment. The project demonstrates core gameplay mechanics such as player movement, shooting, enemy AI, health and item systems, and a responsive UI. The game is designed for educational purposes and showcases modular, maintainable C# scripting in Unity.

## Features
- **Player Controls:** Smooth third-person movement, shooting, and health management.
- **Enemy AI:** Multiple enemy types (melee, ranged, exploder) with unique behaviors and attack patterns.
- **Combat System:** Projectile and melee combat, damage calculation, and hit effects.
- **Item System:** Health pickups and damage-boosting potions with visual and audio feedback.
- **UI:** Dynamic score, health bar, enemies remaining, and booster timer displays.
- **Audio:** Background music, sound effects for actions, and UI feedback.
- **Scene Management:** Main menu, level transitions, and fade effects.
- **Singleton Managers:** Centralized management for game state, audio, player stats, and effects.

## Project Structure
```
3D-Arena-Game/
├── Assets/
│   ├── Animation/
│   ├── Audio/
│   ├── Camera/
│   ├── Enemy/
│   ├── Gameplay/
│   ├── GameplayUI/
│   ├── Items/
│   ├── MainMenu/
│   ├── Player/
│   ├── Prefabs/
│   ├── Scenes/
│   ├── Scripts/
│   ├── Sprites/
│   └── TextMesh Pro/
├── ProjectSettings/
├── Packages/
├── UserSettings/
├── 3D Arena Game.sln
├── 3D-Arena-Game.sln
├── Assembly-CSharp.csproj
├── LICENSE
└── README.md
```

## Key Scripts
- `GameManager.cs`: Handles game state, score, enemy tracking, and UI updates.
- `PlayerStats.cs`: Manages player health persistence across scenes.
- `EnemyBase.cs` & derived: Base and specialized enemy behaviors.
- `Health.cs`, `HealthBarUI.cs`: Player health logic and UI.
- `SoundManager.cs`, `MusicManager.cs`: Audio management for SFX and music.
- `SceneFader.cs`, `MainMenu.cs`: Scene transitions and menu navigation.

## How to Play
1. **Start the Game:** Launch the project in Unity and play from the Main Menu.
2. **Controls:**
	- Move: WASD or Arrow Keys
	- Shoot: (Left)Mouse Button
	- Pick up items: Walk over them
3. **Objective:** Defeat all enemies in the arena. Use pickups to restore health or boost damage.
4. **Win/Lose:** The game resets player health and UI after each win or loss, allowing replay.

## Setup Instructions
1. Open the project in Unity (recommended version: 2021 .3 LTS or later).
2. Ensure all packages in `Packages/manifest.json` are installed.
3. Assign required references in the Unity Editor (e.g., UI elements, audio clips, prefabs).
4. Press Play to start the game.

## Customization
- Add new enemy types by extending `EnemyBase`.
- Adjust player or enemy stats in the Inspector.
- Replace or add audio clips in the `Audio` folder and assign them in the `SoundManager`.
- Modify UI by editing Canvas elements in the scene.

## Credits
- Developed by 許如來, 溫裕豐, 陳成金, 煜欣.
- Assets and code are for educational use.

## License
See `LICENSE` for details.