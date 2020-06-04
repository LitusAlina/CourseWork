using System;

namespace Tasker.Exceptions
{
    public class NoExecutorException : Exception
    {
        public NoExecutorException()
        {
        }

        public NoExecutorException(string message) : base(message)
        {
        }

        public NoExecutorException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
