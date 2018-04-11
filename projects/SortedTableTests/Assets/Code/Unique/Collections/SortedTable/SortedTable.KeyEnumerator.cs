
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
		public struct KeyEnumerator
		{
			internal KeyEnumerator (SortedTable<TKey, TValue> table)
			{
				_table = table;
				_index = 0;

				_key = default(TKey);
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
					_key = _table._keys[_index];
					++_index;

					return true;
				}
				
				_index = count + 1;
				_key = default(TKey);
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
				_key = default(TKey);
			}
			
			public TKey Current
			{
				get 
				{
					return _key;
				}
			}
			
			private readonly SortedTable<TKey, TValue> _table;
			private int _index;

			private TKey _key;
			private int _version;
		}
	}
}