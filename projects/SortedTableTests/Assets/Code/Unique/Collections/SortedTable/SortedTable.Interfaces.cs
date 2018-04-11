
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
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains (KeyValuePair<TKey, TValue> pair)
        {
            int index = IndexOfKey(pair.Key);
            return index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], pair.Value);
        }
        
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove (KeyValuePair<TKey, TValue> pair)
        {
            int index = IndexOfKey(pair.Key);
            if (index >= 0 && EqualityComparer<TValue>.Default.Equals(_values[index], pair.Value))
            {
                RemoveAt(index);
                return true;
            }
            
            return false;
        }
        
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo (KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (null == array)
            {
                throw new ArgumentNullException("array is null");
            }
            
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                var text = string.Format("ArrayIndex = {0}, array.Length={1}", arrayIndex, array.Length);
                throw new ArgumentOutOfRangeException(text);
            }
            
            if (array.Length - arrayIndex < Count)
            {
                var text = string.Format("ArrayIndex = {0}, array.Length={1}", arrayIndex, array.Length);
                throw new ArgumentException(text);
            }
            
            for (int i= 0; i < Count; ++i)
            {
                var pair = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
                array[arrayIndex + i] = pair;
            }
        }

		void IDictionary.Add (object key, object value)
		{
			this.Add (_ToTKey (key), _ToTValue (value));
		}

		bool IDictionary.Contains (object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException ("key");
			}
			return key is TKey && this.ContainsKey ((TKey)((object)key));
		}

		void IDictionary.Remove (object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException ("key");
			}
			if (key is TKey)
			{
				this.Remove ((TKey)((object)key));
			}
		}

		IDictionaryEnumerator IDictionary.GetEnumerator ()
		{
			return new SortedTable<TKey, TValue>.Enumerator (this);
		}

		object IDictionary.this [object key]
		{
			get
			{
				if (key is TKey && this.ContainsKey ((TKey)((object)key)))
				{
					return this [_ToTKey (key)];
				}
				return null;
			}
			set
			{
				this [_ToTKey (key)] = _ToTValue (value);
			}
		}

		private TKey _ToTKey (object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException ("key");
			}
			if (!(key is TKey))
			{
				throw new ArgumentException ("not of type: " + typeof(TKey).ToString (), "key");
			}

			return (TKey)((object)key);
		}

		private TValue _ToTValue (object value)
		{
			if (value == null && !typeof(TValue).IsValueType)
			{
				return default(TValue);
			}
			if (!(value is TValue))
			{
				throw new ArgumentException ("not of type: " + typeof(TValue).ToString (), "value");
			}

			return (TValue)((object)value);
		}

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        { 
            get 
            { 
                return false;
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get 
            {
				return this.Keys;
            }
        }

		ICollection IDictionary.Keys
		{
			get
			{
				return this.Keys;
			}
		}
        
        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get 
            {
				return this.Values;
            }
        }

		ICollection IDictionary.Values
		{
			get
			{
				return this.Values;
			}
		}

        void ICollection.CopyTo (Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException ("array");
            }

            if (array.Rank > 1 || array.GetLowerBound (0) != 0)
            {
                throw new ArgumentException ("Array must be zero based and single dimentional", "array");
            }

            var count = this._size;
            for (int i= 0; i< count; ++i)
            {
                var item = new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
                array.SetValue(item, i + arrayIndex);
            }
        }

		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}
		
		bool IDictionary.IsReadOnly
		{
			get
			{
				return false;
			}
		}

        bool ICollection.IsSynchronized
        {
            get 
            {
                return false;
            }
        }
        
        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }
	}
}