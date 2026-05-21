using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Education.API.Data;
using Education.API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Education.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WalletController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemCard([FromBody] RedeemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.SecureCode))
                return BadRequest(new { Message = "Code cannot be empty." });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var student = await _context.Students.FindAsync(request.StudentID);
                if (student == null) 
                    return NotFound(new { Message = "Student profile not found." });

                if (student.DeviceID != null && student.DeviceID != request.CurrentDeviceID)
                {
                    return StatusCode(403, new { Message = "Security Lock: This account belongs to another device." });
                }

                if (student.DeviceID == null)
                {
                    student.DeviceID = request.CurrentDeviceID;
                }

                var card = await _context.ScratchCards.FirstOrDefaultAsync(c => c.SecureCode == request.SecureCode);
                if (card == null) 
                    return NotFound(new { Message = "This scratch card code is invalid." });

                if (card.IsUsed) 
                    return BadRequest(new { Message = $"This card was already used at {card.UsedAt}." });

                card.IsUsed = true;
                card.UsedByStudentID = student.StudentID;
                card.UsedAt = DateTime.UtcNow;

                student.WalletBalance += card.Value;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); 

                return Ok(new { 
                    Message = "Card redeemed successfully!", 
                    AmountAdded = card.Value, 
                    NewBalance = student.WalletBalance 
                });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(); 
                return StatusCode(500, new { Message = "An internal database error occurred." });
            }
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateCards([FromBody] GenerateCardsRequest request)
        {
            if (request.Count <= 0 || request.CardValue <= 0)
                return BadRequest(new { Message = "Invalid parameters." });

            var generatedCodes = new List<string>();

            for (int i = 0; i < request.Count; i++)
            {
                string uniqueCode = $"{request.Prefix.ToUpper()}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                
                var card = new ScratchCard
                {
                    SecureCode = uniqueCode,
                    Value = request.CardValue,
                    IsUsed = false
                };

                _context.ScratchCards.Add(card);
                generatedCodes.Add(uniqueCode);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = $"{request.Count} cards generated successfully.", Codes = generatedCodes });
        }
    }

    public class RedeemRequest
    {
        public int StudentID { get; set; }
        public string SecureCode { get; set; } = string.Empty;
        public string CurrentDeviceID { get; set; } = string.Empty;
    }

    public class GenerateCardsRequest
    {
        public string Prefix { get; set; } = "EDU"; 
        public int Count { get; set; } 
        public decimal CardValue { get; set; } 
    }
}