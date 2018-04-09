
/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;

namespace Unique
{
    public static class PartTypeIndices
    {
        public static int SetDefaultTypeIndex(Type type)
        {
            int typeIndex;
            if (!_typeIndices.TryGetValue(type, out typeIndex))
            {
                typeIndex = ++_typeGenerator;
                _typeIndices[type] = typeIndex;
            }

            return typeIndex;
        }

        private static int _typeGenerator;
        private static readonly Dictionary<Type, int> _typeIndices = new Dictionary<Type, int>();
    }
}
