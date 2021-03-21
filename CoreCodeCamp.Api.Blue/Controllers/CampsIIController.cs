using AutoMapper;
using CoreCodeCamp.Api.Blue.Data;
using CoreCodeCamp.Api.Blue.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreCodeCamp.Api.Blue.Controllers
{
    [Route("api/camps")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CampsIIController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsIIController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _campRepository = campRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        [HttpGet("{includeTalks:bool?}")]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                var results = await _campRepository.GetAllCampsAsync(includeTalks);
                var result = new
                {
                    Count = results.Count(),
                    Results = _mapper.Map<CampModel[]>(results)
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Database Failure{ex.Message}");
            }
        }
        [HttpGet("{moniker}")]
        public async Task<IActionResult> Get(string moniker)
        {
            try
            {
                var results = await _campRepository.GetCampAsync(moniker);
                return results == null ? (ActionResult)NotFound() : Ok(_mapper.Map<CampModel>(results));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _campRepository.GetAllCampsByEventDate(theDate, includeTalks);
                return !results.Any() ? (ActionResult)NotFound() : Ok(_mapper.Map<CampModel[]>(results));
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Database Failure{ex.Message}");
            }
        }
        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var existing = await _campRepository.GetCampAsync(model.Moniker);
                if (existing != null)
                {
                    return BadRequest("Moniker in use");
                }
                var location = _linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Couldn't use current moniker");
                }
                var camp = _mapper.Map<Camp>(model);
                _campRepository.Add(camp);
                return await _campRepository.SaveChangesAsync() ? Created(location, _mapper.Map<CampModel>(camp)) : (ActionResult)BadRequest();
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Database Failure{ex.Message}");
            }
        }
        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await _campRepository.GetCampAsync(moniker) ;
                if (oldCamp == null)
                {
                    return NotFound($"Couldn't find camp with moniker {moniker}");
                }
                _mapper.Map(model, oldCamp);

                return await _campRepository.SaveChangesAsync() ? Ok(_mapper.Map<CampModel>(oldCamp)) : (ActionResult)BadRequest();
                
            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Database Failure{ex.Message}");
            }
        }
        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = await _campRepository.GetCampAsync(moniker);
                if (oldCamp == null)
                {
                    return NotFound($"Couldn't find camp with moniker {moniker}");
                }
                _campRepository.Delete(oldCamp);

                return await _campRepository.SaveChangesAsync() ? Ok() : (ActionResult)BadRequest();

            }
            catch (Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Database Failure{ex.Message}");
            }
        }
    }
}
