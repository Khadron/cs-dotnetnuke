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
using System.Web.Compilation;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Services.Exceptions;

namespace DotNetNuke.Framework
{
    /// <summary>
    /// Library responsible for reflection
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class Reflection
    {
        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        /// <returns>The created Object</returns>
        /// <remarks>Overload for creating an object from a Provider configured in web.config</remarks>
        public static object CreateObject( string ObjectProviderType )
        {
            return CreateObject( ObjectProviderType, true );
        }

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        /// <param name="UseCache">Caching switch</param>
        /// <returns>The created Object</returns>
        /// <remarks>Overload for creating an object from a Provider configured in web.config</remarks>
        public static object CreateObject( string ObjectProviderType, bool UseCache )
        {
            return CreateObject( ObjectProviderType, "", "", "", UseCache );
        }

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        /// <param name="ObjectNamespace">The namespace of the object to create.</param>
        /// <param name="ObjectAssemblyName">The assembly of the object to create.</param>
        /// <returns>The created Object</returns>
        /// <remarks>Overload for creating an object from a Provider including NameSpace and
        /// AssemblyName ( this allows derived providers to share the same config )</remarks>
        public static object CreateObject( string ObjectProviderType, string ObjectNamespace, string ObjectAssemblyName )
        {
            return CreateObject( ObjectProviderType, "", ObjectNamespace, ObjectAssemblyName, true );
        }

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        /// <param name="ObjectNamespace">The namespace of the object to create.</param>
        /// <param name="ObjectAssemblyName">The assembly of the object to create.</param>
        /// <param name="UseCache">Caching switch</param>
        /// <returns>The created Object</returns>
        /// <remarks>Overload for creating an object from a Provider including NameSpace and
        /// AssemblyName ( this allows derived providers to share the same config )</remarks>
        public static object CreateObject( string ObjectProviderType, string ObjectNamespace, string ObjectAssemblyName, bool UseCache )
        {
            return CreateObject( ObjectProviderType, "", ObjectNamespace, ObjectAssemblyName, UseCache );
        }

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        /// <param name="ObjectProviderName">The name of the Provider</param>
        /// <param name="ObjectNamespace">The namespace of the object to create.</param>
        /// <param name="ObjectAssemblyName">The assembly of the object to create.</param>
        /// <returns>The created Object</returns>
        /// <remarks>Overload for creating an object from a Provider including NameSpace,
        /// AssemblyName and ProviderName</remarks>
        public static object CreateObject( string ObjectProviderType, string ObjectProviderName, string ObjectNamespace, string ObjectAssemblyName )
        {
            return CreateObject( ObjectProviderType, ObjectProviderName, ObjectNamespace, ObjectAssemblyName, true );
        }

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        /// <param name="ObjectProviderName">The name of the Provider</param>
        /// <param name="ObjectNamespace">The namespace of the object to create.</param>
        /// <param name="ObjectAssemblyName">The assembly of the object to create.</param>
        /// <param name="UseCache">Caching switch</param>
        /// <returns>The created Object</returns>
        /// <remarks>Overload for creating an object from a Provider including NameSpace,
        /// AssemblyName and ProviderName</remarks>
        public static object CreateObject( string ObjectProviderType, string ObjectProviderName, string ObjectNamespace, string ObjectAssemblyName, bool UseCache )
        {
            string TypeName = "";

            // get the provider configuration based on the type
            ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration( ObjectProviderType );

            // if both the Namespace and AssemblyName are provided then we will construct an "assembly qualified typename" - ie. "NameSpace.ClassName, AssemblyName"
            if( !String.IsNullOrEmpty(ObjectNamespace) && !String.IsNullOrEmpty(ObjectAssemblyName) )
            {
                if( ObjectProviderName == "" )
                {
                    // dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider )
                    TypeName = ObjectNamespace + "." + objProviderConfiguration.DefaultProvider + ", " + ObjectAssemblyName + "." + objProviderConfiguration.DefaultProvider;
                }
                else
                {
                    // dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider )
                    TypeName = ObjectNamespace + "." + ObjectProviderName + ", " + ObjectAssemblyName + "." + ObjectProviderName;
                }
            }
            else
            {
                // if only the Namespace is provided then we will construct an "full typename" - ie. "NameSpace.ClassName"
                if( !String.IsNullOrEmpty(ObjectNamespace) )
                {
                    if( ObjectProviderName == "" )
                    {
                        // dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider )
                        TypeName = ObjectNamespace + "." + objProviderConfiguration.DefaultProvider;
                    }
                    else
                    {
                        // dynamically create the typename from the constants ( this enables private assemblies to share the same configuration as the base provider )
                        TypeName = ObjectNamespace + "." + ObjectProviderName;
                    }
                }
                else
                {
                    // if neither Namespace or AssemblyName are provided then we will get the typename from the default provider
                    if( ObjectProviderName == "" )
                    {
                        // get the typename of the default Provider from web.config
                        TypeName = ( (Provider)objProviderConfiguration.Providers[objProviderConfiguration.DefaultProvider] ).Type;
                    }
                    else
                    {
                        // get the typename of the specified ProviderName from web.config
                        TypeName = ( (Provider)objProviderConfiguration.Providers[ObjectProviderName] ).Type;
                    }
                }
            }

            return CreateObject( TypeName, TypeName, UseCache );
        }

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="TypeName">The fully qualified TypeName</param>
        /// <param name="CacheKey">The Cache Key</param>
        /// <returns>The created Object</returns>
        /// <remarks>Overload that takes a fully-qualified typename and a Cache Key</remarks>
        public static object CreateObject( string TypeName, string CacheKey )
        {
            return CreateObject( TypeName, CacheKey, true );
        }

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="TypeName">The fully qualified TypeName</param>
        /// <param name="CacheKey">The Cache Key</param>
        /// <param name="UseCache">Caching switch</param>
        /// <returns>The created Object</returns>
        /// <remarks>Overload that takes a fully-qualified typename and a Cache Key</remarks>
        public static object CreateObject( string TypeName, string CacheKey, bool UseCache )
        {
            if( CacheKey == "" )
            {
                CacheKey = TypeName;
            }

            Type objType = null;

            // use the cache for performance
            if( UseCache )
            {
                objType = (Type)DataCache.GetCache( CacheKey );
            }

            // is the type in the cache?
            if( objType == null )
            {
                try
                {
                    // use reflection to get the type of the class
                    objType = BuildManager.GetType( TypeName, true, true );

                    if( UseCache )
                    {
                        // insert the type into the cache
                        DataCache.SetCache( CacheKey, objType );
                    }
                }
                catch( Exception exc )
                {
                    // could not load the type
                    Exceptions.LogException( exc );
                }
            }

            // dynamically create the object
            return Activator.CreateInstance( objType, null );
        }

        /// <summary>
        /// Creates an object
        /// </summary>
        /// <typeparam name="T">The type of object to create</typeparam>
        /// <returns></returns>
        /// <remarks>Generic version</remarks>
        public static T CreateObject< T >()
        {
            //dynamically create the object.
            return Activator.CreateInstance<T>();
        }

        // dynamically create an object from a TypeName using a CacheKey
        internal static object CreateObjectNotCached( string ObjectProviderType )
        {
            string TypeName = "";
            Type objType = null;

            // get the provider configuration based on the type
            ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration( ObjectProviderType );

            // get the typename of the Base DataProvider from web.config
            TypeName = ( (Provider)objProviderConfiguration.Providers[objProviderConfiguration.DefaultProvider] ).Type;

            try
            {
                // use reflection to get the type of the class
                objType = Type.GetType( TypeName, true );
            }
            catch( Exception exc )
            {
                // could not load the type
                Exceptions.LogException( exc );
            }

            // dynamically create the object
            return Activator.CreateInstance( objType, null );
        }
    }
}