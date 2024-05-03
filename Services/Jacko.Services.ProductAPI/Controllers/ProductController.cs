using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Jacko.Services.ProductAPI.Data;
using Jacko.Services.ProductAPI.Models;
using Jacko.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Jacko.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private ResponseDto _responseDto;

        public ProductController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }

        // GET: api/values
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                var productList = _dbContext.Products.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<ProductDto>>(productList);
            }
            catch(Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [Authorize]
        public ResponseDto Get(int id)
        {
            try
            {
                var product = _dbContext.Products.First(p=> p.ProductId==id);
                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        // POST api/values
        [HttpPost]
        public ResponseDto Post([FromBody]ProductDto model)
        {
            try
            {
                var product = _mapper.Map<Product>(model);
                _dbContext.Products.Add(product);
                _dbContext.SaveChanges();

                if (model.Image != null)
                {
                    string fileName = product.ProductId + Path.GetExtension(model.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    FileInfo file = new FileInfo(directoryLocation);

                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (FileStream fileStream = new (filePathDirectory, FileMode.Create))
                    {
                        model.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }

                _dbContext.Products.Update(product);
                _dbContext.SaveChanges();

                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        // PUT api/values
        [HttpPut]
        public ResponseDto Put([FromBody]ProductDto model)
        {
            try
            {
                var product = _mapper.Map<Product>(model);

                if (model.Image != null)
                {
                    string fileName = product.ProductId + Path.GetExtension(model.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    FileInfo file = new FileInfo(directoryLocation);

                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (FileStream fileStream = new(filePathDirectory, FileMode.Create))
                    {
                        model.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }

                _dbContext.Products.Update(product);
                _dbContext.SaveChanges();
                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public ResponseDto Delete(int id)
        {
            try
            {
                var product = _dbContext.Products.First(p => p.ProductId == id);

                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                _dbContext.Products.Remove(product);
                _dbContext.SaveChanges();                
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }
    }
}

