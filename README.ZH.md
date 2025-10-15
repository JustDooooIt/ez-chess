# EZ-CHESS

专为战棋而生的框架，使用`Godot C#`！

EZ-Chess 是个脚手架，将战棋流程抽象出来，为开发提供便利。本框架提供了一套模块化、可扩展的架构，用于在 Godot 引擎中使用 C# 开发回合制棋盘游戏。其核心思想是分离棋子状态与表现、事件驱动的管道系统、以及基于`GitHub Discussions`的多人同步机制。


## 0. 框架特色

1. **状态和节点分离**：我将棋子的状态和游戏节点分离为`PieceState`和`PieceInstance`，然后通过某种方式让`PieceState`驱动`PieceInstance`改变。目的是实现前后端分离，也是实现异步更新前端的基础。  
2. **管线化**：玩家的操作会封装为对象并入队，由状态管线监听并异步执行，渲染管线也是如此，这样就实现异步更新。  
3. **事件驱动**：管线之间不是直接耦合的，而是通过事件总线联系在一起。当状态管线接受到玩家的操作事件后，会将事件`subscribe`到事件总线。然后玩家操作执行完成后，会调用`publish`执行注册的事件，由该事件去执行渲染管线相关的操作。  
4. **组件化**：战棋游戏往往存在很多装备和buff。为了应对这些复杂多变的状态，过分依赖继承可能会导致组合爆炸，所以我采用装饰器链将`PieceState`和`PieceInstance`层层封装，进行更加灵活的配置。  


## 1. 快速开始

1. 框架主要代码都在`addons/script`中。  
2. `valve`是对玩家操作的抽象，诸如移动、攻击都是由valve来调用。  
3. `state`和`instance`是对状态和节点的抽象，存储了必要的参数。一般是不需要继承的，除非有特别需求。  
4. `decorator`是装饰器，分为`PieceStateDecorator`和`PieceInstanceDecorator`，分别封装状态和节点。使用方法是继承这两个类，然后添加响应的接口和状态即可。  
5. `PieceAdapter`封装了`state`和`instance`，继承它并实现诸如移动等操作，一般调用的是`state`的函数。  
6. `PieceFactory`，创建棋子的工厂，需要继承`PieceFactoryBase`，然后将其挂载在`game.tscn`的`GameLoader`节点。  

如何运行`example`：在godot直接运行项目即可,在界面填入github token(需要开启discussion的写权限和repository读权限),然后点击create room作为房主进入游戏.其他人想要加入可以输入discussion的number(显示在房主的左上角)。


## 2. 核心架构概览

### 2.1 棋子系统

- PieceAdapter: 棋子的核心适配器，它是一个 Godot Node，负责将棋子的逻辑状态 (IPieceState) 和视觉表现 (IPieceInstance) 连接起来。它是棋子在场景树中的实际载体。
- IPieceState / PieceState: 定义棋子的逻辑状态和行为。例如，棋子的类型、阵营、生命值、移动力等。它不直接处理视觉或用户输入。
- IPieceInstance / PieceInstance: 定义棋子的视觉表现和交互逻辑。例如，棋子的纹理、选中状态、鼠标悬停反馈、移动动画等。它不直接处理棋子的游戏逻辑。
- PieceDecorator (及其子类 PieceStateDecorator, PieceInstanceDecorator): 实现了装饰器模式，允许在运行时动态地为棋子添加新的行为或修改现有行为，而无需修改其核心类。
- IPiece: 棋子的基础接口，由 PieceState、PieceInstance 和所有 PieceDecorator 实现。
- IInterfaceQueryable: 提供 Query<T>() 方法，允许在棋子对象（包括其装饰器链）上查询特定的接口实现。

### 2.2 事件与管道系统

- Pipeline (抽象基类): 定义了事件处理管道的基础结构，是一个 ILaunchable 和 IStopable 对象，可以启动和停止。
- StatePipeline: 专门用于处理棋子逻辑状态变更的事件。
- RenderPipeline: 专门用于处理棋子视觉表现和动画的事件。
- Valve (阀门): 管道中的最小执行单元。每个 Valve 封装了一个具体的动作，并与一个特定的 PieceEvent 关联。
    - StateValve: 在 StatePipeline 中执行，处理 IPieceState 相关的事件。
    - InstanceValve: 在 RenderPipeline 中执行，处理 IPieceInstance 相关的事件。
- PipelineEventBus: 一个全局的事件总线，允许不同阀门之间发布和订阅事件，实现解耦通信。
- RegisterValveAttribute / ValveGenerator: 代码生成器机制。RegisterValveAttribute 用于标记自定义的 Valve 类，ValveGenerator 会根据这些标记自动生成 StateValve 和 InstanceValve 的部分代码，将事件与 IAction<TEvent> 接口连接起来，大大简化了事件处理的开发。
- IAction<TEvent>: 接口，表示一个可以接收特定事件 TEvent 并执行相应动作的对象。

### 2.3 地图与寻路

- HexMap: 负责管理六边形地图的逻辑，包括坐标转换、寻路等。
- TerrainLayers: 管理地图上的不同地形层级，每个层级可以有不同的寻路成本。
- AStar: 自定义的 A* 寻路算法实现，支持六边形网格和地形成本计算。

### 2.4 游戏管理 (Game Management)

- GameManager: 游戏的主控制器，管理所有玩家、棋子集合，并协调游戏流程。
- GameLoader: 负责游戏的初始化设置，包括加载游戏配置、创建玩家管道、实例化棋子等。
- GameState: 一个全局单例，存储整个游戏的状态信息，如当前回合、阶段、玩家阵营、选中的棋子等。
- GameConfig / GameSelector: 用于游戏启动时选择游戏模块、配置房间、登录 GitHub 账号等。

### 2.5 多人同步 (Multiplayer Synchronization)

- GithubUtils: 封装了与 GitHub Discussions API 交互的所有逻辑，用于创建房间、发送/接收游戏操作（作为评论）、同步游戏状态。
- IOperationRunner / IEnvironmentRunner: 接口，定义了如何执行从 GitHub Discussions 中获取到的游戏操作和环境更新。
- Pipelines: 在多人游戏中扮演协调者角色，负责在游戏启动时从 GitHub Discussions 恢复历史操作，并在游戏进行中轮询新的操作和环境更新。
- PlayerPipeline / OtherPipeline: 代表一个玩家的管道集合（StatePipeline 和 RenderPipeline）。OtherPipeline 用于处理非当前玩家的操作。

### 2.6 辅助工具 (Utilities)

- Group<K, V>: 一个自定义的集合类，用于将多个值与一个键关联起来（Dictionary<K, List<V>> 的封装）。
- HexUtils: 六边形网格的辅助工具，用于计算六边形之间的距离。
- AdjectiveConverter: 辅助代码生成器，将动词转换为以 “-able” 结尾的形容词。
- Vector2IConverter: Vector2I 的 JSON 序列化/反序列化转换器。


## 3. 关键接口与类详解

### 3.1 棋子的核心

主要包括：IPiece、PieceAdapter、IPieceState、IPieceInstance

- IPiece: 所有棋子相关组件的基础接口，定义了 PipelineAdapter、PiecesManager、Faction、PieceType 等通用属性，以及 Origin、Wrapped、Proxy、Wrapper 等用于装饰器模式的属性。
- PieceAdapter:
    - 继承自 Node，是棋子在 Godot 场景树中的实际节点。
    - 通过 Init(IPieceState state, IPieceInstance instance) 方法，将逻辑状态和视觉表现绑定到自身。
    - 维护 _state_node_dict 和 _node_state_dict 静态字典，用于 IPieceState 和 IPieceInstance 实例 ID 之间的映射。
    - 负责将 GameManager、PiecesManager、HexMap、PlayerPipeline 等引用注入到 IPieceState 和 IPieceInstance 中。
    - Decorate(PieceStateDecorator stateDecorator, PieceInstanceDecorator instanceDecorator) 方法允许在运行时应用装饰器。
- IPieceState / PieceState:
    - PieceState 继承自 RefCounted，表示棋子的数据和逻辑。
    - 它不直接与 Godot 场景交互，而是通过 PieceAdapter 间接获取场景信息。
    - Faction 和 PieceType 是其核心属性。
- IPieceInstance / PieceInstance:
    - PieceInstance 继承自 Node2D，表示棋子的视觉和交互。
    - 包含 Area2D 用于检测鼠标事件。
    - AddCover() 方法用于添加棋子封面（纹理）。
    - SetOutline() 用于控制棋子的描边效果。
    - 处理鼠标悬停 (Hover/UnHover) 和点击 (OnPieceLeftClicked) 事件。

### 3.2 行为扩展

- PieceDecorator(IPiece wrapped):
    - 所有装饰器的基类，继承自 RefCounted。
    - 通过构造函数接收一个 IPiece 对象进行包装。
    - 实现了 IPiece 接口，并将大部分属性和方法委托给被包装的 _wrapped 对象。
    - Query<V>() 方法允许沿着装饰器链向上查询特定接口的实现。
    - GetProxy() 方法用于获取装饰器链中最底层的原始 IPiece 对象。
- PieceStateDecorator<E> / PieceInstanceDecorator<E>:
    - 分别用于装饰 IPieceState 和 IPieceInstance，并实现 IAction<E> 接口，其中 E 是一个 PieceEvent。
    - _ReciveEvent(E @event) 抽象方法是子类实现具体事件处理逻辑的地方。
    - AddValve<T, V>(E @event) 方法用于向管道中添加新的阀门，通常在接收到事件后触发。
    - _SaveOperation(TEvent @event) 或 _SaveOperation(Operation op) 用于将操作保存到 GitHub。

### 3.3 事件驱动核心

- PipelineImpl<T>:
    - StatePipeline 和 RenderPipeline 的抽象基类。
    - 内部使用 Channel<T> (LaunchableList) 来异步地处理 Valve。
    - Launch() 方法启动管道，持续从 LaunchableList 读取并执行 Valve。
    - AddValve(Valve valve) 方法将阀门写入通道。
- Valve:
    - 管道中的执行单元，继承自 RefCounted。
    - Launch() 方法调用抽象的 DoLaunch() 来执行具体操作。
    - ValveStates 枚举表示阀门的生命周期状态。
- RegisterValveAttribute:
    - 一个自定义特性，用于标记 Valve 类。
    - ValveGenerator 会扫描带有此特性的类，并根据其 Action、ValveType 和 EventReciveable 属性自动生成相应的 StateValve 或 InstanceValve 代码。
    - 例如，如果有一个 MoveEvent 和一个 IMoveable 接口，标记一个 MoveValve，生成器会创建 MoveStateValve，其 DoLaunch 方法会调用 _pieceState.Query<IMoveable>().ReciveEvent(@event)。
- PipelineEventBus:
    - 单例模式，提供 Subscribe<T>() 和 Publish<T>() 方法。
    - 允许 StateValve 在完成状态更新后，发布一个事件到总线，然后由 RenderPipeline 中的 RenderEventHandler 订阅并创建 InstanceValve 来更新视觉表现。

### 3.4 游戏加载与配置

- GameLoader:
    - 在 _Ready() 中执行游戏初始化流程。
    - 加载 config.json（或 GameState.Instance.Config 中的配置）。
    - 通过反射或脚本加载 IPieceFactory、BaseRenderEventHandler、IOperationRunner、IEnvironmentRunner 的实现。
    - 根据配置创建 PlayerPipeline (区分本地玩家和远程玩家)。
    - 遍历配置中的阵营和棋子，使用 IPieceFactory 创建 PieceAdapter 实例，并将其添加到 PiecesManager 中。
    - 在所有棋子创建完成后，触发 GameLoaded 事件，并启动管道。

### 3.5 多人同步：GithubUtils

- GithubUtils:
    - 静态类，提供了与 GitHub GraphQL API 交互的方法。
    - 登录: Login() 验证 GitHub Token。
    - 房间管理: CreateRoom() 创建讨论作为游戏房间，GetRoomInfo() 获取房间信息，EnterRoom() / ExitRoom() 发送玩家进入/退出消息。
    - o操作同步:
        - SaveOperation() 将本地操作缓存。
        - SubmitOperationOnInterval() 定时将缓存的操作作为评论提交到 GitHub Discussion。
        - ApplyComment() / ApplyOperations() 轮询 GitHub Discussion，获取新的游戏操作和环境更新，并交由 IOperationRunner 和 IEnvironmentRunner 处理。
    - 状态恢复: Recover() 用于在游戏启动时，读取所有历史操作来恢复游戏状态。
    - 环境更新: ChangeFaction() / IncreaseTurn() 发送环境数据（如当前回合玩家、回合数）。
- Operation / GameData<T> / Environment: 定义了通过 GitHub Discussions 传递的数据结构。


## 4. 如何使用与扩展

### 4.1 创建自定义棋子

1.定义棋子状态和实例类:

- 创建继承自 PieceState 的类 (例如 MyPawnState)，添加自定义的逻辑属性。
- 创建继承自 PieceInstance 的类 (例如 MyPawnInstance)，处理自定义的视觉表现和交互。

2.创建棋子工厂:

- 创建一个继承自 PieceFactoryBase 的工厂类 (例如 MyPieceFactory)。
- 实现 Create() 方法，在该方法中，你可以使用 Create<T>() 辅助方法来实例化 PieceAdapter，并传入你的 MyPawnState 和 MyPawnInstance 的创建逻辑。

3.配置 GameLoader:

- 在 GameLoader 节点上，将你的 MyPieceFactory 脚本指定给 PieceFactoryScript 导出变量。

4.在 config.json 中配置棋子:

- 在游戏的 config.json 文件中，为你的棋子定义 name、type、faces (纹理和属性)、position、size 等。

### 4.2 添加棋子行为 (使用装饰器和管道)

1. 定义事件:
- 创建一个继承自 PieceEvent 的事件类 (例如 MoveEvent)，包含事件所需的数据。

2. 定义行为接口:
- 创建一个接口 (例如 IMoveable)，继承自 IAction<MoveEvent>，并定义 ReciveEvent(MoveEvent @event) 方法。

3. 创建行为装饰器:
- 创建一个继承自 PieceStateDecorator<MoveEvent> 或 PieceInstanceDecorator<MoveEvent> 的装饰器类 (例如 MoveDecorator)，并实现 IMoveable 接口。
- 在 _ReciveEvent(MoveEvent @event) 方法中实现棋子移动的具体逻辑。

4. 注册阀门:
- 创建一个继承自 Valve 的阀门类 (例如 MoveValve)。
- 使用 [RegisterValve("Move", ValveTypes.STATE, true)] (如果处理状态) 或 [RegisterValve("Move", ValveTypes.INSTANCE)] (如果处理表现) 特性标记它。
- ValveGenerator 会自动生成 MoveStateValve 和 MoveInstanceValve。

5. 在棋子中应用装饰器:
- 在棋子的 Init() 或 _Ready() 方法中，或者通过其他逻辑，使用 PieceAdapter.Decorate(new MoveStateDecorator(State), new MoveInstanceDecorator(Instance)) 来为棋子添加移动能力。
- 当需要触发移动时，可以通过 pieceAdapter.State.Query<IMoveable>().ReciveEvent(new MoveEvent(...)) 来发送事件。

### 4.3 自定义地图地形

- 在 HexMap 节点中，可以通过 _terrainCost 导出变量配置不同地形类型的移动成本。
- 在 TileMap 的 TileSet 中，为每个地形瓦片设置 Terrain 属性，以关联到 HexMap 中的成本。

### 4.4 实现多人游戏逻辑

1. 实现 IOperationRunner:
- 创建一个类实现 IOperationRunner 接口。
- 在 RunOperation(GameManager manager, JsonObject operation, bool recovered) 方法中，解析 operation (通常包含 PieceName、Faction 和 OperationType 等)，然后找到对应的 PieceAdapter，并触发其上的 IAction 接口。
- recovered 参数用于区分是恢复历史操作还是实时操作。

2. 实现 IEnvironmentRunner:
- 创建一个类实现 IEnvironmentRunner 接口。
- 在 SetEnvironment(GameManager manager, JsonObject environment) 方法中，根据 environment 中的 Type (如 EnvDataType.FACTION, EnvDataType.TURN) 更新 GameState。

3. 配置 GameLoader:
- 在 GameLoader 节点上，将你的 IOperationRunner 和 IEnvironmentRunner 脚本指定给相应的导出变量。

4. 房主操作:
- 房主可以通过 GithubUtils.SaveOperation() 或直接调用 GithubUtils.AddComment() 来提交游戏操作。
- 房主也可以通过 GithubUtils.ChangeFaction() 和 GithubUtils.IncreaseTurn() 来更新环境状态。


## 5. 配置与启动

1. 游戏模块选择 (GameSelector):

- GameSelector 允许用户加载外部 Godot PCK 模块。
- 每个模块应包含一个 config.json 文件，定义游戏名称、版本、描述、以及最重要的：棋子的初始布局、阵营、默认尺寸、首位行动者等。
- config.json 示例结构:
 {
  "name": "MyChessGame",
  "version": "1.0",
  "description": "A custom chess game.",
  "path": "res://path/to/my_game_scene.tscn", // 游戏主场景路径
  "factions": ["White", "Black"], // 阵营名称
  "firstMover": "White", // 谁先行动
  "defaultSize": { "x": 64, "y": 64 }, // 默认棋子区域大小
  "White": { // 阵营配置
    "group": 0,
    "pieces": [
      {
        "name": "Pawn1",
        "type": 0,
        "position": { "x": 0, "y": 1 },
        "size": { "x": 60, "y": 60 }, // 可覆盖默认尺寸
        "faces": [
          { "image": "res://path/to/white_pawn.png", "default": true, "property": {} }
        ]
      }
    ]
  },
  "Black": { /* ... */ }
}

2. 游戏配置 (GameConfig):

- 在 GameSelector 中选择模块后，会跳转到 GameConfig 场景。
- 用户可以在这里选择自己的阵营、输入 GitHub Token (用于登录)、输入房间号加入现有房间，或创建新房间。

3. 游戏加载 (GameLoader):

- GameLoader 根据 GameConfig 提供的配置或 config.json 文件，初始化游戏世界。
- 它会实例化 PieceFactory、RenderEventHandler、OperationRunner、EnvironmentRunner 的脚本，并创建所有棋子、设置玩家管道。
