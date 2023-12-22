using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CovidAPI.Models;
using CovidAPI.Services.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

public class GeolocationServiceTests
{
    [Fact]
    public async Task GetGeolocationInfoAsync_SuccessfulResponse()
    {
        var country = "Portugal";
var apiKey = "24cf9b5e55c34471a0e08a5a25e0c4b2";
var expectedApiResponse = new GeolocationApiResponse(); // Your expected response here

var handlerMock = new Mock<HttpMessageHandler>();
handlerMock
  .Protected()
  .Setup<Task<HttpResponseMessage>>(
     "SendAsync",
     ItExpr.IsAny<HttpRequestMessage>(),
     ItExpr.IsAny<CancellationToken>()
  )
  .ReturnsAsync(new HttpResponseMessage()
  {
      StatusCode = HttpStatusCode.OK,
      Content = new StringContent(JsonConvert.SerializeObject(expectedApiResponse)),
  })
  .Verifiable();

var httpClient = new HttpClient(handlerMock.Object);

var httpClientFactoryMock = new Mock<IHttpClientFactory>();
httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

var configurationMock = new Mock<IConfiguration>();
var loggerMock = new Mock<ILogger<GeolocationService>>();

var geolocationService = new GeolocationService(httpClientFactoryMock.Object, configurationMock.Object, loggerMock.Object);

// Act
var result = await geolocationService.GetGeolocationInfoAsync(country);

// Assert
Assert.NotNull(result);
// Add more assertions based on your expected response
    }

    [Fact]
    public async Task GetGeolocationInfoAsync_FailedResponse()
    {
        // Arrange
        var country = "Portugal";
        var apiKey = "24cf9b5e55c34471a0e08a5a25e0c4b2";
        var expectedApiResponse = new GeolocationApiResponse(); // Your expected response here

        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
          .Protected()
          .Setup<Task<HttpResponseMessage>>(
             "SendAsync",
             ItExpr.IsAny<HttpRequestMessage>(),
             ItExpr.IsAny<CancellationToken>()
          )
          .ReturnsAsync(new HttpResponseMessage()
          {
              StatusCode = HttpStatusCode.BadRequest,
              Content = new StringContent(JsonConvert.SerializeObject(expectedApiResponse)),
          })
          .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<GeolocationService>>();

        var geolocationService = new GeolocationService(httpClientFactoryMock.Object, configurationMock.Object, loggerMock.Object);

        // Act
        var result = await geolocationService.GetGeolocationInfoAsync(country);

        // Assert
        Assert.Null(result);
        // Add more assertions based on your expected error handling
    }
}
