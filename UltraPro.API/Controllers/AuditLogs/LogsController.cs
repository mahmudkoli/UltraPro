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
using UltraPro.API.Models.Logs;

namespace UltraPro.API.Controllers.SingleValue
{
    [Authorize]
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ApiVersion("1")]
    [Route("api/v{v:apiVersion}/logs")]
    public class LogsController : BaseController
    {
        private IRequestResponseLogService _requestResponseLogService;
        private IAuditLogService _auditLogService;
        private readonly IMapper _mapper;

        public LogsController(
            IRequestResponseLogService requestResponseLogService,
            IAuditLogService auditLogService,
            IMapper mapper)
        {
            this._requestResponseLogService = requestResponseLogService;
            this._auditLogService = auditLogService;
            this._mapper = mapper;
        }

        [HttpGet("RequestResponseLogs")]
        [Authorize(Permissions.RequestResponseLogs.ListView)]
        public async Task<IActionResult> GetRequestResponseLogs([FromQuery] RequestResponseLogQuery query)
        {
            var result = await _requestResponseLogService.GetAllAsync(query);
            var queryResult = _mapper.Map<QueryResult<RequestResponseLog>, QueryResult<RequestResponseLogModel>>(result);
            return OkResult(queryResult);
        }

        [HttpGet("RequestResponseLog/{id}")]
        [Authorize(Permissions.RequestResponseLogs.DetailsView)]
        public async Task<IActionResult> GetRequestResponseLog(Guid id)
        {
            var requestResponseLog = await _requestResponseLogService.GetByIdAsync(id);
            var result = _mapper.Map<RequestResponseLog, RequestResponseLogModel>(requestResponseLog);
            return OkResult(result);
        }

        [HttpGet("AuditLogs")]
        [Authorize(Permissions.AuditLogs.ListView)]
        public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogQuery query)
        {
            var result = await _auditLogService.GetAllAsync(query);
            var queryResult = _mapper.Map<QueryResult<AuditLog>, QueryResult<AuditLogModel>>(result);
            return OkResult(queryResult);
        }

        [HttpGet("AuditLog/{id}")]
        [Authorize(Permissions.AuditLogs.DetailsView)]
        public async Task<IActionResult> GetAuditLog(Guid id)
        {
            var auditLog = await _auditLogService.GetByIdAsync(id);
            var result = _mapper.Map<AuditLog, AuditLogModel>(auditLog);
            return OkResult(result);
        }
    }
}
