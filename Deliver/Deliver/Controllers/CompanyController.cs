﻿using Deliver.CustomAttribute;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Models.Request.Company;
using Models.Response._Core;
using Models.Response.Company;
using Services.Interface;

namespace Deliver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly AppSettings _appSettings;

        public CompanyController(ICompanyService companyService, IOptions<AppSettings> options)
        {
            _companyService = companyService;
            _appSettings = options.Value;
        }

        [HttpPost("Create")]
        [Authorize("admin")]
        public async Task<BaseResponse<CompanyResponse>> Create(CreateCompanyRequest request)
        {
            return await _companyService.Create(request);
        }
    }
}
