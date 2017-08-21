namespace RestApi
{
    public class QueryParam
    {
        public string Name { get; }

        public string Value { get; }

        public QueryParam(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}