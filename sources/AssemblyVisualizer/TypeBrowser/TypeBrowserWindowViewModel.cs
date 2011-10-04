// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Infrastructure;
using AssemblyVisualizer.Common;
using AssemblyVisualizer.HAL;
using System.Collections.ObjectModel;
using AssemblyVisualizer.Properties;
using AssemblyVisualizer.Model;
using System.Windows.Input;

namespace AssemblyVisualizer.InteractionBrowser
{
    class InteractionBrowserWindowViewModel : ViewModelBase
    {
        private Dictionary<MemberInfo, MemberViewModel> _viewModelsDictionary = new Dictionary<MemberInfo, MemberViewModel>();
        private MemberGraph _graph;
        private IEnumerable<TypeInfo> _types;
        private IEnumerable<IEnumerable<TypeViewModel>> _hierarchies;
        private IDictionary<TypeInfo, TypeViewModel> _viewModelCorrespondence = new Dictionary<TypeInfo, TypeViewModel>();
        private bool _isTypeSelectionVisible = true;

        public InteractionBrowserWindowViewModel(IEnumerable<TypeInfo> types)
        {
            _types = types;

            Commands = new ObservableCollection<UserCommand>
			           	{
			           		new UserCommand(Resources.FillGraph, OnFillGraphRequest),
			           		new UserCommand(Resources.OriginalSize, OnOriginalSizeRequest),	
                            new UserCommand(Resources.SelectTypes, ShowSelectionViewCommandHandler)
                            //new UserCommand(Resources.SearchInGraph, ShowSearchCommand),                                                    
			           	};
            ApplySelectionCommand = new DelegateCommand(ApplySelectionCommandHandler);
            HideSelectionViewCommand = new DelegateCommand(HideSelectionViewCommandHandler);

            _hierarchies = types
                .Select(GetHierarchy)
                .Select(h => h.Select(GetViewModelForType).ToArray())
                .ToArray();            
        }

        public event Action FillGraphRequest;
        public event Action OriginalSizeRequest;
        //public event Action FocusSearchRequest;

        public IEnumerable<UserCommand> Commands { get; private set; }

        public ICommand ApplySelectionCommand { get; private set; }
        public ICommand HideSelectionViewCommand { get; private set; }

        public IEnumerable<IEnumerable<TypeViewModel>> Hierarchies
        {
            get
            {
                return _hierarchies;
            }
            set
            {
                _hierarchies = value;
                OnPropertyChanged("Hierarchies");
            }
        }

        public MemberGraph Graph
        {
            get
            {
                return _graph;
            }
            set
            {
                _graph = value;
                OnPropertyChanged("Graph");
            }
        }

        public bool IsTypeSelectionVisible
        {
            get
            {
                return _isTypeSelectionVisible;
            }
            set
            {
                _isTypeSelectionVisible = value;
                OnPropertyChanged("IsTypeSelectionVisible");
            }
        }

        public string Title
        {
            get
            {
                return Resources.InteractionBrowser;
            }
        }

        private TypeViewModel GetViewModelForType(TypeInfo typeInfo)
        {
            if (_viewModelCorrespondence.ContainsKey(typeInfo))
            {
                return _viewModelCorrespondence[typeInfo];
            }
            var viewModel = new TypeViewModel(typeInfo);
            _viewModelCorrespondence.Add(typeInfo, viewModel);
            return viewModel;
        }

        private List<TypeInfo> GetHierarchy(TypeInfo typeInfo)
        {
            var hierarchy = new List<TypeInfo>();
            var currentType = typeInfo;
            hierarchy.Add(typeInfo);
            while (currentType.BaseType != null)
            {
                var t = currentType.BaseType;
                hierarchy.Add(t);
                currentType = t;
            }
            return hierarchy;
        }

        private MemberGraph CreateGraph(IEnumerable<TypeInfo> types)
        {
            var graph = new MemberGraph(true);            

            foreach (var type in types)
            {
                foreach (var method in type.Methods.Where(m => !m.Name.StartsWith("<")).Concat(type.Accessors))
                {
                    var mvm = GetViewModelForMethod(method);
                    if (!graph.ContainsVertex(mvm))
                    {
                        graph.AddVertex(mvm);
                    }

                    var usedMethods = Helper.GetUsedMethods(method.MemberReference)
                        .Where(m => types.Any(t => t.FullName == m.DeclaringType.FullName) && !m.Name.StartsWith("<"))
                        .ToArray();
                    foreach (var usedMethod in usedMethods)
                    {
                        var vm = GetViewModelForMethod(usedMethod);
                        if (!graph.ContainsVertex(vm))
                        {
                            graph.AddVertex(vm);
                        }
                        graph.AddEdge(new Controls.Graph.QuickGraph.Edge<MemberViewModel>(mvm, vm));
                    }                   

                    var usedFields = Helper.GetUsedFields(method.MemberReference)
                        .Where(m => types.Contains(m.DeclaringType) && !m.Name.StartsWith("CS$") && !m.Name.EndsWith("k__BackingField"))
                        .ToArray();
                    foreach (var usedField in usedFields)
                    {
                        var vm = GetViewModelForField(usedField);
                        if (!graph.ContainsVertex(vm))
                        {
                            graph.AddVertex(vm);
                        }
                        graph.AddEdge(new Controls.Graph.QuickGraph.Edge<MemberViewModel>(mvm, vm));
                    }
                }
            }

            return graph;
        }

        private MemberViewModel GetViewModelForField(FieldInfo fieldInfo)
        {  
            if (_viewModelsDictionary.ContainsKey(fieldInfo))
            {
                return _viewModelsDictionary[fieldInfo];
            }

            var eventInfo = Helper.GetEventForBackingField(fieldInfo.MemberReference);
            if (eventInfo != null)
            {
                if (_viewModelsDictionary.ContainsKey(eventInfo))
                {
                    return _viewModelsDictionary[eventInfo];
                }
                var evm = new EventViewModel(eventInfo);
                _viewModelsDictionary.Add(eventInfo, evm);
                return evm;
            }

            var vm = new FieldViewModel(fieldInfo);
            _viewModelsDictionary.Add(fieldInfo, vm);
            return vm;
        }

        private MemberViewModel GetViewModelForMethod(MethodInfo methodInfo)
        {
            if (_viewModelsDictionary.ContainsKey(methodInfo))
            {
                return _viewModelsDictionary[methodInfo];
            }

            if (IsPropertyAccessor(methodInfo))
            {
                var propertyInfo = Helper.GetAccessorProperty(methodInfo.MemberReference);
                if (_viewModelsDictionary.ContainsKey(propertyInfo))
                {
                    return _viewModelsDictionary[propertyInfo];
                }
                var pvm = new PropertyViewModel(propertyInfo);
                _viewModelsDictionary.Add(propertyInfo, pvm);
                return pvm;
            }
            if (IsEventAccessor(methodInfo))
            {
                var eventInfo = Helper.GetAccessorEvent(methodInfo.MemberReference);
                if (_viewModelsDictionary.ContainsKey(eventInfo))
                {
                    return _viewModelsDictionary[eventInfo];
                }
                var evm = new EventViewModel(eventInfo);
                _viewModelsDictionary.Add(eventInfo, evm);
                return evm;
            }
            
            var vm = new MethodViewModel(methodInfo);
            _viewModelsDictionary.Add(methodInfo, vm);
            return vm;
        }

        private void OnFillGraphRequest()
        {
            var handler = FillGraphRequest;

            if (handler != null)
            {
                handler();
            }
        }

        private void ApplySelectionCommandHandler()
        {
            IsTypeSelectionVisible = false;
            var types = _hierarchies
                .SelectMany(h => h.Where(t => t.IsSelected).Select(t => t.TypeInfo))
                .Distinct()
                .ToArray();
            Graph = CreateGraph(types);
        }

        private void HideSelectionViewCommandHandler()
        {
            IsTypeSelectionVisible = false;
        }

        private void ShowSelectionViewCommandHandler()
        {
            IsTypeSelectionVisible = true;
        }

        private void OnOriginalSizeRequest()
        {
            var handler = OriginalSizeRequest;

            if (handler != null)
            {
                handler();
            }
        }

        private static bool IsPropertyAccessor(MethodInfo method)
        {
            return (method.IsSpecialName
                    && (method.Name.IndexOf("get_") != -1 || method.Name.IndexOf("set_") != -1));
        }

        private static bool IsEventAccessor(MethodInfo method)
        {
            return (method.IsSpecialName
                    && (method.Name.IndexOf("add_") != -1 || method.Name.IndexOf("remove_") != -1));
        }
    }
}