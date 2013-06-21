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

    public class StaffController : ControllerCustom
    {

        private readonly IUsuarioRepository usuarioRepository;
        private readonly IAuthenticationService authenticationService;
        private readonly INHibernateRepository<Clube> clubeRepository;
        private readonly IClubeRepository clubeQueryRepository;
        private readonly INHibernateRepository<Staff> staffRepository;
        private readonly INHibernateRepository<Controle> controleRepository;
        private readonly INHibernateRepository<JogadorOferta> staffofertaRepository;
        private readonly INHibernateRepository<Divisao> divisaoRepository;
        private readonly IPartidaRepository partidaRepository;
        private readonly INHibernateRepository<Gol> golRepository;
        private readonly INHibernateRepository<DivisaoTabela> divisaotabelaRepository;
        private readonly INHibernateRepository<JogadorPedido> staffpedidoRepository;
        private readonly INHibernateRepository<UsuarioOferta> usuarioofertaRepository;
        private readonly INHibernateRepository<Noticia> noticiaRepository;

        public StaffController(IUsuarioRepository usuarioRepository,
            IAuthenticationService authenticationService,
            INHibernateRepository<Clube> clubeRepository,
            IClubeRepository clubeQueryRepository,
            INHibernateRepository<Staff> staffRepository,
            INHibernateRepository<Controle> controleRepository,
            INHibernateRepository<JogadorOferta> staffofertaRepository,
            INHibernateRepository<Divisao> divisaoRepository,
            IPartidaRepository partidaRepository,
            INHibernateRepository<Gol> golRepository,
            INHibernateRepository<DivisaoTabela> divisaotabelaRepository,
            INHibernateRepository<JogadorPedido> staffpedidoRepository,
            INHibernateRepository<UsuarioOferta> usuarioofertaRepository,
            INHibernateRepository<Noticia> noticiaRepository)
        {
            this.usuarioRepository = usuarioRepository;
            this.authenticationService = authenticationService;
            this.clubeRepository = clubeRepository;
            this.clubeQueryRepository = clubeQueryRepository;
            this.staffRepository = staffRepository;
            this.controleRepository = controleRepository;
            this.staffofertaRepository = staffofertaRepository;
            this.divisaoRepository = divisaoRepository;
            this.partidaRepository = partidaRepository;
            this.golRepository = golRepository;
            this.divisaotabelaRepository = divisaotabelaRepository;
            this.staffpedidoRepository = staffpedidoRepository;
            this.usuarioofertaRepository = usuarioofertaRepository;
            this.noticiaRepository = noticiaRepository;
        }

        [Authorize]
        public ActionResult Index(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            ViewBag.Clube = usuario.Clube;
            ViewBag.Controle = controleRepository.GetAll().FirstOrDefault();

            var staff = staffRepository.Get(id);

            return View(staff);
        }

        [Authorize]
        public ActionResult Busca(int? p)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            var stafffiltro = new StaffFiltroView();

            if (Session["StaffFiltroView"] != null)
            {
                stafffiltro = (StaffFiltroView)Session["StaffFiltroView"];

                var lstStaff = staffRepository.GetAll()
                            .Where(x => x.Usuario == null);

                if (stafffiltro.Tipo != 0)
                    lstStaff = lstStaff.Where(x => x.Tipo == stafffiltro.Tipo);

                lstStaff = lstStaff.OrderBy(x => x.Nome);

                var skip = 0;

                if (p.HasValue)
                {
                    if (p.Value > 1)
                        skip = 40 * (p.Value - 1);
                }

                lstStaff = lstStaff.Skip(skip).Take(40);

                ViewBag.Resultado = lstStaff.ToList();
            }
            else
            {
                ViewBag.Resultado = new List<Staff>();
            }

            ViewBag.Clube = usuario.Clube;
            ViewBag.Page = p.HasValue ? p.Value : 1;

            return View(stafffiltro);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Busca(FormCollection form)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            var stafffiltro = new StaffFiltroView();

            TryUpdateModel(stafffiltro, form);

            Session["StaffFiltroView"] = stafffiltro;

            var lstStaff = staffRepository.GetAll()
                            .Where(x => x.Usuario == null);

            if (stafffiltro.Tipo != 0)
                lstStaff = lstStaff.Where(x => x.Tipo == stafffiltro.Tipo);

            lstStaff = lstStaff.OrderBy(x => x.Nome).Take(40);            

            ViewBag.Resultado = lstStaff.ToList();
            ViewBag.Clube = usuario.Clube;
            ViewBag.Page = 1;

            return View(stafffiltro);
        }

        [Authorize]
        public ActionResult StaffOferta(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();

            var staff = staffRepository.Get(id);

            if (usuario.Staffs.Where(x => x.Tipo == staff.Tipo).Count() > 0)
            {
                TempData["MsgErro"] = "Você já possui um " + Util.Util.RetornaStaffTipo(staff.Tipo) + ". Para contratar outro, primeiro encerre o contrato com o atual.";
                return RedirectToAction("Index", "Staff", new { id = staff.Id });
            }
            else if (staff.Usuario != null)
            {
                TempData["MsgErro"] = "Este profissional já está vinculado a uma comissão técnica.";
                return RedirectToAction("Index", "Staff", new { id = staff.Id });
            }

            return View(staff);
        }

        [Authorize]
        [HttpPost]
        [Transaction]
        public ActionResult StaffOferta(int idstaff, FormCollection form)
        {
            var controle = controleRepository.GetAll().FirstOrDefault();

            var usuario = authenticationService.GetUserAuthenticated();

            var staff = staffRepository.Get(idstaff);

            if (usuario.Staffs.Where(x => x.Tipo == staff.Tipo).Count() > 0)
            {
                TempData["MsgErro"] = "Você já possui um " + Util.Util.RetornaStaffTipo(staff.Tipo) + ". Para contratar outro, primeiro encerre o contrato com o atual.";
                return RedirectToAction("Index", "Staff", new { id = staff.Id });
            }
            else if (staff.Usuario != null)
            {
                TempData["MsgErro"] = "Este profissional já está vinculado a uma comissão técnica.";
                return RedirectToAction("Index", "Staff", new { id = staff.Id });
            }

            TryUpdateModel(staff, form);

            var nota = Convert.ToInt32(Math.Ceiling((decimal)staff.H / 20));
            var reputacao = Convert.ToInt32(Math.Ceiling((decimal)usuario.ReputacaoGeral / 20));
            var salariomin = 3000 * (nota + (5 - reputacao));

            if (staff.Salario >= salariomin)
            {
                staff.Usuario = usuario;
                staffRepository.SaveOrUpdate(staff);

                TempData["MsgOk"] = staff.Nome + " aceitou sua proposta e já ingressou na sua comissão técnica.";
                return RedirectToAction("Index", "Staff", new { id = staff.Id });
            }
            else
            {
                TempData["MsgErro"] = staff.Nome + " rejeitou sua proposta.";
                return RedirectToAction("Index", "Staff", new { id = staff.Id });
            }
        }   
       
        [Authorize]
        public ActionResult StaffRenovar(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var staff = staffRepository.Get(id);

            if (staff.Usuario == null || staff.Usuario.Id != usuario.Id)
                return RedirectToAction("Index", "Conta");

            return View(staff);
        }

        [Authorize]
        [Transaction]
        [HttpPost]
        public ActionResult StaffRenovar(FormCollection form)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var staff = staffRepository.Get(Convert.ToInt32(form["Jogador"]));

            if (staff.Usuario != null || staff.Usuario.Id != usuario.Id)
                return RedirectToAction("Index", "Conta");

            var salario = Convert.ToDecimal(form["Salario"]);
            var contrato = Convert.ToInt32(form["Contrato"]);

            if (staff != null)
            {
                var salariomin = (staff.Salario / 100) * 120;

                if (salario >= salariomin)
                {
                    staff.Salario = salario;
                    staff.Contrato = contrato;
                    staffRepository.SaveOrUpdate(staff);

                    TempData["MsgOk"] = staff.Nome + " aceitou sua proposta e renovou por " + contrato + " temporada(s)!";
                    return RedirectToAction("Index", "Staff", new { id = staff.Id });
                }
                else
                {
                    TempData["MsgErro"] = staff.Nome + " rejeitou sua proposta de renovação!";
                }
            }
            return View(staff);
        }

        [Authorize]
        public ActionResult StaffDispensar(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var staff = staffRepository.Get(id);

            if (staff.Usuario == null || staff.Usuario.Id != usuario.Id)
                return RedirectToAction("Index", "Conta");

            return View(staff);
        }        
        
        [Authorize]
        [Transaction]
        public ActionResult StaffDispensarConfirma(int id)
        {
            var usuario = authenticationService.GetUserAuthenticated();
            var staff = staffRepository.Get(id);

            if (staff.Usuario == null || staff.Usuario.Id != usuario.Id)
                return RedirectToAction("Index", "Conta");

            staff.Usuario = null;
            staff.Salario = 0;
            staff.Contrato = 0;
            staffRepository.SaveOrUpdate(staff);

            TempData["MsgOk"] = staff.Nome + " foi dispensado com sucesso!";
            return RedirectToAction("Index", "Staff", new { id = id });
        }          
    }
}
