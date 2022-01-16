using EdwardHsu.Toolkit.Tasks;

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EdwardHsu.Toolkit.Tests
{
    public class AsyncHelperTest
    {

        private void SampleV(ref int v)
        {
            Thread.Sleep(100);
            v = 123;
        }
        private int SampleG()
        {
            Thread.Sleep(100);
            return 123;
        }

        [Fact]
        public async Task TaskG()
        {
            var value = await AsyncHelper.ToTask(() => SampleG());
            Assert.Equal(value, 123);
        }

        [Fact]
        public async Task TaskV()
        {
            int value = 0;
            await AsyncHelper.ToTask(() => SampleV(ref value));
            Assert.Equal(value, 123);
        }
    }
}
