// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if ILSpy

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.HAL.ILSpy
{
    class Converter : IConverter
    {
        private Dictionary<AssemblyDefinition, AssemblyInfo> _assemblyCorrespondence = new Dictionary<AssemblyDefinition, AssemblyInfo>();
        private Dictionary<ModuleDefinition, ModuleInfo> _moduleCorrespondence = new Dictionary<ModuleDefinition, ModuleInfo>();
        private Dictionary<TypeDefinition, TypeInfo> _typeCorrespondence = new Dictionary<TypeDefinition, TypeInfo>();

        public void ClearCache()
        {
            _assemblyCorrespondence.Clear();
            _moduleCorrespondence.Clear();
            _typeCorrespondence.Clear();
        }

        public AssemblyInfo Assembly(object assembly)
        {
            var assemblyDefinition = assembly as AssemblyDefinition;

            if (_assemblyCorrespondence.ContainsKey(assemblyDefinition))
            {
                return _assemblyCorrespondence[assemblyDefinition];
            }

            var typeDefinitions = assemblyDefinition.Modules.SelectMany(m => m.Types.SelectMany(t => GetNestedTypesAndSelfRecursive(t))).ToArray();
            
            var assemblyInfo = new AssemblyInfo
            {
                Name = assemblyDefinition.Name.Name,
                FullName = assemblyDefinition.FullName,
                ExportedTypesCount = typeDefinitions.Count(
                    t => t.IsPublic 
                         || t.IsNestedPublic 
                         || t.IsNestedFamilyOrAssembly 
                         || t.IsNestedFamily),
                InternalTypesCount = typeDefinitions.Count(
                    t => t.IsNotPublic 
                         || t.IsNestedAssembly 
                         || t.IsNestedFamilyAndAssembly 
                         || t.IsNestedPrivate),
                Version = assemblyDefinition.Name.Version
            };

            _assemblyCorrespondence.Add(assemblyDefinition, assemblyInfo);
            assemblyInfo.Modules = assemblyDefinition.Modules.Select(m => Module(m));
            assemblyInfo.ReferencedAssemblies = assemblyDefinition.Modules
                .SelectMany(m => m.AssemblyReferences
                    .Select(r => m.AssemblyResolver.Resolve(r))
                    .Where(ad => ad != null)
                    .Select(ad => Assembly(ad)));

            return assemblyInfo;
        }        
        
        public ModuleInfo Module(ModuleDefinition moduleDefinition)
        {
            if (_moduleCorrespondence.ContainsKey(moduleDefinition))
            {
                return _moduleCorrespondence[moduleDefinition];
            }

            var moduleInfo = new ModuleInfo
            {
                Assembly = Assembly(moduleDefinition.Assembly)
            };
            _moduleCorrespondence.Add(moduleDefinition, moduleInfo);            
            
            moduleInfo.Types = moduleDefinition.Types
                .SelectMany(t => GetNestedTypesAndSelfRecursive(t).Select(type => Type(type)));

            return moduleInfo;
        }

        private static IEnumerable<TypeDefinition> GetNestedTypesAndSelfRecursive(TypeDefinition type)
        {
            return new[] { type }.Concat(type.NestedTypes.SelectMany(nt => GetNestedTypesAndSelfRecursive(nt)));            
        }

        public TypeInfo Type(object type)
        {
            var typeReference = type as TypeReference;
            if (typeReference != null)
            {
                return Type(typeReference);
            }

            var typeDefinition = type as TypeDefinition;
            return Type(typeDefinition);
        }
        
        public TypeInfo Type(TypeReference typeReference)
        {
            if (typeReference == null)
            {
                return null;
            }

            var typeDefinition = typeReference.Resolve();
            return Type(typeDefinition);
        }

        public TypeInfo Type(TypeDefinition typeDefinition)
        {
            if (typeDefinition == null)
            {
                return null;
            }

            if (_typeCorrespondence.ContainsKey(typeDefinition))
            {
                return _typeCorrespondence[typeDefinition];
            }

            var methods = typeDefinition.Methods
                    .Where(m => !m.IsGetter && !m.IsSetter && !m.IsAddOn && !m.IsRemoveOn);

            var typeInfo = new TypeInfo
            {
                BaseTypeRetriever = () => Type(typeDefinition.BaseType),
                DeclaringType = Type(typeDefinition.DeclaringType),
                Name = MainWindow.Instance.CurrentLanguage.FormatTypeName(typeDefinition),
                Icon = TypeTreeNode.GetIcon(typeDefinition),
                Events = typeDefinition.Events.Select(e => Event(e)),
                Fields = typeDefinition.Fields.Select(f => Field(f)),
                Methods = methods.Select(m => Method(m)),
                Properties = typeDefinition.Properties.Select(p => Property(p)),
                MembersCount = methods.Count() + typeDefinition.Events.Count + typeDefinition.Properties.Count + typeDefinition.Fields.Count,
                IsInternal = typeDefinition.IsNotPublic 
                             || typeDefinition.IsNestedAssembly 
                             || typeDefinition.IsNestedFamilyAndAssembly 
                             || typeDefinition.IsNestedPrivate,
                IsPublic = typeDefinition.IsPublic 
                           || typeDefinition.IsNestedPublic 
                           || typeDefinition.IsNestedFamilyOrAssembly 
                           || typeDefinition.IsNestedFamily,
                MemberReference = typeDefinition,
                IsEnum = typeDefinition.IsEnum,
                IsInterface = typeDefinition.IsInterface,
                IsValueType = typeDefinition.IsValueType,
                IsSealed = typeDefinition.IsSealed,
                IsAbstract = typeDefinition.IsAbstract
            };
            typeInfo.FullName = GetFullName(typeDefinition.Namespace, typeInfo.Name);

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

            _typeCorrespondence.Add(typeDefinition, typeInfo);

            typeInfo.Module = Module(typeDefinition.Module);

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

        public EventInfo Event(EventDefinition eventDefinition)
        {
            var eventInfo = new EventInfo
            {
                Text = EventTreeNode.GetText(eventDefinition, MainWindow.Instance.CurrentLanguage) as string,
                Name = eventDefinition.Name,
                FullName = eventDefinition.FullName,
                Icon = EventTreeNode.GetIcon(eventDefinition),
                IsInternal = eventDefinition.AddMethod.IsAssembly,
                IsPrivate = eventDefinition.AddMethod.IsPrivate,
                IsPublic = eventDefinition.AddMethod.IsPublic,
                IsProtected = eventDefinition.AddMethod.IsFamily,
                IsProtectedAndInternal = eventDefinition.AddMethod.IsFamilyAndAssembly,
                IsProtectedOrInternal = eventDefinition.AddMethod.IsFamilyOrAssembly,
                IsStatic = eventDefinition.AddMethod.IsStatic,
                MemberReference = eventDefinition
            };

            return eventInfo;
        }

        public FieldInfo Field(FieldDefinition fieldDefinition)
        {
            var fieldInfo = new FieldInfo
            {
                Text = fieldDefinition.Name,
                Name = fieldDefinition.Name,
                FullName = fieldDefinition.FullName,
                Icon = FieldTreeNode.GetIcon(fieldDefinition),
                IsInternal = fieldDefinition.IsAssembly,
                IsPrivate = fieldDefinition.IsPrivate,
                IsPublic = fieldDefinition.IsPublic,
                IsProtected = fieldDefinition.IsFamily,
                IsProtectedAndInternal = fieldDefinition.IsFamilyAndAssembly,
                IsProtectedOrInternal = fieldDefinition.IsFamilyOrAssembly,
                IsStatic = fieldDefinition.IsStatic,
                IsLiteral = fieldDefinition.IsLiteral,
                IsInitOnly = fieldDefinition.IsInitOnly,
                IsSpecialName = fieldDefinition.IsSpecialName,
                MemberReference = fieldDefinition
            };

            return fieldInfo;
        }

        public MethodInfo Method(MethodDefinition methodDefinition)
        {
            var methodInfo = new MethodInfo
            {
                Text = MethodTreeNode.GetText(methodDefinition, MainWindow.Instance.CurrentLanguage) as string,
                Name = methodDefinition.Name,
                FullName = methodDefinition.FullName,
                Icon = MethodTreeNode.GetIcon(methodDefinition),
                IsInternal = methodDefinition.IsAssembly,
                IsPrivate = methodDefinition.IsPrivate,
                IsPublic = methodDefinition.IsPublic,
                IsProtected = methodDefinition.IsFamily,
                IsProtectedAndInternal = methodDefinition.IsFamilyAndAssembly,
                IsProtectedOrInternal = methodDefinition.IsFamilyOrAssembly,
                IsVirtual = methodDefinition.IsVirtual,
                IsSpecialName = methodDefinition.IsSpecialName,
                IsOverride = IsOverride(methodDefinition),
                IsStatic = methodDefinition.IsStatic,
                IsFinal = methodDefinition.IsFinal,
                MemberReference = methodDefinition
            };

            return methodInfo;
        }

        private bool IsOverride(MethodDefinition methodDefinition)
        {
            if (methodDefinition.DeclaringType.FullName == "System.Object")
            {
                // Method System.Object.Finalize() looks like override in IL
                return false;
            }
            return methodDefinition.IsVirtual && !methodDefinition.IsNewSlot;
        }

        public PropertyInfo Property(PropertyDefinition propertyDefinition)
        {
            var propertyInfo = new PropertyInfo
            {
                Text = PropertyTreeNode.GetText(propertyDefinition, MainWindow.Instance.CurrentLanguage) as string,
                Name = propertyDefinition.Name,
                FullName = propertyDefinition.FullName,
                Icon = PropertyTreeNode.GetIcon(propertyDefinition),
                IsPublic = propertyDefinition.GetMethod != null
                           && propertyDefinition.GetMethod.IsPublic
                           || propertyDefinition.SetMethod != null
                           && propertyDefinition.SetMethod.IsPublic,
                IsProtected = propertyDefinition.GetMethod != null
                              && propertyDefinition.GetMethod.IsFamily
                              || propertyDefinition.SetMethod != null
                              && propertyDefinition.SetMethod.IsFamily,
                IsInternal = propertyDefinition.GetMethod != null
                             && propertyDefinition.GetMethod.IsAssembly
                             || propertyDefinition.SetMethod != null
                             && propertyDefinition.SetMethod.IsAssembly,
                IsPrivate = propertyDefinition.GetMethod != null
                            && propertyDefinition.GetMethod.IsPrivate
                            || propertyDefinition.SetMethod != null
                            && propertyDefinition.SetMethod.IsPrivate,
                IsProtectedOrInternal = propertyDefinition.GetMethod != null
                                        && propertyDefinition.GetMethod.IsFamilyOrAssembly
                                        || propertyDefinition.SetMethod != null
                                        && propertyDefinition.SetMethod.IsFamilyOrAssembly,
                IsProtectedAndInternal = propertyDefinition.GetMethod != null
                                         && propertyDefinition.GetMethod.IsFamilyAndAssembly
                                         || propertyDefinition.SetMethod != null
                                         && propertyDefinition.SetMethod.IsFamilyAndAssembly,
                IsVirtual = propertyDefinition.GetMethod != null && propertyDefinition.GetMethod.IsVirtual
                            || propertyDefinition.SetMethod != null && propertyDefinition.SetMethod.IsVirtual,
                IsOverride = propertyDefinition.GetMethod != null
                             && propertyDefinition.GetMethod.IsVirtual
                             && !propertyDefinition.GetMethod.IsNewSlot
                             || propertyDefinition.SetMethod != null
                             && propertyDefinition.SetMethod.IsVirtual
                             && !propertyDefinition.SetMethod.IsNewSlot,
                IsStatic = propertyDefinition.GetMethod != null && propertyDefinition.GetMethod.IsStatic
                            || propertyDefinition.SetMethod != null && propertyDefinition.SetMethod.IsStatic,
                IsFinal = propertyDefinition.GetMethod != null && propertyDefinition.GetMethod.IsFinal
                            || propertyDefinition.SetMethod != null && propertyDefinition.SetMethod.IsFinal,
                MemberReference = propertyDefinition
            };

            return propertyInfo;
        }
    }
}

#endif