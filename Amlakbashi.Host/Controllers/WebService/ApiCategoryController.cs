using Amlakbashi.Application.Services.Category.Interfaces;
using Amlakbashi.Core.Common.Utilities;
using Amlakbashi.Core.Entities;
using Amlakbashi.Host.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amlakbashi.Host.Controllers.WebService
{
    [ApiController]
    [Route("api/category")]
    public class ApiCategoryController : ApiBaseController
    {
        private readonly ICategoryAppService categoryService;
        public ApiCategoryController(ICategoryAppService categoryService)
        {
            this.categoryService = categoryService;
        }
    }
}
