using System;

namespace MainProject.Helpers
{
    public class CustomException : Exception
    {
        public new string Message { get; private set; }

        public CustomException(string errorText)
        {
            Message = errorText;
        }
    }
}
