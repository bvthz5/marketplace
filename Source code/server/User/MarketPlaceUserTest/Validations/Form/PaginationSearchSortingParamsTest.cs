using MarketPlaceUser.Bussiness.Dto.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlaceUserTest.Validations.Form
{
    public class PaginationSearchSortingParamsTest
    {
        [Fact]
        public void PaginationSearchSortingParams_DefaultValues()
        {
            // Arrange
            var paramsObj = new PaginationSearchSortingParams();

            // Act & Assert
            Assert.Equal(1, paramsObj.PageNumber);
            Assert.Equal(10, paramsObj.PageSize);
            Assert.Null(paramsObj.Search);
            Assert.Null(paramsObj.SortBy);
            Assert.False(paramsObj.SortByDesc);
        }

        [Fact]
        public void PaginationSearchSortingParams_ValuesSet()
        {
            // Arrange
            var paramsObj = new PaginationSearchSortingParams()
            {
                PageNumber = 3,
                PageSize = 25,
                Search = "test",
                SortBy = "name",
                SortByDesc = true
            };

            // Act & Assert
            Assert.Equal(3, paramsObj.PageNumber);
            Assert.Equal(25, paramsObj.PageSize);
            Assert.Equal("test", paramsObj.Search);
            Assert.Equal("name", paramsObj.SortBy);
            Assert.True(paramsObj.SortByDesc);
        }

        [Fact]
        public void PaginationSearchSortingParams_InvalidStringLength()
        {
            // Arrange
            var paramsObj = new PaginationSearchSortingParams()
            {
                Search = new string('a', 256),
                SortBy = new string('b', 21)
            };

            // Act
            var searchValidationResult = new List<ValidationResult>();
            var searchIsValid = Validator.TryValidateProperty(paramsObj.Search,
                new ValidationContext(paramsObj) { MemberName = nameof(paramsObj.Search) },
                searchValidationResult);

            var sortByValidationResult = new List<ValidationResult>();
            var sortByIsValid = Validator.TryValidateProperty(paramsObj.SortBy,
                new ValidationContext(paramsObj) { MemberName = nameof(paramsObj.SortBy) },
                sortByValidationResult);

            // Assert
            Assert.False(searchIsValid);
            Assert.False(sortByIsValid);

            Assert.Collection(searchValidationResult,
                validationResult => Assert.Equal("The field Search must be a string with a maximum length of 255.", validationResult.ErrorMessage));

            Assert.Collection(sortByValidationResult,
                validationResult => Assert.Equal("The field SortBy must be a string with a maximum length of 20.", validationResult.ErrorMessage));
        }


        [Fact]
        public void PaginationSearchSortingParams_ValidStringLength()
        {
            // Arrange
            var paramsObj = new PaginationSearchSortingParams()
            {
                Search = new string('a', 255),
                SortBy = new string('b', 20)
            };

            // Act
            var searchValidationResult = new List<ValidationResult>();
            var searchIsValid = Validator.TryValidateProperty(paramsObj.Search,
                new ValidationContext(paramsObj) { MemberName = nameof(paramsObj.Search) },
                searchValidationResult);

            var sortByValidationResult = new List<ValidationResult>();
            var sortByIsValid = Validator.TryValidateProperty(paramsObj.SortBy,
                new ValidationContext(paramsObj) { MemberName = nameof(paramsObj.SortBy) },
                sortByValidationResult);

            // Assert
            Assert.True(searchIsValid);
            Assert.True(sortByIsValid);
            Assert.Empty(searchValidationResult);
            Assert.Empty(sortByValidationResult);
        }

    }
}
