
/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;

namespace Unique
{
    internal class PartTickSystem
    {
        static PartTickSystem()
        {

        }

        private PartTickSystem()
        {
            Entity.OnPartCreated += _OnPartCreated;
        }

        private void _OnPartCreated(IPart part)
        {
            if (part is ITickable && part is IIsDisposed)
            {
                _AddTickPart(part);
            }
        }

        internal void Tick()
        {
            if (_hasNewPart)
            {
                Array.Sort(_typeIndices, _tickParts);
                _hasNewPart = false;
            }

            var hasDisposed = _TickParts();

            if (hasDisposed)
            {
                _RemoveDisposedParts();
            }
        }

        private bool _TickParts()
        {
            var count = _size;
            if (count > 0)
            {
                var hasDisposed = false;
                for (int i = 0; i < count; ++i)
                {
                    var part = _tickParts[i] as IIsDisposed;
                    if (!part.IsDisposed())
                    {
                        (part as ITickable).Tick();
                    }
                    else
                    {
                        hasDisposed = true;
                    }
                }

                return hasDisposed;
            }

            return false;
        }

        private void _RemoveDisposedParts()
        {
            int i;
            for (i = 0; i < _size; i++)
            {
                var part = _tickParts[i] as IIsDisposed;
                if (part.IsDisposed())
                {
                    break;
                }
            }

            if (i == _size)
            {
                return;
            }

            int j;
            for (j = i + 1; j < _size; j++)
            {
                var part = _tickParts[i] as IIsDisposed;
                if (part.IsDisposed())
                {
                    _tickParts[i] = _tickParts[j];
                    _typeIndices[i] = _typeIndices[j];

                    ++i;
                }
            }

            var removedCount = j - i;
            if (removedCount > 0)
            {
                Array.Clear(_tickParts, i, removedCount);

                for (int k= 0; k < removedCount; ++k)
                {
                    _typeIndices[i+k] = _kMaxTypeIndex;
                }
            }

            _size = i;
        }

        private void _AddTickPart(IPart part)
        {
            if (_size == _capacity)
            {
                _capacity <<= 1;
                var tickParts = new IPart[_capacity];
                var typeIndices = new int[_capacity];

                Array.Copy(_tickParts, 0, tickParts, 0, _size);
                Array.Copy(_typeIndices, 0, typeIndices, 0, _size);

                for (int i= _size; i< _capacity; ++i)
                {
                    typeIndices[i] = _kMaxTypeIndex;
                }

                _tickParts = tickParts;
                _typeIndices = typeIndices;
            }

            _tickParts[_size] = part;
            _typeIndices[_size] = PartTypeIndices.SetDefaultTypeIndex(part.GetType());
            ++_size;

            _hasNewPart = true;
        }

        private const int _kMaxTypeIndex = 0x850506;
        private int[] _typeIndices = new int[]{_kMaxTypeIndex, _kMaxTypeIndex, _kMaxTypeIndex, _kMaxTypeIndex};
        private IPart[] _tickParts = new IPart[4];
        private int _capacity = 4;
        private int _size = 0;
        private bool _hasNewPart;

        public static readonly PartTickSystem Instance = new PartTickSystem();
    }
}
