using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Text;
using EmpreendaVc.Domain;
using EmpreendaVc.Web.Mvc.Controllers.ViewModels;


namespace EmpreendaVc.Web.Mvc.Util
{
    public static class Util
    {
        public static string RetornaPosicao(int pos)
        {
            if (pos == 1)
                return "G";
            else if (pos == 2)
                return "LD";
            else if (pos == 3)
                return "Z";
            else if (pos == 4)
                return "LE";
            else if (pos == 5)
                return "V";
            else if (pos == 6)
                return "MO";
            else
                return "A";
        }

        public static string RetornaPosicaoCompleta(int pos)
        {
            if (pos == 1)
                return "GOLEIRO";
            else if (pos == 2)
                return "LATERAL-DIREITO";
            else if (pos == 3)
                return "ZAGUEIRO";
            else if (pos == 4)
                return "LATERAL-ESQUERDO";
            else if (pos == 5)
                return "VOLANTE";
            else if (pos == 6)
                return "MEIO-OFENSIVO";
            else
                return "ATACANTE";
        }

        public static List<SelectListItem> RetornaListaPosicao()
        {
            var lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Text = "GOLEIRO", Value = "1" });
            lst.Add(new SelectListItem { Text = "LATERAL-DIREITO", Value = "2" });
            lst.Add(new SelectListItem { Text = "ZAGUEIRO", Value = "3" });
            lst.Add(new SelectListItem { Text = "LATERA-ESQUERDO", Value = "4" });
            lst.Add(new SelectListItem { Text = "VOLANTE", Value = "5" });
            lst.Add(new SelectListItem { Text = "MEIA-OFENSIVO", Value = "6" });
            lst.Add(new SelectListItem { Text = "ATACANTE", Value = "7" });

            return lst;
        }

        public static List<SelectListItem> RetornaListaContrato()
        {
            var lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Text = "SEM CLUBE", Value = "-1" });
            lst.Add(new SelectListItem { Text = "TERMINANDO", Value = "1" });

            return lst;
        }

        public static List<SelectListItem> RetornaListaOrdenacao()
        {
            var lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Text = "NOME", Value = "0" });
            lst.Add(new SelectListItem { Text = "POSIÇÃO - D > A", Value = "1" });
            lst.Add(new SelectListItem { Text = "POSIÇÃO - A > D", Value = "2" });
            lst.Add(new SelectListItem { Text = "NOTA MÉDIA", Value = "3" });

            return lst;
        }

        public static List<SelectListItem> RetornaPosicaoEscalacao(IList<Escalacao> lstEscalacao)
        {
            return lstEscalacao.Select(x => new SelectListItem() { Text = RetornaPosicao(x.Posicao), Value = x.Id.ToString() }).ToList();
        }

        public static int RetornaHabilidadePosicao(Jogador jogador, int posformacao)
        {
            int h = jogador.H;

            if (jogador.Posicao == 1)
            {
                if (posformacao != 1)
                    h = 1;
            }
            else if (jogador.Posicao == 2)
            {
                if (posformacao == 1)
                    h = 1;
                else if (posformacao == 3)
                    h = jogador.H - 30;
                else if (posformacao == 4)
                    h = jogador.H - 10;
                else if (posformacao == 5)
                    h = jogador.H - 20;
                else if (posformacao == 6)
                    h = jogador.H - 40;
                else if (posformacao == 7)
                    h = jogador.H - 60;
            }
            else if (jogador.Posicao == 3)
            {
                if (posformacao == 1)
                    h = 1;
                else if (posformacao == 2 || posformacao == 4)
                    h = jogador.H - 20;
                else if (posformacao == 5)
                    h = jogador.H - 20;
                else if (posformacao == 6)
                    h = jogador.H - 40;
                else if (posformacao == 7)
                    h = jogador.H - 60;
            }
            else if (jogador.Posicao == 4)
            {
                if (posformacao == 1)
                    h = 1;
                else if (posformacao == 3)
                    h = jogador.H - 30;
                else if (posformacao == 2)
                    h = jogador.H - 10;
                else if (posformacao == 5)
                    h = jogador.H - 20;
                else if (posformacao == 6)
                    h = jogador.H - 40;
                else if (posformacao == 7)
                    h = jogador.H - 60;
            }
            else if (jogador.Posicao == 5)
            {
                if (posformacao == 1)
                    h = 1;
                else if (posformacao == 2 || posformacao == 4)
                    h = jogador.H - 20;
                else if (posformacao == 3)
                    h = jogador.H - 20;
                else if (posformacao == 6)
                    h = jogador.H - 30;
                else if (posformacao == 7)
                    h = jogador.H - 60;
            }
            else if (jogador.Posicao == 6)
            {
                if (posformacao == 1)
                    h = 1;
                else if (posformacao == 2 || posformacao == 4)
                    h = jogador.H - 40;
                else if (posformacao == 3)
                    h = jogador.H - 60;
                else if (posformacao == 5)
                    h = jogador.H - 30;
                else if (posformacao == 7)
                    h = jogador.H - 20;
            }
            else
            {
                if (posformacao == 1)
                    h = 1;
                else if (posformacao == 2 || posformacao == 4)
                    h = jogador.H - 60;
                else if (posformacao == 3)
                    h = jogador.H - 60;
                else if (posformacao == 5)
                    h = jogador.H - 40;
                else if (posformacao == 6)
                    h = jogador.H - 20;
            }

            if (h >= 90)
                h = h + 20;
            else if (h >= 80)
                h = h + 10;
            else if (h >= 70)
                h = h + 5;

            if (h < 0)
                h = 1;

            return h;
        }

        public static List<SelectListItem> RetornaValorVenda()
        {
            var lst = new List<SelectListItem>();
            decimal valor = 0;

            while (valor <= 80000000)
            {
                lst.Add(new SelectListItem { Text = "$" + valor.ToString("N2"), Value = valor.ToString() });

                if (valor <= 300000)
                    valor = valor + 50000;
                else if (valor <= 250000)
                    valor = valor + 250000;
                else if (valor <= 10000000)
                    valor = valor + 500000;
                else if (valor <= 30000000)
                    valor = valor + 1000000;
                else if (valor <= 50000000)
                    valor = valor + 2500000;
                else if (valor > 50000000)
                    valor = valor + 5000000;
            }

            return lst;
        }

        public static List<SelectListItem> RetornaValorSalario()
        {
            var lst = new List<SelectListItem>();
            decimal valor = 0;

            while (valor <= 1000000)
            {
                lst.Add(new SelectListItem { Text = "$" + valor.ToString("N2"), Value = valor.ToString() });

                if (valor <= 10000)
                    valor = valor + 1000;
                else if (valor <= 50000)
                    valor = valor + 2500;
                else if (valor <= 100000)
                    valor = valor + 5000;
                else if (valor <= 300000)
                    valor = valor + 10000;
                else if (valor <= 1000000)
                    valor = valor + 25000;
                else if (valor > 1000000)
                    valor = valor + 50000;
            }

            return lst;
        }

        public static List<SelectListItem> RetornaDuracaoContrato()
        {
            var lst = new List<SelectListItem>();
            int temporada = 1;

            while (temporada < 5)
            {
                lst.Add(new SelectListItem { Text = temporada.ToString() + " ano(s)", Value = temporada.ToString() });

                temporada++;
            }

            return lst;
        }

        public static List<SelectListItem> RetornaSituacao()
        {
            var lst = new List<SelectListItem>();
            
            lst.Add(new SelectListItem { Text = "Este jogador é muito imporante para o clube", Value = "1" });
            lst.Add(new SelectListItem { Text = "Este jogador é considerado reserva no clube", Value = "2" });
            lst.Add(new SelectListItem { Text = "Este jogador está disponível para venda", Value = "3" });

            return lst;
        }

        public static List<SelectListItem> RetornaListaIngresso()
        {
            var lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Text = "$10,00", Value = "10" });
            lst.Add(new SelectListItem { Text = "$15,00", Value = "15" });
            lst.Add(new SelectListItem { Text = "$20,00", Value = "20" });
            lst.Add(new SelectListItem { Text = "$25,00", Value = "25" });
            lst.Add(new SelectListItem { Text = "$30,00", Value = "30" });
            lst.Add(new SelectListItem { Text = "$35,00", Value = "35" });
            lst.Add(new SelectListItem { Text = "$40,00", Value = "40" });
            lst.Add(new SelectListItem { Text = "$45,00", Value = "45" });
            lst.Add(new SelectListItem { Text = "$50,00", Value = "50" });

            return lst;
        }

        public static List<SelectListItem> SelectListMeses()
        {
            var lst = new List<SelectListItem>();
            int mes = 1;
            foreach (var item in NomesDosMeses())
            {
                lst.Add(new SelectListItem { Text = item, Value = mes.ToString(), Selected = mes == DateTime.Now.Month });
                mes++;
            }

            return lst;
        }

        public static string[] NomesDosMeses()
        {

            CultureInfo ci = new CultureInfo("pt-BR");
            DateTimeFormatInfo dtfi = ci.DateTimeFormat;

            return dtfi.MonthGenitiveNames.Where(x => string.IsNullOrEmpty(x) == false).ToArray();
        }

        public static string CurrentUserLogin
        {
            get
            {
                string login = string.Empty;

                if (HttpContext.Current.Request.IsAuthenticated)
                {
                    login = HttpContext.Current.User.Identity.Name.Split('|')[0];
                }

                return login;
            }
        }


        public static SelectListItem CountryItem(string value)
        {
            var item = CountrySelectListItem().FirstOrDefault(x => x.Value == value);

            if (item == null)
                item = new SelectListItem();

            return item;
        }


        public static List<SelectListItem> CountrySelectListItem()
        {
            var lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Value = "ZA", Text = "África do Sul" });
            lst.Add(new SelectListItem { Value = "DE", Text = "Alemanha" });
            lst.Add(new SelectListItem { Value = "SA", Text = "Arábia Saudita" });
            lst.Add(new SelectListItem { Value = "AR", Text = "Argentina" });
            lst.Add(new SelectListItem { Value = "AU", Text = "Austrália" });
            lst.Add(new SelectListItem { Value = "AT", Text = "Áustria" });
            lst.Add(new SelectListItem { Value = "BE", Text = "Bélgica" });
            lst.Add(new SelectListItem { Value = "BR", Text = "Brasil" });
            lst.Add(new SelectListItem { Value = "BG", Text = "Bulgária" });
            lst.Add(new SelectListItem { Value = "CA", Text = "Canadá" });
            lst.Add(new SelectListItem { Value = "QA", Text = "Catar" });
            lst.Add(new SelectListItem { Value = "CL", Text = "Chile" });
            lst.Add(new SelectListItem { Value = "CN", Text = "China" });
            lst.Add(new SelectListItem { Value = "CY", Text = "Chipre" });
            lst.Add(new SelectListItem { Value = "SG", Text = "Cingapura" });
            lst.Add(new SelectListItem { Value = "CO", Text = "Colômbia" });
            lst.Add(new SelectListItem { Value = "KR", Text = "Coreia do Sul" });
            lst.Add(new SelectListItem { Value = "CR", Text = "Costa Rica" });
            lst.Add(new SelectListItem { Value = "HR", Text = "Croácia" });
            lst.Add(new SelectListItem { Value = "DK", Text = "Dinamarca" });
            lst.Add(new SelectListItem { Value = "EG", Text = "Egito" });
            lst.Add(new SelectListItem { Value = "AE", Text = "Emirados Árabes Unidos" });
            lst.Add(new SelectListItem { Value = "EC", Text = "Equador" });
            lst.Add(new SelectListItem { Value = "SK", Text = "Eslováquia" });
            lst.Add(new SelectListItem { Value = "SI", Text = "Eslovênia" });
            lst.Add(new SelectListItem { Value = "ES", Text = "Espanha" });
            lst.Add(new SelectListItem { Value = "US", Text = "Estados Unidos" });
            lst.Add(new SelectListItem { Value = "EE", Text = "Estônia" });
            lst.Add(new SelectListItem { Value = "PH", Text = "Filipinas" });
            lst.Add(new SelectListItem { Value = "FI", Text = "Finlândia" });
            lst.Add(new SelectListItem { Value = "FR", Text = "França" });
            lst.Add(new SelectListItem { Value = "GR", Text = "Grécia" });
            lst.Add(new SelectListItem { Value = "GL", Text = "Groenlândia" });
            lst.Add(new SelectListItem { Value = "GT", Text = "Guatemala" });
            lst.Add(new SelectListItem { Value = "HK", Text = "Hong Kong" });
            lst.Add(new SelectListItem { Value = "HU", Text = "Hungria" });
            lst.Add(new SelectListItem { Value = "IN", Text = "Índia" });
            lst.Add(new SelectListItem { Value = "ID", Text = "Indonésia" });
            lst.Add(new SelectListItem { Value = "IE", Text = "Irlanda" });
            lst.Add(new SelectListItem { Value = "IS", Text = "Islândia" });
            lst.Add(new SelectListItem { Value = "IL", Text = "Israel" });
            lst.Add(new SelectListItem { Value = "IT", Text = "Itália" });
            lst.Add(new SelectListItem { Value = "JM", Text = "Jamaica" });
            lst.Add(new SelectListItem { Value = "JP", Text = "Japão" });
            lst.Add(new SelectListItem { Value = "JO", Text = "Jordânia" });
            lst.Add(new SelectListItem { Value = "KW", Text = "Kuwait" });
            lst.Add(new SelectListItem { Value = "LI", Text = "Listenstaine" });
            lst.Add(new SelectListItem { Value = "LT", Text = "Lituânia" });
            lst.Add(new SelectListItem { Value = "LU", Text = "Luxemburgo" });
            lst.Add(new SelectListItem { Value = "MY", Text = "Malásia" });
            lst.Add(new SelectListItem { Value = "MV", Text = "Maldivas" });
            lst.Add(new SelectListItem { Value = "MT", Text = "Malta" });
            lst.Add(new SelectListItem { Value = "MX", Text = "México" });
            lst.Add(new SelectListItem { Value = "MC", Text = "Mônaco" });
            lst.Add(new SelectListItem { Value = "NO", Text = "Noruega" });
            lst.Add(new SelectListItem { Value = "NZ", Text = "Nova Zelândia" });
            lst.Add(new SelectListItem { Value = "OM", Text = "Omã" });
            lst.Add(new SelectListItem { Value = "NL", Text = "Países Baixos" });
            lst.Add(new SelectListItem { Value = "PA", Text = "Panamá" });
            lst.Add(new SelectListItem { Value = "PY", Text = "Paraguai" });
            lst.Add(new SelectListItem { Value = "PE", Text = "Peru" });
            lst.Add(new SelectListItem { Value = "PL", Text = "Polônia" });
            lst.Add(new SelectListItem { Value = "PR", Text = "Porto Rico" });
            lst.Add(new SelectListItem { Value = "PT", Text = "Portugal" });
            lst.Add(new SelectListItem { Value = "GB", Text = "Reino Unido" });
            lst.Add(new SelectListItem { Value = "DO", Text = "República Dominicana" });
            lst.Add(new SelectListItem { Value = "CZ", Text = "República Tcheca" });
            lst.Add(new SelectListItem { Value = "RO", Text = "Romênia" });
            lst.Add(new SelectListItem { Value = "RU", Text = "Rússia" });
            lst.Add(new SelectListItem { Value = "RS", Text = "Sérvia" });
            lst.Add(new SelectListItem { Value = "SE", Text = "Suécia" });
            lst.Add(new SelectListItem { Value = "CH", Text = "Suíça" });
            lst.Add(new SelectListItem { Value = "TH", Text = "Tailândia" });
            lst.Add(new SelectListItem { Value = "TW", Text = "Taiwan" });
            lst.Add(new SelectListItem { Value = "TR", Text = "Turquia" });
            lst.Add(new SelectListItem { Value = "UA", Text = "Ucrânia" });
            lst.Add(new SelectListItem { Value = "VE", Text = "Venezuela" });
            lst.Add(new SelectListItem { Value = "VN", Text = "Vietnã" });

            return lst;
        }        

        public static string GetStringSemAcentos(string str)
        {
            return HttpUtility.UrlEncode(str, Encoding.GetEncoding(28597)).Replace("+", "").Replace(" ", "");
        }

        public static string GetNomeArquivoUpload(string p)
        {
            var arr = p.Replace("\\", "|").Split('|');
            var c = arr.Count();
            if (c > 1)
            {
                var posicao = arr[c - 1].ToString();
                return GetStringSemAcentos(posicao);
            }
            else
            {
                return GetStringSemAcentos(p);
            }
        }
    }
}

