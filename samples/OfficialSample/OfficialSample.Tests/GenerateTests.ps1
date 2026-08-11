$chapters = Get-ChildItem -Path "..\*" -Directory -Exclude "obj", "bin" | Where-Object { $_.Name -match "^\d{2}-" }

foreach ($chapterDir in $chapters) {
    $chapterName = $chapterDir.Name
    $chapterNumberStr = $chapterName.Substring(0, 2)
    $chapterNamespace = "Chapter$chapterNumberStr"
    
    $testFileName = "Chapter${chapterNumberStr}Tests.cs"
    if (Test-Path $testFileName) { continue }
    
    $programCsPath = Join-Path $chapterDir.FullName "Program.cs"
    if (-not (Test-Path $programCsPath)) { continue }
    
    $content = Get-Content $programCsPath -Raw
    
    $testContent = @"
using $chapterNamespace;
using EricksonLopez.DomainPrimitives;

namespace OfficialSample.Tests;

public class Chapter${chapterNumberStr}Tests
{
"@

    # If it defines CustomerId
    if ($content -match "record struct CustomerId") {
        $testContent += @"
    [Fact]
    public void CustomerId_New_ShouldCreateValidId()
    {
        var id = CustomerId.New();
        Assert.NotEqual(Guid.Empty, id.Value);
    }
"@
    }

    # If it defines OrderId
    if ($content -match "record struct OrderId") {
        $testContent += @"
    [Fact]
    public void OrderId_New_ShouldCreateValidId()
    {
        var id = OrderId.New();
        Assert.NotEqual(Guid.Empty, id.Value);
    }
"@
    }
    
    # If it defines ProductId
    if ($content -match "record struct ProductId") {
        $testContent += @"
    [Fact]
    public void ProductId_New_ShouldCreateValidId()
    {
        var id = ProductId.New();
        Assert.NotEqual(Guid.Empty, id.Value);
    }
"@
    }

    # If it defines EmailAddress
    if ($content -match "record struct EmailAddress") {
        $testContent += @"
    [Theory]
    [InlineData("test@example.com")]
    public void EmailAddress_Create_WithValidEmail_ShouldSucceed(string email)
    {
        var result = EmailAddress.TryCreate(email);
        Assert.True(result.IsSuccess);
    }
"@
    }
    
    # If it defines Money
    if ($content -match "record struct Money") {
        $testContent += @"
    [Fact]
    public void Money_Create_WithValidAmount_ShouldSucceed()
    {
        var result = Money.TryCreate(100.50m);
        Assert.True(result.IsSuccess);
    }
"@
    }

    $testContent += @"
}
"@
    
    Set-Content -Path $testFileName -Value $testContent
}
