using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using tzWepApi.Interfaces;
using tzWepApi.Models;

namespace tzWepApi.Controllers;

[Route("api")]
[ApiController]
public class FileController : ControllerBase
{
    
    private readonly IFileService _fileService;
    
    

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    [HttpGet("hello")]
    public IActionResult Hello()
    {
        return Ok(new
        {
            message = "Hello There"
        });
    }

    [HttpPost("saveFile")]
    public async Task<IActionResult> SaveFile(TzModel data, CancellationToken cancellationToken)
    {
        if (Path.GetExtension(data.File.FileName) != ".docx")
        {
            return BadRequest("Invalid file format. Only .docx files are allowed.");
        }

        var email = new EmailAddressAttribute();
        if (!email.IsValid(data.Email))
        {
            return BadRequest("Invalid email format.");
        }
        
        try
        {
            await _fileService.SaveFile("tz-docx-container", data.File, data.Email);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"General Error: {ex.Message}");
        }
        
        return Ok("Success");
    }
    
    [HttpGet("downloadFile")]
    public async Task<IActionResult> DownloadFile(string blobName)
    {
        try
        {
            var memoryStream = await _fileService.DownloadBlobToFileAsync("tz-docx-container", blobName);
            return File(memoryStream, "application/octet-stream", blobName);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"General Error: {ex.Message}");
        }
    }

    [HttpDelete("deleteFile")]
    public async Task<IActionResult> DeleteFile(string blobName)
    {
        try
        {
            await _fileService.DeleteFile("tz-docx-container", blobName);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"General Error: {ex.Message}");
        }
        
        return Ok("Success");
    }
}