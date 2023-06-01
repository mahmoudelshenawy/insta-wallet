using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.DataTableViewModels;
using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using OfficeOpenXml;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;

namespace AdminLte.Controllers
{
    [Authorize(AuthenticationSchemes = "Admin")]
    [Route("admin/deposits")]

    public class DepositController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;

        public DepositController(ApplicationDbContext context, IMapper mapper, IDistributedCache distributedCache)
        {
            _context = context;
            _mapper = mapper;
            _distributedCache = distributedCache;
        }

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            ViewBag.item = "transactions";
            ViewBag.subItem = "deposits";

            var deposits = await _context.Deposits.ToListAsync();

            var redisCustomerList = await _distributedCache.GetAsync("deposits");
            if (redisCustomerList != null)
            {
                var deop = Encoding.UTF8.GetString(redisCustomerList);
                var res = JsonSerializer.Deserialize<List<Deposit>>(deop);
                return Ok(res);
            }

            var serializedDeposits = JsonSerializer.Serialize(deposits);
            var redisSerializedDeposits = Encoding.UTF8.GetBytes(serializedDeposits);
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)).SetSlidingExpiration(TimeSpan.FromMinutes(2));
            await _distributedCache.SetAsync("deposits", redisSerializedDeposits, options);

            return View();
        }
        [HttpPost("datatable")]
        public async Task<IActionResult> GetDepositsDataTable()
        {
            var length = Request.Form["length"].FirstOrDefault();
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();

            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();


            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;



            IQueryable<Deposit> deposits = _context.Deposits.Include(d => d.User).Include(d => d.Currency)
                .Where(deposit => string.IsNullOrEmpty(searchValue) ? true :
            (deposit.User.FirstName.Contains(searchValue) ||
            deposit.User.LastName.Contains(searchValue) ||
            deposit.User.Email.Contains(searchValue))
            );

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                deposits = deposits.OrderBy(string.Concat(sortColumn, " ", sortColumnDirection));

            var data = deposits.Skip(skip).Take(pageSize)
                .ToList();
            var dataTable = _mapper.Map<List<DepositsDataTable>>(data);


            recordsTotal = deposits.Count();

            var dataJson = new { data = dataTable, recordsTotal, recordsFiltered = recordsTotal };

            return Ok(dataJson);
        }
        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportToCsv()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Id,date,user,amount,currency,status");
            var deposits = _context.Deposits.Include(x => x.User).Include(x => x.Currency).ToList();

            if (deposits.Any())
            {
                foreach (var deposit in deposits)
                {
                    builder.AppendLine($"{deposit.Id},{deposit.CreatedAt.Date.ToString()},{deposit.User.FirstName},{deposit.Amount.ToString()},{deposit.Currency.Code},{deposit.Status.ToString()}");
                }
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "deposits.csv");
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var workSheet = workbook.Worksheets.Add("Deposits");
                var currentRow = 1;
                workSheet.Cell(currentRow, 1).Value = "ID";
                workSheet.Cell(currentRow, 2).Value = "Date";
                workSheet.Cell(currentRow, 3).Value = "User";
                workSheet.Cell(currentRow, 4).Value = "Amount";
                workSheet.Cell(currentRow, 5).Value = "Currency";
                workSheet.Cell(currentRow, 6).Value = "Status";

                var deposits = _context.Deposits.Include(x => x.User).Include(x => x.Currency).ToList();

                foreach (var deposit in deposits)
                {
                    currentRow++;
                    workSheet.Cell(currentRow, 1).Value = deposit.Id;
                    workSheet.Cell(currentRow, 2).Value = deposit.CreatedAt.Date;
                    workSheet.Cell(currentRow, 3).Value = deposit.User.FirstName + " " + deposit.User.LastName;
                    workSheet.Cell(currentRow, 4).Value = deposit.Amount;
                    workSheet.Cell(currentRow, 5).Value = deposit.Currency.Code;
                    workSheet.Cell(currentRow, 6).Value = deposit.Status.ToString();
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "deposits.xlsx");
                }

            }
        }

        [HttpPost("import-excel")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            var depositDataTables = new List<DepositImport>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var excel = new ExcelPackage(stream))
                {
                    var workSheet = excel.Workbook.Worksheets[0];
                    var rowCount = workSheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        depositDataTables.Add(new DepositImport
                        {
                            Id = workSheet.Cells[row, 1].Value.ToString(),
                            CreatedAt = workSheet.Cells[row, 2].Value.ToString(),
                            User = workSheet.Cells[row, 3].Value.ToString(),
                            Amount = workSheet.Cells[row, 4].Value.ToString(),
                            Currency = workSheet.Cells[row, 5].Value.ToString(),
                            Status = workSheet.Cells[row, 6].Value.ToString()
                        });
                    }
                }
            }
            return Ok(depositDataTables);
        }

    }
}
