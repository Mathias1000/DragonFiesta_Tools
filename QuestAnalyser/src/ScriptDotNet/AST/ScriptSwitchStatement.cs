﻿#region using

using Irony.Compiler;
using ScriptNET.Runtime;
using System.Collections.Generic;

#endregion using

namespace ScriptNET.Ast
{
    /// <summary>
    ///
    /// </summary>
    internal class ScriptSwitchStatement : ScriptStatement
    {
        private List<ScriptSwitchCaseStatement> cases;
        private ScriptSwitchDefaultStatement defaultCase;

        public ScriptSwitchStatement(AstNodeArgs args)
            : base(args)
        {
            cases = new List<ScriptSwitchCaseStatement>();
            foreach (ScriptSwitchCaseStatement caseStatement in ChildNodes[0].ChildNodes)
            {
                cases.Add(caseStatement);
            }
            if (ChildNodes.Count == 2)
                defaultCase = ChildNodes[1] as ScriptSwitchDefaultStatement;
        }

        public override void Evaluate(IScriptContext context)
        {
            foreach (ScriptSwitchCaseStatement caseStatement in cases)
            {
                caseStatement.Evaluate(context);
                if (context.IsBreak() || context.IsReturn())
                {
                    context.SetBreak(false);
                    return;
                }
            }

            if (defaultCase != null)
                defaultCase.Evaluate(context);
        }
    }
}