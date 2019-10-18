using NT.Atributes;
using XNode;

namespace NT.Nodes.Arithmetic
{

    [System.Serializable]
    public class DivNode : NTNode
    {
        [NTInput] public float valueA;
        [NTInput] public float valueB;

        [NTOutput] public float result;


        public override object GetValue(NodePort port)
        {
            float val1 = GetInputValue<float>(nameof(valueA), this.valueA);
            float val2 = GetInputValue<float>(nameof(valueB), this.valueB);

            // Control division by 0
            result = (val2 == 0) ? 0 : val1 / val2;

            return result;
        }

        public override string GetDisplayName()
        {
            return "Div";
        }


    }

}