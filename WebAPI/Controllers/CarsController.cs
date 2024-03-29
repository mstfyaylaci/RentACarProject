﻿using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        //------------------------------------------Post--------------------------------------

        [HttpPost("add")]
        public IActionResult Add(Car car)
        {
            var result = _carService.Add(car);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("update")]
        public IActionResult Update(Car car)
        {
            var result = _carService.Update(car);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("delete")]
        public IActionResult Delete(Car car)
        {
            var result = _carService.Delete(car);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        //------------------------------------Get--------------------------------------------------

        //[HttpGet("getall")] 
        //public IActionResult GetAll() 
        //{
        //    var result=_carService.GetAll();

        //    if (result.Success)
        //    {
        //        return Ok(result);
        //    }
        //    return BadRequest(result);
        //}

        //[HttpGet("getbyid")]
        //public IActionResult GetById(int id)
        //{
        //    var result = _carService.GetById(id);

        //    if (result.Success)
        //    {
        //        return Ok(result);
        //    }
        //    return BadRequest(result);
        //}


        //[HttpGet("getcarsbybrandid")]
        //public IActionResult GetCarsByBrandId(int id)
        //{
        //    var result = _carService.GetCarsByBrandId(id);

        //    if (result.Success)
        //    {
        //        return Ok(result);
        //    }
        //    return BadRequest(result);
        //}

        //[HttpGet("getcarsbycolorid")]
        //public IActionResult GetCarsByColorId(int id)
        //{
        //    var result = _carService.GetCarsByColorId(id);

        //    if (result.Success)
        //    {
        //        return Ok(result);
        //    }
        //    return BadRequest(result);
        //}

        //-------------------------------------------CarDetailDto Get-----------------

        [HttpGet("getcardetails")]
        public IActionResult GetCarDetails()
        {
            var result = _carService.GetCarDetails();

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getcardetailsid")]
        public IActionResult GetCarDetailsId(int carId)
        {
            var result = _carService.GetCarDetailsId(carId);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getcarbybrandiddetails")]
        public IActionResult GetCarByBrandIdDetails(int brandId)
        {
            var result = _carService.GetCarByBrandIdDetails(brandId);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getcarbycoloriddetails")]
        public IActionResult GetCarByColorIdDetails(int colorId)
        {
            var result = _carService.GetCarByColorIdDetails(colorId);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getcarbybrandandcolor")]
        public IActionResult GetCarByBrandAndColor(int brandId,int colorId)
        {
            var result = _carService.GetCarByBrandAndColor(brandId,colorId);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }



    }
}
