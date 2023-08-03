namespace Assessment.Api.Exceptions
{
    public class UniqueException : Exception
    {
        public UniqueException(string? message) : base(message)
        {
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string? message) : base(message)
        {
        }
    }

    public class RequiredException : Exception
    {
        public RequiredException(string? message) : base(message)
        {
        }
    }
    public class MaxLengthException : Exception
    {
        public MaxLengthException(string? message) : base(message)
        {
        }
    }

}
