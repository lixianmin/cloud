
/********************************************************************
created:    2018-04-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Client
{
    public class TypeComparer : IComparer<Type>
    {
        private TypeComparer()
        {

        }

        public int Compare(Type lhs, Type rhs)
        {
            var lhsFullName = lhs.FullName;
            var rhsFullName = rhs.FullName;

            var lhsLength = lhsFullName.Length;
            var rhsLength = rhsFullName.Length;
            if (lhsLength < rhsLength)
            {
                return -1;
            }
            else if (lhsLength > rhsLength)
            {
                return 1;
            }

            return string.CompareOrdinal(lhsFullName, rhsFullName);
        }

        public static readonly TypeComparer Instance = new TypeComparer();
    }
}