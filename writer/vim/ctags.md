
---
ctags对lua的支持很弱，需要手动自定义规则：

```
--regex-LUA=/^.*\s*function[ \t]*([a-zA-Z0-9_]+):([a-zA-Z0-9_]+).*$/\2/f,function/
--regex-LUA=/^.*\s*function[ \t]*([a-zA-Z0-9_]+)\.([a-zA-Z0-9_]+).*$/\2/f,function/
--regex-LUA=/^.*\s*function[ \t]*([a-zA-Z0-9_]+)\s*\(.*$/\1/f,function/

--regex-LUA=/([a-zA-Z0-9_]+) = require[ (]"([^"]+)"/\1/r,require/

--regex-LUA=/[ \t]{1}([a-zA-Z0-9_]+)[ \t]*[=][^=]/\1/v,variable/

--regex-LUA=/[ \t]*([a-zA-Z0-9_]+)[ \t]*=[ \t]*module_define.*$/\1/m,module/
--regex-LUA=/func_table\[ msg\.([A-Z_]+) \].+/\1/
--regex-LUA=/\([ \t]*msg.([A-Z_]+)[ \t]*\)/\1/

```

作者使用的是mac osx系统，将以上代码附加到~/.ctags尾部即可

其它操作系统可以参考[ctags lua规则增强](https://gist.github.com/yongkangchen/10120546)

----
#### reference

1. http://ctags.sourceforge.net/ctags.html
