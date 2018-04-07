

/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;

namespace ECS
{
    public class Entity
    {
        public IComponent AddComponent(Type type)
        {
            if (null != type)
            {
                var component = Activator.CreateInstance(type) as IComponent;
                if (null != component)
                {
                    var hasEntity = component as IHaveEntity;
                    if (null != hasEntity)
                    {
                        hasEntity.SetEntity(this);
                    }

                    var initializable = component as IInitalizable;
                    if (null != initializable)
                    {
                        initializable.Initalize();
                    }

                    _components.Add(type, component);
                    return component;
                }
            }

            return null;
        }

        public bool RemoveComponent(Type type)
        {
            if (null != type)
            {
                var component = _components[type];
                if (null != component)
                {
                    var disposable = component as IDisposable;
                    if (null != disposable)
                    {
                        disposable.Dispose();
                    }

                    _components.Remove(type);
                    return true;
                }
            }

            return false;
        }

        private readonly Hashtable _components = new Hashtable();
    }
}