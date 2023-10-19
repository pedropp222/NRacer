using Assets.Scripts.NRacer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.NRacer.GameMode.Career.Reward
{
    public struct RecompensaData
    {
        public RecompensaTipo recompensaTipo;
        public CarroFiltro carroLootbox;
        public CarroData carroGarantido;
        public int dinheiro;

        public static RecompensaData DINHEIRO(int valor)
        {
            return new RecompensaData
            {
                recompensaTipo = RecompensaTipo.DINHEIRO,
                dinheiro = valor
            };
        }

        public static RecompensaData LOOTBOX(CarroFiltro filtro)
        {
            return new RecompensaData
            { 
                recompensaTipo = RecompensaTipo.CARRO_LOOTBOX,
                carroLootbox = filtro
            };
        }

        public static RecompensaData CARRO(CarroData carro)
        {
            return new RecompensaData
            {
                recompensaTipo = RecompensaTipo.CARRO_GARANTIDO,
                carroGarantido = carro
            };
        }
    }

    public enum RecompensaTipo
    { 
        CARRO_LOOTBOX,
        CARRO_GARANTIDO,
        DINHEIRO
    }

}
