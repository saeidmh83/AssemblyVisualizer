// Copyright 2011 Denis Markelov
// Based on fragments of code from ILSpy project
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)

#if ILSpy
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace AssemblyVisualizer.TypeBrowser
{
    class AnalyzerEngine
    {
        public static IEnumerable<MethodDefinition> GetUsedMethods(MethodDefinition analyzedMethod)
        {
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
                        yield return def;
                    }
                }
            }
        }

        public static IEnumerable<FieldDefinition> GetUsedFields(MethodDefinition analyzedMethod)
        {
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
                        yield return def;
                    }
                }
            }
        }
    }
}
#endif