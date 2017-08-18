﻿/*
 * SonarAnalyzer for .NET
 * Copyright (C) 2015-2017 SonarSource SA
 * mailto: contact AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SonarAnalyzer.Helpers.FlowAnalysis.Common;

namespace SonarAnalyzer.Helpers.FlowAnalysis.CSharp
{
    internal class BooleanConstraintDecorator : ConstraintDecorator
    {
        public BooleanConstraintDecorator(CSharpExplodedGraphWalker explodedGraphWalker)
            : base(explodedGraphWalker)
        {
        }

        public override ProgramState PostProcessInstruction(SyntaxNode instruction, ProgramState preProgramState,
            ProgramState postProgramState)
        {
            var newProgramState = postProgramState;

            switch (instruction.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                    break; // Constant literal expressions are already handled by exploded graph walker

                case SyntaxKind.IsExpression:
                    {
                        SymbolicValue argSV;
                        preProgramState.PopValue(out argSV);
                        if (argSV.HasConstraint(ObjectConstraint.Null, newProgramState))
                        {
                            var sv = newProgramState.ExpressionStack.Peek();
                            newProgramState = sv.SetConstraint(BoolConstraint.False, newProgramState);
                        }
                        break;
                    }
            }

            return newProgramState;
        }
    }
}
