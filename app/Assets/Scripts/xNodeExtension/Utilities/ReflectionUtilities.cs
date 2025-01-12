using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NT.SceneObjects;
using NT.Variables;
using UnityEngine;

namespace NT
{
    public static class ReflectionUtilities {
        public static Type[] nodeTypes { get { return _nodeTypes != null ? _nodeTypes : _nodeTypes = GetNodeTypes(); } }
        public static Type[] variableNodeTypes { get { return _variableNodeTypes != null ? _variableNodeTypes : _variableNodeTypes = GetVariableNodeTypes(); } }

        [NonSerialized] private static Type[] _nodeTypes = null;
        [NonSerialized] private static Type[] _variableNodeTypes = null;

        public static Type[] GetNodeTypes() {
            return GetDerivedTypes(typeof(NT.NTNode));
        }

        public static Type[] GetVariableNodeTypes() {
            return GetDerivedTypes(typeof(NTVariable));
        }

        public static Type[] GetDerivedTypes(Type baseType) {
            List<System.Type> types = new List<System.Type>();
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) {
                types.AddRange(assembly.GetTypes().Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t)).ToArray());
            }
            return types.ToArray();
        }

        
        public static Dictionary<Type, List<string>> DesgloseInBasicTypes(Type t){
            return DesgloseInBasicTypes(t, new List<string>());
        }


        private static Dictionary<Type, Dictionary<Type, List<string>> > cache = new Dictionary<Type, Dictionary<Type, List<string>>>();

        public static Dictionary<Type, List<string>> DesgloseInBasicTypes(Type t, List<string> ignored){
        
            if(cache.ContainsKey(t)){
                return cache[t];
            }

            FieldInfo[] fi = t.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            Dictionary<Type, List<string>> desglosed = new Dictionary<Type, List<string>>();

            foreach(FieldInfo f in fi){
                if(f.GetCustomAttribute(typeof(HideInInspector)) != null) continue;
                if(ignored.Contains(f.Name)) continue;
                

                if(IsBasicType(f.FieldType)){
                    List<string> variables;

                    if(desglosed.ContainsKey(f.FieldType)){
                        variables = desglosed[f.FieldType];
                        variables.Add(f.Name);
                        desglosed[f.FieldType] = variables;
                    }
                    else
                    {
                        variables = new List<string>();
                        variables.Add(f.Name);
                        desglosed.Add(f.FieldType, variables);
                    }
                }
                else
                {
                    Desglose(f.FieldType, ref desglosed, f.Name+ "/", new List<Type>() );
                }
            }

            if(!cache.ContainsKey(t)) cache.Add(t, desglosed);
            return desglosed;
        }

        public static void SetValueOf(ref object o, object value, List<string> path)
        {
            if(path.Count == 0){
                o = value;
            }
            else
            {
                if (o == null)
                {
                    return;
                }

                string current = path[0];
                Type t = o.GetType();

                FieldInfo fi = t.GetField(current);
                PropertyInfo fp = t.GetProperty(current);

                if (fi != null)
                {
                    object fio = fi.GetValue(o);
                    path.RemoveAt(0);

                    SetValueOf(ref fio, value, path);
                    fi.SetValue(o, fio);
                    return;
                }
                else if (fp != null) {
                    object fio = fp.GetValue(o);
                    path.RemoveAt(0);

                    SetValueOf(ref fio, value, path);
                    fp.SetValue(o, fio);
                    return;
                }
                else if (current.StartsWith("[") && current.EndsWith("]"))
                {
                    string key = current.Substring(1, current.Length - 2);

                    if (o is IDictionary dict)
                    {
                        object fio = dict[key];
                        path.RemoveAt(0);
                        SetValueOf(ref fio, value, path);
                        //dict[key] = fio;
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private static void Desglose(Type t, ref Dictionary<Type, List<string>> deg, string root, List<Type> visisitedTypes){
            FieldInfo[] fi = t.GetFields();

            foreach(FieldInfo f in fi){

                if(f.IsNotSerialized) continue;

                if(IsBasicType(f.FieldType)){
                    List<string> variables;

                    if(deg.ContainsKey(f.FieldType)){
                        variables = deg[f.FieldType];
                        variables.Add(root + f.Name);
                        deg[f.FieldType] = variables;
                    }
                    else
                    {
                        variables = new List<string>();
                        variables.Add(root + f.Name);
                        deg.Add(f.FieldType, variables);                        
                    }
                }
                else
                {
                    if(!visisitedTypes.Contains(f.FieldType)){
                        visisitedTypes.Add(f.FieldType);
                    }
                    else
                    {
                        return; 
                    }

                    Desglose(f.FieldType, ref deg, root + f.Name+ "/", visisitedTypes);
                }
            }

        }

        public static bool IsBasicType(Type t){
            bool isBasic = (t == typeof(string) ||
                            t == typeof(int) ||
                            t == typeof(float) ||
                            t == typeof(double) ||
                            t == typeof(bool) ||
                            t.IsEnum ||
                            t == typeof(SceneGameObjectReference));
                            //|| isTupleType(t));

            return isBasic;
        }

        //public static bool isTupleType(Type type)
        //{
        //    if (type == typeof(Tuple))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        if (type.IsGenericType)
        //        {
        //            var genType = type.GetGenericTypeDefinition();
        //            return (genType == typeof(Tuple<>)
        //                || genType == typeof(Tuple<,>)
        //                || genType == typeof(Tuple<,,>)
        //                || genType == typeof(Tuple<,,,>)
        //                || genType == typeof(Tuple<,,,,>)
        //                || genType == typeof(Tuple<,,,,,>)
        //                || genType == typeof(Tuple<,,,,,,>)
        //                || genType == typeof(Tuple<,,,,,,,>));
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        public static object GetValueOf(List<string> path, object o){
            if(path.Count > 0){
                string current = path[0];

                if(o == null){
                    return null;
                }

                Type t = o.GetType();

                FieldInfo fi = t.GetField(current);
                PropertyInfo fp = t.GetProperty(current);

                if(fi != null){
                    object fio = fi.GetValue(o);

                    path.RemoveAt(0);

                    return GetValueOf(path, fio);
                }
                else if(fp != null)
                {
                    object fio = fp.GetValue(o);

                    path.RemoveAt(0);

                    return GetValueOf(path, fio);
                }
                else if (current.StartsWith("[") && current.EndsWith("]"))
                {
                    string key = current.Substring(1, current.Length - 2);

                    if (o is IDictionary dict)
                    {
                        object fio = dict[key];
                        path.RemoveAt(0);
                        return GetValueOf(path, fio);
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return o;
            }
        }
    }
}