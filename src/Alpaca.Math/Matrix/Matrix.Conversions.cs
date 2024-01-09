// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

using Alpaca.Math;

namespace Accord.Math
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
#if !NETSTANDARD1_4
    using System.Data;
#endif
    using System.Globalization;
    using System.Linq;

    public static partial class Matrix
    {

        /// <summary>
        ///   Converts a jagged-array into a multidimensional array.
        /// </summary>
        /// 
        public static Array DeepToMatrix(this Array array)
        {
            int[] shape = array.GetLength();
            int totalLength = array.GetTotalLength();
            Type elementType = array.GetInnerMostType();

            Array flat = array.DeepFlatten();

            Array result = Array.CreateInstance(elementType, shape);
            Buffer.BlockCopy(flat, 0, result, 0, totalLength);
            return result;
        }

        /// <summary>
        ///   Converts a jagged-array into a multidimensional array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this T[][] array, bool transpose = false)
        {
            int rows = array.Length;
            if (rows == 0) return new T[0, rows];
            int cols = array[0].Length;

            T[,] m;

            if (transpose)
            {
                m = new T[cols, rows];
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        m[j, i] = array[i][j];
            }
            else
            {
                m = new T[rows, cols];
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < cols; j++)
                        m[i, j] = array[i][j];
            }

            return m;
        }

        /// <summary>
        ///   Converts an array into a multidimensional array.
        /// </summary>
        /// 
        public static T[][] ToJagged<T>(this T[] array, bool asColumnVector = true)
        {
            if (asColumnVector)
            {
                T[][] m = new T[array.Length][];
                for (int i = 0; i < array.Length; i++)
                    m[i] = new[] { array[i] };
                return m;
            }
            else
            {
                return new T[][] { array };
            }
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this T[] array, bool asColumnVector = true)
        {
            return ToJagged(array, asColumnVector: asColumnVector);
        }

        /// <summary>
        ///   Converts an array into a multidimensional array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this T[] array, bool asColumnVector = false)
        {
            if (asColumnVector)
            {
                T[,] m = new T[array.Length, 1];
                for (int i = 0; i < array.Length; i++)
                    m[i, 0] = array[i];
                return m;
            }
            else
            {
                T[,] m = new T[1, array.Length];
                for (int i = 0; i < array.Length; i++)
                    m[0, i] = array[i];
                return m;
            }
        }

        /// <summary>
        ///   Converts a multidimensional array into a jagged array.
        /// </summary>
        /// 
        public static T[][] ToJagged<T>(this T[,] matrix, bool transpose = false)
        {
            T[][] array;

            if (transpose)
            {
                int cols = matrix.GetLength(1);

                array = new T[cols][];
                for (int i = 0; i < cols; i++)
                    array[i] = matrix.GetColumn(i);
            }
            else
            {
                int rows = matrix.GetLength(0);

                array = new T[rows][];
                for (int i = 0; i < rows; i++)
                    array[i] = matrix.GetRow(i);
            }

            return array;
        }

        /// <summary>
        ///   Obsolete.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this T[,] matrix, bool transpose = false)
        {
            return ToJagged(matrix, transpose);
        }



        #region Type conversions

        /// <summary>
        ///   Converts the values of a vector using the given converter expression.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="vector">The vector to be converted.</param>
        /// 
        public static TOutput[] Convert<TInput, TOutput>(this TInput[] vector)
            where TOutput : TInput
        {
            TOutput[] result = new TOutput[vector.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = (TOutput)vector[i];
            return result;
        }

#if !NETSTANDARD1_4
        /// <summary>
        ///   Converts the values of a vector using the given converter expression.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="vector">The vector to be converted.</param>
        /// <param name="converter">The converter function.</param>
        /// 
        public static TOutput[] Convert<TInput, TOutput>(this TInput[] vector, Converter<TInput, TOutput> converter)
        {
            return Array.ConvertAll(vector, converter);
        }

        /// <summary>
        ///   Converts the values of a matrix using the given converter expression.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="matrix">The matrix to be converted.</param>
        /// <param name="converter">The converter function.</param>
        /// 
        public static TOutput[][] Convert<TInput, TOutput>(this TInput[][] matrix, Converter<TInput, TOutput> converter)
        {
            TOutput[][] result = new TOutput[matrix.Length][];

            for (int i = 0; i < matrix.Length; i++)
            {
                result[i] = new TOutput[matrix[i].Length];
                for (int j = 0; j < matrix[i].Length; j++)
                    result[i][j] = converter(matrix[i][j]);
            }

            return result;
        }

        /// <summary>
        ///   Converts the values of a matrix using the given converter expression.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// <param name="matrix">The vector to be converted.</param>
        /// <param name="converter">The converter function.</param>
        /// 
        public static TOutput[,] Convert<TInput, TOutput>(this TInput[,] matrix, Converter<TInput, TOutput> converter)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            TOutput[,] result = new TOutput[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[i, j] = converter(matrix[i, j]);

            return result;
        }

        /// <summary>
        ///   Converts an object into another type, irrespective of whether
        ///   the conversion can be done at compile time or not. This can be
        ///   used to convert generic types to numeric types during runtime.
        /// </summary>
        /// 
        /// <typeparam name="TOutput">The type of the output.</typeparam>
        /// 
        /// <param name="array">The vector or array to be converted.</param>
        /// 
        public static TOutput To<TOutput>(this Array array)
            where TOutput : class, ICloneable, IList, ICollection, IEnumerable
#if !NET35
, IStructuralComparable, IStructuralEquatable
#endif
        {
            return To(array, typeof(TOutput)) as TOutput;
        }

        /// <summary>
        ///   Converts an object into another type, irrespective of whether
        ///   the conversion can be done at compile time or not. This can be
        ///   used to convert generic types to numeric types during runtime.
        /// </summary>
        /// 
        /// <param name="array">The vector or array to be converted.</param>
        /// <param name="outputType">The type of the output.</param>
        /// 
        public static object To(this Array array, Type outputType)
        {
            Type inputType = array.GetType();

            Type inputElementType = inputType.GetElementType();
            Type outputElementType = outputType.GetElementType();

            Array result;

            if (inputElementType.IsArray && !outputElementType.IsArray)
            {
                // jagged -> multidimensional
                result = Array.CreateInstance(outputElementType, array.GetLength(true));

                foreach (var idx in GetIndices(result))
                {
                    object inputValue = array.GetValue(true, idx);
                    object outputValue = convertValue(outputElementType, inputValue);
                    result.SetValue(outputValue, idx);
                }
            }
            else if (!inputElementType.IsArray && outputElementType.IsArray)
            {
                // multidimensional -> jagged
                result = Array.CreateInstance(outputElementType, array.GetLength(0));

                foreach (var idx in GetIndices(array))
                {
                    object inputValue = array.GetValue(idx);
                    object outputValue = convertValue(outputElementType, inputValue);
                    result.SetValue(outputValue, true, idx);
                }
            }
            else
            {
                // Same nature (jagged or multidimensional) array
                result = Array.CreateInstance(outputElementType, array.GetLength(false));

                foreach (var idx in GetIndices(array))
                {
                    object inputValue = array.GetValue(idx);
                    object outputValue = convertValue(outputElementType, inputValue);
                    result.SetValue(outputValue, idx);
                }
            }

            return result;
        }
#endif


        /// <summary>
        ///  Gets the value at the specified position in the multidimensional System.Array.
        ///  The indexes are specified as an array of 32-bit integers.
        /// </summary>
        /// 
        /// <param name="array">A jagged or multidimensional array.</param>
        /// <param name="deep">If set to true, internal arrays in jagged arrays will be followed.</param>
        /// <param name="indices">A one-dimensional array of 32-bit integers that represent the
        ///   indexes specifying the position of the System.Array element to get.</param>
        ///   
        public static object GetValue(this Array array, bool deep, int[] indices)
        {
            if (array.IsVector())
                return array.GetValue(indices);

            if (deep && array.IsJagged())
            {
                Array current = array.GetValue(indices[0]) as Array;
                if (indices.Length == 1)
                    return current;
                int[] last = indices.Get(1, 0);
                return GetValue(current, true, last);
            }
            else
            {
                return array.GetValue(indices);
            }
        }

        /// <summary>
        ///  Gets the value at the specified position in the multidimensional System.Array.
        ///  The indexes are specified as an array of 32-bit integers.
        /// </summary>
        /// 
        /// <param name="array">A jagged or multidimensional array.</param>
        /// <param name="deep">If set to true, internal arrays in jagged arrays will be followed.</param>
        /// <param name="indices">A one-dimensional array of 32-bit integers that represent the
        ///   indexes specifying the position of the System.Array element to get.</param>
        /// <param name="value">The value retrieved from the array.</param>
        ///   
        public static bool TryGetValue(this Array array, bool deep, int[] indices, out object value)
        {
            value = null;

            if (array.IsVector())
            {
                if (indices.Length > array.Rank)
                    return false;

                for (int i = 0; i < indices.Length; i++)
                {
                    if (indices[i] > array.GetUpperBound(i))
                        return false;
                }

                value = array.GetValue(indices);
                return true;
            }

            if (deep && array.IsJagged())
            {
                Array current = array.GetValue(indices[0]) as Array;
                if (indices.Length == 1)
                {
                    value = current;
                    return true;
                }
                int[] last = indices.Get(1, 0);
                return TryGetValue(current, true, last, out value);
            }
            else
            {
                value = array.GetValue(indices);
                return true;
            }
        }

        /// <summary>
        ///   Sets a value to the element at the specified position in the multidimensional
        ///   or jagged System.Array. The indexes are specified as an array of 32-bit integers.
        /// </summary>
        /// 
        /// <param name="array">A jagged or multidimensional array.</param>
        /// <param name="value">The new value for the specified element.</param>
        /// <param name="deep">If set to true, internal arrays in jagged arrays will be followed.</param>
        /// <param name="indices">A one-dimensional array of 32-bit integers that represent
        ///   the indexes specifying the position of the element to set.</param>
        ///   
        public static void SetValue(this Array array, object value, bool deep, int[] indices)
        {
            if (deep && array.IsJagged())
            {
                Array current = array.GetValue(indices[0]) as Array;
                int[] last = indices.Get(1, 0);
                SetValue(current, value, true, last);
            }
            else
            {
                array.SetValue(value, indices);
            }
        }

#if !NETSTANDARD1_4
        private static object convertValue(Type outputElementType, object inputValue)
        {
            object outputValue = null;

            Array inputArray = inputValue as Array;

            if (outputElementType.IsEnum)
            {
                outputValue = Enum.ToObject(outputElementType, (int)System.Convert.ChangeType(inputValue, typeof(int)));
            }
            else if (inputArray != null)
            {
                outputValue = To(inputArray, outputElementType);
            }
            else
            {
                outputValue = System.Convert.ChangeType(inputValue, outputElementType);
            }
            return outputValue;
        }
#endif


        #endregion

        /// <summary>
        ///   Creates a vector containing every index that can be used to
        ///   address a given <paramref name="array"/>, in order.
        /// </summary>
        /// 
        /// <param name="array">The array whose indices will be returned.</param>
        /// <param name="deep">Pass true to retrieve all dimensions of the array,
        ///   even if it contains nested arrays (as in jagged matrices).</param>
        /// <param name="max">Bases computations on the maximum length possible for 
        ///   each dimension (in case the jagged matrices has different lengths).</param>
        /// 
        /// <returns>
        ///   An enumerable object that can be used to iterate over all
        ///   positions of the given <paramref name="array">System.Array</paramref>.
        /// </returns>
        /// 
        /// <example>
        /// <code>
        ///   double[,] a = 
        ///   { 
        ///      { 5.3, 2.3 },
        ///      { 4.2, 9.2 }
        ///   };
        ///   
        ///   foreach (int[] idx in a.GetIndices())
        ///   {
        ///      // Get the current element
        ///      double e = (double)a.GetValue(idx);
        ///   }
        /// </code>
        /// </example>
        /// 
        /// <seealso cref="Accord.Math.Vector.GetIndices{T}(T[])"/>
        /// 
        public static IEnumerable<int[]> GetIndices(this Array array, bool deep = false, bool max = false)
        {
            return Combinatorics.Sequences(array.GetLength(deep, max));
        }

        #region DataTable Conversions
#if !NETSTANDARD1_4
        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static double[,] ToMatrix(this DataTable table)
        {
            return ToMatrix<double>(table);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static double[,] ToMatrix(this DataTable table, out string[] columnNames)
        {
            return ToMatrix<double>(table, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static double[,] ToMatrix(this DataTable table, IFormatProvider provider)
        {
            return ToMatrix<double>(table, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static double[,] ToMatrix(this DataTable table, params string[] columnNames)
        {
            return ToMatrix<double>(table, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table, IFormatProvider provider)
        {
            String[] names;
            return ToMatrix<T>(table, out names, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table)
        {
            String[] names;
            return ToMatrix<T>(table, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table, out string[] columnNames)
        {
            T[,] m = new T[table.Rows.Count, table.Columns.Count];
            columnNames = new string[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    object obj = table.Rows[i][j];
                    m[i, j] = (T)System.Convert.ChangeType(obj, typeof(T));
                }

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table, out string[] columnNames, IFormatProvider provider)
        {
            T[,] m = new T[table.Rows.Count, table.Columns.Count];
            columnNames = new string[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    object obj = table.Rows[i][j];
                    m[i, j] = (T)System.Convert.ChangeType(obj, typeof(T), provider);
                }

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table, params string[] columnNames)
        {
            T[,] m = new T[table.Rows.Count, columnNames.Length];

            for (int j = 0; j < columnNames.Length; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    m[i, j] = (T)System.Convert.ChangeType(table.Rows[i][columnNames[j]], typeof(T));
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static T[,] ToMatrix<T>(this DataTable table, IFormatProvider provider, params string[] columnNames)
        {
            T[,] m = new T[table.Rows.Count, columnNames.Length];

            for (int j = 0; j < columnNames.Length; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    m[i, j] = (T)System.Convert.ChangeType(table.Rows[i][columnNames[j]], typeof(T), provider);
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static DataTable ToTable(this double[,] matrix)
        {
            int cols = matrix.GetLength(1);

            String[] columnNames = new String[cols];
            for (int i = 0; i < columnNames.Length; i++)
                columnNames[i] = "Column " + i;
            return ToTable(matrix, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static DataTable ToTable(this double[,] matrix, params string[] columnNames)
        {
            DataTable table = new DataTable();
            table.Locale = CultureInfo.CurrentCulture;

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int i = 0; i < cols; i++)
                table.Columns.Add(columnNames[i], typeof(double));

            for (int i = 0; i < rows; i++)
                table.Rows.Add(matrix.GetRow(i).Convert(x => (object)x));

            return table;
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static DataTable ToTable(this double[][] matrix)
        {
            int cols = matrix[0].Length;

            String[] columnNames = new String[cols];
            for (int i = 0; i < columnNames.Length; i++)
                columnNames[i] = "Column " + i;
            return ToTable(matrix, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[,] array.
        /// </summary>
        /// 
        public static DataTable ToTable(this double[][] matrix, params string[] columnNames)
        {
            DataTable table = new DataTable();
            table.Locale = CultureInfo.CurrentCulture;

            for (int i = 0; i < columnNames.Length; i++)
                table.Columns.Add(columnNames[i], typeof(double));

            for (int i = 0; i < matrix.Length; i++)
                table.Rows.Add(matrix[i].Convert(x => (object)x));

            return table;
        }

        /// <summary>
        ///   Converts an array of values into a <see cref="DataTable"/>,
        ///   attempting to guess column types by inspecting the data.
        /// </summary>
        /// 
        /// <param name="values">The values to be converted.</param>
        /// 
        /// <returns>A <see cref="DataTable"/> containing the given values.</returns>
        /// 
        /// <example>
        /// <code>
        /// // Specify some data in a table format
        /// //
        /// object[,] data = 
        /// {
        ///     { "Id", "IsSmoker", "Age" },
        ///     {   0,       1,        10  },
        ///     {   1,       1,        15  },
        ///     {   2,       0,        40  },
        ///     {   3,       1,        20  },
        ///     {   4,       0,        70  },
        ///     {   5,       0,        55  },
        /// };
        /// 
        /// // Create a new table with the data
        /// DataTable dataTable = data.ToTable();
        /// </code>
        /// </example>
        /// 
        public static DataTable ToTable(this object[,] values)
        {
            var columnNames = new string[values.Columns()];
            for (int i = 0; i < columnNames.Length; i++)
                columnNames[i] = "Column " + i;
            return ToTable(values, columnNames);
        }

        /// <summary>
        ///   Converts an array of values into a <see cref="DataTable"/>,
        ///   attempting to guess column types by inspecting the data.
        /// </summary>
        /// 
        /// <param name="matrix">The values to be converted.</param>
        /// <param name="columnNames">The column names to use in the data table.</param>
        /// 
        /// <returns>A <see cref="DataTable"/> containing the given values.</returns>
        /// 
        /// <example>
        /// <code>
        /// // Specify some data in a table format
        /// //
        /// object[,] data = 
        /// {
        ///     { "Id", "IsSmoker", "Age" },
        ///     {   0,       1,        10  },
        ///     {   1,       1,        15  },
        ///     {   2,       0,        40  },
        ///     {   3,       1,        20  },
        ///     {   4,       0,        70  },
        ///     {   5,       0,        55  },
        /// };
        /// 
        /// // Create a new table with the data
        /// DataTable dataTable = data.ToTable();
        /// </code>
        /// </example>
        /// 
        public static DataTable ToTable(this object[,] matrix, string[] columnNames)
        {
            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;

            object[] headers = matrix.GetRow(0);

            if (headers.All(x => x is String))
            {
                // Get first data row to set types
                object[] first = matrix.GetRow(1);

                // Assume first row is header row
                for (int i = 0; i < first.Length; i++)
                    table.Columns.Add(headers[i] as String, first[i].GetType());

                // Parse all the other rows
                int rows = matrix.GetLength(0);
                for (int i = 1; i < rows; i++)
                {
                    var row = matrix.GetRow(i);
                    table.Rows.Add(row);
                }
            }
            else
            {
                for (int i = 0; i < matrix.Columns(); i++)
                {
                    Type columnType = GetHighestEnclosingType(matrix.GetColumn(i));
                    table.Columns.Add(columnNames[i], columnType);
                }

                int rows = matrix.GetLength(0);
                for (int i = 0; i < rows; i++)
                {
                    var row = matrix.GetRow(i);
                    table.Rows.Add(row);
                }
            }

            return table;
        }

        private static Type GetHighestEnclosingType(object[] values)
        {
            var types = values.Select(x => x != null ? x.GetType() : null);
            if (types.Any(x => x == typeof(object)))
                return typeof(object);
            if (types.Any(x => x == typeof(string)))
                return typeof(string);
            if (types.Any(x => x == typeof(decimal)))
                return typeof(decimal);
            if (types.Any(x => x == typeof(double)))
                return typeof(double);
            if (types.Any(x => x == typeof(float)))
                return typeof(float);
            if (types.Any(x => x == typeof(int)))
                return typeof(int);
            if (types.Any(x => x == typeof(uint)))
                return typeof(uint);
            if (types.Any(x => x == typeof(short)))
                return typeof(int);
            if (types.Any(x => x == typeof(byte)))
                return typeof(int);
            if (types.Any(x => x == typeof(sbyte)))
                return typeof(int);

            return typeof(object);
        }



        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[][] ToJagged(this DataTable table)
        {
            return ToJagged<double>(table);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[][] ToJagged(this DataTable table, IFormatProvider provider)
        {
            return ToJagged<double>(table, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[][] ToJagged(this DataTable table, out string[] columnNames)
        {
            return ToJagged<double>(table, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[][] ToJagged(this DataTable table, IFormatProvider provider, out string[] columnNames)
        {
            return ToJagged<double>(table, provider, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[][] ToJagged(this DataTable table, params string[] columnNames)
        {
            return ToJagged<double>(table, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static T[][] ToJagged<T>(this DataTable table)
        {
            String[] names;
            return ToJagged<T>(table, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static T[][] ToJagged<T>(this DataTable table, IFormatProvider provider)
        {
            String[] names;
            return ToJagged<T>(table, provider, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static T[][] ToJagged<T>(this DataTable table, out string[] columnNames)
        {
            T[][] m = new T[table.Rows.Count][];
            columnNames = new string[table.Columns.Count];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = new T[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var value = table.Rows[i][j];
                    m[i][j] = (T)System.Convert.ChangeType(value, typeof(T));
                }

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static T[][] ToJagged<T>(this DataTable table, IFormatProvider provider, out string[] columnNames)
        {
            T[][] m = new T[table.Rows.Count][];
            columnNames = new string[table.Columns.Count];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = new T[table.Columns.Count];

            for (int j = 0; j < table.Columns.Count; j++)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                    m[i][j] = (T)System.Convert.ChangeType(table.Rows[i][j], typeof(T), provider);

                columnNames[j] = table.Columns[j].Caption;
            }

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static T[][] ToJagged<T>(this DataTable table, params string[] columnNames)
        {
            T[][] m = new T[table.Rows.Count][];

            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = new T[columnNames.Length];

            for (int j = 0; j < columnNames.Length; j++)
            {
                DataColumn col = table.Columns[columnNames[j]];

                for (int i = 0; i < table.Rows.Count; i++)
                    m[i][j] = (T)System.Convert.ChangeType(table.Rows[i][col], typeof(T));
            }

            return m;
        }





        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[] ToVector(this DataTable table)
        {
            return ToVector<double>(table);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static double[] ToVector(this DataTable table, IFormatProvider provider)
        {
            return ToVector<double>(table, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        public static double[] ToVector(this DataTable table, out string columnName)
        {
            return ToVector<double>(table, out columnName);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static double[] ToVector(this DataTable table, IFormatProvider provider, out string columnName)
        {
            return ToVector<double>(table, provider, out columnName);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static double[] ToVector(this DataTable table, string columnName)
        {
            return ToVector<double>(table, columnName);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static T[] ToVector<T>(this DataTable table)
        {
            String name;
            return ToVector<T>(table, out name);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static T[] ToVector<T>(this DataTable table, IFormatProvider provider)
        {
            String name;
            return ToVector<T>(table, provider, out name);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static T[] ToVector<T>(this DataTable table, out string columnName)
        {
            if (table.Columns.Count > 1)
                throw new ArgumentException("The given table has more than one column. Please specify which column should be converted.");

            columnName = table.Columns[0].ColumnName;
            return table.Columns[0].ToArray<T>();
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static T[] ToVector<T>(this DataTable table, IFormatProvider provider, out string columnName)
        {
            if (table.Columns.Count > 1)
                throw new ArgumentException("The given table has more than one column. Please specify which column should be converted.");

            columnName = table.Columns[0].ColumnName;
            return table.Columns[0].ToArray<T>(provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[] array.
        /// </summary>
        /// 
        public static T[] ToVector<T>(this DataTable table, string columnName)
        {
            return table.Columns[columnName].ToArray<T>();
        }






        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static double[][] ToArray(this DataTable table)
        {
            return ToJagged<double>(table);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static double[][] ToArray(this DataTable table, IFormatProvider provider)
        {
            return ToJagged<double>(table, provider);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static double[][] ToArray(this DataTable table, out string[] columnNames)
        {
            return ToJagged<double>(table, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static double[][] ToArray(this DataTable table, IFormatProvider provider, out string[] columnNames)
        {
            return ToJagged<double>(table, provider, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static double[][] ToArray(this DataTable table, params string[] columnNames)
        {
            return ToJagged<double>(table, columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this DataTable table)
        {
            String[] names;
            return ToJagged<T>(table, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this DataTable table, IFormatProvider provider)
        {
            String[] names;
            return ToJagged<T>(table, provider, out names);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this DataTable table, out string[] columnNames)
        {
            return ToJagged<T>(table, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this DataTable table, IFormatProvider provider, out string[] columnNames)
        {
            return ToJagged<T>(table, provider, out columnNames);
        }

        /// <summary>
        ///   Converts a DataTable to a double[][] array.
        /// </summary>
        /// 
        [Obsolete("Please use ToJagged() instead.")]
        public static T[][] ToArray<T>(this DataTable table, params string[] columnNames)
        {
            return ToJagged<T>(table, columnNames);
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static double[] ToArray(this DataColumn column)
        {
            return ToArray<double>(column);
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static double[] ToArray(this DataColumn column, IFormatProvider provider)
        {
            return ToArray<double>(column, provider);
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static double[] ToArray(this DataRow row, IFormatProvider provider, params string[] colNames)
        {
            return ToArray<double>(row, provider, colNames);
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static double[] ToArray(this DataRow row, params string[] colNames)
        {
            return ToArray<double>(row, colNames);
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataColumn column)
        {
            T[] m = new T[column.Table.Rows.Count];

            for (int i = 0; i < m.Length; i++)
                m[i] = (T)System.Convert.ChangeType(column.Table.Rows[i][column], typeof(T));

            return m;
        }

        /// <summary>
        ///   Converts a DataColumn to a double[] array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataColumn column, IFormatProvider provider)
        {
            T[] m = new T[column.Table.Rows.Count];

            for (int i = 0; i < m.Length; i++)
                m[i] = (T)System.Convert.ChangeType(column.Table.Rows[i][column], typeof(T), provider);

            return m;
        }

        /// <summary>
        ///   Converts a DataColumn to a generic array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataRow row, params string[] colNames)
        {
            T[] m = new T[colNames.Length];

            for (int i = 0; i < m.Length; i++)
                m[i] = (T)System.Convert.ChangeType(row[colNames[i]], typeof(T));

            return m;
        }

        /// <summary>
        ///   Converts a DataColumn to a generic array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataRow row, IFormatProvider provider, params string[] colNames)
        {
            T[] m = new T[colNames.Length];

            for (int i = 0; i < m.Length; i++)
                m[i] = (T)System.Convert.ChangeType(row[colNames[i]], typeof(T), provider);

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a generic array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataTable table, string columnName)
        {
            T[] m = new T[table.Rows.Count];

            DataColumn col = table.Columns[columnName];
            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = (T)System.Convert.ChangeType(table.Rows[i][col], typeof(T));

            return m;
        }

        /// <summary>
        ///   Converts a DataTable to a generic array.
        /// </summary>
        /// 
        public static T[] ToArray<T>(this DataTable table, IFormatProvider provider, string columnName)
        {
            T[] m = new T[table.Rows.Count];

            DataColumn col = table.Columns[columnName];
            for (int i = 0; i < table.Rows.Count; i++)
                m[i] = (T)System.Convert.ChangeType(table.Rows[i][col], typeof(T), provider);

            return m;
        }
#endif
        #endregion
        
        #region Obsolete
#if !NETSTANDARD1_4
        /// <summary>
        ///   Converts a DataColumn to a int[] array.
        /// </summary>
        /// 
        [Obsolete("Use ToArray<T> instead.")]
        public static int[] ToInt32Array(this DataColumn column)
        {
            return ToArray<int>(column);
        }

        /// <summary>
        ///   Converts a DataTable to a int[][] array.
        /// </summary>
        /// 
        [Obsolete("Use ToArray<T> instead.")]
        public static int[][] ToIntArray(this DataTable table, params string[] columnNames)
        {
            return ToArray<int>(table, columnNames);
        }
#endif
        #endregion

        /// <summary>
        ///   Converts a integer to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this int[] value)
        {
            return ToInt16(value, new short[value.Length]);
        }

        /// <summary>
        ///   Converts a integer to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this int[,] value)
        {
            return ToInt16(value, Matrix.CreateAs<int, short>(value));
        }

        /// <summary>
        ///   Converts a integer to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this int[][] value)
        {
            return ToInt16(value, Jagged.CreateAs<int, short>(value));
        }

        /// <summary>
        ///   Converts a integer to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this int[][][] value)
        {
            return ToInt16(value, Jagged.CreateAs<int, short>(value));
        }




        /// <summary>
        ///   Converts a integer array to a short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this int[] value, short[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int16)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this int[,] value, short[,] result)
        {
            unsafe
            {
                fixed (int* src = value)
                fixed (short* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int16)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this int[,] value, short[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int16)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this int[][] value, short[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int16)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this int[][][] value, short[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int16)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this int[][] value, short[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int16)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this int[] value)
        {
            return ToSingle(value, new float[value.Length]);
        }

        /// <summary>
        ///   Converts a integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this int[,] value)
        {
            return ToSingle(value, Matrix.CreateAs<int, float>(value));
        }

        /// <summary>
        ///   Converts a integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this int[][] value)
        {
            return ToSingle(value, Jagged.CreateAs<int, float>(value));
        }

        /// <summary>
        ///   Converts a integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this int[][][] value)
        {
            return ToSingle(value, Jagged.CreateAs<int, float>(value));
        }




        /// <summary>
        ///   Converts a integer array to a single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this int[] value, float[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Single)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this int[,] value, float[,] result)
        {
            unsafe
            {
                fixed (int* src = value)
                fixed (float* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Single)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this int[,] value, float[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Single)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this int[][] value, float[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Single)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this int[][][] value, float[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Single)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this int[][] value, float[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Single)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this int[] value)
        {
            return ToDouble(value, new double[value.Length]);
        }

        /// <summary>
        ///   Converts a integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this int[,] value)
        {
            return ToDouble(value, Matrix.CreateAs<int, double>(value));
        }

        /// <summary>
        ///   Converts a integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this int[][] value)
        {
            return ToDouble(value, Jagged.CreateAs<int, double>(value));
        }

        /// <summary>
        ///   Converts a integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this int[][][] value)
        {
            return ToDouble(value, Jagged.CreateAs<int, double>(value));
        }




        /// <summary>
        ///   Converts a integer array to a double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this int[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Double)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this int[,] value, double[,] result)
        {
            unsafe
            {
                fixed (int* src = value)
                fixed (double* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Double)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this int[,] value, double[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Double)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this int[][] value, double[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Double)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this int[][][] value, double[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Double)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this int[][] value, double[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Double)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a integer to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this int[] value)
        {
            return ToInt64(value, new long[value.Length]);
        }

        /// <summary>
        ///   Converts a integer to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this int[,] value)
        {
            return ToInt64(value, Matrix.CreateAs<int, long>(value));
        }

        /// <summary>
        ///   Converts a integer to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this int[][] value)
        {
            return ToInt64(value, Jagged.CreateAs<int, long>(value));
        }

        /// <summary>
        ///   Converts a integer to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this int[][][] value)
        {
            return ToInt64(value, Jagged.CreateAs<int, long>(value));
        }




        /// <summary>
        ///   Converts a integer array to a long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this int[] value, long[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int64)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this int[,] value, long[,] result)
        {
            unsafe
            {
                fixed (int* src = value)
                fixed (long* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int64)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this int[,] value, long[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int64)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this int[][] value, long[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int64)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this int[][][] value, long[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int64)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this int[][] value, long[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int64)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this int[] value)
        {
            return ToByte(value, new byte[value.Length]);
        }

        /// <summary>
        ///   Converts a integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this int[,] value)
        {
            return ToByte(value, Matrix.CreateAs<int, byte>(value));
        }

        /// <summary>
        ///   Converts a integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this int[][] value)
        {
            return ToByte(value, Jagged.CreateAs<int, byte>(value));
        }

        /// <summary>
        ///   Converts a integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this int[][][] value)
        {
            return ToByte(value, Jagged.CreateAs<int, byte>(value));
        }




        /// <summary>
        ///   Converts a integer array to a 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this int[] value, byte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Byte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this int[,] value, byte[,] result)
        {
            unsafe
            {
                fixed (int* src = value)
                fixed (byte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Byte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this int[,] value, byte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Byte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this int[][] value, byte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Byte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this int[][][] value, byte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Byte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this int[][] value, byte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Byte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this int[] value)
        {
            return ToSByte(value, new sbyte[value.Length]);
        }

        /// <summary>
        ///   Converts a integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this int[,] value)
        {
            return ToSByte(value, Matrix.CreateAs<int, sbyte>(value));
        }

        /// <summary>
        ///   Converts a integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this int[][] value)
        {
            return ToSByte(value, Jagged.CreateAs<int, sbyte>(value));
        }

        /// <summary>
        ///   Converts a integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this int[][][] value)
        {
            return ToSByte(value, Jagged.CreateAs<int, sbyte>(value));
        }




        /// <summary>
        ///   Converts a integer array to a signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this int[] value, sbyte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (SByte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this int[,] value, sbyte[,] result)
        {
            unsafe
            {
                fixed (int* src = value)
                fixed (sbyte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (SByte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this int[,] value, sbyte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (SByte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this int[][] value, sbyte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (SByte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this int[][][] value, sbyte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (SByte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this int[][] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (SByte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this int[] value)
        {
            return ToDecimal(value, new decimal[value.Length]);
        }

        /// <summary>
        ///   Converts a integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this int[,] value)
        {
            return ToDecimal(value, Matrix.CreateAs<int, decimal>(value));
        }

        /// <summary>
        ///   Converts a integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this int[][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<int, decimal>(value));
        }

        /// <summary>
        ///   Converts a integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this int[][][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<int, decimal>(value));
        }




        /// <summary>
        ///   Converts a integer array to a decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this int[] value, decimal[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Decimal)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this int[,] value, decimal[,] result)
        {
            unsafe
            {
                fixed (int* src = value)
                fixed (decimal* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Decimal)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this int[,] value, decimal[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Decimal)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this int[][] value, decimal[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Decimal)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this int[][][] value, decimal[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Decimal)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this int[][] value, decimal[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Decimal)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this int[] value)
        {
            return ToBoolean(value, new bool[value.Length]);
        }

        /// <summary>
        ///   Converts a integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this int[,] value)
        {
            return ToBoolean(value, Matrix.CreateAs<int, bool>(value));
        }

        /// <summary>
        ///   Converts a integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this int[][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<int, bool>(value));
        }

        /// <summary>
        ///   Converts a integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this int[][][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<int, bool>(value));
        }




        /// <summary>
        ///   Converts a integer array to a boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this int[] value, bool[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this int[,] value, bool[,] result)
        {
            unsafe
            {
                fixed (int* src = value)
                fixed (bool* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] != 0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this int[,] value, bool[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this int[][] value, bool[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this int[][][] value, bool[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this int[][] value, bool[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] != 0;
            return result;
        }
        /// <summary>
        ///   Converts a integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this int[] value)
        {
            return ToObject(value, new object[value.Length]);
        }

        /// <summary>
        ///   Converts a integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this int[,] value)
        {
            return ToObject(value, Matrix.CreateAs<int, object>(value));
        }

        /// <summary>
        ///   Converts a integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this int[][] value)
        {
            return ToObject(value, Jagged.CreateAs<int, object>(value));
        }

        /// <summary>
        ///   Converts a integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this int[][][] value)
        {
            return ToObject(value, Jagged.CreateAs<int, object>(value));
        }




        /// <summary>
        ///   Converts a integer array to a object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this int[] value, object[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Object)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this int[,] value, object[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Object)value[i, j];

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this int[,] value, object[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Object)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this int[][] value, object[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Object)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this int[][][] value, object[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Object)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this int[][] value, object[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Object)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this int[] value)
        {
            return ToString(value, new string[value.Length]);
        }

        /// <summary>
        ///   Converts a integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this int[,] value)
        {
            return ToString(value, Matrix.CreateAs<int, string>(value));
        }

        /// <summary>
        ///   Converts a integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this int[][] value)
        {
            return ToString(value, Jagged.CreateAs<int, string>(value));
        }

        /// <summary>
        ///   Converts a integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this int[][][] value)
        {
            return ToString(value, Jagged.CreateAs<int, string>(value));
        }




        /// <summary>
        ///   Converts a integer array to a string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this int[] value, string[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this int[,] value, string[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = value[i, j].ToString(); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional integer array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this int[,] value, string[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this int[][] value, string[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this int[][][] value, string[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged integer array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this int[][] value, string[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j].ToString(); ;
            return result;
        }
        /// <summary>
        ///   Converts a short integer to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this short[] value)
        {
            return ToInt32(value, new int[value.Length]);
        }

        /// <summary>
        ///   Converts a short integer to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this short[,] value)
        {
            return ToInt32(value, Matrix.CreateAs<short, int>(value));
        }

        /// <summary>
        ///   Converts a short integer to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this short[][] value)
        {
            return ToInt32(value, Jagged.CreateAs<short, int>(value));
        }

        /// <summary>
        ///   Converts a short integer to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this short[][][] value)
        {
            return ToInt32(value, Jagged.CreateAs<short, int>(value));
        }




        /// <summary>
        ///   Converts a short integer array to a integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this short[] value, int[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int32)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this short[,] value, int[,] result)
        {
            unsafe
            {
                fixed (short* src = value)
                fixed (int* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int32)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this short[,] value, int[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int32)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this short[][] value, int[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int32)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this short[][][] value, int[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int32)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this short[][] value, int[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int32)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a short integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this short[] value)
        {
            return ToSingle(value, new float[value.Length]);
        }

        /// <summary>
        ///   Converts a short integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this short[,] value)
        {
            return ToSingle(value, Matrix.CreateAs<short, float>(value));
        }

        /// <summary>
        ///   Converts a short integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this short[][] value)
        {
            return ToSingle(value, Jagged.CreateAs<short, float>(value));
        }

        /// <summary>
        ///   Converts a short integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this short[][][] value)
        {
            return ToSingle(value, Jagged.CreateAs<short, float>(value));
        }




        /// <summary>
        ///   Converts a short integer array to a single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this short[] value, float[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Single)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this short[,] value, float[,] result)
        {
            unsafe
            {
                fixed (short* src = value)
                fixed (float* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Single)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this short[,] value, float[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Single)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this short[][] value, float[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Single)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this short[][][] value, float[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Single)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this short[][] value, float[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Single)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a short integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this short[] value)
        {
            return ToDouble(value, new double[value.Length]);
        }

        /// <summary>
        ///   Converts a short integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this short[,] value)
        {
            return ToDouble(value, Matrix.CreateAs<short, double>(value));
        }

        /// <summary>
        ///   Converts a short integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this short[][] value)
        {
            return ToDouble(value, Jagged.CreateAs<short, double>(value));
        }

        /// <summary>
        ///   Converts a short integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this short[][][] value)
        {
            return ToDouble(value, Jagged.CreateAs<short, double>(value));
        }




        /// <summary>
        ///   Converts a short integer array to a double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this short[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Double)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this short[,] value, double[,] result)
        {
            unsafe
            {
                fixed (short* src = value)
                fixed (double* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Double)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this short[,] value, double[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Double)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this short[][] value, double[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Double)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this short[][][] value, double[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Double)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this short[][] value, double[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Double)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a short integer to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this short[] value)
        {
            return ToInt64(value, new long[value.Length]);
        }

        /// <summary>
        ///   Converts a short integer to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this short[,] value)
        {
            return ToInt64(value, Matrix.CreateAs<short, long>(value));
        }

        /// <summary>
        ///   Converts a short integer to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this short[][] value)
        {
            return ToInt64(value, Jagged.CreateAs<short, long>(value));
        }

        /// <summary>
        ///   Converts a short integer to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this short[][][] value)
        {
            return ToInt64(value, Jagged.CreateAs<short, long>(value));
        }




        /// <summary>
        ///   Converts a short integer array to a long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this short[] value, long[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int64)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this short[,] value, long[,] result)
        {
            unsafe
            {
                fixed (short* src = value)
                fixed (long* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int64)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this short[,] value, long[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int64)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this short[][] value, long[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int64)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this short[][][] value, long[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int64)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this short[][] value, long[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int64)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a short integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this short[] value)
        {
            return ToByte(value, new byte[value.Length]);
        }

        /// <summary>
        ///   Converts a short integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this short[,] value)
        {
            return ToByte(value, Matrix.CreateAs<short, byte>(value));
        }

        /// <summary>
        ///   Converts a short integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this short[][] value)
        {
            return ToByte(value, Jagged.CreateAs<short, byte>(value));
        }

        /// <summary>
        ///   Converts a short integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this short[][][] value)
        {
            return ToByte(value, Jagged.CreateAs<short, byte>(value));
        }




        /// <summary>
        ///   Converts a short integer array to a 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this short[] value, byte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Byte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this short[,] value, byte[,] result)
        {
            unsafe
            {
                fixed (short* src = value)
                fixed (byte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Byte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this short[,] value, byte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Byte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this short[][] value, byte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Byte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this short[][][] value, byte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Byte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this short[][] value, byte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Byte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a short integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this short[] value)
        {
            return ToSByte(value, new sbyte[value.Length]);
        }

        /// <summary>
        ///   Converts a short integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this short[,] value)
        {
            return ToSByte(value, Matrix.CreateAs<short, sbyte>(value));
        }

        /// <summary>
        ///   Converts a short integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this short[][] value)
        {
            return ToSByte(value, Jagged.CreateAs<short, sbyte>(value));
        }

        /// <summary>
        ///   Converts a short integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this short[][][] value)
        {
            return ToSByte(value, Jagged.CreateAs<short, sbyte>(value));
        }




        /// <summary>
        ///   Converts a short integer array to a signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this short[] value, sbyte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (SByte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this short[,] value, sbyte[,] result)
        {
            unsafe
            {
                fixed (short* src = value)
                fixed (sbyte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (SByte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this short[,] value, sbyte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (SByte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this short[][] value, sbyte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (SByte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this short[][][] value, sbyte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (SByte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this short[][] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (SByte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a short integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this short[] value)
        {
            return ToDecimal(value, new decimal[value.Length]);
        }

        /// <summary>
        ///   Converts a short integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this short[,] value)
        {
            return ToDecimal(value, Matrix.CreateAs<short, decimal>(value));
        }

        /// <summary>
        ///   Converts a short integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this short[][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<short, decimal>(value));
        }

        /// <summary>
        ///   Converts a short integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this short[][][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<short, decimal>(value));
        }




        /// <summary>
        ///   Converts a short integer array to a decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this short[] value, decimal[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Decimal)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this short[,] value, decimal[,] result)
        {
            unsafe
            {
                fixed (short* src = value)
                fixed (decimal* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Decimal)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this short[,] value, decimal[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Decimal)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this short[][] value, decimal[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Decimal)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this short[][][] value, decimal[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Decimal)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this short[][] value, decimal[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Decimal)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a short integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this short[] value)
        {
            return ToBoolean(value, new bool[value.Length]);
        }

        /// <summary>
        ///   Converts a short integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this short[,] value)
        {
            return ToBoolean(value, Matrix.CreateAs<short, bool>(value));
        }

        /// <summary>
        ///   Converts a short integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this short[][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<short, bool>(value));
        }

        /// <summary>
        ///   Converts a short integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this short[][][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<short, bool>(value));
        }




        /// <summary>
        ///   Converts a short integer array to a boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this short[] value, bool[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this short[,] value, bool[,] result)
        {
            unsafe
            {
                fixed (short* src = value)
                fixed (bool* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] != 0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this short[,] value, bool[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this short[][] value, bool[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this short[][][] value, bool[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this short[][] value, bool[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] != 0;
            return result;
        }
        /// <summary>
        ///   Converts a short integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this short[] value)
        {
            return ToObject(value, new object[value.Length]);
        }

        /// <summary>
        ///   Converts a short integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this short[,] value)
        {
            return ToObject(value, Matrix.CreateAs<short, object>(value));
        }

        /// <summary>
        ///   Converts a short integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this short[][] value)
        {
            return ToObject(value, Jagged.CreateAs<short, object>(value));
        }

        /// <summary>
        ///   Converts a short integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this short[][][] value)
        {
            return ToObject(value, Jagged.CreateAs<short, object>(value));
        }




        /// <summary>
        ///   Converts a short integer array to a object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this short[] value, object[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Object)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this short[,] value, object[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Object)value[i, j];

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this short[,] value, object[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Object)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this short[][] value, object[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Object)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this short[][][] value, object[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Object)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this short[][] value, object[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Object)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a short integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this short[] value)
        {
            return ToString(value, new string[value.Length]);
        }

        /// <summary>
        ///   Converts a short integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this short[,] value)
        {
            return ToString(value, Matrix.CreateAs<short, string>(value));
        }

        /// <summary>
        ///   Converts a short integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this short[][] value)
        {
            return ToString(value, Jagged.CreateAs<short, string>(value));
        }

        /// <summary>
        ///   Converts a short integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this short[][][] value)
        {
            return ToString(value, Jagged.CreateAs<short, string>(value));
        }




        /// <summary>
        ///   Converts a short integer array to a string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this short[] value, string[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this short[,] value, string[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = value[i, j].ToString(); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional short integer array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this short[,] value, string[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this short[][] value, string[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this short[][][] value, string[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged short integer array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this short[][] value, string[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j].ToString(); ;
            return result;
        }
        /// <summary>
        ///   Converts a single-precision floating point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this float[] value)
        {
            return ToInt32(value, new int[value.Length]);
        }

        /// <summary>
        ///   Converts a single-precision floating point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this float[,] value)
        {
            return ToInt32(value, Matrix.CreateAs<float, int>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this float[][] value)
        {
            return ToInt32(value, Jagged.CreateAs<float, int>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this float[][][] value)
        {
            return ToInt32(value, Jagged.CreateAs<float, int>(value));
        }




        /// <summary>
        ///   Converts a single-precision floating point array to a integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this float[] value, int[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int32)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this float[,] value, int[,] result)
        {
            unsafe
            {
                fixed (float* src = value)
                fixed (int* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int32)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this float[,] value, int[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int32)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this float[][] value, int[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int32)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this float[][][] value, int[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int32)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this float[][] value, int[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int32)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a single-precision floating point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this float[] value)
        {
            return ToInt16(value, new short[value.Length]);
        }

        /// <summary>
        ///   Converts a single-precision floating point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this float[,] value)
        {
            return ToInt16(value, Matrix.CreateAs<float, short>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this float[][] value)
        {
            return ToInt16(value, Jagged.CreateAs<float, short>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this float[][][] value)
        {
            return ToInt16(value, Jagged.CreateAs<float, short>(value));
        }




        /// <summary>
        ///   Converts a single-precision floating point array to a short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this float[] value, short[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int16)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this float[,] value, short[,] result)
        {
            unsafe
            {
                fixed (float* src = value)
                fixed (short* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int16)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this float[,] value, short[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int16)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this float[][] value, short[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int16)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this float[][][] value, short[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int16)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this float[][] value, short[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int16)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a single-precision floating point to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this float[] value)
        {
            return ToDouble(value, new double[value.Length]);
        }

        /// <summary>
        ///   Converts a single-precision floating point to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this float[,] value)
        {
            return ToDouble(value, Matrix.CreateAs<float, double>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this float[][] value)
        {
            return ToDouble(value, Jagged.CreateAs<float, double>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this float[][][] value)
        {
            return ToDouble(value, Jagged.CreateAs<float, double>(value));
        }




        /// <summary>
        ///   Converts a single-precision floating point array to a double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this float[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Double)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this float[,] value, double[,] result)
        {
            unsafe
            {
                fixed (float* src = value)
                fixed (double* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Double)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this float[,] value, double[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Double)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this float[][] value, double[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Double)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this float[][][] value, double[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Double)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this float[][] value, double[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Double)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a single-precision floating point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this float[] value)
        {
            return ToInt64(value, new long[value.Length]);
        }

        /// <summary>
        ///   Converts a single-precision floating point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this float[,] value)
        {
            return ToInt64(value, Matrix.CreateAs<float, long>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this float[][] value)
        {
            return ToInt64(value, Jagged.CreateAs<float, long>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this float[][][] value)
        {
            return ToInt64(value, Jagged.CreateAs<float, long>(value));
        }




        /// <summary>
        ///   Converts a single-precision floating point array to a long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this float[] value, long[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int64)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this float[,] value, long[,] result)
        {
            unsafe
            {
                fixed (float* src = value)
                fixed (long* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int64)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this float[,] value, long[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int64)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this float[][] value, long[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int64)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this float[][][] value, long[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int64)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this float[][] value, long[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int64)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a single-precision floating point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this float[] value)
        {
            return ToByte(value, new byte[value.Length]);
        }

        /// <summary>
        ///   Converts a single-precision floating point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this float[,] value)
        {
            return ToByte(value, Matrix.CreateAs<float, byte>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this float[][] value)
        {
            return ToByte(value, Jagged.CreateAs<float, byte>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this float[][][] value)
        {
            return ToByte(value, Jagged.CreateAs<float, byte>(value));
        }




        /// <summary>
        ///   Converts a single-precision floating point array to a 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this float[] value, byte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Byte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this float[,] value, byte[,] result)
        {
            unsafe
            {
                fixed (float* src = value)
                fixed (byte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Byte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this float[,] value, byte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Byte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this float[][] value, byte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Byte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this float[][][] value, byte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Byte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this float[][] value, byte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Byte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a single-precision floating point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this float[] value)
        {
            return ToSByte(value, new sbyte[value.Length]);
        }

        /// <summary>
        ///   Converts a single-precision floating point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this float[,] value)
        {
            return ToSByte(value, Matrix.CreateAs<float, sbyte>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this float[][] value)
        {
            return ToSByte(value, Jagged.CreateAs<float, sbyte>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this float[][][] value)
        {
            return ToSByte(value, Jagged.CreateAs<float, sbyte>(value));
        }




        /// <summary>
        ///   Converts a single-precision floating point array to a signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this float[] value, sbyte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (SByte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this float[,] value, sbyte[,] result)
        {
            unsafe
            {
                fixed (float* src = value)
                fixed (sbyte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (SByte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this float[,] value, sbyte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (SByte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this float[][] value, sbyte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (SByte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this float[][][] value, sbyte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (SByte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this float[][] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (SByte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a single-precision floating point to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this float[] value)
        {
            return ToDecimal(value, new decimal[value.Length]);
        }

        /// <summary>
        ///   Converts a single-precision floating point to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this float[,] value)
        {
            return ToDecimal(value, Matrix.CreateAs<float, decimal>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this float[][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<float, decimal>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this float[][][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<float, decimal>(value));
        }




        /// <summary>
        ///   Converts a single-precision floating point array to a decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this float[] value, decimal[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Decimal)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this float[,] value, decimal[,] result)
        {
            unsafe
            {
                fixed (float* src = value)
                fixed (decimal* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Decimal)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this float[,] value, decimal[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Decimal)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this float[][] value, decimal[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Decimal)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this float[][][] value, decimal[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Decimal)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this float[][] value, decimal[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Decimal)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a single-precision floating point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this float[] value)
        {
            return ToBoolean(value, new bool[value.Length]);
        }

        /// <summary>
        ///   Converts a single-precision floating point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this float[,] value)
        {
            return ToBoolean(value, Matrix.CreateAs<float, bool>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this float[][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<float, bool>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this float[][][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<float, bool>(value));
        }




        /// <summary>
        ///   Converts a single-precision floating point array to a boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this float[] value, bool[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this float[,] value, bool[,] result)
        {
            unsafe
            {
                fixed (float* src = value)
                fixed (bool* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] != 0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this float[,] value, bool[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this float[][] value, bool[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this float[][][] value, bool[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this float[][] value, bool[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] != 0;
            return result;
        }
        /// <summary>
        ///   Converts a single-precision floating point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this float[] value)
        {
            return ToObject(value, new object[value.Length]);
        }

        /// <summary>
        ///   Converts a single-precision floating point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this float[,] value)
        {
            return ToObject(value, Matrix.CreateAs<float, object>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this float[][] value)
        {
            return ToObject(value, Jagged.CreateAs<float, object>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this float[][][] value)
        {
            return ToObject(value, Jagged.CreateAs<float, object>(value));
        }




        /// <summary>
        ///   Converts a single-precision floating point array to a object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this float[] value, object[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Object)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this float[,] value, object[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Object)value[i, j];

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this float[,] value, object[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Object)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this float[][] value, object[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Object)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this float[][][] value, object[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Object)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this float[][] value, object[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Object)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a single-precision floating point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this float[] value)
        {
            return ToString(value, new string[value.Length]);
        }

        /// <summary>
        ///   Converts a single-precision floating point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this float[,] value)
        {
            return ToString(value, Matrix.CreateAs<float, string>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this float[][] value)
        {
            return ToString(value, Jagged.CreateAs<float, string>(value));
        }

        /// <summary>
        ///   Converts a single-precision floating point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this float[][][] value)
        {
            return ToString(value, Jagged.CreateAs<float, string>(value));
        }




        /// <summary>
        ///   Converts a single-precision floating point array to a string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this float[] value, string[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this float[,] value, string[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = value[i, j].ToString(); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional single-precision floating point array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this float[,] value, string[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this float[][] value, string[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this float[][][] value, string[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged single-precision floating point array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this float[][] value, string[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j].ToString(); ;
            return result;
        }
        /// <summary>
        ///   Converts a double-precision floating point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this double[] value)
        {
            return ToInt32(value, new int[value.Length]);
        }

        /// <summary>
        ///   Converts a double-precision floating point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this double[,] value)
        {
            return ToInt32(value, Matrix.CreateAs<double, int>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this double[][] value)
        {
            return ToInt32(value, Jagged.CreateAs<double, int>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this double[][][] value)
        {
            return ToInt32(value, Jagged.CreateAs<double, int>(value));
        }




        /// <summary>
        ///   Converts a double-precision floating point array to a integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this double[] value, int[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int32)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this double[,] value, int[,] result)
        {
            unsafe
            {
                fixed (double* src = value)
                fixed (int* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int32)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this double[,] value, int[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int32)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this double[][] value, int[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int32)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this double[][][] value, int[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int32)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this double[][] value, int[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int32)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a double-precision floating point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this double[] value)
        {
            return ToInt16(value, new short[value.Length]);
        }

        /// <summary>
        ///   Converts a double-precision floating point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this double[,] value)
        {
            return ToInt16(value, Matrix.CreateAs<double, short>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this double[][] value)
        {
            return ToInt16(value, Jagged.CreateAs<double, short>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this double[][][] value)
        {
            return ToInt16(value, Jagged.CreateAs<double, short>(value));
        }




        /// <summary>
        ///   Converts a double-precision floating point array to a short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this double[] value, short[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int16)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this double[,] value, short[,] result)
        {
            unsafe
            {
                fixed (double* src = value)
                fixed (short* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int16)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this double[,] value, short[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int16)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this double[][] value, short[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int16)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this double[][][] value, short[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int16)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this double[][] value, short[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int16)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a double-precision floating point to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this double[] value)
        {
            return ToSingle(value, new float[value.Length]);
        }

        /// <summary>
        ///   Converts a double-precision floating point to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this double[,] value)
        {
            return ToSingle(value, Matrix.CreateAs<double, float>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this double[][] value)
        {
            return ToSingle(value, Jagged.CreateAs<double, float>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this double[][][] value)
        {
            return ToSingle(value, Jagged.CreateAs<double, float>(value));
        }




        /// <summary>
        ///   Converts a double-precision floating point array to a single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this double[] value, float[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Single)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this double[,] value, float[,] result)
        {
            unsafe
            {
                fixed (double* src = value)
                fixed (float* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Single)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this double[,] value, float[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Single)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this double[][] value, float[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Single)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this double[][][] value, float[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Single)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this double[][] value, float[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Single)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a double-precision floating point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this double[] value)
        {
            return ToInt64(value, new long[value.Length]);
        }

        /// <summary>
        ///   Converts a double-precision floating point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this double[,] value)
        {
            return ToInt64(value, Matrix.CreateAs<double, long>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this double[][] value)
        {
            return ToInt64(value, Jagged.CreateAs<double, long>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this double[][][] value)
        {
            return ToInt64(value, Jagged.CreateAs<double, long>(value));
        }




        /// <summary>
        ///   Converts a double-precision floating point array to a long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this double[] value, long[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int64)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this double[,] value, long[,] result)
        {
            unsafe
            {
                fixed (double* src = value)
                fixed (long* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int64)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this double[,] value, long[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int64)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this double[][] value, long[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int64)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this double[][][] value, long[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int64)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this double[][] value, long[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int64)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a double-precision floating point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this double[] value)
        {
            return ToByte(value, new byte[value.Length]);
        }

        /// <summary>
        ///   Converts a double-precision floating point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this double[,] value)
        {
            return ToByte(value, Matrix.CreateAs<double, byte>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this double[][] value)
        {
            return ToByte(value, Jagged.CreateAs<double, byte>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this double[][][] value)
        {
            return ToByte(value, Jagged.CreateAs<double, byte>(value));
        }




        /// <summary>
        ///   Converts a double-precision floating point array to a 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this double[] value, byte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Byte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this double[,] value, byte[,] result)
        {
            unsafe
            {
                fixed (double* src = value)
                fixed (byte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Byte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this double[,] value, byte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Byte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this double[][] value, byte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Byte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this double[][][] value, byte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Byte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this double[][] value, byte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Byte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a double-precision floating point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this double[] value)
        {
            return ToSByte(value, new sbyte[value.Length]);
        }

        /// <summary>
        ///   Converts a double-precision floating point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this double[,] value)
        {
            return ToSByte(value, Matrix.CreateAs<double, sbyte>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this double[][] value)
        {
            return ToSByte(value, Jagged.CreateAs<double, sbyte>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this double[][][] value)
        {
            return ToSByte(value, Jagged.CreateAs<double, sbyte>(value));
        }




        /// <summary>
        ///   Converts a double-precision floating point array to a signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this double[] value, sbyte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (SByte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this double[,] value, sbyte[,] result)
        {
            unsafe
            {
                fixed (double* src = value)
                fixed (sbyte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (SByte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this double[,] value, sbyte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (SByte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this double[][] value, sbyte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (SByte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this double[][][] value, sbyte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (SByte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this double[][] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (SByte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a double-precision floating point to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this double[] value)
        {
            return ToDecimal(value, new decimal[value.Length]);
        }

        /// <summary>
        ///   Converts a double-precision floating point to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this double[,] value)
        {
            return ToDecimal(value, Matrix.CreateAs<double, decimal>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this double[][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<double, decimal>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this double[][][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<double, decimal>(value));
        }




        /// <summary>
        ///   Converts a double-precision floating point array to a decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this double[] value, decimal[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Decimal)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this double[,] value, decimal[,] result)
        {
            unsafe
            {
                fixed (double* src = value)
                fixed (decimal* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Decimal)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this double[,] value, decimal[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Decimal)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this double[][] value, decimal[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Decimal)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this double[][][] value, decimal[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Decimal)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this double[][] value, decimal[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Decimal)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a double-precision floating point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this double[] value)
        {
            return ToBoolean(value, new bool[value.Length]);
        }

        /// <summary>
        ///   Converts a double-precision floating point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this double[,] value)
        {
            return ToBoolean(value, Matrix.CreateAs<double, bool>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this double[][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<double, bool>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this double[][][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<double, bool>(value));
        }




        /// <summary>
        ///   Converts a double-precision floating point array to a boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this double[] value, bool[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this double[,] value, bool[,] result)
        {
            unsafe
            {
                fixed (double* src = value)
                fixed (bool* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] != 0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this double[,] value, bool[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this double[][] value, bool[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this double[][][] value, bool[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this double[][] value, bool[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] != 0;
            return result;
        }
        /// <summary>
        ///   Converts a double-precision floating point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this double[] value)
        {
            return ToObject(value, new object[value.Length]);
        }

        /// <summary>
        ///   Converts a double-precision floating point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this double[,] value)
        {
            return ToObject(value, Matrix.CreateAs<double, object>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this double[][] value)
        {
            return ToObject(value, Jagged.CreateAs<double, object>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this double[][][] value)
        {
            return ToObject(value, Jagged.CreateAs<double, object>(value));
        }




        /// <summary>
        ///   Converts a double-precision floating point array to a object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this double[] value, object[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Object)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this double[,] value, object[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Object)value[i, j];

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this double[,] value, object[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Object)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this double[][] value, object[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Object)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this double[][][] value, object[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Object)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this double[][] value, object[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Object)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a double-precision floating point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this double[] value)
        {
            return ToString(value, new string[value.Length]);
        }

        /// <summary>
        ///   Converts a double-precision floating point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this double[,] value)
        {
            return ToString(value, Matrix.CreateAs<double, string>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this double[][] value)
        {
            return ToString(value, Jagged.CreateAs<double, string>(value));
        }

        /// <summary>
        ///   Converts a double-precision floating point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this double[][][] value)
        {
            return ToString(value, Jagged.CreateAs<double, string>(value));
        }




        /// <summary>
        ///   Converts a double-precision floating point array to a string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this double[] value, string[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this double[,] value, string[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = value[i, j].ToString(); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional double-precision floating point array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this double[,] value, string[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this double[][] value, string[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this double[][][] value, string[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged double-precision floating point array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this double[][] value, string[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j].ToString(); ;
            return result;
        }
        /// <summary>
        ///   Converts a long integer to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this long[] value)
        {
            return ToInt32(value, new int[value.Length]);
        }

        /// <summary>
        ///   Converts a long integer to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this long[,] value)
        {
            return ToInt32(value, Matrix.CreateAs<long, int>(value));
        }

        /// <summary>
        ///   Converts a long integer to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this long[][] value)
        {
            return ToInt32(value, Jagged.CreateAs<long, int>(value));
        }

        /// <summary>
        ///   Converts a long integer to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this long[][][] value)
        {
            return ToInt32(value, Jagged.CreateAs<long, int>(value));
        }




        /// <summary>
        ///   Converts a long integer array to a integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this long[] value, int[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int32)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this long[,] value, int[,] result)
        {
            unsafe
            {
                fixed (long* src = value)
                fixed (int* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int32)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this long[,] value, int[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int32)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this long[][] value, int[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int32)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this long[][][] value, int[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int32)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this long[][] value, int[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int32)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a long integer to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this long[] value)
        {
            return ToInt16(value, new short[value.Length]);
        }

        /// <summary>
        ///   Converts a long integer to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this long[,] value)
        {
            return ToInt16(value, Matrix.CreateAs<long, short>(value));
        }

        /// <summary>
        ///   Converts a long integer to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this long[][] value)
        {
            return ToInt16(value, Jagged.CreateAs<long, short>(value));
        }

        /// <summary>
        ///   Converts a long integer to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this long[][][] value)
        {
            return ToInt16(value, Jagged.CreateAs<long, short>(value));
        }




        /// <summary>
        ///   Converts a long integer array to a short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this long[] value, short[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int16)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this long[,] value, short[,] result)
        {
            unsafe
            {
                fixed (long* src = value)
                fixed (short* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int16)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this long[,] value, short[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int16)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this long[][] value, short[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int16)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this long[][][] value, short[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int16)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this long[][] value, short[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int16)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a long integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this long[] value)
        {
            return ToSingle(value, new float[value.Length]);
        }

        /// <summary>
        ///   Converts a long integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this long[,] value)
        {
            return ToSingle(value, Matrix.CreateAs<long, float>(value));
        }

        /// <summary>
        ///   Converts a long integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this long[][] value)
        {
            return ToSingle(value, Jagged.CreateAs<long, float>(value));
        }

        /// <summary>
        ///   Converts a long integer to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this long[][][] value)
        {
            return ToSingle(value, Jagged.CreateAs<long, float>(value));
        }




        /// <summary>
        ///   Converts a long integer array to a single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this long[] value, float[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Single)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this long[,] value, float[,] result)
        {
            unsafe
            {
                fixed (long* src = value)
                fixed (float* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Single)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this long[,] value, float[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Single)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this long[][] value, float[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Single)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this long[][][] value, float[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Single)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this long[][] value, float[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Single)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a long integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this long[] value)
        {
            return ToDouble(value, new double[value.Length]);
        }

        /// <summary>
        ///   Converts a long integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this long[,] value)
        {
            return ToDouble(value, Matrix.CreateAs<long, double>(value));
        }

        /// <summary>
        ///   Converts a long integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this long[][] value)
        {
            return ToDouble(value, Jagged.CreateAs<long, double>(value));
        }

        /// <summary>
        ///   Converts a long integer to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this long[][][] value)
        {
            return ToDouble(value, Jagged.CreateAs<long, double>(value));
        }




        /// <summary>
        ///   Converts a long integer array to a double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this long[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Double)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this long[,] value, double[,] result)
        {
            unsafe
            {
                fixed (long* src = value)
                fixed (double* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Double)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this long[,] value, double[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Double)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this long[][] value, double[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Double)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this long[][][] value, double[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Double)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this long[][] value, double[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Double)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a long integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this long[] value)
        {
            return ToByte(value, new byte[value.Length]);
        }

        /// <summary>
        ///   Converts a long integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this long[,] value)
        {
            return ToByte(value, Matrix.CreateAs<long, byte>(value));
        }

        /// <summary>
        ///   Converts a long integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this long[][] value)
        {
            return ToByte(value, Jagged.CreateAs<long, byte>(value));
        }

        /// <summary>
        ///   Converts a long integer to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this long[][][] value)
        {
            return ToByte(value, Jagged.CreateAs<long, byte>(value));
        }




        /// <summary>
        ///   Converts a long integer array to a 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this long[] value, byte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Byte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this long[,] value, byte[,] result)
        {
            unsafe
            {
                fixed (long* src = value)
                fixed (byte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Byte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this long[,] value, byte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Byte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this long[][] value, byte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Byte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this long[][][] value, byte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Byte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this long[][] value, byte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Byte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a long integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this long[] value)
        {
            return ToSByte(value, new sbyte[value.Length]);
        }

        /// <summary>
        ///   Converts a long integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this long[,] value)
        {
            return ToSByte(value, Matrix.CreateAs<long, sbyte>(value));
        }

        /// <summary>
        ///   Converts a long integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this long[][] value)
        {
            return ToSByte(value, Jagged.CreateAs<long, sbyte>(value));
        }

        /// <summary>
        ///   Converts a long integer to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this long[][][] value)
        {
            return ToSByte(value, Jagged.CreateAs<long, sbyte>(value));
        }




        /// <summary>
        ///   Converts a long integer array to a signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this long[] value, sbyte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (SByte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this long[,] value, sbyte[,] result)
        {
            unsafe
            {
                fixed (long* src = value)
                fixed (sbyte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (SByte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this long[,] value, sbyte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (SByte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this long[][] value, sbyte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (SByte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this long[][][] value, sbyte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (SByte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this long[][] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (SByte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a long integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this long[] value)
        {
            return ToDecimal(value, new decimal[value.Length]);
        }

        /// <summary>
        ///   Converts a long integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this long[,] value)
        {
            return ToDecimal(value, Matrix.CreateAs<long, decimal>(value));
        }

        /// <summary>
        ///   Converts a long integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this long[][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<long, decimal>(value));
        }

        /// <summary>
        ///   Converts a long integer to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this long[][][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<long, decimal>(value));
        }




        /// <summary>
        ///   Converts a long integer array to a decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this long[] value, decimal[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Decimal)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this long[,] value, decimal[,] result)
        {
            unsafe
            {
                fixed (long* src = value)
                fixed (decimal* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Decimal)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this long[,] value, decimal[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Decimal)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this long[][] value, decimal[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Decimal)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this long[][][] value, decimal[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Decimal)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this long[][] value, decimal[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Decimal)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a long integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this long[] value)
        {
            return ToBoolean(value, new bool[value.Length]);
        }

        /// <summary>
        ///   Converts a long integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this long[,] value)
        {
            return ToBoolean(value, Matrix.CreateAs<long, bool>(value));
        }

        /// <summary>
        ///   Converts a long integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this long[][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<long, bool>(value));
        }

        /// <summary>
        ///   Converts a long integer to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this long[][][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<long, bool>(value));
        }




        /// <summary>
        ///   Converts a long integer array to a boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this long[] value, bool[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this long[,] value, bool[,] result)
        {
            unsafe
            {
                fixed (long* src = value)
                fixed (bool* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] != 0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this long[,] value, bool[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this long[][] value, bool[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this long[][][] value, bool[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this long[][] value, bool[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] != 0;
            return result;
        }
        /// <summary>
        ///   Converts a long integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this long[] value)
        {
            return ToObject(value, new object[value.Length]);
        }

        /// <summary>
        ///   Converts a long integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this long[,] value)
        {
            return ToObject(value, Matrix.CreateAs<long, object>(value));
        }

        /// <summary>
        ///   Converts a long integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this long[][] value)
        {
            return ToObject(value, Jagged.CreateAs<long, object>(value));
        }

        /// <summary>
        ///   Converts a long integer to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this long[][][] value)
        {
            return ToObject(value, Jagged.CreateAs<long, object>(value));
        }




        /// <summary>
        ///   Converts a long integer array to a object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this long[] value, object[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Object)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this long[,] value, object[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Object)value[i, j];

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this long[,] value, object[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Object)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this long[][] value, object[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Object)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this long[][][] value, object[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Object)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this long[][] value, object[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Object)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a long integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this long[] value)
        {
            return ToString(value, new string[value.Length]);
        }

        /// <summary>
        ///   Converts a long integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this long[,] value)
        {
            return ToString(value, Matrix.CreateAs<long, string>(value));
        }

        /// <summary>
        ///   Converts a long integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this long[][] value)
        {
            return ToString(value, Jagged.CreateAs<long, string>(value));
        }

        /// <summary>
        ///   Converts a long integer to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this long[][][] value)
        {
            return ToString(value, Jagged.CreateAs<long, string>(value));
        }




        /// <summary>
        ///   Converts a long integer array to a string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this long[] value, string[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this long[,] value, string[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = value[i, j].ToString(); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional long integer array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this long[,] value, string[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this long[][] value, string[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this long[][][] value, string[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged long integer array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this long[][] value, string[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j].ToString(); ;
            return result;
        }
        /// <summary>
        ///   Converts a 8-bit byte to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this byte[] value)
        {
            return ToInt32(value, new int[value.Length]);
        }

        /// <summary>
        ///   Converts a 8-bit byte to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this byte[,] value)
        {
            return ToInt32(value, Matrix.CreateAs<byte, int>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this byte[][] value)
        {
            return ToInt32(value, Jagged.CreateAs<byte, int>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this byte[][][] value)
        {
            return ToInt32(value, Jagged.CreateAs<byte, int>(value));
        }




        /// <summary>
        ///   Converts a 8-bit byte array to a integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this byte[] value, int[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int32)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this byte[,] value, int[,] result)
        {
            unsafe
            {
                fixed (byte* src = value)
                fixed (int* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int32)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this byte[,] value, int[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int32)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this byte[][] value, int[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int32)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this byte[][][] value, int[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int32)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this byte[][] value, int[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int32)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a 8-bit byte to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this byte[] value)
        {
            return ToInt16(value, new short[value.Length]);
        }

        /// <summary>
        ///   Converts a 8-bit byte to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this byte[,] value)
        {
            return ToInt16(value, Matrix.CreateAs<byte, short>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this byte[][] value)
        {
            return ToInt16(value, Jagged.CreateAs<byte, short>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this byte[][][] value)
        {
            return ToInt16(value, Jagged.CreateAs<byte, short>(value));
        }




        /// <summary>
        ///   Converts a 8-bit byte array to a short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this byte[] value, short[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int16)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this byte[,] value, short[,] result)
        {
            unsafe
            {
                fixed (byte* src = value)
                fixed (short* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int16)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this byte[,] value, short[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int16)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this byte[][] value, short[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int16)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this byte[][][] value, short[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int16)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this byte[][] value, short[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int16)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a 8-bit byte to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this byte[] value)
        {
            return ToSingle(value, new float[value.Length]);
        }

        /// <summary>
        ///   Converts a 8-bit byte to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this byte[,] value)
        {
            return ToSingle(value, Matrix.CreateAs<byte, float>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this byte[][] value)
        {
            return ToSingle(value, Jagged.CreateAs<byte, float>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this byte[][][] value)
        {
            return ToSingle(value, Jagged.CreateAs<byte, float>(value));
        }




        /// <summary>
        ///   Converts a 8-bit byte array to a single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this byte[] value, float[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Single)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this byte[,] value, float[,] result)
        {
            unsafe
            {
                fixed (byte* src = value)
                fixed (float* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Single)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this byte[,] value, float[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Single)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this byte[][] value, float[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Single)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this byte[][][] value, float[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Single)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this byte[][] value, float[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Single)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a 8-bit byte to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this byte[] value)
        {
            return ToDouble(value, new double[value.Length]);
        }

        /// <summary>
        ///   Converts a 8-bit byte to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this byte[,] value)
        {
            return ToDouble(value, Matrix.CreateAs<byte, double>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this byte[][] value)
        {
            return ToDouble(value, Jagged.CreateAs<byte, double>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this byte[][][] value)
        {
            return ToDouble(value, Jagged.CreateAs<byte, double>(value));
        }




        /// <summary>
        ///   Converts a 8-bit byte array to a double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this byte[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Double)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this byte[,] value, double[,] result)
        {
            unsafe
            {
                fixed (byte* src = value)
                fixed (double* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Double)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this byte[,] value, double[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Double)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this byte[][] value, double[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Double)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this byte[][][] value, double[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Double)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this byte[][] value, double[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Double)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a 8-bit byte to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this byte[] value)
        {
            return ToInt64(value, new long[value.Length]);
        }

        /// <summary>
        ///   Converts a 8-bit byte to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this byte[,] value)
        {
            return ToInt64(value, Matrix.CreateAs<byte, long>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this byte[][] value)
        {
            return ToInt64(value, Jagged.CreateAs<byte, long>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this byte[][][] value)
        {
            return ToInt64(value, Jagged.CreateAs<byte, long>(value));
        }




        /// <summary>
        ///   Converts a 8-bit byte array to a long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this byte[] value, long[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int64)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this byte[,] value, long[,] result)
        {
            unsafe
            {
                fixed (byte* src = value)
                fixed (long* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int64)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this byte[,] value, long[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int64)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this byte[][] value, long[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int64)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this byte[][][] value, long[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int64)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this byte[][] value, long[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int64)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a 8-bit byte to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this byte[] value)
        {
            return ToSByte(value, new sbyte[value.Length]);
        }

        /// <summary>
        ///   Converts a 8-bit byte to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this byte[,] value)
        {
            return ToSByte(value, Matrix.CreateAs<byte, sbyte>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this byte[][] value)
        {
            return ToSByte(value, Jagged.CreateAs<byte, sbyte>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this byte[][][] value)
        {
            return ToSByte(value, Jagged.CreateAs<byte, sbyte>(value));
        }




        /// <summary>
        ///   Converts a 8-bit byte array to a signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this byte[] value, sbyte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (SByte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this byte[,] value, sbyte[,] result)
        {
            unsafe
            {
                fixed (byte* src = value)
                fixed (sbyte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (SByte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this byte[,] value, sbyte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (SByte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this byte[][] value, sbyte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (SByte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this byte[][][] value, sbyte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (SByte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this byte[][] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (SByte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a 8-bit byte to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this byte[] value)
        {
            return ToDecimal(value, new decimal[value.Length]);
        }

        /// <summary>
        ///   Converts a 8-bit byte to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this byte[,] value)
        {
            return ToDecimal(value, Matrix.CreateAs<byte, decimal>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this byte[][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<byte, decimal>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this byte[][][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<byte, decimal>(value));
        }




        /// <summary>
        ///   Converts a 8-bit byte array to a decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this byte[] value, decimal[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Decimal)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this byte[,] value, decimal[,] result)
        {
            unsafe
            {
                fixed (byte* src = value)
                fixed (decimal* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Decimal)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this byte[,] value, decimal[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Decimal)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this byte[][] value, decimal[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Decimal)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this byte[][][] value, decimal[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Decimal)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this byte[][] value, decimal[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Decimal)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a 8-bit byte to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this byte[] value)
        {
            return ToBoolean(value, new bool[value.Length]);
        }

        /// <summary>
        ///   Converts a 8-bit byte to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this byte[,] value)
        {
            return ToBoolean(value, Matrix.CreateAs<byte, bool>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this byte[][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<byte, bool>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this byte[][][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<byte, bool>(value));
        }




        /// <summary>
        ///   Converts a 8-bit byte array to a boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this byte[] value, bool[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this byte[,] value, bool[,] result)
        {
            unsafe
            {
                fixed (byte* src = value)
                fixed (bool* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] != 0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this byte[,] value, bool[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this byte[][] value, bool[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this byte[][][] value, bool[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this byte[][] value, bool[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] != 0;
            return result;
        }
        /// <summary>
        ///   Converts a 8-bit byte to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this byte[] value)
        {
            return ToObject(value, new object[value.Length]);
        }

        /// <summary>
        ///   Converts a 8-bit byte to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this byte[,] value)
        {
            return ToObject(value, Matrix.CreateAs<byte, object>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this byte[][] value)
        {
            return ToObject(value, Jagged.CreateAs<byte, object>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this byte[][][] value)
        {
            return ToObject(value, Jagged.CreateAs<byte, object>(value));
        }




        /// <summary>
        ///   Converts a 8-bit byte array to a object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this byte[] value, object[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Object)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this byte[,] value, object[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Object)value[i, j];

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this byte[,] value, object[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Object)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this byte[][] value, object[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Object)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this byte[][][] value, object[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Object)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this byte[][] value, object[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Object)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a 8-bit byte to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this byte[] value)
        {
            return ToString(value, new string[value.Length]);
        }

        /// <summary>
        ///   Converts a 8-bit byte to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this byte[,] value)
        {
            return ToString(value, Matrix.CreateAs<byte, string>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this byte[][] value)
        {
            return ToString(value, Jagged.CreateAs<byte, string>(value));
        }

        /// <summary>
        ///   Converts a 8-bit byte to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this byte[][][] value)
        {
            return ToString(value, Jagged.CreateAs<byte, string>(value));
        }




        /// <summary>
        ///   Converts a 8-bit byte array to a string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this byte[] value, string[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this byte[,] value, string[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = value[i, j].ToString(); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional 8-bit byte array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this byte[,] value, string[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this byte[][] value, string[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this byte[][][] value, string[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged 8-bit byte array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this byte[][] value, string[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j].ToString(); ;
            return result;
        }
        /// <summary>
        ///   Converts a signed 7-bit byte to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this sbyte[] value)
        {
            return ToInt32(value, new int[value.Length]);
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this sbyte[,] value)
        {
            return ToInt32(value, Matrix.CreateAs<sbyte, int>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this sbyte[][] value)
        {
            return ToInt32(value, Jagged.CreateAs<sbyte, int>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this sbyte[][][] value)
        {
            return ToInt32(value, Jagged.CreateAs<sbyte, int>(value));
        }




        /// <summary>
        ///   Converts a signed 7-bit byte array to a integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this sbyte[] value, int[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int32)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this sbyte[,] value, int[,] result)
        {
            unsafe
            {
                fixed (sbyte* src = value)
                fixed (int* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int32)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this sbyte[,] value, int[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int32)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this sbyte[][] value, int[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int32)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this sbyte[][][] value, int[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int32)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this sbyte[][] value, int[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int32)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a signed 7-bit byte to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this sbyte[] value)
        {
            return ToInt16(value, new short[value.Length]);
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this sbyte[,] value)
        {
            return ToInt16(value, Matrix.CreateAs<sbyte, short>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this sbyte[][] value)
        {
            return ToInt16(value, Jagged.CreateAs<sbyte, short>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this sbyte[][][] value)
        {
            return ToInt16(value, Jagged.CreateAs<sbyte, short>(value));
        }




        /// <summary>
        ///   Converts a signed 7-bit byte array to a short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this sbyte[] value, short[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int16)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this sbyte[,] value, short[,] result)
        {
            unsafe
            {
                fixed (sbyte* src = value)
                fixed (short* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int16)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this sbyte[,] value, short[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int16)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this sbyte[][] value, short[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int16)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this sbyte[][][] value, short[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int16)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this sbyte[][] value, short[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int16)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a signed 7-bit byte to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this sbyte[] value)
        {
            return ToSingle(value, new float[value.Length]);
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this sbyte[,] value)
        {
            return ToSingle(value, Matrix.CreateAs<sbyte, float>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this sbyte[][] value)
        {
            return ToSingle(value, Jagged.CreateAs<sbyte, float>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this sbyte[][][] value)
        {
            return ToSingle(value, Jagged.CreateAs<sbyte, float>(value));
        }




        /// <summary>
        ///   Converts a signed 7-bit byte array to a single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this sbyte[] value, float[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Single)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this sbyte[,] value, float[,] result)
        {
            unsafe
            {
                fixed (sbyte* src = value)
                fixed (float* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Single)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this sbyte[,] value, float[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Single)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this sbyte[][] value, float[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Single)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this sbyte[][][] value, float[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Single)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this sbyte[][] value, float[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Single)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a signed 7-bit byte to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this sbyte[] value)
        {
            return ToDouble(value, new double[value.Length]);
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this sbyte[,] value)
        {
            return ToDouble(value, Matrix.CreateAs<sbyte, double>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this sbyte[][] value)
        {
            return ToDouble(value, Jagged.CreateAs<sbyte, double>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this sbyte[][][] value)
        {
            return ToDouble(value, Jagged.CreateAs<sbyte, double>(value));
        }




        /// <summary>
        ///   Converts a signed 7-bit byte array to a double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this sbyte[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Double)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this sbyte[,] value, double[,] result)
        {
            unsafe
            {
                fixed (sbyte* src = value)
                fixed (double* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Double)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this sbyte[,] value, double[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Double)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this sbyte[][] value, double[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Double)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this sbyte[][][] value, double[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Double)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this sbyte[][] value, double[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Double)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a signed 7-bit byte to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this sbyte[] value)
        {
            return ToInt64(value, new long[value.Length]);
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this sbyte[,] value)
        {
            return ToInt64(value, Matrix.CreateAs<sbyte, long>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this sbyte[][] value)
        {
            return ToInt64(value, Jagged.CreateAs<sbyte, long>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this sbyte[][][] value)
        {
            return ToInt64(value, Jagged.CreateAs<sbyte, long>(value));
        }




        /// <summary>
        ///   Converts a signed 7-bit byte array to a long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this sbyte[] value, long[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int64)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this sbyte[,] value, long[,] result)
        {
            unsafe
            {
                fixed (sbyte* src = value)
                fixed (long* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int64)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this sbyte[,] value, long[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int64)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this sbyte[][] value, long[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int64)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this sbyte[][][] value, long[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int64)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this sbyte[][] value, long[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int64)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a signed 7-bit byte to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this sbyte[] value)
        {
            return ToByte(value, new byte[value.Length]);
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this sbyte[,] value)
        {
            return ToByte(value, Matrix.CreateAs<sbyte, byte>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this sbyte[][] value)
        {
            return ToByte(value, Jagged.CreateAs<sbyte, byte>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this sbyte[][][] value)
        {
            return ToByte(value, Jagged.CreateAs<sbyte, byte>(value));
        }




        /// <summary>
        ///   Converts a signed 7-bit byte array to a 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this sbyte[] value, byte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Byte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this sbyte[,] value, byte[,] result)
        {
            unsafe
            {
                fixed (sbyte* src = value)
                fixed (byte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Byte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this sbyte[,] value, byte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Byte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this sbyte[][] value, byte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Byte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this sbyte[][][] value, byte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Byte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this sbyte[][] value, byte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Byte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a signed 7-bit byte to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this sbyte[] value)
        {
            return ToDecimal(value, new decimal[value.Length]);
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this sbyte[,] value)
        {
            return ToDecimal(value, Matrix.CreateAs<sbyte, decimal>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this sbyte[][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<sbyte, decimal>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this sbyte[][][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<sbyte, decimal>(value));
        }




        /// <summary>
        ///   Converts a signed 7-bit byte array to a decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this sbyte[] value, decimal[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Decimal)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this sbyte[,] value, decimal[,] result)
        {
            unsafe
            {
                fixed (sbyte* src = value)
                fixed (decimal* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Decimal)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this sbyte[,] value, decimal[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Decimal)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this sbyte[][] value, decimal[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Decimal)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this sbyte[][][] value, decimal[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Decimal)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this sbyte[][] value, decimal[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Decimal)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a signed 7-bit byte to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this sbyte[] value)
        {
            return ToBoolean(value, new bool[value.Length]);
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this sbyte[,] value)
        {
            return ToBoolean(value, Matrix.CreateAs<sbyte, bool>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this sbyte[][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<sbyte, bool>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this sbyte[][][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<sbyte, bool>(value));
        }




        /// <summary>
        ///   Converts a signed 7-bit byte array to a boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this sbyte[] value, bool[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this sbyte[,] value, bool[,] result)
        {
            unsafe
            {
                fixed (sbyte* src = value)
                fixed (bool* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] != 0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this sbyte[,] value, bool[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this sbyte[][] value, bool[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this sbyte[][][] value, bool[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this sbyte[][] value, bool[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] != 0;
            return result;
        }
        /// <summary>
        ///   Converts a signed 7-bit byte to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this sbyte[] value)
        {
            return ToObject(value, new object[value.Length]);
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this sbyte[,] value)
        {
            return ToObject(value, Matrix.CreateAs<sbyte, object>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this sbyte[][] value)
        {
            return ToObject(value, Jagged.CreateAs<sbyte, object>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this sbyte[][][] value)
        {
            return ToObject(value, Jagged.CreateAs<sbyte, object>(value));
        }




        /// <summary>
        ///   Converts a signed 7-bit byte array to a object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this sbyte[] value, object[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Object)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this sbyte[,] value, object[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Object)value[i, j];

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this sbyte[,] value, object[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Object)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this sbyte[][] value, object[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Object)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this sbyte[][][] value, object[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Object)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this sbyte[][] value, object[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Object)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a signed 7-bit byte to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this sbyte[] value)
        {
            return ToString(value, new string[value.Length]);
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this sbyte[,] value)
        {
            return ToString(value, Matrix.CreateAs<sbyte, string>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this sbyte[][] value)
        {
            return ToString(value, Jagged.CreateAs<sbyte, string>(value));
        }

        /// <summary>
        ///   Converts a signed 7-bit byte to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this sbyte[][][] value)
        {
            return ToString(value, Jagged.CreateAs<sbyte, string>(value));
        }




        /// <summary>
        ///   Converts a signed 7-bit byte array to a string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this sbyte[] value, string[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this sbyte[,] value, string[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = value[i, j].ToString(); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional signed 7-bit byte array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this sbyte[,] value, string[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this sbyte[][] value, string[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this sbyte[][][] value, string[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged signed 7-bit byte array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this sbyte[][] value, string[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j].ToString(); ;
            return result;
        }
        /// <summary>
        ///   Converts a decimal fixed-point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this decimal[] value)
        {
            return ToInt32(value, new int[value.Length]);
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this decimal[,] value)
        {
            return ToInt32(value, Matrix.CreateAs<decimal, int>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this decimal[][] value)
        {
            return ToInt32(value, Jagged.CreateAs<decimal, int>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this decimal[][][] value)
        {
            return ToInt32(value, Jagged.CreateAs<decimal, int>(value));
        }




        /// <summary>
        ///   Converts a decimal fixed-point array to a integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this decimal[] value, int[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int32)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this decimal[,] value, int[,] result)
        {
            unsafe
            {
                fixed (decimal* src = value)
                fixed (int* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int32)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this decimal[,] value, int[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int32)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this decimal[][] value, int[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int32)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this decimal[][][] value, int[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int32)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this decimal[][] value, int[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int32)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a decimal fixed-point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this decimal[] value)
        {
            return ToInt16(value, new short[value.Length]);
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this decimal[,] value)
        {
            return ToInt16(value, Matrix.CreateAs<decimal, short>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this decimal[][] value)
        {
            return ToInt16(value, Jagged.CreateAs<decimal, short>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this decimal[][][] value)
        {
            return ToInt16(value, Jagged.CreateAs<decimal, short>(value));
        }




        /// <summary>
        ///   Converts a decimal fixed-point array to a short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this decimal[] value, short[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int16)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this decimal[,] value, short[,] result)
        {
            unsafe
            {
                fixed (decimal* src = value)
                fixed (short* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int16)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this decimal[,] value, short[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int16)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this decimal[][] value, short[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int16)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this decimal[][][] value, short[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int16)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this decimal[][] value, short[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int16)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a decimal fixed-point to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this decimal[] value)
        {
            return ToSingle(value, new float[value.Length]);
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this decimal[,] value)
        {
            return ToSingle(value, Matrix.CreateAs<decimal, float>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this decimal[][] value)
        {
            return ToSingle(value, Jagged.CreateAs<decimal, float>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this decimal[][][] value)
        {
            return ToSingle(value, Jagged.CreateAs<decimal, float>(value));
        }




        /// <summary>
        ///   Converts a decimal fixed-point array to a single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this decimal[] value, float[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Single)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this decimal[,] value, float[,] result)
        {
            unsafe
            {
                fixed (decimal* src = value)
                fixed (float* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Single)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this decimal[,] value, float[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Single)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this decimal[][] value, float[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Single)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this decimal[][][] value, float[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Single)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this decimal[][] value, float[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Single)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a decimal fixed-point to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this decimal[] value)
        {
            return ToDouble(value, new double[value.Length]);
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this decimal[,] value)
        {
            return ToDouble(value, Matrix.CreateAs<decimal, double>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this decimal[][] value)
        {
            return ToDouble(value, Jagged.CreateAs<decimal, double>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this decimal[][][] value)
        {
            return ToDouble(value, Jagged.CreateAs<decimal, double>(value));
        }




        /// <summary>
        ///   Converts a decimal fixed-point array to a double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this decimal[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Double)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this decimal[,] value, double[,] result)
        {
            unsafe
            {
                fixed (decimal* src = value)
                fixed (double* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Double)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this decimal[,] value, double[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Double)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this decimal[][] value, double[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Double)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this decimal[][][] value, double[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Double)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this decimal[][] value, double[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Double)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a decimal fixed-point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this decimal[] value)
        {
            return ToInt64(value, new long[value.Length]);
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this decimal[,] value)
        {
            return ToInt64(value, Matrix.CreateAs<decimal, long>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this decimal[][] value)
        {
            return ToInt64(value, Jagged.CreateAs<decimal, long>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this decimal[][][] value)
        {
            return ToInt64(value, Jagged.CreateAs<decimal, long>(value));
        }




        /// <summary>
        ///   Converts a decimal fixed-point array to a long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this decimal[] value, long[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int64)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this decimal[,] value, long[,] result)
        {
            unsafe
            {
                fixed (decimal* src = value)
                fixed (long* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Int64)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this decimal[,] value, long[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int64)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this decimal[][] value, long[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int64)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this decimal[][][] value, long[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int64)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this decimal[][] value, long[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int64)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a decimal fixed-point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this decimal[] value)
        {
            return ToByte(value, new byte[value.Length]);
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this decimal[,] value)
        {
            return ToByte(value, Matrix.CreateAs<decimal, byte>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this decimal[][] value)
        {
            return ToByte(value, Jagged.CreateAs<decimal, byte>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this decimal[][][] value)
        {
            return ToByte(value, Jagged.CreateAs<decimal, byte>(value));
        }




        /// <summary>
        ///   Converts a decimal fixed-point array to a 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this decimal[] value, byte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Byte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this decimal[,] value, byte[,] result)
        {
            unsafe
            {
                fixed (decimal* src = value)
                fixed (byte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (Byte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this decimal[,] value, byte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Byte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this decimal[][] value, byte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Byte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this decimal[][][] value, byte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Byte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this decimal[][] value, byte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Byte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a decimal fixed-point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this decimal[] value)
        {
            return ToSByte(value, new sbyte[value.Length]);
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this decimal[,] value)
        {
            return ToSByte(value, Matrix.CreateAs<decimal, sbyte>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this decimal[][] value)
        {
            return ToSByte(value, Jagged.CreateAs<decimal, sbyte>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this decimal[][][] value)
        {
            return ToSByte(value, Jagged.CreateAs<decimal, sbyte>(value));
        }




        /// <summary>
        ///   Converts a decimal fixed-point array to a signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this decimal[] value, sbyte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (SByte)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this decimal[,] value, sbyte[,] result)
        {
            unsafe
            {
                fixed (decimal* src = value)
                fixed (sbyte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = (SByte)src[i];
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this decimal[,] value, sbyte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (SByte)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this decimal[][] value, sbyte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (SByte)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this decimal[][][] value, sbyte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (SByte)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this decimal[][] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (SByte)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a decimal fixed-point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this decimal[] value)
        {
            return ToBoolean(value, new bool[value.Length]);
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this decimal[,] value)
        {
            return ToBoolean(value, Matrix.CreateAs<decimal, bool>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this decimal[][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<decimal, bool>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this decimal[][][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<decimal, bool>(value));
        }




        /// <summary>
        ///   Converts a decimal fixed-point array to a boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this decimal[] value, bool[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this decimal[,] value, bool[,] result)
        {
            unsafe
            {
                fixed (decimal* src = value)
                fixed (bool* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] != 0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this decimal[,] value, bool[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this decimal[][] value, bool[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this decimal[][][] value, bool[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] != 0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this decimal[][] value, bool[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] != 0;
            return result;
        }
        /// <summary>
        ///   Converts a decimal fixed-point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this decimal[] value)
        {
            return ToObject(value, new object[value.Length]);
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this decimal[,] value)
        {
            return ToObject(value, Matrix.CreateAs<decimal, object>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this decimal[][] value)
        {
            return ToObject(value, Jagged.CreateAs<decimal, object>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this decimal[][][] value)
        {
            return ToObject(value, Jagged.CreateAs<decimal, object>(value));
        }




        /// <summary>
        ///   Converts a decimal fixed-point array to a object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this decimal[] value, object[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Object)value[i];
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this decimal[,] value, object[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Object)value[i, j];

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this decimal[,] value, object[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Object)value[i, j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this decimal[][] value, object[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Object)value[i][j];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this decimal[][][] value, object[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Object)value[i][j][k];
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this decimal[][] value, object[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Object)value[i][j];
            return result;
        }
        /// <summary>
        ///   Converts a decimal fixed-point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this decimal[] value)
        {
            return ToString(value, new string[value.Length]);
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this decimal[,] value)
        {
            return ToString(value, Matrix.CreateAs<decimal, string>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this decimal[][] value)
        {
            return ToString(value, Jagged.CreateAs<decimal, string>(value));
        }

        /// <summary>
        ///   Converts a decimal fixed-point to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this decimal[][][] value)
        {
            return ToString(value, Jagged.CreateAs<decimal, string>(value));
        }




        /// <summary>
        ///   Converts a decimal fixed-point array to a string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this decimal[] value, string[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this decimal[,] value, string[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = value[i, j].ToString(); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional decimal fixed-point array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this decimal[,] value, string[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this decimal[][] value, string[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this decimal[][][] value, string[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged decimal fixed-point array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this decimal[][] value, string[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j].ToString(); ;
            return result;
        }
        /// <summary>
        ///   Converts a boolean to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this bool[] value)
        {
            return ToInt32(value, new int[value.Length]);
        }

        /// <summary>
        ///   Converts a boolean to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this bool[,] value)
        {
            return ToInt32(value, Matrix.CreateAs<bool, int>(value));
        }

        /// <summary>
        ///   Converts a boolean to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this bool[][] value)
        {
            return ToInt32(value, Jagged.CreateAs<bool, int>(value));
        }

        /// <summary>
        ///   Converts a boolean to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this bool[][][] value)
        {
            return ToInt32(value, Jagged.CreateAs<bool, int>(value));
        }




        /// <summary>
        ///   Converts a boolean array to a integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this bool[] value, int[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] ? (Int32)1 : (Int32)0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this bool[,] value, int[,] result)
        {
            unsafe
            {
                fixed (bool* src = value)
                fixed (int* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] ? (Int32)1 : (Int32)0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this bool[,] value, int[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] ? (Int32)1 : (Int32)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this bool[][] value, int[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] ? (Int32)1 : (Int32)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this bool[][][] value, int[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] ? (Int32)1 : (Int32)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this bool[][] value, int[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] ? (Int32)1 : (Int32)0;
            return result;
        }
        /// <summary>
        ///   Converts a boolean to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this bool[] value)
        {
            return ToInt16(value, new short[value.Length]);
        }

        /// <summary>
        ///   Converts a boolean to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this bool[,] value)
        {
            return ToInt16(value, Matrix.CreateAs<bool, short>(value));
        }

        /// <summary>
        ///   Converts a boolean to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this bool[][] value)
        {
            return ToInt16(value, Jagged.CreateAs<bool, short>(value));
        }

        /// <summary>
        ///   Converts a boolean to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this bool[][][] value)
        {
            return ToInt16(value, Jagged.CreateAs<bool, short>(value));
        }




        /// <summary>
        ///   Converts a boolean array to a short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this bool[] value, short[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] ? (Int16)1 : (Int16)0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this bool[,] value, short[,] result)
        {
            unsafe
            {
                fixed (bool* src = value)
                fixed (short* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] ? (Int16)1 : (Int16)0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this bool[,] value, short[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] ? (Int16)1 : (Int16)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this bool[][] value, short[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] ? (Int16)1 : (Int16)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this bool[][][] value, short[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] ? (Int16)1 : (Int16)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this bool[][] value, short[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] ? (Int16)1 : (Int16)0;
            return result;
        }
        /// <summary>
        ///   Converts a boolean to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this bool[] value)
        {
            return ToSingle(value, new float[value.Length]);
        }

        /// <summary>
        ///   Converts a boolean to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this bool[,] value)
        {
            return ToSingle(value, Matrix.CreateAs<bool, float>(value));
        }

        /// <summary>
        ///   Converts a boolean to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this bool[][] value)
        {
            return ToSingle(value, Jagged.CreateAs<bool, float>(value));
        }

        /// <summary>
        ///   Converts a boolean to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this bool[][][] value)
        {
            return ToSingle(value, Jagged.CreateAs<bool, float>(value));
        }




        /// <summary>
        ///   Converts a boolean array to a single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this bool[] value, float[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] ? (Single)1 : (Single)0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this bool[,] value, float[,] result)
        {
            unsafe
            {
                fixed (bool* src = value)
                fixed (float* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] ? (Single)1 : (Single)0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this bool[,] value, float[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] ? (Single)1 : (Single)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this bool[][] value, float[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] ? (Single)1 : (Single)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this bool[][][] value, float[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] ? (Single)1 : (Single)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this bool[][] value, float[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] ? (Single)1 : (Single)0;
            return result;
        }
        /// <summary>
        ///   Converts a boolean to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this bool[] value)
        {
            return ToDouble(value, new double[value.Length]);
        }

        /// <summary>
        ///   Converts a boolean to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this bool[,] value)
        {
            return ToDouble(value, Matrix.CreateAs<bool, double>(value));
        }

        /// <summary>
        ///   Converts a boolean to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this bool[][] value)
        {
            return ToDouble(value, Jagged.CreateAs<bool, double>(value));
        }

        /// <summary>
        ///   Converts a boolean to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this bool[][][] value)
        {
            return ToDouble(value, Jagged.CreateAs<bool, double>(value));
        }




        /// <summary>
        ///   Converts a boolean array to a double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this bool[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] ? (Double)1 : (Double)0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this bool[,] value, double[,] result)
        {
            unsafe
            {
                fixed (bool* src = value)
                fixed (double* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] ? (Double)1 : (Double)0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this bool[,] value, double[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] ? (Double)1 : (Double)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this bool[][] value, double[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] ? (Double)1 : (Double)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this bool[][][] value, double[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] ? (Double)1 : (Double)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this bool[][] value, double[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] ? (Double)1 : (Double)0;
            return result;
        }
        /// <summary>
        ///   Converts a boolean to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this bool[] value)
        {
            return ToInt64(value, new long[value.Length]);
        }

        /// <summary>
        ///   Converts a boolean to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this bool[,] value)
        {
            return ToInt64(value, Matrix.CreateAs<bool, long>(value));
        }

        /// <summary>
        ///   Converts a boolean to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this bool[][] value)
        {
            return ToInt64(value, Jagged.CreateAs<bool, long>(value));
        }

        /// <summary>
        ///   Converts a boolean to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this bool[][][] value)
        {
            return ToInt64(value, Jagged.CreateAs<bool, long>(value));
        }




        /// <summary>
        ///   Converts a boolean array to a long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this bool[] value, long[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] ? (Int64)1 : (Int64)0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this bool[,] value, long[,] result)
        {
            unsafe
            {
                fixed (bool* src = value)
                fixed (long* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] ? (Int64)1 : (Int64)0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this bool[,] value, long[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] ? (Int64)1 : (Int64)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this bool[][] value, long[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] ? (Int64)1 : (Int64)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this bool[][][] value, long[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] ? (Int64)1 : (Int64)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this bool[][] value, long[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] ? (Int64)1 : (Int64)0;
            return result;
        }
        /// <summary>
        ///   Converts a boolean to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this bool[] value)
        {
            return ToByte(value, new byte[value.Length]);
        }

        /// <summary>
        ///   Converts a boolean to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this bool[,] value)
        {
            return ToByte(value, Matrix.CreateAs<bool, byte>(value));
        }

        /// <summary>
        ///   Converts a boolean to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this bool[][] value)
        {
            return ToByte(value, Jagged.CreateAs<bool, byte>(value));
        }

        /// <summary>
        ///   Converts a boolean to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this bool[][][] value)
        {
            return ToByte(value, Jagged.CreateAs<bool, byte>(value));
        }




        /// <summary>
        ///   Converts a boolean array to a 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this bool[] value, byte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] ? (Byte)1 : (Byte)0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this bool[,] value, byte[,] result)
        {
            unsafe
            {
                fixed (bool* src = value)
                fixed (byte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] ? (Byte)1 : (Byte)0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this bool[,] value, byte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] ? (Byte)1 : (Byte)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this bool[][] value, byte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] ? (Byte)1 : (Byte)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this bool[][][] value, byte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] ? (Byte)1 : (Byte)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this bool[][] value, byte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] ? (Byte)1 : (Byte)0;
            return result;
        }
        /// <summary>
        ///   Converts a boolean to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this bool[] value)
        {
            return ToSByte(value, new sbyte[value.Length]);
        }

        /// <summary>
        ///   Converts a boolean to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this bool[,] value)
        {
            return ToSByte(value, Matrix.CreateAs<bool, sbyte>(value));
        }

        /// <summary>
        ///   Converts a boolean to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this bool[][] value)
        {
            return ToSByte(value, Jagged.CreateAs<bool, sbyte>(value));
        }

        /// <summary>
        ///   Converts a boolean to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this bool[][][] value)
        {
            return ToSByte(value, Jagged.CreateAs<bool, sbyte>(value));
        }




        /// <summary>
        ///   Converts a boolean array to a signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this bool[] value, sbyte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] ? (SByte)1 : (SByte)0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this bool[,] value, sbyte[,] result)
        {
            unsafe
            {
                fixed (bool* src = value)
                fixed (sbyte* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] ? (SByte)1 : (SByte)0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this bool[,] value, sbyte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] ? (SByte)1 : (SByte)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this bool[][] value, sbyte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] ? (SByte)1 : (SByte)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this bool[][][] value, sbyte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] ? (SByte)1 : (SByte)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this bool[][] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] ? (SByte)1 : (SByte)0;
            return result;
        }
        /// <summary>
        ///   Converts a boolean to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this bool[] value)
        {
            return ToDecimal(value, new decimal[value.Length]);
        }

        /// <summary>
        ///   Converts a boolean to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this bool[,] value)
        {
            return ToDecimal(value, Matrix.CreateAs<bool, decimal>(value));
        }

        /// <summary>
        ///   Converts a boolean to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this bool[][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<bool, decimal>(value));
        }

        /// <summary>
        ///   Converts a boolean to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this bool[][][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<bool, decimal>(value));
        }




        /// <summary>
        ///   Converts a boolean array to a decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this bool[] value, decimal[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] ? (Decimal)1 : (Decimal)0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this bool[,] value, decimal[,] result)
        {
            unsafe
            {
                fixed (bool* src = value)
                fixed (decimal* dst = result)
                {
                    for (int i = 0; i < value.Length; i++)
                        dst[i] = src[i] ? (Decimal)1 : (Decimal)0;
                }
            }

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this bool[,] value, decimal[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] ? (Decimal)1 : (Decimal)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this bool[][] value, decimal[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] ? (Decimal)1 : (Decimal)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this bool[][][] value, decimal[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] ? (Decimal)1 : (Decimal)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this bool[][] value, decimal[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] ? (Decimal)1 : (Decimal)0;
            return result;
        }
        /// <summary>
        ///   Converts a boolean to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this bool[] value)
        {
            return ToObject(value, new object[value.Length]);
        }

        /// <summary>
        ///   Converts a boolean to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this bool[,] value)
        {
            return ToObject(value, Matrix.CreateAs<bool, object>(value));
        }

        /// <summary>
        ///   Converts a boolean to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this bool[][] value)
        {
            return ToObject(value, Jagged.CreateAs<bool, object>(value));
        }

        /// <summary>
        ///   Converts a boolean to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this bool[][][] value)
        {
            return ToObject(value, Jagged.CreateAs<bool, object>(value));
        }




        /// <summary>
        ///   Converts a boolean array to a object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this bool[] value, object[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i] ? (Object)1 : (Object)0;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this bool[,] value, object[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = value[i, j] ? (Object)1 : (Object)0;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this bool[,] value, object[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j] ? (Object)1 : (Object)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this bool[][] value, object[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j] ? (Object)1 : (Object)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this bool[][][] value, object[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k] ? (Object)1 : (Object)0;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this bool[][] value, object[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j] ? (Object)1 : (Object)0;
            return result;
        }
        /// <summary>
        ///   Converts a boolean to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this bool[] value)
        {
            return ToString(value, new string[value.Length]);
        }

        /// <summary>
        ///   Converts a boolean to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this bool[,] value)
        {
            return ToString(value, Matrix.CreateAs<bool, string>(value));
        }

        /// <summary>
        ///   Converts a boolean to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this bool[][] value)
        {
            return ToString(value, Jagged.CreateAs<bool, string>(value));
        }

        /// <summary>
        ///   Converts a boolean to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this bool[][][] value)
        {
            return ToString(value, Jagged.CreateAs<bool, string>(value));
        }




        /// <summary>
        ///   Converts a boolean array to a string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this bool[] value, string[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = value[i].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this bool[,] value, string[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = value[i, j].ToString(); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional boolean array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this bool[,] value, string[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = value[i, j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this bool[][] value, string[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = value[i][j].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this bool[][][] value, string[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = value[i][j][k].ToString(); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged boolean array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this bool[][] value, string[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = value[i][j].ToString(); ;
            return result;
        }
        /// <summary>
        ///   Converts a object to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this object[] value)
        {
            return ToInt32(value, new int[value.Length]);
        }

        /// <summary>
        ///   Converts a object to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this object[,] value)
        {
            return ToInt32(value, Matrix.CreateAs<object, int>(value));
        }

        /// <summary>
        ///   Converts a object to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this object[][] value)
        {
            return ToInt32(value, Jagged.CreateAs<object, int>(value));
        }

        /// <summary>
        ///   Converts a object to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this object[][][] value)
        {
            return ToInt32(value, Jagged.CreateAs<object, int>(value));
        }




        /// <summary>
        ///   Converts a object array to a integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this object[] value, int[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int32)System.Convert.ChangeType(value[i], typeof(Int32));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this object[,] value, int[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Int32)System.Convert.ChangeType(value[i, j], typeof(Int32));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this object[,] value, int[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int32)System.Convert.ChangeType(value[i, j], typeof(Int32));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this object[][] value, int[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int32)System.Convert.ChangeType(value[i][j], typeof(Int32));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this object[][][] value, int[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int32)System.Convert.ChangeType(value[i][j][k], typeof(Int32));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this object[][] value, int[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int32)System.Convert.ChangeType(value[i][j], typeof(Int32));
            return result;
        }
        /// <summary>
        ///   Converts a object to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this object[] value)
        {
            return ToInt16(value, new short[value.Length]);
        }

        /// <summary>
        ///   Converts a object to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this object[,] value)
        {
            return ToInt16(value, Matrix.CreateAs<object, short>(value));
        }

        /// <summary>
        ///   Converts a object to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this object[][] value)
        {
            return ToInt16(value, Jagged.CreateAs<object, short>(value));
        }

        /// <summary>
        ///   Converts a object to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this object[][][] value)
        {
            return ToInt16(value, Jagged.CreateAs<object, short>(value));
        }




        /// <summary>
        ///   Converts a object array to a short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this object[] value, short[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int16)System.Convert.ChangeType(value[i], typeof(Int16));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this object[,] value, short[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Int16)System.Convert.ChangeType(value[i, j], typeof(Int16));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this object[,] value, short[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int16)System.Convert.ChangeType(value[i, j], typeof(Int16));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this object[][] value, short[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int16)System.Convert.ChangeType(value[i][j], typeof(Int16));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this object[][][] value, short[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int16)System.Convert.ChangeType(value[i][j][k], typeof(Int16));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this object[][] value, short[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int16)System.Convert.ChangeType(value[i][j], typeof(Int16));
            return result;
        }
        /// <summary>
        ///   Converts a object to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this object[] value)
        {
            return ToSingle(value, new float[value.Length]);
        }

        /// <summary>
        ///   Converts a object to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this object[,] value)
        {
            return ToSingle(value, Matrix.CreateAs<object, float>(value));
        }

        /// <summary>
        ///   Converts a object to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this object[][] value)
        {
            return ToSingle(value, Jagged.CreateAs<object, float>(value));
        }

        /// <summary>
        ///   Converts a object to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this object[][][] value)
        {
            return ToSingle(value, Jagged.CreateAs<object, float>(value));
        }




        /// <summary>
        ///   Converts a object array to a single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this object[] value, float[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Single)System.Convert.ChangeType(value[i], typeof(Single));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this object[,] value, float[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Single)System.Convert.ChangeType(value[i, j], typeof(Single));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this object[,] value, float[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Single)System.Convert.ChangeType(value[i, j], typeof(Single));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this object[][] value, float[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Single)System.Convert.ChangeType(value[i][j], typeof(Single));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this object[][][] value, float[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Single)System.Convert.ChangeType(value[i][j][k], typeof(Single));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this object[][] value, float[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Single)System.Convert.ChangeType(value[i][j], typeof(Single));
            return result;
        }
        /// <summary>
        ///   Converts a object to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this object[] value)
        {
            return ToDouble(value, new double[value.Length]);
        }

        /// <summary>
        ///   Converts a object to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this object[,] value)
        {
            return ToDouble(value, Matrix.CreateAs<object, double>(value));
        }

        /// <summary>
        ///   Converts a object to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this object[][] value)
        {
            return ToDouble(value, Jagged.CreateAs<object, double>(value));
        }

        /// <summary>
        ///   Converts a object to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this object[][][] value)
        {
            return ToDouble(value, Jagged.CreateAs<object, double>(value));
        }




        /// <summary>
        ///   Converts a object array to a double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this object[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Double)System.Convert.ChangeType(value[i], typeof(Double));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this object[,] value, double[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Double)System.Convert.ChangeType(value[i, j], typeof(Double));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this object[,] value, double[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Double)System.Convert.ChangeType(value[i, j], typeof(Double));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this object[][] value, double[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Double)System.Convert.ChangeType(value[i][j], typeof(Double));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this object[][][] value, double[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Double)System.Convert.ChangeType(value[i][j][k], typeof(Double));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this object[][] value, double[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Double)System.Convert.ChangeType(value[i][j], typeof(Double));
            return result;
        }
        /// <summary>
        ///   Converts a object to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this object[] value)
        {
            return ToInt64(value, new long[value.Length]);
        }

        /// <summary>
        ///   Converts a object to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this object[,] value)
        {
            return ToInt64(value, Matrix.CreateAs<object, long>(value));
        }

        /// <summary>
        ///   Converts a object to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this object[][] value)
        {
            return ToInt64(value, Jagged.CreateAs<object, long>(value));
        }

        /// <summary>
        ///   Converts a object to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this object[][][] value)
        {
            return ToInt64(value, Jagged.CreateAs<object, long>(value));
        }




        /// <summary>
        ///   Converts a object array to a long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this object[] value, long[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Int64)System.Convert.ChangeType(value[i], typeof(Int64));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this object[,] value, long[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Int64)System.Convert.ChangeType(value[i, j], typeof(Int64));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this object[,] value, long[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Int64)System.Convert.ChangeType(value[i, j], typeof(Int64));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this object[][] value, long[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Int64)System.Convert.ChangeType(value[i][j], typeof(Int64));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this object[][][] value, long[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Int64)System.Convert.ChangeType(value[i][j][k], typeof(Int64));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this object[][] value, long[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Int64)System.Convert.ChangeType(value[i][j], typeof(Int64));
            return result;
        }
        /// <summary>
        ///   Converts a object to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this object[] value)
        {
            return ToByte(value, new byte[value.Length]);
        }

        /// <summary>
        ///   Converts a object to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this object[,] value)
        {
            return ToByte(value, Matrix.CreateAs<object, byte>(value));
        }

        /// <summary>
        ///   Converts a object to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this object[][] value)
        {
            return ToByte(value, Jagged.CreateAs<object, byte>(value));
        }

        /// <summary>
        ///   Converts a object to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this object[][][] value)
        {
            return ToByte(value, Jagged.CreateAs<object, byte>(value));
        }




        /// <summary>
        ///   Converts a object array to a 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this object[] value, byte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Byte)System.Convert.ChangeType(value[i], typeof(Byte));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this object[,] value, byte[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Byte)System.Convert.ChangeType(value[i, j], typeof(Byte));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this object[,] value, byte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Byte)System.Convert.ChangeType(value[i, j], typeof(Byte));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this object[][] value, byte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Byte)System.Convert.ChangeType(value[i][j], typeof(Byte));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this object[][][] value, byte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Byte)System.Convert.ChangeType(value[i][j][k], typeof(Byte));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this object[][] value, byte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Byte)System.Convert.ChangeType(value[i][j], typeof(Byte));
            return result;
        }
        /// <summary>
        ///   Converts a object to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this object[] value)
        {
            return ToSByte(value, new sbyte[value.Length]);
        }

        /// <summary>
        ///   Converts a object to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this object[,] value)
        {
            return ToSByte(value, Matrix.CreateAs<object, sbyte>(value));
        }

        /// <summary>
        ///   Converts a object to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this object[][] value)
        {
            return ToSByte(value, Jagged.CreateAs<object, sbyte>(value));
        }

        /// <summary>
        ///   Converts a object to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this object[][][] value)
        {
            return ToSByte(value, Jagged.CreateAs<object, sbyte>(value));
        }




        /// <summary>
        ///   Converts a object array to a signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this object[] value, sbyte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (SByte)System.Convert.ChangeType(value[i], typeof(SByte));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this object[,] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (SByte)System.Convert.ChangeType(value[i, j], typeof(SByte));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this object[,] value, sbyte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (SByte)System.Convert.ChangeType(value[i, j], typeof(SByte));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this object[][] value, sbyte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (SByte)System.Convert.ChangeType(value[i][j], typeof(SByte));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this object[][][] value, sbyte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (SByte)System.Convert.ChangeType(value[i][j][k], typeof(SByte));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this object[][] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (SByte)System.Convert.ChangeType(value[i][j], typeof(SByte));
            return result;
        }
        /// <summary>
        ///   Converts a object to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this object[] value)
        {
            return ToDecimal(value, new decimal[value.Length]);
        }

        /// <summary>
        ///   Converts a object to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this object[,] value)
        {
            return ToDecimal(value, Matrix.CreateAs<object, decimal>(value));
        }

        /// <summary>
        ///   Converts a object to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this object[][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<object, decimal>(value));
        }

        /// <summary>
        ///   Converts a object to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this object[][][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<object, decimal>(value));
        }




        /// <summary>
        ///   Converts a object array to a decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this object[] value, decimal[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Decimal)System.Convert.ChangeType(value[i], typeof(Decimal));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this object[,] value, decimal[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Decimal)System.Convert.ChangeType(value[i, j], typeof(Decimal));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this object[,] value, decimal[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Decimal)System.Convert.ChangeType(value[i, j], typeof(Decimal));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this object[][] value, decimal[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Decimal)System.Convert.ChangeType(value[i][j], typeof(Decimal));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this object[][][] value, decimal[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Decimal)System.Convert.ChangeType(value[i][j][k], typeof(Decimal));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this object[][] value, decimal[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Decimal)System.Convert.ChangeType(value[i][j], typeof(Decimal));
            return result;
        }
        /// <summary>
        ///   Converts a object to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this object[] value)
        {
            return ToBoolean(value, new bool[value.Length]);
        }

        /// <summary>
        ///   Converts a object to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this object[,] value)
        {
            return ToBoolean(value, Matrix.CreateAs<object, bool>(value));
        }

        /// <summary>
        ///   Converts a object to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this object[][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<object, bool>(value));
        }

        /// <summary>
        ///   Converts a object to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this object[][][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<object, bool>(value));
        }




        /// <summary>
        ///   Converts a object array to a boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this object[] value, bool[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Boolean)System.Convert.ChangeType(value[i], typeof(Boolean));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this object[,] value, bool[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Boolean)System.Convert.ChangeType(value[i, j], typeof(Boolean));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this object[,] value, bool[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Boolean)System.Convert.ChangeType(value[i, j], typeof(Boolean));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this object[][] value, bool[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Boolean)System.Convert.ChangeType(value[i][j], typeof(Boolean));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this object[][][] value, bool[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Boolean)System.Convert.ChangeType(value[i][j][k], typeof(Boolean));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this object[][] value, bool[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Boolean)System.Convert.ChangeType(value[i][j], typeof(Boolean));
            return result;
        }
        /// <summary>
        ///   Converts a object to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this object[] value)
        {
            return ToString(value, new string[value.Length]);
        }

        /// <summary>
        ///   Converts a object to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this object[,] value)
        {
            return ToString(value, Matrix.CreateAs<object, string>(value));
        }

        /// <summary>
        ///   Converts a object to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this object[][] value)
        {
            return ToString(value, Jagged.CreateAs<object, string>(value));
        }

        /// <summary>
        ///   Converts a object to a string.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this object[][][] value)
        {
            return ToString(value, Jagged.CreateAs<object, string>(value));
        }




        /// <summary>
        ///   Converts a object array to a string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[] ToString(this object[] value, string[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (String)System.Convert.ChangeType(value[i], typeof(String));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this object[,] value, string[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (String)System.Convert.ChangeType(value[i, j], typeof(String));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional object array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this object[,] value, string[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (String)System.Convert.ChangeType(value[i, j], typeof(String));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][] ToString(this object[][] value, string[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (String)System.Convert.ChangeType(value[i][j], typeof(String));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a jagged string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[][][] ToString(this object[][][] value, string[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (String)System.Convert.ChangeType(value[i][j][k], typeof(String));
            return result;
        }

        /// <summary>
        ///   Converts a jagged object array to a multidimensional string array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string[,] ToString(this object[][] value, string[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (String)System.Convert.ChangeType(value[i][j], typeof(String));
            return result;
        }
        /// <summary>
        ///   Converts a string to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this string[] value)
        {
            return ToInt32(value, new int[value.Length]);
        }

        /// <summary>
        ///   Converts a string to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this string[,] value)
        {
            return ToInt32(value, Matrix.CreateAs<string, int>(value));
        }

        /// <summary>
        ///   Converts a string to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this string[][] value)
        {
            return ToInt32(value, Jagged.CreateAs<string, int>(value));
        }

        /// <summary>
        ///   Converts a string to a integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this string[][][] value)
        {
            return ToInt32(value, Jagged.CreateAs<string, int>(value));
        }




        /// <summary>
        ///   Converts a string array to a integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[] ToInt32(this string[] value, int[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = Int32.Parse(value[i]); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this string[,] value, int[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = Int32.Parse(value[i, j]); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this string[,] value, int[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = Int32.Parse(value[i, j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][] ToInt32(this string[][] value, int[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = Int32.Parse(value[i][j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[][][] ToInt32(this string[][][] value, int[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = Int32.Parse(value[i][j][k]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a multidimensional integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int[,] ToInt32(this string[][] value, int[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = Int32.Parse(value[i][j]); ;
            return result;
        }
        /// <summary>
        ///   Converts a string to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this string[] value)
        {
            return ToInt16(value, new short[value.Length]);
        }

        /// <summary>
        ///   Converts a string to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this string[,] value)
        {
            return ToInt16(value, Matrix.CreateAs<string, short>(value));
        }

        /// <summary>
        ///   Converts a string to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this string[][] value)
        {
            return ToInt16(value, Jagged.CreateAs<string, short>(value));
        }

        /// <summary>
        ///   Converts a string to a short integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this string[][][] value)
        {
            return ToInt16(value, Jagged.CreateAs<string, short>(value));
        }




        /// <summary>
        ///   Converts a string array to a short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[] ToInt16(this string[] value, short[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = Int16.Parse(value[i]); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this string[,] value, short[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = Int16.Parse(value[i, j]); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this string[,] value, short[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = Int16.Parse(value[i, j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][] ToInt16(this string[][] value, short[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = Int16.Parse(value[i][j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[][][] ToInt16(this string[][][] value, short[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = Int16.Parse(value[i][j][k]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a multidimensional short integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static short[,] ToInt16(this string[][] value, short[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = Int16.Parse(value[i][j]); ;
            return result;
        }
        /// <summary>
        ///   Converts a string to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this string[] value)
        {
            return ToSingle(value, new float[value.Length]);
        }

        /// <summary>
        ///   Converts a string to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this string[,] value)
        {
            return ToSingle(value, Matrix.CreateAs<string, float>(value));
        }

        /// <summary>
        ///   Converts a string to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this string[][] value)
        {
            return ToSingle(value, Jagged.CreateAs<string, float>(value));
        }

        /// <summary>
        ///   Converts a string to a single-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this string[][][] value)
        {
            return ToSingle(value, Jagged.CreateAs<string, float>(value));
        }




        /// <summary>
        ///   Converts a string array to a single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[] ToSingle(this string[] value, float[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = Single.Parse(value[i]); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this string[,] value, float[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = Single.Parse(value[i, j]); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this string[,] value, float[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = Single.Parse(value[i, j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][] ToSingle(this string[][] value, float[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = Single.Parse(value[i][j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[][][] ToSingle(this string[][][] value, float[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = Single.Parse(value[i][j][k]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a multidimensional single-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static float[,] ToSingle(this string[][] value, float[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = Single.Parse(value[i][j]); ;
            return result;
        }
        /// <summary>
        ///   Converts a string to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this string[] value)
        {
            return ToDouble(value, new double[value.Length]);
        }

        /// <summary>
        ///   Converts a string to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this string[,] value)
        {
            return ToDouble(value, Matrix.CreateAs<string, double>(value));
        }

        /// <summary>
        ///   Converts a string to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this string[][] value)
        {
            return ToDouble(value, Jagged.CreateAs<string, double>(value));
        }

        /// <summary>
        ///   Converts a string to a double-precision floating point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this string[][][] value)
        {
            return ToDouble(value, Jagged.CreateAs<string, double>(value));
        }




        /// <summary>
        ///   Converts a string array to a double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[] ToDouble(this string[] value, double[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = Double.Parse(value[i]); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this string[,] value, double[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = Double.Parse(value[i, j]); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this string[,] value, double[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = Double.Parse(value[i, j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][] ToDouble(this string[][] value, double[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = Double.Parse(value[i][j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[][][] ToDouble(this string[][][] value, double[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = Double.Parse(value[i][j][k]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a multidimensional double-precision floating point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static double[,] ToDouble(this string[][] value, double[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = Double.Parse(value[i][j]); ;
            return result;
        }
        /// <summary>
        ///   Converts a string to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this string[] value)
        {
            return ToInt64(value, new long[value.Length]);
        }

        /// <summary>
        ///   Converts a string to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this string[,] value)
        {
            return ToInt64(value, Matrix.CreateAs<string, long>(value));
        }

        /// <summary>
        ///   Converts a string to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this string[][] value)
        {
            return ToInt64(value, Jagged.CreateAs<string, long>(value));
        }

        /// <summary>
        ///   Converts a string to a long integer.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this string[][][] value)
        {
            return ToInt64(value, Jagged.CreateAs<string, long>(value));
        }




        /// <summary>
        ///   Converts a string array to a long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[] ToInt64(this string[] value, long[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = Int64.Parse(value[i]); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this string[,] value, long[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = Int64.Parse(value[i, j]); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this string[,] value, long[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = Int64.Parse(value[i, j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][] ToInt64(this string[][] value, long[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = Int64.Parse(value[i][j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[][][] ToInt64(this string[][][] value, long[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = Int64.Parse(value[i][j][k]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a multidimensional long integer array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static long[,] ToInt64(this string[][] value, long[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = Int64.Parse(value[i][j]); ;
            return result;
        }
        /// <summary>
        ///   Converts a string to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this string[] value)
        {
            return ToByte(value, new byte[value.Length]);
        }

        /// <summary>
        ///   Converts a string to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this string[,] value)
        {
            return ToByte(value, Matrix.CreateAs<string, byte>(value));
        }

        /// <summary>
        ///   Converts a string to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this string[][] value)
        {
            return ToByte(value, Jagged.CreateAs<string, byte>(value));
        }

        /// <summary>
        ///   Converts a string to a 8-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this string[][][] value)
        {
            return ToByte(value, Jagged.CreateAs<string, byte>(value));
        }




        /// <summary>
        ///   Converts a string array to a 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByte(this string[] value, byte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = Byte.Parse(value[i]); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this string[,] value, byte[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = Byte.Parse(value[i, j]); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this string[,] value, byte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = Byte.Parse(value[i, j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][] ToByte(this string[][] value, byte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = Byte.Parse(value[i][j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[][][] ToByte(this string[][][] value, byte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = Byte.Parse(value[i][j][k]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a multidimensional 8-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[,] ToByte(this string[][] value, byte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = Byte.Parse(value[i][j]); ;
            return result;
        }
        /// <summary>
        ///   Converts a string to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this string[] value)
        {
            return ToSByte(value, new sbyte[value.Length]);
        }

        /// <summary>
        ///   Converts a string to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this string[,] value)
        {
            return ToSByte(value, Matrix.CreateAs<string, sbyte>(value));
        }

        /// <summary>
        ///   Converts a string to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this string[][] value)
        {
            return ToSByte(value, Jagged.CreateAs<string, sbyte>(value));
        }

        /// <summary>
        ///   Converts a string to a signed 7-bit byte.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this string[][][] value)
        {
            return ToSByte(value, Jagged.CreateAs<string, sbyte>(value));
        }




        /// <summary>
        ///   Converts a string array to a signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[] ToSByte(this string[] value, sbyte[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = SByte.Parse(value[i]); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this string[,] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = SByte.Parse(value[i, j]); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this string[,] value, sbyte[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = SByte.Parse(value[i, j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][] ToSByte(this string[][] value, sbyte[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = SByte.Parse(value[i][j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[][][] ToSByte(this string[][][] value, sbyte[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = SByte.Parse(value[i][j][k]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a multidimensional signed 7-bit byte array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static sbyte[,] ToSByte(this string[][] value, sbyte[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = SByte.Parse(value[i][j]); ;
            return result;
        }
        /// <summary>
        ///   Converts a string to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this string[] value)
        {
            return ToDecimal(value, new decimal[value.Length]);
        }

        /// <summary>
        ///   Converts a string to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this string[,] value)
        {
            return ToDecimal(value, Matrix.CreateAs<string, decimal>(value));
        }

        /// <summary>
        ///   Converts a string to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this string[][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<string, decimal>(value));
        }

        /// <summary>
        ///   Converts a string to a decimal fixed-point.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this string[][][] value)
        {
            return ToDecimal(value, Jagged.CreateAs<string, decimal>(value));
        }




        /// <summary>
        ///   Converts a string array to a decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[] ToDecimal(this string[] value, decimal[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = Decimal.Parse(value[i]); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this string[,] value, decimal[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = Decimal.Parse(value[i, j]); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this string[,] value, decimal[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = Decimal.Parse(value[i, j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][] ToDecimal(this string[][] value, decimal[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = Decimal.Parse(value[i][j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[][][] ToDecimal(this string[][][] value, decimal[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = Decimal.Parse(value[i][j][k]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a multidimensional decimal fixed-point array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static decimal[,] ToDecimal(this string[][] value, decimal[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = Decimal.Parse(value[i][j]); ;
            return result;
        }
        /// <summary>
        ///   Converts a string to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this string[] value)
        {
            return ToBoolean(value, new bool[value.Length]);
        }

        /// <summary>
        ///   Converts a string to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this string[,] value)
        {
            return ToBoolean(value, Matrix.CreateAs<string, bool>(value));
        }

        /// <summary>
        ///   Converts a string to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this string[][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<string, bool>(value));
        }

        /// <summary>
        ///   Converts a string to a boolean.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this string[][][] value)
        {
            return ToBoolean(value, Jagged.CreateAs<string, bool>(value));
        }




        /// <summary>
        ///   Converts a string array to a boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[] ToBoolean(this string[] value, bool[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = Boolean.Parse(value[i]); ;
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this string[,] value, bool[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = Boolean.Parse(value[i, j]); ;

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this string[,] value, bool[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = Boolean.Parse(value[i, j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][] ToBoolean(this string[][] value, bool[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = Boolean.Parse(value[i][j]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[][][] ToBoolean(this string[][][] value, bool[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = Boolean.Parse(value[i][j][k]); ;
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a multidimensional boolean array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool[,] ToBoolean(this string[][] value, bool[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = Boolean.Parse(value[i][j]); ;
            return result;
        }
        /// <summary>
        ///   Converts a string to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this string[] value)
        {
            return ToObject(value, new object[value.Length]);
        }

        /// <summary>
        ///   Converts a string to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this string[,] value)
        {
            return ToObject(value, Matrix.CreateAs<string, object>(value));
        }

        /// <summary>
        ///   Converts a string to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this string[][] value)
        {
            return ToObject(value, Jagged.CreateAs<string, object>(value));
        }

        /// <summary>
        ///   Converts a string to a object.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this string[][][] value)
        {
            return ToObject(value, Jagged.CreateAs<string, object>(value));
        }




        /// <summary>
        ///   Converts a string array to a object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[] ToObject(this string[] value, object[] result)
        {
            for (int i = 0; i < value.Length; i++)
                result[i] = (Object)System.Convert.ChangeType(value[i], typeof(Object));
            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this string[,] value, object[,] result)
        {
            for (int i = 0; i < value.Rows(); i++)
                for (int j = 0; j < value.Columns(); j++)
                    result[i, j] = (Object)System.Convert.ChangeType(value[i, j], typeof(Object));

            return result;
        }

        /// <summary>
        ///   Converts a multidimensional string array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this string[,] value, object[][] result)
        {
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result[i].Length; j++)
                    result[i][j] = (Object)System.Convert.ChangeType(value[i, j], typeof(Object));
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][] ToObject(this string[][] value, object[][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i][j] = (Object)System.Convert.ChangeType(value[i][j], typeof(Object));
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a jagged object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[][][] ToObject(this string[][][] value, object[][][] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    for (int k = 0; k < value[i][j].Length; k++)
                        result[i][j][k] = (Object)System.Convert.ChangeType(value[i][j][k], typeof(Object));
            return result;
        }

        /// <summary>
        ///   Converts a jagged string array to a multidimensional object array.
        /// </summary>
        /// 
#if NET45 || NET46 || NET462 || NETSTANDARD2_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object[,] ToObject(this string[][] value, object[,] result)
        {
            for (int i = 0; i < value.Length; i++)
                for (int j = 0; j < value[i].Length; j++)
                    result[i, j] = (Object)System.Convert.ChangeType(value[i][j], typeof(Object));
            return result;
        }
    }
}
