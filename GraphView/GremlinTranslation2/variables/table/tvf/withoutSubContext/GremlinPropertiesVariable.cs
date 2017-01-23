﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphView
{
    internal class GremlinPropertiesVariable: GremlinScalarTableVariable
    {
        public List<string> PropertyKeys { get; set; }
        public GremlinVariable ProjectVariable { get; set; }

        public GremlinPropertiesVariable(GremlinVariable projectVariable, List<string> propertyKeys)
        {
            ProjectVariable = projectVariable;
            PropertyKeys = new List<string>(propertyKeys);
        }

        public override WTableReference ToTableReference()
        {
            List<WScalarExpression> parameters = new List<WScalarExpression>();
            if (PropertyKeys.Count == 0)
            {
                parameters.Add(SqlUtil.GetColumnReferenceExpr(ProjectVariable.VariableName, "*"));
            }
            else
            {
                foreach (var property in PropertyKeys)
                {
                    parameters.Add(SqlUtil.GetColumnReferenceExpr(ProjectVariable.VariableName, property));
                }
            }
            
            var secondTableRef = SqlUtil.GetFunctionTableReference(GremlinKeyword.func.Properties, parameters, this, VariableName);
            return SqlUtil.GetCrossApplyTableReference(null, secondTableRef);
        }

        internal override void Key(GremlinToSqlContext currentContext)
        {
            GremlinKeyVariable newVariable = new GremlinKeyVariable(DefaultVariableProperty());
            currentContext.VariableList.Add(newVariable);
            currentContext.TableReferences.Add(newVariable);
            currentContext.SetPivotVariable(newVariable);
        }

        internal override void Value(GremlinToSqlContext currentContext)
        {
            GremlinValueVariable newVariable = new GremlinValueVariable(DefaultVariableProperty());
            currentContext.VariableList.Add(newVariable);
            currentContext.TableReferences.Add(newVariable);
            currentContext.SetPivotVariable(newVariable);
        }

        internal override void Drop(GremlinToSqlContext currentContext)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            foreach (var propertyKey in PropertyKeys)
            {
                properties[propertyKey] = null;
            }
            if (PropertyKeys.Count == 0)
            {
                properties["*"] = null;
            }
            if (ProjectVariable is GremlinVertexTableVariable)
            {
                UpdateVariable = new GremlinUpdateNodePropertiesVariable(ProjectVariable.DefaultVariableProperty(), properties);
            }
            else if (ProjectVariable is GremlinEdgeTableVariable)
            {
                GremlinVariableProperty nodeProperty = ProjectVariable.GetVariableProperty(GremlinKeyword.EdgeSourceV);
                GremlinVariableProperty edgeProperty = ProjectVariable.GetVariableProperty(GremlinKeyword.EdgeID);
                UpdateVariable = new GremlinUpdateEdgePropertiesVariable(nodeProperty, edgeProperty, properties);
            }
            else
            {
                throw new QueryCompilationException();
            }
            currentContext.VariableList.Add(UpdateVariable);
            currentContext.TableReferences.Add(UpdateVariable);
            currentContext.SetPivotVariable(UpdateVariable);
        }

        internal override void HasKey(GremlinToSqlContext currentContext, List<string> values)
        {
            throw new NotImplementedException();
            //foreach (var value in values)
            //{
            //    Has(currentContext, "_value", value);
            //}
        }

        internal override void HasValue(GremlinToSqlContext currentContext, List<object> values)
        {
            throw new NotImplementedException();
            //foreach (var value in values)
            //{
            //    Has(currentContext, "_value", value);
            //}
        }
    }
}
