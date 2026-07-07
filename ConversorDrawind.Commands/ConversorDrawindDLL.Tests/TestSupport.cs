namespace ConversorDrawindDLL.Tests;

internal sealed class CultureScope : IDisposable
{
    private readonly CultureInfo currentCulture;
    private readonly CultureInfo currentUICulture;

    public CultureScope(string cultureName)
    {
        currentCulture = CultureInfo.CurrentCulture;
        currentUICulture = CultureInfo.CurrentUICulture;

        var culture = CultureInfo.GetCultureInfo(cultureName);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = currentCulture;
        CultureInfo.CurrentUICulture = currentUICulture;
    }
}

internal sealed class TestWorkspace : IDisposable
{
    public TestWorkspace(string folderName)
    {
        Root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
        Directory.CreateDirectory(Root);
    }

    public string Root { get; }

    public static TestWorkspace Create()
    {
        return new TestWorkspace("TestData_" + Guid.NewGuid().ToString("N"));
    }

    public string GetFile(string fileName)
    {
        return Path.Combine(Root, fileName);
    }

    public void Dispose()
    {
        if (Directory.Exists(Root))
        {
            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    Directory.Delete(Root, recursive: true);
                    return;
                }
                catch (IOException)
                {
                    Thread.Sleep(50);
                }
                catch (UnauthorizedAccessException)
                {
                    Thread.Sleep(50);
                }
            }
        }
    }
}

internal static class TestState
{
    public static void Reset()
    {
        TestHooks.ResetGlobalState();
    }
}

