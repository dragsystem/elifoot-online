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
    using EmpreendaVc.Infrastructure.Queries.Clubes;
    using EmpreendaVc.Infrastructure.Queries.Partidas;
    using System.Web.Security;
    using EmpreendaVc.Web.Mvc.Controllers.ViewModels;
    using NHibernate.Criterion;
    using NHibernate.Transform;

    public class EngineController : ControllerCustom
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

        public EngineController(IUsuarioRepository usuarioRepository,
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
            INHibernateRepository<JogadorHistorico> jogadorhistoricoRepository)
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
        }

        //OK - GerarCampeonato(); 
        
        [Transaction]
        public ActionResult Index()
        {
            try
            {
                //var controle = controleRepository.GetAll().FirstOrDefault();

                //GerarCampeonato();
                //AtualizaTabela();
                //if (controle.Data.Day < DateTime.Now.Day)
                //{
                //    //MudarDia();
                //    if (controle.Dia == 31)
                //    {
                //        //MudarAno();
                //    }
                //}
            }
            catch (Exception ex)
            {
                return View(ex.ToString());
                throw;
            }

            return View();
        }

        [Transaction]
        public ActionResult MudarDia()
        {
            return RedirectToAction("ZerarDelayUsuario", "Engine");
        }

        #region MudarDia

        [Transaction]
        public ActionResult AtualizarDataDia()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            controle.Data = DateTime.Now;
            controle.Dia = controle.Dia + 1 <= controle.DiaMax ? controle.Dia + 1 : 1;
            controleRepository.SaveOrUpdate(controle);

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ZerarDelayUsuario()
        {
            foreach (var usuario in usuarioRepository.UsuarioDelay())
            {
                usuario.DelayTroca = usuario.DelayTroca - 1;
                if (usuario.DelayTroca < 0)
                    usuario.DelayTroca = 0;

                usuarioRepository.SaveOrUpdate(usuario);
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult CriarJogadoresTemporarios()
        {
            foreach (var clube in clubeRepository.GetAll())
            {
                var nomes = nomeRepository.GetAll();
                var rnd = new Random();

                for (int pos = 1; pos < 8; pos++)
                {
                    var min = 0;

                    if (pos == 1 || pos == 2 || pos == 4)
                        min = 1;
                    else if (pos == 3 || pos == 5 || pos == 6 || pos == 7)
                        min = 2;

                    if (clube.Jogadores.Where(x => x.Posicao == pos && !x.Temporario).Count() < min)
                    {
                        var jogador = new Jogador();
                        jogador.Clube = clube;
                        jogador.Posicao = pos;
                        jogador.Temporario = true;
                        jogador.H = 1;
                        jogador.HF = 1;

                        var objnome = nomes.ElementAt(rnd.Next(0, nomes.Count()));
                        if (!objnome.Comum) { objnome = nomes.ElementAt(rnd.Next(0, nomes.Count())); }

                        jogador.Nome = objnome.NomeJogador.ToUpper();
                        jogadorRepository.SaveOrUpdate(jogador);
                    }
                    else if (clube.Jogadores.Where(x => x.Posicao == pos && !x.Temporario).Count() > min)
                    {
                        foreach (var jog in clube.Jogadores.Where(x => x.Temporario))
                        {
                            jog.Clube = null;
                            jogadorRepository.SaveOrUpdate(jog);
                        }
                    }
                }
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("UpdateFinancas", "Engine");
        }

        [Transaction]
        public ActionResult UpdateFinancas()
        {
            foreach (var clube in clubeRepository.GetAll())
            {
                var salarios = clube.Jogadores.Sum(x => x.Salario);
                var socios = clube.Socios * 30;
                decimal staff = 0;

                if (clube.Usuario != null)
                    staff = clube.Usuario.Staffs.Sum(x => x.Salario);

                clube.Dinheiro = clube.Dinheiro + (socios - (salarios + staff));
                clubeRepository.SaveOrUpdate(clube);
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("VerificaTecnicos", "Engine");
        }

        [Transaction]
        public ActionResult VerificaTecnicos()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var ultdivisao = divisaoRepository.GetAll().OrderByDescending(x => x.Numero).FirstOrDefault().Numero;
            foreach (var clube in clubeRepository.GetAll())
            {
                if (clube.Usuario != null && clube.Usuario.Reputacao == 0)
                {
                    var tecnicoatual = usuarioRepository.Get(clube.Usuario.Id);

                    tecnicoatual.Clube = null;
                    tecnicoatual.Reputacao = 30;
                    tecnicoatual.DelayTroca = 0;
                    tecnicoatual.ReputacaoGeral = tecnicoatual.ReputacaoGeral - 10 < 0 ? 0 : tecnicoatual.ReputacaoGeral - 10;
                    tecnicoatual.IdUltimoClube = clube.Id;
                    usuarioRepository.SaveOrUpdate(tecnicoatual);

                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Você foi despedido do " + clube.Nome + ", pois a diretoria não estava satisfeita com seu trabalho.<br /><br />Procure outro clube para dirigir.";
                    noticia.Usuario = tecnicoatual;
                    noticiaRepository.SaveOrUpdate(noticia);

                    clubeQueryRepository.TirarTreinador(clube.Id);

                    var repgeral = clube.Divisao.Numero < ultdivisao ? (80 / clube.Divisao.Numero) : 0;
                    foreach (var tecniconovo in usuarioRepository.GetAll().Where(x => x.ReputacaoGeral >= repgeral && x.DelayTroca == 0 && x.IdUltimoClube != clube.Id))
                    {
                        noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = clube.Nome + " despediu o técnico e está a procura de um novo técnico.";
                        noticia.Usuario = tecniconovo;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                }
                else if (clube.Usuario == null && clube.ReputacaoAI < 5)
                {
                    var repgeral = clube.Divisao.Numero < ultdivisao ? (80 / clube.Divisao.Numero) : 0;
                    foreach (var tecniconovo in usuarioRepository.GetAll().Where(x => x.ReputacaoGeral >= repgeral && x.DelayTroca == 0 && x.IdUltimoClube != clube.Id))
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = clube.Nome + " despediu o técnico e está a procura de um novo técnico.";
                        noticia.Usuario = tecniconovo;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    clube.ReputacaoAI = 30;
                    clubeRepository.SaveOrUpdate(clube);
                }                
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("ZeraTransferencias", "Engine");
        }

        //[Transaction]
        //public ActionResult AtualizarTransferencias()
        //{
        //    var controle = controleRepository.GetAll().FirstOrDefault();
        //    foreach (var leilao in leilaoRepository.GetAll().Where(x => x.Dia <= controle.Dia && x.OfertaVencedora == null))
        //    {
        //        var cancelar = false;
        //        var vendido = false;

        //        foreach (var oferta in jogadorofertaRepository.GetAll().Where(x => x.Leilao.Id == leilao.Id).OrderByDescending(x => x.Salario).ThenBy(x => x.Clube.Divisao.Numero).ThenByDescending(x => x.Clube.Dinheiro))
        //        {
        //            var clubecomprador = clubeRepository.Get(oferta.Clube.Id);
        //            var clubevendedor = clubeRepository.Get(leilao.Clube.Id);
        //            var jogador = leilao.Jogador;

        //            if (clubevendedor.Jogadores.Count() > 14 && !vendido && oferta.Clube.Dinheiro >= leilao.Valor && oferta.Clube.Dinheiro > 200000)
        //            {
        //                clubecomprador.Dinheiro = clubecomprador.Dinheiro - leilao.Valor;
        //                clubeRepository.SaveOrUpdate(clubecomprador);

        //                jogador.Clube = clubecomprador;
        //                jogador.Contrato = true;
        //                jogador.Salario = oferta.Salario;
        //                jogadorRepository.SaveOrUpdate(jogador);

        //                clubevendedor.Dinheiro = clubecomprador.Dinheiro + leilao.Valor;
        //                clubeRepository.SaveOrUpdate(clubevendedor);

        //                vendido = true;
        //                leilao.OfertaVencedora = oferta;
        //                leilaoRepository.SaveOrUpdate(leilao);
        //            }
        //            else
        //            {
        //                if (clubevendedor.Jogadores.Count < 15 && !vendido)
        //                    cancelar = true;

        //                jogadorofertaRepository.Delete(oferta);
        //            }
        //        }

        //        if (cancelar)
        //        {
        //            leilaoRepository.Delete(leilao);
        //        }
        //    }

        //    //return RedirectToAction("Index", "Engine");
        //    return RedirectToAction("AlterarFinancasVerificaTecnicos", "Engine");
        //}

        [Transaction]
        public ActionResult AtualizarTransferencias()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            foreach (var jogadoroferta in jogadorofertaRepository.GetAll().Where(x => x.Dia < controle.Dia && x.Estagio == 2).OrderByDescending(x => x.Pontos))
            {
                var jogador = jogadoroferta.Jogador;
                var clubecomprador = clubeRepository.Get(jogadoroferta.Clube.Id);
                var clubevendedor = clubeRepository.Get(jogador.Clube.Id);
                var vendido = "";

                if (clubevendedor.Jogadores.Where(x => !x.Temporario).Count() > 14 && vendido == "" && clubecomprador.Dinheiro >= jogadoroferta.Valor && jogadoroferta.Pontos > 0)
                {
                    var escalacao = escalacaoRepository.GetAll().FirstOrDefault(x => x.Jogador != null && x.Jogador.Id == jogador.Id);
                    if (escalacao != null)
                    {
                        escalacao.Jogador = null;
                        escalacao.H = 0;
                        escalacaoRepository.SaveOrUpdate(escalacao);
                    }

                    clubecomprador.Dinheiro = clubecomprador.Dinheiro - jogadoroferta.Valor;
                    clubeRepository.SaveOrUpdate(clubecomprador);

                    jogador.Clube = clubecomprador;
                    jogador.Contrato = jogadoroferta.Contrato;
                    jogador.Salario = jogadoroferta.Salario;
                    jogadorRepository.SaveOrUpdate(jogador);

                    clubevendedor.Dinheiro = clubecomprador.Dinheiro + jogadoroferta.Valor;
                    clubeRepository.SaveOrUpdate(clubevendedor);

                    var historico = new JogadorHistorico();
                    historico.Ano = controle.Ano;
                    historico.Clube = clubevendedor;
                    historico.Gols = jogador.Gols.Where(x => x.Clube.Id == clubevendedor.Id).Count();
                    historico.Jogador = jogador;
                    historico.Jogos = jogador.Jogos;
                    historico.NotaMedia = jogador.NotaMedia > 0.0 ? jogador.NotaMedia : 0.00;
                    historico.Valor = jogadoroferta.Valor;
                    jogadorhistoricoRepository.SaveOrUpdate(historico);

                    if (clubecomprador.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "<a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> (" + clubevendedor.Nome + ") foi vendido para o " + clubecomprador.Nome + " por $" + jogadoroferta.Valor.ToString("N2"); 
                        noticia.Usuario = clubecomprador.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                    if (clubevendedor.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "<a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> (" + clubevendedor.Nome + ") foi vendido para o " + clubecomprador.Nome + " por $" + jogadoroferta.Valor.ToString("N2"); 
                        noticia.Usuario = clubevendedor.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    vendido = clubecomprador.Nome;
                    jogadorofertaRepository.Delete(jogadoroferta);
                }
                else if (jogadoroferta.Pontos < 1)
                {
                    if (clubecomprador.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "<a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> (" + clubevendedor.Nome + ") rejeitou sua proposta.";
                        noticia.Usuario = clubecomprador.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    jogadorofertaRepository.Delete(jogadoroferta);
                }
                else if (vendido != "")
                {
                    if (clubecomprador.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "<a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> (" + clubevendedor.Nome + ") rejeitou sua proposta e foi vendido para o " + clubecomprador.Nome + " por $" + jogadoroferta.Valor.ToString("N2");
                        noticia.Usuario = clubecomprador.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    jogadorofertaRepository.Delete(jogadoroferta);
                }
                else if (clubevendedor.Jogadores.Where(x => !x.Temporario).Count() > 14)
                {
                    if (clubecomprador.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = clubevendedor.Nome + " cancelou a venda de <a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> por estar com poucos jogadores no elenco.";
                        noticia.Usuario = clubecomprador.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    jogadorofertaRepository.Delete(jogadoroferta);
                }
                else if (clubecomprador.Dinheiro < jogadoroferta.Valor)
                {
                    if (clubecomprador.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Sua proposta por <a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> foi cancelada, pois você não possui dinheiro para fechar a compra.";
                        noticia.Usuario = clubecomprador.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }

                    jogadorofertaRepository.Delete(jogadoroferta);
                }
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("UpdateFinancas", "Engine");
        }

        //[Transaction]
        //public ActionResult ZeraTransferencias()
        //{
        //    var controle = controleRepository.GetAll().FirstOrDefault();
        //    foreach (var leilao in leilaoRepository.GetAll().Where(x => x.Dia < controle.Dia && x.OfertaVencedora == null))
        //    {
        //        foreach (var oferta in jogadorofertaRepository.GetAll().Where(x => x.Leilao.Id == leilao.Id))
        //        {
        //            jogadorofertaRepository.Delete(oferta);
        //        }

        //        leilaoRepository.Delete(leilao);
        //    }

        //    //return RedirectToAction("Index", "Engine");
        //    return RedirectToAction("VariarJogadorH", "Engine");
        //}

        //[Transaction]
        //public ActionResult ZerarPedidoJogadores()
        //{
        //    var controle = controleRepository.GetAll().FirstOrDefault();
        //    foreach (var pedidojog in jogadorpedidoRepository.GetAll().Where(x => x.Dia < controle.Dia))
        //    {
        //        var clube = clubeRepository.Get(pedidojog.Jogador.Clube.Id);

        //        if (clube.Jogadores.Count() > 14)
        //        {
        //            var jogposicao = clube.Jogadores.Where(x => x.Posicao == pedidojog.Jogador.Posicao).Count();
        //            if ((jogposicao > 1 && pedidojog.Jogador.Posicao == 1) || (jogposicao > 1 && pedidojog.Jogador.Posicao == 2) || (jogposicao > 1 && pedidojog.Jogador.Posicao == 4) ||
        //                (jogposicao > 1 && pedidojog.Jogador.Posicao == 5) || (jogposicao > 1 && pedidojog.Jogador.Posicao == 6) || (jogposicao > 1 && pedidojog.Jogador.Posicao == 7) ||
        //                (jogposicao > 2 && pedidojog.Jogador.Posicao == 3))
        //            {
        //                Random rnd = new Random();

        //                if (rnd.Next(0, 100) < 70)
        //                {
        //                    var leilao = new Leilao();

        //                    leilao.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
        //                    leilao.Espontaneo = true;
        //                    leilao.Jogador = pedidojog.Jogador;
        //                    leilao.Clube = pedidojog.Jogador.Clube;
        //                    leilao.Valor = pedidojog.Jogador.H * 50000;
        //                }
        //                else
        //                {
        //                    var jogador = jogadorRepository.Get(pedidojog.Jogador.Id);

        //                    jogador.Contrato = true;
        //                    jogador.Salario = pedidojog.Salario;
        //                }
        //            }
        //            else
        //            {
        //                var jogador = jogadorRepository.Get(pedidojog.Jogador.Id);

        //                jogador.Contrato = true;
        //                jogador.Salario = pedidojog.Salario;
        //            }
        //        }
        //        jogadorpedidoRepository.Delete(pedidojog);
        //    }

        //    return RedirectToAction("Index", "Engine");
        //}

        //[Transaction]
        //public ActionResult CriarPedidoSaidasJogadores()
        //{
        //    var controle = controleRepository.GetAll().FirstOrDefault();
        //    foreach (var clube in clubeRepository.GetAll())
        //    {
        //        Random rnd = new Random();

        //        var lstjog = clube.Jogadores.ToList();

        //        if (rnd.Next(1, 3) == 1)
        //        {
        //            if (lstjog.Where(x => !x.Contrato).Count() > 0 && rnd.Next(0, 100) > 70)
        //            {
        //                var lstjognovo = lstjog.Where(x => !x.Contrato).ToList();
        //                var jog = rnd.Next(0, lstjognovo.Count());

        //                decimal aumento = (rnd.Next(1, 4)) / 10;

        //                var pedidojog = new JogadorPedido();
        //                pedidojog.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
        //                pedidojog.Jogador = lstjognovo[jog];
        //                pedidojog.Salario = lstjognovo[jog].Salario + (lstjognovo[jog].Salario * aumento);

        //                jogadorpedidoRepository.SaveOrUpdate(pedidojog);
        //            }
        //        }
        //        else
        //        {
        //            if (lstjog.Where(x => !x.Contrato).Count() > 0 && rnd.Next(0, 100) > 50)
        //            {
        //                var g = lstjog.Where(x => x.Posicao == 1).Count();
        //                var ld = lstjog.Where(x => x.Posicao == 2).Count();
        //                var z = lstjog.Where(x => x.Posicao == 3).Count();
        //                var le = lstjog.Where(x => x.Posicao == 4).Count();
        //                var v = lstjog.Where(x => x.Posicao == 5).Count();
        //                var mo = lstjog.Where(x => x.Posicao == 6).Count();
        //                var a = lstjog.Where(x => x.Posicao == 7).Count();

        //                var lstPos = new List<int>();

        //                if (g > 1)
        //                    lstPos.Add(1);
        //                if (ld > 1)
        //                    lstPos.Add(2);
        //                if (z > 2)
        //                    lstPos.Add(3);
        //                if (le > 1)
        //                    lstPos.Add(4);
        //                if (v > 2)
        //                    lstPos.Add(5);
        //                if (mo > 2)
        //                    lstPos.Add(6);
        //                if (a > 2)
        //                    lstPos.Add(7);

        //                var lstjognovo = lstjog.Where(x => !x.Contrato && x.Posicao.IsIn(lstPos)).ToList();
        //                var jog = rnd.Next(0, lstjognovo.Count());

        //                var leilao = new Leilao();
        //                leilao.Dia = controle.Dia + 1 < controle.DiaMax ? controle.Dia + 1 : 1;
        //                leilao.Espontaneo = true;
        //                leilao.Jogador = lstjognovo[jog];
        //                leilao.Clube = lstjognovo[jog].Clube;
        //                leilao.Valor = lstjognovo[jog].H * 50000;
        //                leilaoRepository.SaveOrUpdate(leilao);
        //            }
        //        }
        //    }
        //    return RedirectToAction("Index", "Engine");
        //}

        [Transaction]
        public ActionResult VariarJogador()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var rnd = new Random();
            foreach (var jogador in jogadorRepository.GetAll().Where(x => !x.Temporario))
            {
                var variavel = rnd.Next(-2, 3);

                jogador.H = jogador.H + (variavel);

                if (jogador.H > (jogador.HF + 10))
                    jogador.H = jogador.HF + 10;
                else if (jogador.H < (jogador.HF - 10))
                    jogador.H = jogador.HF - 10;

                var clubejogadores = jogador.Clube.Jogadores.Where(x => !x.Temporario);

                var hmediotime = clubejogadores.Sum(x => x.H) / clubejogadores.Count();

                if (jogador.Lesionado == 0)
                {
                    var probjogarbem = 50 + (jogador.H - hmediotime);
                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {


                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    jogador.Treinos = jogador.Treinos + 1;
                    jogador.TreinoTotal = jogador.TreinoTotal + Convert.ToDecimal(nota);
                    jogador.TreinoUlt = Convert.ToDecimal(nota);

                    var escalacao = escalacaoRepository.GetAll().FirstOrDefault(x => x.Jogador != null && x.Jogador.Id == jogador.Id);
                    if (escalacao == null)
                        jogador.NotaUlt = 0;
                }
                else
                {
                    jogador.Lesionado = jogador.Lesionado - 1;
                    jogador.TreinoUlt = 0;

                    if (jogador.Lesionado == 0 && jogador.Clube != null && jogador.Clube.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "<a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> está 100% recuperado da lesão e disponível para jogar.";
                        noticia.Usuario = jogador.Clube.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                }                

                jogadorRepository.SaveOrUpdate(jogador);
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("AtualizarDataDia", "Engine");
        }

        [Transaction]
        public ActionResult LesionarJogador()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var rnd = new Random();
            var lstjogador = jogadorRepository.GetAll().Where(x => !x.Temporario);

            var qnt = Convert.ToInt32(lstjogador.Count() / 100);

            for (int i = 0; i < qnt; i++)
            {
                var jogador = lstjogador.ElementAt(rnd.Next(0, lstjogador.Count()));

                if (jogador.Lesionado == 0)
                {
                    var tempo = rnd.Next(1, 11);

                    if (jogador.Clube.Usuario != null)
                    {
                        var usuario = jogador.Clube.Usuario;
                        if (usuario.Staffs.Where(x => x.Tipo == 2).Count() > 0)
                        {
                            var medico = usuario.Staffs.Where(x => x.Tipo == 2).FirstOrDefault();
                            if (rnd.Next(1, 101) <= medico.H)
                                tempo = tempo / 2;
                        }
                    }
                    else
                        tempo = tempo / 2;

                    jogador.Lesionado = tempo;
                    jogadorRepository.SaveOrUpdate(jogador);

                    var escalacao = escalacaoRepository.GetAll().FirstOrDefault(x => x.Jogador != null && x.Jogador.Id == jogador.Id);
                    if (escalacao != null)
                    {
                        escalacao.Jogador = null;
                        escalacao.H = 0;
                        escalacaoRepository.SaveOrUpdate(escalacao);
                    }

                    if (jogador.Clube.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "<a href='" + Url.Action("Index", "Jogador", new { id = jogador.Id }) + "'>" + jogador.Nome + "</a> se lesionou por " + jogador.Lesionado + " dia(s)";
                        noticia.Usuario = jogador.Clube.Usuario;
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                }
            }

            return RedirectToAction("Index", "Engine");
            //return RedirectToAction("AtualizarDataDia", "Engine");
        }

        #endregion

        #region MudarAno

        [Transaction]
        public ActionResult AtualizarDataAno()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            controle.Data = DateTime.Now;
            controle.Dia = 1;
            controle.Ano = controle.Ano + 1;
            controle.Taca = 32;
            controleRepository.SaveOrUpdate(controle);

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult GerarHistoricoTaca()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var partidafinaltaca = partidaRepository.GetAll().Where(x => x.Tipo == "TACA" && x.Rodada == 1 && x.Realizada && x.Mao == 2).FirstOrDefault();
            var historico = new Historico();

            if (partidafinaltaca.Vencedor.Usuario != null)
            {
                var usuario = partidafinaltaca.Vencedor.Usuario;
                historico.Usuario = usuario;

                usuario.Reputacao = (usuario.Reputacao + 40) > 50 ? 50 : (usuario.Reputacao + 40);
                usuario.ReputacaoGeral = usuario.ReputacaoGeral + 40;
                usuarioRepository.SaveOrUpdate(usuario);

                var noticia = new Noticia();
                noticia.Dia = controle.Dia;
                noticia.Texto = "Parabéns! Você conquistou a TAÇA!";
                noticiaRepository.SaveOrUpdate(noticia);
            }

            historico.Ano = controle.Ano;
            historico.Taca = true;
            historico.Campeao = partidafinaltaca.Vencedor;
            historico.Vice = partidafinaltaca.Vencedor == partidafinaltaca.Clube1 ? partidafinaltaca.Clube2 : partidafinaltaca.Clube1;
            var artilheiro = artilheiroRepository.GetAll().OrderByDescending(x => x.Taca).FirstOrDefault();
            historico.Artilheiro = artilheiro.Jogador;
            historico.ArtilheiroClube = artilheiro.Jogador.Clube;
            historico.Gols = artilheiro.Taca;
            historicoRepository.SaveOrUpdate(historico);

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ZeraAnoJogadores()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            foreach (var jog in jogadorRepository.GetAll().Where(x => !x.Temporario && x.Clube != null))
            {
                var historico = new JogadorHistorico();
                historico.Ano = controle.Ano;
                historico.Clube = jog.Clube;
                historico.Jogador = jog;
                historico.NotaMedia = jog.NotaMedia > 0.0 ? jog.NotaMedia : 0.00;
                historico.Jogos = jog.Jogos;
                historico.Gols = jog.Gols.Where(x => x.Clube.Id == jog.Clube.Id).Count();
                jogadorhistoricoRepository.SaveOrUpdate(historico);

                jog.Jogos = 0;
                jog.NotaTotal = 0;
                jog.NotaUlt = 0;
                jog.Treinos = 0;
                jog.TreinoTotal = 0;
                jog.TreinoUlt = 0;
                jog.Contrato = (jog.Contrato - 1) >= 0 ? jog.Contrato - 1 : 0;

                if (jog.Contrato == 0)
                {
                    if (jog.Clube.Jogadores.Count() > 14)
                    {
                        if (jog.Clube.Usuario != null)
                        {
                            var noticia = new Noticia();
                            noticia.Dia = controle.Dia;
                            noticia.Texto = "<a href='" + Url.Action("Index", "Jogador", new { id = jog.Id }) + "'>" + jog.Nome + "</a> encerrou seu contrato e deixou o clube.";
                            noticia.Usuario = jog.Clube.Usuario;
                        }

                        jog.Clube = null;
                        jog.Jogos = 0;
                        jog.Salario = 0;
                    }
                    else
                    {
                        if (jog.Clube.Usuario != null)
                        {
                            var noticia = new Noticia();
                            noticia.Dia = controle.Dia;
                            noticia.Texto = "<a href='" + Url.Action("Index", "Jogador", new { id = jog.Id }) + "'>" + jog.Nome + "</a> teve seu contrato prorrogado por 1 ano com aumento de 20%, pois você não pode ter menos que 14 jogadores.";
                            noticia.Usuario = jog.Clube.Usuario;
                        }

                        jog.Salario = (jog.Salario / 100) * 120;
                        jog.Contrato = 1;
                    }
                }
                

                jogadorRepository.SaveOrUpdate(jog);
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ZerarAnoStaff()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            foreach (var staff in staffRepository.GetAll().Where(x => x.Usuario != null))
            {
                staff.Contrato = (staff.Contrato - 1) >= 0 ? staff.Contrato - 1 : 0;

                if (staff.Contrato == 0)
                {
                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Seu contrato com o " + Util.Util.RetornaStaffTipo(staff.Tipo) + " <a href='" + Url.Action("Index", "Staff", new { id = staff.Id }) + "'>" + staff.Nome + "</a> terminou. Ele deixou sua comissão técnica.";
                    noticia.Usuario = staff.Usuario;
                    noticiaRepository.SaveOrUpdate(noticia);

                    staff.Usuario = null;
                    staff.Salario = 0;
                }                

                staffRepository.SaveOrUpdate(staff);
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult AlterarDivisaoTimes()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            var ultdivisao = divisaoRepository.GetAll().OrderByDescending(x => x.Numero).FirstOrDefault().Numero;

            foreach (var divisao in divisaoRepository.GetAll().OrderBy(x => x.Numero))
            {
                var tabela = divisaotabelaRepository.GetAll().Where(x => x.Divisao.Id == divisao.Id).OrderByDescending(x => x.Posicao).ToList();

                var clube1 = tabela[0].Clube; //Campeão
                var clube2 = tabela[1].Clube; //Vice-Campeão
                var clube11 = tabela[10].Clube; //Penúltimo
                var clube12 = tabela[11].Clube; //Último

                ////////////////////////////////////////////Gerar reputação
                if (clube1.Usuario != null)
                {
                    var usuario = clube1.Usuario;

                    usuario.Reputacao = (usuario.Reputacao + (40 / divisao.Numero)) > 50 ? 50 : usuario.Reputacao + (40 / divisao.Numero);
                    usuario.ReputacaoGeral = usuario.ReputacaoGeral + (40 / divisao.Numero);
                    usuarioRepository.SaveOrUpdate(usuario);

                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Parabéns! Você foi o CAMPEÃO da " + divisao.Nome +"!";
                    noticiaRepository.SaveOrUpdate(noticia);
                }
                else
                {
                    clube1.ReputacaoAI = (clube1.ReputacaoAI + (40 / divisao.Numero)) > 50 ? 50 : clube1.ReputacaoAI + (40 / divisao.Numero);
                }

                if (clube2.Usuario != null)
                {
                    var usuario = clube2.Usuario;

                    usuario.Reputacao = (usuario.Reputacao + (20 / divisao.Numero)) > 50 ? 50 : usuario.Reputacao + (20 / divisao.Numero);
                    usuario.ReputacaoGeral = usuario.ReputacaoGeral + (20 / divisao.Numero);
                    usuarioRepository.SaveOrUpdate(usuario);

                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Parabéns! Você foi o vice-campeão da " + divisao.Nome + "!";
                    noticiaRepository.SaveOrUpdate(noticia);
                }
                else
                {
                    clube2.ReputacaoAI = (clube2.ReputacaoAI + (20 / divisao.Numero)) > 50 ? 50 : clube2.ReputacaoAI + (20 / divisao.Numero);
                }

                if (clube11.Usuario != null)
                {
                    var usuario = clube11.Usuario;

                    usuario.Reputacao = usuario.Reputacao - 30 < 0 ? 0 : usuario.Reputacao - 30;
                    usuario.ReputacaoGeral = usuario.ReputacaoGeral - 15;
                    usuarioRepository.SaveOrUpdate(usuario);
                }
                else
                {
                    clube11.ReputacaoAI = clube11.ReputacaoAI - 30 < 0 ? 0 : clube11.ReputacaoAI - 30;
                }

                if (clube12.Usuario != null)
                {
                    var usuario = clube12.Usuario;

                    usuario.Reputacao = usuario.Reputacao - 30 < 0 ? 0 : usuario.Reputacao - 30;
                    usuario.ReputacaoGeral = usuario.ReputacaoGeral - 15;
                    usuarioRepository.SaveOrUpdate(usuario);
                }
                else
                {
                    clube12.ReputacaoAI = clube12.ReputacaoAI - 30 < 0 ? 0 : clube12.ReputacaoAI - 30;
                }

                ////////////////////////////////////////////Gerar historico
                var historico = new Historico();
                historico.Usuario = clube1.Usuario != null ? clube1.Usuario : null;
                historico.Ano = controle.Ano;
                historico.Divisao = divisao;
                historico.Campeao = clube1;
                historico.Vice = clube2;
                var artilheiro = artilheiroRepository.GetAll().Where(x => x.Clube.Divisao.Id == divisao.Id).OrderByDescending(x => x.Divisao).FirstOrDefault();
                historico.Artilheiro = artilheiro.Jogador;
                historico.ArtilheiroClube = artilheiro.Jogador.Clube;
                historico.Gols = artilheiro.Divisao;
                historicoRepository.SaveOrUpdate(historico);

                ////////////////////////////////////////////Pagar prêmios
                var premio = 6000000 / divisao.Numero;
                clube1.Dinheiro = clube1.Dinheiro + premio;
                clube2.Dinheiro = clube1.Dinheiro + (premio * Convert.ToDecimal(0.30));

                if (divisao.Numero > 1)
                {
                    var promodivisao = divisaoRepository.GetAll().Where(x => x.Numero == (divisao.Numero - 1)).FirstOrDefault();

                    clube1.Divisao = promodivisao;
                    clube2.Divisao = promodivisao;

                    if (clube1.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Você foi promovido para a " + promodivisao.Nome + " devido ao título!";
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                    if (clube2.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Você foi promovido para a " + promodivisao.Nome + " devido ao vice-campeonato!";
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                }

                if (divisao.Numero < ultdivisao)
                {
                    var rebaixadivisao = divisaoRepository.GetAll().Where(x => x.Numero == (divisao.Numero + 1)).FirstOrDefault();

                    clube11.Divisao = rebaixadivisao;
                    clube12.Divisao = rebaixadivisao;

                    if (clube11.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Você foi rebaixado para a " + rebaixadivisao.Nome + " devido a sua posição final!";
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                    if (clube12.Usuario != null)
                    {
                        var noticia = new Noticia();
                        noticia.Dia = controle.Dia;
                        noticia.Texto = "Você foi promovido para a " + rebaixadivisao.Nome + " devido a sua posição final!";
                        noticiaRepository.SaveOrUpdate(noticia);
                    }
                }

                clubeRepository.SaveOrUpdate(clube1);
                clubeRepository.SaveOrUpdate(clube2);
                clubeRepository.SaveOrUpdate(clube11);
                clubeRepository.SaveOrUpdate(clube12);
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ZerarCampeonato()
        {
            foreach (var partida in partidaRepository.GetAll())
            {
                partidaRepository.Delete(partida);
            }
            foreach (var tabela in divisaotabelaRepository.GetAll())
            {
                divisaotabelaRepository.Delete(tabela);
            }
            foreach (var gol in golRepository.GetAll())
            {
                golRepository.Delete(gol);
            }
            foreach (var clube in clubeRepository.GetAll())
            {
                clube.Taca = false;
                clubeRepository.SaveOrUpdate(clube);
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult GerarCampeonato()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            foreach (var divisao in divisaoRepository.GetAll().OrderBy(x => x.Numero))
            {
                var lstclubes = divisao.Clubes.OrderBy(x => x.Nome).ToList();

                ////////////////////////////////////////////Gerar Tabelas
                foreach (var clube in lstclubes)
                {
                    var i = 1;
                    var divisatabela = new DivisaoTabela();
                    divisatabela.Clube = clube;
                    divisatabela.D = 0;
                    divisatabela.Divisao = divisao;
                    divisatabela.E = 0;
                    divisatabela.GC = 0;
                    divisatabela.GP = 0;
                    divisatabela.Pontos = 0;
                    divisatabela.Posicao = i;
                    divisatabela.V = 0;

                    divisaotabelaRepository.SaveOrUpdate(divisatabela);

                    i++;
                }

                ////////////////////////////////////////////Gerar Partidas
                lstclubes = lstclubes.OrderByDescending(x => x.Dinheiro).ToList();

                int t1 = 0;
                int t2 = 1;
                int t3 = 11; // ultimo time
                int t4 = 1;
                int t5 = 11;
                int dia = 1;
                for (int i = 1; i < 23; i++) // 22 = rodadas
                {
                    if (dia == 4 || dia == 7 || dia == 10 || dia == 13 || dia == 16 || dia == 19 || dia == 22 || dia == 25 || dia == 28)
                        dia++;

                    t2 = t4;
                    t3 = t5;
                    if ((i % 2) != 0)
                    {
                        var partida = new Partida();
                        partida.Clube1 = lstclubes[t1];
                        partida.Clube2 = lstclubes[t2];
                        partida.Rodada = i;
                        partida.Tipo = "DIVISAO";
                        partida.Dia = dia;
                        partida.Divisao = divisao;
                        partida.Mao = i < 12 ? 1 : 2;
                        partidaRepository.SaveOrUpdate(partida);

                        for (int i2 = 1; i2 < 6; i2++) // 6 = metade do total de times
                        {
                            t2++;
                            if (t2 > 11) // 12 = max de times
                                t2 = 1;

                            partida = new Partida();
                            partida.Clube1 = lstclubes[t3];
                            partida.Clube2 = lstclubes[t2];
                            partida.Rodada = i;
                            partida.Tipo = "DIVISAO";
                            partida.Dia = dia;
                            partida.Divisao = divisao;
                            partida.Mao = i < 12 ? 1 : 2;
                            partidaRepository.SaveOrUpdate(partida);

                            t3--;
                            if (t3 < 1) // 2 = min de times
                                t3 = 11;
                        }
                    }
                    else
                    {
                        var partida = new Partida();
                        partida.Clube1 = lstclubes[t2];
                        partida.Clube2 = lstclubes[t1];
                        partida.Rodada = i;
                        partida.Tipo = "DIVISAO";
                        partida.Dia = dia;
                        partida.Divisao = divisao;
                        partida.Mao = i < 12 ? 1 : 2;
                        partidaRepository.SaveOrUpdate(partida);

                        for (int i2 = 1; i2 < 6; i2++) // 6 = metade do total de times
                        {
                            t2++;
                            if (t2 > 11) // 12 = max de times
                                t2 = 1;

                            partida = new Partida();
                            partida.Clube1 = lstclubes[t2];
                            partida.Clube2 = lstclubes[t3];
                            partida.Rodada = i;
                            partida.Tipo = "DIVISAO";
                            partida.Dia = dia;
                            partida.Divisao = divisao;
                            partida.Mao = i < 12 ? 1 : 2;
                            partidaRepository.SaveOrUpdate(partida);
                            t3--;
                            if (t3 < 1) // 12 = max de times
                                t3 = 11;
                        }
                    }
                    t4--;
                    if (t4 < 1) // 2 = min de times
                        t4 = 11;
                    t5--;
                    if (t5 < 1) // 2 = min de times
                        t5 = 11;

                    dia++;
                }
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult ClassificadosTaca()
        {
            var itaca = 1;
            var controle = controleRepository.GetAll().FirstOrDefault();

            foreach (var divisaotabela in divisaotabelaRepository.GetAll().OrderBy(x => x.Divisao.Numero).ThenBy(x => x.Posicao))
            {
                var clube = clubeRepository.Get(divisaotabela.Clube.Id);
                clube.Taca = true;

                clubeRepository.SaveOrUpdate(clube);

                if (clube.Usuario != null)
                {
                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Devido sua colocação você foi classificado para participar da TAÇA.";
                    noticiaRepository.SaveOrUpdate(noticia);
                }

                itaca++;
                if (itaca > 32)
                    break;
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult GerarTacaAno()
        {
            var lstTaca = clubeRepository.GetAll().Where(x => x.Taca).ToList();         


            for (int i = 0; i < 16; i++) // 16 partidas
            {
                Random rnd = new Random();

                var maxclubes = 32 - (2 * i);
                var clube1 = lstTaca[rnd.Next(0, maxclubes)];
                var clube2 = lstTaca[rnd.Next(0, maxclubes)];

                while (clube1.Id == clube2.Id)
                {
                    clube2 = lstTaca[rnd.Next(0, maxclubes)];
                }

                var partida1 = new Partida();
                partida1.Dia = 4;
                partida1.Mao = 1;
                partida1.Rodada = 16;
                partida1.Tipo = "TACA";
                partida1.Clube1 = clube1;
                partida1.Clube2 = clube2;

                var partida2 = new Partida();
                partida2.Dia = 7;
                partida2.Mao = 2;
                partida2.Rodada = 16;
                partida2.Tipo = "TACA";
                partida2.Clube1 = clube2;
                partida2.Clube2 = clube1;

                partidaRepository.SaveOrUpdate(partida1);
                partidaRepository.SaveOrUpdate(partida2);

                lstTaca.Remove(clube1);
                lstTaca.Remove(clube2);
            }

            return RedirectToAction("Index", "Engine");
        }

        [Transaction]
        public ActionResult IniciarAno()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            foreach (var clube in clubeRepository.GetAll())
            {
                var valor = 8000000 / clube.Divisao.Numero;

                if (clube.Usuario != null)
                {
                    var noticia = new Noticia();
                    noticia.Dia = controle.Dia;
                    noticia.Texto = "Você recebeu $" + valor.ToString("N2") + " pelos direitos televisivos da " + clube.Divisao.Nome + ".";
                    noticia.Usuario = clube.Usuario;
                    noticiaRepository.SaveOrUpdate(noticia);
                }

                clube.Dinheiro = clube.Dinheiro + valor;
                clubeRepository.SaveOrUpdate(clube);
            }

            return RedirectToAction("Index", "Engine");
        }

        #endregion

        #region GerarTaca

        [Transaction]
        public ActionResult GerarTaca()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();
            
            var lstTaca = new List<Clube>();
            var rodada = controle.Taca / 2;

            var partidas = partidaRepository.GetAll().Where(x => x.Tipo == "TACA" && x.Realizada && x.Mao == 2 && x.Rodada == rodada).OrderByDescending(x => x.Rodada);

            var dia1 = 0;
            var dia2 = 0;

            if (partidas.Count() > 0)
            {
                foreach (var part in partidas)
                {
                    lstTaca.Add(part.Vencedor);
                }

                controle.Taca = rodada;
                controleRepository.SaveOrUpdate(controle);

                if (rodada == 16)
                {
                    rodada = 8;
                    dia1 = 10;
                    dia2 = 13;
                }
                else if (rodada == 8)
                {
                    rodada = 4;
                    dia1 = 16;
                    dia2 = 19;
                }
                else if (rodada == 4)
                {
                    rodada = 2;
                    dia1 = 22;
                    dia2 = 25;
                }
                else
                {
                    rodada = 1;
                    dia1 = 28;
                    dia2 = 32;
                }

                for (int i = 0; i < rodada; i++) //rodada = numero de partidas
                {
                    Random rnd = new Random();

                    var maxclubes = (rodada * 2) - (2 * i);
                    var clube1 = lstTaca[rnd.Next(0, maxclubes)];
                    var clube2 = lstTaca[rnd.Next(0, maxclubes)];

                    while (clube1.Id == clube2.Id)
                    {
                        clube2 = lstTaca[rnd.Next(0, maxclubes)];
                    }

                    var partida1 = new Partida();
                    partida1.Dia = dia1;
                    partida1.Mao = 1;
                    partida1.Rodada = rodada;
                    partida1.Tipo = "TACA";
                    partida1.Clube1 = clube1;
                    partida1.Clube2 = clube2;

                    var partida2 = new Partida();
                    partida2.Dia = dia2;
                    partida2.Mao = 2;
                    partida2.Rodada = rodada;
                    partida2.Tipo = "TACA";
                    partida2.Clube1 = clube2;
                    partida2.Clube2 = clube1;

                    partidaRepository.SaveOrUpdate(partida1);
                    partidaRepository.SaveOrUpdate(partida2);

                    lstTaca.Remove(clube1);
                    lstTaca.Remove(clube2);
                }
            }
            return RedirectToAction("Index", "Engine");
        }
        #endregion

        #region Partida

        [Transaction]
        public ActionResult RodaPartida()
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            var lstPartidas = partidaRepository.GetAll().Where(x => x.Dia <= controle.Dia && !x.Realizada).ToList();

            try
            {
                ////////////////////////////////FOR PARTIDAS
                foreach (var partida in lstPartidas)
                {
                    var clube1 = clubeRepository.Get(partida.Clube1.Id);
                    var clube2 = clubeRepository.Get(partida.Clube2.Id);
                    var gol1 = 0;
                    var gol2 = 0;

                    var def1 = clube1.Escalacao.Where(x => x.Posicao == 1 || x.Posicao == 2 || x.Posicao == 3 || x.Posicao == 4 || x.Posicao == 5);
                    var def2 = clube2.Escalacao.Where(x => x.Posicao == 1 || x.Posicao == 2 || x.Posicao == 3 || x.Posicao == 4 || x.Posicao == 5);
                    var ata1 = clube1.Escalacao.Where(x => x.Posicao == 6 || x.Posicao == 7);
                    var ata2 = clube2.Escalacao.Where(x => x.Posicao == 6 || x.Posicao == 7);

                    var Defesa1 = Convert.ToInt32(def1.Sum(x => x.H) / def1.Count());
                    var Defesa2 = Convert.ToInt32(def2.Sum(x => x.H) / def2.Count());
                    var Ataque1 = Convert.ToInt32(ata1.Sum(x => x.H) / ata1.Count());
                    var Ataque2 = Convert.ToInt32(ata2.Sum(x => x.H) / ata2.Count());

                    var Diferenca = (Defesa1 - Defesa2) + (Ataque1 - Ataque2);

                    var Prob1 = 60;
                    var Prob2 = 40;
                    var ProbEmpate = 0;

                    if ((Diferenca * (-1)) > 30)
                        ProbEmpate = 6;
                    else if ((Diferenca * (-1)) > 20)
                        ProbEmpate = 12;
                    else if ((Diferenca * (-1)) > 10)
                        ProbEmpate = 30;
                    else if ((Diferenca * (-1)) >= 0)
                        ProbEmpate = 40;

                    Prob1 = (Prob1 + (Diferenca));
                    Prob2 = (Prob2 - (Diferenca));

                    Random rnd = new Random();

                    var resultado = rnd.Next(0, (Prob1 + Prob2 + ProbEmpate));
                    var vencedor = new Clube();

                    var placar = rnd.Next(1, 101);

                    if (resultado <= Prob1)
                    {
                        vencedor = clube1;
                        if (clube1.Usuario != null)
                        {
                            var usuario = clube1.Usuario;

                            usuario.Reputacao = usuario.Reputacao + 4 > 50 ? 50 : usuario.Reputacao + 4;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else
                        {
                            clube1.ReputacaoAI = clube1.ReputacaoAI + 4 > 50 ? 50 : clube1.ReputacaoAI + 4;
                        }

                        if (clube2.Usuario != null)
                        {
                            var usuario = clube2.Usuario;

                            usuario.Reputacao = usuario.Reputacao - 6 < 0 ? 0 : usuario.Reputacao - 6;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else
                        {
                            clube2.ReputacaoAI = clube2.ReputacaoAI - 6 < 0 ? 0 : clube2.ReputacaoAI - 6;
                        }

                        var dif7 = Convert.ToInt32(((double)Prob1 / 100) * 2);
                        var dif6 = Convert.ToInt32(((double)Prob1 / 100) * 2);
                        var dif5 = Convert.ToInt32(((double)Prob1 / 100) * 3);
                        var dif4 = Convert.ToInt32(((double)Prob1 / 100) * 5);
                        var dif3 = Convert.ToInt32(((double)Prob1 / 100) * 20);
                        var dif2 = Convert.ToInt32(((double)Prob1 / 100) * 50);

                        if (placar <= dif7)
                        {
                            gol2 = rnd.Next(0, 2);
                            gol1 = gol2 + 7;
                        }
                        else if (placar <= (dif7 + dif6))
                        {
                            gol2 = rnd.Next(0, 2);
                            gol1 = gol2 + 6;
                        }
                        else if (placar <= (dif7 + dif6 + dif5))
                        {
                            gol2 = rnd.Next(0, 2);
                            gol1 = gol2 + 5;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4))
                        {
                            gol2 = rnd.Next(0, 3);
                            gol1 = gol2 + 4;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3))
                        {
                            gol2 = rnd.Next(0, 2);
                            gol1 = gol2 + 3;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                        {
                            gol2 = rnd.Next(0, 3);
                            gol1 = gol2 + 2;
                        }
                        else if (placar > (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                        {
                            gol2 = rnd.Next(0, 3);
                            gol1 = gol2 + 1;
                        }
                    }
                    else if (resultado <= (Prob1 + ProbEmpate))
                    {
                        vencedor = null;
                        if (clube1.Usuario != null)
                        {
                            var usuario = clube1.Usuario;

                            usuario.Reputacao = usuario.Reputacao - 3 < 0 ? 0 : usuario.Reputacao - 3;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else
                        {
                            clube1.ReputacaoAI = clube1.ReputacaoAI - 3 < 0 ? 0 : clube1.ReputacaoAI - 3;
                        }

                        if (placar < 35)
                        {
                            gol2 = 0;
                            gol1 = 0;
                        }
                        else if (placar < 70)
                        {
                            gol2 = 1;
                            gol1 = 1;
                        }
                        else if (placar < 90)
                        {
                            gol2 = 2;
                            gol1 = 2;
                        }
                        else if (placar < 98)
                        {
                            gol2 = 3;
                            gol1 = 3;
                        }
                        else if (placar >= 98)
                        {
                            gol2 = 4;
                            gol1 = 4;
                        }
                    }
                    else
                    {
                        vencedor = clube2;
                        if (clube1.Usuario != null)
                        {
                            var usuario = clube1.Usuario;

                            usuario.Reputacao = usuario.Reputacao - 8 < 0 ? 0 : usuario.Reputacao - 8;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else
                        {
                            clube1.ReputacaoAI = clube1.ReputacaoAI - 8 < 0 ? 0 : clube1.ReputacaoAI - 8;
                        }

                        if (clube2.Usuario != null)
                        {
                            var usuario = clube2.Usuario;

                            usuario.Reputacao = usuario.Reputacao + 6 > 50 ? 50 : usuario.Reputacao + 6;
                            usuarioRepository.SaveOrUpdate(usuario);
                        }
                        else
                        {
                            clube2.ReputacaoAI = clube2.ReputacaoAI + 6 > 50 ? 50 : clube2.ReputacaoAI + 6;
                        }

                        var dif7 = Convert.ToInt32(((double)Prob1 / 100) * 2);
                        var dif6 = Convert.ToInt32(((double)Prob1 / 100) * 2);
                        var dif5 = Convert.ToInt32(((double)Prob1 / 100) * 3);
                        var dif4 = Convert.ToInt32(((double)Prob1 / 100) * 4);
                        var dif3 = Convert.ToInt32(((double)Prob1 / 100) * 15);
                        var dif2 = Convert.ToInt32(((double)Prob1 / 100) * 40);

                        if (placar <= dif7)
                        {
                            gol1 = rnd.Next(0, 2);
                            gol2 = gol1 + 7;
                        }
                        else if (placar <= (dif7 + dif6))
                        {
                            gol1 = rnd.Next(0, 2);
                            gol2 = gol1 + 6;
                        }
                        else if (placar <= (dif7 + dif6 + dif5))
                        {
                            gol1 = rnd.Next(0, 2);
                            gol2 = gol1 + 5;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4))
                        {
                            gol1 = rnd.Next(0, 3);
                            gol2 = gol1 + 4;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3))
                        {
                            gol1 = rnd.Next(0, 2);
                            gol2 = gol1 + 3;
                        }
                        else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                        {
                            gol1 = rnd.Next(0, 3);
                            gol2 = gol1 + 2;
                        }
                        else if (placar > (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                        {
                            gol1 = rnd.Next(0, 3);
                            gol2 = gol1 + 1;
                        }
                    }

                    if (partida.Tipo == "TACA" && partida.Mao == 2)
                    {
                        var partida1mao = partidaRepository.GetAll().Where(x => x.Rodada == partida.Rodada && x.Mao == 1 && x.Clube1.Id == partida.Clube2.Id && x.Clube2.Id == partida.Clube1.Id).FirstOrDefault();

                        var teste1 = "1 MAO: " + partida1mao.Clube1.Nome + " " + partida1mao.Gol1 + " x " + partida1mao.Gol2 + " " + partida1mao.Clube2.Nome;
                        var teste2 = "2 MAO: " + partida.Clube1.Nome + " " + gol1 + " x " + gol2 + " " + partida.Clube2.Nome;

                        var totalgol1 = gol1 + partida1mao.Gol2;
                        var totalgol2 = gol2 + partida1mao.Gol1;

                        if (totalgol1 > totalgol2)
                            vencedor = partida.Clube1;
                        else if (totalgol2 > totalgol1)
                            vencedor = partida.Clube2;
                        else
                        {
                            if (partida1mao.Gol2 > gol2)
                                vencedor = partida.Clube1;
                            else if (partida1mao.Gol2 < gol2)
                                vencedor = partida.Clube2;
                            else if (rnd.Next(1, 3) == 1)
                            {
                                vencedor = partida.Clube1;
                                var penal = rnd.Next(0, 3);
                                if (penal == 0)
                                    partida.Penalti = "5 x 4";
                                else if (penal == 1)
                                    partida.Penalti = "4 x 3";
                                else
                                    partida.Penalti = "3 x 2";
                            }
                            else
                            {
                                vencedor = partida.Clube2;
                                var penal = rnd.Next(0, 3);
                                if (penal == 0)
                                    partida.Penalti = "4 x 5";
                                else if (penal == 1)
                                    partida.Penalti = "3 x 4";
                                else
                                    partida.Penalti = "2 x 3";
                            }
                        }

                    }

                    rnd = new Random();

                    if (gol1 > 0)
                    {
                        var totalgols = clube1.Escalacao.Sum(x => x.HGol) + 1;
                        for (int i = 1; i <= gol1; i++)
                        {
                            var gol = new Gol();
                            gol.Clube = clube1;
                            gol.Minuto = rnd.Next(1, 94);
                            gol.Partida = partida;
                            var contagem = 0;
                            var goleador = rnd.Next(1, totalgols);
                            foreach (var jog in clube1.Escalacao.OrderByDescending(x => x.Posicao))
                            {
                                contagem = contagem + jog.HGol;
                                if (goleador <= contagem)
                                {
                                    gol.Jogador = jog.Jogador;
                                    break;
                                }
                            }
                            golRepository.SaveOrUpdate(gol);
                        }
                    }

                    rnd = new Random();

                    var listgol2 = new List<Gol>();
                    if (gol2 > 0)
                    {
                        var totalgols = clube2.Escalacao.Sum(x => x.HGol) + 1;
                        for (int i = 1; i <= gol2; i++)
                        {
                            var gol = new Gol();
                            gol.Clube = clube2;
                            gol.Minuto = rnd.Next(1, 94);
                            gol.Partida = partida;
                            var contagem = 0;
                            var goleador = rnd.Next(1, totalgols);
                            foreach (var jog in clube2.Escalacao.OrderByDescending(x => x.Posicao))
                            {
                                contagem = contagem + jog.HGol;
                                if (goleador <= contagem)
                                {
                                    gol.Jogador = jog.Jogador;
                                    break;
                                }
                            }
                            golRepository.SaveOrUpdate(gol);
                        }
                    }

                    //publico 
                    var publico = 0;
                    publico = clube1.Socios * 5;
                    publico = publico - ((publico / 12) * (divisaotabelaRepository.GetAll().Where(x => x.Clube.Id == clube1.Id).FirstOrDefault().Posicao - 1));
                    if (clube1.Ingresso > 35)
                        publico = (publico / 100) * 30;
                    else if (clube1.Ingresso > 25)
                        publico = (publico / 100) * 50;

                    publico = rnd.Next((publico - 3000), publico);
                    if (publico > clube1.Estadio)
                        publico = clube1.Estadio;
                    else if (publico <= 0)
                        publico = rnd.Next(500, 2000);

                    clube1.Dinheiro = clube1.Dinheiro + (publico * clube1.Ingresso);
                    clubeRepository.SaveOrUpdate(clube1);
                    clubeRepository.SaveOrUpdate(clube2);

                    partida.Gol1 = gol1;
                    partida.Gol2 = gol2;
                    if (partida.Tipo == "TACA" && partida.Mao == 1)
                        partida.Vencedor = null;
                    else
                        partida.Vencedor = vencedor;
                    partida.Realizada = true;
                    partida.Publico = publico;
                    partidaRepository.SaveOrUpdate(partida);
                }
            }
            catch (Exception ex)
            {
                var teste = ex.ToString();
            }

            return RedirectToAction("Index", "Engine");
        }

        #endregion

        #region AtualizaTabela

        [Transaction]
        public ActionResult AtualizaTabela()
        {
            foreach (var divisao in divisaoRepository.GetAll())
            {
                var list = new List<DivisaoTabela>();

                foreach (var divisaotabela in divisaotabelaRepository.GetAll().Where(x => x.Divisao.Id == divisao.Id))
                {
                    var partidasdoclube = partidaRepository.GetAll().Where(x => x.Tipo != "TACA" && x.Realizada && (x.Clube1.Id == divisaotabela.Clube.Id || x.Clube2.Id == divisaotabela.Clube.Id));

                    var vitorias = partidasdoclube.Where(x => x.Vencedor != null && x.Vencedor.Id == divisaotabela.Clube.Id).Count();
                    var empates = partidasdoclube.Where(x => x.Vencedor == null).Count();
                    var derrotas = partidasdoclube.Where(x => x.Vencedor != null && x.Vencedor.Id != divisaotabela.Clube.Id).Count();
                    var jogos = vitorias + empates + derrotas;
                    var pontos = (vitorias * 3) + empates;
                    var golspro = partidasdoclube.Where(x => x.Clube1.Id == divisaotabela.Clube.Id).Sum(x => x.Gol1) + partidasdoclube.Where(x => x.Clube2.Id == divisaotabela.Clube.Id).Sum(x => x.Gol2);
                    var golscontra = partidasdoclube.Where(x => x.Clube1.Id == divisaotabela.Clube.Id).Sum(x => x.Gol2) + partidasdoclube.Where(x => x.Clube2.Id == divisaotabela.Clube.Id).Sum(x => x.Gol1);

                    divisaotabela.D = derrotas;
                    divisaotabela.E = empates;
                    divisaotabela.GC = golscontra;
                    divisaotabela.GP = golspro;
                    divisaotabela.J = jogos;
                    divisaotabela.Pontos = pontos;
                    divisaotabela.V = vitorias;

                    list.Add(divisaotabela);
                }

                var i = 1;

                foreach (var dt in list.OrderByDescending(x => x.Pontos).ThenByDescending(x => x.V).ThenByDescending(x => x.Saldo).ThenByDescending(x => x.GP))
                {
                    dt.Posicao = i;
                    divisaotabelaRepository.SaveOrUpdate(dt);
                    i++;
                }
            }

            return RedirectToAction("Index", "Engine");
        }

        #endregion

        #region EscalaTimes

        [Transaction]
        public ActionResult EscalaTimes()
        {
            try
            {            
                foreach (var clube in clubeRepository.GetAll())
                {
                    var lstnova = new List<Escalacao>();

                    //GOLEIRO
                    var escalacao = new Escalacao();
                    escalacao.Clube = clube;
                    escalacao.Posicao = 1;
                    escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 1).OrderByDescending(x => x.H).FirstOrDefault();
                    if (escalacao.Jogador.H >= 90)
                        escalacao.H = escalacao.Jogador.H + 20;
                    else if (escalacao.Jogador.H >= 80)
                        escalacao.H = escalacao.Jogador.H + 10;
                    else if (escalacao.Jogador.H >= 70)
                        escalacao.H = escalacao.Jogador.H + 5;
                    else
                        escalacao.H = escalacao.Jogador.H;
                    
                    //escalacao.HGol = 1;
                    lstnova.Add(escalacao);

                    //LATERAL-DIREITO
                    if (clube.Formacao.Substring(0, 1) != "3")
                    {
                        escalacao = new Escalacao();
                        escalacao.Clube = clube;
                        escalacao.Posicao = 2;
                        escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 2).OrderByDescending(x => x.H).FirstOrDefault();
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        //escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
                        lstnova.Add(escalacao);
                    }

                    //ZAGUEIROS
                    var zagueiros = clube.Jogadores.Where(x => x.Posicao == 3).OrderByDescending(x => x.H).Take(3);
                    if (clube.Formacao.Substring(0, 1) == "4")
                        zagueiros = clube.Jogadores.Where(x => x.Posicao == 3).OrderByDescending(x => x.H).Take(2);

                    foreach (var zag in zagueiros)
                    {
                        escalacao = new Escalacao();
                        escalacao.Clube = clube;
                        escalacao.Posicao = 3;
                        escalacao.Jogador = zag;
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        //escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
                        lstnova.Add(escalacao);
                    }

                    //LATERAL-ESQUERDO
                    if (clube.Formacao.Substring(0, 1) != "3")
                    {
                        escalacao = new Escalacao();
                        escalacao.Clube = clube;
                        escalacao.Posicao = 4;
                        escalacao.Jogador = clube.Jogadores.Where(x => x.Posicao == 4).OrderByDescending(x => x.H).FirstOrDefault();
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        //escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
                        lstnova.Add(escalacao);
                    }

                    //VOLANTE
                    var volantes = clube.Jogadores.Where(x => x.Posicao == 5).OrderByDescending(x => x.H).Take(Convert.ToInt32(clube.Formacao.Substring(1, 1)));
                    foreach (var vol in volantes)
                    {
                        escalacao = new Escalacao();
                        escalacao.Clube = clube;
                        escalacao.Posicao = 5;
                        escalacao.Jogador = vol;
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        //escalacao.HGol = escalacao.Jogador.H / 5 > 1 ? escalacao.Jogador.H / 5 : 1;
                        lstnova.Add(escalacao);
                    }

                    //MEIA OFENSIVO
                    var meias = clube.Jogadores.Where(x => x.Posicao == 6).OrderByDescending(x => x.H).Take(Convert.ToInt32(clube.Formacao.Substring(2, 1)));
                    foreach (var mei in meias)
                    {
                        escalacao = new Escalacao();
                        escalacao.Clube = clube;
                        escalacao.Posicao = 6;
                        escalacao.Jogador = mei;
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        //escalacao.HGol = escalacao.Jogador.H / 2 > 1 ? escalacao.Jogador.H / 2 : 1;
                        lstnova.Add(escalacao);
                    }

                    //ATACANTES
                    var atacantes = clube.Jogadores.Where(x => x.Posicao == 7).OrderByDescending(x => x.H).Take(Convert.ToInt32(clube.Formacao.Substring(3, 1)));
                    foreach (var ata in atacantes)
                    {
                        escalacao = new Escalacao();
                        escalacao.Clube = clube;
                        escalacao.Posicao = 7;
                        escalacao.Jogador = ata;
                        if (escalacao.Jogador.H >= 90)
                            escalacao.H = escalacao.Jogador.H + 20;
                        else if (escalacao.Jogador.H >= 80)
                            escalacao.H = escalacao.Jogador.H + 10;
                        else if (escalacao.Jogador.H >= 70)
                            escalacao.H = escalacao.Jogador.H + 5;
                        else
                            escalacao.H = escalacao.Jogador.H;

                        //escalacao.HGol = escalacao.Jogador.H;
                        lstnova.Add(escalacao);
                    }

                    var i = 0;
                    var lstescalacao = escalacaoRepository.GetAll().Where(x => x.Clube.Id == clube.Id);

                    foreach (var item in lstescalacao)
                    {
                        item.Posicao = lstnova[i].Posicao;
                        if (item.Jogador != null && clube.Usuario != null)
                        {
                            item.H = Util.Util.RetornaHabilidadePosicao(item.Jogador, item.Posicao);
                        }
                        else
                        {
                            item.Jogador = lstnova[i].Jogador;
                            item.H = Util.Util.RetornaHabilidadePosicao(item.Jogador, item.Posicao);
                        }
                        escalacaoRepository.SaveOrUpdate(item);

                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                var teste = ex.ToString();
            }

            return RedirectToAction("Index", "Engine");
        }

        #endregion

        #region GerarJogador

        [Transaction]
        public ActionResult GerarJogador(int id)
        {
            var clube = clubeRepository.Get(id);
            var divisao = clube.Divisao.Numero;
            var nomes = nomeRepository.GetAll();
            var rnd = new Random();

            var g = 2;
            var ld = 2;
            var le = 2;
            var z = 3;
            var v = 3;
            var mo = 3;
            var a = 3;

            for (int i = 0; i < 18; i++)
            {
                var jogador = new Jogador();
                jogador.Clube = clube;

                if (g > 0) {   jogador.Posicao = 1; g--; }
                else if (ld > 0) { jogador.Posicao = 2; ld--; }
                else if (z > 0) { jogador.Posicao = 3; z--; }
                else if (le > 0) { jogador.Posicao = 4; le--; }
                else if (v > 0) { jogador.Posicao = 5; v--; }
                else if (mo > 0) { jogador.Posicao = 6; mo--; }
                else if (a > 0) { jogador.Posicao = 7; a--; }

                var objnome = nomes.ElementAt(rnd.Next(0, nomes.Count()));
                if (!objnome.Comum) { objnome = nomes.ElementAt(rnd.Next(0, nomes.Count())); }

                jogador.Nome = objnome.NomeJogador.ToUpper();

                var h = Convert.ToInt32((60 / divisao) / 5) * 5;
                h = h + (5 * rnd.Next(1, 5)) - 10;
                jogador.HF = h > 0 ? h : 1;
                
                jogador.H = jogador.HF;
                jogador.Salario = ((jogador.HF / 2) / divisao) * 1000;
                jogador.Contrato = 2;

                jogadorRepository.SaveOrUpdate(jogador);
            }

            return RedirectToAction("DetalheClube", "Adm", new { id = id });
        }

        #endregion

        #region GerarStaff

        [Transaction]
        public ActionResult GerarStaff()
        {
            var nomes = nomeRepository.GetAll();
            var sobrenomes = sobrenomeRepository.GetAll();
            var rnd = new Random();
            
            for (int i = 0; i < 25; i++)
            {
                var staff = new Staff();                

                var nome = nomes.ElementAt(rnd.Next(0, nomes.Count()));
                var sobrenome = sobrenomes.ElementAt(rnd.Next(0, sobrenomes.Count()));

                staff.Nome = nome.NomeJogador.ToUpper() + " " + sobrenome.SobrenomeJogador.ToUpper();
                staff.Tipo = 1;
                staff.H = rnd.Next(1, 101);

                staffRepository.SaveOrUpdate(staff);
            }

            for (int i = 0; i < 25; i++)
            {
                var staff = new Staff();

                var nome = nomes.ElementAt(rnd.Next(0, nomes.Count()));
                var sobrenome = sobrenomes.ElementAt(rnd.Next(0, sobrenomes.Count()));

                staff.Nome = nome.NomeJogador.ToUpper() + " " + sobrenome.SobrenomeJogador.ToUpper();

                staff.H = rnd.Next(1, 101);
                staff.Tipo = 2;
                staffRepository.SaveOrUpdate(staff);
            }

            return RedirectToAction("GridStaff", "Adm");
        }

        #endregion

        public ActionResult PartidaTeste()
        {
            var partida = new Partida();
            var lstGols = new List<Gol>();

            partida.Clube1 = clubeRepository.Get(1);
            partida.Clube2 = clubeRepository.Get(2);
            partida.Mao = 1;
            partida.Rodada = 1;
            partida.Tipo = "DIVISAO";
            partida.Dia = 1;
            partida.Divisao = divisaoRepository.Get(1);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////ALTERAR ABAIXO
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////ALTERAR ABAIXO
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////ALTERAR ABAIXO

            var clube1 = clubeRepository.Get(partida.Clube1.Id);
            var clube2 = clubeRepository.Get(partida.Clube2.Id);
            var gol1 = 0;
            var gol2 = 0;

            var def1 = clube1.Escalacao.Where(x => x.Posicao == 1 || x.Posicao == 2 || x.Posicao == 3 || x.Posicao == 4 || x.Posicao == 5);
            var def2 = clube2.Escalacao.Where(x => x.Posicao == 1 || x.Posicao == 2 || x.Posicao == 3 || x.Posicao == 4 || x.Posicao == 5);
            var ata1 = clube1.Escalacao.Where(x => x.Posicao == 6 || x.Posicao == 7);
            var ata2 = clube2.Escalacao.Where(x => x.Posicao == 6 || x.Posicao == 7);

            var Defesa1 = Convert.ToInt32(def1.Sum(x => x.H) / def1.Count());
            var Defesa2 = Convert.ToInt32(def2.Sum(x => x.H) / def2.Count());
            var Ataque1 = Convert.ToInt32(ata1.Sum(x => x.H) / ata1.Count());
            var Ataque2 = Convert.ToInt32(ata2.Sum(x => x.H) / ata2.Count());

            var Diferenca = (Defesa1 - Defesa2) + (Ataque1 - Ataque2);

            var Prob1 = 55;
            var Prob2 = 45;
            var ProbEmpate = 0;

            if ((Diferenca * (-1)) > 30)
                ProbEmpate = 6;
            else if ((Diferenca * (-1)) > 20)
                ProbEmpate = 12;
            else if ((Diferenca * (-1)) > 10)
                ProbEmpate = 30;
            else if ((Diferenca * (-1)) >= 0)
                ProbEmpate = 40;

            Prob1 = (Prob1 + (Diferenca));
            Prob2 = (Prob2 - (Diferenca));

            Random rnd = new Random();

            var resultado = rnd.Next(0, (Prob1 + Prob2 + ProbEmpate));
            var vencedor = new Clube();

            var placar = rnd.Next(1, 101);

            if (resultado <= Prob1)
            {
                vencedor = clube1;
                if (clube1.Usuario != null)
                {
                    var usuario = clube1.Usuario;
                    usuario.Reputacao = usuario.Reputacao + 4 > 50 ? 50 : usuario.Reputacao + 4;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }
                else { clube1.ReputacaoAI = clube1.ReputacaoAI + 4 > 50 ? 50 : clube1.ReputacaoAI + 4; }

                if (clube2.Usuario != null)
                {
                    var usuario = clube2.Usuario;
                    usuario.Reputacao = usuario.Reputacao - 6 < 0 ? 0 : usuario.Reputacao - 6;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }
                else { clube2.ReputacaoAI = clube2.ReputacaoAI - 6 < 0 ? 0 : clube2.ReputacaoAI - 6; }

                decimal notaclube1 = 0;
                decimal notaclube2 = 0;
                
                //notas CLUBE 1
                foreach (var escalacao in clube1.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa2);

                        if ((jogador.H - Defesa2) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem + 15;

                    }
                    else
                    {
                        probjogarbem = 50 + (jogador.H - Ataque2);

                        if ((jogador.H - Ataque2) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem + 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {
                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube1 = notaclube1 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                //notas CLUBE 2
                foreach (var escalacao in clube2.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa1);

                        if ((jogador.H - Defesa1) > 20)
                            probjogarbem = probjogarbem - 40;
                        else
                            probjogarbem = probjogarbem - 15;

                    }
                    else
                    {
                        probjogarbem = 50 + (jogador.H - Ataque1);

                        if ((jogador.H - Ataque1) > 20)
                            probjogarbem = probjogarbem - 40;
                        else
                            probjogarbem = probjogarbem - 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {
                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube2 = notaclube2 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                notaclube1 = notaclube1 / 11;
                notaclube2 = notaclube2 / 11;

                var dif7 = 0;
                var dif6 = 0;
                var dif5 = 0;
                var dif4 = 0;
                var dif3 = 0;
                var dif2 = 0;

                if (notaclube1 >= notaclube2)
                {
                    if ((notaclube1 - notaclube2) > 3)
                    {
                        dif7 = 2; dif6 = 5; dif5 = 8; dif4 = 15; dif3 = 35; dif2 = 30; //dif1 = 5
                    }
                    else if ((notaclube1 - notaclube2) > 2)
                    {
                        dif6 = 2; dif5 = 5; dif4 = 8; dif3 = 30; dif2 = 40; //dif1 = 15
                    }
                    else if ((notaclube1 - notaclube2) > 1)
                    {
                        dif5 = 2; dif4 = 8; dif3 = 10; dif2 = 50; //dif1 = 30
                    }
                    else
                    {
                        dif4 = 2; dif3 = 8; dif2 = 35; //dif1 = 55
                    }
                }
                else
                {
                    if ((notaclube2 - notaclube1) > 3)
                    {
                        dif2 = 2; //dif1 = 98
                    }
                    else if ((notaclube2 - notaclube1) > 2)
                    {
                        dif2 = 10; //dif1 = 90
                    }
                    else if ((notaclube2 - notaclube1) > 1)
                    {
                        dif4 = 2; dif3 = 8; dif2 = 20; //dif1 = 70
                    }
                    else
                    {
                        dif4 = 2; dif3 = 8; dif2 = 40; //dif1 = 50
                    }
                }

                if (placar <= dif7)
                {
                    gol2 = rnd.Next(0, 2);
                    gol1 = gol2 + 7;
                }
                else if (placar <= (dif7 + dif6))
                {
                    gol2 = rnd.Next(0, 2);
                    gol1 = gol2 + 6;
                }
                else if (placar <= (dif7 + dif6 + dif5))
                {
                    gol2 = rnd.Next(0, 2);
                    gol1 = gol2 + 5;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4))
                {
                    gol2 = rnd.Next(0, 3);
                    gol1 = gol2 + 4;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3))
                {
                    gol2 = rnd.Next(0, 2);
                    gol1 = gol2 + 3;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                {
                    gol2 = rnd.Next(0, 3);
                    gol1 = gol2 + 2;
                }
                else if (placar > (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                {
                    gol2 = rnd.Next(0, 3);
                    gol1 = gol2 + 1;
                }
            }
            else if (resultado <= (Prob1 + ProbEmpate))
            {
                vencedor = null;
                if (clube1.Usuario != null)
                {
                    var usuario = clube1.Usuario;

                    usuario.Reputacao = usuario.Reputacao - 3 < 0 ? 0 : usuario.Reputacao - 3;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }
                else
                {
                    clube1.ReputacaoAI = clube1.ReputacaoAI - 3 < 0 ? 0 : clube1.ReputacaoAI - 3;
                }

                decimal notaclube1 = 0;
                decimal notaclube2 = 0;

                //notas CLUBE 1
                foreach (var escalacao in clube1.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa2);

                        if ((jogador.H - Defesa2) > 20)
                            probjogarbem = probjogarbem - 40;
                        else if ((jogador.H - Defesa2) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem - 15;

                    }
                    else
                    {
                        probjogarbem = 50 + (jogador.H - Ataque2);

                        if ((jogador.H - Ataque2) > 20)
                            probjogarbem = probjogarbem - 40;
                        else if ((jogador.H - Ataque2) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem - 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {
                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube1 = notaclube1 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                //notas CLUBE 2
                foreach (var escalacao in clube2.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa1);

                        if ((jogador.H - Defesa1) > 20)
                            probjogarbem = probjogarbem - 40;
                        else if ((jogador.H - Defesa1) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem - 15;

                    }
                    else
                    {
                        if ((jogador.H - Ataque1) > 20)
                            probjogarbem = probjogarbem - 40;
                        else if ((jogador.H - Ataque1) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem - 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {
                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube2 = notaclube2 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                if (placar < 35)
                {
                    gol2 = 0;
                    gol1 = 0;
                }
                else if (placar < 70)
                {
                    gol2 = 1;
                    gol1 = 1;
                }
                else if (placar < 90)
                {
                    gol2 = 2;
                    gol1 = 2;
                }
                else if (placar < 98)
                {
                    gol2 = 3;
                    gol1 = 3;
                }
                else if (placar >= 98)
                {
                    gol2 = 4;
                    gol1 = 4;
                }
            }
            else
            {
                vencedor = clube2;
                if (clube1.Usuario != null)
                {
                    var usuario = clube1.Usuario;

                    usuario.Reputacao = usuario.Reputacao - 8 < 0 ? 0 : usuario.Reputacao - 8;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }
                else
                {
                    clube1.ReputacaoAI = clube1.ReputacaoAI - 8 < 0 ? 0 : clube1.ReputacaoAI - 8;
                }

                if (clube2.Usuario != null)
                {
                    var usuario = clube2.Usuario;

                    usuario.Reputacao = usuario.Reputacao + 6 > 50 ? 50 : usuario.Reputacao + 6;
                    //usuarioRepository.SaveOrUpdate(usuario);
                }
                else
                {
                    clube2.ReputacaoAI = clube2.ReputacaoAI + 6 > 50 ? 50 : clube2.ReputacaoAI + 6;
                }

                decimal notaclube1 = 0;
                decimal notaclube2 = 0;

                //notas CLUBE 1
                foreach (var escalacao in clube1.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa2);

                        if ((jogador.H - Defesa2) > 20)
                            probjogarbem = probjogarbem - 40;
                        else
                            probjogarbem = probjogarbem - 15;
                    }
                    else
                    {
                        probjogarbem = 50 - (jogador.H + Ataque2);

                        if ((jogador.H - Ataque1) > 20)
                            probjogarbem = probjogarbem - 40;
                        else
                            probjogarbem = probjogarbem - 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {
                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube1 = notaclube1 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                //notas CLUBE 2
                foreach (var escalacao in clube2.Escalacao)
                {
                    var jogador = escalacao.Jogador;
                    var probjogarbem = 50;

                    if (jogador.Posicao == 1 || jogador.Posicao == 2 || jogador.Posicao == 3 || jogador.Posicao == 4 || jogador.Posicao == 5)
                    {
                        probjogarbem = 50 + (jogador.H - Defesa1);

                        if ((jogador.H - Defesa1) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem + 15;

                    }
                    else
                    {
                        probjogarbem = 50 + (jogador.H - Ataque1);

                        if ((jogador.H - Ataque1) < -20)
                            probjogarbem = probjogarbem + 40;
                        else
                            probjogarbem = probjogarbem + 15;
                    }

                    probjogarbem = probjogarbem > 90 ? 90 : probjogarbem < 10 ? 10 : probjogarbem;

                    double nota = 0;
                    var rndnota = rnd.Next(1, 101);
                    var nota1 = Convert.ToInt32(((double)probjogarbem / 100) * 25);
                    var nota2 = Convert.ToInt32(((double)probjogarbem / 100) * 20);
                    var nota3 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota4 = Convert.ToInt32(((double)probjogarbem / 100) * 15);
                    var nota5 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota6 = Convert.ToInt32(((double)probjogarbem / 100) * 10);
                    var nota7 = Convert.ToInt32(((double)probjogarbem / 100) * 5);

                    if (rnd.Next(1, 101) <= probjogarbem)
                    {


                        if (rndnota < nota1)
                            nota = 5.5;
                        else if (rndnota < (nota1 + nota2))
                            nota = 6.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 7.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 7.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 8.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 8.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 9.0;
                        else
                            nota = (double)(jogador.H > 55 ? jogador.H / 10 : 6.0);
                    }
                    else
                    {
                        if (rndnota < nota1)
                            nota = 5.0;
                        else if (rndnota < (nota1 + nota2))
                            nota = 4.5;
                        else if (rndnota < (nota1 + nota2 + nota3))
                            nota = 4.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4))
                            nota = 3.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5))
                            nota = 2.5;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6))
                            nota = 2.0;
                        else if (rndnota < (nota1 + nota2 + nota3 + nota4 + nota5 + nota6 + nota7))
                            nota = 1.5;
                        else
                            nota = (double)(jogador.H > 30 ? jogador.H / 10 : 3.0);
                    }

                    notaclube2 = notaclube2 + Convert.ToDecimal(nota);

                    jogador.Jogos = jogador.Jogos + 1;
                    jogador.NotaTotal = jogador.NotaTotal + Convert.ToDecimal(nota);
                    jogador.NotaUlt = Convert.ToDecimal(nota);

                    jogadorRepository.SaveOrUpdate(jogador);
                }

                notaclube1 = notaclube1 / 11;
                notaclube2 = notaclube2 / 11;

                var dif7 = 0;
                var dif6 = 0;
                var dif5 = 0;
                var dif4 = 0;
                var dif3 = 0;
                var dif2 = 0;

                if (notaclube2 >= notaclube1)
                {
                    if ((notaclube2 - notaclube1) > 3)
                    {
                        dif7 = 2; dif6 = 5; dif5 = 8; dif4 = 15; dif3 = 35; dif2 = 30; //dif1 = 5
                    }
                    else if ((notaclube2 - notaclube1) > 2)
                    {
                        dif6 = 2; dif5 = 5; dif4 = 8; dif3 = 30; dif2 = 40; //dif1 = 15
                    }
                    else if ((notaclube2 - notaclube1) > 1)
                    {
                        dif5 = 2; dif4 = 8; dif3 = 10; dif2 = 50; //dif1 = 30
                    }
                    else
                    {
                        dif4 = 2; dif3 = 8; dif2 = 35; //dif1 = 55
                    }
                }
                else
                {
                    if ((notaclube1 - notaclube2) > 3)
                    {
                        dif2 = 2; //dif1 = 98
                    }
                    else if ((notaclube1 - notaclube2) > 2)
                    {
                        dif2 = 10; //dif1 = 90
                    }
                    else if ((notaclube1 - notaclube2) > 1)
                    {
                        dif4 = 2; dif3 = 8; dif2 = 20; //dif1 = 70
                    }
                    else
                    {
                        dif4 = 2; dif3 = 8; dif2 = 40; //dif1 = 50
                    }
                }

                if (placar <= dif7)
                {
                    gol1 = rnd.Next(0, 2);
                    gol2 = gol1 + 7;
                }
                else if (placar <= (dif7 + dif6))
                {
                    gol1 = rnd.Next(0, 2);
                    gol2 = gol1 + 6;
                }
                else if (placar <= (dif7 + dif6 + dif5))
                {
                    gol1 = rnd.Next(0, 2);
                    gol2 = gol1 + 5;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4))
                {
                    gol1 = rnd.Next(0, 3);
                    gol2 = gol1 + 4;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3))
                {
                    gol1 = rnd.Next(0, 2);
                    gol2 = gol1 + 3;
                }
                else if (placar <= (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                {
                    gol1 = rnd.Next(0, 3);
                    gol2 = gol1 + 2;
                }
                else if (placar > (dif7 + dif6 + dif5 + dif4 + dif3 + dif2))
                {
                    gol1 = rnd.Next(0, 3);
                    gol2 = gol1 + 1;
                }
            }

            if (partida.Tipo == "TACA" && partida.Mao == 2)
            {
                var partida1mao = partidaRepository.GetAll().Where(x => x.Rodada == partida.Rodada && x.Mao == 1 && x.Clube1.Id == partida.Clube2.Id && x.Clube2.Id == partida.Clube1.Id).FirstOrDefault();

                var teste1 = "1 MAO: " + partida1mao.Clube1.Nome + " " + partida1mao.Gol1 + " x " + partida1mao.Gol2 + " " + partida1mao.Clube2.Nome;
                var teste2 = "2 MAO: " + partida.Clube1.Nome + " " + gol1 + " x " + gol2 + " " + partida.Clube2.Nome;

                var totalgol1 = gol1 + partida1mao.Gol2;
                var totalgol2 = gol2 + partida1mao.Gol1;

                if (totalgol1 > totalgol2)
                    vencedor = partida.Clube1;
                else if (totalgol2 > totalgol1)
                    vencedor = partida.Clube2;
                else
                {
                    if (partida1mao.Gol2 > gol2)
                        vencedor = partida.Clube1;
                    else if (partida1mao.Gol2 < gol2)
                        vencedor = partida.Clube2;
                    else if (rnd.Next(1, 3) == 1)
                    {
                        vencedor = partida.Clube1;
                        var penal = rnd.Next(0, 3);
                        if (penal == 0)
                            partida.Penalti = "5 x 4";
                        else if (penal == 1)
                            partida.Penalti = "4 x 3";
                        else
                            partida.Penalti = "3 x 2";
                    }
                    else
                    {
                        vencedor = partida.Clube2;
                        var penal = rnd.Next(0, 3);
                        if (penal == 0)
                            partida.Penalti = "4 x 5";
                        else if (penal == 1)
                            partida.Penalti = "3 x 4";
                        else
                            partida.Penalti = "2 x 3";
                    }
                }

            }

            rnd = new Random();

            if (gol1 > 0)
            {
                var totalgols = clube1.Escalacao.OrderByDescending(x => x.Jogador.NotaUlt).Take(5).Sum(x => x.HGol) + 1;
                for (int i = 1; i <= gol1; i++)
                {
                    var gol = new Gol();
                    gol.Clube = clube1;
                    gol.Minuto = rnd.Next(1, 94);
                    gol.Partida = partida;
                    var contagem = 0;
                    var goleador = rnd.Next(1, totalgols);
                    foreach (var jog in clube1.Escalacao.OrderByDescending(x => x.Jogador.NotaUlt).ThenByDescending(x => x.Posicao).Take(5))
                    {
                        contagem = contagem + jog.HGol;
                        if (goleador <= contagem)
                        {
                            gol.Jogador = jog.Jogador;
                            break;
                        }
                    }
                    lstGols.Add(gol); /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR
                    //golRepository.SaveOrUpdate(gol);
                }
            }

            rnd = new Random();
            rnd.Next(1, 100);

            var listgol2 = new List<Gol>();
            if (gol2 > 0)
            {
                var totalgols = clube2.Escalacao.OrderByDescending(x => x.Jogador.NotaUlt).Take(5).Sum(x => x.HGol) + 1;
                for (int i = 1; i <= gol2; i++)
                {
                    var gol = new Gol();
                    gol.Clube = clube2;
                    gol.Minuto = rnd.Next(1, 94);
                    gol.Partida = partida;
                    var contagem = 0;
                    var goleador = rnd.Next(1, totalgols);
                    foreach (var jog in clube2.Escalacao.OrderByDescending(x => x.Jogador.NotaUlt).ThenByDescending(x => x.Posicao).Take(5))
                    {
                        contagem = contagem + jog.HGol;
                        if (goleador <= contagem)
                        {
                            gol.Jogador = jog.Jogador;
                            break;
                        }
                    }
                    lstGols.Add(gol); /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR /// RETIRAR
                    //golRepository.SaveOrUpdate(gol);
                }
            }

            //publico 
            var publico = 0;
            publico = clube1.Socios * 5;
            publico = publico - ((publico / 12) * (divisaotabelaRepository.GetAll().Where(x => x.Clube.Id == clube1.Id).FirstOrDefault().Posicao - 1));
            if (clube1.Ingresso > 35)
                publico = (publico / 100) * 30;
            else if (clube1.Ingresso > 25)
                publico = (publico / 100) * 50;

            publico = rnd.Next((publico - 3000), publico);
            if (publico > clube1.Estadio)
                publico = clube1.Estadio;
            else if (publico <= 0)
                publico = rnd.Next(500, 2000);

            clube1.Dinheiro = clube1.Dinheiro + (publico * clube1.Ingresso);
            //clubeRepository.SaveOrUpdate(clube1);
            //clubeRepository.SaveOrUpdate(clube2);

            partida.Gol1 = gol1;
            partida.Gol2 = gol2;
            if (partida.Tipo == "TACA" && partida.Mao == 1)
                partida.Vencedor = null;
            else
                partida.Vencedor = vencedor;
            partida.Realizada = true;
            partida.Publico = publico;
            //partidaRepository.SaveOrUpdate(partida);

            ViewBag.Escalacao1 = clube1.Escalacao.ToList();
            ViewBag.Escalacao2 = clube2.Escalacao.ToList();
            partida.Gols = lstGols;

            return View(partida);
        }
    }
}
