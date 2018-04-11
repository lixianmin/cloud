
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
        public struct ValueList : IList<TValue>, IList, ICollection<TValue>, ICollection, IEnumerable<TValue>, IEnumerable
        {
            internal ValueList (SortedTable<TKey, TValue> table)
            {
                _table = table;
            }

			public void ForEach (Action<TValue> action)
			{
				if (null != action && null != _table)
				{
					var values  = _table._values;
					var count	= _table._size;

					for (int i= 0; i< count; ++i)
					{
						action(values[i]);
					}
				}
			}
			            
            public ValueEnumerator GetEnumerator ()
            {
                return new ValueEnumerator(_table);
            }
            
            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator ()
            {
				return _GetEnumerator();
            }
            
            IEnumerator IEnumerable.GetEnumerator ()
            {
				return _GetEnumerator();
            }

			private IEnumerator<TValue> _GetEnumerator ()
			{
				var lastVersion = _table._version;
				var count = _table._size;
				var values = _table._values;

				for (int i= 0; i< count; ++i)
				{
					yield return values[i];

					if (lastVersion != _table._version)
					{
						throw new InvalidOperationException("Invalid table version");
					}
				}
			}

			public TValue this [int index]
			{
				get 
				{
					var count = _table._size;
					if (index >= 0 && index < count)
					{
						return _table._values[index];
					}

					var message = string.Format("[ValueList.get_Item()] index={0}, count={1}", index.ToString(), count.ToString());
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

			public int IndexOf (TValue item)
			{
				var values = _table._values;
				var count  = _table._size;

				for (int index= 0; index< count; ++index)
				{
					if (object.Equals(values[index], item))
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
					return this.IndexOf ((TValue)((object)item));
				}
				catch (NullReferenceException)
				{
				}
				catch (InvalidCastException)
				{
				}
				return -1;
			}

			void IList<TValue>.Insert (int index, TValue item)
			{
				throw new NotImplementedException();
			}

			void IList.Insert (int index, object item)
			{
				throw new NotImplementedException();
			}

			void IList<TValue>.RemoveAt (int index)
			{
				throw new NotImplementedException();
			}

			void IList.RemoveAt (int index)
			{
				throw new NotImplementedException();
			}

			void ICollection<TValue>.Add (TValue item)
			{
				throw new NotImplementedException();
			}

			int IList.Add (object item)
			{
				throw new NotImplementedException();
			}

			public bool Contains (TValue item)
			{
				return IndexOf(item) != -1;
			}

			bool IList.Contains (object item)
			{
				try
				{
					return this.Contains ((TValue)((object)item));
				}
				catch (NullReferenceException)
				{
				}
				catch (InvalidCastException)
				{
				}
				return false;
			}

			bool ICollection<TValue>.Remove (TValue item)
			{
				throw new NotImplementedException();
			}

			void IList.Remove (object item)
			{
				throw new NotImplementedException();
			}

			public void CopyTo (TValue[] array)
			{
				Array.Copy (_table._values, 0, array, 0, _table._size);
			}

			public void CopyTo (TValue[] array, int arrayIndex)
			{
				Array.Copy (_table._values, 0, array, arrayIndex, _table._size);
			}

			void ICollection.CopyTo (Array array, int arrayIndex)
			{
				Array.Copy (_table._values, 0, array, arrayIndex, _table._size);
			}

			public TValue[] ToArray ()
			{
				var count = _table._size;
				if (count > 0)
				{
					var array = new TValue[count];
					Array.Copy(_table._values, 0, array, 0, count);
					
					return array;
				}

				return _emptyValues;
			}

			void ICollection<TValue>.Clear ()
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

			bool ICollection<TValue>.IsReadOnly
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