
/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unique
{
    public class Part : IInitalizable, IDisposable, IIsDisposed, IHaveEntity
    {
        void IInitalizable.Initalize()
        {
            DoInitialize();
        }

        void IDisposable.Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            DoDispose();
        }

        public bool IsDisposed()
        {
            return _isDisposed;
        }

        public Entity GetEntity()
        {
            return _entity;
        }

        void IHaveEntity.SetEntity(Entity entity)
        {
            _entity = entity;
        }

        protected virtual void DoInitialize() { }
        protected virtual void DoDispose() { }

        private Entity _entity;
        private bool _isDisposed;
    }
}