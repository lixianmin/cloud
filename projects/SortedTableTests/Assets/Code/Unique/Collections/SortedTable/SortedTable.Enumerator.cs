
/*********************************************************************
created:    2014-08-11
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
        [Serializable]
		public struct Enumerator : IDictionaryEnumerator, IEnumerator
		{
            internal Enumerator (SortedTable<TKey, TValue> table)
			{
				_table = table;
				_index = 0;

				_pair  = default(KeyValuePair<TKey, TValue>);
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
					var key = _table._keys[_index];
					var val = _table._values[_index];
					_pair = new KeyValuePair<TKey, TValue>(key, val);
					++_index;

					return true;
				}

				_index = count + 1;
				_pair  = default(KeyValuePair<TKey, TValue>);
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
				_pair  = default(KeyValuePair<TKey, TValue>);
			}

            public KeyValuePair<TKey, TValue> Current
			{
				get 
				{
					return _pair;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return (this as IDictionaryEnumerator).Entry;
				}
			}

			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					return new DictionaryEntry(_pair.Key, _pair.Value);
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					return _pair.Key;
				}
			}
			
			object IDictionaryEnumerator.Value
			{
				get
				{
					return _pair.Value;
				}
			}

            private readonly SortedTable<TKey, TValue> _table;
			private int _index;

			private KeyValuePair<TKey, TValue> _pair;
            private int _version;
		}
	}
}