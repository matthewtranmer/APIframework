namespace APIcontroller
{
    public struct APIRequest
    {
        public Dictionary<string, string>? request_body;
    }

    public struct APIResponse
    {
        public string? response_code;
        public string? error_message;

        public Dictionary<string, string>? response_body;

        public APIResponse(Dictionary<string, string>? response_body = null, string? error_message = null)
        {
            if (error_message != null)
            {
                response_code = "bad";
            }
            else
            {
                response_code = "ok";
            }

            this.response_body = response_body;
            this.error_message = error_message;
        }
    }

    public enum Method
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
