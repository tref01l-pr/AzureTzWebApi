using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using tzWepApi.Interfaces;

namespace tzWepApi.Controllers;

public class EmailSenderController : ControllerBase
{
    private readonly IEmailSender _emailSender;

    public EmailSenderController(IEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    
    [HttpPost("sendMessage")]
    public async Task<IActionResult> SendMessage(string receiver = "protsayroman228@gmail.com", string subject = "Test", string message = "Hello world")
    {
        try
        {
            await _emailSender.SendEmailAsync(receiver, subject, message);
        }
        catch (SmtpException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"SMTP Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"General Error: {ex.Message}");
        }

        return Ok("Success");
    }
}