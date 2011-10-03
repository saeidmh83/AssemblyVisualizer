// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if ILSpy
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Infrastructure;
using AssemblyVisualizer.Common;
using Mono.Cecil;
using AssemblyVisualizer.HAL;
using System.Collections.ObjectModel;
using AssemblyVisualizer.Properties;

namespace AssemblyVisualizer.TypeBrowser
{
    class TypeBrowserWindowViewModel : ViewModelBase
    {
        private Dictionary<MemberReference, MemberViewModel> _viewModelsDictionary = new Dictionary<MemberReference, MemberViewModel>();
        private MemberGraph _graph;
        private TypeDefinition _typeDefinition;

        public TypeBrowserWindowViewModel(TypeDefinition typeDefinition)
        {
            _typeDefinition = typeDefinition;

            Commands = new ObservableCollection<UserCommand>
			           	{
			           		new UserCommand(Resources.FillGraph, OnFillGraphRequest),
			           		new UserCommand(Resources.OriginalSize, OnOriginalSizeRequest),	
                            //new UserCommand(Resources.SearchInGraph, ShowSearchCommand),                                                    
			           	};        

            Graph = CreateGraph(typeDefinition);
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
                return _typeDefinition.Name;
            }
        }

        private MemberGraph CreateGraph(TypeDefinition typeDefinition)
        {
            var graph = new MemberGraph(true);

            var list = new List<TypeDefinition>();
            var currentType = typeDefinition;
            list.Add(typeDefinition);
            while (currentType.BaseType != null)
            {
                var t = currentType.BaseType.Resolve();
                list.Add(t);
                currentType = t;
            }

            foreach (var type in list)
            {
                foreach (var method in type.Methods.Where(m => !m.Name.StartsWith("<")))
                {
                    var mvm = GetViewModel(method);
                    if (!graph.ContainsVertex(mvm))
                    {
                        graph.AddVertex(mvm);
                    }

                    var uses = AnalyzerEngine.GetUsedMethods(method).Where(m => list.Contains(m.DeclaringType) && !m.Name.StartsWith("<")).ToArray();
                    foreach (var usage in uses)
                    {
                        var vm = GetViewModel(usage);
                        if (!graph.ContainsVertex(vm))
                        {
                            graph.AddVertex(vm);
                        }
                        graph.AddEdge(new Controls.Graph.QuickGraph.Edge<MemberViewModel>(mvm, vm));
                    }

                    var usesf = AnalyzerEngine.GetUsedFields(method).Where(m => list.Contains(m.DeclaringType) && !m.Name.StartsWith("CS$") && !m.Name.EndsWith("k__BackingField")).ToArray();
                    foreach (var usage in usesf)
                    {
                        var vm = GetViewModel(usage);
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

        private MemberViewModel GetViewModel(MemberReference memberReference)
        {
            if (_viewModelsDictionary.ContainsKey(memberReference))
            {
                return _viewModelsDictionary[memberReference];
            }

            if (memberReference is FieldDefinition)
            {
                var ev = GetEventForBackingField(memberReference as FieldDefinition);
                if (ev != null)
                {
                    var einfo = Converter.Event(ev);

                    if (!_viewModelsDictionary.ContainsKey(ev))
                    {
                        var evm = new EventViewModel(einfo);
                        _viewModelsDictionary.Add(ev, evm);
                    }

                    return _viewModelsDictionary[ev];
                }
                var info = Converter.Field(memberReference);
                var vm = new FieldViewModel(info);
                _viewModelsDictionary.Add(memberReference, vm);
                return vm;
            }

            if (memberReference is MethodDefinition)
            {
                var methodDef = memberReference as MethodDefinition;
                if (IsPropertyAccessor(methodDef))
                {
                    var prop = GetAccessorProperty(methodDef);
                    var pinfo = Converter.Property(prop);

                    if (!_viewModelsDictionary.ContainsKey(prop))
                    {
                        var pvm = new PropertyViewModel(pinfo);
                        _viewModelsDictionary.Add(prop, pvm);
                    }

                    return _viewModelsDictionary[prop];
                }
                if (IsEventAccessor(methodDef))
                {
                    var ev = GetAccessorEvent(methodDef);
                    var einfo = Converter.Event(ev);

                    if (!_viewModelsDictionary.ContainsKey(ev))
                    {
                        var evm = new EventViewModel(einfo);
                        _viewModelsDictionary.Add(ev, evm);
                    }

                    return _viewModelsDictionary[ev];
                }

                var info = Converter.Method(memberReference);
                var vm = new MethodViewModel(info);
                _viewModelsDictionary.Add(memberReference, vm);
                return vm;
            }

            throw new Exception("Invalid member reference");
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

        private static bool IsPropertyAccessor(MethodDefinition methodDefinition)
        {
            return (methodDefinition.IsSpecialName
                    && (methodDefinition.Name.IndexOf("get_") != -1 || methodDefinition.Name.IndexOf("set_") != -1));
        }

        private static bool IsEventAccessor(MethodDefinition methodDefinition)
        {
            return (methodDefinition.IsSpecialName
                    && (methodDefinition.Name.IndexOf("add_") != -1 || methodDefinition.Name.IndexOf("remove_") != -1));
        }

        private static EventDefinition GetEventForBackingField(FieldDefinition fieldDefinition)
        {
            return fieldDefinition.DeclaringType.Events.FirstOrDefault(e => e.Name == fieldDefinition.Name && e.EventType == fieldDefinition.FieldType);
        }

        private static PropertyDefinition GetAccessorProperty(MethodDefinition methodDefinition)
        {
            var type = methodDefinition.DeclaringType;
            var prop = type.Properties.Single(p => p.GetMethod == methodDefinition || p.SetMethod == methodDefinition);
            return prop;
        }

        private static EventDefinition GetAccessorEvent(MethodDefinition methodDefinition)
        {
            var type = methodDefinition.DeclaringType;
            var ev = type.Events.Single(p => p.AddMethod == methodDefinition || p.RemoveMethod == methodDefinition);
            return ev;
        }
    }
}
#endif