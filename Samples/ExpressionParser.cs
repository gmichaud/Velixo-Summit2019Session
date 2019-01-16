using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Common.Parser;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.IN;

namespace Velixo.Summit2019Samples
{
    public class InventoryMaintParserDemo : PXGraphExtension<InventoryItemMaint>
    {
        public PXAction<InventoryItem> TestParser;
        [PXButton]
        [PXUIField(DisplayName = "Test Expression Parser")]
        protected IEnumerable testParser(PXAdapter adapter)
        {
            var expressionText = PXNoteAttribute.GetNote(Base.Item.Cache, Base.Item.Current);
            if (String.IsNullOrEmpty(expressionText)) throw new PXException("Please enter expression to evaluate in item notes and save item.");

            var parsedExpression = INExpressionParser.Parse(expressionText);
            var result = parsedExpression.Eval(Base);

            throw new PXException($"Your expression was evaluated.\n\nExpression: {expressionText}\nResult:{result}");
        }
    }

    class INExpressionParser : ExpressionParser
    {
        private INExpressionParser(string text) : base(text)
        {
        }

        protected override ExpressionContext CreateContext()
        {
            return new INExpressionContext();
        }

        protected override NameNode CreateNameNode(ExpressionNode node, string tokenString)
        {
            return new INNameNode(node, tokenString, Context);
        }

        protected override void ValidateName(NameNode node, string tokenString)
        {

        }

        protected override bool IsAggregate(string nodeName)
        {
            return false;
        }

        protected override AggregateNode CreateAggregateNode(string name, string dataField)
        {
            throw new NotImplementedException();
        }

        public static ExpressionNode Parse(string formula)
        {
            var expr = new INExpressionParser(formula);
            return expr.Parse();
        }
    }

    public class INExpressionContext : ExpressionContext
    {
        internal const string AttributeSuffix = "_Attributes";

        public INExpressionContext()
        {
        }

        public virtual object Evaluate(INNameNode node, PXGraph graph)
        {
            var cache = graph.Caches[typeof(InventoryItem)];
            var currentItem = (InventoryItem)cache.Current;
            if (currentItem == null) throw new ArgumentNullException("No current item.");

            if (node.FieldName.EndsWith(AttributeSuffix))
            {
                return EvaluateAttribute(graph, currentItem.NoteID, node.FieldName.Substring(0, node.FieldName.Length - AttributeSuffix.Length));
            }
            else
            {
                return ConvertFromExtValue(cache.GetValueExt(currentItem, node.FieldName));
            }
        }

        private object EvaluateAttribute(PXGraph graph, Guid? noteID, string attribute)
        {
            PXResultset<CSAnswers> res = PXSelectJoin<CSAnswers,
                InnerJoin<CSAttribute, On<CSAttribute.attributeID, Equal<CSAnswers.attributeID>>>,
                Where<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>,
                    And<CSAnswers.attributeID, Equal<Required<CSAnswers.attributeID>>>>>.Select(graph, noteID, attribute);

            CSAnswers ans = null;
            CSAttribute attr = null;
            if (res.Count > 0)
            {
                ans = (CSAnswers)res[0][0];
                attr = (CSAttribute)res[0][1];
            }

            if (ans == null || ans.AttributeID == null)
            {
                //answer not found. if attribute exists return the default value.
                attr = PXSelect<CSAttribute, Where<CSAttribute.attributeID, Equal<Required<CSAttribute.attributeID>>>>.Select(graph, attribute);

                if (attr != null && attr.ControlType == CSAttribute.CheckBox)
                {
                    return false;
                }
            }

            if (ans != null)
            {
                if (ans.Value != null)
                {
                    if (attr.ControlType == CSAttribute.CheckBox)
                    {
                        return ans.Value == "1";
                    }
                    else
                    {
                        return ans.Value;
                    }
                }
                else
                {
                    if (attr != null && attr.ControlType == CSAttribute.CheckBox)
                    {
                        return false;
                    }
                }
            }

            return string.Empty;
        }

        protected object ConvertFromExtValue(object extValue)
        {
            PXFieldState fs = extValue as PXFieldState;
            if (fs != null)
                return fs.Value;
            else
            {
                return extValue;
            }
        }
    }

    public class INNameNode : NameNode
    {
        public INObjectType ObjectName { get; protected set; }

        public string FieldName { get; protected set; }

        public INNameNode(ExpressionNode node, string tokenString, ExpressionContext context)
            : base(node, tokenString, context)
        {
            string[] parts = Name.Split('.');

            if (parts.Length == 2)
            {
                ObjectName = (INObjectType)Enum.Parse(typeof(INObjectType), parts[0], true);
                FieldName = parts[1];
            }
            else
            {
                ObjectName = INObjectType.InventoryItem;
                FieldName = Name;
            }
        }

        public override object Eval(object row)
        {
            return ((INExpressionContext)context).Evaluate(this, (PXGraph)row);
        }
    }

    public enum INObjectType
    {
        InventoryItem
    }
}
