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
    using EmpreendaVc.Infrastructure.Queries.Clubes;
    using System.Web.Security;
    using EmpreendaVc.Web.Mvc.Controllers.ViewModels;
    using NHibernate.Criterion;
    using NHibernate.Transform;

    public class AIController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IClubeRepository clubeQueryRepository;
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
        private readonly INHibernateRepository<Sobrenome> sobrenomeRepository;
        private readonly INHibernateRepository<Artilheiro> artilheiroRepository;
        private readonly INHibernateRepository<JogadorHistorico> jogadorhistoricoRepository;
        private readonly INHibernateRepository<Staff> staffRepository;
        private readonly INHibernateRepository<JogadorTeste> jogadortesteRepository;
        private readonly INHibernateRepository<Patrocinio> patrocinioRepository;
        private readonly INHibernateRepository<PatrocinioClube> patrocinioclubeRepository;
        private readonly INHibernateRepository<PatrocinioRecusa> patrociniorecusaRepository;

        public AIController(IUsuarioRepository usuarioRepository,
            IClubeRepository clubeQueryRepository,
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
            INHibernateRepository<Sobrenome> sobrenomeRepository,
            INHibernateRepository<Staff> staffRepository,
            INHibernateRepository<Artilheiro> artilheiroRepository,
            INHibernateRepository<JogadorHistorico> jogadorhistoricoRepository,
            INHibernateRepository<JogadorTeste> jogadortesteRepository, 
            INHibernateRepository<Patrocinio> patrocinioRepository,
            INHibernateRepository<PatrocinioClube> patrocinioclubeRepository,
            INHibernateRepository<PatrocinioRecusa> patrociniorecusaRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.clubeQueryRepository = clubeQueryRepository;
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
            this.sobrenomeRepository = sobrenomeRepository;
            this.artilheiroRepository = artilheiroRepository;
            this.staffRepository = staffRepository;
            this.jogadorhistoricoRepository = jogadorhistoricoRepository;
            this.jogadortesteRepository = jogadortesteRepository;
            this.patrocinioRepository = patrocinioRepository;
            this.patrocinioclubeRepository = patrocinioclubeRepository;
            this.patrociniorecusaRepository = patrociniorecusaRepository;
        }

        
        [Transaction]
        public ActionResult Index()
        {
            return View();
        }

        [Transaction]
        public ActionResult FechaPatrocinios()
        {
            Random rnd = new Random();

            var lstclubes = clubeRepository.GetAll().OrderByDescending(x => x.Socios).ToList();

            foreach (var clube in lstclubes.Where(x => x.Usuario == null && x.PatrocinioClubes.Count() == 0))
            {
                //patrocinios
                var lstpatrocinios = patrocinioRepository.GetAll().Where(x => x.DivisaoMinima == clube.Divisao.Numero && x.Tipo == 1);
                if (lstpatrocinios.Count() < 2)
                    lstpatrocinios = patrocinioRepository.GetAll().Where(x => x.DivisaoMinima > clube.Divisao.Numero && x.Tipo == 1).OrderBy(x => x.DivisaoMinima).Take(8);

                var patroc = lstpatrocinios.ElementAt(rnd.Next(0, (lstpatrocinios.Count() - 1)));
                var patroc2 = lstpatrocinios.ElementAt(rnd.Next(0, (lstpatrocinios.Count() - 1)));

                while (patroc.Id == patroc2.Id)
                {
                    patroc2 = lstpatrocinios.ElementAt(rnd.Next(0, (lstpatrocinios.Count() - 1)));
                }

                var valormax = Util.Util.RetornaValorMaxPatrocinio(patroc, 1, clube, lstclubes);
                var valormax2 = Util.Util.RetornaValorMaxPatrocinio(patroc2, 2, clube, lstclubes);

                var patroclube = new PatrocinioClube();
                patroclube.Clube = clube;
                patroclube.Contrato = 2;
                patroclube.Patrocinio = patroc;
                patroclube.Tipo = 1;
                patroclube.Valor = valormax;
                patrocinioclubeRepository.SaveOrUpdate(patroclube);

                patroclube = new PatrocinioClube();
                patroclube.Clube = clube;
                patroclube.Contrato = 2;
                patroclube.Patrocinio = patroc2;
                patroclube.Tipo = 2;
                patroclube.Valor = valormax2;
                patrocinioclubeRepository.SaveOrUpdate(patroclube);

                //fornecedor
                lstpatrocinios = patrocinioRepository.GetAll().Where(x => x.DivisaoMinima == clube.Divisao.Numero && x.Tipo == 2);
                if (lstpatrocinios.Count() < 2)
                    lstpatrocinios = patrocinioRepository.GetAll().Where(x => x.DivisaoMinima > clube.Divisao.Numero && x.Tipo == 2).OrderBy(x => x.DivisaoMinima).Take(4);

                var fornec = lstpatrocinios.ElementAt(rnd.Next(0, (lstpatrocinios.Count() - 1)));

                var fornecvalormax = Util.Util.RetornaValorMaxPatrocinio(fornec, 3, clube, lstclubes);

                patroclube = new PatrocinioClube();
                patroclube.Clube = clube;
                patroclube.Contrato = 2;
                patroclube.Patrocinio = fornec;
                patroclube.Tipo = 3;
                patroclube.Valor = fornecvalormax;
                patrocinioclubeRepository.SaveOrUpdate(patroclube);
            }

            return RedirectToAction("Index", "AI");
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

                    var lstjogadores = jogadorRepository.GetAll().Where(x => (x.Clube == null || (x.Clube != null && x.Clube.Id != clube.Id)) && x.JogadorPreContrato.Count() == 0 && !x.Temporario);                                                             
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

                            if (lsta.Where(x => x.Clube == null && (x.H > (posmedia >= 70 ? posmedia - 20 : posmedia - 30) && dinheiro > 1000000)  ).Count() > 0)
                            {
                                var jogador = lsta.Where(x => x.Clube == null).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia).FirstOrDefault();
                                dinheiro = dinheiro - jogador.Valor;
                                lstcompra.Add(jogador);
                            }
                            else
                            {
                                foreach (var jogador in lsta.Where(x => x.H > (posmedia >= 70 ? posmedia - 20 : posmedia - 30)).OrderBy(x => x.Valor).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia))
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
                            var jogador = lsta.Where(x => x.Clube == null).OrderByDescending(x => x.Valor).ThenByDescending(x => x.NotaMedia).FirstOrDefault();
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
                        var salarioplus = 0;

                        if (jogador.Clube != null)
                            valor = jogador.Valor;
                        else
                            valor = 0;

                        if (ofertas > 0 && valor > 0)
                        {
                            valor = valor + (valor / 2);
                            salarioplus = 15000;
                        }

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

                            if (salarioplus > 0)
                                jogadoroferta.Salario = jogadoroferta.Salario + salarioplus;

                            var pontos = jogador.Salario > 0 ? Convert.ToInt32((jogadoroferta.Salario - jogador.Salario) / 10000) : Convert.ToInt32(jogadoroferta.Salario / 10000);

                            if (jogador.Clube != null)
                            {
                                pontos = pontos + (jogador.Clube.Divisao.Numero - jogadoroferta.Clube.Divisao.Numero);
                                pontos = pontos + (jogador.Situacao - 1) + jogador.Satisfacao;
                            }
                            else
                                pontos = pontos + (jogadoroferta.Clube.Socios / 1000);

                            jogadoroferta.Pontos = pontos;
                            jogadoroferta.Contrato = 3;
                            jogadorofertaRepository.SaveOrUpdate(jogadoroferta);

                            if (jogador.Clube != null && jogador.Clube.Usuario != null)
                            {
                                var noticia = new Noticia();
                                noticia.Dia = controle.Dia;
                                noticia.Texto = jogadoroferta.Clube.Nome + " apresentou uma proposta por <a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a>. <br />Entre em <a href='" + Url.Action("MeusJogadores", "Jogador") + "'>PROPOSTAS</a> para responder.";
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

                var lstdispensa = new List<Jogador>();
                var i = 1;

                var lstjogadores = jogadorRepository.GetAll().Where(x => x.Clube != null && x.Clube.Id == clube.Id && !x.Temporario);
                //goleiro
                foreach (var jogador in lstjogadores.Where(x => x.Posicao == 1).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia))
                {
                    if (i < 2)
                        jogador.Situacao = 1;
                    else if (i < 3)
                        jogador.Situacao = 2;
                    else if (i > 2)
                        jogador.Situacao = 3;

                    if ((i > 2 && jogador.NotaMedia < 6.0 && jogador.H <= (g - 20)) || (i > 3 && jogador.H <= (g - 10)))
                        lstdispensa.Add(jogador);

                    i++;
                }

                //lateral-direito
                i = 1;
                foreach (var jogador in lstjogadores.Where(x => x.Posicao == 2).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia))
                {
                    if (i < 2)
                        jogador.Situacao = 1;
                    else if (i < 3)
                        jogador.Situacao = 2;
                    else if (i > 2)
                        jogador.Situacao = 3;

                    if ((i > 2 && jogador.NotaMedia < 6.0 && jogador.H <= (ld - 20)) || (i > 3 && jogador.H <= (ld - 10)))
                        lstdispensa.Add(jogador);

                    i++;
                }

                //zagueiro
                i = 1;
                foreach (var jogador in lstjogadores.Where(x => x.Posicao == 3).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia))
                {
                    if (i < 3)
                        jogador.Situacao = 1;
                    else if (i < 4)
                        jogador.Situacao = 2;
                    else if (i > 3)
                        jogador.Situacao = 3;

                    if ((i > 3 && jogador.NotaMedia < 6.0 && jogador.H <= (z - 20)) || (i > 4 && jogador.H <= (z - 10)))
                        lstdispensa.Add(jogador);

                    i++;
                }

                //lateral-esquerdo
                i = 1;
                foreach (var jogador in lstjogadores.Where(x => x.Posicao == 4).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia))
                {
                    if (i < 2)
                        jogador.Situacao = 1;
                    else if (i < 3)
                        jogador.Situacao = 2;
                    else if (i > 2)
                        jogador.Situacao = 3;

                    if ((i > 2 && jogador.NotaMedia < 6.0 && jogador.H <= (le - 20)) || (i > 3 && jogador.H <= (le - 10)))
                        lstdispensa.Add(jogador);

                    i++;
                }

                //volante
                i = 1;
                foreach (var jogador in lstjogadores.Where(x => x.Posicao == 5).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia))
                {
                    if (i < 3)
                        jogador.Situacao = 1;
                    else if (i < 4)
                        jogador.Situacao = 2;
                    else if (i > 3)
                        jogador.Situacao = 3;

                    if ((i > 3 && jogador.NotaMedia < 6.0 && jogador.H <= (v - 20)) || (i > 4 && jogador.H <= (v - 10)))
                        lstdispensa.Add(jogador);

                    i++;
                }

                //meia
                i = 1;
                foreach (var jogador in lstjogadores.Where(x => x.Posicao == 6).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia))
                {
                    if (i < 3)
                        jogador.Situacao = 1;
                    else if (i < 4)
                        jogador.Situacao = 2;
                    else if (i > 3)
                        jogador.Situacao = 3;

                    if ((i > 3 && jogador.NotaMedia < 6.0 && jogador.H <= (mo - 20)) || (i > 4 && jogador.H <= (mo - 10)))
                        lstdispensa.Add(jogador);

                    i++;
                }

                //atacante
                i = 1;
                foreach (var jogador in lstjogadores.Where(x => x.Posicao == 7).OrderByDescending(x => x.H).ThenByDescending(x => x.NotaMedia))
                {
                    if (i < 3)
                        jogador.Situacao = 1;
                    else if (i < 4)
                        jogador.Situacao = 2;
                    else if (i > 3)
                        jogador.Situacao = 3;

                    if ((i > 3 && jogador.NotaMedia < 6.0 && jogador.H <= (a - 20)) || (i > 4 && jogador.H <= (a - 10)))
                        lstdispensa.Add(jogador);

                    i++;
                }

                foreach (var jogador in lstdispensa)
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
                    jogador.Situacao = 0;
                    jogador.Satisfacao = 0;
                    jogadorRepository.SaveOrUpdate(jogador);
                }
            }
            return RedirectToAction("Index", "AI");
        }

        [Transaction]
        public ActionResult ResponderPropostas()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var lstjogadoroferta = jogadorofertaRepository.GetAll().Where(x => x.Jogador.Clube != null && x.Jogador.Clube.Usuario == null);

            foreach (var jogadoroferta in lstjogadoroferta.Where(x => x.Jogador.Clube != null && x.Estagio > 0 && x.Estagio < 3 && x.Tipo == 1))
            {
                var jogador = jogadoroferta.Jogador;
                var clube = clubeRepository.Get(jogador.Clube.Id);

                var mediaclube = clube.Jogadores.Where(x => !x.Temporario && x.Id != jogador.Id).Sum(x => x.H) / clube.Jogadores.Where(x => !x.Temporario && x.Id != jogador.Id).Count();
                var gtotal = clube.Jogadores.Where(x => x.Posicao == 1 && !x.Temporario).Count();
                var ldtotal = clube.Jogadores.Where(x => x.Posicao == 2 && !x.Temporario).Count();
                var ztotal = clube.Jogadores.Where(x => x.Posicao == 3 && !x.Temporario).Count();
                var letotal = clube.Jogadores.Where(x => x.Posicao == 4 && !x.Temporario).Count();
                var vtotal = clube.Jogadores.Where(x => x.Posicao == 5 && !x.Temporario).Count();
                var mototal = clube.Jogadores.Where(x => x.Posicao == 6 && !x.Temporario).Count();
                var atotal = clube.Jogadores.Where(x => x.Posicao == 7 && !x.Temporario).Count();

                decimal valormin = jogador.Valor;                 

                if (lstjogadoroferta.Where(x => x.Jogador.Id == jogador.Id && x.Estagio == 3).Count() > 0)
                    valormin = valormin * 2;
                else if (clube.Dinheiro < 300000)
                    valormin = (valormin / 100) * 80;

                if (jogador.H >= (mediaclube + 20))
                    valormin = valormin + (valormin / 2);

                if ((jogador.Posicao == 1 && gtotal < 2) || (jogador.Posicao == 2 && ldtotal < 2) || (jogador.Posicao == 3 && ztotal < 3) || (jogador.Posicao == 4 && letotal < 2) || (jogador.Posicao == 5 && vtotal < 3) || (jogador.Posicao == 6 && mototal < 3) || (jogador.Posicao == 7 && atotal < 3))
                    valormin = (valormin * 2) + (valormin / 2);

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
