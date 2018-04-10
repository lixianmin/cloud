
/********************************************************************
created:    2018-04-10
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Unique;
using UnityEngine;

namespace Client
{
    public class MBHashtableMemoryTest : MonoBehaviour
    {
        IEnumerator Start()
        {
            var testCount = 10000;
            var tableSize = 20;

            var types = new Type[testCount];
            var typeNames = new string[testCount];

            {
                var index = 0;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        types[index] = type;
                        typeNames[index] = type.Name;
                        ++index;

                        if (index == testCount)
                        {
                            break;
                        }
                    }

                    if (index == testCount)
                    {
                        break;
                    }
                }
            }

            var forceFullCollection = false;

            while (true)
            {
                var baseMemory = GC.GetTotalMemory(forceFullCollection);
                // arrays
                var arrays = new object[testCount];
                for (int i = 0; i < testCount; ++i)
                {
                    var array = new object[tableSize];
                    for (int j = 0; j < tableSize; ++j)
                    {
                        array[j] = new {type= types[j], name=typeNames[j]};
                    }

                    arrays[i] = array;
                }

                var afterArrayMemory = GC.GetTotalMemory(forceFullCollection);
                var averageArraySize = (afterArrayMemory - baseMemory)/testCount;

                // hashtables
                var hashtables = new object[testCount];
                for (int i = 0; i < testCount; ++i)
                {
                    var table = new Hashtable();
                    for (int j = 0; j < tableSize; ++j)
                    {
                        table.Add(types[j], typeNames[j]);
                    }

                    hashtables[i] = table;
                }

                var afterHashtableMemory = GC.GetTotalMemory(forceFullCollection);
                var averageHashtableSize = (afterHashtableMemory - afterArrayMemory)/testCount;

                // dictionarys
                var dicts = new object[testCount];
                for (int i = 0; i < testCount; ++i)
                {
                    var table = new Dictionary<Type, string>();
                    for (int j = 0; j < tableSize; ++j)
                    {
                        table.Add(types[j], typeNames[j]);
                    }

                    dicts[i] = table;
                }

                var afterDictMemory = GC.GetTotalMemory(forceFullCollection);
                var averageDictSize = (afterDictMemory - afterHashtableMemory)/testCount;

                yield return new WaitForSeconds(1.0f);

                arrays = null;
                hashtables = null;
                dicts = null;
                GC.Collect();
                yield return new WaitForSeconds(1.0f);

                Debug.LogFormat("averageArraySize ={0}k, averageHashtableSize = {1}k, averageDictSize={2}k, totalDelta={3}k"
                , averageArraySize / 1024.0f, averageHashtableSize / 1024.0f, averageDictSize / 1024.0f, (afterDictMemory-baseMemory)/1024.0f);

                yield return new WaitForSeconds(1.0f);
            }

        }
    }
}