using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuppeteerSharp;

namespace MDToHTML;

public static class MDToHTML
{
    [FunctionName("MDToHTML")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var html = JsonConvert.DeserializeObject<Inbound>(requestBody);
        
        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();
        await page.SetContentAsync(html.Html);
        var outputStream = await page.PdfStreamAsync();
        var outputBytes = await page.PdfDataAsync();
        await browser.CloseAsync();

        if (html.Base64)
        {
            var outboundObject = new Outbound
            {
                base64Pdf = Convert.ToBase64String(outputBytes)
            };
            var serializedJson = JsonConvert.SerializeObject(outboundObject);
            return new OkObjectResult(serializedJson);
        }

        return new FileStreamResult(outputStream, "application/pdf");
    }
}

public class Inbound
{
    public string Html { get; set; }
    public bool Base64 { get; set; }
}

public class Outbound
{
    public string base64Pdf { get; set; }
}