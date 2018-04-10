
/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unique
{
    public class Part : IPart, IInitPart, IDisposable, IIsDisposed
    {
        void IInitPart.InitPart(Entity entity)
        {
            _entity = entity;
            _DoInitPart();
        }

        void IDisposable.Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _DoDispose();
        }

        public bool IsDisposed()
        {
            return _isDisposed;
        }

        public Entity GetEntity()
        {
            return _entity;
        }

        protected virtual void _DoInitPart() { }
        protected virtual void _DoDispose() { }

        private Entity _entity;
        private bool _isDisposed;
    }
}