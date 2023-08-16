# MD To HTML
____
## What does this do?

This Azure Function App is designed to intake HTML and return a PDF file. 
The app is capable of returning either a file or a base64 encoded string containing the PDF for attaching to emails, for example.

## How do I use this?

You'll need the following:
- Microsoft Azure subscription
- Microsoft Azure Resource Group
- Microsoft Azure Function App (Linux, .NET)
- Azure Command Line Tools (Azure Powershell is used as the example)

### Deploying to Azure

1. Launch Powershell and install the Azure Powershell module with `Import-Module az`
2. Connect to Azure Powershell with Connect-AzAccount
3. Navigate to the directory with the .cs Function App file and enter `func azure functionapp publish {YOUR FUNCTION APP NAME}`
   - For example, if your function app was named `SuperAwesomeFunctions`, you'd run `func azure functionapp publish SuperAwesomeFunctions`
4. Your output will show the results of building the .NET application dll, then the upload progress, then finally an Invoke Url

### Using the function app

The function app will expect a POST of the following JSON:
```json
{
  "html": "StringifiedHtml",
  "base64": true
}
```

Setting base64 to `true` will return:
```json
{
  "base64Pdf": "Base64String"
}
```

Setting base64 to `false` will return the PDF file as a download