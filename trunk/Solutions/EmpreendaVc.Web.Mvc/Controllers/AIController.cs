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
        private readonly INHibernateRepository<JogadorLeilao> jogadorleilaoRepository;
        private readonly INHibernateRepository<JogadorLeilaoOferta> jogadorleilaoofertaRepository;
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
            INHibernateRepository<JogadorLeilao> jogadorleilaoRepository,
            INHibernateRepository<JogadorLeilaoOferta> jogadorleilaoofertaRepository,
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
            this.jogadorleilaoRepository = jogadorleilaoRepository;
            this.jogadorleilaoofertaRepository = jogadorleilaoofertaRepository;
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
        public ActionResult LeiloarJogadores()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            foreach (var clube in clubeRepository.GetAll().Where(x => x.Usuario == null && x.Jogadores.Count() > 14))
            {
                var gtotal = clube.Jogadores.Where(x => x.Posicao == 1 && !x.Temporario).Count();
                var ldtotal = clube.Jogadores.Where(x => x.Posicao == 2 && !x.Temporario).Count();
                var ztotal = clube.Jogadores.Where(x => x.Posicao == 3 && !x.Temporario).Count();
                var letotal = clube.Jogadores.Where(x => x.Posicao == 4 && !x.Temporario).Count();
                var vtotal = clube.Jogadores.Where(x => x.Posicao == 5 && !x.Temporario).Count();
                var mototal = clube.Jogadores.Where(x => x.Posicao == 6 && !x.Temporario).Count();
                var atotal = clube.Jogadores.Where(x => x.Posicao == 7 && !x.Temporario).Count();

                var lstvenda = new List<Jogador>();

                if (gtotal > 3)
                {
                    lstvenda.Add(clube.Jogadores.OrderBy(x => x.H).FirstOrDefault(x => x.Posicao == 1));
                }
                if (ldtotal > 2)
                {
                    lstvenda.Add(clube.Jogadores.OrderBy(x => x.H).FirstOrDefault(x => x.Posicao == 2));
                }
                if (ztotal > 4)
                {
                    lstvenda.Add(clube.Jogadores.OrderBy(x => x.H).FirstOrDefault(x => x.Posicao == 3));
                }
                if (letotal > 2)
                {
                    lstvenda.Add(clube.Jogadores.OrderBy(x => x.H).FirstOrDefault(x => x.Posicao == 4));
                }
                if (vtotal > 4)
                {
                    lstvenda.Add(clube.Jogadores.OrderBy(x => x.H).FirstOrDefault(x => x.Posicao == 5));
                }
                if (mototal > 4)
                {
                    lstvenda.Add(clube.Jogadores.OrderBy(x => x.H).FirstOrDefault(x => x.Posicao == 6));
                }
                if (atotal > 4)
                {
                    lstvenda.Add(clube.Jogadores.OrderBy(x => x.H).FirstOrDefault(x => x.Posicao == 7));
                }

                if (clube.Dinheiro < 0 && lstvenda.Count == 0)
                {
                    var lstt = clube.Jogadores.OrderBy(x => x.H).Take(3);

                    foreach (var jog in lstt)
                    {
                        if (clube.Jogadores.Where(x => x.Posicao == jog.Posicao && !x.Temporario).Count() > 1)
                        {
                            lstvenda.Add(jog);
                        }
                    }
                }

                var qntjogadores = clube.Jogadores.Count();

                foreach (var item in lstvenda)
                {
                    if (qntjogadores < 15)
                        break;
                    else
                    {
                        qntjogadores--;

                        var ofertaleilao = new JogadorLeilao();

                        ofertaleilao.Clube = clube;
                        ofertaleilao.Dia = controle.Dia;
                        ofertaleilao.Estagio = 1;
                        ofertaleilao.Jogador = item;
                        ofertaleilao.Valor = item.Valor;

                        jogadorleilaoRepository.SaveOrUpdate(ofertaleilao);
                    }
                }
            }

            return RedirectToAction("Index", "AI");
        }

        [Transaction]
        public ActionResult ComprarJogador()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            foreach (var clube in clubeRepository.GetAll().Where(x => x.Usuario == null))
            {
                //if ((clube.DivisaoTabelas.Count() > 0 && clube.DivisaoTabelas.FirstOrDefault(x => x.Clube.Id == clube.Id && x.Divisao.Id == clube.Divisao.Id).Posicao > 3) || clube.DivisaoTabelas.Count() == 0)
                //{
                var dinheiro = clube.Dinheiro - 600000;
                var gtotal = clube.Jogadores.Where(x => x.Posicao == 1 && !x.Temporario).Count();
                var ldtotal = clube.Jogadores.Where(x => x.Posicao == 2 && !x.Temporario).Count();
                var ztotal = clube.Jogadores.Where(x => x.Posicao == 3 && !x.Temporario).Count();
                var letotal = clube.Jogadores.Where(x => x.Posicao == 4 && !x.Temporario).Count();
                var vtotal = clube.Jogadores.Where(x => x.Posicao == 5 && !x.Temporario).Count();
                var mototal = clube.Jogadores.Where(x => x.Posicao == 6 && !x.Temporario).Count();
                var atotal = clube.Jogadores.Where(x => x.Posicao == 7 && !x.Temporario).Count();
                var lstjogadores = jogadorleilaoRepository.GetAll().Where(x => x.Estagio < 2 && (x.Clube != null && x.Clube.Id != clube.Id) && x.Jogador.JogadorPreContrato.Count() == 0 && x.Jogador.H > (clube.Jogadores.Average(y => y.H) - 15) && x.Jogador.H < (clube.Jogadores.Average(y => y.H) + 40) && !x.Jogador.Temporario && x.Valor < dinheiro); // && 
                                                                            //((x.Jogador.Posicao == 1 && x.Jogador.H >= g) ||
                                                                            //(x.Jogador.Posicao == 2 && x.Jogador.H >= ld) ||
                                                                            //(x.Jogador.Posicao == 3 && x.Jogador.H >= z) ||
                                                                            //(x.Jogador.Posicao == 4 && x.Jogador.H >= le) ||
                                                                            //(x.Jogador.Posicao == 5 && x.Jogador.H >= v) ||
                                                                            //(x.Jogador.Posicao == 6 && x.Jogador.H >= mo) ||
                                                                            //(x.Jogador.Posicao == 7 && x.Jogador.H >= a)));

                var lstjogadoressemclube = jogadorRepository.GetAll().Where(x => x.Clube == null && x.JogadorPreContrato.Count() == 0 && x.H > (clube.Jogadores.Average(y => y.H) - 15) && x.H < (clube.Jogadores.Average(y => y.H) + 30) && !x.Temporario); //&& 
                //                                                            ((x.Posicao == 1 && x.H >= g) ||
                //                                                            (x.Posicao == 2 && x.H >= ld) ||
                //                                                            (x.Posicao == 3 && x.H >= z) ||
                //                                                            (x.Posicao == 4 && x.H >= le) ||
                //                                                            (x.Posicao == 5 && x.H >= v) ||
                //                                                            (x.Posicao == 6 && x.H >= mo) ||
                //                                                            (x.Posicao == 7 && x.H >= a)));
                var lstcompra = new List<JogadorLeilao>();
                var lstcomprasemclube = new List<Jogador>();

                //JOGADORES SEM CLUBE
                foreach (var jogador in lstjogadoressemclube)
                {
                    if ((gtotal < 3 && jogador.Posicao == 1) || (ldtotal < 2 && jogador.Posicao == 2) || (ztotal < 4 && jogador.Posicao == 3) ||
                        (letotal < 2 && jogador.Posicao == 4) || (vtotal < 4 && jogador.Posicao == 5) || (mototal < 4 && jogador.Posicao == 6) ||
                        (atotal < 4 && jogador.Posicao == 7))
                    {
                        lstcomprasemclube.Add(jogador);
                    }
                    else if (clube.Jogadores.Average(x => x.H) < (jogador.H - 5))
                        lstcomprasemclube.Add(jogador);
                }

                //JOGADORES LEILÃO
                if (dinheiro > 0)
                {
                    foreach (var leilao in lstjogadores)
                    {
                        var jogador = leilao.Jogador;
                        var avg = clube.Jogadores.Average(x => x.H);
                        if ((gtotal < 3 && jogador.Posicao == 1) || (ldtotal < 2 && jogador.Posicao == 2) || (ztotal < 4 && jogador.Posicao == 3) ||
                            (letotal < 2 && jogador.Posicao == 4) || (vtotal < 4 && jogador.Posicao == 5) || (mototal < 4 && jogador.Posicao == 6) ||
                            (atotal < 4 && jogador.Posicao == 7))
                        {
                            
                            if ((avg - 15) < leilao.Jogador.H)
                            {
                                if (leilao.Valor > (jogador.Valor + (jogador.Valor / 4)))
                                    lstcompra.Add(leilao);
                            }
                            else if (avg < leilao.Jogador.H)
                            {
                                if (leilao.Valor > (jogador.Valor + (jogador.Valor / 2)))
                                    lstcompra.Add(leilao);
                            }
                        }
                        else if (avg < (jogador.H - 5))
                        {
                            if (leilao.Valor < (jogador.Valor + (jogador.Valor / 2)))
                                lstcompra.Add(leilao);
                        }
                    }
                }

                foreach (var jogador in lstcomprasemclube)
                {
                    var rnd = new Random();

                    var jogadoroferta = new JogadorOferta();
                    jogadoroferta.Clube = clube;
                    jogadoroferta.Dia = controle.Dia;
                    jogadoroferta.Jogador = jogador;
                    jogadoroferta.Estagio = 2;
                    jogadoroferta.Tipo = 2;
                    jogadoroferta.Salario = ((jogador.H / 4) * 3) * (rnd.Next(9, 13) * 100);
                        
                    var pontos = jogador.Salario > 0 ? Convert.ToInt32((jogadoroferta.Salario - jogador.Salario) / 10000) : Convert.ToInt32(jogadoroferta.Salario / 10000);
                    pontos = pontos + (jogadoroferta.Clube.Socios / 1000);

                    jogadoroferta.Pontos = pontos;
                    jogadoroferta.Contrato = 2;
                    jogadorofertaRepository.SaveOrUpdate(jogadoroferta);
                }

                foreach (var leilao in lstcompra)
                {
                    var rnd = new Random();

                    var jogador = leilao.Jogador;
                    var jogadoroferta = new JogadorLeilaoOferta();
                    jogadoroferta.Clube = clube;
                    jogadoroferta.JogadorLeilao = leilao;
                    jogadoroferta.Estagio = 1;
                    jogadoroferta.Salario = ((jogador.H / 4) * 3) * (rnd.Next(9, 13) * 100);

                    var pontos = jogador.Salario > 0 ? Convert.ToInt32((jogadoroferta.Salario - jogador.Salario) / 10000) : Convert.ToInt32(jogadoroferta.Salario / 10000);
                    pontos = pontos + (jogadoroferta.Clube.Socios / 1000);

                    jogadoroferta.Pontos = pontos;
                    jogadoroferta.Contrato = 2;
                    jogadorleilaoofertaRepository.SaveOrUpdate(jogadoroferta);
                }
                //}
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
                            noticiaRepository.SaveOrUpdate(noticia);
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
                        noticiaRepository.SaveOrUpdate(noticia);
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
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    jogadoroferta.Estagio = 0;
                    jogadorofertaRepository.SaveOrUpdate(jogadoroferta);
                }
            }

            return RedirectToAction("Index", "AI");
        }
    }
}
