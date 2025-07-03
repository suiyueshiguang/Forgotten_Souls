using System;
using System.Collections.Generic;

/// <summary>
/// �����˷���λ��ģʽ
/// </summary>
public class ServiceLocator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    /// <summary>
    /// ע�����
    /// </summary>
    /// <typeparam name="T">Լ��һ��������class��</typeparam>
    /// <param name="_service">Ҫע��ķ���</param>
    public static void Register<T>(T _service) where T : class
    {
        if (!services.ContainsKey(typeof(T)))
        {
            services.Add(typeof(T), _service);
        }
    }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    /// <typeparam name="T">Լ��һ��������class��</typeparam>
    /// <returns>���ط���</returns>
    /// <exception cref="Exception">����û���ҵ��÷���</exception>
    public static T GetService<T>() where T : class
    {
        if (services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }

        return null;
    }

    /// <summary>
    /// �Ƴ����з���
    /// </summary>
    public static void RemoveAllService() => services.Clear();
}
