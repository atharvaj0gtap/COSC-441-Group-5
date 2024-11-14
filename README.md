
# COSC-441-Group-5 Project Setup

## Step 1: Clone the Repository

Clone the repository:

```bash
git clone https://github.com/atharvaj0gtap/COSC-441-Group-5.git
```

### Open the Project in Unity

1. Open Unity Hub.
2. Click **Open** and navigate to the cloned project folder.
3. Select the folder and click **Open** to load it as a Unity project.

## Step 2: Create GameObjects and Configure Components

### 1. Main Camera
- The main camera is automatically created when the project opens.
- Ensure it’s correctly positioned to view the scene.

### 2. Canvas
- **Hierarchy**: Right-click and select **UI > Canvas**.
- Add UI elements (e.g., Text for "Enter ID" and a Button for "START") as needed.
- **EventSystem**: Automatically created with the Canvas; if not, add it by right-clicking in the Hierarchy and selecting **UI > Event System**.

### 3. GameManager
- **Hierarchy**: Right-click, select **Create Empty**, and rename it to `GameManager`.
- **Script**: Attach the `GameManager` script.
- **OnClick for Start Button**:
  - Select the Start button in the Canvas.
  - In the **Inspector** under **Button (Script)**, add a new **On Click ()** event.
  - Drag the `GameManager` GameObject into the event field and select the `StartGame` method.

### 4. TargetSpawner
- **Hierarchy**: Right-click, select **Create Empty**, and rename it to `TargetSpawner`.
- **Script**: Attach the `TargerManager` script.
- **Prefab References**:
  - Drag and assign the relevant target prefabs (e.g., Goal and Non-Goal targets) to the respective fields in the `TargetSpawner` component.

### 5. BubbleCursor
- **Hierarchy**: Right-click, select **Create Empty**, and rename it to `BubbleCursor`.
- **Components**:
  - **Sprite Renderer**: Add a Sprite Renderer, assign a circular sprite (e.g., "Circle"), and set Material to `Sprite-Lit-Default`.
  - **Audio Source**: Add an Audio Source component, uncheck **Play On Awake**, and adjust other settings as needed.
- **BubbleCursor Script**: Attach the `BubbleCursor` script and configure its fields:
  - **Max Radius**: 3
  - **Min Radius**: 0.5
  - **Contact Filter**: Set to detect appropriate layers.
  - **Bubble Sprite**: Drag the Sprite Renderer from `BubbleCursor`.
- **Inner Ring Sprite**:
  - **Hierarchy**: Right-click `BubbleCursor` and select **Create Empty**. Rename it to `InnerRing`.
  - **Component**: Add a Sprite Renderer, assign a circular sprite, and set Material to `RadialFillMaterial`.
  - Drag `InnerRing`'s Sprite Renderer into the Inner Ring Sprite field.
- **Colors**: Set colors as follows:
  - **Hover Color**: Yellow
  - **Default Color**: White
  - **Non Goal Hover Color**: Red
  - **Target In Range Color**: Blue
  - **Correct Click Color**: Green
  - **Wrong Click Color**: Red
- **Audio Source**: Drag `BubbleCursor`’s Audio Source component here.
- **Correct Sound**: Drag in the success sound (e.g., "Success Sound Effect 1").
- **Bomb Sound**: Drag in the bomb sound (e.g., "Explosion Hit Sound Effect").

### 6. PointCursor
- **Hierarchy**: Right-click, select **Create Empty**, and rename it to `PointCursor`.
- **Script**: Attach the `PointCursor` script.

### 7. StudyBehavior
- **Hierarchy**: Right-click, select **Create Empty**, and rename it to `StudyBehavior`.
- **Script**: Attach the `StudyBehavior` script.
- **Configure StudyBehavior Fields**:
  - **Block Sequence**: 0
  - **Study Settings**:
    - **Target Sizes**: Set size to 3 and values to 0.5, 0.3, and 0.1.
    - **Target Amplitudes**: Set size to 3 and values to 0.2, 0.4, and 0.8.
    - **EW To W Ratio**: Set size to 2 and values to 2 and 1.
    - **Cursor Type**: Select Bubble Cursor.
    - **Number of White Targets**: 5
    - **Total Trials**: 5
    - **Current Trial Index**: 0

### 8. LevelCanvas
- **Hierarchy**: Right-click, select **UI > Canvas**, and rename it to `LevelCanvas`.
- **Canvas Properties**:
  - **Render Mode**: Screen Space - Overlay
  - **Canvas Scaler**: Set **UI Scale Mode** to **Constant Pixel Size**.
- **Add levelText Text Element**:
  - **Hierarchy**: Right-click `LevelCanvas` and select **UI > Text - TextMeshPro**. Rename it to `levelText`.
  - **Rect Transform**:
    - **Position**: Set **Pos X** to 280, **Pos Y** to 190.
    - **Width**: 150, **Height**: 30 (adjust as needed).
  - **TextMeshPro Settings**:
    - **Text**: Set initial text to "Level: ".
    - **Font Asset**: Assign a font (e.g., LiberationSans SDF).
    - **Font Size and Alignment**: Adjust as needed.

### 9. Add StreakText for Streak Display
- **Hierarchy**: Right-click `LevelCanvas` and select **UI > Text - TextMeshPro**. Rename it to `streakText`.
- **Rect Transform**:
  - **Position**: Set **Pos X** to 280, **Pos Y** to 160 (or adjust as needed).
  - **Width**: 150, **Height**: 30 (adjust as needed).
- **TextMeshPro Settings**:
  - **Text**: Set initial text to "Streak: ".
  - **Font Asset**: Assign a font (e.g., LiberationSans SDF).
  - **Font Size and Alignment**: Adjust as needed.
- **GameManager Script**: Ensure that the `streakText` field in the `GameManager` script is linked to this TextMeshPro component.

## Step 3: Streak Functionality and Difficulty Increase

### Streak Functionality
The streak functionality is designed to increase the game’s difficulty by adding more distractor targets as the player achieves consecutive correct selections. Here’s how it works:

1. **Correct Target Selection**: Each time the player selects the correct red target, the streak value increases by 1.
2. **Streak Reset on Incorrect Selection**: If the player selects an incorrect target (white distractor), the streak value resets to 0.
3. **Difficulty Scaling**: When the streak reaches 3 or more, the number of white distractor targets increases by multiplying the streak value. For example, if the streak value is 4, the number of distractors is multiplied by 4.

This feature aims to make the game more challenging as players perform well, adding additional white distractors based on their streak.

## Step 4: Verify and Test

1. **Play Mode**: Enter Play mode in Unity to ensure all components are functioning correctly.
2. **Test Streak Functionality**:
   - Click on the red target multiple times to see the streak increase.
   - Click on a white distractor target to reset the streak.
   - Observe that the number of distractor targets increases as the streak value goes above 3.
3. **Adjustments**: Make any necessary adjustments to positioning, colors, or component values.

This setup should help you configure and test the project effectively, with all necessary components and features, including the new streak functionality and difficulty scaling based on streak value.
