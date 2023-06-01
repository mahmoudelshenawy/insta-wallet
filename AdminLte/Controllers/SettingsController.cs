using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Data.Enums;
using AdminLte.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AdminLte.Controllers
{
    [Authorize(AuthenticationSchemes ="Admin")]
    [Route("admin")]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("settings/{tab?}")]
        public async Task<IActionResult> Index(string tab)
        {
            ViewBag.subItem = "Settings";

            if (!string.IsNullOrEmpty(tab))
            {
                tab = tab.ToLower();
            }
            else
            {
                tab = "general";
            }


            SettingsViewModel viewModel = await GetSettingsData(tab);


            return View(viewModel);
        }
        [HttpPost("settings/{tab?}")]
        public async Task<IActionResult> AddGeneralSettingData(GeneralSettingsViewModel settingsViewModel, string tab)
        {
            ViewBag.subItem = "Settings";
            if (ModelState.IsValid)
            {
                var generalSettings = await _context.Settings.Where(s => s.Type == tab).ToListAsync();
                foreach (var item in generalSettings)
                {
                    if (settingsViewModel.GetType().GetProperty(item.Name) != null)
                    {
                        if (settingsViewModel.GetType().GetProperty(item.Name).GetValue(settingsViewModel) != item.Value)
                        {
                            item.Value = (string?)settingsViewModel.GetType().GetProperty(item.Name).GetValue(settingsViewModel);

                            _context.Settings.Update(item);
                        }
                    }

                }


                await _context.SaveChangesAsync();
            }

            SettingsViewModel viewModel = await GetSettingsData(tab);

            return View("Index", viewModel);
        }
        private async Task<SettingsViewModel> GetSettingsData(string tab)
        {
            var settingViewModel = new SettingsViewModel()
            {
                Tab = tab,
            };
            switch (tab)
            {
                case "general":
                    var generalSettings = await _context.Settings.Where(s => s.Type == tab).ToListAsync();
                    var generalViewModel = new GeneralSettingsViewModel()
                    {
                        Tab = tab
                    };
                    if (generalSettings.Count() > 0)
                    {
                        foreach (var item in generalSettings)
                        {
                            if (generalViewModel.GetType().GetProperty(item.Name) != null)
                                generalViewModel.GetType().GetProperty(item.Name).SetValue(generalViewModel, item.Value);
                        }
                    }
                    settingViewModel.GeneralSettings = generalViewModel;
                    break;
                default:
                    break;
            }

            return settingViewModel;
        }

    }

    public enum SettingsEnum
    {
        GENERAL,
        REFERRAL,
        ADMINSECURITYSETTINGS,
        SOCIALLINKS,
        VIRTUALCARDSETTINGS,
        THEMES,
        SMAILSETTINGS,
    }
}

//foreach (PropertyInfo prop in settingsViewModel.GetType().GetProperties())
//{
//    if ((string)prop.GetValue(settingsViewModel) != null)
//    {
//        var attr = prop.Name;

//        var newGeneralSetting = await _context.Settings.Where(s => s.Name == attr).FirstOrDefaultAsync();

//        if (newGeneralSetting == null)
//        {
//            var setting = new Settings()
//            {
//                Name = attr,
//                Value = (string)prop.GetValue(settingsViewModel),
//                Type = tab
//            };
//            _context.Settings.Add(setting);
//        }
//        else
//        {
//            newGeneralSetting.Value = (string)prop.GetValue(settingsViewModel);

//            _context.Settings.Update(newGeneralSetting);
//        }
//    }
//}