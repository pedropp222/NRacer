namespace Assets.Scripts.NRacer.Controllers
{
    [System.Serializable]
    public struct CarroData
    {
        public int id;
        public int trimId;

        public CarroData(int id, int trimId)
        {
            this.id = id;
            this.trimId = trimId;
        }

        public static CarroData Vazio
        {
            get { return new CarroData(-1, -1); }
        }

        public static bool operator ==(CarroData a, CarroData b)
        {
            return a.id == b.id && a.trimId == b.trimId;
        }

        public static bool operator !=(CarroData a, CarroData b)
        {
            return a.id != b.id || a.trimId != b.trimId;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return "CarroData: ID>" + id + " |Trim> " + trimId;
        }
    }
}
