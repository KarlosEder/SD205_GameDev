================================================================================
SD205 GAME PROJECT
================================================================================

TEAM MEMBERS:
- Jack Pengelly (Scrum Master & Lead Programmer)
- Karlos Eder (Product Owner & Game Director)

================================================================================
GAME DESCRIPTION
================================================================================

A first-person shooter wave-based survival game where players fight against 
increasingly difficult waves of zombies in an urban environment. Features 
Enemy AI with NavMesh pathfinding, dynamic wave spawning, XP progression, 
rogue-like items, and 9 unique zombie variations.

================================================================================
INSTALLATION INSTRUCTIONS - No install
================================================================================

SYSTEM REQUIREMENTS:
- Operating System: Windows 10/11 (64-bit)
- Unity Version: 6.3 LTS (for source code)
- RAM: 4GB minimum, 8GB recommended
- Storage: 500MB available space
- Graphics: DirectX 11 compatible graphics card

CONTROLS:
- WASD: Move
- Mouse: Look around
- Left Click: Shoot
- R: Reload
- Space: Jump
- Shift: Sprint
- ESC: Pause Menu
- T: Spawn Next wave (ontop of current wave)

================================================================================
HOW TO PLAY
================================================================================

OBJECTIVE:
Survive as many waves as possible by eliminating all zombies in each wave.

GAMEPLAY:
1. Start the game from the main menu
2. Zombies will spawn at various points around the city
3. Eliminate all zombies to complete the wave
4. Gain XP from zombie kills to level up
5. Each wave increases in difficulty
6. Game ends when player health reaches zero

ZOMBIE TYPES:
- Dad Zombies (3 variants): Standard enemies
- Mum Zombies (3 variants): Standard enemies  
- Son Zombies (3 variants): Standard enemies

WAVE PROGRESSION:
- Wave 1-2: Increasing zombie count
- Wave 3+: Fewer zombies but higher damage output

================================================================================
FEATURES IMPLEMENTED
================================================================================

PLAYER SYSTEMS:
- First-person movement with responsive controls
- Weapon system with shooting mechanics and recoil
- Health and shield dual-layer defense
- XP and leveling progression

ENEMY AI:
- 3-state finite state machine (Idle, Chase, Attack)
- NavMesh pathfinding for intelligent navigation
- Combo attack system
- 9 zombie variations across 3 families

GAME SYSTEMS:
- Wave-based spawning with difficulty scaling
- Dynamic UI with real-time updates
- Damage feedback with floating numbers
- Comprehensive audio (SFX and music)

ENVIRONMENT:
- City level themed as "day one of outbreak"
- Multiple combat areas with varied terrain
- Strategic spawn points

================================================================================
KNOWN ISSUES / LIMITATIONS
================================================================================

- Performance: Some lag during initial game load
- Performance: Minor frame drops when spawning multiple zombies simultaneously
- Current build has 3 test waves (system supports unlimited)
- Single weapon type implemented

================================================================================
SOURCE CODE
================================================================================

GitHub Repository: https://github.com/KarlosEder/SD205_GameDev

REPOSITORY CONTENTS:
- Complete Unity project source files
- All C# scripts for game logic
- Asset files (models, animations, audio)
- Project documentation

NOTE: Repository link is also included in "GitHub_Repository_Link.txt"

To open the project:
1. Clone or download the repository
2. Open Unity Hub
3. Add the project folder
4. Open with Unity 6.3 LTS (or compatible version)

================================================================================
DEVELOPMENT NOTES
================================================================================

TECHNOLOGIES USED:
- Unity 6.3 LTS (Game Engine)
- C# (Programming Language)
- Visual Studio Code (IDE)
- Git/GitHub (Version Control)
- Mixamo (Animation Assets)
- NavMesh (AI Pathfinding)

METHODOLOGIES:
- Scrum framework with 5 sprints
- 8-week active development cycle
- Role-based specialization (AI/Player Systems)

================================================================================
CREDITS
================================================================================

DEVELOPMENT:
- Jack Pengelly: Enemy AI, Animations, Level Design, Wave Spawner
- Karlos Eder: Player Systems, UI/UX, Weapons, Audio, XP System

ASSETS:
- Animations: Mixamo (Adobe)
- 3D Models: Various online sources

================================================================================
