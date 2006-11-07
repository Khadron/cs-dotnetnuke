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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using Microsoft.VisualBasic;

namespace DotNetNuke.Common.Utilities
{
    /// <summary>
    /// Class that hydrates custom business objects with data. Please
    /// note that this utility class can only be used on objects with "simple"
    /// data types. If the object contains "complex" data types such as
    /// ArrayLists, HashTables, Custom Objects, etc... then the developer
    /// will need to write custom code for the hydration of these "complex"
    /// data types.
    /// </summary>
    public class CBO
    {
        public static object CloneObject( object ObjectToClone ) //Implements System.ICloneable.Clone
        {
            try
            {
                object newObject = Reflection.CreateObject( ObjectToClone.GetType().AssemblyQualifiedName, ObjectToClone.GetType().AssemblyQualifiedName );

                ArrayList props = GetPropertyInfo( newObject.GetType() );

                int i = 0;

                for( i = 0; i <= GetPropertyInfo( ObjectToClone.GetType() ).Count - 1; i++ )
                {
                    PropertyInfo p = (PropertyInfo)GetPropertyInfo( ObjectToClone.GetType() )[i];

                    Type ICloneType = p.PropertyType.GetInterface( "ICloneable", true );
                    if( ( (PropertyInfo)props[i] ).CanWrite )
                    {
                        if( !( ICloneType == null ) )
                        {
                            ICloneable IClone = (ICloneable)p.GetValue( ObjectToClone, null );
                            ( (PropertyInfo)props[i] ).SetValue( newObject, IClone.Clone(), null );
                        }
                        else
                        {
                            ( (PropertyInfo)props[i] ).SetValue( newObject, p.GetValue( ObjectToClone, null ), null );
                        }

                        Type IEnumerableType = p.PropertyType.GetInterface( "IEnumerable", true );
                        if( !( IEnumerableType == null ) )
                        {
                            IEnumerable IEnum = (IEnumerable)p.GetValue( ObjectToClone, null );

                            Type IListType = ( (PropertyInfo)props[i] ).PropertyType.GetInterface( "IList", true );
                            Type IDicType = ( (PropertyInfo)props[i] ).PropertyType.GetInterface( "IDictionary", true );

                            int j = 0;
                            if( !( IListType == null ) )
                            {
                                IList list = (IList)( ( (PropertyInfo)props[i] ).GetValue( newObject, null ) );

                                object obj;
                                foreach( object tempLoopVar_obj in IEnum )
                                {
                                    obj = tempLoopVar_obj;
                                    ICloneType = obj.GetType().GetInterface( "ICloneable", true );

                                    if( !( ICloneType == null ) )
                                    {
                                        ICloneable tmpClone = (ICloneable)obj;
                                        list[j] = tmpClone.Clone();
                                    }

                                    j++;
                                }
                            }
                            else
                            {
                                if( !( IDicType == null ) )
                                {
                                    IDictionary dic = (IDictionary)( ( (PropertyInfo)props[i] ).GetValue( newObject, null ) );
                                    j = 0;

                                    DictionaryEntry de;
                                    foreach( DictionaryEntry tempLoopVar_de in IEnum )
                                    {
                                        de = tempLoopVar_de;

                                        ICloneType = de.Value.GetType().GetInterface( "ICloneable", true );

                                        if( !( ICloneType == null ) )
                                        {
                                            ICloneable tmpClone = (ICloneable)de.Value;
                                            dic[de.Key] = tmpClone.Clone();
                                        }
                                        j++;
                                    }
                                }
                            }
                        }
                    }
                }
                return newObject;
            }
            catch( Exception exc )
            {
                Exceptions.LogException( exc );
                return null;
            }
        }

        private static object CreateObject( Type objType, IDataReader dr, ArrayList objProperties, int[] arrOrdinals )
        {
            PropertyInfo objPropertyInfo;
            object objValue;
            Type objPropertyType = null;
            int intProperty;

            object objObject = Activator.CreateInstance( objType, null );

            // fill object with values from datareader
            for( intProperty = 0; intProperty <= objProperties.Count - 1; intProperty++ )
            {
                objPropertyInfo = (PropertyInfo)objProperties[intProperty];
                if( objPropertyInfo.CanWrite )
                {
                    objValue = Null.SetNull( objPropertyInfo );
                    if( arrOrdinals[intProperty] != -1 )
                    {
                        if( Information.IsDBNull( dr.GetValue( arrOrdinals[intProperty] ) ) )
                        {
                            // translate Null value
                            objPropertyInfo.SetValue( objObject, objValue, null );
                        }
                        else
                        {
                            try
                            {
                                // try implicit conversion first
                                objPropertyInfo.SetValue( objObject, dr.GetValue( arrOrdinals[intProperty] ), null );
                            }
                            catch
                            {
                                // business object info class member data type does not match datareader member data type
                                try
                                {
                                    objPropertyType = objPropertyInfo.PropertyType;
                                    //need to handle enumeration conversions differently than other base types
                                    if( objPropertyType.BaseType.Equals( typeof( Enum ) ) )
                                    {
                                        // check if value is numeric and if not convert to integer ( supports databases like Oracle )
                                        if( Information.IsNumeric( dr.GetValue( arrOrdinals[intProperty] ) ) )
                                        {
                                            ( (PropertyInfo)objProperties[intProperty] ).SetValue( objObject, Enum.ToObject( objPropertyType, Convert.ToInt32( dr.GetValue( arrOrdinals[intProperty] ) ) ), null );
                                        }
                                        else
                                        {
                                            ( (PropertyInfo)objProperties[intProperty] ).SetValue( objObject, Enum.ToObject( objPropertyType, dr.GetValue( arrOrdinals[intProperty] ) ), null );
                                        }
                                    }
                                    else
                                    {
                                        // try explicit conversion
                                        objPropertyInfo.SetValue( objObject, Convert.ChangeType( dr.GetValue( arrOrdinals[intProperty] ), objPropertyType ), null );
                                    }
                                }
                                catch
                                {
                                    objPropertyInfo.SetValue( objObject, Convert.ChangeType( dr.GetValue( arrOrdinals[intProperty] ), objPropertyType ), null );
                                }
                            }
                        }
                    }
                    else
                    {
                        // property does not exist in datareader
                    }
                }
            }

            return objObject;
        }

        /// <summary>
        /// Generic version of CreateObject creates an object of a specified type from the
        /// provided DataReader
        /// </summary>
        /// <typeparam name="T">The type of the business object</typeparam>
        /// <param name="dr">The DataReader</param>
        /// <returns>The custom business object</returns>
        /// <remarks></remarks>
        private static T CreateObject< T >( IDataReader dr )
        {
            PropertyInfo objPropertyInfo;
            object objValue;
            Type objPropertyType = null;
            int intProperty;

            T objObject = Activator.CreateInstance<T>();

            // get properties for type
            ArrayList objProperties = GetPropertyInfo( objObject.GetType() );

            // get ordinal positions in datareader
            int[] arrOrdinals = GetOrdinals( objProperties, dr );

            // fill object with values from datareader
            for( intProperty = 0; intProperty <= objProperties.Count - 1; intProperty++ )
            {
                objPropertyInfo = (PropertyInfo)objProperties[intProperty];
                if( objPropertyInfo.CanWrite )
                {
                    objValue = Null.SetNull( objPropertyInfo );
                    if( arrOrdinals[intProperty] != -1 )
                    {
                        if( Information.IsDBNull( dr.GetValue( arrOrdinals[intProperty] ) ) )
                        {
                            // translate Null value
                            objPropertyInfo.SetValue( objObject, objValue, null );
                        }
                        else
                        {
                            try
                            {
                                // try implicit conversion first
                                objPropertyInfo.SetValue( objObject, dr.GetValue( arrOrdinals[intProperty] ), null );
                            }
                            catch
                            {
                                // business object info class member data type does not match datareader member data type
                                try
                                {
                                    objPropertyType = objPropertyInfo.PropertyType;
                                    //need to handle enumeration conversions differently than other base types
                                    if( objPropertyType.BaseType.Equals( typeof( Enum ) ) )
                                    {
                                        // check if value is numeric and if not convert to integer ( supports databases like Oracle )
                                        if( Information.IsNumeric( dr.GetValue( arrOrdinals[intProperty] ) ) )
                                        {
                                            ( (PropertyInfo)objProperties[intProperty] ).SetValue( objObject, Enum.ToObject( objPropertyType, Convert.ToInt32( dr.GetValue( arrOrdinals[intProperty] ) ) ), null );
                                        }
                                        else
                                        {
                                            ( (PropertyInfo)objProperties[intProperty] ).SetValue( objObject, Enum.ToObject( objPropertyType, dr.GetValue( arrOrdinals[intProperty] ) ), null );
                                        }
                                    }
                                    else
                                    {
                                        // try explicit conversion
                                        objPropertyInfo.SetValue( objObject, Convert.ChangeType( dr.GetValue( arrOrdinals[intProperty] ), objPropertyType ), null );
                                    }
                                }
                                catch
                                {
                                    objPropertyInfo.SetValue( objObject, Convert.ChangeType( dr.GetValue( arrOrdinals[intProperty] ), objPropertyType ), null );
                                }
                            }
                        }
                    }
                    else
                    {
                        // property does not exist in datareader
                    }
                }
            }

            return objObject;
        }

        public static ArrayList FillCollection( IDataReader dr, Type objType )
        {
            ArrayList objFillCollection = new ArrayList();
            object objFillObject;

            // get properties for type
            ArrayList objProperties = GetPropertyInfo( objType );

            // get ordinal positions in datareader
            int[] arrOrdinals = GetOrdinals( objProperties, dr );

            // iterate datareader
            while( dr.Read() )
            {
                // fill business object
                objFillObject = CreateObject( objType, dr, objProperties, arrOrdinals );
                // add to collection
                objFillCollection.Add( objFillObject );
            }

            // close datareader
            if( dr != null )
            {
                dr.Close();
            }

            return objFillCollection;
        }

        public static IList FillCollection( IDataReader dr, Type objType, ref IList objToFill )
        {
            object objFillObject;

            // get properties for type
            ArrayList objProperties = GetPropertyInfo( objType );

            // get ordinal positions in datareader
            int[] arrOrdinals = GetOrdinals( objProperties, dr );

            // iterate datareader
            while( dr.Read() )
            {
                // fill business object
                objFillObject = CreateObject( objType, dr, objProperties, arrOrdinals );
                // add to collection
                objToFill.Add( objFillObject );
            }

            // close datareader
            if( dr != null )
            {
                dr.Close();
            }

            return objToFill;
        }

        /// <summary>
        /// Generic version of FillCollection fills a List custom business object of a specified type
        /// from the supplied DataReader
        /// </summary>
        /// <typeparam name="T">The type of the business object</typeparam>
        /// <param name="dr">The IDataReader to use to fill the object</param>
        /// <returns>A List of custom business objects</returns>
        /// <remarks></remarks>
        public static List<T> FillCollection< T >( IDataReader dr )
        {
            List<T> objFillCollection = new List<T>();
            T objFillObject;

            // iterate datareader
            while( dr.Read() )
            {
                // fill business object
                objFillObject = CreateObject<T>( dr );
                // add to collection
                objFillCollection.Add( objFillObject );
            }

            // close datareader
            if( dr != null )
            {
                dr.Close();
            }

            return objFillCollection;
        }

        /// <summary>
        /// Generic version of FillCollection fills a provided IList with custom business
        /// objects of a specified type from the supplied DataReader
        /// </summary>
        /// <typeparam name="T">The type of the business object</typeparam>
        /// <param name="dr">The IDataReader to use to fill the object</param>
        /// <param name="objToFill">The IList to fill</param>
        /// <returns>An IList of custom business objects</returns>
        /// <remarks></remarks>
        public static IList<T> FillCollection< T >( IDataReader dr, ref IList<T> objToFill )
        {
            T objFillObject;

            // iterate datareader
            while( dr.Read() )
            {
                // fill business object
                objFillObject = CreateObject<T>( dr );
                // add to collection
                objToFill.Add( objFillObject );
            }

            // close datareader
            if( dr != null )
            {
                dr.Close();
            }

            return objToFill;
        }

        public static object FillObject( IDataReader dr, Type objType )
        {
            return FillObject( dr, objType, true );
        }

        public static object FillObject( IDataReader dr, Type objType, bool ManageDataReader )
        {
            object objFillObject;

            // get properties for type
            ArrayList objProperties = GetPropertyInfo( objType );

            // get ordinal positions in datareader
            int[] arrOrdinals = GetOrdinals( objProperties, dr );

            bool @Continue;
            if( ManageDataReader )
            {
                @Continue = false;
                // read datareader
                if( dr.Read() )
                {
                    @Continue = true;
                }
            }
            else
            {
                @Continue = true;
            }

            if( @Continue )
            {
                // create custom business object
                objFillObject = CreateObject( objType, dr, objProperties, arrOrdinals );
            }
            else
            {
                objFillObject = null;
            }

            if( ManageDataReader )
            {
                // close datareader
                if( dr != null )
                {
                    dr.Close();
                }
            }

            return objFillObject;
        }

        /// <summary>
        /// Generic version of FillObject fills a custom business object of a specified type
        /// from the supplied DataReader
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="dr">The IDataReader to use to fill the object</param>
        /// <returns>The object</returns>
        /// <remarks>This overloads sets the ManageDataReader parameter to true and calls
        /// the other overload</remarks>
        public static T FillObject< T >( IDataReader dr )
        {
            return FillObject<T>( dr, true );
        }

        /// <summary>
        /// Generic version of FillObject fills a custom business object of a specified type
        /// from the supplied DataReader
        /// </summary>
        /// <typeparam name="T">The type of the object</typeparam>
        /// <param name="dr">The IDataReader to use to fill the object</param>
        /// <param name="ManageDataReader">A boolean that determines whether the DatReader
        /// is managed</param>
        /// <returns>The object</returns>
        /// <remarks>This overloads allows the caller to determine whether the ManageDataReader
        /// parameter is set</remarks>
        public static T FillObject< T >( IDataReader dr, bool ManageDataReader )
        {
            T objFillObject;

            bool doContinue;
            if( ManageDataReader )
            {
                doContinue = false;
                // read datareader
                if( dr.Read() )
                {
                    doContinue = true;
                }
            }
            else
            {
                doContinue = true;
            }

            if( doContinue )
            {
                // create custom business object
                objFillObject = CreateObject<T>( dr );
            }
            else
            {
                objFillObject = default( T );
            }

            if( ManageDataReader )
            {
                // close datareader
                if( dr != null )
                {
                    dr.Close();
                }
            }

            return objFillObject;
        }

        private static int[] GetOrdinals( ArrayList objProperties, IDataReader dr )
        {
            int[] arrOrdinals = new int[objProperties.Count + 1];
            int intProperty;

            if( dr != null )
            {
                for( intProperty = 0; intProperty <= objProperties.Count - 1; intProperty++ )
                {
                    arrOrdinals[intProperty] = -1;
                    try
                    {
                        arrOrdinals[intProperty] = dr.GetOrdinal( ( (PropertyInfo)objProperties[intProperty] ).Name );
                    }
                    catch
                    {
                        // property does not exist in datareader
                    }
                }
            }

            return arrOrdinals;
        }

        public static ArrayList GetPropertyInfo( Type objType )
        {
            // Use the cache because the reflection used later is expensive
            ArrayList objProperties = (ArrayList)DataCache.GetCache( objType.FullName );

            if( objProperties == null )
            {
                objProperties = new ArrayList();
                PropertyInfo objProperty;
                foreach( PropertyInfo tempLoopVar_objProperty in objType.GetProperties() )
                {
                    objProperty = tempLoopVar_objProperty;
                    objProperties.Add( objProperty );
                }
                DataCache.SetCache( objType.FullName, objProperties );
            }

            return objProperties;
        }

        public static object InitializeObject( object objObject, Type objType )
        {
            PropertyInfo objPropertyInfo;
            object objValue;
            int intProperty;

            // get properties for type
            ArrayList objProperties = GetPropertyInfo( objType );

            // initialize properties
            for( intProperty = 0; intProperty <= objProperties.Count - 1; intProperty++ )
            {
                objPropertyInfo = (PropertyInfo)objProperties[intProperty];
                if( objPropertyInfo.CanWrite )
                {
                    objValue = Null.SetNull( objPropertyInfo );
                    objPropertyInfo.SetValue( objObject, objValue, null );
                }
            }

            return objObject;
        }

        public static XmlDocument Serialize( object objObject )
        {
            XmlSerializer objXmlSerializer = new XmlSerializer( objObject.GetType() );

            StringBuilder objStringBuilder = new StringBuilder();

            TextWriter objTextWriter = new StringWriter( objStringBuilder );

            objXmlSerializer.Serialize( objTextWriter, objObject );

            StringReader objStringReader = new StringReader( objTextWriter.ToString() );

            DataSet objDataSet = new DataSet();

            objDataSet.ReadXml( objStringReader );

            XmlDocument xmlSerializedObject = new XmlDocument();

            xmlSerializedObject.LoadXml( objDataSet.GetXml() );

            return xmlSerializedObject;
        }
    }
}