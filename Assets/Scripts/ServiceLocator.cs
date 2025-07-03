using System;
using System.Collections.Generic;

/// <summary>
/// 采用了服务定位器模式
/// </summary>
public class ServiceLocator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    /// <summary>
    /// 注册服务
    /// </summary>
    /// <typeparam name="T">约束一定必须是class类</typeparam>
    /// <param name="_service">要注册的服务</param>
    public static void Register<T>(T _service) where T : class
    {
        if (!services.ContainsKey(typeof(T)))
        {
            services.Add(typeof(T), _service);
        }
    }

    /// <summary>
    /// 获取服务
    /// </summary>
    /// <typeparam name="T">约束一定必须是class类</typeparam>
    /// <returns>返回服务</returns>
    /// <exception cref="Exception">警告没有找到该服务</exception>
    public static T GetService<T>() where T : class
    {
        if (services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }

        return null;
    }

    /// <summary>
    /// 移除所有服务
    /// </summary>
    public static void RemoveAllService() => services.Clear();
}
