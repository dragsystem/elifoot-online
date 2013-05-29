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
        private readonly INHibernateRepository<Leilao> leilaoRepository;
        private readonly INHibernateRepository<LeilaoOferta> leilaoofertaRepository;
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

        public AIController(IUsuarioRepository usuarioRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            INHibernateRepository<Jogador> jogadorRepository,
            INHibernateRepository<Controle> controleRepository,
            INHibernateRepository<Leilao> leilaoRepository,
            INHibernateRepository<LeilaoOferta> leilaoofertaRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            IPartidaRepository partidaRepository,
            INHibernateRepository<Gol> golRepository,
            INHibernateRepository<DivisaoTabela> divisaotabelaRepository,
            INHibernateRepository<JogadorPedido> jogadorpedidoRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Noticia> noticiaRepository,
            INHibernateRepository<Escalacao> escalacaoRepository,
            INHibernateRepository<Historico> historicoRepository,
            INHibernateRepository<Nome> nomeRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.authenticationService = authenticationService;
            this.clubeRepository = clubeRepository;
            this.jogadorRepository = jogadorRepository;
            this.controleRepository = controleRepository;
            this.leilaoRepository = leilaoRepository;
            this.leilaoofertaRepository = leilaoofertaRepository;
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
                foreach (var jogador in clube.Jogadores.Where(x => x.Contrato == false))
                {
                    jogador.Contrato = true;
                    jogadorRepository.SaveOrUpdate(jogador);
                }
            }
            return RedirectToAction("Index", "AI");
        }

        [Transaction]
        public ActionResult ComprarJogador()
        {
            foreach (var clube in clubeRepository.GetAll().Where(x => x.Usuario == null).OrderByDescending(x => x.Dinheiro))
            {
                var controle = controleRepository.GetAll().FirstOrDefault();
                var g = clube.Jogadores.Where(x => x.Posicao == 1).OrderBy(x => x.H);
                var gc = false;
                var ld = clube.Jogadores.Where(x => x.Posicao == 2).OrderBy(x => x.H);
                var ldc = false;
                var z = clube.Jogadores.Where(x => x.Posicao == 3).OrderBy(x => x.H);
                var le = clube.Jogadores.Where(x => x.Posicao == 4).OrderBy(x => x.H);
                var lec = false;
                var v = clube.Jogadores.Where(x => x.Posicao == 5).OrderBy(x => x.H);
                var mo = clube.Jogadores.Where(x => x.Posicao == 6).OrderBy(x => x.H);
                var a = clube.Jogadores.Where(x => x.Posicao == 7).OrderBy(x => x.H);

                foreach (var leilao in leilaoRepository.GetAll().Where(x => x.Clube.Id != clube.Id && x.OfertaVencedora == null && x.Dia == controle.Dia).OrderByDescending(x => x.Jogador.H))
                {
                    //Verifica se tem dinheiro
                    var compra = false;
                    if ((clube.Dinheiro - 300000) >= leilao.Valor)
                    {
                        //Verifica se o valor não é absurdo
                        var valormax = leilao.Jogador.H * (50000 * leilao.Jogador.Posicao);
                        if (valormax >= leilao.Valor)
                        {
                            //verifica se é goleiro
                            if (leilao.Jogador.Posicao == 1 && !gc)
                            {
                                var media = g.Sum(x => x.H) / g.Count();

                                if ((g.Count() < 2 && leilao.Jogador.H >= (media - 10)) || leilao.Jogador.H > media)
                                {
                                    gc = true;
                                    compra = true;
                                }
                            }
                            //verifica se é lateral direito
                            if (leilao.Jogador.Posicao == 2 && !ldc)
                            {
                                var media = ld.Sum(x => x.H) / ld.Count();

                                if ((ld.Count() < 2 && leilao.Jogador.H >= (media - 10)) || leilao.Jogador.H > media)
                                {
                                    ldc = true;
                                    compra = true;
                                }
                            }
                            //verifica se é zagueiro
                            if (leilao.Jogador.Posicao == 3 && !lec)
                            {
                                var media = z.Sum(x => x.H) / z.Count();

                                if ((z.Count() < 3 && leilao.Jogador.H >= (media - 10)) || leilao.Jogador.H > media)
                                {
                                    lec = true;
                                    compra = true;
                                }
                            }
                            //verifica se é lateral esquerdo
                            if (leilao.Jogador.Posicao == 4)
                            {
                                var media = le.Sum(x => x.H) / le.Count();

                                if ((le.Count() < 2 && leilao.Jogador.H >= (media - 10)) || leilao.Jogador.H > media)
                                    compra = true;
                            }
                            //verifica se é volante
                            if (leilao.Jogador.Posicao == 5)
                            {
                                var media = v.Sum(x => x.H) / v.Count();

                                if ((v.Count() < 3 && leilao.Jogador.H >= (media - 10)) || leilao.Jogador.H > media)
                                    compra = true;
                            }
                            //verifica se é MO
                            if (leilao.Jogador.Posicao == 6)
                            {
                                var media = mo.Sum(x => x.H) / mo.Count();

                                if ((mo.Count() < 3 && leilao.Jogador.H >= (media - 10)) || leilao.Jogador.H > media)
                                    compra = true;
                            }
                            //verifica se é atacante
                            if (leilao.Jogador.Posicao == 7)
                            {
                                var media = a.Sum(x => x.H) / a.Count();

                                if ((a.Count() < 3 && leilao.Jogador.H >= (media - 10)) || leilao.Jogador.H > media)
                                    compra = true;
                            }
                        }
                    }

                    if (compra)
                    {
                        var leilaooferta = new LeilaoOferta();
                        leilaooferta.Clube = clube;
                        leilaooferta.Leilao = leilao;
                        leilaooferta.Salario = (leilao.Jogador.Salario * 150) / 100;

                        leilaoofertaRepository.SaveOrUpdate(leilaooferta);
                    }
                }
            }

            return RedirectToAction("Index", "AI");
        }

        [Transaction]
        public ActionResult VenderJogador()
        {
            foreach (var clube in clubeRepository.GetAll().Where(x => x.Usuario == null).OrderByDescending(x => x.Dinheiro))
            {
                var controle = controleRepository.GetAll().FirstOrDefault();

                var g = clube.Jogadores.Where(x => x.Posicao == 1).OrderBy(x => x.H);
                if (g.Count() > 2 || g.FirstOrDefault().H < (g.Sum(x => x.H) / g.Count() - 20))
                {
                    var leilao = new Leilao();
                    leilao.Clube = clube;
                    leilao.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
                    leilao.Jogador = g.FirstOrDefault();
                    leilao.Valor = (leilao.Jogador.H * (leilao.Jogador.H - 20 > 1 ? leilao.Jogador.H - 20 : 1)) + (leilao.Jogador.Posicao * 200000);

                    leilaoRepository.SaveOrUpdate(leilao);
                }

                var ld = clube.Jogadores.Where(x => x.Posicao == 2).OrderBy(x => x.H);
                if (ld.Count() > 2 || ld.FirstOrDefault().H < (ld.Sum(x => x.H) / ld.Count() - 20))
                {
                    var leilao = new Leilao();
                    leilao.Clube = clube;
                    leilao.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
                    leilao.Jogador = ld.FirstOrDefault();
                    leilao.Valor = (leilao.Jogador.H * (leilao.Jogador.H - 20 > 1 ? leilao.Jogador.H - 20 : 1)) + (leilao.Jogador.Posicao * 200000);

                    leilaoRepository.SaveOrUpdate(leilao);
                }

                var z = clube.Jogadores.Where(x => x.Posicao == 3).OrderBy(x => x.H);
                if (z.Count() > 3 || z.FirstOrDefault().H < (z.Sum(x => x.H) / z.Count() - 20))
                {
                    var leilao = new Leilao();
                    leilao.Clube = clube;
                    leilao.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
                    leilao.Jogador = z.FirstOrDefault();
                    leilao.Valor = (leilao.Jogador.H * (leilao.Jogador.H - 20 > 1 ? leilao.Jogador.H - 20 : 1)) + (leilao.Jogador.Posicao * 200000);

                    leilaoRepository.SaveOrUpdate(leilao);
                }

                var le = clube.Jogadores.Where(x => x.Posicao == 4).OrderBy(x => x.H);
                if (le.Count() > 3 || le.FirstOrDefault().H < (le.Sum(x => x.H) / le.Count() - 20))
                {
                    var leilao = new Leilao();
                    leilao.Clube = clube;
                    leilao.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
                    leilao.Jogador = le.FirstOrDefault();
                    leilao.Valor = (leilao.Jogador.H * (leilao.Jogador.H - 20 > 1 ? leilao.Jogador.H - 20 : 1)) + (leilao.Jogador.Posicao * 200000);

                    leilaoRepository.SaveOrUpdate(leilao);
                }

                var v = clube.Jogadores.Where(x => x.Posicao == 5).OrderBy(x => x.H);
                if (v.Count() > 3 || v.FirstOrDefault().H < (v.Sum(x => x.H) / v.Count() - 20))
                {
                    var leilao = new Leilao();
                    leilao.Clube = clube;
                    leilao.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
                    leilao.Jogador = v.FirstOrDefault();
                    leilao.Valor = (leilao.Jogador.H * (leilao.Jogador.H - 20 > 1 ? leilao.Jogador.H - 20 : 1)) + (leilao.Jogador.Posicao * 200000);

                    leilaoRepository.SaveOrUpdate(leilao);
                }

                var mo = clube.Jogadores.Where(x => x.Posicao == 6).OrderBy(x => x.H);
                if (mo.Count() > 3 || mo.FirstOrDefault().H < (mo.Sum(x => x.H) / mo.Count() - 20))
                {
                    var leilao = new Leilao();
                    leilao.Clube = clube;
                    leilao.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
                    leilao.Jogador = mo.FirstOrDefault();
                    leilao.Valor = (leilao.Jogador.H * (leilao.Jogador.H - 20 > 1 ? leilao.Jogador.H - 20 : 1)) + (leilao.Jogador.Posicao * 200000);

                    leilaoRepository.SaveOrUpdate(leilao);
                }

                var a = clube.Jogadores.Where(x => x.Posicao == 7).OrderBy(x => x.H);
                if (a.Count() > 4 || a.FirstOrDefault().H < (a.Sum(x => x.H) / a.Count() - 20))
                {
                    var leilao = new Leilao();
                    leilao.Clube = clube;
                    leilao.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
                    leilao.Jogador = a.FirstOrDefault();
                    leilao.Valor = (leilao.Jogador.H * (leilao.Jogador.H - 20 > 1 ? leilao.Jogador.H - 20 : 1)) + (leilao.Jogador.Posicao * 200000);

                    leilaoRepository.SaveOrUpdate(leilao);
                }
            }
            return RedirectToAction("Index", "AI");
        }

        [Transaction]
        public ActionResult VenderJogadorPorCaixa()
        {
            foreach (var clube in clubeRepository.GetAll().Where(x => x.Usuario == null).OrderByDescending(x => x.Dinheiro))
            {
                if (clube.Dinheiro < 200000)
                {
                    var controle = controleRepository.GetAll().FirstOrDefault();

                    var jogador = clube.Jogadores.OrderBy(x => (x.Salario / x.H)).FirstOrDefault();

                    var leilao = new Leilao();
                    leilao.Clube = clube;
                    leilao.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
                    leilao.Jogador = jogador;
                    leilao.Valor = (leilao.Jogador.H * (leilao.Jogador.H - 10 > 1 ? leilao.Jogador.H - 10 : 1)) + (leilao.Jogador.Posicao * 200000);

                    leilaoRepository.SaveOrUpdate(leilao);
                }
            }
            return RedirectToAction("Index", "AI");
        }
    }
}
