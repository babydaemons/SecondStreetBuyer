using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SecondStreetBuyer;

class Program
{
    private const string SessionFile = "session.json";
    private const string LoginUrl = "https://www.2ndstreet.jp/user/login";

    public static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false
        });

        BrowserNewContextOptions contextOptions = new()
        {
            UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 16_4 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.4 Mobile/15E148 Safari/604.1",
            ViewportSize = new ViewportSize { Width = 390, Height = 844 },
            IsMobile = true,
            DeviceScaleFactor = 3,
            HasTouch = true,
            Locale = "ja-JP"
        };

        // セッションファイルがあれば読み込む
        if (File.Exists(SessionFile))
        {
            contextOptions.StorageStatePath = SessionFile;
            Console.WriteLine("★ セッションファイルを読み込みました（自動ログイン）");
        }
        else
        {
            Console.WriteLine("☆ セッションファイルが存在しません。手動ログインモードに入ります。");
        }

        var context = await browser.NewContextAsync(contextOptions);
        var page = await context.NewPageAsync();

        if (!File.Exists(SessionFile))
        {
            // 手動ログイン用ページへ移動
            await page.GotoAsync(LoginUrl);
            Console.WriteLine("☆ ブラウザでログインしてください（CAPTCHA含む）");
            await context.StorageStateAsync(new() { Path = SessionFile });
            Console.WriteLine("★ セッション保存完了！");
        }

        // ログイン済みで、ここから自動処理をスタート
        await page.GotoAsync("https://www.2ndstreet.jp/");
        Console.WriteLine("★ ログイン済み状態でサイトにアクセスしました");

        var viewLinks = new List<Item>();

        var itemLinks = new ItemLinks(page);
        foreach (var url in new[]
        {
            ItemLinks.MensWare,
            ItemLinks.LadiesWare,
            ItemLinks.FassionGoods,
            ItemLinks.Luxury,
            ItemLinks.PopularBrands
        })
        {
            var links = await itemLinks.GetItemLinks(url);
            viewLinks.AddRange(links);
        }

        View(viewLinks);

        // 終了
        await browser.CloseAsync();
    }

    [STAThread]
    private static void View(IEnumerable<Item> viewLinks)
    {
        var app = new Application();
        var mainWindow = new MainWindow();
        mainWindow.LoadItems(viewLinks);
        app.Run(mainWindow);
    }
}
