using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	//[Route("api/VillaAPIController")] mehema dannath puluwan
	[Route("api/[controller]")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		// get all villa
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			return Ok(VillaStore.villaList);
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
				return BadRequest();
			}
			var villa = VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);
			if (villa == null)
			{
				return NotFound();
			}
			return Ok(villa);
		}

		//crate a villa
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
		{
			//Custom Error msg pennanne mehema
			if(VillaStore.villaList.FirstOrDefault(villa=>villa.Name.ToLower() == villaDTO.Name.ToLower())!=null)
			{
				ModelState.AddModelError("CustomError", "Villa Already Exhist");
				return BadRequest(ModelState);
			}
			if(villaDTO == null)
			{
				return BadRequest(villaDTO);
			}
			if (villaDTO.Id > 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			villaDTO.Id = VillaStore.villaList.OrderByDescending(villa=>villa.Id).FirstOrDefault().Id +1;
			VillaStore.villaList.Add(villaDTO);

			//return Ok(villaDTO); // meka mehema return krnna pukuwanaulak na. eth itawda hodai pahala widihata use krna eka.
			return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO); // mehema denakota wenama function ekakata return krnna puluwan. methandi kra thiynne Gellvilla kiyna ekata return krla thiynne
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
			var villa = VillaStore.villaList.FirstOrDefault(villa=>villa.Id == id);
			if(villa == null)
			{
				return NotFound();
			}
			VillaStore.villaList.Remove(villa);
			return Ok(villa);
			//return NoContent(); //meka mehema dannath puluwan
		}


	}
}
