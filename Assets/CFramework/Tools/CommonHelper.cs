using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CommonHelper
{
    /// <summary>
    /// 反射深拷贝
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="tIn"></param>
    /// <returns></returns>
    public static TOut TransReflection<TIn, TOut>(TIn tIn)
    {
        TOut tOut = Activator.CreateInstance<TOut>();
        var tInType = tIn.GetType();
        foreach (var itemOut in tOut.GetType().GetProperties())
        {
            var itemIn = tInType.GetProperty(itemOut.Name); ;
            if (itemIn != null)
            {
                itemOut.SetValue(tOut, itemIn.GetValue(tIn));
            }
        }
        return tOut;
    }

    /// <summary>
    /// 序列化深拷贝
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    /// <param name="tIn"></param>
    /// <returns></returns>
    public static TOut TransSerialize<TOut,TIn>(TIn tIn)
    {
        TOut tOut = JsonUtility.FromJson<TOut>(JsonUtility.ToJson(tIn));
        return tOut;
    }

    /// <summary>
    /// 表达式树深拷贝
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public static class TransExp<TIn, TOut>
    {
        private static readonly Func<TIn, TOut> cache = GetFunc();
        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite)
                    continue;

                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut Trans(TIn tIn)
        {
            return cache(tIn);
        }
    }

}
