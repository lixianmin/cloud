
/*********************************************************************
created:    2014-05-26
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
		public struct ValueEnumerator
		{
			public ValueEnumerator (SortedTable<TKey, TValue> table)
			{
				_table = table;
				_index = 0;

				_value = default(TValue);
				_version = _table._version;
			}
			
			public bool MoveNext ()
			{
				if (null == _table)
				{
					return false;
				}

				if (_version != _table._version)
				{
					throw new InvalidOperationException("Invalid table version");
				}

				var count = _table._size;
				if (_index < count)
				{
					_value = _table._values[_index];
					++_index;

					return true;
				}
				
				_index = count + 1;
				_value = default(TValue);
				return false;
			}
			
			public void Reset ()
			{
				if (null == _table)
				{
					return;
				}

				if (_version != _table._version)
				{
					throw new InvalidOperationException("Invalid table version");
				}
				
				_index = 0;
				_value = default(TValue);
			}

			public TValue Current
			{
				get 
				{
					return _value;
				}
			}
			
			private readonly SortedTable<TKey, TValue> _table;
			private int _index;

			private TValue _value;
			private int _version;
		}
	}
}