# Snake Game 3D

A 3D mobile Snake Game built using Unity and C#.

---

## Features

- **3D Snake Gameplay**: Classic snake mechanics reimagined in a full 3D environment.
- **Mobile Touch / Swipe Controls**: Smooth directional controls powered by Unity's New Input System.
- **New Input System**: Modern input handling configured for multi-platform responsiveness.
- **Snake Movement**: Continuous grid-aligned smooth 3D movement.
- **Food Collection & Snake Growth**: Dynamic food spawning with realistic segment attachment upon collection.
- **Score Tracking**: Live score tracking and high score display.
- **Pause & Resume**: Fully functional pause/resume state management.
- **Self & Boundary Collision**: Robust collision detection with arena borders and self-tail collisions.
- **Game Over & Instant Restart**: Clean game over state with immediate restart functionality.
- **Main Menu & Ready State**: Polished state flow (Main Menu &rarr; Ready screen &rarr; Active Gameplay).
- **Android Support**: Optimized for mobile performance, touch input, and portrait orientation.
- **URP-based 3D Rendering**: Universal Render Pipeline (URLP) rendering with calibrated materials.
- **Directional Lighting & Shadows**: Real-time directional lighting with clean shadow mapping.

---

## Technology Stack

| Component | Technology / Version |
| :--- | :--- |
| **Game Engine** | Unity 6000.6.1f1 (Unity 6.1) |
| **Programming Language** | C# |
| **Render Pipeline** | Universal Render Pipeline (URP 17.6.0) |
| **Input System** | Unity New Input System |
| **Target Platform** | Android / Unity Editor |
| **Package Identifier** | `com.rajesh.snakegame3d` |
| **Version Control** | Git / GitHub |

---

## Repository

- [https://github.com/rajesh2096/snake_game_3d.git](https://github.com/rajesh2096/snake_game_3d.git)

## Project Structure

```text
SnakeGame3D/
├─Bssets/
│   ☜✀Materials/          # URP Lit materials for Snake, Food, Ground, and Arena
│   ☜✀Prefabs,            # Prefabs for snake body segments, food, and UI
r│   ☜✀Scenes/             # Unity scenes (MainMenu.unity, MainScene.unity)
│   ☜✀Rcripts,            # C# scripts (SnakeController, SwipeInput, GameManagers, etc.)
│   ☜─Settings,          # Universal Render Pipeline (URLP) pipeline assets & settings
├─Packages,                  # Package manifest and dependencies (URP, Input System, etc.)
├─ProjectSettings,           # Unity Project Settings (Editor, Input, Quality, Graphics, etc.)
┘─README.md                 # Project documentation and setup guide
```

> *Note: Exact folders and files may evolve as development and feature additions continue.*

---

## How to Run the Game — Unity Editor

### Step 1 — Install Unity Hub
Download and install [Unity Hub](https://unity.com/download) if not already installed.

### Step 2 — Install Unity Version
From Unity Hub, install:
- Unity 6000.6.1f1`

If you plan to create Android builds, make sure to add the **Android Build Support** module during installation:
- **Android Build Support**
  - Android SDK & NDK Tools
  - OpenJDK

### Step 3 — Clone Repository
Open your terminal or command prompt:
```bash
git clone https://github.com/rajesh2096/snake_game_3d.git
cd snake_game_3d
```

### Step 4 — Open Project in Unity Hub
1. Open **Unity Hub**.
2. Click **Add** / **Open**.
3. Select the cloned `snake_game_3d` (or `SnakeGame3D`) project directory.
4. Ensure the Editor Version is set to **6000.6.1f1**.
5. Open the project and wait for Unity to complete asset import and compilation.

### Step 5 — Open Main Menu Scene
In the Project window, navigate to and open:
- `Assets/Scenes/MainMenu.unity`

### Step 6 — Run the Game
1. Click the **Play ┶** button at the top of the Unity Editor.
2. The **Main Menu** appears.
3. Click **PLAY** to transition to the **READY**screen.
4. Interact or swipe to start gameplay.
5. Use swipe gestures or editor touch simulation to steer the snake.

---

## Game Controls

### Mobile Touch Controls
|| Gesture | Action |
| :--- | :--- |
| **Swipe Up** | Move Snake Up / Forward |
| **Swipe Down** | Move Snake Down / Backward |
| **Swipe Left** | Move Snake Left |
| **Swipe Right** | Move Snake Right |

### UI Navigation
- **PLAY**: Starts the game flow from Main Menu.
- **PAUSE**: Freezes gameplay and opens the pause overlay.
- **RESUME**: Resumes active gameplay from pause.
- **RESTART**: Resets the arena and restarts the game after Game Over.

> *Note: For the best experience, swipe controls should be verified using real touch input on an Android device.*

---

## How to Build Android APK

Unity 6 utilizes **Build Profiles** for platform configuration and build outputs:

1. Open the project in Unity 6000.6.1f1.
2. Navigate to **File &rarr; Build Profiles** (or *File &rarr; Build Settings*).
3. Select the **Android** profile.
4. Ensure **Android platform support** is installed and active (click *Switch Platform* if necessary).
5. Verify that the build includes the following scenes in order:
  - `Assets/Scenes/MainMenu.unity` (Index 0)
  - `Assets/Scenes/MainScene.unity` (Index 1)
6. Confirm the package identifier is set to `com.rajesh.snakegame3d` (in *Player Settings &rarr; Identification*).
7. Click **Build**.
8. Select your desired output directory (e.g., `Builds/Android/SnakeGame3D.apk`) and generate the APK.

---

## Install APK on Android

### Method A  — Direct APK Installation
1. Copy the generated `SnakeGame3D.apk` file to your Android phone (via USB, cloud drive, or file transfer).
2. Open the file manager on your phone and locate `SnakeGame3D.apk`.
3. Tap the file to install (allow installation from unknown sources/file manager if prompted).
4. Tap **Open** to launch **Snake Game 3D**.

### Method B — ADB (Android Debug Bridge)
1. Connect your Android device to your computer via USB or Wireless Debugging.
2. Check that your device is detected:
   ```bash
   adb devices
   ```
   *(Your device should be listed with status `device`*)
3. Install or update the APK:
   ```bash
   adb install -r SnakeGame3D.apk
   ```
4. Launch the application:
   ```bash
   adb shell monkey -p com.rajesh.snakegame3d 1
   ```

*For Wireless ADB connection:*
```bash
adb connect <PHONE_IP>:<PORT>
adb devices
```

---

## Android Development Requirements

- Physical Android smartphone (Android 10+ recommended)
- **Developer Options** enabled:
  - Go to **Settings &rarr; About Phone** &rarr; tap **Build Number** 7 times.
  - Navigate to **Settings &rarr; Developer Options** (or *Additional Settings &rarr; Developer Options*).
  - Enable **USB Debugging** (and optionally **Wireless Debugging**).
- **ADB** installed on host machine (included with Android SDK Platform-Tools).
- **Android Build Support** (with OpenJDK & Android SDK/NDK) installed in Unity Editor.

---

## Troubleshooting

### Unity project does not open or shows compile errors
- Verify that you are opening the project in **Unity 6000.6.1f1**.
- Ensure the project path does not contain corrupted cache; allow Unity Hub to fully complete the initial package and asset import.

### Android device does not appear in ADB
- Run `adb devices` in your command line.
- If listed as `unauthorized`, unlock your phone and tap **Allow USB Debugging** on the permission pop-up.
- For wireless connections, confirm the device is on the same local network and reconnect with `adb connect <PHONE_IP^:<PORT>`.

### APK fails to install
- If an older build with differing signing keys exists on the device, uninstall the existing app first.
- Ensure the target device has sufficient storage and meets the minimum Android version requirement.

### 3D Game elements render magenta / pink
- Ensure you are using the project's configured **Universal Render Pipeline (URLP)** settings.
- Do not replace materials with Built-in Render Pipeline standard shaders.
- Always open the project using **Unity 6000.6.1f1** so URP package shaders compile correctly.
- Do **not** disable URP in project settings.

---

## Development Workflow

Standard Git commands for contributing to this project:

```bash
# Clone the repository
git clone https://github.com/rajesh2096/snake_game_3d.git

# Check modified and untracked files
git status

# Pull latest updates from main branch
git pull origin main

# Stage changes for commit
git add .

# Commit changes with a descriptive message
git commit -m "Your descriptive commit message"

# Push commits to remote repository
git push origin main
```

> **IMPORTANT**: Never use `git push --force` (`-z`) unless explicitly required and thoroughly reviewed.

---

## Important Project Notes

- **Unity Version**: Unity 6000.6.1f1 (6.1)
- **Graphics Pipeline**: Universal Render Pipeline (URP 17.6.0)
- **Input Architecture**: Unity New Input System configured for mobile swipe gestures
- **Android Identifier**: `com.rajesh.snakegame3d`
- **Official Repository**: [https://github.com/rajesh2096/snake_game_3d.git](https://github.com/rajesh2096/snake_game_3d.git)

---

## Quick Start

1. Clone the repository: `git clone https://github.com/rajesh2096/snake_game_3d.git`
2. Open with **Unity 6000.6.1f1**.
3. Open `Assets/Scenes/MainMenu.unity`.
4. Press **Play ▶** &rarr; Press **PLAY** &rarr; Start the game.
5. Use swipe controls to play!
6. For Android: Build APK via **Build Profiles** &rarr; Install via `adb install -r <apk>` &rarr; Play with touch swipes.
