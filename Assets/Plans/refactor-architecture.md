# Plan: Refactor Architecture and Data Separation

## Project Overview
- **Game Title:** Collect-3 Magic
- **High-Level Concept:** A match-3 variant where items are collected from bubbles into containers.
- **Goal:** Ensure strict separation between Data (Configs), Logic (Controller/Model), and Presentation (Views).

## Architectural Changes
### 1. Data Layer Refactoring
- Move `ItemType` to `Assets/Scripts/_game/ItemType.cs`.
- Move `LevelConfig` to `Assets/Scripts/_Data/LevelConfig.cs`.
- Create `GameplayConfig` in `Assets/Scripts/_Data/GameplayConfig.cs` to hold `ItemData` (ItemType to Sprite mapping) and potentially other global gameplay settings.
- Remove `ItemData` usage from `GameplayView`.

### 2. Logic Layer Refactoring
- **GameplayController**:
    - Inject `GameplayConfig`.
    - Handle sprite retrieval from `GameplayConfig` during level generation.
    - Decouple from `GameplayView`'s internal data.

### 3. View Layer Refactoring
- **GameplayView**:
    - Remove `itemSprites` and `GetSprite`.
    - Focus only on providing scene references and prefabs.
    - Keep `SetupContainers` as a visual initialization step.

## Key Assets & Context
- `GameplayConfig`: ScriptableObject with `List<ItemData>`.
- `GameplayController`: Pure C# logic class.
- `GameplayView`: Scene-bound visual orchestrator.

## Implementation Steps
1. **Create `ItemType.cs`** in `Assets/Scripts/_game/`.
2. **Create `LevelConfig.cs`** in `Assets/Scripts/_Data/`.
3. **Create `GameplayConfig.cs`** in `Assets/Scripts/_Data/`.
4. **Update `GameplayView.cs`** to remove data.
5. **Update `GameplayController.cs`** to use `GameplayConfig`.
6. **Update `MainSceneInstaller.cs`** to bind `GameplayConfig`.
7. **Delete `LevelData.cs`**.

## Setup Instructions for Views
### ItemView
- Attach to Item Prefab.
- Assign a child `SpriteRenderer` to the `spriteRenderer` field.
- Ensure the object has a `Collider2D` (e.g., CircleCollider2D) so `OnMouseDown` works.

### BubbleView
- Attach to Bubble Prefab (Small and Big variants).
- Assign a `Rigidbody2D` (Dynamic).
- Create child GameObjects for item slots and attach `SlotView` to them.
- Assign these child objects to the `itemSlots` list in `BubbleView`.

### BucketView
- Attach to the main container GameObject in the scene.
- Setup walls and bottom using `BoxCollider2D` or similar.

### ContainerView
- Place at the top of the screen.
- Represents the target for matching 3 items.

### GameplayView
- Assign prefabs: `ItemPrefab`, `SmallBubblePrefab`, `BigBubblePrefab`.
- Link scene objects: `PoolContainer` (empty transform), `Bucket`, `Containers` (list of ContainerViews), `Slots` (list of overflow SlotViews).
- Assign `LevelConfig` and `GameplayConfig` (via Installer).

## Verification & Testing
- Ensure the project compiles after refactoring.
- Verify that items still spawn with correct sprites assigned via `GameplayConfig`.
- Check that clicking items correctly triggers matching logic.
