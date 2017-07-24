
###  Lua Code Style Guide -- Lua代码规范

---
#### Naming 命名规则

```
    -- local变量、函数参数, 小写第一个单词
    lowerFirstWords

    -- public成员方法、成员变量、常量、枚举值，使用全大写单词
    CapWords

    -- private成员方法、成员变量，在名字前加下划线`_`
    _BeginWithUnderscore
    _beginWithUnderscore
```

---
#### Define and Usage 定义与使用

- `File` and `Package` 文件和包
```
	-- Define: 文件名唯一, 全大写单词首字母
	PackageName.lua
	ClassName.lua
	EnumName.lua
	DataName.lua

	-- Usage
	local PackageName = require "PackageName"

```

- `Enum` 枚举类型
```
	local EnumName =
	{
		EnumValueName1 = 1,
	    EnumValueName2 = 2,
	}
```

---
#### `Function` and `Method` 函数和成员方法

- `Function` 函数

```
	-- Define
    local function PublicFunctionName (argName1, argName2, ...)
        -- TODO: Do something
    end

    local function _PrivateFunctionName (argName1, argName2, ...)
        -- TODO: Do something
    end

	-- Usage
    PublicFunctionName(argName1, argName2, ...)
    _PrivateFunctionName(argName1, argName2, ...)
```

- `Method` 成员方法
```
	-- Define
    function ClassName:PublicMethodName(argName1, argName2, ...)
		-- TODO: Do something
    end

    function ClassName:_PrivateMethodName(argName1, argName2, ...)
        -- TODO: Do something
    end

	-- Usage
	self:MethodName(argName1, argName2, ...)

```

- `Static Method` 静态成员方法

```
	-- Define
    function ClassName.PublicMethodName (argName1, argName2, ...)
        -- TODO: Do something
    end

    function ClassName._PrivateMethodName (argName1, argName2, ...)
        -- TODO: Do something
    end

	-- Usage
    ClassName.MethodName (argName1, argName2, ...)
```

----
#### `Variable` and `Member` 变量与成员变量

-  `Variable` 变量
```
	-- variable变量
	variableName = XXXXXX

	-- Const Variable 常量
    ConstVariableName = XXXXXX

    -- Global Variable 全局变量
    GlobalVariableName = XXXXXX
```

- `Member` 成员变量

```
	-- Define
	self.PublicVariableName = XXXXXX
	self._privateVariableName = XXXXXX

	-- Usage
	self.PublicVariableName
	self._privateVariableName
```

- `Static Member` 静态成员变量
```
	-- Define
	ClassName.PublicVariableName = XXXXXX
	ClassName._privateVariableName = XXXXXX

	-- Usage
	ClassName.PublicVariableName
	ClassName._privateVariableName
```

---
####  `Comment` 注释

-  `Single-Line` 单行注释
```
-- Single Line Conment
```

- `Multi-Line` 多行注释
```
--[[
    Multi-Line
    Conment
]]
```

 -  `TODO` 功能注释
```
-- TODO: XXXXXX
```

-  `FIXME` 提醒注释
```
-- FIXME: XXXXXX
```

---
####  Class 类

-  Sample Class `SampleClass.lua`
```
	-- TODO: require package
	local BasicClass = require "BasicClass"

	-- TODO: class define
	local This = BasicClass:subclass -- class interitance
	{
	    StaticVariable = nil, -- public static member variable
	    _privateStaticVariable = {},  -- private static member variable
	    ConstVariable = "Sample Const Variable", -- const variable
	}

	-- TODO: public static member function
	function This.GetStaticVariable (arg, ...)
	    return This._GetStaticVariable(arg, ...)
	end

	-- TODO: internal static member function
	function This._GetStaticVariable (arg, ...)
	    print(This.ConstVariable)
	    if not This.StaticVariable then -- test StaticVariable is nil
	        return This._InternalStaticVariable
	    end
	    return nil
	end

	-- TODO: class constructor
	function This:initialize (arg, ...)
	    self.PublicMember = arg
	    self._privateMember = {}
	end

	-- TODO: public member function
	function This:GetMember(arg, ...)
	    return self:_GetMember(arg, ...)
	end

	-- TODO: private member function
	function This:_GetMember(arg, ...)
	    print(This.ConstVariable) -- use const variable
	    if self.PublicMember then -- test publicMember is not nil
	        return self._privateMember
	    end
	    return nil
	end

	return This
```

- Test Sample Class
```
	-- TODO: Sample Code
	local SampleClass = require("SampleClass")

	SampleClass.GetStaticVariable() -- call public static member function
	local sample = SampleClass:new(0) -- new SampleClass
	sample:GetMember() -- call public member function
```

---
#### 使用规范
- 废弃`module`函数
        - 由于`module`函数有缺陷, 并且Lua后续的版本已经废弃`module`, 因此我们也不使用`module`
        - 对`module`函数的讨论 http://lua-users.org/wiki/LuaModuleFunctionCritiqued

- 使用`require`函数跨文件引用
	-`require`函数被重写, 不支持交叉引用

- Lua OOP 面向对象编程
	- http://lua-users.org/wiki/ObjectOrientedProgramming

