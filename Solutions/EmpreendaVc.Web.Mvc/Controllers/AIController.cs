namespace EmpreendaVc.Web.Mvc.Controllers
{
    using System.Web.Mvc;
    using Domain;
    using SharpArch.NHibernate.Web.Mvc;
    using SharpArch.Domain.Commands;
    using SharpArch.NHibernate.Contracts.Repositories;
    using MvcContrib;
    using System.Linq;
    using System.Collections.Generic;
    using MvcContrib.Sorting;
    using MvcContrib.UI.Grid;
    using MvcContrib.Pagination;
    using System;
    using EmpreendaVc.Infrastructure.Queries.Authentication;
    using EmpreendaVc.Infrastructure.Queries.Usuarios;
    using EmpreendaVc.Infrastructure.Queries.Partidas;
    using System.Web.Security;
    using EmpreendaVc.Web.Mvc.Controllers.ViewModels;
    using NHibernate.Criterion;
    using NHibernate.Transform;

    public class AIController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly INHibernateRepository<Clube> clubeRepository;
        private readonly INHibernateRepository<Jogador> jogadorRepository;
        private readonly INHibernateRepository<Controle> controleRepository;
        private readonly INHibernateRepository<JogadorOferta> jogadorofertaRepository;
        private readonly INHibernateRepository<Divisao> divisaoRepository;
        private readonly IPartidaRepository partidaRepository;
        private readonly INHibernateRepository<Gol> golRepository;
        private readonly INHibernateRepository<DivisaoTabela> divisaotabelaRepository;
        private readonly INHibernateRepository<JogadorPedido> jogadorpedidoRepository;
        private readonly INHibernateRepository<UsuarioOferta> usuarioofertaRepository;
        private readonly INHibernateRepository<Noticia> noticiaRepository;
        private readonly INHibernateRepository<Escalacao> escalacaoRepository;
        private readonly INHibernateRepository<Historico> historicoRepository;
        private readonly INHibernateRepository<Nome> nomeRepository;
        private readonly INHibernateRepository<JogadorHistorico> jogadorhistoricoRepository;

        public AIController(IUsuarioRepository usuarioRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            INHibernateRepository<Jogador> jogadorRepository,
            INHibernateRepository<Controle> controleRepository,
            INHibernateRepository<JogadorOferta> jogadorofertaRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            IPartidaRepository partidaRepository,
            INHibernateRepository<Gol> golRepository,
            INHibernateRepository<DivisaoTabela> divisaotabelaRepository,
            INHibernateRepository<JogadorPedido> jogadorpedidoRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Noticia> noticiaRepository,
            INHibernateRepository<Escalacao> escalacaoRepository,
            INHibernateRepository<Historico> historicoRepository,
            INHibernateRepository<Nome> nomeRepository,
            INHibernateRepository<JogadorHistorico> jogadorhistoricoRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.authenticationService = authenticationService;
            this.clubeRepository = clubeRepository;
            this.jogadorRepository = jogadorRepository;
            this.controleRepository = controleRepository;
            this.jogadorofertaRepository = jogadorofertaRepository;
            this.divisaoRepository = divisaoRepository;
            this.partidaRepository = partidaRepository;
            this.golRepository = golRepository;
            this.divisaotabelaRepository = divisaotabelaRepository;
            this.jogadorpedidoRepository = jogadorpedidoRepository;
            this.usuarioofertaRepository = usuarioofertaRepository;
            this.noticiaRepository = noticiaRepository;
            this.escalacaoRepository = escalacaoRepository;
            this.historicoRepository = historicoRepository;
            this.nomeRepository = nomeRepository;
            this.jogadorhistoricoRepository = jogadorhistoricoRepository;
        }

        
        [Transaction]
        public ActionResult Index()
        {
            return View();
        }

        [Transaction]
        public ActionResult RenovaContratos()
        {
            foreach (var clube in clubeRepository.GetAll().Where(x => x.Usuario == null).OrderByDescending(x => x.Dinheiro))
            {
                var g = 2;
                var ld = 2;
                var le = 2;
                var z = 3;
                var v = 3;
                var mo = 3;
                var a = 3;

                foreach (var jogador in clube.Jogadores.Where(x => x.Contrato == 1 && !x.Temporario).OrderByDescending(x => x.HF))
                {
                    bool renova = false;
                    if (jogador.Posicao == 1 && g > 0)
                    {
                        renova = true;
                        g--;
                    }
                    else if (jogador.Posicao == 2 && ld > 0)
                    {
                        renova = true;
                        ld--;
                    }
                    else if (jogador.Posicao == 3 && z > 0)
                    {
                        renova = true;
                        z--;
                    }
                    else if (jogador.Posicao == 4 && le > 0)
                    {
                        renova = true;
                        le--;
                    }
                    else if (jogador.Posicao == 5 && v > 0)
                    {
                        renova = true;
                        v--;
                    }
                    else if (jogador.Posicao == 6 && mo > 0)
                    {
                        renova = true;
                        mo--;
                    }
                    else if (jogador.Posicao == 7 && a > 0)
                    {
                        renova = true;
                        a--;
                    }

                    if (renova)
                    {
                        jogador.Contrato = 2;
                        jogadorRepository.SaveOrUpdate(jogador);
                    }
                }
            }
            return RedirectToAction("Index", "AI");
        }

        [Transaction]
        public ActionResult ComprarJogador()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            foreach (var clube in clubeRepository.GetAll().Where(x => x.Usuario == null))//.GetAll()
            {
                var dinheiro = clube.Dinheiro - 500000;
                if (dinheiro > 0)
                {
                    var gtotal = clube.Jogadores.Where(x => x.Posicao == 1 && !x.Temporario).Count();
                    var g = clube.Jogadores.Where(x => x.Posicao == 1 && !x.Temporario).Sum(x => x.H) / gtotal;
                    var ldtotal = clube.Jogadores.Where(x => x.Posicao == 2 && !x.Temporario).Count();
                    var ld = clube.Jogadores.Where(x => x.Posicao == 2 && !x.Temporario).Sum(x => x.H) / ldtotal;
                    var ztotal = clube.Jogadores.Where(x => x.Posicao == 3 && !x.Temporario).Count();
                    var z = clube.Jogadores.Where(x => x.Posicao == 3 && !x.Temporario).Sum(x => x.H) / ztotal;
                    var letotal = clube.Jogadores.Where(x => x.Posicao == 4 && !x.Temporario).Count();
                    var le = clube.Jogadores.Where(x => x.Posicao == 4 && !x.Temporario).Sum(x => x.H) / letotal;
                    var vtotal = clube.Jogadores.Where(x => x.Posicao == 5 && !x.Temporario).Count();
                    var v = clube.Jogadores.Where(x => x.Posicao == 5 && !x.Temporario).Sum(x => x.H) / vtotal;
                    var mototal = clube.Jogadores.Where(x => x.Posicao == 6 && !x.Temporario).Count();
                    var mo = clube.Jogadores.Where(x => x.Posicao == 6 && !x.Temporario).Sum(x => x.H) / mototal;
                    var atotal = clube.Jogadores.Where(x => x.Posicao == 7 && !x.Temporario).Count();
                    var a = clube.Jogadores.Where(x => x.Posicao == 7 && !x.Temporario).Sum(x => x.H) / atotal;

                    var lstjogadores = jogadorRepository.GetAll().Where(x => (x.Clube == null || (x.Clube != null && x.Clube.Id != clube.Id)) && !x.Temporario);                                                             
                    var lstjogadoroferta = jogadorofertaRepository.GetAll();
                    var lstcompra = new List<Jogador>();

                    ////////////////////////////////////////////////////////////////////////////////////////////////
                    ////////////////////////////////////////////////////////////////////////////////////////////////NECESSIDADES
                    ////////////////////////////////////////////////////////////////////////////////////////////////
                    for (int i = 7; i > 0; i--)
                    {
                        var pos = i;
                        var posmedia = 0;
                        var postotal = 0;
                        var posmin = 3;
                        if (pos == 7) { posmedia = a; postotal = atotal; }
                        else if (pos == 6) { posmedia = mo; postotal = mototal; }
                        else if (pos == 5) { posmedia = v; postotal = vtotal; }
                        else if (pos == 4) { posmedia = le; postotal = letotal; posmin = 2; }
                        else if (pos == 3) { posmedia = z; postotal = ztotal; }
                        else if (pos == 2) { posmedia = ld; postotal = ldtotal; posmin = 2; }
                        else { posmedia = g; postotal = gtotal; posmin = 2; }

                        if (postotal < posmin)
                        {
                            var lsta = lstjogadores.Where(x => x.Posicao == pos && x.H <= posmedia);

                            if (lsta.Where(x => x.Clube == null && (x.H > (posmedia - 30) && dinheiro > 1000000)  ).Count() > 0)
                            {
                                var jogador = lsta.Where(x => x.Clube == null).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia).FirstOrDefault();
                                dinheiro = dinheiro - jogador.Valor;
                                lstcompra.Add(jogador);
                            }
                            else
                            {
                                foreach (var jogador in lsta.OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia))
                                {
                                    if (lstjogadoroferta.Where(x => x.Jogador.Id == jogador.Id && x.Estagio == 3).Count() == 0)
                                    {
                                        var ofertas = lstjogadoroferta.Where(x => x.Clube.Id == clube.Id && x.Jogador.Id == jogador.Id).Count();
                                        if (ofertas < 2)
                                        {
                                            var valor = jogador.Valor;

                                            if (ofertas > 0)
                                                valor = jogador.Valor + (jogador.Valor / 2);

                                            if (dinheiro >= valor)
                                            {
                                                dinheiro = dinheiro - jogador.Valor;
                                                lstcompra.Add(jogador);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }   
                    }                    
                    
                    ////////////////////////////////////////////////////////////////////////////////////////////////
                    ////////////////////////////////////////////////////////////////////////////////////////////////LUXOS
                    ////////////////////////////////////////////////////////////////////////////////////////////////
                    var pos2 = 7;
                    var posmedia2 = a;
                    if (g <= a && g <= mo && g <= v && g <= le && g <= ld && g <= z)
                    {
                        pos2 = 1;
                        posmedia2 = g;
                    }
                    else if (ld <= a && ld <= mo && ld <= v && ld <= le && ld <= g && ld <= z)
                    {
                        pos2 = 2;
                        posmedia2 = ld;
                    }
                    else if (z <= a && z <= mo && z <= v && z <= le && z <= g && z <= ld)
                    {
                        pos2 = 3;
                        posmedia2 = z;
                    }
                    else if (le <= a && le <= mo && le <= v && le <= z && le <= g && le <= ld)
                    {
                        pos2 = 4;
                        posmedia2 = le;
                    }
                    else if (v <= a && v <= mo && v <= le && v <= z && v <= g && v <= ld)
                    {
                        pos2 = 5;
                        posmedia2 = v;
                    }
                    else if (mo <= a && mo <= le && mo <= v && mo <= z && mo <= g && mo <= ld)
                    {
                        pos2 = 6;
                        posmedia2 = mo;
                    }

                    if (dinheiro > 0)
                    {
                        var lsta = lstjogadores.Where(x => x.Posicao == pos2);
                        if (dinheiro > 4000000)
                            lsta = lsta.Where(x => x.H > (posmedia2 + 20));
                        else if (dinheiro > 1000000)
                            lsta = lsta.Where(x => x.H > (posmedia2 + 10));
                        else if (dinheiro <= 1000000)
                            lsta = lsta.Where(x => x.H > posmedia2);

                        if (lsta.Where(x => x.Clube == null).Count() > 0)
                        {
                            var jogador = lsta.Where(x => x.Clube == null).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia).FirstOrDefault();
                            dinheiro = dinheiro - jogador.Valor;
                            lstcompra.Add(jogador);                        
                        }
                        else
                        {
                            foreach (var jogador in lsta.OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia))
                            {
                                if (lstjogadoroferta.Where(x => x.Jogador.Id == jogador.Id && x.Estagio == 3).Count() == 0)
                                {
                                    var ofertas = lstjogadoroferta.Where(x => x.Clube.Id == clube.Id && x.Jogador.Id == jogador.Id).Count();
                                    if (ofertas < 2)
                                    {
                                        var valor = jogador.Valor;

                                        if (ofertas > 0)
                                            valor = jogador.Valor + (jogador.Valor / 2);

                                        if (dinheiro >= valor)
                                        {
                                            dinheiro = dinheiro - jogador.Valor;
                                            lstcompra.Add(jogador);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var lstaSemClube = lstjogadores.Where(x => x.Clube == null)
                                                    .Where(x => (x.Posicao == 1 && x.H > (g + 5)) || (x.Posicao == 2 && x.H > (ld + 5)) || (x.Posicao == 3 && x.H > (z + 5)) || (x.Posicao == 4 && x.H > (le + 5)) || (x.Posicao == 5 && x.H > (v + 5)) || (x.Posicao == 6 && x.H > (mo + 5)) || (x.Posicao == 7 && x.H > (a + 5)));

                    foreach(var jogador in lstaSemClube.OrderByDescending(x => x.H))
                    {
                        if (lstcompra.Where(x => x.Posicao == jogador.Posicao).Count() == 0)
                            lstcompra.Add(jogador);
                    }

                    dinheiro = clube.Dinheiro - 500000;

                    foreach (var jogador in lstcompra)
                    {
                        decimal valor = 0;
                        var ofertas = lstjogadoroferta.Where(x => x.Clube.Id == clube.Id && x.Jogador.Id == jogador.Id).Count();

                        if (jogador.Clube != null)
                            valor = jogador.Valor;
                        else
                            valor = 0;

                        if (ofertas > 0 && valor > 0)
                            valor = valor + (valor / 2);

                        var naofez = lstjogadoroferta.Where(x => x.Clube.Id == clube.Id && x.Jogador.Id == jogador.Id && x.Estagio > 0 && x.Estagio < 3).Count() == 0;

                        if (dinheiro >= valor && naofez)
                        {
                            dinheiro = (dinheiro - valor) > 0 ? (dinheiro - valor) : 0;

                            var rnd = new Random();

                            var jogadoroferta = new JogadorOferta();
                            jogadoroferta.Clube = clube;
                            jogadoroferta.Dia = controle.Dia;
                            jogadoroferta.Jogador = jogador;
                            if (jogador.Clube != null)
                                jogadoroferta.Tipo = 1;
                            else
                            {
                                jogadoroferta.Estagio = 2;
                                jogadoroferta.Tipo = 2;
                            }
                            jogadoroferta.Valor = valor;
                            jogadoroferta.Salario = jogador.Salario > 0 ? (jogador.Salario / 100) * (rnd.Next(12, 16) * 10) : ((jogador.H / 4) * 3) * (rnd.Next(9, 13) * 100);

                            var pontos = jogador.Salario > 0 ? Convert.ToInt32((jogadoroferta.Salario - jogador.Salario) / 10000) : 1;

                            if (jogador.Clube != null)
                                pontos = pontos + (jogador.Clube.Divisao.Numero - jogadoroferta.Clube.Divisao.Numero);
                            else
                                pontos = pontos + (jogadoroferta.Clube.Socios / 1000);

                            jogadoroferta.Pontos = pontos;
                            jogadoroferta.Contrato = 3;
                            jogadorofertaRepository.SaveOrUpdate(jogadoroferta);

                            if (jogador.Clube != null && jogador.Clube.Usuario != null)
                            {
                                var noticia = new Noticia();
                                noticia.Dia = controle.Dia;
                                noticia.Texto = jogadoroferta.Clube.Nome + " apresentou uma proposta por <a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a>. <br /><br />Entre em <a href='" + Url.Action("MeusJogadores", "Jogador") + "'>PROPOSTAS</a> para responder.";
                                noticia.Usuario = jogador.Clube.Usuario;

                                noticiaRepository.SaveOrUpdate(noticia);
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index", "AI");
        }

        [Transaction]
        public ActionResult DefinirSituacaoJogador()
        {
            foreach (var clube in clubeRepository.GetAll().Where(x => x.Usuario == null))
            {
                var controle = controleRepository.GetAll().FirstOrDefault();

                var gtotal = clube.Jogadores.Where(x => x.Posicao == 1 && !x.Temporario).Count();
                var g = clube.Jogadores.Where(x => x.Posicao == 1 && !x.Temporario).Sum(x => x.H) / gtotal;     
                var ldtotal = clube.Jogadores.Where(x => x.Posicao == 2 && !x.Temporario).Count();
                var ld = clube.Jogadores.Where(x => x.Posicao == 2 && !x.Temporario).Sum(x => x.H) / ldtotal;
                var ztotal = clube.Jogadores.Where(x => x.Posicao == 3 && !x.Temporario).Count();
                var z = clube.Jogadores.Where(x => x.Posicao == 3 && !x.Temporario).Sum(x => x.H) / ztotal;
                var letotal = clube.Jogadores.Where(x => x.Posicao == 4 && !x.Temporario).Count();
                var le = clube.Jogadores.Where(x => x.Posicao == 4 && !x.Temporario).Sum(x => x.H) / letotal;
                var vtotal = clube.Jogadores.Where(x => x.Posicao == 5 && !x.Temporario).Count();
                var v = clube.Jogadores.Where(x => x.Posicao == 5 && !x.Temporario).Sum(x => x.H) / vtotal;
                var mototal =clube.Jogadores.Where(x => x.Posicao == 6 && !x.Temporario).Count();
                var mo = clube.Jogadores.Where(x => x.Posicao == 6 && !x.Temporario).Sum(x => x.H) / mototal;
                var atotal = clube.Jogadores.Where(x => x.Posicao == 7 && !x.Temporario).Count();
                var a = clube.Jogadores.Where(x => x.Posicao == 7 && !x.Temporario).Sum(x => x.H) / atotal;

                foreach (var jogador in clube.Jogadores)
                {
                    bool dispensa = false;
                    if (jogador.Posicao == 1)
                    {
                        if (jogador.H > g)
                            jogador.Situacao = 1;
                        else if (jogador.H > (g - 10))
                            jogador.Situacao = 3;
                        else
                        {
                            if (clube.Jogadores.Count() > 14 && gtotal > 2)
                                dispensa = true;
                        }
                    }
                    if (jogador.Posicao == 2)
                    {
                        if (jogador.H > ld)
                            jogador.Situacao = 1;
                        else if (jogador.H > (ld - 10))
                            jogador.Situacao = 3;
                        else
                        {
                            if (clube.Jogadores.Count() > 14 && ldtotal > 2)
                                dispensa = true;
                        }
                    }
                    if (jogador.Posicao == 3)
                    {
                        if (jogador.H > z)
                            jogador.Situacao = 1;
                        else if (jogador.H > (z - 10))
                            jogador.Situacao = 3;
                        else
                        {
                            if (clube.Jogadores.Count() > 14 && ztotal > 3)
                                dispensa = true;
                        }
                    }
                    if (jogador.Posicao == 4)
                    {
                        if (jogador.H > le)
                            jogador.Situacao = 1;
                        else if (jogador.H > (le - 10))
                            jogador.Situacao = 3;
                        else
                        {
                            if (clube.Jogadores.Count() > 14 && letotal > 2)
                                dispensa = true;
                        }
                    }
                    if (jogador.Posicao == 5)
                    {
                        if (jogador.H > v)
                            jogador.Situacao = 1;
                        else if (jogador.H > (v - 10))
                            jogador.Situacao = 3;
                        else
                        {
                            if (clube.Jogadores.Count() > 14 && vtotal > 3)
                                dispensa = true;
                        }
                    }
                    if (jogador.Posicao == 6)
                    {
                        if (jogador.H > mo)
                            jogador.Situacao = 1;
                        else if (jogador.H > (mo - 10))
                            jogador.Situacao = 3;
                        else
                        {
                            if (clube.Jogadores.Count() > 14 && mototal > 3)
                                dispensa = true;
                        }
                    }
                    if (jogador.Posicao == 7)
                    {
                        if (jogador.H > a)
                            jogador.Situacao = 1;
                        else if (jogador.H > (a - 10))
                            jogador.Situacao = 3;
                        else
                        {
                            if (clube.Jogadores.Count() > 14 && atotal > 3)
                                dispensa = true;
                        }
                    }

                    if (dispensa)
                    {
                        foreach (var jogadoroferta in jogadorofertaRepository.GetAll().Where(x => x.Jogador.Id == jogador.Id))
                        {
                            if (jogadoroferta.Clube.Usuario != null)
                            {
                                var noticia = new Noticia();
                                noticia.Dia = controle.Dia;
                                noticia.Texto = "Sua proposta por <a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> foi cancelada, pois o mesmo foi dispensado pelo " + clube.Nome + ".";
                                noticia.Usuario = jogadoroferta.Clube.Usuario;
                            }

                            jogadorofertaRepository.Delete(jogadoroferta);
                        }

                        var historico = new JogadorHistorico();
                        historico.Ano = controle.Ano;
                        historico.Clube = clube;
                        historico.Gols = jogador.Gols.Where(x => x.Clube.Id == clube.Id).Count();
                        historico.Jogador = jogador;
                        historico.Jogos = jogador.Jogos;
                        historico.NotaMedia = jogador.NotaMedia > 0.0 ? jogador.NotaMedia : 0.00;
                        historico.Valor = 0;
                        jogadorhistoricoRepository.SaveOrUpdate(historico);

                        jogador.Clube = null;
                        jogador.Contrato = 0;
                        jogador.Salario = 0;
                        jogador.Jogos = 0;
                        jogador.NotaTotal = 0;
                        jogador.NotaUlt = 0;
                        jogador.Treinos = 0;
                        jogador.TreinoTotal = 0;
                    }

                    jogadorRepository.SaveOrUpdate(jogador);
                }
            }
            return RedirectToAction("Index", "AI");
        }

        [Transaction]
        public ActionResult ResponderPropostas()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var lstjogadoroferta = jogadorofertaRepository.GetAll();

            foreach (var jogadoroferta in lstjogadoroferta.Where(x => x.Jogador.Clube != null && x.Estagio > 0 && x.Estagio < 3))
            {
                var jogador = jogadoroferta.Jogador;
                var clube = jogador.Clube;
                decimal valormin = jogador.Valor;                 

                if (lstjogadoroferta.Where(x => x.Jogador.Id == jogador.Id && x.Estagio == 3).Count() > 0)
                    valormin = valormin * 2;
                else if (clube.Dinheiro < 300000)
                    valormin = (valormin / 100) * 80;

                if (jogadoroferta.Valor >= valormin)
                {
                    if (jogadoroferta.Clube.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Sua proposta por <a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> foi aceita pelo " + jogador.Clube.Nome + ", você deve esperar a resposta do contrato do jogador.";
                        noticia.Usuario = jogadoroferta.Clube.Usuario;
                    }

                    jogadoroferta.Estagio = 2;
                    jogadorofertaRepository.SaveOrUpdate(jogadoroferta);
                }
                else
                {
                    if (jogadoroferta.Clube.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Sua proposta por <a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> foi rejeitada pelo " + jogador.Clube.Nome + ".";
                        noticia.Usuario = jogadoroferta.Clube.Usuario;
                    }

                    jogadoroferta.Estagio = 0;
                    jogadorofertaRepository.SaveOrUpdate(jogadoroferta);
                }
            }

            return RedirectToAction("Index", "AI");
        }
    }
}
