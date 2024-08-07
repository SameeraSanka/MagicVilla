﻿using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logger;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
	//[Route("api/VillaAPIController")] mehema dannath puluwan
	[Route("api/[controller]")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		//me widihata contoller ekath pass krnne defalt eken CMD ekata information denkota.
		//private readonly ILogger<VillaAPIController> _logger;
		//      public VillaAPIController(ILogger<VillaAPIController> logger )
		//      {
		//          _logger = logger;
		//      }

		private readonly ApplicationDbContext _db;
		private readonly ILoggerCustom _logger;
		private readonly IMapper _mapper;
		public VillaAPIController(ILoggerCustom logger, ApplicationDbContext db, IMapper mapper)
		{
			_logger = logger;
			_db = db;
			_mapper = mapper;
		}


		// get all villa
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
		{
			_logger.Log("Getting all villas", "");
			IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
			return Ok(_mapper.Map<List<VillaDTO>>(villaList));
		}

		//get one villa
		[HttpGet("{id:int}", Name ="GetVilla")] // methana me dena Name ea denne function name eka widihata.
		[ProducesResponseType(StatusCodes.Status200OK)] 
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<VillaDTO>> GetVilla(int id)
		{
			if (id == 0)
			{
				_logger.Log("Get Villa Error with Id " + id, "error");
				return BadRequest();
			}
			var villa = await _db.Villas.FirstOrDefaultAsync(villa => villa.Id == id);
			if (villa == null)
			{
				_logger.Log("Not Found Id :"+ id, "error");
				return NotFound();
			}
			return Ok(_mapper.Map<VillaDTO>(villa));
		}

		//crate a villa
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO villaCreateDTO)
		{
			//Custom Error msg pennanne mehema
			if(await _db.Villas.FirstOrDefaultAsync(villa=>villa.Name.ToLower() == villaCreateDTO.Name.ToLower())!=null)
			{
				ModelState.AddModelError("CustomError", "Villa Already Exhist");
				return BadRequest(ModelState);
			}
			if(villaCreateDTO == null)
			{
				return BadRequest(villaCreateDTO);
			}

			Villa model = _mapper.Map<Villa>(villaCreateDTO);

			await _db.Villas.AddAsync(model);
			await _db.SaveChangesAsync();

			return CreatedAtRoute("GetVilla", new { id = model.Id }, model); // mehema denakota wenama function ekakata return krnna puluwan. methandi kra thiynne Gellvilla kiyna ekata return krla thiynne
		}

		// delete Villa
		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> DeleteVilla(int id)
		{
			if(id == 0)
			{
				return BadRequest();
			}
			var villa =  await _db.Villas.FirstOrDefaultAsync(villa => villa.Id == id);
			if(villa == null)
			{
				return NotFound();
			}
			 _db.Villas.Remove(villa);
			await _db.SaveChangesAsync();
			return Ok(villa);
			//return NoContent(); //meka mehema dannath puluwan
		}

		//update All villa recodes
		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaUpdateDTO)
		{
			if (villaUpdateDTO == null || id != villaUpdateDTO.Id)
			{
				return BadRequest(new { message = "Invalid data." });
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			Villa model = _mapper.Map<Villa>(villaUpdateDTO);
			
			_db.Villas.Update(model);
			await _db.SaveChangesAsync();
			return Ok(model);
		}


		//update spesific data
		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{
			if(patchDTO == null || id == 0)
			{
				return BadRequest();
			}
			var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(villa=>villa.Id == id);

			VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

			if (villa == null)
			{
				return NotFound();
			}
			patchDTO.ApplyTo(villaDTO, ModelState);

			Villa model = _mapper.Map<Villa>(villaDTO);

			_db.Villas.Update(model);
			await _db.SaveChangesAsync();
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(villa);
		}
	}
}
