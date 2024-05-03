using System;
using System.Text;
using Jacko.Services.EmailAPI.Data;
using Jacko.Services.EmailAPI.Models;
using Jacko.Services.EmailAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Jacko.Services.EmailAPI.Service
{
	public class EmailService: IEmailService
	{
        private DbContextOptions<AppDbContext> _dbContextOptions;

        public EmailService(DbContextOptions<AppDbContext> dbContextOptions)
		{
            _dbContextOptions = dbContextOptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested");
            message.AppendLine("<br/>Total :" + cartDto.CartHeader.CartTotal);
            message.AppendLine("<br/>");
            message.AppendLine("<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                message.AppendLine("<li>");
                message.AppendLine(item.Product.Name + " x " + item.Count);
                message.AppendLine("<li>");
            };
            message.AppendLine("</ul>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);

        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {

                EmailLogger emailLogger = new()
                {
                    Message = message,
                    Email = email,
                    EmailSent = DateTime.UtcNow
                };

                await using var dbContext = new AppDbContext(_dbContextOptions);
                dbContext.EmailLoggers.Add(emailLogger);
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}

