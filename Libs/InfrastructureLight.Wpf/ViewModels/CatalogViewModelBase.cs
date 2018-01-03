﻿using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace InfrastructureLight.Wpf.ViewModels
{
    using Behaviors;
    using Commands;
    using EventArgs;    

    public abstract class CatalogViewModelBase : AsyncViewModel
    {
        protected ISearch _searchCommand;
        readonly ICommand _updateCommand;

        public CatalogViewModelBase()
        {
            _updateCommand = new DelegateCommand(action => Update(), action => CanUpdate());
        }

        #region Fileds
       
        string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value; RaisePropertyChangedEvent(); }
        }

        #endregion

        #region Commands

        public ICommand SearchCommand => _searchCommand.Searched();
        public ICommand UpdateCommand => _updateCommand;
        protected virtual void Update()
        {
            RefreshAsynch();
        }
        protected virtual bool CanUpdate()
        {
            return true;
        }
        
        #endregion
       
        public virtual void RefreshAsynch()
        {
            Go(Refresh);
        }

        public virtual void Refresh()
        {

        }
    }

    public abstract class CatalogViewModelBase<T> : CatalogViewModelBase where T : class
    {
        public CatalogViewModelBase()
        {
            _searchCommand = new SearchCommon<T>(this);
        }
       
        ObservableCollection<T> _itemsSource;
        public ObservableCollection<T> ItemsSource
        {
            get { return _itemsSource ?? (_itemsSource = new ObservableCollection<T>()); }
            set { _itemsSource = value; RaisePropertyChangedEvent(); }
        }
        
        T _selectedItem;
        public virtual T SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; RaisePropertyChangedEvent(); }
        }

        #region Commands

        protected override bool CanSave()
        {
            return SelectedItem != null;
        }

        #endregion
    }
}
