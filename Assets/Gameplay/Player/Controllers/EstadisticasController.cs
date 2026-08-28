using EternalTowers.Data.Player;
using EternalTowers.Gameplay.Player.Services;

namespace EternalTowers.Gameplay.Player.Controllers
{
    public class EstadisticasController
    {
        private readonly EstadisticasService estadisticasService;

        public EstadisticasController(EstadisticasService estadisticasService)
        {
            this.estadisticasService = estadisticasService;
        }

        public Estadisticas CrearEstadisticas(int idUsuario)
        {
            return estadisticasService.CrearEstadisticas(idUsuario);
        }

        public void RegistrarEnemigoDerrotado(Estadisticas estadisticas)
        {
            estadisticasService.RegistrarEnemigoDerrotado(estadisticas);
        }

        public void ActualizarPuntuacion(
            Estadisticas estadisticas,
            int nuevaPuntuacion)
        {
            estadisticasService.ActualizarPuntuacionMaxima(
                estadisticas,
                nuevaPuntuacion);
        }
    }
}