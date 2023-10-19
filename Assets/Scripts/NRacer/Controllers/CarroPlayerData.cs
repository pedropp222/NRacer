using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NRacer.Controllers
{
    [System.Serializable]
    public struct CarroPlayerData
    {
        public int id;
        public int trimId;
        public CarroCor chassisCor;
        public int nivel;

        public CarroPlayerData(int id, int trimId)
        {
            this.id = id;
            this.trimId = trimId;
            chassisCor = CarroCor.BRANCO();
            nivel = 1;
        }

        public CarroPlayerData(int id, int trimId, Color cor)
        {
            this.id = id;
            this.trimId = trimId;
            chassisCor = new CarroCor(cor);
            nivel = 1;
        }

        public static CarroPlayerData Vazio
        {
            get { return new CarroPlayerData(-1, -1); }
        }

        public static bool operator == (CarroPlayerData a, CarroPlayerData b)
        {
            return a.id == b.id && a.trimId == b.trimId && a.chassisCor == b.chassisCor && a.nivel == b.nivel;
        }

        public static bool operator != (CarroPlayerData a, CarroPlayerData b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [System.Serializable]
    public struct CarroCor
    {
        public float r, g, b;

        public CarroCor(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public CarroCor(Color c)
        {
            this.r = c.r;
            this.g = c.g;
            this.b = c.b;
        }

        public static CarroCor BRANCO()
        {
            return new CarroCor(1f,1f,1f);
        }

        public static CarroCor RANDOM()
        {
            //30% de probabilidade de ser grayscale
            if (UnityEngine.Random.Range(0f, 100f) <= 30f)
            {
                float f = UnityEngine.Random.Range(0f, 1f);
                return new CarroCor(f, f, f);
            }

            return new CarroCor(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        }

        public Color ParaCor()
        {
            return new Color(r, g, b,1.0f);
        }

        public static bool operator == (CarroCor a, CarroCor b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b;
        }

        public static bool operator != (CarroCor a, CarroCor b )
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
