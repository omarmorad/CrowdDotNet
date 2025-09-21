using CrowdfundingAPI.Application.Models;

namespace CrowdfundingAPI.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(decimal amount, string paymentMethod);
    Task<bool> RefundPaymentAsync(string transactionId);
}