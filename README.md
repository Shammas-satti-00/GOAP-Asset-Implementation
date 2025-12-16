# GOAP-Based Food-Eating AI System in Unity

## Overview

This project demonstrates a **GOAP (Goal-Oriented Action Planning) system** implemented in Unity for AI agents (players) that navigate a world to find and eat food objects. The system supports **multiple AI players**, each autonomously selecting food targets, moving toward them, and eating them according to a dynamic plan.  

> **Note:** This project is primarily for **learning and understanding GOAP** in Unity.

---

## Scope

The purpose of this project is to:

1. Demonstrate a **GOAP AI system** with multiple actions and goals.
2. Handle **multiple AI agents** competing for shared resources (food).
3. Ensure each AI agent selects the **nearest available food** and avoids conflicts.
4. Implement realistic **player movement with gravity**.
5. Support **dynamic goals via ScriptableObjects**, making it easy to add new goals without modifying code.
6. Provide a modular setup that can be extended with new actions, goals, or world states.

---

## Features

### 1. GOAP (Goal-Oriented Action Planning)

- **WorldState**: Tracks boolean states of the world (`HasFood`, `IsHungry`, `isAtTarget`).
- **GOAPAction**: Encapsulates preconditions and effects. Actions include:
  - `MoveToFood`: Move toward a food object.
  - `PickFood`: Pick up the food.
  - `Eat`: Consume the food.
- **GOAPPlanner**: Generates a sequence of actions (plan) to satisfy a goal.
- **GOAPGoal**: The desired world state (e.g., `IsHungry = false`).

### 2. ScriptableObject Goals

- Goals can now be created as **ScriptableObjects** in Unity.
- Each goal ScriptableObject defines a `goalName` and a set of desired states (`Dictionary<string, bool>`).
- Players can dynamically convert these ScriptableObjects into runtime goals.
- This allows for **modular and extensible AI behavior** without modifying code.

### 3. Multi-Agent Food Targeting

- Each `Food` object has an `isClaimed` flag.
- Players only target **unclaimed food**, preventing multiple agents from choosing the same target.
- Claim is set when a player selects a food and released after eating.
- Prevents **race conditions** where two agents could target the same food simultaneously.

### 4. Player Movement and Eating

- Players use `CharacterController` to move toward their target food.
- Eating is performed over time via a coroutine that reduces the `Food`’s health until consumed.
- Movement includes **smooth rotation** toward the target.

### 5. Gravity

- Gravity is applied manually to the `CharacterController` to ensure realistic falling.
- Ground detection ensures vertical velocity is reset when the player is on the ground.
- Movement and gravity are combined for smooth motion.

---

## GOAP Flow Diagram

```mermaid
flowchart TD
    A[Player (GOAPAgent)] --> B[Check WorldState]
    B --> C[GOAPPlanner generates plan]
    C --> D{Plan available?}
    D -- No --> E[Wait / Retry]
    D -- Yes --> F[Execute Actions sequentially]
    F --> F1[MoveToFood]
    F --> F2[PickFood]
    F --> F3[Eat Food]
    F3 --> G[Update WorldState]
    G --> H{Goal achieved?}
    H -- No --> B
    H -- Yes --> I[Plan Complete / Next Goal]
## Project Structure

Assets/
└── GOAP/
└── Scripts/
├── GOAPAgent.cs         # Main AI agent script
├── GOAPAction.cs        # Base GOAP action class
├── GOAPPlanner.cs       # GOAP planning logic
├── GOAPGoal.cs          # Goal representation
├── WorldState.cs        # World state tracking
├── MoveToFood.cs        # Move and eat food action
├── MoveToAction.cs      # Optional simpler move action
└── Food.cs              # Food object with health and claim

---

## Usage

1. Add **Food** prefabs in the scene with the `Food` script attached.
2. Add multiple **Player prefabs** with `GOAPAgent` and `MoveToFood` scripts.
3. Set `speed`, `eatAmount`, and `eatInterval` on the `MoveToFood` component.
4. Press **Play**: Players will autonomously plan, move, and eat food.

---

## Notes / Future Improvements

- Currently, goals are hardcoded. Can be extended to **ScriptableObject goals** for dynamic goal assignment.
- The AI currently only supports boolean world states. Can be extended for numeric states (e.g., health, hunger levels).
- Additional actions (e.g., running away from enemies, picking up items) can be added easily using the modular GOAP system.
- Optional **FoodManager** could be added for large-scale worlds to optimize target selection and reduce frame cost.

---

## License

This project is open-source and free to use for educational purposes.
