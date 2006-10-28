using System;
using System.Collections;

namespace DotNetNuke.Services.Log.EventLog
{
    public class LogInfoArray : IList
    {
        private ArrayList arrLogs = new ArrayList();

        public int Count
        {
            get
            {
                return arrLogs.Count;
            }
        }

        public LogInfo GetItem(int Index)
        {
            return ((LogInfo)arrLogs[Index]);
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return arrLogs.GetEnumerator();
        }

        public int Add(object objLogInfo)
        {
            return this.arrLogs.Add(objLogInfo);
        }

        public void Remove(object objLogInfo)
        {
            arrLogs.Remove(objLogInfo);
        }

        public System.Collections.IEnumerator GetEnumerator(int index, int count)
        {
            return arrLogs.GetEnumerator(index, count);
        }

        public void CopyTo(System.Array array, int index)
        {
            arrLogs.CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get
            {
                return arrLogs.IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return arrLogs.SyncRoot;
            }
        }

        public void Clear()
        {
            arrLogs.Clear();
        }

        public bool Contains(object value)
        {
            if (arrLogs.Contains(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int IndexOf(object value)
        {
            return arrLogs.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            arrLogs.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get
            {
                return arrLogs.IsFixedSize;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return arrLogs.IsReadOnly;
            }
        }

        public object this[int index]
        {
            get
            {
                return arrLogs[index];
            }
            set
            {
                arrLogs[index] = value;
            }
        }

        public void RemoveAt(int index)
        {
            arrLogs.RemoveAt(index);
        }
    }
}