using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenTTDAdminPort.Common
{
    internal static class ExceptionExtensions
    {
        internal static bool TryGetInnerException(this Exception exception, Type exceptionType, [NotNullWhen(true)] out Exception? innerException)
        {
            while(exception?.InnerException != null)
            {
                if(exception.InnerException.GetType() == exceptionType)
                {
                    innerException = exception.InnerException;
                    return true;
                }

                exception = exception.InnerException;
            }

            innerException = null;
            return false;
        }
    }
}
