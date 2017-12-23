


---
#### Abstract

设计过程中规避了上一个项目（torchlight火炬之光移动版）中遇到的一些坑，并参考了兄弟团队（SLG项目）的一些设计经验

---
#### 隐藏Load/Unload逻辑

对游戏本身而言，资源管理是一个不可回避的话题（而且通常很复杂），但对UI程序员来说，他们不应该关心UI资源的Load与Unload过程，只需要关心跟业务最相关的基本操作。因此，UI框架中的基本操作只提供两个：打开/关闭Window：

```
	UIManager:OpenWindow (name)
	UIManager:CloseWindow (name)
```

而打开/关闭Window背后的复杂问题，则全部由UI框架接管：

1. 资源（异步）加载回调处理
2. 资源加载失败处理
3. UI缓存策略，平衡内存占用与UI打开速度

---
#### 严格的回调事件序列

回调事件是给UI程序员填写UI逻辑的地方，因此通常是成对出现的。框架目前提供6个回调事件，按触发的先后顺序分别是：

回调事件 | 描述
---|---
OnLoaded | Window资源加载事件，加载成功后触发
OnOpened	| Window打开事件，打开后触发
OnActivated	| Window激活事件，获得焦点后触发
OnDeactivating	| Window失活事件，失去焦点前触发
OnClosing	| Window关闭事件，关闭关触发
OnUnloading	| Window资源卸载事件，卸载前触发；由于Window可能被缓存，因此该事件的触发时机不确定

**问题：如果资源加载失败了，是否应该执行OnLoaded()？**

这6个事件成三对，分别是OnLoaded/OnUnloading, OnOpened/OnClosing, OnActivated/OnDeactivating，每一对事件只要前者发生了，则后者一定会发生，而且时间顺序上按照OnLoaded --> OnOpened --> OnActivated的顺序绝对不会错乱。举例说来：

1. 如果OnOpened事件触发了，那么OnClosing事件一定会触发

2. OnClosing事件触发后，可以重新触发第二次OnOpened的事件，那么接下来在某个时间点一定会触发第二次OnClosing事件，以便与第二次的OnOpened事件对应

3. 如果触发OnOpened事件的话，那么一定是在OnLoaded事件触发完成后

4. OnOpened事件可能没有机会触发，那么此种情况下也必然不会触发OnClosing事件

只所以标题内会强调说**严格的回调事件序列**，是因为上一项目（torchlight）中有同学抱怨说UI框架的_onShow与_onHide方法有时候竟然不是成对出现的，而且时间顺序上也会出错，存在_onHide先执行，_onShow后执行的情况，这让程序开发的时候会不知所措。

---
#### 极速控件获取

程序要书写UI界面逻辑，首先需要拿到UIWindow内部的控件，这个看起来很简单直接的事情，带来的问题比想象的要多得多。

在2011的Touch页游项目（Unity3d 3.5.7）中，我们采用的是transform.Find (fullpath)的方式，其中fullpath是一个从UI根节点到具体控件的transform的全路径。这个方案的优点是：因为提供了全路径，因此查找速度没有出现问题（也可能跟页游是运行在PC端有关）。而缺点是：当调整UI界面结构（UI策划）的时候，代码/配置（程序员）必须要调整，换句话说：本以为是一个简单的改动，结局却是一个**多人协作**的改动。

后来在2013的torchlight手游项目（Unity3d 5.4.3）中，我们采用了DeepFindEx (name)的方式，其中name是UIWindow中某个目标节点的名字（而不再是全路径）。DeepFindEx采用的是DFS深度优先搜索算法，从UI根节点一步一步搜索下去，直到找到名字为name的节点为止。这个方案的优点是：当调整UI界面结构（不包括rename）时，只要UI策划一个人修改就可以，不再涉及程序代码的修改。而缺点是：UI界面中所有的节点不能重名，否则极有可能Find到错误的节点。当然这一点可能通过做工具来规避这个问题，虽然带来了一些额外的工作量（主要是UI策划），但可以接受。UI控件的初始化代码大概类似于如下情形：

```
local ach1Tran = self._transform:DeepFindEx("stage_achievement1")
local ach1Label = self:_getControl("UILabel", "stage_label", ach1Tran)
local popoTs = self:_getControl("Transform", "popo/popo_sprite3"", ach1Tran)
```

然后，当这个方案大量应用后，出现一个始料未及但却很难回头的问题：有些UI的结构与逻辑太复杂，界面初始化时需要Deep Find大量的控件，这可能**非常耗时**。

于是，第三个方案应运而生：

在2016年的echo手游项目（Unity3d 2017.1.0f3）中，我们采用的是直接查询lua table的方式，具体的实现思路涉及到：

1. UI策划挂接一个名为UISerializer的脚本到UI根节点，该脚本中包含一个**<有效UI控件的数组>**。所谓有效UI控件，是指在UI代码里真正使用到的那些控件。

2. UIWindow加载成功时，UI框架根据这个有效UI控件数组初始化一张lua table （我们称之为WidgetHolder），书写程序逻辑时，UI程序员按照自己设定好的key去查询这张lua table即可。

3. 在书写程序逻辑前，UI程序员先在一张名为Layout的特殊lua table中记录哪些控件是程序需要的。这是可行的，因为尽管Layout中的控件一直在变动，但UI程序员是最了解哪些控件是有效控件的人。

4. 发布机在发布资源时，根据Layout table中的内容，生成<有效UI控件数组>并填写到UISerializer脚本中。

5. UI程序员在调试程序逻辑时，很可能会需要一些并不在<有效UI控件数组>中的控件，此时退化为Deep Find模式

该方案的优点是：UI界面打开时仅付出了初始化<有效UI控件数组>的时间，而这个代价原则上是无法避免的，另外在UI逻辑中使用控件时采用查询lua table (WidgetHolder)的方式，时间复杂度可以认为是O(1)，这已经很快了。缺点是：整个UI结构中仍然不允许出现同名节点，另外UISerializer脚本初始化也需要一些额外的时间。可以说，echo项目的控件获取方案是torchlight方案的加强版。一份典型的使用UI控件的代码如下：

```
local This = UIWindow : subclass
{
    RESOURCE_PATH = 'prefabs/ui/_char.ab/uichar_miscbag_sell',
    Queue   = 'Transparent',
    Layout  =
    {
        NumUpButton = {'numberup_button', 'UIButton'},
        NumDownButton = {'numberdown_button', 'UIButton'},
        PriceNumText = {'unitpricenumber_text', 'UIText'},
    }
}

function This:OnOpened ()
    local holder = self:GetWidgetHolder()
    holder.PriceNumText:SetText(tostring(self._price))
    holder.NumDownButton:SetInteractable(false)
    holder.NumUpButton:SetInteractable(self._totalCount > self._purchaseCount)
end

```

---
#### 异步（延时）带来的问题与Window状态机（FSM）

框架设计经常有一个隐含的要求，那就是：**基础API可以在任意时刻以任意顺序调用而不产生副作用**。

UI框架的代码都在主线程中，因此没有线程同步的问题，但仍然要处理一些异步情况。异步意味着延时，意味着跨帧，意味着时长不一定，意味着中间可能发生任何意外的情况，意味着需要影响每一种API调用，因此最终我得出一个结论：**凡异步必带来状态**。

UI框架设计之初并没有引入FSM，虽然FSM在游戏设计中是一种常见设计模式，但我本人并不太喜欢用（从业游戏9年，8年都没用过，但这次闪避失败了）。UI框架最初只有LoadState这一个异步态，因此并没有引入FSM，后来多了OpenAnimationState与CloseAnimationState，我发现像我这么强的人人只用普通函数都已经hold不住了，无奈之下才入手了FSM。

下面是到目前为止设计完成的UIWindow状态机转换图，相关说明如下：

1. NoneState是初始状态，同时也是结束状态

2. 每个矩形/圆形都代表一个状态：
    1. 矩形是瞬时状态，即该状态会在当帧根据一些条件切换到下一个状态
    2. 圆形是持续状态，即该状态会保持一段时间，可以持续很多帧，满足某些条件后切换到下一个状态

3. 粗体的OnLoaded, OnOpened等是UI框架的回调事件

![fsm-代码在下面](https://raw.githubusercontent.com/lixianmin/cloud/master/writer/game/ui-framework-design/fsm.png)

<!---
graph TD

NoneState[<mark>NoneState]
NoneState --> |OnOpenWindow| LoadState

LoadState((LoadState))
LoadState --> |OnCloseWindow| NoneState
LoadState --> |Loading Done<br/><br/><b>OnLoaded|OpenAnimationState
LoadState --> |Loading Failure | FailureState

OpenAnimationState((OpenAnimationState))
OpenAnimationState --> |Animation Done| OpenedState
OpenAnimationState --> |Animation Done <br/>& OnCloseWindow| CloseAnimationState
OpenAnimationState --> |No Animation| OpenedState

OpenedState[<b>OnOpened</b><br/>OpenedState<br/><b>OnClosing</b>]
OpenedState --> |OnOpenWindow| ActivateWindow><b>OnDeactivating<br/>OnActivated<b/>]
OpenedState --> |OnCloseWindow| CloseAnimationState

CloseAnimationState((CloseAnimationState))
CloseAnimationState --> |Animation Done| ClosedState
CloseAnimationState --> |Animation Done<br/>& OnOpenWindow| OpenAnimationState
CloseAnimationState --> |No Animation| ClosedState

ClosedState --> |Cachable<br/>& OnOpenWindow| OpenAnimationState
ClosedState --> |Not Cachable| UnloadState

UnloadState[<b>OnUnloading</b><br/>UnloadState]
UnloadState--> NoneState
FailureState --> NoneState

-->

---
#### 控件（Control）支持



