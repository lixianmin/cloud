

----
#### Android

1. CreateFromFile
    - 磁盘上小文件(<2M)默认使用这种加载方式；
    - 文件较大时加载会卡顿；
    - bundle.LoadAsset()耗时大；
    - bundle.LoadAssetAsync()耗时小，是LotWebPrefab的默认加载方式；

2. new WWW
    * 加载比较平缓，且比较慢；
    * 会产生一个巨大的WebStream：
        * 占用内存大小与ab文件在磁盘上的大小相同；
        * 即使调用www.Dispose()，该WebStream仍然存在，除非把bundle也销毁；

    * bundle.LoadAsset()耗时小；
    * **场景的bundle必须使用new WWW()加载**，测试WWW.LoadFromCacheOrDownload()加载mainAsset时会耗时12s
    * **new WWW()多了在android上会导致崩溃**；

3. LoadFromCacheOrDownload
    * apk内的小文件，以及LotFile默认使用这种加载方式；
    * 先解压到磁盘缓冲区，然后再从磁盘缓冲区中加载；
    * 怀疑从磁盘缓冲区加载时使用的就是CreateFromFile()：
        * 文件较大时跟CreateFromFile()一样会卡顿；
        * bundle.LoadAsset()跟CreateFromFile()一样耗时大；

----
#### iOS

1. CreateFromFile
    * 磁盘上小文件（<2M）默认使用这种加载方式；
    * 文件太大了加载会卡顿；
    * **当加载文件过多（<256）时会返回null**，此时退回到使用new WWW()加载；

2. new WWW
    * 加载比较平缓，且比较慢；
    * 有可能会产生一个巨大的WebStream，没有实测过；

3. LoadFromCacheOrDownload
    * iOS下不需要这种加载方式；
