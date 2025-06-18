using EntranceManager.Models;
using EntranceManager.Models.Mappers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace EntranceManager.Controllers.Mvc
{
    public class AuthMvcController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthMvcController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7145/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(result);

                // Съхраняваме токена в cookie
                Response.Cookies.Append("auth_token", authResponse.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return RedirectToAction("Index", "ApartmentsMvc");
            }

            ViewBag.Error = "Invalid login.";
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth_token");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var json = JsonConvert.SerializeObject(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://localhost:7145/api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                // Registration successful, redirect to login page
                return RedirectToAction("Login");
            }

            // Read error message from API response (e.g., "User already exists.")
            var errorMessage = await response.Content.ReadAsStringAsync();
            ViewBag.Error = errorMessage;

            return View(registerDto);
        }
    }
}