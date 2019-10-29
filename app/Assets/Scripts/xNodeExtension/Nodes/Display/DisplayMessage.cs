namespace NT.Nodes.Display {
    
    public class DisplayMessage : FlowNode {

        public string input;
        

        public object GetValue() {
            return null;
        }

        public override string GetDisplayName(){
            return "Display Message";
        }
    }
}
