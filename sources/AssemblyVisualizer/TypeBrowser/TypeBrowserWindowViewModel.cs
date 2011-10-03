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

namespace AssemblyVisualizer.TypeBrowser
{
    class TypeBrowserWindowViewModel : ViewModelBase
    {
        private Dictionary<MemberInfo, MemberViewModel> _viewModelsDictionary = new Dictionary<MemberInfo, MemberViewModel>();
        private MemberGraph _graph;
        private TypeInfo _typeInfo;

        public TypeBrowserWindowViewModel(TypeInfo typeInfo)
        {
            _typeInfo = typeInfo;

            Commands = new ObservableCollection<UserCommand>
			           	{
			           		new UserCommand(Resources.FillGraph, OnFillGraphRequest),
			           		new UserCommand(Resources.OriginalSize, OnOriginalSizeRequest),	
                            //new UserCommand(Resources.SearchInGraph, ShowSearchCommand),                                                    
			           	};

            Graph = CreateGraph(typeInfo);
        }

        public event Action FillGraphRequest;
        public event Action OriginalSizeRequest;
        //public event Action FocusSearchRequest;

        public IEnumerable<UserCommand> Commands { get; private set; }

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

        public string Title
        {
            get
            {
                return _typeInfo.Name;
            }
        }

        private MemberGraph CreateGraph(TypeInfo typeInfo)
        {
            var graph = new MemberGraph(true);

            var hierarchy = new List<TypeInfo>();
            var currentType = typeInfo;
            hierarchy.Add(typeInfo);
            while (currentType.BaseType != null)
            {
                var t = currentType.BaseType;
                hierarchy.Add(t);
                currentType = t;
            }

            foreach (var type in hierarchy)
            {
                foreach (var method in type.Methods.Where(m => !m.Name.StartsWith("<")).Concat(type.Accessors))
                {
                    var mvm = GetViewModelForMethod(method);
                    if (!graph.ContainsVertex(mvm))
                    {
                        graph.AddVertex(mvm);
                    }

                    var usedMethods = Helper.GetUsedMethods(method.MemberReference)
                        .Where(m => hierarchy.Any(t => t.FullName == m.DeclaringType.FullName) && !m.Name.StartsWith("<"))
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
                        .Where(m => hierarchy.Contains(m.DeclaringType) && !m.Name.StartsWith("CS$") && !m.Name.EndsWith("k__BackingField"))
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