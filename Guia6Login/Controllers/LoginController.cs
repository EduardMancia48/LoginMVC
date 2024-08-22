using Azure.Core;
using Guia6Login.Models;
using Guia6Login.Models.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Guia6Login.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IFilesService _filesService;
        private readonly UsuarioContext _userContext;

        public LoginController(IUsuarioService usuarioService, IFilesService filesService, UsuarioContext userContext)
        {
            _usuarioService = usuarioService;
            _filesService = filesService;
            _userContext = userContext;
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario, IFormFile Imagen)
        {
            Stream image = Imagen.OpenReadStream();
            string urlImagen = await _filesService.SubirArchivo(image, Imagen.FileName);
            usuario.Clave = Utilidades.EncriptarClave(usuario.Clave);
            Usuario usuarioCreado = await _usuarioService.SaveUsuario(usuario);
            if (usuarioCreado.Id > 0)
            {
                return RedirectToAction("IniciarSesion", "Login");
            }
            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string clave)
        {
            Usuario usuarioEncontrado = await _usuarioService.GetUsuario(correo, Utilidades.EncriptarClave(clave));
            if (usuarioEncontrado == null)
            {
                ViewData["Mensaje"] = "El usuario no ha sido encontrado";
                return View();
            }

            // Verifica que los valores no sean null antes de crear los Claims
            string nombreUsuario = usuarioEncontrado.NombreUsuario ?? "Nombre no disponible";
            string fotoPerfil = usuarioEncontrado.URLFotoPerfil ?? string.Empty;

            List<Claim> claims = new List<Claim>() {
        new Claim(ClaimTypes.Name, nombreUsuario),
        new Claim("FotoPerfil", fotoPerfil),
    };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            return RedirectToAction("Index", "Home");
        }

    }

}
