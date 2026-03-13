using Moq;
using ApiLaboratorioAgua.ClienteController;
namespace ApiLaboratorioAgua.test.Controllers
{
    public class ClienteControllerTest
    {
        [Fact]
        public async Task RegistrarCliente_RetornaOk()
        {
            // Arrange
            var mockService = new Mock<ClienteService>(null, null);
            mockService.Setup(s => s.RegistrarClienteAsync(It.IsAny<ClientesDto>()))
                       .Returns(Task.CompletedTask);

            var controller = new ClienteController(mockService.Object);

            // Act
            var result = await controller.RegistrarCliente(new ClientesDto());

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
