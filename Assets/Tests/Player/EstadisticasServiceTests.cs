using NUnit.Framework;
using EternalTowers.Data.Player;
using EternalTowers.Gameplay.Player.Services;

namespace EternalTowers.Tests.Player
{
    public class EstadisticasServiceTests
    {
        private EstadisticasService estadisticasService;
        private Estadisticas estadisticas;

        [SetUp]
        public void SetUp()
        {
            estadisticasService = new EstadisticasService();
            estadisticas = estadisticasService.CrearEstadisticas(1);
        }

        [Test]
        public void CrearEstadisticas_DebeInicializarValoresEnCero()
        {
            Assert.AreEqual(1, estadisticas.IdUsuario);
            Assert.AreEqual(0, estadisticas.EnemigosDerrotados);
            Assert.AreEqual(0, estadisticas.PuntuacionMaxima);
        }

        [Test]
        public void RegistrarEnemigoDerrotado_DebeIncrementarContador()
        {
            estadisticasService.RegistrarEnemigoDerrotado(estadisticas);

            Assert.AreEqual(1, estadisticas.EnemigosDerrotados);
        }

        [Test]
        public void RegistrarEnemigoDerrotado_DebePermitirVariosEnemigos()
        {
            estadisticasService.RegistrarEnemigoDerrotado(estadisticas);
            estadisticasService.RegistrarEnemigoDerrotado(estadisticas);
            estadisticasService.RegistrarEnemigoDerrotado(estadisticas);

            Assert.AreEqual(3, estadisticas.EnemigosDerrotados);
        }

        [Test]
        public void ActualizarPuntuacion_DebeActualizarSiEsMayor()
        {
            estadisticasService.ActualizarPuntuacionMaxima(
                estadisticas,
                500);

            Assert.AreEqual(500, estadisticas.PuntuacionMaxima);
        }

        [Test]
        public void ActualizarPuntuacion_NoDebeReemplazarUnRecordMayor()
        {
            estadisticasService.ActualizarPuntuacionMaxima(
                estadisticas,
                500);

            estadisticasService.ActualizarPuntuacionMaxima(
                estadisticas,
                300);

            Assert.AreEqual(500, estadisticas.PuntuacionMaxima);
        }

        [Test]
        public void ActualizarPuntuacion_DebeActualizarSiLaNuevaPuntuacionEsMayor()
        {
            estadisticasService.ActualizarPuntuacionMaxima(
                estadisticas,
                500);

            estadisticasService.ActualizarPuntuacionMaxima(
                estadisticas,
                800);

            Assert.AreEqual(800, estadisticas.PuntuacionMaxima);
        }
    }
}