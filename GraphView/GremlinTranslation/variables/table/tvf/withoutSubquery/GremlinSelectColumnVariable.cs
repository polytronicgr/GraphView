﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphView
{
    internal class GremlinSelectColumnVariable : GremlinTableVariable
    {
        public GremlinVariable InputVariable { get; set; }
        public GremlinKeyword.Column Column { get; set; }

        public GremlinSelectColumnVariable(GremlinVariable inputVariable, GremlinKeyword.Column column) : base(GremlinVariableType.Unknown)
        {
            GremlinVariableType inputVariableType = inputVariable.GetVariableType();
            if (!(GremlinVariableType.NULL <= inputVariableType && inputVariableType <= GremlinVariableType.Map))
            {
                throw new SyntaxErrorException("The inputVariable of select() can not be " + inputVariableType);
            }

            this.InputVariable = inputVariable;
            this.Column = column;
        }

        internal override bool Populate(string property, string label = null)
        {
            if (base.Populate(property, label))
            {
                this.InputVariable.Populate(property, null);
                return true;
            }
            else if (this.InputVariable.Populate(property, label))
            {
                base.Populate(property, null);
                return true;
            }
            else
            {
                return false;
            }
        }

        internal override List<GremlinVariable> FetchAllVars()
        {
            List<GremlinVariable> variableList = new List<GremlinVariable>() { this };
            variableList.AddRange(this.InputVariable.FetchAllVars());
            return variableList;
        }

        public override WTableReference ToTableReference()
        {
            List<WScalarExpression> parameters = new List<WScalarExpression>();
            parameters.Add(this.InputVariable.DefaultProjection().ToScalarExpression());
            parameters.Add(SqlUtil.GetValueExpr(this.Column == GremlinKeyword.Column.Keys ? "Keys" : "Values"));
            parameters.Add(SqlUtil.GetValueExpr(this.DefaultProperty()));
            parameters.AddRange(this.ProjectedProperties.Select(SqlUtil.GetValueExpr));
            var tableRef = SqlUtil.GetFunctionTableReference(GremlinKeyword.func.SelectColumn, parameters, GetVariableName());
            return SqlUtil.GetCrossApplyTableReference(tableRef);
        }
    }

    internal class GremlinOrderLocalInitVariable : GremlinVariable
    {
        public GremlinOrderLocalInitVariable(GremlinVariable inputVariable): base(inputVariable.GetVariableType())
        {
            this.VariableName = GremlinKeyword.Compose1TableDefaultName;
        }
    }
}
