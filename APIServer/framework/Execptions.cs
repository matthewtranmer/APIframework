namespace APIcontroller
{
    public class MethodNotStaticExeption : Exception
    {
        public MethodNotStaticExeption(string method_name) : base($"The method '{method_name}' was created as a resource but is not a static method.") { }
    }
    public class IncorrectReturnTypeExeption : Exception
    {
        public IncorrectReturnTypeExeption(string method_name) : base($"The method '{method_name}' was created as a resource but it's return type isnt of type 'Response'.") { }
    }
    public class NoParametersException : Exception
    {
        public NoParametersException(string method_name) : base($"The method '{method_name}' was created as a resource but it doesn't define any parameters.") { }
    }
    public class FirstParameterTypeException : Exception
    {
        public FirstParameterTypeException(string method_name) : base($"The method '{method_name}' was created as a resource but its parameter is not of type APIRequest.") { }
    }
}
