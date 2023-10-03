using MarketPlaceAdmin.Bussiness.Dto.Views;
using Xunit;

namespace MarketplaceAdminTest.Dto
{
    public class PagerTests
    {

        public class TestDto
        {
            public TestDto() { }
        }

        [Fact]
        public void Pager_Valid_Inputs()
        {
            // Arrage

            int pageNumber = 1;
            int pageSize = 1;
            IEnumerable<TestDto> resultSet = new List<TestDto>()
            {
                new TestDto (),
                new TestDto ()
            };


            // Act

            var pager = new Pager<TestDto>(pageNumber, pageSize, resultSet);

            // Assert

            Assert.NotNull(pager);
            Assert.Equal(pageSize, pager.PageSize);
            Assert.Equal(pageNumber, pager.CurrentPage);
            Assert.Equal(pageSize, pager.Result.Count());

        }

        [Fact]
        public void Pager_InValid_Inputs()
        {
            // Arrage

            int pageNumber = -1;
            int pageSize = -1;
            IEnumerable<TestDto> resultSet = new List<TestDto>()
            {
                new TestDto (),
                new TestDto ()
            };


            // Act

            var pager = new Pager<TestDto>(pageNumber, pageSize, resultSet);

            // Assert

            Assert.NotNull(pager);
            Assert.Equal(10, pager.PageSize);
            Assert.Equal(1, pager.CurrentPage);
            Assert.Equal(resultSet.Count(), pager.Result.Count());

        }

        [Fact]
        public void Pager_Valid_Inputs2()
        {
            // Arrage

            int pageNumber = 1;
            int pageSize = 1;
            IEnumerable<TestDto> resultSet = new List<TestDto>()
            {
                new TestDto (),
                new TestDto ()
            };


            // Act

            var pager = new Pager<TestDto>(pageNumber, pageSize, resultSet.Count());

            // Assert

            Assert.NotNull(pager);
            Assert.Equal(pageSize, pager.PageSize);
            Assert.Equal(pageNumber, pager.CurrentPage);

        }

        [Fact]
        public void Pager_InValid_Inputs2()
        {
            // Arrage

            int pageNumber = -1;
            int pageSize = -1;
            IEnumerable<TestDto> resultSet = new List<TestDto>()
            {
                new TestDto (),
                new TestDto ()
            };


            // Act

            var pager = new Pager<TestDto>(pageNumber, pageSize, resultSet.Count());

            // Assert

            Assert.NotNull(pager);
            Assert.Equal(10, pager.PageSize);
            Assert.Equal(1, pager.CurrentPage);

        }

    }
}
