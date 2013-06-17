﻿namespace EmpreendaVc.Web.Mvc.Controllers
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

            foreach (var clube in clubeRepository.GetAll().Where(x => x.Usuario == null).OrderByDescending(x => x.Dinheiro))
            {
                if (clube.Dinheiro > 300000)
                {
                    var dinheiro = clube.Dinheiro - 300000;
                    var jogadores = clube.Jogadores.Where(x => !x.Temporario);
                    //var g = clube.Jogadores.Where(x => x.Posicao == 1).OrderBy(x => x.H);
                    //var ld = clube.Jogadores.Where(x => x.Posicao == 2).OrderBy(x => x.H);
                    //var z = clube.Jogadores.Where(x => x.Posicao == 3).OrderBy(x => x.H);
                    //var le = clube.Jogadores.Where(x => x.Posicao == 4).OrderBy(x => x.H);
                    //var v = clube.Jogadores.Where(x => x.Posicao == 5).OrderBy(x => x.H);
                    //var mo = clube.Jogadores.Where(x => x.Posicao == 6).OrderBy(x => x.H);
                    //var a = clube.Jogadores.Where(x => x.Posicao == 7).OrderBy(x => x.H);

                    var lstjogadores = jogadorRepository.GetAll().Where(x => x.Clube != null && !x.Temporario && (x.Clube.Divisao.Numero <= clube.Divisao.Numero ||
                                                                        x.Clube.Divisao.Numero > clube.Divisao.Numero && x.Situacao > 1) &&
                                                                        (x.H > ((jogadores.Where(y => y.Posicao == x.Posicao).Sum(y => y.H) / jogadores.Where(y => y.Posicao == x.Posicao).Count()) + 10) ||
                                                                        jogadores.Where(y => y.Posicao == x.Posicao).Count() < 2)).OrderByDescending(x => x.NotaMedia).ThenByDescending(x => x.H).ThenByDescending(x => x.Situacao);

                    foreach (var jogador in lstjogadores)
                    {
                        if (dinheiro <= 0)
                            break;

                        decimal oferta = 0;

                        if (jogador.Situacao == 1)
                            oferta = (jogador.H * (jogador.H - 20 > 1 ? jogador.H - 20 : 1)) * 1000 + (jogador.Posicao * 200000);
                        else if (jogador.Situacao == 2)
                            oferta = (jogador.H * (jogador.H - 20 > 1 ? jogador.H - 20 : 1)) * 800 + (jogador.Posicao * 200000);
                        else if (jogador.Situacao == 3)
                            oferta = (jogador.H * (jogador.H - 20 > 1 ? jogador.H - 20 : 1)) * 500 + (jogador.Posicao * 200000);

                        if (dinheiro > oferta)
                        {
                            var jogadoroferta = new JogadorOferta();
                            jogadoroferta.Clube = clube;
                            jogadoroferta.Dia = controle.Dia;
                            jogadoroferta.Jogador = jogador;
                            jogadoroferta.Tipo = 1;
                            jogadoroferta.Valor = oferta;
                            jogadoroferta.Salario = (jogador.Salario / 100) * 120;
                            
                            var pontos = Convert.ToInt32((jogadoroferta.Salario - jogador.Salario) / 10000);
                            pontos = pontos + (jogador.Clube.Divisao.Numero - jogadoroferta.Clube.Divisao.Numero);

                            jogadoroferta.Pontos = pontos;
                            jogadoroferta.Contrato = 3;
                            jogadorofertaRepository.SaveOrUpdate(jogadoroferta);

                            if (jogador.Clube.Usuario != null)
                            {
                                var noticia = new Noticia();
                                noticia.Dia = controle.Dia;
                                noticia.Texto = jogadoroferta.Clube.Nome + " apresentou uma proposta por <a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a>. <br /><br />Entre em <a href='" + Url.Action("MeusJogadores", "Jogador") + "'>PROPOSTAS</a> para responder.";
                                noticia.Usuario = jogador.Clube.Usuario;

                                noticiaRepository.SaveOrUpdate(noticia);
                            }

                            dinheiro = dinheiro - oferta;
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
                
                var g = clube.Jogadores.Where(x => x.Posicao == 1 && !x.Temporario).Sum(x => x.H) / clube.Jogadores.Where(x => x.Posicao == 1 && !x.Temporario).Count();
                var ld = clube.Jogadores.Where(x => x.Posicao == 2 && !x.Temporario).Sum(x => x.H) / clube.Jogadores.Where(x => x.Posicao == 2 && !x.Temporario).Count();
                var z = clube.Jogadores.Where(x => x.Posicao == 3 && !x.Temporario).Sum(x => x.H) / clube.Jogadores.Where(x => x.Posicao == 3 && !x.Temporario).Count();
                var le = clube.Jogadores.Where(x => x.Posicao == 4 && !x.Temporario).Sum(x => x.H) / clube.Jogadores.Where(x => x.Posicao == 4 && !x.Temporario).Count();
                var v = clube.Jogadores.Where(x => x.Posicao == 5 && !x.Temporario).Sum(x => x.H) / clube.Jogadores.Where(x => x.Posicao == 5 && !x.Temporario).Count();
                var mo = clube.Jogadores.Where(x => x.Posicao == 6 && !x.Temporario).Sum(x => x.H) / clube.Jogadores.Where(x => x.Posicao == 6 && !x.Temporario).Count();
                var a = clube.Jogadores.Where(x => x.Posicao == 7 && !x.Temporario).Sum(x => x.H) / clube.Jogadores.Where(x => x.Posicao == 7 && !x.Temporario).Count();

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
                            if (clube.Jogadores.Count() > 14)
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
                            if (clube.Jogadores.Count() > 14)
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
                            if (clube.Jogadores.Count() > 14)
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
                            if (clube.Jogadores.Count() > 14)
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
                            if (clube.Jogadores.Count() > 14)
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
                            if (clube.Jogadores.Count() > 14)
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
                            if (clube.Jogadores.Count() > 14)
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
            foreach (var clube in clubeRepository.GetAll().Where(x => x.Usuario == null))
            {
                foreach (var jogadoroferta in jogadorofertaRepository.GetAll().Where(x => x.Jogador.Clube.Id == clube.Id))
                {
                    var jogador = jogadoroferta.Jogador;
                    decimal valormin = 0;

                    if (jogador.Situacao == 1)
                        valormin = (jogador.H * (jogador.H - 20 > 1 ? jogador.H - 20 : 1)) * 1000 + (jogador.Posicao * 200000);
                    else if (jogador.Situacao == 2)
                        valormin = (jogador.H * (jogador.H - 20 > 1 ? jogador.H - 20 : 1)) * 800 + (jogador.Posicao * 200000);
                    else if (jogador.Situacao == 3)
                        valormin = (jogador.H * (jogador.H - 20 > 1 ? jogador.H - 20 : 1)) * 500 + (jogador.Posicao * 200000);

                    if (clube.Dinheiro < 300000)
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

                        jogadorofertaRepository.Delete(jogadoroferta);
                    }
                }
            }
            return RedirectToAction("Index", "AI");
        }
    }
}
