using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.NRacer.GameMode.Career
{
    public class Calendario
    {
        private DateTime date;

        private static DateTime dataInicial;

        private static string[] meses = {
            "Janeiro","Fevereiro",
            "Março","Abril","Maio",
            "Junho","Julho","Agosto",
            "Setembro","Outubro",
            "Novembro","Dezembro"
        };

        public Calendario() 
        {
            date = DateTime.Parse("2024-01-01");
            dataInicial = DateTime.Parse("2024-01-01");
        }

        public Calendario(CalendarioData data)
        {
            date = DateTime.Parse(data.Ano + "-" + data.Mes + "-" + data.Dia);
            dataInicial = DateTime.Parse("2024-01-01");
        }

        public string GetDataString()
        {
            return date.Day + " de " + meses[date.Month-1]+"\nAno "+(date.Year-2023);
        }

        public int GetMesAtual()
        {
            return date.Month;
        }

        public int GetAnoAtual()
        {
            return date.Year;
        }

        public int GetDiaAtual()
        {
            return date.Day;
        }

        public int GetDiasMes(int mes)
        {
            return DateTime.DaysInMonth(date.Year, mes);
        }

        public void IncrementarDia()
        {
            date = date.AddDays(1);
        }

        public string GetMesNome(int id)
        {
            return meses[id - 1];
        }

        public CalendarioData ParaCalendarioData()
        {
            return new CalendarioData(date.Year,date.Month,date.Day);
        }

        [System.Serializable]
        public struct CalendarioData
        {
            public int Ano;
            public int Mes;
            public int Dia;

            public CalendarioData(int ano, int mes, int dia)
            {
                this.Ano = ano;
                this.Mes = mes;
                this.Dia = dia;
            }

            public static CalendarioData CriarFromDia(int dia)
            {
                DateTime data = dataInicial.AddDays(dia-1);
                return new CalendarioData
                {
                    Ano = data.Year,
                    Mes = data.Month,
                    Dia = data.Day
                };
            }

            public static bool operator == (CalendarioData x, CalendarioData y)
            {
                return x.Ano == y.Ano && x.Mes == y.Mes && x.Dia == y.Dia;
            }

            public static bool operator != (CalendarioData x, CalendarioData y)
            {
                return !(x == y);
            }

            public string GetDataString()
            {
                return (Ano-2023)+"-"+Mes+"-"+Dia;
            }

            public string GetDataStringFancy()
            {
                return Dia + " de " + meses[Mes-1];
            }
        }

    }
}
