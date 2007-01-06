#region DotNetNuke License
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2006
// by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
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