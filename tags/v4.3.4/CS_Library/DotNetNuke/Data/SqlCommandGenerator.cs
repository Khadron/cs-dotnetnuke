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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace DotNetNuke.Data
{
    public sealed class SqlCommandGenerator
    {
        private SqlCommandGenerator()
        {
            throw new NotSupportedException();
        }

        public static string ReturnValueParameterName = "RETURN_VALUE";
        public static object[] NoValues;

        public static SqlCommand GenerateCommand(SqlConnection Connection, MethodInfo Method, object[] Values, CommandType SQLCommandType, string SQLCommandText)
        {
            if (Method == null)
            {
                Method = (MethodInfo)new StackTrace().GetFrame(1).GetMethod();
            }

            SqlCommand command = new SqlCommand();
            command.Connection = Connection;
            command.CommandType = SQLCommandType;

            if (SQLCommandText.Length == 0)
            {
                command.CommandText = Method.Name;
            }
            else
            {
                command.CommandText = SQLCommandText;
            }

            if (command.CommandType == CommandType.StoredProcedure)
            {
                GenerateCommandParameters(command, Method, Values);

                command.Parameters.Add(ReturnValueParameterName, SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
            }

            return command;
        }

        private static void GenerateCommandParameters(SqlCommand command, MethodInfo method, object[] values)
        {
            ParameterInfo[] methodParameters = method.GetParameters();

            int paramIndex = 0;

            ParameterInfo paramInfo;
            foreach (ParameterInfo tempLoopVar_paramInfo in methodParameters)
            {
                paramInfo = tempLoopVar_paramInfo;

                if (!Attribute.IsDefined(paramInfo, typeof(NonCommandParameterAttribute)))
                {
                    SqlParameterAttribute paramAttribute = (SqlParameterAttribute)Attribute.GetCustomAttribute(paramInfo, typeof(SqlParameterAttribute));

                    if (paramAttribute == null)
                    {
                        paramAttribute = new SqlParameterAttribute(null, SqlDbType.BigInt, 0, byte.MinValue, byte.MinValue, ((ParameterDirection)0));
                    }

                    SqlParameter sqlParameter = new SqlParameter();

                    if (paramAttribute.IsNameDefined)
                    {
                        sqlParameter.ParameterName = paramAttribute.Name;
                    }
                    else
                    {
                        sqlParameter.ParameterName = paramInfo.Name;
                    }
                    if (!sqlParameter.ParameterName.StartsWith("@"))
                    {
                        sqlParameter.ParameterName = "@" + sqlParameter.ParameterName;
                    }
                    if (paramAttribute.IsTypeDefined)
                    {
                        sqlParameter.SqlDbType = paramAttribute.SqlDbType;
                    }
                    if (paramAttribute.IsSizeDefined)
                    {
                        sqlParameter.Size = paramAttribute.Size;
                    }
                    if (paramAttribute.IsScaleDefined)
                    {
                        sqlParameter.Scale = paramAttribute.Scale;
                    }
                    if (paramAttribute.IsPrecisionDefined)
                    {
                        sqlParameter.Precision = paramAttribute.Precision;
                    }
                    if (paramAttribute.IsDirectionDefined)
                    {
                        sqlParameter.Direction = paramAttribute.Direction;
                    }
                    else
                    {
                        if (paramInfo.ParameterType.IsByRef)
                        {
                            if (paramInfo.IsOut)
                            {
                                sqlParameter.Direction = ParameterDirection.Output;
                            }
                            else
                            {
                                sqlParameter.Direction = ParameterDirection.InputOutput;
                            }
                        }
                        else
                        {
                            sqlParameter.Direction = ParameterDirection.Input;
                        }
                    }

                    sqlParameter.Value = values[paramIndex];
                    command.Parameters.Add(sqlParameter);

                    paramIndex++;
                }
            }
        }
    }
}