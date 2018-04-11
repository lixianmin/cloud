
#pragma warning disable 0219
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unique.Collections;
using System;
using System.Diagnostics;

namespace Client
{

    public class MBHashtableSpeedTest : MonoBehaviour
    {
        private void Start()
        {
            _InitContainers();

            for (int i = 0; i < 10; ++i)
            {
                _OneTest(10000);
            }
        }

        private void _InitContainers()
        {
            var size = 100;
            _typeArray = TypeTools.EnumerateTypes().Take(size).ToArray();
            _dict = new Dictionary<Type, object>(size);
            _hashtable = new Hashtable(size);

            for (int i = 0; i < size; ++i)
            {
                var type = _typeArray[i];
                _dict[type] = type;
                _hashtable[type] = type;
            }
        }

        private void Update()
        {

        }

        private void _OneTest(int repeat)
        {
            var typeArrayTime = _RunByWatch(repeat, () => _TestTypeArray());
            var dictTime = _RunByWatch(repeat, () => _TestDict()) - typeArrayTime;
            var hashtableTime = _RunByWatch(repeat, () => _TestHashtable()) - typeArrayTime;

            UnityEngine.Debug.LogFormat("typeArrayTime={0}, dictTime={1}, hashtableTime={2}",
                typeArrayTime, dictTime, hashtableTime);

            UnityEngine.Debug.LogFormat("dictRatio={0}, hashtableRatio={1}",
                dictTime / typeArrayTime, hashtableTime / typeArrayTime);
        }

        private void _TestTypeArray()
        {
            var count = _typeArray.Length;
            for (int i = 0; i < count; ++i)
            {
                var type = _typeArray[i];
            }
        }

        private void _TestDict()
        {
            var count = _typeArray.Length;
            for (int i = 0; i < count; ++i)
            {
                var type = _typeArray[i];
                var result = _dict[type];
            }
        }

        private void _TestHashtable()
        {
            var count = _typeArray.Length;
            for (int i = 0; i < count; ++i)
            {
                var type = _typeArray[i];
                var result = _hashtable[type];
            }
        }

        private float _RunByWatch(int repeat, Action action)
        {
            var watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < repeat; ++i)
            {
                action();
            }
            watch.Stop();

            var seconds = watch.ElapsedMilliseconds * 0.001f;
            return seconds;
        }

        private Type[] _typeArray;
        private Dictionary<Type, object> _dict;
        private Hashtable _hashtable;
    }
}