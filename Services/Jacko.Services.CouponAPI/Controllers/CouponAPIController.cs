using System.Collections.Generic;
using AutoMapper;
using Jacko.Services.CouponAPI.Data;
using Jacko.Services.CouponAPI.Models;
using Jacko.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Jacko.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [Authorize]
    public class CouponAPIController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private ResponseDto _response;

        public CouponAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        // GET: api/values
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> couponsList = _appDbContext.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(couponsList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            
            return _response;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon coupon = _appDbContext.Coupons.First(c => c.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon coupon = _appDbContext.Coupons.First(c => c.CouponCode.ToLower() == code.ToLower());
                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // POST api/values
        [HttpPost]
        public ResponseDto Post([FromBody]CouponDto couponDto)
        {
            try
            {
                Coupon newCoupon = _mapper.Map<Coupon>(couponDto);
                _appDbContext.Coupons.Add(newCoupon);
                _appDbContext.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(newCoupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // PUT api/values/5
        [HttpPut]
        public ResponseDto Put([FromBody]CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _appDbContext.Coupons.Update(coupon);
                _appDbContext.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon coupon = _appDbContext.Coupons.First(c => c.CouponId == id);
                _appDbContext.Coupons.Remove(coupon);
                _appDbContext.SaveChanges();             
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}

