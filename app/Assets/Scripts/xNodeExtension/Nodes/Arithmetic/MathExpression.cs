using CodingSeb.ExpressionEvaluator;
using NT.Atributes;
using System.Collections.Generic;
using XNode;

namespace NT.Nodes.Arithmetic
{

    [System.Serializable]
    public class MathExpression : NTNode
    {
        public string expression;

        [NTOutput] public double result;


        public override object GetValue(NodePort port)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                ExpressionEvaluator evaluator = customMathEvaluator();
                

                result = evaluator.Evaluate<double>(expression);
                return result;
            } else
            {
                result = 0;
                return null;
            }
        }

        private ExpressionEvaluator customMathEvaluator()
        {
            ExpressionEvaluator evaluator = new ExpressionEvaluator();
            // Set all necessary options
            evaluator.OptionCaseSensitiveEvaluationActive = false;
            evaluator.OptionForceIntegerNumbersEvaluationsAsDoubleByDefault = true;

            evaluator.OptionNewFunctionEvaluationActive = false;
            evaluator.OptionNewKeywordEvaluationActive = false;

            // TODO: https://github.com/codingseb/ExpressionEvaluator/wiki/Options
            // Quizas? OptionInlineNamespacesEvaluationActive, OptionFluidPrefixingActive

            // Seguridad mediante bloqueo de tipos? https://github.com/codingseb/ExpressionEvaluator/wiki/C%23-Types-Management
            //evaluator.TypesToBlock.Add();


            // Add User Variables
            SessionManager sm = SessionManager.Instance;
            evaluator.Variables = new Dictionary<string, object>();

            foreach (string key in sm.userVariables.Keys)
            {
                var value = sm.GetUserVariable(key);
                evaluator.Variables.Add(key, value);
            }

            return evaluator;
        }

        public override string GetDisplayName()
        {
            return "Math Expression";
        }


    }

}