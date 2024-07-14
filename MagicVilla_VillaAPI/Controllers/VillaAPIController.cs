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
		[HttpGet("{id:int}")]
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
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
		{
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

			return Ok(villaDTO);
		} 
	}
}
