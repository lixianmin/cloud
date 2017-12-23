


---
#### Abstract

新项目伊始，决定从NGUI切换到UGUI，而以前项目中别人设计UI框架或多或少存在一些问题，那么就重新设计一套。 我以前没做过UI框架，从网上草草搜索了一下也没见到有什么靠谱的设计方案，于是干脆从0开始。

UI框架的设计目标是使UI程序员可以尽可能的去关注游戏业务逻辑，而不会分散精力去考虑加载、控件获取、性能调优等无关问题。设计过程中参考了一些有经验的同学的建议，并规避了过去项目中遇到的一些坑，感谢兄弟团队。

本文主要记录一些UI框架设计过程中的设计思路，遇到的问题及相关思考，很多并不是最佳的解决方案，它只是记录一个心理路程，防止遗忘，并对后续框架的演变提供一些指引。

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
####窗体显示顺序

在torchlight项目初期采用的是定level值的方式，在一个名为UIControllerLevelConfig.lua的脚本给每个窗体确定了一个level值，比如4000，在显示窗体的时候根据该值的大小确认窗体间的遮挡关系。

原则上这个level值是由策划配置的，因为默认策划是最清楚窗体遮挡关系的人。这个方案在游戏初期没什么问题，但当越来越多的窗体加入后，窗体间的遮挡关系开始不那么明确，策划在配置level值时开始复制旧的配置表并不加修改的应用到新窗体上。而更糟糕的是，假定有a, b, c三个窗体，有时候会要求遮挡关系为 a > b > c > a，这就很难满足了。

参考一些同学的建议后，新设计采取了以下设计方案：

1. 预置了4个显示队列，分别是Background, Geometry, Transparent, Overlay，这四个队列内的窗体之间存在严格的前后遮挡关系（有没有觉得很眼熟，没错，这就是shader中的四个Queue的名字）

2. 在每一个队列内部，后调用OpenWindow()打开的窗体遮挡先调用OpenWindow()打开的窗体，具体就是通过transform之间的sibling index控制显示顺序，因为这种方式最直观，可以很方便的在Unity3d编辑器的Hierarchy窗口中看到

> 关于Unity3d中的物体的渲染顺序：
1. 不同Camera之间通过Depth控制
2. 同tranform下通过sibling index控制
3. 如果想严格控制同一transform下不同Canvas之间的显示顺序的话，可以**调整Canvas上的Sorting Layer，以及Order in Layer**

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

在2011的Touch项目中，我们采用的是transform.Find (fullpath)的方式，其中fullpath是一个从UI根节点到具体控件的transform的全路径。这个方案的优点是：因为提供了全路径，因此查找速度没有出现问题（也可能跟页游是运行在PC端有关）。而缺点是：当调整UI界面结构（UI策划）的时候，代码/配置（程序员）必须要调整，换句话说：本以为是一个简单的改动，结局却是一个**多人协作**的改动。

后来在2013的torchlight手游项目中，我们采用了DeepFindEx (name)的方式，其中name是UIWindow中某个目标节点的名字（而不再是全路径）。DeepFindEx采用的是DFS深度优先搜索算法，从UI根节点一步一步搜索下去，直到找到名字为name的节点为止。这个方案的优点是：当调整UI界面结构（不包括rename）时，只要UI策划一个人修改就可以，不再涉及程序代码的修改。而缺点是：UI界面中所有的节点不能重名，否则极有可能Find到错误的节点。当然这一点可能通过做工具来规避这个问题，虽然带来了一些额外的工作量（主要是UI策划），但可以接受。UI控件的初始化代码大概类似于如下情形：

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
local This = UIWindow:subclass
{
    RESOURCE_PATH = "prefabs/ui/_common.ab/uicharmenu",
    Queue = "Overlay",
    Layout =
    {
        InteractingRawButton = {"interacting_button", "UI.CharacterMenu.InteractingButton"},
        NearByList = {"uinearby", "UI.CharacterMenu.UINearby"},
        PortraitImage = {"portrait_image", "UIImage"},
        MenuTitle = {"charmenu_title_text", "UIText"}
    }
}

function This:OnOpened()
    local holder = self:GetWidgetHolder()
    holder.MenuTitle:SetText(CharacterMenuManager:GetMenuTitle())
    holder.InteractingRawButton:SetActive(false)
    holder.NearByList:SetActive(false)
end

```

---
#### 自定义控件（Control）支持

在2015做程序员调查的时候，有不少同学提到，torchlight项目中大量UI模块存在重复功能（但又并不完全相同）。由于大家并不知道别人做过类似的功能，或者无法直接拿来套用的原因，导致了大量相似功能的重复开发，浪费人力物力。那时我们畅想：如果有人专门负责控件类功能的开发或维护就好了。

后来，有兄弟团队的主程告诉我，他们以前开项目的时候会专门设立一个UI组长，专门负责UI控件功能的开发。新项目成立后，我也一度萌发过设立UI组长的念头，但是由于一直缺乏合适的人选，一直拖到现在。

但是UI控件开发的需求一直是存在的，而且随着项目研发进度的前行，越来越多的相似需求正在被提出来。然而，最终迫使我不得不开发自定义控件机制的最后一根稻草竟然是“我们的UI机制不允许节点重名”。重名节点会导致Deep Find找到错误的节点，这一点在前文有叙述。为了防止策划手工检查不到位，我还做了工具做自动化检查。然而实际上，除了程序有Dont Repeat Yourself的需求外，UI策划在制作UI时也有类似的需求：他们希望相同功能的UI节点只制作一份prefab（其实就是自定义控件），这样在功能修改的时候只改一处就可以很方便的同步到其它节点。在这种需求下，跨UIWindow的控件还好，如果同一UIWindow下多次使用同一控件的话，就会与程序要求的“禁止节点重名”机制相冲突。

为此我轮询了兄弟团队的做法，他们的做法比较复杂：他们做了一个编辑器用于自动改名，然后在UI框架加载的时候加入了支持加载数组的新语法，可以批量加载名称相似的控件组并初始化。这不是我想要的做法，这不但使UI制作流程复杂化了，而且引入了新的语法规则。

于是我决定单独开发一个自定义控件机制，以解决上面所有的问题，大抵设想如下：

对UI策划而言：

1. 自定义控件保存为独立prefab，可独立修改并上传到svn

2. 修改的内容可以很方便的广播到使用它的UIWindow或其它自定义控件中

3. 自定义控件之间支持嵌套

4. 嵌套的自定义控件之间，以及自定义控件与它所在的UIWindow之间，允许节点重名

对UI程序而言，需要单独定义一个lua脚本处理自定义控件相关的逻辑，所有其它的使用方式则保持不变：

1. 配置Layout的方式不变：只不过在填写widget类型的时候不再是最基本的UIButton或UIImage，而是自定义控件的lua脚本名称

2. 仍然使用widget holder (查找lua table) 的方式去索引自定义控件

3. 自定义控件本身支持UISerializer脚本，因此编写控件自身逻辑的时候也使用widget holder索引其它控件

在实际实施编码的过程遇到的主要问题有两个：

一个是嵌套控件的更新问题：比如一个名为uiwindow的prefab同时使用了两个自定义控件uiparent与uichild，而uiparent本身也使用了uichild，那么当修改uichild.prefab之后，在广播修改内容时uiparent.prefab需要先于uiwindow.prefab更新，否则就会更新失效。这纯粹是编码逻辑过于复杂，需要好好规划。

另一个是嵌套prefab的问题：Unity3d的prefab默认并不支持嵌套，需要自己设计。我去网上查了一些网友提供的嵌套设计方案，感觉没有一套好用。记录fullpath受限很大，因为prefab的文件夹很可能会调整；记录name倒是允许调整文件夹了，但是除了要求名字全局唯一外，也不允许改名；记录prefab引用也不行，随便修改一下可能就missing了。最终设计为记录guid字符串的方案，即：添加一个名为MBControlTracker的脚本，里面记录了自定义控件prefab的guid，这样无论是重命名，复制，还是其它操作，只要一切在Unity3d编辑器内进行，基本搞不坏。

这套机制执行后，弱化了对UI组长的需求，解决UIWindow下节点不允许重名的问题，一切看起来都很美好，而实际执行效果可能比我想象的还要好。UI组长只能有一个人，因此他只能开发与业务逻辑耦合较低的控件或机制，而这套机制简单易行，使得所有UI程序员都可以开发控件，那么大家开发的UI控件就可以跟业务逻辑扯上关系了，内容可以包罗万象，控件的量就上来了，重复代码的量就下来了。

---
#### 异步（延时）带来的问题与UI状态机（FSM）

框架设计经常有一个隐含的要求，那就是：**基础API可以在任意时刻以任意顺序调用而不产生副作用**。

UI框架的代码都在主线程中，因此没有线程同步的问题，但仍然要处理一些异步情况。异步意味着延时，意味着跨帧，意味着时长不一定，意味着中间可能发生任何意外的情况，意味着需要影响每一种API调用，因此最终我得出一个结论：**凡异步必带来状态**。

UI框架设计之初并没有引入FSM，虽然FSM在游戏设计中是一种常见设计模式，但我本人并不太喜欢用（从业游戏9年，8年都没用过，但这次闪避失败了）。UI框架最初只有LoadState这一个异步态，因此并没有引入FSM，后来多了OpenAnimationState与CloseAnimationState，我发现像我这么强的人人只用普通函数都已经hold不住了，无奈之下才入手了FSM。

下面是到目前为止设计完成的UIWindow状态机转换图，相关说明如下：

1. NoneState是初始状态，同时也是结束状态

2. 每个矩形/圆形都代表一个状态：
    1. 矩形是瞬时状态，即该状态会在当帧根据一些条件切换到下一个状态
    2. 圆形是持续状态，即该状态会保持一段时间，可以持续很多帧，满足某些条件后切换到下一个状态

3. 粗体的OnLoaded, OnOpened等是UI框架的回调事件

![fsm图示](https://raw.githubusercontent.com/lixianmin/cloud/master/writer/game/ui-framework-design/fsm.png)


---
#### References

1. Touch项目，启于2011年，基于Unity3d 3.5.7，舞蹈类页游，http://t.wanmei.com/

2. Torchlight项目，火炬之光移动版， 启于2013年，基于Unity3d 5.4.3，ARPG类手游，http://hj.laohu.com/

3. [UGUI多个Canvas的渲染先后层次关系设置](http://blog.csdn.net/huutu/article/details/43636241)


