using MarketPlaceAdmin.Bussiness.Dto.Views;
using Xunit;

namespace MarketplaceAdminTest.Dto
{
    public class PagerOffsetTests
    {

        public class TestDto
        {
            public TestDto() { }
        }

        [Fact]
        public void PagerOffset_Valid()
        {
            // Arrage

            int index = -1;
            int limit = 10;
            int count = 10;


            IEnumerable<TestDto> resultSet = new List<TestDto>()
            {
                new TestDto (),
                new TestDto ()
            };

            // Act

            var reult = new PagerOffset<TestDto>(index, limit, count, resultSet.ToList());

            // Assert

            Assert.NotNull(reult);
        }

        [Fact]
        public void PagerOffset_Valid2()
        {
            // Arrage

            int index = -1;
            int limit = -1;
            int count = 10;


            IEnumerable<TestDto> resultSet = new List<TestDto>()
            {
                new TestDto (),
                new TestDto ()
            };

            // Act

            var reult = new PagerOffset<TestDto>(index, limit, count, resultSet.ToList());

            // Assert

            Assert.NotNull(reult);
        }
    }
}
