using EternalTowers.Data.Player;

namespace EternalTowers.Gameplay.Player.Services
{
    public class EstadisticasService
    {
        public Estadisticas CrearEstadisticas(int idUsuario)
        {
            return new Estadisticas
            {
                IdUsuario = idUsuario,
                EnemigosDerrotados = 0,
                PuntuacionMaxima = 0
            };
        }

        public void RegistrarEnemigoDerrotado(Estadisticas estadisticas)
        {
            if (estadisticas == null)
                return;

            estadisticas.EnemigosDerrotados++;
        }

        public void ActualizarPuntuacionMaxima(
            Estadisticas estadisticas,
            int nuevaPuntuacion)
        {
            if (estadisticas == null)
                return;

            if (nuevaPuntuacion > estadisticas.PuntuacionMaxima)
            {
                estadisticas.PuntuacionMaxima = nuevaPuntuacion;
            }
        }
    }
}