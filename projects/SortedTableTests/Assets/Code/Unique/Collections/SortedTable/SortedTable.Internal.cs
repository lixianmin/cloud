
/*********************************************************************
created:    2017-04-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;

namespace Unique.Collections
{
	partial class SortedTable<TKey, TValue>
	{
        public void InsertByIndex (int index, TKey key, TValue val)
		{
			if (index < 0)
			{
				var message = string.Format("[Insert()] index < 0, index={0}", index);
				throw new ArgumentOutOfRangeException(message);
			}

			if (_size == _keys.Length)
			{
				_EnsureCapacity(_size + 1);
			}

			if (index < _size)
			{
				Array.Copy(_keys, index, _keys, index + 1, _size - index);
				Array.Copy(_values, index, _values, index + 1, _size - index);
			}

			_keys[index]= key;
			_values[index] = val;
			++_size;
			++_version;
		}

        public void _Append (TKey key, TValue val)
		{
			if (_isKeyNullable && null == key)
			{
				throw new ArgumentNullException("key is null");
			}

			if (_size == _capacity)
			{
				_EnsureCapacity(_size + 1);
			}

			var index = _size;
			_keys[index]= key;
			_values[index] = val;
			++_size;
			++_version;
		}

        public void _Sort ()
		{
			Array.Sort(_keys, _values, 0, _size, _comparer);
		}
	}
}