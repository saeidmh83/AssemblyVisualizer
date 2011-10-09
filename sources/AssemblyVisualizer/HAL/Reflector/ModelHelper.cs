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

namespace AssemblyVisualizer.HAL.Reflector
{
    class ModelHelper
    {
        public EventInfo GetEventForBackingField(object field)
        {
            var fieldDefinition = field as IFieldDeclaration;
            var type = fieldDefinition.DeclaringType as ITypeDeclaration;
            var eventDefinition = type.Events.OfType<IEventDeclaration>().FirstOrDefault(e => e.Name == fieldDefinition.Name && e.EventType == fieldDefinition.FieldType);
           
            return eventDefinition == null ? null : HAL.Converter.Event(eventDefinition);
        }

        public PropertyInfo GetAccessorProperty(object method)
        {
            var methodDeclaration = method as IMethodDeclaration;
            var type = methodDeclaration.DeclaringType as ITypeDeclaration;
            var property = type.Properties.OfType<IPropertyDeclaration>().Single(p => p.GetMethod == methodDeclaration || p.SetMethod == methodDeclaration);
            
            return HAL.Converter.Property(property);            
        }

        public EventInfo GetAccessorEvent(object method)
        {
            var methodDeclaration = method as IMethodDeclaration;
            var type = methodDeclaration.DeclaringType as ITypeDeclaration;
            var eventDefinition = type.Events.OfType<IEventDeclaration>().Single(p => p.AddMethod == methodDeclaration || p.RemoveMethod == methodDeclaration);

            return HAL.Converter.Event(eventDefinition);          
        }

        public IEnumerable<MethodInfo> GetUsedMethods(object method)
        {
            var analyzedMethod = method as IMethodDeclaration;
            if (analyzedMethod.Body == null)
            {
                yield break;
            }

            var body = analyzedMethod.Body as IMethodBody;
            var instructions = body.Instructions.OfType<IInstruction>();
            
            foreach (IInstruction instr in instructions)
            {
                IMethodReference mr = instr.Value as IMethodReference;
                if (mr != null)
                {
                    IMethodDeclaration def = mr.Resolve();
                    if (def != null)
                    {
                        yield return HAL.Converter.Method(def);
                    }
                }
            }
            yield break;
        }

        public IEnumerable<FieldInfo> GetUsedFields(object method)
        {
            var analyzedMethod = method as IMethodDeclaration;
            if (analyzedMethod.Body == null)
            {
                yield break;
            }

            var body = analyzedMethod.Body as IMethodBody;
            var instructions = body.Instructions.OfType<IInstruction>();

            foreach (IInstruction instr in instructions)
            {
                IFieldReference fr = instr.Value as IFieldReference;
                if (fr != null)
                {
                    IFieldDeclaration def = fr.Resolve();
                    if (def != null)
                    {
                        yield return HAL.Converter.Field(def);
                    }
                }
            }
            yield break;
        }

        public TypeInfo LoadDeclaringType(object member)
        {
            var memberReference = member as IMemberReference;
            return HAL.Converter.Type(memberReference.DeclaringType);
        }
    }
}
#endif