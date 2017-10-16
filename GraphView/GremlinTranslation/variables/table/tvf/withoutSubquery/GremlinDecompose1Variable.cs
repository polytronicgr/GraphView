﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphView
{
    internal class GremlinDecompose1Variable: GremlinTableVariable
    {
        public GremlinVariable ComposeVariable { get; set; }
        public List<string> SideEffectKeys { get; set; }
        
        public GremlinDecompose1Variable(GremlinVariable composeVariable) : base(GremlinVariableType.Unknown)
        {
            this.ComposeVariable = composeVariable;
            this.SideEffectKeys = new List<string>();
        }

        public GremlinDecompose1Variable(GremlinVariable composeVariable, List<string> sideEffectKeys) : base(GremlinVariableType.Unknown)
        {
            this.ComposeVariable = composeVariable;
            this.SideEffectKeys = sideEffectKeys;
        }

        internal override bool Populate(string property, string label = null)
        {
            if (this.SideEffectKeys.Count == 0)
            {
                if (this.ComposeVariable is GremlinPathVariable)
                {
                    this.ComposeVariable.PopulateStepProperty(property, label);
                }
                else
                {
                    this.ComposeVariable.Populate(property, label);
                }
                base.Populate(property, null);
                return true;
            }
            else
            {
                foreach (string sideEffectKey in this.SideEffectKeys)
                {
                    if (this.ComposeVariable is GremlinPathVariable)
                    {
                        this.ComposeVariable.PopulateStepProperty(property, sideEffectKey);
                    }
                    else
                    {
                        this.ComposeVariable.Populate(property, sideEffectKey);
                    }
                }
                base.Populate(property, null);
                return true;
            }
        }

        public override WTableReference ToTableReference()
        {
            List<WScalarExpression> parameters = new List<WScalarExpression>();
            parameters.Add(SqlUtil.GetColumnReferenceExpr(GremlinKeyword.Compose1TableDefaultName, GremlinKeyword.TableDefaultColumnName));
            parameters.Add(SqlUtil.GetValueExpr(this.DefaultProperty()));
            parameters.AddRange(this.ProjectedProperties.Select(SqlUtil.GetValueExpr));

            var tableRef = SqlUtil.GetFunctionTableReference(GremlinKeyword.func.Decompose1, parameters, GetVariableName());
            return SqlUtil.GetCrossApplyTableReference(tableRef);
        }
    }
}
