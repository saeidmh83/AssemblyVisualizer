// Copyright 2011 Denis Markelov
// Code of GetUsedMethods() and GetUsedFields() is taken from ILSpy project
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if ILSpy
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using AssemblyVisualizer.Model;
using Mono.Cecil.Cil;

namespace AssemblyVisualizer.HAL.ILSpy
{
    class ModelHelper
    {
        public EventInfo GetEventForBackingField(object field)
        {
            var fieldDefinition = field as FieldDefinition;
            var eventDefinition = fieldDefinition.DeclaringType.Events
                .FirstOrDefault(e => e.Name == fieldDefinition.Name && e.EventType == fieldDefinition.FieldType);
            return eventDefinition == null ? null : HAL.Converter.Event(eventDefinition);
        }

        public PropertyInfo GetAccessorProperty(object method)
        {
            var methodDefinition = method as MethodDefinition;
            var type = methodDefinition.DeclaringType;
            var prop = type.Properties.Single(p => p.GetMethod == methodDefinition || p.SetMethod == methodDefinition);
            return HAL.Converter.Property(prop);
        }

        public EventInfo GetAccessorEvent(object method)
        {
            var methodDefinition = method as MethodDefinition;
            var type = methodDefinition.DeclaringType;
            var ev = type.Events.Single(p => p.AddMethod == methodDefinition || p.RemoveMethod == methodDefinition);
            return HAL.Converter.Event(ev);
        }

        public IEnumerable<MethodInfo> GetUsedMethods(object method)
        {
            var analyzedMethod = method as MethodDefinition;
            if (!analyzedMethod.HasBody)
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
            }
        }

        public IEnumerable<FieldInfo> GetUsedFields(object method)
        {
            var analyzedMethod = method as MethodDefinition;
            if (!analyzedMethod.HasBody)
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
            }
        }

        public TypeInfo LoadDeclaringType(object member)
        {
            var memberReference = member as MemberReference;
            return HAL.Converter.Type(memberReference.DeclaringType);
        }
    }
}
#endif