// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if ILSpy
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mono.Cecil;
using AssemblyVisualizer.Common;
using AssemblyVisualizer.HAL;

namespace AssemblyVisualizer.TypeBrowser
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    partial class TypeBrowserWindow : Window
    {
        public TypeBrowserWindow(TypeDefinition typeDefinition)
        {
            InitializeComponent();
            WindowManager.AddTypeBrowser(this);
            
            graphLayout.Graph = CreateGraph(typeDefinition);

        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            WindowManager.RemoveTypeBrowser(this);
        }

        private Dictionary<MemberReference, MemberViewModel> _members = new Dictionary<MemberReference, MemberViewModel>();

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
                    var mvm = GetVMOrCreate(method);
                    if (!graph.ContainsVertex(mvm))
                    {
                        graph.AddVertex(mvm);
                    }

                    var uses = AnalyzerEngine.GetUsedMethods(method).Where(m => list.Contains(m.DeclaringType) && !m.Name.StartsWith("<")).ToArray();
                    foreach (var usage in uses)
                    {
                        var vm = GetVMOrCreate(usage);
                        if (!graph.ContainsVertex(vm))
                        {
                            graph.AddVertex(vm);
                        }
                        graph.AddEdge(new Controls.Graph.QuickGraph.Edge<MemberViewModel>(mvm, vm));
                    }

                    var usesf = AnalyzerEngine.GetUsedFields(method).Where(m => list.Contains(m.DeclaringType) && !m.Name.StartsWith("CS$")).ToArray();
                    foreach (var usage in usesf)
                    {
                        var vm = GetVMOrCreate(usage);
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

        private MemberViewModel GetVMOrCreate(MemberReference reference)
        {
            if (_members.ContainsKey(reference))
            {
                return _members[reference];
            }

            if (reference is FieldDefinition)
            {
                var info = Converter.Field(reference);
                var vm = new FieldViewModel(info);
                _members.Add(reference, vm);
                return vm;
            }            

            if (reference is MethodDefinition)
            {
                var methodDef = reference as MethodDefinition;
                if (IsAccessor(methodDef))
                {
                    var prop = GetAccessorProperty(methodDef);
                    var pinfo = Converter.Property(prop);
                    
                    if (!_members.ContainsKey(prop))
                    { 
                        var pvm = new PropertyViewModel(pinfo);
                        _members.Add(prop, pvm);
                    }

                    return _members[prop];
                }

                var info = Converter.Method(reference);
                var vm = new MethodViewModel(info);
                _members.Add(reference, vm);
                return vm;
            }


            throw new Exception("error");
        }

        private bool IsAccessor(MethodDefinition methodDefinition)
        {
            return (methodDefinition.IsSpecialName && (methodDefinition.Name.StartsWith("get_") || methodDefinition.Name.StartsWith("set_")));
        }

        private PropertyDefinition GetAccessorProperty(MethodDefinition methodDefinition)
        {
            var type = methodDefinition.DeclaringType;
            var propName = methodDefinition.Name.Substring(4);
            var prop = type.Properties.Single(p => p.Name == propName);
            return prop;
        }
    }
}
#endif