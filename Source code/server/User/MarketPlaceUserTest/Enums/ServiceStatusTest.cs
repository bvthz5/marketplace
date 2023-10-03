using MarketPlaceUser.Bussiness.Enums;
using MarketPlaceUser.Bussiness.Helper;
using System.Net;

namespace MarketPlaceUserTest.Enums
{
    public class ServiceStatusTest
    {
        [Fact]
        public void ServiceStatus_Success_Equals200()
        {
            // Arrange
            var expected = HttpStatusCode.OK;

            // Act
            var actual = (HttpStatusCode)ServiceStatus.Success;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ServiceStatus_Created_Equals201()
        {
            // Arrange
            var expected = HttpStatusCode.Created;

            // Act
            var actual = (HttpStatusCode)ServiceStatus.Created;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ServiceStatus_BadRequest_Equals400()
        {
            // Arrange
            var expected = HttpStatusCode.BadRequest;

            // Act
            var actual = (HttpStatusCode)ServiceStatus.BadRequest;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ServiceStatus_Unauthorized_Equals401()
        {
            // Arrange
            var expected = HttpStatusCode.Unauthorized;

            // Act
            var actual = (HttpStatusCode)ServiceStatus.Unauthorized;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ServiceStatus_NotFound_Equals404()
        {
            // Arrange
            var expected = HttpStatusCode.NotFound;

            // Act
            var actual = (HttpStatusCode)ServiceStatus.NotFound;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ServiceStatus_InternalServerError_Equals500()
        {
            // Arrange
            var expected = HttpStatusCode.InternalServerError;

            // Act
            var actual = (HttpStatusCode)ServiceStatus.InternalServerError;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ServiceStatus_AlreadyExists_Equals409()
        {
            // Arrange
            var expected = HttpStatusCode.Conflict;

            // Act
            var actual = (HttpStatusCode)ServiceStatus.AlreadyExists;

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
