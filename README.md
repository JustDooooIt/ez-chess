# EZ-CHESS

A framework designed specifically for tactical turn-based games, developed with `Godot C#`!

EZ-Chess is a scaffolding framework that abstracts the tactical game flow to facilitate development. It provides a modular and extensible architecture for developing turn-based board games in the Godot engine using C#. Its core concepts include separating game state from presentation, an event-driven pipeline system, and a multiplayer synchronization mechanism based on `GitHub Discussions`.

## 0. Framework Features

1. **State and Node Separation**:  
   The game piece’s state and node are separated into `PieceState` and `PieceInstance`. 
   The `PieceState` drives the changes in `PieceInstance`. 
   This allows frontend-backend separation and asynchronous frontend updates.  

2. **Pipeline Architecture**:  
   Player actions are encapsulated as objects and queued. 
   The state pipeline listens and executes them asynchronously. 
   The rendering pipeline works the same way, enabling asynchronous updates.  

3. **Event-Driven**:  
   Pipelines are not directly coupled but communicate through an event bus. 
   When the state pipeline receives a player operation, it subscribes the event to the event bus. 
   Once execution completes, it publishes the event, which triggers rendering operations in the render pipeline.  

4. **Component-Based Design**:  
   Tactical games usually contain many equipments and buffs. 
   Overusing inheritance can cause class explosion, so EZ-Chess uses decorator chains to wrap `PieceState` and `PieceInstance`, 
   allowing more flexible configuration.  

## 1. Quick Start

1. The main framework code is located in `addons/script`.  
2. `valve` abstracts player actions such as movement and attack.  
3. `state` and `instance` abstract logic and node data. They generally don’t need inheritance unless special behavior is required.  
4. `decorator` contains decorators, divided into `PieceStateDecorator` and `PieceInstanceDecorator`, which wrap state and instance respectively.  
5. `PieceAdapter` wraps `state` and `instance`. Inherit it and implement actions such as movement (usually by calling functions in `state`).  
6. `PieceFactory` creates chess pieces. Inherit `PieceFactoryBase` and attach it to the `GameLoader` node in `game.tscn`.  

How to run `example`:  
Run the project directly in Godot. Enter your GitHub token (must have “Discussions: write” and “Repository: read” permissions), 
then click **Create Room** to host the game. Other players can join by entering the discussion number (shown on the top-left of the host’s screen).

## 2. Core Architecture Overview

### 2.1 Piece System

- **PieceAdapter**: Core adapter for chess pieces. A Godot Node that connects the logical state (`IPieceState`) and the visual representation (`IPieceInstance`).  
- **IPieceState / PieceState**: Defines logic such as type, faction, HP, and mobility. Does not handle visuals or input.  
- **IPieceInstance / PieceInstance**: Defines visual behavior such as textures, hover feedback, selection, and animation.  
- **PieceDecorator (and subtypes PieceStateDecorator, PieceInstanceDecorator)**: Implements the decorator pattern for dynamic behavior extension.  
- **IPiece**: The base interface implemented by all piece-related objects.  
- **IInterfaceQueryable**: Provides `Query<T>()` to search for specific interface implementations within a piece or its decorators.

### 2.2 Event and Pipeline System

- **Pipeline (abstract base)**: Defines the event-handling structure and lifecycle. Implements `ILaunchable` and `IStoppable`.  
- **StatePipeline**: Handles logical state change events.  
- **RenderPipeline**: Handles visual rendering and animation events.  
- **Valve**: The smallest executable unit in a pipeline.  
  - **StateValve**: Executes `IPieceState`-related events.  
  - **InstanceValve**: Executes `IPieceInstance`-related events.  
- **PipelineEventBus**: A global event bus for decoupled communication between valves and pipelines.  
- **RegisterValveAttribute / ValveGenerator**: Attribute-based code generator that auto-binds custom Valve classes with their actions.  
- **IAction<TEvent>**: Interface representing an object capable of executing an action when a specific event is received.

### 2.3 Map and Pathfinding

- **HexMap**: Manages hex grid logic, coordinate conversion, and pathfinding.  
- **TerrainLayers**: Manages terrain layers with different movement costs.  
- **AStar**: Custom A* pathfinding implementation supporting hex grids and terrain cost calculation.

### 2.4 Game Management

- **GameManager**: Main controller managing players, pieces, and the game flow.  
- **GameLoader**: Initializes settings, creates players, pipelines, and chess pieces.  
- **GameState**: A global singleton storing the entire game state (turns, players, selection, etc.).  
- **GameConfig / GameSelector**: Handles game module selection, room configuration, and GitHub login.

### 2.5 Multiplayer Synchronization

- **GithubUtils**: Handles GitHub Discussions API interactions for room creation, message posting, state sync, and operation management.  
- **IOperationRunner / IEnvironmentRunner**: Define how operations and environmental updates are executed.  
- **Pipelines**: Synchronize game history and continuously poll for updates.  
- **PlayerPipeline / OtherPipeline**: Represent the current player’s pipelines and others’ pipelines.

### 2.6 Utilities

- **Group<K, V>**: A dictionary wrapper allowing multiple values per key.  
- **HexUtils**: Hex grid utilities for distance and adjacency.  
- **AdjectiveConverter**: Converts verbs to “-able” adjectives for code generation.  
- **Vector2IConverter**: JSON serializer/deserializer for `Vector2I`.

## 3. Key Interfaces and Classes

### 3.1 Piece Core

Includes `IPiece`, `PieceAdapter`, `IPieceState`, `IPieceInstance`.

- **IPiece**: Base interface defining shared attributes such as `PipelineAdapter`, `PiecesManager`, `Faction`, `PieceType`, etc.  
- **PieceAdapter**:  
  - Inherits from `Node`.  
  - `Init(IPieceState state, IPieceInstance instance)` binds logic and visuals.  
  - Maintains mapping dictionaries between state and instance IDs.  
  - Injects references like `GameManager`, `HexMap`, and pipelines into pieces.  
  - `Decorate()` applies runtime decorators.  
- **PieceState**: Inherits from `RefCounted`, holds logical attributes (e.g., Faction, PieceType).  
- **PieceInstance**: Inherits from `Node2D`, handles visual behavior, hover and click events.

### 3.2 Behavior Extension

- **PieceDecorator(IPiece wrapped)**: Base class for all decorators. Implements `IPiece` and delegates most properties to `_wrapped`.  
- **PieceStateDecorator<E> / PieceInstanceDecorator<E>**:  
  Implement `IAction<E>` where `E` is a `PieceEvent`.  
  Defines `_ReceiveEvent(E @event)` for event handling.  
  Supports adding valves via `AddValve<T, V>(E @event)` and saving operations to GitHub.

### 3.3 Event-Driven Core

- **PipelineImpl<T>**: Base class for state/render pipelines using asynchronous valve execution.  
- **Valve**: A `RefCounted` unit with `DoLaunch()` for concrete logic.  
- **RegisterValveAttribute**: Custom attribute for marking Valve classes, enabling automatic generation of `StateValve` or `InstanceValve`.  
- **PipelineEventBus**: Singleton providing `Subscribe<T>()` and `Publish<T>()`. 
  Used to notify render pipelines when state changes complete.

### 3.4 Game Loading and Configuration

- **GameLoader**:  
  Executes initialization in `_Ready()`, loads configurations, creates players, factories, and pipelines, then launches the game.

### 3.5 Multiplayer: GithubUtils

- **GithubUtils**:  
  - **Login()**: Authenticate GitHub Token.  
  - **CreateRoom() / GetRoomInfo() / EnterRoom() / ExitRoom()**: Manage game room discussions.  
  - **SaveOperation() / SubmitOperationOnInterval()**: Cache and post game operations.  
  - **ApplyComment() / ApplyOperations()**: Retrieve and apply remote operations.  
  - **Recover()**: Restore game state from history.  
  - **ChangeFaction() / IncreaseTurn()**: Update environment data like current turn and player.

## 4. Usage and Extension

### 4.1 Creating Custom Pieces

1. Define piece state and instance classes:  
   - Create a subclass of `PieceState` (e.g., `MyPawnState`).  
   - Create a subclass of `PieceInstance` (e.g., `MyPawnInstance`).  

2. Create a piece factory:  
   - Subclass `PieceFactoryBase` (e.g., `MyPieceFactory`).  
   - Implement `Create()` using `Create<T>()` to instantiate `PieceAdapter`.  

3. Configure `GameLoader`:  
   - Assign your factory script to `GameLoader.PieceFactoryScript`.  

4. Configure pieces in `config.json`:  
   - Define name, type, texture, position, and size.

### 4.2 Adding Piece Behaviors

1. Define an event (e.g., `MoveEvent`).  
2. Define a behavior interface (e.g., `IMoveable : IAction<MoveEvent>`).  
3. Create decorators (`MoveStateDecorator`, `MoveInstanceDecorator`).  
4. Register valves using `[RegisterValve("Move", ValveTypes.STATE)]` or `[RegisterValve("Move", ValveTypes.INSTANCE)]`.  
5. Apply decorators with `PieceAdapter.Decorate(...)`.  

### 4.3 Custom Map Terrain

- Configure `_terrainCost` in `HexMap`.  
- Set `Terrain` properties in `TileSet` to link with movement costs.

### 4.4 Implementing Multiplayer Logic

1. Implement `IOperationRunner` to handle operations.  
2. Implement `IEnvironmentRunner` to update environment.  
3. Configure them in `GameLoader`.  
4. Use `GithubUtils` to submit operations and update turns.

## 5. Configuration and Launch

1. **Game Module Selection (GameSelector)**:  
   Allows loading of external Godot PCK modules.  
   Each module contains a `config.json` defining name, version, description, factions, layout, etc.

2. **Game Configuration (GameConfig)**:  
   Choose faction, input GitHub Token, join or create a room.

3. **Game Loading (GameLoader)**:  
   Initializes the world, loads factories and handlers, creates pieces, and starts the pipelines.
