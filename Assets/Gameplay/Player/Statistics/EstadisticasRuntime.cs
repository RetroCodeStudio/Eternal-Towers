using UnityEngine;
using EternalTowers.Data.Player;
using EternalTowers.Gameplay.Player.Services;
using EternalTowers.Gameplay.Player.Controllers;

namespace EternalTowers.Gameplay.Player.Statistics
{
    public class EstadisticasRuntime : MonoBehaviour
    {
        private Estadisticas estadisticas;
        private EstadisticasService estadisticasService;
        private EstadisticasController estadisticasController;

        public Estadisticas Datos => estadisticas;

        private void Awake()
        {
            estadisticasService = new EstadisticasService();
            estadisticasController =
                new EstadisticasController(estadisticasService);

            estadisticas = estadisticasController.CrearEstadisticas(1);
        }

        public void RegistrarEnemigoDerrotado()
        {
            estadisticasController.RegistrarEnemigoDerrotado(
                estadisticas);
        }

        public void RegistrarPuntuacion(int puntuacion)
        {
            estadisticasController.ActualizarPuntuacion(
                estadisticas,
                puntuacion);
        }
    }
}