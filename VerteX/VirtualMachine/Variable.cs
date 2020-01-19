namespace VerteX.VirtualMachine
{
    public class Variable
    {
        public VariableType type;
        public string value;

        public Variable(VariableType type, string value)
        {
            this.type = type;
            this.value = value;
        }

        public override string ToString()
        {
            if (type == VariableType.String)
            {
                return $"\"{value}\"";
            }
            return value;
        }
    }
}
