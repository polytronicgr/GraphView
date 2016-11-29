﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphView.GremlinTranslationOps.map
{
    internal class GremlinMeanOp: GremlinTranslationOperator
    {
        public Scope Scope;
        public GremlinMeanOp() { }

        public GremlinMeanOp(Scope scope)
        {
            Scope = scope;
        }
        public override GremlinToSqlContext GetContext()
        {
            GremlinToSqlContext inputContext = GetInputContext();

            return GremlinUtil.ProcessByFunctionStep("mean", inputContext, Labels);

            //var functionTableReference = GremlinUtil.GetSchemaObjectFunctionTableReference("mean");

            //GremlinDerivedVariable newVariable = new GremlinDerivedVariable(functionTableReference, "mean");

            //inputContext.AddNewVariable(newVariable, Labels);
            //inputContext.SetDefaultProjection(newVariable);
            //inputContext.SetCurrVariable(newVariable);

            //return inputContext;
        }
    }
}
