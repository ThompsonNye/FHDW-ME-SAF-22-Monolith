using System;
using Xunit;

namespace Nuyken.VeGasCo.Backend.Application.Tests.Unit.Apis.Helpers;

public class ExceptionCatchWrapper
{
    public static void WrapCallInExceptionChecker(Type exception, Action action)
    {
        if (exception is null) throw new ArgumentNullException(nameof(exception));

        if (action is null) throw new ArgumentNullException(nameof(action));

        try
        {
            action.Invoke();
        }
        // Exceptions sometimes get wrapped in AggregateExceptions
        catch (AggregateException ex)
        {
            // In case an AggregateException is actually expected
            if (exception == typeof(AggregateException))
                Assert.Equal(exception, ex.GetType());
            // Extract the inner exception and compare types
            else if (ex.InnerException is Exception inner)
                Assert.Equal(exception, inner.GetType());
            else
                // Unexpected, so just rethrow
                throw;
        }
        catch (Exception ex)
        {
            Assert.Equal(exception, ex.GetType());
        }
    }
}