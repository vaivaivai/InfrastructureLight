﻿namespace InfrastructureLight.Wpf.Common.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    public class DispatchHelper
    {
        /// <summary>
        /// Выполняет операцию в осноном потоке приложения
        /// </summary>
        public static void Invoke(Action action)
        {
            Dispatcher dispatchObject = Application.Current.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                action();
            }
            else
            {
                dispatchObject.Invoke(action);
            }
        }

        /// <summary>
        /// Выполняет операцию в осноном потоке приложения и возвращает результат в текущий поток
        /// </summary>
        public static T Invoke<T>(Func<T> action)
        {
            Dispatcher dispatchObject = Application.Current.Dispatcher;
            if (dispatchObject == null || dispatchObject.CheckAccess())
            {
                return action();
            }
            else
            {
                return (T)dispatchObject.Invoke(action);
            }
        }
    }
}