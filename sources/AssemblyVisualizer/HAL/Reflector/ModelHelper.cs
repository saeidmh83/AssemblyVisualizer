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
            /*var eventDefinition = fieldDefinition.DeclaringType.Events
                .FirstOrDefault(e => e.Name == fieldDefinition.Name && e.EventType == fieldDefinition.FieldType);
            return eventDefinition == null ? null : HAL.Converter.Event(eventDefinition);*/
            return null;
        }

        public PropertyInfo GetAccessorProperty(object method)
        {
            var methodDefinition = method as IMethodDeclaration;
            /*var type = methodDefinition.DeclaringType;
            var prop = type.Properties.Single(p => p.GetMethod == methodDefinition || p.SetMethod == methodDefinition);
            return HAL.Converter.Property(prop);*/
            return null;
        }

        public EventInfo GetAccessorEvent(object method)
        {
            var methodDefinition = method as IMethodDeclaration;
            /*var type = methodDefinition.DeclaringType;
            var ev = type.Events.Single(p => p.AddMethod == methodDefinition || p.RemoveMethod == methodDefinition);
            return HAL.Converter.Event(ev);*/
            return null;
        }

        public IEnumerable<MethodInfo> GetUsedMethods(object method)
        {
            var analyzedMethod = method as IMethodDeclaration;
            /*if (!analyzedMethod.HasBody)
            {
                yield break;
            }

            foreach (Instruction instr in analyzedMethod.Body.Instructions)
            {
                MethodReference mr = instr.Operand as MethodReference;
                if (mr != null)
                {
                    MethodDefinition def = mr.Resolve();
                    if (def != null)
                    {
                        yield return HAL.Converter.Method(def);
                    }
                }
            }*/
            yield break;
        }

        public IEnumerable<FieldInfo> GetUsedFields(object method)
        {
            var analyzedMethod = method as IMethodDeclaration;
            /*if (!analyzedMethod.HasBody)
            {
                yield break;
            }

            foreach (Instruction instr in analyzedMethod.Body.Instructions)
            {
                FieldReference fr = instr.Operand as FieldReference;
                if (fr != null)
                {
                    FieldDefinition def = fr.Resolve();
                    if (def != null)
                    {
                        yield return HAL.Converter.Field(def);
                    }
                }
            }*/
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