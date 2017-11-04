using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleAppCore.Util
{

    public delegate Int32 Morpher<TResult, in TArgument>(Int32 startValue, TArgument argument, out TResult morphResult);


    public static class Helpers
    {
        public static TResult Morpher<TResult, TArgument>(ref Int32 target, TArgument argument,
            Morpher<TResult, TArgument> morpher)
        {
            TResult morphResult;
            Int32 currentValue = target, startValue, desiredValue;
            do
            {
                startValue = currentValue;
                desiredValue = morpher(startValue, argument, out morphResult);
                currentValue = Interlocked.CompareExchange(ref target, desiredValue, startValue);
            } while (startValue != currentValue);

            return morphResult;
        }
    }
}
