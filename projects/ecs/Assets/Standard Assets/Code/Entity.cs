

/********************************************************************
created:    2018-04-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;

namespace Unique
{
    public class Entity
    {
        public IPart AddPart(Type type)
        {
            if (null != type)
            {
                var part = Activator.CreateInstance(type) as IPart;
                if (null != part)
                {
                    var haveEntity = part as IHaveEntity;
                    if (null != haveEntity)
                    {
                        haveEntity.SetEntity(this);
                    }

                    var initializable = part as IInitalizable;
                    if (null != initializable)
                    {
                        initializable.Initalize();
                    }

                    _parts.Add(type, part);
                    if (null != OnPartCreated)
                    {
                        OnPartCreated(part);
                    }
                    return part;
                }
            }

            return null;
        }

        public bool RemovePart(Type type)
        {
            if (null != type)
            {
                var part = _parts[type];
                if (null != part)
                {
                    var disposable = part as IDisposable;
                    if (null != disposable)
                    {
                        disposable.Dispose();
                    }

                    _parts.Remove(type);
                    return true;
                }
            }

            return false;
        }

        public static event Action<IPart> OnPartCreated;
        private readonly Hashtable _parts = new Hashtable();
    }
}