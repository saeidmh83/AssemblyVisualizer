// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if Reflector

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILSpyVisualizer.Model;
using Reflector.CodeModel;
using Reflector.CodeModel.Memory;

namespace ILSpyVisualizer.HAL.Reflector
{
    class Converter : IConverter
    {
        private Dictionary<IAssembly, AssemblyInfo> _assemblyCorrespondence = new Dictionary<IAssembly, AssemblyInfo>();
        private Dictionary<IModule, ModuleInfo> _moduleCorrespondence = new Dictionary<IModule, ModuleInfo>();
        private Dictionary<ITypeDeclaration, TypeInfo> _typeCorrespondence = new Dictionary<ITypeDeclaration, TypeInfo>();

        public AssemblyInfo Assembly(object assembly)
        {
            var assemblyDefinition = assembly as IAssembly;

            if (_assemblyCorrespondence.ContainsKey(assemblyDefinition))
            {
                return _assemblyCorrespondence[assemblyDefinition];
            }

            var typeDefinitions = assemblyDefinition.Modules.OfType<IModule>().SelectMany(GetTypeDeclarations).ToArray();

            var assemblyInfo = new AssemblyInfo
            {
                Name = assemblyDefinition.Name,
                FullName = assemblyDefinition.Name, // TODO: make real FullName
                ExportedTypesCount = typeDefinitions.Count(t => t.Visibility == TypeVisibility.Public),
                InternalTypesCount = typeDefinitions.Count(t => t.Visibility == TypeVisibility.Private)
            };

            _assemblyCorrespondence.Add(assemblyDefinition, assemblyInfo);
            assemblyInfo.Modules = assemblyDefinition.Modules.OfType<IModule>().Select(m => Module(m));

            return assemblyInfo;
        }

        private IEnumerable<ITypeDeclaration> GetTypeDeclarations(IModule module)
        {
            return module.Types.OfType<ITypeDeclaration>();
        }
        
        public ModuleInfo Module(IModule module)
        {
            if (_moduleCorrespondence.ContainsKey(module))
            {
                return _moduleCorrespondence[module];
            }

            var moduleInfo = new ModuleInfo
            {
                Assembly = Assembly(module.Assembly)
            };
            _moduleCorrespondence.Add(module, moduleInfo);
            moduleInfo.Types = module.Types.OfType<ITypeDeclaration>().Select(t => Type(t));

            return moduleInfo;
        }

        public TypeInfo Type(object type)
        {
            var typeReference = type as ITypeReference;
            if (typeReference != null)
            {
                return Type(typeReference);
            }

            var typeDefinition = type as ITypeDeclaration;
            return Type(typeDefinition);
        }
        
        public TypeInfo Type(ITypeReference typeReference)
        {
            if (typeReference == null)
            {
                return null;
            }

            var typeDeclaration = typeReference.Resolve();
            return Type(typeDeclaration);
        }

        public TypeInfo Type(ITypeDeclaration type)
        {
            if (type == null)
            {
                return null;
            }

            if (_typeCorrespondence.ContainsKey(type))
            {
                return _typeCorrespondence[type];
            }

            var methods = type.Methods.OfType<IMethodDeclaration>();
                   // .Where(m => !m..IsGetter && !m.IsSetter && !m.IsAddOn && !m.IsRemoveOn);

            var typeInfo = new TypeInfo
            {
                BaseTypeRetriever = () => Type(type.BaseType),
                //DeclaringType = Type(type.d),
                Name = type.Name,
                //Icon = type.ico,
                Events = type.Events.OfType<IEventDeclaration>().Select(e => Event(e)),
                Fields = type.Fields.OfType<IFieldDeclaration>().Select(f => Field(f)),
                Methods = methods.Select(m => Method(m)),
                Properties = type.Properties.OfType<IPropertyDeclaration>().Select(p => Property(p)),
                MembersCount = methods.Count() + type.Events.Count + type.Properties.Count + type.Fields.Count,
                IsInternal = type.Visibility != TypeVisibility.Public,
                IsPublic = type.Visibility == TypeVisibility.Public,
                MemberReference = type,
                //IsEnum = type.Enum,
                IsInterface = type.Interface,
                IsValueType = type.ValueType
            };
            typeInfo.FullName = GetFullName(type.Namespace, typeInfo.Name);

            foreach (var eventInfo in typeInfo.Events)
            {
                eventInfo.DeclaringType = typeInfo;
            }
            foreach (var methodInfo in typeInfo.Methods)
            {
                methodInfo.DeclaringType = typeInfo;
            }
            foreach (var fieldInfo in typeInfo.Fields)
            {
                fieldInfo.DeclaringType = typeInfo;
            }
            foreach (var propertyInfo in typeInfo.Properties)
            {
                propertyInfo.DeclaringType = typeInfo;
            }
                      
            // TODO: find a module
            //typeInfo.Module = Module(type.Module);

            return typeInfo;
        }

        private static string GetFullName(string typeNamespace, string typeName)
        {
            if (string.IsNullOrEmpty(typeNamespace))
            {
                return typeName;
            }
            return string.Format("{0}.{1}", typeNamespace, typeName);
        }

        public EventInfo Event(IEventDeclaration ieventDeclaration)
        {
            var eventDeclaration = ieventDeclaration as EventDeclaration;
            var addMethod = eventDeclaration.AddMethod as MethodDeclaration;

            var eventInfo = new EventInfo
            {
                Text = eventDeclaration.Name,
                Name = eventDeclaration.Name,
                //FullName = eventDefinition.,
                //Icon = EventTreeNode.GetIcon(eventDeclaration),
                IsInternal = addMethod.Visibility == MethodVisibility.Assembly,
                IsPrivate = addMethod.Visibility == MethodVisibility.Private,
                IsPublic = addMethod.Visibility == MethodVisibility.Public,
                IsProtected = addMethod.Visibility == MethodVisibility.Family,
                IsProtectedAndInternal = addMethod.Visibility == MethodVisibility.FamilyAndAssembly,
                IsProtectedOrInternal = addMethod.Visibility == MethodVisibility.FamilyOrAssembly,
                MemberReference = eventDeclaration
            };

            return eventInfo;
        }

        public FieldInfo Field(IFieldDeclaration fieldDefinition)
        {
            var fieldInfo = new FieldInfo
            {
                Text = fieldDefinition.Name,
                Name = fieldDefinition.Name,
                //FullName = fieldDefinition.FullName,
                //Icon = FieldTreeNode.GetIcon(fieldDefinition),
                IsInternal = fieldDefinition.Visibility == FieldVisibility.Assembly,
                IsPrivate = fieldDefinition.Visibility == FieldVisibility.Private,
                IsPublic = fieldDefinition.Visibility == FieldVisibility.Public,
                IsProtected = fieldDefinition.Visibility == FieldVisibility.Family,
                IsProtectedAndInternal = fieldDefinition.Visibility == FieldVisibility.FamilyAndAssembly,
                IsProtectedOrInternal = fieldDefinition.Visibility == FieldVisibility.FamilyOrAssembly,
                MemberReference = fieldDefinition
            };

            return fieldInfo;
        }

        public MethodInfo Method(IMethodDeclaration method)
        {
            var methodInfo = new MethodInfo
            {
                //Text = MethodTreeNode.GetText(methodDefinition, MainWindow.Instance.CurrentLanguage) as string,
                Text = method.Name,
                Name = method.Name,
                //FullName = methodDefinition.FullName,
                //Icon = MethodTreeNode.GetIcon(methodDefinition),
                IsInternal = method.Visibility == MethodVisibility.Assembly,
                IsPrivate = method.Visibility == MethodVisibility.Private,
                IsPublic = method.Visibility == MethodVisibility.Public,
                IsProtected = method.Visibility == MethodVisibility.Family,
                IsProtectedAndInternal = method.Visibility == MethodVisibility.FamilyAndAssembly,
                IsProtectedOrInternal = method.Visibility == MethodVisibility.FamilyOrAssembly,
                IsVirtual = method.Virtual,
                IsOverride = method.Overrides != null && method.Overrides.Count > 0,
                MemberReference = method
            };

            return methodInfo;
        }

        /*private bool IsOverride(MethodDefinition methodDefinition)
        {
            if (methodDefinition.DeclaringType.FullName == "System.Object")
            {
                // Method System.Object.Finalize() looks like override in IL
                return false;
            }
            return methodDefinition.IsVirtual && !methodDefinition.IsNewSlot;
        }*/

        public PropertyInfo Property(IPropertyDeclaration propertyDefinition)
        {            
            var getMethod =  propertyDefinition.GetMethod == null ? null : propertyDefinition.GetMethod.Resolve();
            var setMethod = propertyDefinition.SetMethod == null ? null : propertyDefinition.SetMethod.Resolve();

            var propertyInfo = new PropertyInfo
            {
                //Text = PropertyTreeNode.GetText(propertyDefinition, MainWindow.Instance.CurrentLanguage) as string,
                Text = propertyDefinition.Name,
                Name = propertyDefinition.Name,
                //FullName = propertyDefinition.FullName,
                //Icon = PropertyTreeNode.GetIcon(propertyDefinition),
                IsPublic = getMethod != null
                           && getMethod.Visibility == MethodVisibility.Public
                           || setMethod != null
                           && setMethod.Visibility == MethodVisibility.Public,
                IsProtected = getMethod != null
                              && getMethod.Visibility == MethodVisibility.Family
                              || setMethod != null
                              && setMethod.Visibility == MethodVisibility.Family,
                IsInternal = getMethod != null
                             && getMethod.Visibility == MethodVisibility.Assembly
                             || setMethod != null
                             && setMethod.Visibility == MethodVisibility.Assembly,
                IsPrivate = getMethod != null
                            && getMethod.Visibility == MethodVisibility.Private
                            || setMethod != null
                            && setMethod.Visibility == MethodVisibility.Private,
                IsProtectedOrInternal = getMethod != null
                                        && getMethod.Visibility == MethodVisibility.FamilyOrAssembly
                                        || setMethod != null
                                        && setMethod.Visibility == MethodVisibility.FamilyOrAssembly,
                IsProtectedAndInternal = getMethod != null
                                         && getMethod.Visibility == MethodVisibility.FamilyAndAssembly
                                         || setMethod != null
                                         && setMethod.Visibility == MethodVisibility.FamilyAndAssembly,
                IsVirtual = getMethod != null && getMethod.Virtual
                            || setMethod != null && setMethod.Virtual,
                IsOverride = getMethod != null
                             && getMethod.Virtual
                             && !getMethod.NewSlot
                             || setMethod != null
                             && setMethod.Virtual
                             && !setMethod.NewSlot,
                MemberReference = propertyDefinition
            };

            return propertyInfo;
        }
    }
}

#endif