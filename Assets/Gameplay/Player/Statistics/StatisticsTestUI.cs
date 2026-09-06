using TMPro;
using UnityEngine;
using EternalTowers.Gameplay.Player.Statistics;

namespace EternalTowers.Gameplay.Player.Statistics
{
    public class StatisticsTestUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EstadisticasRuntime estadisticasRuntime;
        [SerializeField] private TMP_Text enemiesDefeatedText;
        [SerializeField] private TMP_Text highScoreText;
        [SerializeField] private TMP_InputField scoreInput;

        private void Start()
        {
            ActualizarUI();
        }

        public void RegistrarEnemigo()
        {
            estadisticasRuntime.RegistrarEnemigoDerrotado();
            ActualizarUI();
        }

        public void RegistrarPuntuacion()
        {
            if (!int.TryParse(scoreInput.text, out int puntuacion))
            {
                return;
            }

            estadisticasRuntime.RegistrarPuntuacion(puntuacion);
            ActualizarUI();
        }

        public void ActualizarUI()
        {
            if (estadisticasRuntime == null ||
                estadisticasRuntime.Datos == null)
            {
                return;
            }

            enemiesDefeatedText.text =
                $"Enemigos derrotados: {estadisticasRuntime.Datos.EnemigosDerrotados}";

            highScoreText.text =
                $"Puntuación máxima: {estadisticasRuntime.Datos.PuntuacionMaxima}";
        }
    }
}