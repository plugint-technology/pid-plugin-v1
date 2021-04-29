using System;
using PidPlugin.Cache;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace PidPlugin.Test.Unit.Cache
{
    public class MemoryCacheAccessorTests
    {
        private MemoryCacheAccessor Sut { get; set; }

        private Mock<IMemoryCache> MemoryCacheMock { get; set; }


        public MemoryCacheAccessorTests()
        {
            this.MemoryCacheMock = new Mock<IMemoryCache>();

            this.Sut = new MemoryCacheAccessor(this.MemoryCacheMock.Object);
        }

        public class The_Constructor : MemoryCacheAccessorTests
        {
            [Fact]
            public void Should_throw_argument_null_exception_when_parameter_is_null()
            {
                // arrange

                IMemoryCache memoryCache = null;

                // act & assert

                Assert.Throws<ArgumentNullException>(nameof(memoryCache), () => new MemoryCacheAccessor(memoryCache));
            }
        }

        public class The_Method_TryGetValue : MemoryCacheAccessorTests
        {
            [Fact]
            public void Should_call_try_get_value_of_memory_cache()
            {
                // arrange

                string key = "clave";
                object output = null;

                // act

                bool actual = this.Sut.TryGetValue(key, out object value);

                // assert

                this.MemoryCacheMock.Verify(
                    x => x.TryGetValue(
                        It.Is<object>(p => p.ToString() == key), 
                        out output
                    ),
                    Times.Once()
                );
            }
        }

        public class The_Method_Set : MemoryCacheAccessorTests
        {
            [Fact]
            public void Should_call_set_of_memory_cache()
            {
                // arrange

                string   key        = "clave";
                object   value      = "test";
                TimeSpan expiration = TimeSpan.FromSeconds(2);

                Mock<ICacheEntry> cacheEntryMock = new Mock<ICacheEntry>();

                this.MemoryCacheMock
                    .Setup(x => x.CreateEntry(It.IsAny<object>()))
                    .Returns(cacheEntryMock.Object);

                object valueSetted = null;

                cacheEntryMock
                    .SetupSet(p => p.Value = It.IsAny<object>())
                    .Callback<object>(v => valueSetted = v);

                TimeSpan? expirationSetted = null;

                cacheEntryMock
                    .SetupSet(p => p.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan?>())
                    .Callback<TimeSpan?>(e => expirationSetted = e);

                // act

                this.Sut.Set(key, value, expiration);

                // assert

                this.MemoryCacheMock.Verify(
                    x => x.CreateEntry(
                        It.Is<object>(p => p.ToString() == key)
                    ),
                    Times.Once
                );

                valueSetted.Should().BeEquivalentTo(value);
                expirationSetted.Value.Should().Be(expiration);
            }
        }
    }
}
