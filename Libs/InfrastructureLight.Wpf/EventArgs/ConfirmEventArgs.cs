﻿using System;

namespace InfrastructureLight.Wpf.EventArgs
{
    public class ConfirmEventArgs : System.EventArgs
    {
        public string Message { get; private set; }
        public Action Callback { get; private set; }

        public ConfirmEventArgs(string message, Action callback)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (callback == null) throw new ArgumentNullException(nameof(callback));

            Callback = callback;
            Message = message;
        }
    }
}
