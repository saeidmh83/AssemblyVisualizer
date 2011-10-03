// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if Reflector

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssemblyVisualizer.Model;
using Reflector.CodeModel;
using Reflector.CodeModel.Memory;

namespace AssemblyVisualizer.HAL.Reflector
{
    class Converter : IConverter
    {
        private Dictionary<IAssembly, AssemblyInfo> _assemblyCorrespondence = new Dictionary<IAssembly, AssemblyInfo>();
        private Dictionary<IModule, ModuleInfo> _moduleCorrespondence = new Dictionary<IModule, ModuleInfo>();
        private Dictionary<ITypeDeclaration, TypeInfo> _typeCorrespondence = new Dictionary<ITypeDeclaration, TypeInfo>();

        public void ClearCache()
        {
            _assemblyCorrespondence.Clear();
            _moduleCorrespondence.Clear();
            _typeCorrespondence.Clear();
        }

        public AssemblyInfo Assembly(object assembly)
        {
            if (assembly == null)
            {
                return null;
            }

            var assemblyDefinition = assembly as IAssembly;

            if (_assemblyCorrespondence.ContainsKey(assemblyDefinition))
            {
                return _assemblyCorrespondence[assemblyDefinition];
            }

            var typeDefinitions = assemblyDefinition.Modules
                .OfType<IModule>()
                .SelectMany(GetTypeDeclarations)
                .Distinct()
                .ToArray();

            string publicKeyToken;
            if (assemblyDefinition.PublicKeyToken == null || assemblyDefinition.PublicKeyToken.Length == 0)
            {
                publicKeyToken = "null";
            }
            else
            {
                var builder = new StringBuilder();
                for (int i = 0; i < assemblyDefinition.PublicKeyToken.Length; i++)
                {
                    builder.Append(assemblyDefinition.PublicKeyToken[i].ToString("x2"));
                }
                publicKeyToken = builder.ToString();
            }

            var culture = string.IsNullOrWhiteSpace(assemblyDefinition.Culture) ? "neutral" : assemblyDefinition.Culture;

            var fullName = string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}",
                assemblyDefinition.Name, assemblyDefinition.Version.ToString(4), culture, publicKeyToken);

            var assemblyInfo = new AssemblyInfo
            {
                Name = assemblyDefinition.Name,
                FullName = fullName,
                ExportedTypesCount = typeDefinitions.Count(
                    t => t.Visibility == TypeVisibility.Public 
                         || t.Visibility == TypeVisibility.NestedPublic
                         || t.Visibility == TypeVisibility.NestedFamilyOrAssembly 
                         || t.Visibility == TypeVisibility.NestedFamily),
                InternalTypesCount = typeDefinitions.Count(
                    t => t.Visibility == TypeVisibility.Private 
                         || t.Visibility == TypeVisibility.NestedPrivate
                         || t.Visibility == TypeVisibility.NestedFamilyAndAssembly 
                         || t.Visibility == TypeVisibility.NestedAssembly),
                Version = assemblyDefinition.Version
            };

            _assemblyCorrespondence.Add(assemblyDefinition, assemblyInfo);
            assemblyInfo.Modules = assemblyDefinition.Modules.OfType<IModule>().Select(m => Module(m)).ToArray();
            foreach (var module in assemblyInfo.Modules)
            {
                if (module.Assembly == null)
                {
                    module.Assembly = assemblyInfo;
                }
            }

            assemblyInfo.ReferencedAssemblies = assemblyDefinition.Modules
                .OfType<IModule>()
                .SelectMany(m => m.AssemblyReferences.OfType<IAssemblyReference>()
                    .Select(r => r.Resolve())
                    .Where(ad => ad != null)
                    .Select(ad => Assembly(ad)));

            return assemblyInfo;
        }
        
        private IEnumerable<ITypeDeclaration> GetTypeDeclarations(IModule module)
        {
            return module.Types
                .OfType<ITypeDeclaration>()
                .SelectMany(t => GetNestedTypesAndSelfRecursive(t));
        }

        private static IEnumerable<ITypeDeclaration> GetNestedTypesAndSelfRecursive(ITypeDeclaration type)
        {
            return new[] { type }.Concat(type.NestedTypes
                .OfType<ITypeDeclaration>()
                .SelectMany(nt => GetNestedTypesAndSelfRecursive(nt)));
        }

        public ModuleInfo Module(IModule module)
        {
            if (_moduleCorrespondence.ContainsKey(module))
            {
                return _moduleCorrespondence[module];
            }

            var moduleInfo = new ModuleInfo();
            _moduleCorrespondence.Add(module, moduleInfo);
            moduleInfo.Assembly = Assembly(module.Assembly);
            moduleInfo.Types = GetTypeDeclarations(module).Select(t => Type(t, moduleInfo));

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
            return Type(type, null);
        }

        public TypeInfo Type(ITypeDeclaration type, ModuleInfo module)
        {
            if (type == null)
            {
                return null;
            }

            if (_typeCorrespondence.ContainsKey(type))
            {
                return _typeCorrespondence[type];
            }

            var methods = type.Methods.OfType<IMethodDeclaration>()
                   .Where(m => !m.Name.Contains("get_")
                               && !m.Name.Contains("set_")
                               && !m.Name.Contains("add_")
                               && !m.Name.Contains("remove_"));

            var typeInfo = new TypeInfo
            {
                BaseTypeRetriever = () => Type(type.BaseType),
                Name = type.Name,
                Events = type.Events.OfType<IEventDeclaration>().Select(e => Event(e)),
                Properties = type.Properties.OfType<IPropertyDeclaration>().Select(p => Property(p)),
                MembersCount = methods.Count() + type.Events.Count + type.Properties.Count + type.Fields.Count,
                IsInternal = type.Visibility == TypeVisibility.Private 
                             || type.Visibility == TypeVisibility.NestedPrivate
                             || type.Visibility == TypeVisibility.NestedFamilyAndAssembly 
                             || type.Visibility == TypeVisibility.NestedAssembly,
                IsPublic = type.Visibility == TypeVisibility.Public 
                           || type.Visibility == TypeVisibility.NestedPublic
                           || type.Visibility == TypeVisibility.NestedFamilyOrAssembly 
                           || type.Visibility == TypeVisibility.NestedFamily,
                MemberReference = type,
                IsEnum = IsEnum(type),
                IsInterface = type.Interface,
                IsValueType = type.ValueType,
                IsSealed = type.Sealed,
                IsAbstract = type.Abstract
            };
            typeInfo.FullName = GetFullName(type.Namespace, typeInfo.Name);
            typeInfo.Methods = methods.Select(m => Method(m, typeInfo));
            typeInfo.Fields = type.Fields.OfType<IFieldDeclaration>().Select(f => Field(f, typeInfo));

            foreach (var eventInfo in typeInfo.Events)
            {
                eventInfo.DeclaringType = typeInfo;
            }
            foreach (var propertyInfo in typeInfo.Properties)
            {
                propertyInfo.DeclaringType = typeInfo;
            }

            _typeCorrespondence.Add(type, typeInfo);
            typeInfo.Module = module ?? Module(GetModuleForType(type));

            typeInfo.Icon = Images.Images.GetTypeIcon(typeInfo);

            return typeInfo;
        }

        private static bool IsEnum(ITypeDeclaration type)
        {
            return type.ValueType && type.BaseType.Name == "Enum" && type.BaseType.Namespace == "System";
        }

        private static IModule GetModuleForType(ITypeDeclaration typeDeclaration)
        {
            var module = typeDeclaration.Owner as IModule;
            if (module != null)
            {
                return module;
            }
            return GetModuleForType(typeDeclaration.Owner as ITypeDeclaration);
        }

        private static string GetFullName(string typeNamespace, string typeName)
        {
            if (string.IsNullOrEmpty(typeNamespace))
            {
                return typeName;
            }
            return string.Format("{0}.{1}", typeNamespace, typeName);
        }

        public EventInfo Event(IEventDeclaration eventDeclaration)
        {
            var addMethod = eventDeclaration.AddMethod.Resolve();

            var eventInfo = new EventInfo
            {
                Text = eventDeclaration.ToString(),
                Name = eventDeclaration.Name,
                FullName = eventDeclaration.Name,
                IsInternal = addMethod.Visibility == MethodVisibility.Assembly,
                IsPrivate = addMethod.Visibility == MethodVisibility.Private,
                IsPublic = addMethod.Visibility == MethodVisibility.Public,
                IsProtected = addMethod.Visibility == MethodVisibility.Family,
                IsProtectedAndInternal = addMethod.Visibility == MethodVisibility.FamilyAndAssembly,
                IsProtectedOrInternal = addMethod.Visibility == MethodVisibility.FamilyOrAssembly,
                IsStatic = addMethod.Static,
                MemberReference = eventDeclaration
            };

            eventInfo.Text = eventInfo.Text.Substring(eventInfo.Text.LastIndexOf('.') + 1);
            eventInfo.Name = eventInfo.Name.Substring(eventInfo.Name.LastIndexOf('.') + 1);

            eventInfo.Icon = Images.Images.GetEventIcon(eventInfo);

            return eventInfo;
        }

        public FieldInfo Field(IFieldDeclaration fieldDefinition, TypeInfo type)
        {
            var fieldInfo = new FieldInfo
            {
                Text = fieldDefinition.ToString(),
                Name = fieldDefinition.Name,
                FullName = fieldDefinition.Name,
                IsInternal = fieldDefinition.Visibility == FieldVisibility.Assembly,
                IsPrivate = fieldDefinition.Visibility == FieldVisibility.Private,
                IsPublic = fieldDefinition.Visibility == FieldVisibility.Public,
                IsProtected = fieldDefinition.Visibility == FieldVisibility.Family,
                IsProtectedAndInternal = fieldDefinition.Visibility == FieldVisibility.FamilyAndAssembly,
                IsProtectedOrInternal = fieldDefinition.Visibility == FieldVisibility.FamilyOrAssembly,
                IsStatic = fieldDefinition.Static,
                IsLiteral = fieldDefinition.Literal,
                IsInitOnly = fieldDefinition.ReadOnly,
                IsSpecialName = fieldDefinition.SpecialName,
                MemberReference = fieldDefinition,
                DeclaringType = type
            };

            fieldInfo.Text = fieldInfo.Text.Substring(fieldInfo.Text.LastIndexOf('.') + 1);
            fieldInfo.Name = fieldInfo.Name.Substring(fieldInfo.Name.LastIndexOf('.') + 1);

            fieldInfo.Icon = Images.Images.GetFieldIcon(fieldInfo);

            return fieldInfo;
        }

        public MethodInfo Method(IMethodDeclaration method, TypeInfo type)
        {
            var methodInfo = new MethodInfo
            {
                Text = method.ToString(),
                Name = method.Name,
                FullName = method.Name,
                IsInternal = method.Visibility == MethodVisibility.Assembly,
                IsPrivate = method.Visibility == MethodVisibility.Private,
                IsPublic = method.Visibility == MethodVisibility.Public,
                IsProtected = method.Visibility == MethodVisibility.Family,
                IsProtectedAndInternal = method.Visibility == MethodVisibility.FamilyAndAssembly,
                IsProtectedOrInternal = method.Visibility == MethodVisibility.FamilyOrAssembly,
                IsVirtual = method.Virtual,
                IsOverride = method.Virtual && !method.NewSlot,
                IsSpecialName = method.SpecialName,
                IsStatic = method.Static,
                IsFinal = method.Final,
                MemberReference = method,
                DeclaringType = type
            };

            if (method.Overrides.Count > 0)
            {
                var overridden = method.Overrides[0];
                var declaringType = overridden.DeclaringType as ITypeReference;
                if (declaringType.Resolve().Interface)
                {
                    methodInfo.IsOverride = false;
                }
            }

            int correction = methodInfo.Text.Contains(".ctor") || methodInfo.Text.Contains(".cctor") ? 0 : 1;
            methodInfo.Text = methodInfo.Text.Substring(methodInfo.Text.LastIndexOf('.') + correction);
            methodInfo.Name = methodInfo.Name.Substring(methodInfo.Name.LastIndexOf('.') + correction);

            methodInfo.Icon = Images.Images.GetMethodIcon(methodInfo);

            return methodInfo;
        }

        public PropertyInfo Property(IPropertyDeclaration propertyDeclaration)
        {
            var getMethod = propertyDeclaration.GetMethod == null ? null : propertyDeclaration.GetMethod.Resolve();
            var setMethod = propertyDeclaration.SetMethod == null ? null : propertyDeclaration.SetMethod.Resolve();

            var propertyInfo = new PropertyInfo
            {
                Text = propertyDeclaration.ToString(),
                Name = propertyDeclaration.Name,
                FullName = propertyDeclaration.Name,                
                IsVirtual = getMethod != null && getMethod.Virtual
                            || setMethod != null && setMethod.Virtual,
                IsOverride = getMethod != null
                             && getMethod.Virtual
                             && !getMethod.NewSlot
                             || setMethod != null
                             && setMethod.Virtual
                             && !setMethod.NewSlot,
                IsStatic = getMethod != null && getMethod.Static
                           || setMethod != null && setMethod.Static,
                IsFinal = getMethod != null && getMethod.Final
                          || setMethod != null && setMethod.Final,
                MemberReference = propertyDeclaration
            };
            AdjustPropertyVisibility(propertyInfo, propertyDeclaration);

            propertyInfo.Icon = Images.Images.GetPropertyIcon(propertyInfo);

            return propertyInfo;
        }

        private void AdjustPropertyVisibility(PropertyInfo propertyInfo, IPropertyDeclaration propertyDeclaration)
        {
            var firstAccessor = propertyDeclaration.GetMethod == null ? null : propertyDeclaration.GetMethod.Resolve();
            var secondAccessor = propertyDeclaration.SetMethod == null ? null : propertyDeclaration.SetMethod.Resolve();
            if (firstAccessor == null)
            {
                firstAccessor = secondAccessor;
            }
            else if (secondAccessor == null)
            {
                secondAccessor = firstAccessor;
            }

            if (firstAccessor.Visibility == MethodVisibility.Public || secondAccessor.Visibility == MethodVisibility.Public)
            {
                propertyInfo.IsPublic = true;
                return;
            }
            if (firstAccessor.Visibility == MethodVisibility.Family || secondAccessor.Visibility == MethodVisibility.Family)
            {
                propertyInfo.IsProtected = true;
                return;
            }
            if (firstAccessor.Visibility == MethodVisibility.FamilyOrAssembly || secondAccessor.Visibility == MethodVisibility.FamilyOrAssembly)
            {
                propertyInfo.IsProtectedOrInternal = true;
                return;
            }
            if (firstAccessor.Visibility == MethodVisibility.Assembly || secondAccessor.Visibility == MethodVisibility.Assembly)
            {
                propertyInfo.IsInternal = true;
                return;
            }
            if (firstAccessor.Visibility == MethodVisibility.FamilyAndAssembly || secondAccessor.Visibility == MethodVisibility.FamilyAndAssembly)
            {
                propertyInfo.IsProtectedAndInternal = true;
                return;
            }
            propertyInfo.IsPrivate = true;
        }

        public MethodInfo Method(object method)
        {
            return Method(method as IMethodDeclaration);
        }

        public FieldInfo Field(object field)
        {
            return Field(field as IFieldDeclaration);
        }

        public PropertyInfo Property(object property)
        {
            return Property(property as IPropertyDeclaration);
        }

        public EventInfo Event(object ev)
        {
            return Event(ev as EventDeclaration);
        }

    }
}

#endif