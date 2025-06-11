using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Core.Pattern
{
    /// <summary>
    /// tìm tất cả các class kế thừa từ T2 và có constructor không tham số, ko bao gôm abstract class
    /// </summary>
    /// <typeparam name="T1">enum key sử dung để tìm kếm</typeparam>
    /// <typeparam name="T2">source guide</typeparam>
    public class AFactory<T1, T2>
        where T1 : Enum
        where T2 : ITypeOnFactory<T1>
    {
        private static readonly Dictionary<T1, Type> ProductTypes;

        static AFactory()
        {
            ProductTypes = new Dictionary<T1, Type>();
            Init();
        }

        static void Init()
        {
            List<Type> allTypes = FindDerivedTypes(typeof(T2)).ToList();

            foreach (var type in allTypes)
            {
                T2 productTemp = (T2)Activator.CreateInstance(type);

                T1 typeIDOfProduct = productTemp.GetProductType();

                ProductTypes.TryAdd(typeIDOfProduct, type);
            }
        }

        static IEnumerable<Type> FindDerivedTypes(Type baseType)
        {
            Assembly assembly = Assembly.GetAssembly(baseType);
            return assembly.GetTypes()
                .Where(type =>
                    type != baseType && !type.IsAbstract && baseType.IsAssignableFrom(type));
        }

        public static T2 GetProduct(T1 typeID)
        {
            if (!ProductTypes.ContainsKey(typeID))
            {
                Debug.LogError($"not existed implement has type's {typeID}");
                throw new();
            }

            return (T2)Activator.CreateInstance(ProductTypes[typeID]);
        }
    }
}