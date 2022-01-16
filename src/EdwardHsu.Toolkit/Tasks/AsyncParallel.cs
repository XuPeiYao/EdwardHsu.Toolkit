using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace EdwardHsu.Toolkit.Tasks
{
    public static class AsyncParallel
    {
        internal static readonly ParallelOptions _defaultParallelOptions = new ParallelOptions();

        public static Task InvokeAsync(params Func<Task>[] asyncActions)
        {
            return InvokeAsync(_defaultParallelOptions, asyncActions);
        }

        public static async Task InvokeAsync(ParallelOptions parallelOptions, params Func<Task>[] asyncActions)
        {
            var ab = new ActionBlock<Func<Task>>(
                async (Func<Task> task) => { await task(); }, new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = parallelOptions.MaxDegreeOfParallelism,
                    CancellationToken      = parallelOptions.CancellationToken,
                    TaskScheduler          = parallelOptions.TaskScheduler
                });

            foreach (var asyncAction in asyncActions)
            {
                await ab.SendAsync(asyncAction);
            }

            ab.Complete();

            await ab.Completion;
        }


        public static Task ForEach<TSource>(
            IEnumerable<TSource> source,
            Func<TSource, Task>  body
        ) => ForEach(source, _defaultParallelOptions, body);

        public static async Task ForEach<TSource>(
            IEnumerable<TSource> source,
            ParallelOptions      parallelOptions,
            Func<TSource, Task>  body
        )
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (parallelOptions == null)
            {
                throw new ArgumentNullException(nameof(parallelOptions));
            }

            var ab = new ActionBlock<TSource>(
                async (TSource item) => { await body(item); }, new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = parallelOptions.MaxDegreeOfParallelism,
                    CancellationToken      = parallelOptions.CancellationToken,
                    TaskScheduler          = parallelOptions.TaskScheduler
                });

            foreach (var item in source)
            {
                await ab.SendAsync(item);
            }

            ab.Complete();

            await ab.Completion;
        }
        
        public static Task ForEach<TSource>(
            IEnumerable<TSource>                         source,
            Func<TSource, long, Task> body
        ) => ForEach(source, _defaultParallelOptions, body);

        public static async Task ForEach<TSource>(
            IEnumerable<TSource>                         source,
            ParallelOptions                              parallelOptions,
            Func<TSource, long, Task> body
        )
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (parallelOptions == null)
            {
                throw new ArgumentNullException(nameof(parallelOptions));
            }

            var ab = new ActionBlock<(TSource source, int i)>(
                async ((TSource source, int i) item) => { await body(item.source, item.i); }, new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = parallelOptions.MaxDegreeOfParallelism,
                    CancellationToken      = parallelOptions.CancellationToken,
                    TaskScheduler          = parallelOptions.TaskScheduler
                });

            int i = 0;
            foreach (var item in source)
            {
                await ab.SendAsync((item, i++));
            }

            ab.Complete();

            await ab.Completion;
        }
    }
}
