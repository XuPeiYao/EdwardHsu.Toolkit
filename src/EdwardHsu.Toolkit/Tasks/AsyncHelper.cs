using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EdwardHsu.Toolkit.Tasks
{
    public static class AsyncHelper
    {
        public static Task ToTask(Expression<Action> expFunc)
        {
            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            
            Task.Run(
                () =>
                {
                    try
                    {
                        expFunc.Compile().Invoke();
                        tcs.SetResult(null);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                });

            return tcs.Task;
        }

        public static Task<T> ToTask<T>(Expression<Func<T>> expFunc)
        {
            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();

            Task.Run(
                () =>
                {
                    try
                    {
                        var result = expFunc.Compile().Invoke();
                        tcs.SetResult(result);
                    }
                    catch(Exception e)
                    {
                        tcs.SetException(e);
                    }
                });

            return tcs.Task;
        }
    }
}
