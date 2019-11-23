using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EmployeesApp.Models;
using EmployeesApp.Contracts;
using System;
using Microsoft.AspNetCore.DataProtection;
using System.Threading;

namespace EmployeesApp.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _repo;
        private readonly IDataProtector _protector;
        //private readonly IDataProtector _protectorTest; TESTING ANOTHER IDATAPROTECTOR INSTANCE

        public EmployeesController(IEmployeeRepository repo, IDataProtectionProvider provider)
        {
            _repo = repo;
            _protector = provider.CreateProtector("EmployeesApp.EmployeesController");
            //_protectorTest = provider.CreateProtector("TestProtector");
        }

        public IActionResult Index()
        {
            var employees = _repo.GetAll();

            foreach (var emp in employees)
            {
                var stringId = emp.Id.ToString();
                emp.EncryptedId = _protector.Protect(stringId);
            }

            //var testData = _protectorTest.Protect("Test");
            //var unprotectedTest = _protector.Unprotect(testData);

            //var timeLimitedProtector = _protector.ToTimeLimitedDataProtector();
            //var timeLimitedData = timeLimitedProtector.Protect("Test timed protector", lifetime: TimeSpan.FromSeconds(2));

            //just to test that this action works as long as life - time hasn't expired
            //var timedUnprotectedData = timeLimitedProtector.Unprotect(timeLimitedData);

            //Thread.Sleep(3000);

            //var anotherTimedUnprotectTry = timeLimitedProtector.Unprotect(timeLimitedData);

            return View(employees);
        }

        public IActionResult Details(string id)
        {
            var guid_id = Guid.Parse(_protector.Unprotect(id));
            var employee = _repo.GetEmployee(guid_id);

            return View(employee);
        }
    }
}
