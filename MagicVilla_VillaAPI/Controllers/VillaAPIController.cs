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
		public VillaAPIController(ILoggerCustom logger, ApplicationDbContext db)
		{
			_logger = logger;
			_db = db;
		}


		// get all villa
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			_logger.Log("Getting all villas", "");
			return Ok(_db.Villas.ToList());
		}

		//get one villa
		[HttpGet("{id:int}", Name ="GetVilla")] // methana me dena Name ea denne function name eka widihata.
		[ProducesResponseType(StatusCodes.Status200OK)] 
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		//[ProducesResponseType(200, Type = typeof(VillaDTO))] // mehema Type eka dammama function eke type eka dnna one na. mehema dmmama swager eke  expect krana data pennanwa
		//[ProducesResponseType(404)]
		//[ProducesResponseType(400)]
		public ActionResult<VillaDTO> GetVilla(int id)
		{
			if (id == 0)
			{
				_logger.Log("Get Villa Error with Id " + id, "error");
				return BadRequest();
			}
			var villa = _db.Villas.FirstOrDefault(villa => villa.Id == id);
			if (villa == null)
			{
				_logger.Log("Not Found Id :"+ id, "error");
				return NotFound();
			}
			return Ok(villa);
		}

		//crate a villa
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<VillaDTO> CreateVilla([FromBody]VillaCreateDTO villaCreateDTO)
		{
			//Custom Error msg pennanne mehema
			if(_db.Villas.FirstOrDefault(villa=>villa.Name.ToLower() == villaCreateDTO.Name.ToLower())!=null)
			{
				ModelState.AddModelError("CustomError", "Villa Already Exhist");
				return BadRequest(ModelState);
			}
			if(villaCreateDTO == null)
			{
				return BadRequest(villaCreateDTO);
			}
			//if (villaCreateDTO.Id > 0)
			//{
			//	return StatusCode(StatusCodes.Status500InternalServerError);
			//}
			Villa model = new Villa()
			{
				//Id = villaCreateDTO.Id,
				Name = villaCreateDTO.Name,
				Details = villaCreateDTO.Details,
				ImageUrl = villaCreateDTO.ImageUrl,
				Occupancy = villaCreateDTO.Occupancy,
				Rate = villaCreateDTO.Rate,
				Sqft = villaCreateDTO.Sqft,
				Amenity = villaCreateDTO.Amenity,
			};

			_db.Villas.Add(model);
			_db.SaveChanges();

			//return Ok(villaDTO); // meka mehema return krnna pukuwanaulak na. eth itawda hodai pahala widihata use krna eka.
			return CreatedAtRoute("GetVilla", new { id = model.Id }, model); // mehema denakota wenama function ekakata return krnna puluwan. methandi kra thiynne Gellvilla kiyna ekata return krla thiynne
		}

		// delete Villa
		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult DeleteVilla(int id)
		{
			if(id == 0)
			{
				return BadRequest();
			}
			var villa = _db.Villas.FirstOrDefault(villa=>villa.Id == id);
			if(villa == null)
			{
				return NotFound();
			}
			_db.Villas.Remove(villa);
			_db.SaveChanges();
			return Ok(villa);
			//return NoContent(); //meka mehema dannath puluwan
		}

		//update All villa recodes
		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdateVilla(int id, [FromBody] VillaUpdateDTO villaUpdateDTO)
		{
			if (villaUpdateDTO == null || id != villaUpdateDTO.Id)
			{
				return BadRequest(new { message = "Invalid data." });
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			Villa model = new()
			{
				Id = villaUpdateDTO.Id,
				Name = villaUpdateDTO.Name,
				Details = villaUpdateDTO.Details,
				ImageUrl = villaUpdateDTO.ImageUrl,
				Occupancy = villaUpdateDTO.Occupancy,
				Rate = villaUpdateDTO.Rate,
				Sqft = villaUpdateDTO.Sqft,
				Amenity = villaUpdateDTO.Amenity,
			};

			_db.Villas.Update(model);
			_db.SaveChanges();
			return Ok(model);
		}


		//update spesific data
		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
		{
			if(patchDTO == null || id == 0)
			{
				return BadRequest();
			}
			var villa = _db.Villas.AsNoTracking().FirstOrDefault(villa=>villa.Id == id);

			VillaUpdateDTO villaDTO = new ()
			{
				Id = villa.Id,
				Name = villa.Name,
				Details = villa.Details,
				ImageUrl = villa.ImageUrl,
				Occupancy = villa.Occupancy,
				Rate = villa.Rate,
				Sqft = villa.Sqft,
				Amenity = villa.Amenity,
			};
			if (villa == null)
			{
				return NotFound();
			}
			patchDTO.ApplyTo(villaDTO, ModelState);
			Villa model = new Villa()
			{
				Id = villaDTO.Id,
				Name = villaDTO.Name,
				Details = villaDTO.Details,
				ImageUrl = villaDTO.ImageUrl,
				Occupancy = villaDTO.Occupancy,
				Rate = villaDTO.Rate,
				Sqft = villaDTO.Sqft,
				Amenity = villaDTO.Amenity,
			};
			_db.Villas.Update(model);
			_db.SaveChanges();
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(villa);
		}
	}
}
