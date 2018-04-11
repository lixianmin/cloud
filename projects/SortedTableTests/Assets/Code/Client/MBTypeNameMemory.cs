
/********************************************************************
created:    2018-04-11
author:     lixianmin

type.FullName是有缓存的，速度更快，也不会有gcalloc

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unique.Collections;

namespace Client
{
    class MBTypeNameMemory : MonoBehaviour
    {
        private void Start ()
        {
            var count = 100;
            _types = TestTools.EnumerateTypes().Take(count).ToArray();
        }

        private void Update ()
        {
            _UpdateName();
            _UpdateFullName();
        }

        private void _UpdateName ()
        {
            foreach (var type in _types)
            {
                var name = type.Name;
            }
        }

        private void _UpdateFullName ()
        {
            foreach (var type in _types)
            {
                var fullname = type.FullName;
            }
        }


        private Type[] _types;
    }
}