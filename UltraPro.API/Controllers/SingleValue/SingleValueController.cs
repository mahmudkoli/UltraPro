using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;
using UltraPro.Services.Interfaces;
using UltraPro.API.Controllers.Common;
using UltraPro.API.Models;
using UltraPro.API.Models.IdentityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UltraPro.API.Controllers.SingleValue
{
    [Authorize]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/single-value")]
    public class SingleValueController : BaseController
    {
        private ISingleValueService _singleValueService;
        private readonly IMapper _mapper;

        public SingleValueController(
            ISingleValueService singleValueService,
            IMapper mapper)
        {
            this._singleValueService = singleValueService;
            this._mapper = mapper;
        }

        [HttpGet]
        [Authorize(Permissions.SingleValue.ListView)]
        public async Task<IActionResult> Get([FromQuery] SingleValueQuery query)
        {
            var result = await _singleValueService.GetAllAsync(query);
            var queryResult = _mapper.Map<QueryResult<SingleValueDetail>, QueryResult<SingleValueDetailModel>>(result);
            return OkResult(queryResult);
        }

        [HttpGet("{id}")]
        [Authorize(Permissions.SingleValue.DetailsView)]
        public async Task<IActionResult> Get(int id)
        {
            var singleValue = await _singleValueService.GetByIdAsync(id);
            var result = _mapper.Map<SingleValueDetail, SingleValueDetailModel>(singleValue);
            return OkResult(result);
        }

        [HttpPost]
        [Authorize(Permissions.SingleValue.Create)]
        public async Task<IActionResult> Create([FromBody] SaveSingleValueDetailModel model)
        {
            var singleValue = _mapper.Map<SaveSingleValueDetailModel, SingleValueDetail>(model);
            var result = await _singleValueService.AddAsync(singleValue);
            return OkResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Permissions.SingleValue.Edit)]
        public async Task<IActionResult> Update(int id, [FromBody] SaveSingleValueDetailModel model)
        {
            var singleValue = _mapper.Map<SaveSingleValueDetailModel, SingleValueDetail>(model);
            await _singleValueService.UpdateAsync(singleValue);
            return OkResult(true);
        }

        [HttpDelete("{id}")]
        [Authorize(Permissions.SingleValue.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            await _singleValueService.DeleteAsync(id);
            return OkResult(true);
        }

        [HttpPost("activeInactive/{id}")]
        public async Task<IActionResult> ActiveInactive(int id)
        {
            await _singleValueService.ActiveInactiveAsync(id);
            return OkResult(true);
        }

        [HttpGet("select/{typeCode}")]
        public async Task<IActionResult> GetSelect(string typeCode)
        {
            var result = await _singleValueService.GetAllForSelectAsync(typeCode);
            return OkResult(result);
        }

        [HttpGet("select-type")]
        public async Task<IActionResult> GetTypeSelect()
        {
            var result = await _singleValueService.GetAllTypeForSelectAsync();
            return OkResult(result);
        }
    }
}
