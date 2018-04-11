
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
        [Serializable]
		public struct KeyList : IList<TKey>, IList, ICollection<TKey>, ICollection, IEnumerable<TKey>, IEnumerable
        {
            internal KeyList (SortedTable<TKey, TValue> table)
            {
                _table = table;
            }

			public void ForEach (Action<TKey> action)
			{
				if (null != action && null != _table)
				{
					var keys  = _table._keys;
					var count = _table._size;
					for (int i= 0; i< count; ++i)
					{
						action(keys[i]);
					}
				}
			}

            public KeyEnumerator GetEnumerator ()
            {
                return new KeyEnumerator(_table);
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator ()
            {
				return _GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator ()
            {
				return _GetEnumerator();
            }

			private IEnumerator<TKey> _GetEnumerator ()
			{
				var lastVersion = _table._version;
				var count = _table._size;
				var keys = _table._keys;

				for (int i= 0; i< count; ++i)
				{
					yield return keys[i];

					if (lastVersion != _table._version)
					{
						throw new InvalidOperationException("Invalid table version");
					}
				}
			}

			public TKey this [int index]
			{
				get 
				{
					var count = _table._size;
					if (index >= 0 && index < count)
					{
						return _table._keys[index];
					}
					
					var message = string.Format("[KeyList.get_Item()] index={0}, count={1}", index.ToString(), count.ToString());
					throw new IndexOutOfRangeException(message);
				}

				set
				{
					throw new NotImplementedException();
				}
			}

			object IList.this [int index]
			{
				get
				{
					return this [index];
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public int IndexOf (TKey item)
			{
				var keys  = _table._keys;
				var count = _table._size;
				
				for (int index= 0; index< count; ++index)
				{
					if (object.Equals(keys[index], item))
					{
						return index;
					}
				}
				
				return -1;
			}

			int IList.IndexOf (object item)
			{
				try
				{
					return this.IndexOf ((TKey)((object)item));
				}
				catch (NullReferenceException)
				{
				}
				catch (InvalidCastException)
				{
				}
				return -1;
			}
			
			void IList<TKey>.Insert (int index, TKey item)
			{
				throw new NotImplementedException();
			}

			void IList.Insert (int index, object item)
			{
				throw new NotImplementedException();
			}
			
			void IList<TKey>.RemoveAt (int index)
			{
				throw new NotImplementedException();
			}

			void IList.RemoveAt (int index)
			{
				throw new NotImplementedException();
			}

			void ICollection<TKey>.Add (TKey item)
			{
				throw new NotImplementedException();
			}

			int IList.Add (object item)
			{
				throw new NotImplementedException();
			}
			
			public bool Contains (TKey item)
			{
				return IndexOf(item) != -1;
			}

			bool IList.Contains (object item)
			{
				try
				{
					return this.Contains ((TKey)((object)item));
				}
				catch (NullReferenceException)
				{
				}
				catch (InvalidCastException)
				{
				}
				return false;
			}

			bool ICollection<TKey>.Remove (TKey item)
			{
				throw new NotImplementedException();
			}

			void IList.Remove (object item)
			{
				throw new NotImplementedException();
			}

			public void CopyTo (TKey[] array)
			{
				Array.Copy (_table._keys, 0, array, 0, _table._size);
			}

			public void CopyTo (TKey[] array, int arrayIndex)
			{
				Array.Copy (_table._keys, 0, array, arrayIndex, _table._size);
			}

			void ICollection.CopyTo (Array array, int arrayIndex)
			{
				Array.Copy (_table._keys, 0, array, arrayIndex, _table._size);
			}

			public TKey[] ToArray ()
			{
				var count = _table._size;
				if (count > 0)
				{
					var array = new TKey[count];
					Array.Copy(_table._keys, 0, array, 0, count);
					
					return array;
				}
				
				return _emptyKeys;
			}

			void ICollection<TKey>.Clear ()
			{
				throw new NotImplementedException();
			}

			void IList.Clear ()
			{
				throw new NotImplementedException();
			}

			bool IList.IsFixedSize
			{
				get
				{
					return false;
				}
			}
			
			bool IList.IsReadOnly
			{
				get
				{
					return false;
				}
			}

			bool ICollection<TKey>.IsReadOnly
			{
				get
				{
					return true;
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
			
			public int Count { get { return _table._size; } }

            private readonly SortedTable<TKey, TValue> _table;
        }
	}
}