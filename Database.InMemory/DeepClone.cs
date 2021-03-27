// https://github.com/Burtsev-Alexey/net-object-deep-copy

// The MIT License (MIT)
// Copyright (c) 2014 Burtsev Alexey
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Database.InMemory
{
    [ExcludeFromCodeCoverage]
    internal static class DeepClone
    {
        private class ReferenceEqualityComparer : EqualityComparer<object>
        {
            // ReSharper disable once MemberHidesStaticFromOuterClass
            public override bool Equals (object? x, object? y)
            {
                return ReferenceEquals(x, y);
            }

            public override int GetHashCode (object? obj)
            {
                if (obj == null) return 0;
                return obj.GetHashCode();
            }
        }

        private class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse (Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                    maxLengths[i] = array.GetLength(i) - 1;
                Position = new int[array.Rank];
            }

            public bool Step ()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                            Position[j] = 0;
                        return true;
                    }
                }
                return false;
            }
        }

        private static readonly MethodInfo cloneMethod;

        static DeepClone ()
        {
            cloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)!;
            if (cloneMethod is null) throw new MethodAccessException();
        }

        public static object? Copy (object? obj)
        {
            return InternalCopy(obj, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }

        private static void ForEach (Array array, Action<Array, int[]> action)
        {
            if (array.LongLength == 0) return;
            var walker = new ArrayTraverse(array);
            do action(array, walker.Position);
            while (walker.Step());
        }

        private static bool IsPrimitive (Type type)
        {
            if (type == typeof(string)) return true;
            return type.IsValueType & type.IsPrimitive;
        }

        private static object? InternalCopy (object? sourceObj, IDictionary<object, object> visited)
        {
            if (sourceObj == null) return null;
            var typeToReflect = sourceObj.GetType();
            if (IsPrimitive(typeToReflect)) return sourceObj;
            if (visited.ContainsKey(sourceObj)) return visited[sourceObj];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            var clonedObj = cloneMethod.Invoke(sourceObj, null);
            if (clonedObj is null) throw new Exception("Cloned object is null");
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (arrayType != null && !IsPrimitive(arrayType))
                {
                    var clonedArray = (Array)clonedObj;
                    ForEach(clonedArray, (array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }
            }
            visited.Add(sourceObj, clonedObj);
            CopyFields(sourceObj, visited, clonedObj, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(sourceObj, visited, clonedObj, typeToReflect);
            return clonedObj;
        }

        private static void RecursiveCopyBaseTypePrivateFields (object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType is null) return;
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
            CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
        }

        private static void CopyFields (object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool>? filter = default)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
    }
}
