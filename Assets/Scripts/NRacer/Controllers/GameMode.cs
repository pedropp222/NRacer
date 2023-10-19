namespace Assets.Scripts.NRacer.Controllers
{
    public abstract class GameMode
    {
        public abstract void Inicializar();
        public abstract void ReceberResultadoCorrida(CorridaInfo info);

        public abstract void ObterCarro(CarroPlayerData carro);
    }
}