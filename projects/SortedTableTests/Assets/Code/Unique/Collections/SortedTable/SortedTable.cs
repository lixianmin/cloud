
/*********************************************************************
created:    2014-05-26
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unique.Collections
{
    [DebuggerDisplay ("Count={Count}")]
    [Serializable]
	public partial class SortedTable<TKey, TValue> : 
            IDictionary<TKey, TValue>
			, IDictionary
            , ICollection<KeyValuePair<TKey, TValue>>
            , IEnumerable<KeyValuePair<TKey, TValue>>
            , ICollection
            , IEnumerable
	{
		public SortedTable ()
		{
			_keys   = _emptyKeys;
			_values = _emptyValues;
            _SetComparer(null);
        }

		public SortedTable (int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity should be greater than 0.");
			}

			_keys = new TKey[capacity];
			_values = new TValue[capacity];
			_capacity = capacity;
            _SetComparer(null);
		}

        public SortedTable (IComparer<TKey> comparer): this()
        {
            _SetComparer(comparer);
        }

        public SortedTable (int capacity, IComparer<TKey> comparer): this(comparer)
        {
            Capacity = capacity;
        }

        public SortedTable (IDictionary<TKey, TValue> dict): this(dict, null)
        {
        }

        public SortedTable (IDictionary<TKey, TValue> dict, IComparer<TKey> comparer): this((null != dict)? dict.Count: 0, comparer)
        {
            if (null == dict)
            {
                throw new ArgumentNullException("dict is null");
            }

			var count = dict.Count;
			_size = count;
			_keys = new TKey[count];
			_values = new TValue[count];

			// when we have contents, we hope _version != 0, for ClientTable may synchronize
			// contents depending on _version.
			++_version;

			var table = dict as SortedTable<TKey, TValue>;
			if (null != table)
			{
				Array.Copy(table._keys, _keys, count);
				Array.Copy(table._values, _values, count);
			}
			else
			{
				dict.Keys.CopyTo(_keys, 0);
				dict.Values.CopyTo(_values, 0);
				_Sort();
			}
        }

        private void _SetComparer (IComparer<TKey> inputComparer)
        {
			_comparer = inputComparer ?? Comparer<TKey>.Default;
        }
 
		public void Add (TKey key, TValue value)
		{
			if (_isKeyNullable && null == key)
			{
				throw new ArgumentNullException("key is null, value=" + value);
			}

			int index = _BinarySearch(key);
			if (index >= 0)
			{
				var oldValue = _values[index];
				var message= string.Format("Find duplicated key={0}, newValue={1}, oldValue={2}", key, value, oldValue);
				throw new ArgumentException(message);
			}

			InsertByIndex(~index, key, value);
		}

		public void Clear ()
		{
			++_version;

			if (_size > 0)
			{
				Array.Clear(_keys, 0, _size);
				Array.Clear(_values, 0, _size);
				_size = 0;
			}
		}

		public bool ContainsKey (TKey key)
		{
			if (!_isKeyNullable || null != key)
			{
				var index = _BinarySearch(key);
				var result = index >= 0;
				return result;
			}

			return false;
		}

		public bool ContainsValue (TValue value)
		{
			var index = Array.IndexOf(_values, value, 0, _size);
			return index >= 0;
		}

		public int IndexOfKey (TKey key)
		{
			if (_isKeyNullable && null == key)
			{
				throw new ArgumentNullException("key is null");
			}

			return _BinarySearch(key);
		}

		protected int _BinarySearch (TKey key)
		{
//			int i = -1;
//			int j = _size;
//			while (i + 1 != j)
//			{
//				int mid = i + (j - i >> 1);
//				if (_comparer.Compare (_keys [mid], key) < 0)
//				{
//					i = mid;
//				}
//				else
//				{
//					j = mid;
//				}
//			}
//			
//			if (j == _size || _comparer.Compare (_keys [j], key) != 0)
//			{
//				j = ~j;
//			}
//			
//			return j;
			// I choose the following algorithm but not the front one because
			// the following algorithm will call more less _comparer.Compare().

			int left  = 0;
			int right = _size - 1;
			
			while (left <= right)
			{
				int mid  = left + ((right - left) >> 1);
				int test = _comparer.Compare (key, _keys [mid]);
				
				if (test == 0)
				{
					return mid;
				}
				
				if (test < 0)
				{
					right = mid - 1;
				}
				else
				{
					left = mid + 1;
				}
			}
			
			return ~left;
		}

		public int IndexOfValue (TValue value)
		{
			return Array.IndexOf(_values, value, 0, _size);
		}

		public bool Remove (TKey key)
		{
			if (!_isKeyNullable || null != key)
			{
				int index = _BinarySearch(key);
				if (index >= 0)
				{
					RemoveAt(index);
				}
				
				var removed = index >= 0;
				return removed;
			}

			return false;
		}

		public void RemoveAt (int index)
		{
			if (index < 0 || index >= _size)
			{
				throw new ArgumentOutOfRangeException("index is out of range.");
			}

			--_size;

			if (index < _size)
			{
				Array.Copy(_keys, index + 1, _keys, index, _size - index);
				Array.Copy(_values, index + 1, _values, index, _size - index);
			}

			_keys[_size] = default(TKey);
			_values[_size] = default(TValue);
			++_version;
		}

		public int RemoveAll (Func<TKey, TValue, bool> match)
		{
			if (null == match)
			{
				throw new ArgumentNullException("match is null");
			}

			int i;
			for (i = 0; i < _size; i++)
			{
				if (match (_keys[i], _values[i]))
				{
					break;
				}
			}

			if (i == _size)
			{
				return 0;
			}

			_version++;

			int j;
			for (j = i + 1; j < _size; j++)
			{
				if (!match (_keys[j], _values[j]))
				{
					_keys[i]	= _keys[j];
					_values[i]	= _values[j];

					++i;
				}
			}

            var removedCount = j - i;
            if (removedCount > 0)
			{
                Array.Clear (_keys, i, removedCount);
                Array.Clear (_values, i, removedCount);
			}

			_size = i;

			return removedCount;
		}

		public void TrimExcess ()
		{
			int num = (int) ((double) _keys.Length * 0.9);

			if (_size < num)
			{
				Capacity = _size;
			}
		}

		public int TryIndexValue (TKey key, out TValue value)
		{
			if (_isKeyNullable && null == key)
			{
				throw new ArgumentNullException("key is null");
			}
			
			int index = _BinarySearch(key);
			if (index >= 0)
			{
				value = _values[index];
			}
			else
			{
				value = default(TValue);
			}
			
			return index;
		}

		public bool TryGetValue (TKey key, out TValue value)
		{
			if (_isKeyNullable && null == key)
			{
				throw new ArgumentNullException("key is null");
			}
			
			int index = _BinarySearch(key);
			if (index >= 0)
			{
				value = _values[index];
				return true;
			}
			else
			{
				value = default(TValue);
				return false;
			}			
		}

		public void ForEach (Action<KeyValuePair<TKey, TValue>> action)
		{
			if (null != action)
			{
				for (int i= 0; i< _size; ++i)
				{
					var pair = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
					action(pair);
				}
			}
		}

        public void Merge (SortedTable<TKey, TValue> other)
        {
            if (null == other || other.Count == 0)
            {
                return;
            }

            var counter1 = Count;
            var counter2 = other.Count;
            var capacity = counter1 + counter2;

            var keys = new TKey[capacity];
            var values = new TValue[capacity];

            int currentIndex = 0;
            int index1 = 0;
            int index2 = 0;

            while (index1 < counter1 && index2 < counter2)
            {
                var key1 = _keys[index1];
                var key2 = other._keys[index2];

                var flags = _comparer.Compare(key1, key2);
                if (flags < 0)
                {
                    keys[currentIndex] = key1;
                    values[currentIndex] = _values[index1];
                    ++index1;
                }
                else if (flags > 0)
                {
                    keys[currentIndex] = key2;
                    values[currentIndex] = other._values[index2];
                    ++index2;
                }
                else
                {
                    keys[currentIndex] = key2;
                    values[currentIndex] = other._values[index2];
                    ++index2;
                    ++index1;
                }

                ++currentIndex;
            }

            while (index1 < counter1)
            {
                keys[currentIndex] = _keys[index1];
                values[currentIndex] = _values[index1];
                ++index1;
                ++currentIndex;
            }

            while (index2 < counter2)
            {
                keys[currentIndex] = other._keys[index2];
                values[currentIndex] = other._values[index2];
                ++index2;
                ++currentIndex;
            }

            _keys = keys;
            _values = values;
            _size = currentIndex;
            _capacity = capacity;
            ++_version;
        }

        /// <summary>
        /// This method exists for supporting XmlSerializer
        /// </summary>
        /// <param name="pair">Pair.</param>
        public void Add (KeyValuePair<TKey, TValue> pair)
        {
            Add (pair.Key, pair.Value);
        }

        public Enumerator GetEnumerator ()
        {
            return new Enumerator(this);
        }

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator ()
		{
			return _GetEnumerator();
        }
		
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return _GetEnumerator();
        }

		private IEnumerator<KeyValuePair<TKey, TValue>> _GetEnumerator ()
		{
			var lastVersion = _version;

			for (int i= 0; i< _size; ++i)
			{
				var pair = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
				yield return pair;

				if (lastVersion != _version)
				{
					throw new InvalidOperationException("Invalid table version");
				}
			}
		}

		public TValue this [TKey key]
		{
			get 
			{
				if (_isKeyNullable && null == key)
				{
					throw new ArgumentNullException("key is null");
				}

				int index = _BinarySearch (key);
				if (index >= 0)
				{
					return _values[index];
				}

				throw new KeyNotFoundException("key =" + key);
			}

			set 
			{
				if (_isKeyNullable && null == key)
				{
					throw new ArgumentNullException("key is null");
				}

				int index = _BinarySearch(key);
				if(index >= 0)
				{
					_values[index] = value;
					++_version;
					return;
				}

				InsertByIndex(~index, key, value);
			}
		}

		protected void _EnsureCapacity (int min)
		{
			var num = (_keys.Length == 0) ? 4 : (_keys.Length * 2);
			const int max = 2146435071;
			if (num > max)
			{
				num = max;
			}
			else if (num < min)
			{
				num = min;
			}

			Capacity = num;
		}

		public int Capacity
		{
			get 
			{
				return _capacity;
			}

			set 
			{
				if (value != _keys.Length)
				{
					if (value < _size)
					{
						throw new ArgumentOutOfRangeException("New capacity is smaller than already.");
					}

					if (value > 0)
					{
						var destKeys = new TKey[value];
						var destValues = new TValue[value];

						if (_size > 0)
						{
							Array.Copy(_keys, 0, destKeys, 0, _size);
							Array.Copy(_values, 0, destValues, 0, _size);
						}

						_keys = destKeys;
						_values = destValues;
						_capacity = value;
					}
					else
					{
						_keys = _emptyKeys;
						_values = _emptyValues;
						_capacity = 0;
					}
				}
			}
		}

        public override string ToString ()
        {
            var sbText = new System.Text.StringBuilder(1024);
            sbText.Append("[SortedTable] Capacity= ");
            sbText.Append(Capacity);
            
            sbText.Append(", ");
            sbText.Append("Count= ");
            sbText.Append(Count);
            
//            sbText.Append(", ");
//            sbText.Append("Comparer= ");
//            sbText.Append(Comparer);
//            
//            sbText.Append(", ");
//            sbText.Append("Items= ");
//            sbText.Append("\n");
//            
//            var e = GetEnumerator();
//            while (e.MoveNext())
//            {
//                var item = e.Current;
//                sbText.Append("[");
//                sbText.Append(item.Key);
//                sbText.Append(", ");
//                sbText.Append(item.Value);
//                sbText.Append("]\n");
//            }
            
            return sbText.ToString();
        }

		public int              Count       { get { return _size; } }
        public KeyList    		Keys        { get { return new KeyList(this); } }
        public ValueList  		Values      { get { return new ValueList(this); } }
        public IComparer<TKey>  Comparer    { get { return _comparer; }}

		protected TKey[] _keys;
		protected TValue[] _values;
		protected int _size;
		protected int _capacity;
		protected IComparer<TKey> _comparer;
		protected int _version;

		private static readonly TKey[] _emptyKeys = new TKey[0];
		private static readonly TValue[] _emptyValues = new TValue[0];
		private static readonly bool _isKeyNullable = !typeof(TKey).IsSubclassOf(typeof(ValueType));
	}
}