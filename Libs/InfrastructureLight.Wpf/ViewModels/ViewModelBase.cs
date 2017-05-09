﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Diagnostics;

using System.Windows.Input;

namespace InfrastructureLight.Wpf.ViewModels
{
    using Commands;
    using EventArgs;

    public abstract class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null)
            {
                VerifyPropertyName(propertyName);
                var e = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, e);
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Не сущесвует свойство с именем: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion

        #region IDataErrorInfo, Validation Logic

        private Dictionary<string, Binder> ruleMap = new Dictionary<string, Binder>();
        
        public void AddRule<T>(Expression<Func<T>> propertyExpression, Func<bool> ruleDelegate, string errorMessage)
        {
            var propertyName = GetPropertyName(propertyExpression);
            ruleMap.Add(propertyName, new Binder(ruleDelegate, errorMessage));
        }
        
        public bool HasErrors
        {
            get
            {
                var values = ruleMap.Values.ToList();
                values.ForEach(b => b.Update());

                return values.Any(b => b.HasError);
            }
        }
        
        public string Error
        {
            get
            {
                var errors = from b in ruleMap.Values
                             where b.HasError
                             select b.Error;

                return string.Join("\n", errors);
            }
        }
           
        public string this[string columnName]
        {
            get
            {
                if (ruleMap.ContainsKey(columnName))
                {
                    ruleMap[columnName].Update();
                    return ruleMap[columnName].Error;
                }
                return string.Empty;
            }
        }

        private class Binder
        {
            private readonly Func<bool> ruleDelegate;
            private readonly string message;

            internal Binder(Func<bool> ruleDelegate, string message)
            {
                this.ruleDelegate = ruleDelegate;
                this.message = message;
            }

            internal string Error { get; set; }
            internal bool HasError { get; set; }

            internal void Update()
            {
                Error = null;
                HasError = false;
                try
                {
                    if (!ruleDelegate())
                    {
                        Error = message;
                        HasError = true;
                    }
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    HasError = true;
                }
            }
        }

        private string GetPropertyName<T>(Expression<Func<T>> exp)
        {
            return (((MemberExpression)(exp.Body)).Member).Name;
        }

        #endregion

        #region Fields
        
        string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChangedEvent("Title"); }
        }

        #endregion

        #region Commands
        
        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??
                    (_saveCommand = new DelegateCommand(action => Save(), action => CanSave()));
            }
        }
        protected virtual void Save()
        {
            OnSaved();
        }
        protected virtual bool CanSave()
        {
            return true;
        }
        
        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                return _closeCommand ??
                    (_closeCommand = new DelegateCommand(action => Close(), action => CanClose()));
            }
        }
        protected virtual void Close()
        {
            OnClosed(false);
        }
        protected virtual bool CanClose()
        {
            return true;
        }

        #endregion     

        #region Events
        
        private EventHandler _savedInvocList;
        public event EventHandler Saved
        {
            add
            {
                if (_savedInvocList == null || _savedInvocList.GetInvocationList()
                    .All(m => m.Method != value.Method))
                {
                    _savedInvocList += value;
                }
            }
            remove { _savedInvocList -= value; }
        }
        protected virtual void OnSaved()
        {
            EventHandler handler = _savedInvocList;
            if (handler != null) handler(this, System.EventArgs.Empty);
        }
        
        private EventHandler<ConfirmEventArgs> _confirmInvocList;
        public event EventHandler<ConfirmEventArgs> Confirm
        {
            add
            {
                if (_confirmInvocList == null || _confirmInvocList.GetInvocationList()
                    .All(m => m.Method != value.Method))
                {
                    _confirmInvocList += value;
                }
            }
            remove { _confirmInvocList -= value; }
        }
        protected virtual void OnConfirm(string message, Action callback)
        {
            EventHandler<ConfirmEventArgs> handler = _confirmInvocList;
            if (handler != null) handler(this, new ConfirmEventArgs(message, callback));
        }
        
        private EventHandler<CloseDialogEventArgs> _closedInvocList;
        public event EventHandler<CloseDialogEventArgs> Closed
        {
            add
            {
                if (_closedInvocList == null || _closedInvocList.GetInvocationList()
                    .All(m => m.Method != value.Method))
                {
                    _closedInvocList += value;
                }
            }
            remove { _closedInvocList -= value; }
        }
        protected virtual void OnClosed(bool dialogResult)
        {
            EventHandler<CloseDialogEventArgs> handler = _closedInvocList;
            if (handler != null) handler(this, new CloseDialogEventArgs(dialogResult));
        }

        #endregion
    }
}