//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.1.4322.573
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

namespace Server.Engines.Reports
{
    using System;
    using System.Collections;
    
    
    /// <summary>
    /// Strongly typed collection of Server.Engines.Reports.Snapshot.
    /// </summary>
    public class SnapshotCollection : System.Collections.CollectionBase
    {
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SnapshotCollection() : 
                base()
        {
        }
        
        /// <summary>
        /// Gets or sets the value of the Server.Engines.Reports.Snapshot at a specific position in the SnapshotCollection.
        /// </summary>
        public Server.Engines.Reports.Snapshot this[int index]
        {
            get
            {
                return ((Server.Engines.Reports.Snapshot)(this.List[index]));
            }
            set
            {
                this.List[index] = value;
            }
        }
        
        /// <summary>
        /// Append a Server.Engines.Reports.Snapshot entry to this collection.
        /// </summary>
        /// <param name="value">Server.Engines.Reports.Snapshot instance.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(Server.Engines.Reports.Snapshot value)
        {
            return this.List.Add(value);
        }
        
        /// <summary>
        /// Determines whether a specified Server.Engines.Reports.Snapshot instance is in this collection.
        /// </summary>
        /// <param name="value">Server.Engines.Reports.Snapshot instance to search for.</param>
        /// <returns>True if the Server.Engines.Reports.Snapshot instance is in the collection; otherwise false.</returns>
        public bool Contains(Server.Engines.Reports.Snapshot value)
        {
            return this.List.Contains(value);
        }
        
        /// <summary>
        /// Retrieve the index a specified Server.Engines.Reports.Snapshot instance is in this collection.
        /// </summary>
        /// <param name="value">Server.Engines.Reports.Snapshot instance to find.</param>
        /// <returns>The zero-based index of the specified Server.Engines.Reports.Snapshot instance. If the object is not found, the return value is -1.</returns>
        public int IndexOf(Server.Engines.Reports.Snapshot value)
        {
            return this.List.IndexOf(value);
        }
        
        /// <summary>
        /// Removes a specified Server.Engines.Reports.Snapshot instance from this collection.
        /// </summary>
        /// <param name="value">The Server.Engines.Reports.Snapshot instance to remove.</param>
        public void Remove(Server.Engines.Reports.Snapshot value)
        {
            this.List.Remove(value);
        }
        
        /// <summary>
        /// Returns an enumerator that can iterate through the Server.Engines.Reports.Snapshot instance.
        /// </summary>
        /// <returns>An Server.Engines.Reports.Snapshot's enumerator.</returns>
        public new SnapshotCollectionEnumerator GetEnumerator()
        {
            return new SnapshotCollectionEnumerator(this);
        }
        
        /// <summary>
        /// Insert a Server.Engines.Reports.Snapshot instance into this collection at a specified index.
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        /// <param name="value">The Server.Engines.Reports.Snapshot instance to insert.</param>
        public void Insert(int index, Server.Engines.Reports.Snapshot value)
        {
            this.List.Insert(index, value);
        }
        
        /// <summary>
        /// Strongly typed enumerator of Server.Engines.Reports.Snapshot.
        /// </summary>
        public class SnapshotCollectionEnumerator : System.Collections.IEnumerator
        {
            
            /// <summary>
            /// Current index
            /// </summary>
            private int _index;
            
            /// <summary>
            /// Current element pointed to.
            /// </summary>
            private Server.Engines.Reports.Snapshot _currentElement;
            
            /// <summary>
            /// Collection to enumerate.
            /// </summary>
            private SnapshotCollection _collection;
            
            /// <summary>
            /// Default constructor for enumerator.
            /// </summary>
            /// <param name="collection">Instance of the collection to enumerate.</param>
            internal SnapshotCollectionEnumerator(SnapshotCollection collection)
            {
                _index = -1;
                _collection = collection;
            }
            
            /// <summary>
            /// Gets the Server.Engines.Reports.Snapshot object in the enumerated SnapshotCollection currently indexed by this instance.
            /// </summary>
            public Server.Engines.Reports.Snapshot Current
            {
                get
                {
                    if (((_index == -1) 
                                || (_index >= _collection.Count)))
                    {
                        throw new System.IndexOutOfRangeException("Enumerator not started.");
                    }
                    else
                    {
                        return _currentElement;
                    }
                }
            }
            
            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (((_index == -1) 
                                || (_index >= _collection.Count)))
                    {
                        throw new System.IndexOutOfRangeException("Enumerator not started.");
                    }
                    else
                    {
                        return _currentElement;
                    }
                }
            }
            
            /// <summary>
            /// Reset the cursor, so it points to the beginning of the enumerator.
            /// </summary>
            public void Reset()
            {
                _index = -1;
                _currentElement = null;
            }
            
            /// <summary>
            /// Advances the enumerator to the next queue of the enumeration, if one is currently available.
            /// </summary>
            /// <returns>true, if the enumerator was succesfully advanced to the next queue; false, if the enumerator has reached the end of the enumeration.</returns>
            public bool MoveNext()
            {
                if ((_index 
                            < (_collection.Count - 1)))
                {
                    _index = (_index + 1);
                    _currentElement = this._collection[_index];
                    return true;
                }
                _index = _collection.Count;
                return false;
            }
        }
    }
}
