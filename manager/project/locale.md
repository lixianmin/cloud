

---
#### metadata本地化的定义与使用

1. 程序：定义文本字段时，使用LocaleText类型，而不要使用普通的string类型

2. 策划：通过excel导出metadata对应的xml时，记得填入LocaleText的guid字段

假定程序定义的模板格式为：
```
#if UNITY_EDITOR
[Export(ExportFlags.ExportRaw)]
[System.Serializable]
#endif
public class LocaleTextExample : Template
{
    public LocaleText name;
    public LocaleText[] userdata;
}
```

则需要导出的xml格式如下，注意：

1. guid的值在xml中以attribute的方式表示
2. guid的值要求全局唯一

```
<?xml version="1.0" encoding="utf-8"?>
<XmlMetadata xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Templates>
    <Template xsi:type="LocaleTextExample">
      <id>0</id>
      <name guid="guid-name">panda</name>
      <userdata>
        <LocaleText guid="guid-phone">139xxx</LocaleText>
        <LocaleText guid="some-guid">some data</LocaleText>
      </userdata>
    </Template>
  </Templates>
  <Configs />
</XmlMetadata>
```

---
#### metadata如何翻译多国语言

1. metadata中LocaleText数据会被单独导出一个文件，默认文件名locale.zh_cn.raw，是简体中文本地化数据
2. 如果要对其它国家的语言进行本地化处理，则将需要将对应翻译反的配置xml**按国家**放到独立的目录中，以美国英语为例，创建新目录locale.en_us
3. 将翻译的xml按XmlLocaleTextTranslator的格式书写，这些xml可以任意命名
4. 选择菜单"*metadata/Dispatch I18N"导出翻译后的配置
5. 导出的配置名跟存放这些xml的目录名相同，以刚刚创建的美国英语为例，因为目录名是locale.en_us，因此导出的数据文件名为locale.en_us.raw


XmlLocaleTextTranslator的格式如下：

```
<?xml version="1.0" encoding="utf-8"?>
<XmlLocaleTextTranslator xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <items>
    <LocaleText guid="guid-hello">hello</LocaleText>
    <LocaleText guid="guid-world">world</LocaleText>
  </items>
</XmlLocaleTextTranslator>
```

---
#### UI上的广西如何标记本地化

1. 对于所有需要执行本地化的文本控件，制作UI窗体时务必使用自定义控件：UIText
2. 在UIText的Inspector尾部，有guid一项，凡需要翻译的静态文本都需要填入guid，如果不需要翻译的则不用填
3. 凡填入guid的UIText控件，在窗体打开时，程序会按guid找到对应的翻译文本，并替换UIText控件上默认加载的文本

---
#### Lua中如何处理多国语言

1. 在策划svn中，定位到echo_res/resource/lua/locale目录
2. 其中有一个zh_cn子目录，这个是对应中文的本地化，其它国家的翻译文件可以在zh_cn旁边建立兄弟目录，以美国英语为例，可以在zh_cn旁边建立名为en_us的兄弟目录
3. 在zh_cn目录中可以任意添加含在本地化文本的lua文件，记得把新添加的lua文件名写入到_entry.lua中即可

_entry.lua格式如下：

```
local fileList =
{
    'Test1',   -- 含本地化文本数据的lua配置
    'Test2',
}

return fileList

```

举例Test1.lua格式如下：

```
local dict =
{
	["EXP"] = "经验",
	["VIP_EXP"] = "VIP经验",
	["ACTION_POINT"] = "体力",
	["LOCKED"] = "未解锁",
	["LOADING"] = "加载中...",
	["SHIELD"] = "护盾",
}

return dict
```
