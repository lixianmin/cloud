
/********************************************************************
created:    2018-04-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;

namespace Client
{
    public static class TestTools
    {
        public static IEnumerable<Type> EnumerateTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    yield return type;
                }
            }
        }
    }
}