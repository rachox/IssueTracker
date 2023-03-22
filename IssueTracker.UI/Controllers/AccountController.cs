﻿using IssueTracker.Infrastructure.Identity;
using IssueTracker.UI.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.UI.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel login, [FromServices] SignInManager<ApplicationUser> signInManager)
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsDeveloper([FromServices] SignInManager<ApplicationUser> signInManager)
        {
            var result = await signInManager.PasswordSignInAsync("dev@test.com", "Pass123", false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsManager([FromServices] SignInManager<ApplicationUser> signInManager)
        {
            var result = await signInManager.PasswordSignInAsync("manager@test.com", "Pass123", false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoginAsAdmin([FromServices] SignInManager<ApplicationUser> signInManager)
        {
            var result = await signInManager.PasswordSignInAsync("admin@test.com", "Pass123", false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}