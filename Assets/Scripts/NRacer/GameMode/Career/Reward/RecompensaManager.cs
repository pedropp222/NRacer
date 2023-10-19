using Assets.Scripts.NRacer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NRacer.GameMode.Career.Reward
{
    public class RecompensaManager : MonoBehaviour
    {
        public CarroLootboxControlador lootboxControlador;

        public void DarRecompensa(RecompensaData recompensa)
        {


            if (recompensa.recompensaTipo == RecompensaTipo.DINHEIRO)
            {
                //TODO: Fazer um painel de recompensas genericas
            }
            else
            {
                if (recompensa.recompensaTipo == RecompensaTipo.CARRO_LOOTBOX)
                {
                    lootboxControlador.IniciarLootbox(recompensa.carroLootbox);
                }
                else
                {
                    lootboxControlador.IniciarLootbox(recompensa.carroGarantido);
                }
            }
        }
    }
}
