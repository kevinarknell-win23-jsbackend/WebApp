using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using WebApp.ViewModels;


namespace WebApp.Controllers;
[Authorize]
public class AccountController : Controller
{
    private readonly HttpClient _httpClient;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("AuthService");
    }

    public async Task<IActionResult> Details()
    {
        var response = await _httpClient.GetAsync("/api/account/details");
        if (response.IsSuccessStatusCode)
        {
            var viewModel = await response.Content.ReadFromJsonAsync<AccountDetailsViewModel>();
            return View(viewModel);
        }
        else
        {
            return RedirectToAction("SignIn", "Auth");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateBasicInfo(AccountDetailsViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/account/updateBasicInfo", model.Basic);
        TempData["StatusMessage"] = response.IsSuccessStatusCode ? "Updated basic information successfully" : "Unable to save basic information";
        return RedirectToAction("Details");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAddressInfo(AccountDetailsViewModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/account/updateAddressInfo", model.Address);
        TempData["StatusMessage"] = response.IsSuccessStatusCode ? "Updated address information successfully" : "Unable to save address information";
        return RedirectToAction("Details");
    }

    [HttpPost]
    public async Task<IActionResult> UploadProfileImage(IFormFile file)
    {
        if (file != null && file.Length > 0)
        {
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            content.Add(new StreamContent(fileStream), "file", file.FileName);
            var response = await _httpClient.PostAsync("/api/account/uploadProfileImage", content);
            TempData["StatusMessage"] = response.IsSuccessStatusCode ? "Profile image uploaded successfully" : "Unable to upload profile image.";
        }
        else
        {
            TempData["StatusMessage"] = "No file selected.";
        }
        return RedirectToAction("Details");
    }
}
