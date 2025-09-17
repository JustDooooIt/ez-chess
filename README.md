# EZ-CHESS

专为战棋而生的框架


## 使用须知

EZ-Chess 是个脚手架，将战棋流程抽象出来，为开发提供便利。

## 框架特色

1. **状态和节点分离**：我将棋子的状态和游戏节点分离为`PieceState`和`PieceInstance`，然后通过某种方式让`PieceState`驱动`PieceInstance`改变。目的是实现前后端分离，也是实现异步更新前端的基础。  
2. **管线化**：玩家的操作会封装为对象并入队，由状态管线监听并异步执行，渲染管线也是如此，这样就实现异步更新。  
3. **事件驱动**：管线之间不是直接耦合的，而是通过事件总线联系在一起。当状态管线接受到玩家的操作事件后，会将事件`subscribe`到事件总线。然后玩家操作执行完成后，会调用`publish`执行注册的事件，由该事件去执行渲染管线相关的操作。  
4. **组件化**：战棋游戏往往存在很多装备和buff。为了应对这些复杂多变的状态，过分依赖继承可能会导致组合爆炸，所以我采用装饰器链将`PieceState`和`PieceInstance`层层封装，进行更加灵活的配置。  

## 联机特色

目前主要使用了github discussion实现联机, 原理就是轮询玩家在discussion留下的评论.

## 使用方法

1. 框架主要代码都在`addons/script`中。  
2. `valve`是对玩家操作的抽象，诸如移动、攻击都是由valve来调用。  
3. `state`和`instance`是对状态和节点的抽象，存储了必要的参数。一般是不需要继承的，除非有特别需求。  
4. `decorator`是装饰器，分为`PieceStateDecorator`和`PieceInstanceDecorator`，分别封装状态和节点。使用方法是继承这两个类，然后添加响应的接口和状态即可。  
5. `PieceAdapter`封装了`state`和`instance`，继承它并实现诸如移动等操作，一般调用的是`state`的函数。  
6. `PieceFactory`，创建棋子的工厂，需要继承`PieceFactoryBase`，然后将其挂载在`game.tscn`的`GameLoader`节点。  

## 如何运行example

在godot直接运行项目即可,在界面填入github token(需要开启discussion的写权限和repository读权限),然后点击create room作为房主进入游戏.其他人想要加入可以输入discussion的number(显示在房主的左上角)。
