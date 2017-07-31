

---
#### Template与Config有何区别？

1. Template子类：当你需要多个模板对象同时存在的时候，通过id区分不同的Template对象。比如，物品类里，WeaponItemTemplate的每一个对象都代表一种武器的模板，比如剑、枪、弓等

2. Config子类：当只需要全局一份配置数据的时候，直接按类型获取。比如ApplicationConfig，代表游戏中跟Applicaiton相关的公有配置，比如加载资源时使用的协程数，网络链接相关的参数等

---
#### 如何定义一个Template或Config

比如你要定义一个表示武器的模板，我们假定命名为WeaponItemTemplate：

1. 定义WeaponItemTemplate为Template类的子类，并标记属性[System.Serializable]；
1. 为WeaponItemTemplate填充你所要的数据成员，注意只支持public成员变量的序列化（而private成员变量与properties并不考虑支持）；
1. 通过unity3d编辑中的菜单项  *Metadata-> Generate Auto Code，补全序列化逻辑；

定义Config子类的方法完全相同。

以ApplicationConfig为例：

```
    #if UNITY_EDITOR
    [Export(ExportFlags.ExportRaw)]
    [System.Serializable]
    #endif
    public class ApplicationConfig : Config
    {
        public bool runInBackground;
        public int targetFrameRate;
        public long maximumAvailableDiskSpace;
        public int concurrentLoadCount;
    }
```

---
#### Metadata支持哪些数据类型？

1. 所有primitive数据类型，比如int, bool, float, string等；
1. 基础的unity3d数据类型，如Vector3, Vector2, Color；
1. 容器类型只支持Array, List，其它的Dictionary不支持；
1. 自定义类型，包括内部类，需要实现IMetadata接口（这是一个空接口，只是做做样子）；
1. 支持实现了IMetadata接口的struct；

---
#### 如何应对本地化？

简单说来，所有的需要处理本地化的字符串文本，定义时使用LocaleText类型，不要直接使用string类型。

关于本地化，更加详细的介绍请参考: [locale](locale.md)

---
#### 如果某个Template的数据成员只在逻辑中用到但不想序列到文件中怎么办?

加[AutoCodeIngore]属性标记忽略之。

---
#### 如何在lua中使用Metadata？

1. 跟以前一樣在C#中定義Metadata，例如定義FooTemplate類
1. 生成lua代碼，例如FooTemplate.lua
1. 使用FooTemplate.Create(id)創建對象，這是真的生成了一張lua table，請自行衡量release還是cache這張表
1. 使用FooTemplate.GetIDs()獲得FooTemplate類型的所有配置的id

---
#### 使用推荐

1. 不推荐使用使用泛型类型的接口，因为这会导致编译后的代码类型增加
2. 不推荐使用MetadataManager.Instance.GetAllTemplates(...)接口，因为这会导致该类型的所有配置实例好，当配置实例过多时会导致卡顿问题
3. 只在lua中使用的metadata类型，建议放到Editor目录中，这样游戏发布时不会带有相关的C#代码，有利于在线更新
4. ExportFlags属性（attribute）建议使用UNITY_EIDTOR宏包起来，以减少最终游戏代码

1. 定义WeaponItemTemplate为Template类的子类；
