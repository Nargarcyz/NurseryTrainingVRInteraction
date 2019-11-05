using CodingSeb.ExpressionEvaluator;
using NT.Atributes;
using XNode;

namespace NT.Nodes.Arithmetic
{

    [System.Serializable]
    public class MathExpression : NTNode
    {
        public string expression;

        [NTOutput] public float result;


        public override object GetValue(NodePort port)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                ExpressionEvaluator evaluator = new ExpressionEvaluator();
                return evaluator.Evaluate(expression);
            } else
            {
                return null;
            }
        }

        public override string GetDisplayName()
        {
            return "Math Expression";
        }


    }

}