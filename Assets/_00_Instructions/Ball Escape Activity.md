# Ball Escape — Mini Game Activity
**Course:** CS Elec 2B — Game Programming I
**Type:** Individual Activity
**Engine:** Unity 2D (Built-In Render Pipeline)

---

## Overview

You are given a partially built game called **Ball Escape**.

**What is already built for you:**
- Main Menu (with name input and leaderboard UI)
- Player ball with movement
- Enemy with patrol and chase behavior
- Scene loading system (`SceneField.cs`, `S08_MainMenuManager.cs`, `S09_SceneLoadTrigger.cs`)
- HUD with timer and penalty popup

**What you need to build:**
1. Custom Gizmos on the enemy
2. PlayerPrefs — save the player's name
3. JSON save — record and save the player's finish time
4. Leaderboard — read the JSON and display it
5. Five rooms with async scene loading and unloading

**Goal:** The player ball must escape through all 5 rooms. When they reach the end of Room 5, their time is saved to a JSON file. The leaderboard on the main menu reads that file and displays the top scores.

---

## Setup

Do this before anything else.

### 1. Import the package
1. Open Unity Hub → create a new **2D (Built-In Render Pipeline)** project
2. In Unity: **Assets → Import Package → Custom Package**
3. Select `Ball_Escape.unitypackage` → click **Import All**

### 2. Import TextMeshPro Essentials
The UI uses TextMeshPro. If you skip this step, all text fields will show errors.

1. In the Hierarchy: **right-click → UI → Text - TextMeshPro**
2. A popup will appear: **"TMP Essentials"** — click **Import TMP Essentials**
3. Wait for the import to finish, then delete the Text object you just created — it was only needed to trigger the popup

### 3. Fix the Input System conflict
The project uses the **old Input System** (`Input.GetAxisRaw`). Unity 6 defaults to the New Input System only — player movement will not work until you change this.

1. **Edit → Project Settings → Player**
2. Scroll to **Active Input Handling**
3. Change to **Both**
4. Unity will restart — click **Apply**

### 3. Add scenes to Build Settings
1. **File → Build Settings**
2. Click **Add Open Scenes** for each scene — open and add them one by one:
   - `SC00_MainMenu`
   - `SC01_Room1` through `SC05_Room5`
3. Make sure `SC00_MainMenu` is at **index 0**

---

## Part 1 — Custom Gizmos on the Enemy

Your enemy already patrols and chases. Your task is to make the enemy's behavior **visible in the Scene view** using Gizmos so you (and other developers) can tune it without guessing.

### What to add
Add `OnDrawGizmos()` or `OnDrawGizmosSelected()` to your enemy script and draw at least **three** of the following:

| Gizmo Method | What it draws |
|---|---|
| `Gizmos.DrawWireSphere(center, radius)` | A wireframe circle — good for radii |
| `Gizmos.DrawLine(from, to)` | A line between two points |
| `Gizmos.DrawWireCube(center, size)` | A wireframe box |
| `Gizmos.DrawRay(origin, direction)` | A ray from a point in a direction |
| `Gizmos.color = Color.xxx` | Set the color before drawing |

### Hints
- `OnDrawGizmos()` always draws. `OnDrawGizmosSelected()` only draws when the object is selected.
- Use different colors for different radii (e.g. yellow = detection, red = catch).
- You can draw a line from the enemy to each waypoint to visualize the patrol path.
- Gizmos are editor-only — they do not appear in a build.

### Questions to guide you
- How do you make the gizmo radius match the actual detection radius value in your script?
- What color makes the most sense for a "danger zone"?
- How would you draw the patrol path as a line?

### Reference
- Unity Manual — Gizmos: https://docs.unity3d.com/ScriptReference/Gizmos.html

---

## Part 2 — PlayerPrefs (Save the Player Name)

When the player types their name on the main menu and presses OK, that name should be **saved** so it appears again the next time they open the game.

### What is PlayerPrefs?
PlayerPrefs is a **key-value store** that Unity saves on the machine itself — not in your project.

```
Key (string)     →     Value
"PlayerName"     →     "Von"
"MasterVolume"   →     0.75
"HighScore"      →     9999
```

It supports **three types only:**

| Method | Type | Example |
|---|---|---|
| `PlayerPrefs.SetString(key, value)` | string | name, last scene |
| `PlayerPrefs.SetInt(key, value)` | int | score, level |
| `PlayerPrefs.SetFloat(key, value)` | float | volume, time |
| `PlayerPrefs.GetString(key, default)` | string | load saved name |
| `PlayerPrefs.GetInt(key, default)` | int | load saved score |
| `PlayerPrefs.GetFloat(key, default)` | float | load saved volume |
| `PlayerPrefs.Save()` | — | flush to disk immediately |
| `PlayerPrefs.HasKey(key)` | bool | check if key exists |

### Your task
- In `S13_PlayerNameInput.cs`, when the player confirms their name:
  - **Save** the name to PlayerPrefs
  - **Load** the saved name on `Start()` so the field is pre-filled

### Hints
- Use a `const string` for your key so you never mistype it.
- Always provide a default value in `GetString()` for first-time runs.
- Call `PlayerPrefs.Save()` after writing — Unity auto-saves on quit but not on crashes.

### Reference
- Unity Manual — PlayerPrefs: https://docs.unity3d.com/ScriptReference/PlayerPrefs.html

---

## Part 3 — JSON Save (Record Finish Time)

When the player reaches the end of Room 5, their **name and finish time** must be saved to a JSON file on disk.

### What does the save file look like?

```json
{
    "entries": [
        { "playerName": "Von", "finishTime": 42.3 },
        { "playerName": "Juan", "finishTime": 65.1 },
        { "playerName": "Maria", "finishTime": 38.9 }
    ]
}
```

### Saving — hints

```csharp
// Step 1: Create a [Serializable] class that matches the JSON structure

// Step 2: Build the object with the player's name and time

// Step 3: Convert to JSON string
string json = JsonUtility.ToJson(data, prettyPrint: true);

// Step 4: Write to file
File.WriteAllText(path, json);
```

### Loading — hints

```csharp
// Step 1: Read the file
string json = File.ReadAllText(path);

// Step 2: Convert back to your class
MyData data = JsonUtility.FromJson<MyData>(json);
```

### Where does the file go?
Use `Application.persistentDataPath` — this is the correct writable folder per platform:

| Platform | Path |
|---|---|
| Windows | `C:/Users/<user>/AppData/LocalLow/<company>/<product>/` |
| macOS | `~/Library/Application Support/<company>/<product>/` |
| Linux | `~/.config/unity3d/<company>/<product>/` |

```csharp
string path = System.IO.Path.Combine(Application.persistentDataPath, "scores.json");
```

### Your tasks
1. Create a `[Serializable]` class for a leaderboard entry (name + time)
2. Create a `[Serializable]` wrapper class that holds a list/array of entries
3. When the player finishes Room 5, append their entry and save
4. In the leaderboard UI, read the file and populate the scroll list (sort by time, shortest first)

### Hints
- `JsonUtility` does **not** support top-level arrays — always wrap your list in a class.
- Check `File.Exists(path)` before reading — the file won't exist on the first run.
- Sort entries by `finishTime` before displaying so the fastest time is at rank #1.

### Reference
- Unity Manual — JsonUtility: https://docs.unity3d.com/ScriptReference/JsonUtility.html
- Unity Manual — Application.persistentDataPath: https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html

---

## Part 4 — Async Scene Loading (Five Rooms)

The game has **5 rooms**. Each room is a separate Unity scene. When the player reaches the trigger at the end of a room, the next room loads and the current room unloads — all **without a loading screen freeze**.

### Given to you
You are provided with `SceneField.cs` — a custom class that lets you drag a scene asset directly into the Inspector instead of typing a string name.

```csharp
[SerializeField] private SceneField nextRoom;
[SerializeField] private SceneField currentRoom;
```

### Loading a scene additively — hints

```csharp
// Loads the scene on top of what is already loaded
// The persistent scene (player + HUD) stays active
SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
```

### Unloading a scene — hints

```csharp
// Removes the scene from memory once the next one is ready
SceneManager.UnloadSceneAsync(sceneName);
```

### Checking if a scene is already loaded — hints
Before loading, check if the scene is already in memory to avoid duplicates:

```csharp
for (int i = 0; i < SceneManager.sceneCount; i++)
{
    if (SceneManager.GetSceneAt(i).name == sceneName)
    {
        // already loaded — don't load again
    }
}
```

### Your tasks
1. Create 5 room scenes (Room 1 → Room 5) — each with platforms, obstacles, and at least one enemy
2. Add a trigger zone at the **end** of each room using `S09_SceneLoadTrigger`
3. Assign the correct **load** and **unload** scenes in the Inspector using `SceneField`
4. At the end of Room 5, trigger the JSON save instead of loading a new room
5. Add all scenes to **Build Settings** (File → Build Settings → Add Open Scenes)

### Room design requirements
Each room must have:
- At least **one patrolling enemy**
- At least **one obstacle** the player must navigate around
- A **clear path** from entry to exit trigger
- A distinct visual look (different background color or layout)

### Reference
- Unity Manual — SceneManager.LoadSceneAsync: https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html
- Unity Manual — SceneManager.UnloadSceneAsync: https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.UnloadSceneAsync.html
- Unity Manual — LoadSceneMode: https://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.html

---

## Grading Criteria

| # | Criteria | Points |
|---|---|---|
| 1 | **Gizmos** — At least 3 gizmos drawn on the enemy (detection radius, catch radius, patrol path) | 15 |
| 2 | **PlayerPrefs** — Player name saves and loads correctly across sessions | 15 |
| 3 | **JSON Save** — Finish time is saved correctly on Room 5 exit with proper file structure | 20 |
| 4 | **Leaderboard** — JSON is read, entries sorted by time, and displayed in the scroll list | 20 |
| 5 | **Async Loading** — All 5 rooms load and unload correctly without freezing | 20 |
| 6 | **Room Design** — Each room has at least one enemy, one obstacle, and is completable | 10 |
| | **Total** | **100** |

### Deductions
- Game crashes on scene transition: **-10**
- Leaderboard displays unsorted or incorrect data: **-5**
- Missing `PlayerPrefs.Save()` call: **-5**
- Scenes not added to Build Settings: **-10**
- Hardcoded scene name strings instead of `SceneField`: **-5**

---

## Submission

**Filename:** `Lastname_Firstname_BallEscape.unitypackage`

**How to export:**
1. Right-click your project folder in the Project window
2. Select **Export Package**
3. Include all assets, scenes, and scripts
4. Save as `Lastname_Firstname_BallEscape.unitypackage`

**Submit to:** Canvas → Ball Escape Activity → File Upload

**Deadline:** To be announced.

---

*Given assets: Main Menu, Player ball, Enemy with FSM, HUD (timer + penalty), SceneField.cs, SceneLoadTrigger.cs, MainMenuManager.cs*
