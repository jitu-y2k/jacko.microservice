

using System.ComponentModel.DataAnnotations;

namespace Jacko.Services.EmailAPI.Models.Dto
{
    public class CartHeaderDto
    {
        
        public int CartHeaderId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }        
        public double Discount { get; set; }        
        public double CartTotal { get; set; }


     
        public string? Name { get; set; }
     
        public string? Phone { get; set; }
        [Required]
        public string? Email { get; set; }
    }
}

