
/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;

namespace ECS
{
    internal class ComponentTickSystem
    {
        static ComponentTickSystem()
        {

        }

        private ComponentTickSystem()
        {
            Entity.OnComponentCreated += _OnComponentCreated;
        }

        private void _OnComponentCreated(IComponent component)
        {
            if (component is ITickable && component is IIsDisposed)
            {
                _AddTickComponent(component);
            }
        }

        internal void Tick()
        {
            if (_hasNewPart)
            {
                Array.Sort(_typeIndices, _tickComponents);
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
                    var component = _tickComponents[i] as IIsDisposed;
                    if (!component.IsDisposed())
                    {
                        (component as ITickable).Tick();
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
                var component = _tickComponents[i] as IIsDisposed;
                if (component.IsDisposed())
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
                var component = _tickComponents[i] as IIsDisposed;
                if (component.IsDisposed())
                {
                    _tickComponents[i] = _tickComponents[j];
                    _typeIndices[i] = _typeIndices[j];

                    ++i;
                }
            }

            var removedCount = j - i;
            if (removedCount > 0)
            {
                Array.Clear(_tickComponents, i, removedCount);

                for (int k= 0; k < removedCount; ++k)
                {
                    _typeIndices[i+k] = _kMaxTypeIndex;
                }
            }

            _size = i;
        }

        private void _AddTickComponent(IComponent component)
        {
            if (_size == _capacity)
            {
                _capacity <<= 1;
                var tickParts = new IComponent[_capacity];
                var typeIndices = new int[_capacity];

                Array.Copy(_tickComponents, 0, tickParts, 0, _size);
                Array.Copy(_typeIndices, 0, typeIndices, 0, _size);

                for (int i= _size; i< _capacity; ++i)
                {
                    typeIndices[i] = _kMaxTypeIndex;
                }

                _tickComponents = tickParts;
                _typeIndices = typeIndices;
            }

            _tickComponents[_size] = component;
            _typeIndices[_size] = ComponentTypeIndices.SetDefaultTypeIndex(component.GetType());
            ++_size;

            _hasNewPart = true;
        }

        private const int _kMaxTypeIndex = 0x850506;
        private int[] _typeIndices = new int[]{_kMaxTypeIndex, _kMaxTypeIndex, _kMaxTypeIndex, _kMaxTypeIndex};
        private IComponent[] _tickComponents = new IComponent[4];
        private int _capacity = 4;
        private int _size = 0;
        private bool _hasNewPart;

        public static readonly ComponentTickSystem Instance = new ComponentTickSystem();
    }
}
