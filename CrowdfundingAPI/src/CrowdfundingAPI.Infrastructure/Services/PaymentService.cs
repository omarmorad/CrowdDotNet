using CrowdfundingAPI.Application.Interfaces;
using CrowdfundingAPI.Application.Models;

namespace CrowdfundingAPI.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, string paymentMethod)
    {
        // Simulate payment processing with random success/failure
        await Task.Delay(1000); // Simulate API call delay
        
        var random = new Random();
        var isSuccess = random.NextDouble() > 0.1; // 90% success rate
        
        return new PaymentResult
        {
            IsSuccess = isSuccess,
            TransactionId = Guid.NewGuid().ToString(),
            Message = isSuccess ? "Payment processed successfully" : "Payment failed - insufficient funds"
        };
    }

    public async Task<bool> RefundPaymentAsync(string transactionId)
    {
        // Simulate refund processing
        await Task.Delay(500);
        return true; // Always successful for demo
    }
}