using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proyecto.Modules.Autenticacion.Entities;

namespace Proyecto.Controllers;

public sealed class AccountController(SignInManager<ApplicationUser> signInManager) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null) =>
        View(new LoginViewModel { ReturnUrl = returnUrl });

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var resultado = await signInManager.PasswordSignInAsync(
            modelo.Usuario,
            modelo.Contrasena,
            isPersistent: false,
            lockoutOnFailure: false);
        if (resultado.Succeeded)
        {
            return Url.IsLocalUrl(modelo.ReturnUrl)
                ? LocalRedirect(modelo.ReturnUrl!)
                : RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
        return View(modelo);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Ingrese su usuario.")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese su contraseña.")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
