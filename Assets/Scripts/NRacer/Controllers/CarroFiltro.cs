using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.NRacer.Controllers
{
    [System.Serializable]
    public struct CarroFiltro
    {
        public bool usarDesempenho;
        public int minDesempenho;
        public int maxDesempenho;

        public bool usarPeso;
        public int minPeso;
        public int maxPeso;

        public bool usarPotencia;
        public int minPotencia;
        public int maxPotencia;

        public bool usarTracao;
        public CarroStats.Tracao tracao;

        public static CarroFiltro VAZIO()
        {
            return new CarroFiltro();
        }

        public bool AvaliarDesempenho(int value)
        {
            if (!usarDesempenho) return true;

            return usarDesempenho && (value >= minDesempenho && value <= maxDesempenho);
        }

        public bool AvaliarPeso(int value)
        {
            if (!usarPeso) return true;

            return usarPeso && (value >= minPeso && value <= maxPeso);
        }

        public bool AvaliarPotencia(int value)
        {
            if (!usarPotencia) return true;

            return usarPotencia && (value >= minPotencia && value <= maxPotencia);
        }
    }
}
