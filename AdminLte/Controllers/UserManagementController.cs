using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Models;
using AdminLte.Repositories;
using AdminLte.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
namespace AdminLte.Controllers
{
    [Route("admin")]
    public class UserManagementController : Controller
    {
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ApplicationDbContext _context;
        public UserManagementController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,
            IAuthenticationRepository authenticationRepository, ApplicationDbContext context)
        {
            _RoleManager = roleManager;
            _UserManager = userManager;
            _authenticationRepository = authenticationRepository;
            _context = context;
        }

        [HttpGet("roles")]
        public async Task<IActionResult> RolesList()
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Roles";

            var roles = await _RoleManager.Roles.ToListAsync();

            return View(roles);
        }
        [HttpPost("roles")]
        public async Task<IActionResult> AddRole(RoleModel roleModel)
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Roles";

            if (ModelState.IsValid)
            {
                var isExist = await _RoleManager.RoleExistsAsync(roleModel.Name);
                if (isExist)
                {
                    ModelState.AddModelError("", "Role Already Exists");
                }
                else
                {
                    await _RoleManager.CreateAsync(new IdentityRole()
                    {
                        Name = roleModel.Name,
                        NormalizedName = roleModel.Name.ToUpper()
                    });

                    roleModel.Name = "";
                }
            }
            var roles = await _RoleManager.Roles.ToListAsync();

            return View("RolesList", roles);
        }

        [HttpGet("users")]
        public async Task<IActionResult> UsersList()
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Users";
            var Users = await _UserManager.Users.ToListAsync();
            List<UserViewModel> users = new List<UserViewModel>();

            foreach (var user in Users)
            {
                users.Add(new UserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = _UserManager.GetRolesAsync(user).Result.ToList()
                });
            }
            return View(users);
        }

        [HttpGet("dummy/list")]
        public async Task<IActionResult> DummyData()
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Dummy";
            return View();
        }

        [HttpPost("dummies")]
        public async Task<IActionResult> ViewDummyData()
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Dummy";

            var length = Request.Form["length"].FirstOrDefault();
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();

            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();


            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;



            IQueryable<Dummy> dummies = _context.Dummies.Where(m => string.IsNullOrEmpty(searchValue) ? true :
            (m.FirstName.Contains(searchValue) ||
            m.LastName.Contains(searchValue) ||
            m.Email.Contains(searchValue) ||
            m.Contact.Contains(searchValue))
            );

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                dummies = dummies.OrderBy(string.Concat(sortColumn, " ", sortColumnDirection));

            var data = dummies.Skip(skip).Take(pageSize).ToList();


            recordsTotal = dummies.Count();

            var dataJson = new { data, recordsTotal, recordsFiltered = recordsTotal };

            return Ok(dataJson);
        }

        [HttpGet("AddRolesToUser")]
        public async Task<IActionResult> AddRolesToUser(string userId)
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Users";

            var user = await _UserManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var roles = await _RoleManager.Roles.ToListAsync();
            var UserRoles = new UserRolesViewModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(role => new RoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = _UserManager.IsInRoleAsync(user, role.Name).Result
                }).ToList()
            };


            return View(UserRoles);
        }
        [HttpPost("AddRolesToUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRolesToUser(UserRolesViewModel model)
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Users";
            var user = await _UserManager.FindByIdAsync(model.UserId);

            if (user == null)
                return NotFound();

            var userRoles = await _UserManager.GetRolesAsync(user);

            var roles = await _RoleManager.Roles.ToListAsync();

            foreach (var role in model.Roles)
            {
                if (userRoles.Any(r => r == role.RoleName) && !role.IsSelected)
                {
                    //remove from user
                    await _UserManager.RemoveFromRoleAsync(user, role.RoleName);
                }
                if (!userRoles.Any(r => r == role.RoleName) && role.IsSelected)
                {
                    //add to user
                    await _UserManager.AddToRoleAsync(user, role.RoleName);
                }
            }
            ViewBag.IsSuccess = true;

            return RedirectToAction(nameof(UsersList));
        }

        [HttpGet("users/Add")]
        public async Task<IActionResult> AddUser()
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Users";
            var roles = await _RoleManager.Roles.Select(r => new RoleViewModel
            {
                RoleId = r.Id,
                RoleName = r.Name
            }).ToListAsync();

            var model = new AddUserViewModel
            {
                Roles = roles
            };
            return View(model);
        }
        [HttpPost("users/Add")]
        public async Task<IActionResult> AddUser(AddUserViewModel userViewModel)
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Users";
            var roles = await _RoleManager.Roles.Select(r => new RoleViewModel
            {
                RoleId = r.Id,
                RoleName = r.Name
            }).ToListAsync();

            var model = new AddUserViewModel
            {
                Roles = roles
            };

            if (ModelState.IsValid)
            {
                if (await _UserManager.FindByEmailAsync(userViewModel.Email) != null)
                {
                    ModelState.AddModelError("Email", "Email is already exist");
                    return View(model);
                }

                if (await _UserManager.FindByNameAsync(userViewModel.UserName) != null)
                {
                    ModelState.AddModelError("UserName", "username is already exist");
                    return View(model);
                }

                ApplicationUser user = new ApplicationUser
                {
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                    UserName = userViewModel.UserName,
                    Email = userViewModel.Email,
                    NormalizedEmail = userViewModel.Email.ToUpper(),
                };
                var result = await _UserManager.CreateAsync(user, userViewModel.Password);

                if (result.Succeeded)
                {
                    await _UserManager.AddToRolesAsync(user, userViewModel.Roles.Where(r => r.IsSelected).Select(r => r.RoleName));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

            }
            ViewBag.IsSuccess = true;
            return View(model);
        }
        [HttpGet("users/Edit")]
        public async Task<IActionResult> EditUser(string userId)
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Users";
            var user = await _UserManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var model = new EditUserViewModel
            {
                Id = userId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return View(model);
        }
        [HttpPost("users/Edit")]
        public async Task<IActionResult> EditUser(EditUserViewModel editUser)
        {
            ViewBag.item = "UserRole";
            ViewBag.subItem = "Users";
            var user = await _UserManager.FindByIdAsync(editUser.Id);

            if (user == null)
                return NotFound();

            var emailExists = await _UserManager.FindByEmailAsync(editUser.Email);
            if (emailExists != null && user.Email != editUser.Email)
            {
                ModelState.AddModelError("Email", "Email is Already taken");
            }
            var userNameExists = await _UserManager.FindByNameAsync(editUser.UserName);
            if (userNameExists != null && user.UserName != editUser.UserName)
            {
                ModelState.AddModelError("UserName", "UserName is Already taken");
            }

            user.FirstName = editUser.FirstName;
            user.LastName = editUser.LastName;
            user.Email = editUser.Email;
            user.UserName = editUser.UserName;

            var result = await _UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                ViewBag.IsSuccess = true;
            }
            var model = new EditUserViewModel
            {
                Id = editUser.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return View(model);
        }


    }
}
