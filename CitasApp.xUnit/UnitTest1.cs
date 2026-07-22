using CitasApp.Application.Services;
using Xunit;

namespace CitasApp.xUnit
{
    public class CalculadoraServiceTests
    {
        [Fact]
        public void Dividir_PorCero_LanzaExcepcion()
        {
            var calc = new CalculadoraService();
            Assert.Throws<DivideByZeroException>(() => calc.Dividir(10, 0));
        }
    }
}