using System;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

Thread Scape1 = new Thread(new ThreadStart(static () => Scrape(1)));
Thread Scape2 = new Thread(new ThreadStart(static () => Scrape(2)));
Thread Scape3 = new Thread(new ThreadStart(static () => Scrape(3)));
Scape1.Start();
Console.WriteLine();
Scape2.Start();
Console.WriteLine();
Scape3.Start();

static void Scrape(int pageNumber)
{



    string url = $"https://www.myh.se/om-oss/sok-handlingar-i-vart-diarium?katalog=Tillsynsbeslut%20yrkesh%C3%B6gskoleutbildning&p={pageNumber}";
    if (pageNumber == 1)
    {
        url = "https://www.myh.se/om-oss/sok-handlingar-i-vart-diarium?katalog=Tillsynsbeslut%20yrkesh%C3%B6gskoleutbildning";
    }
    try
    {
        // Starta en ny Chrome-webbläsare i "headless" läge
        var options = new ChromeOptions();
        options.AddArgument("--headless"); // Kör i headless-läge för att inte visa webbläsarens UI
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        using (var driver = new ChromeDriver(options))
        {
            // Navigera till URL
            driver.Navigate().GoToUrl(url);

            // Vänta tills sidan är helt laddad
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete")); // Vänta tills ett specifikt element finns på sidan
            Thread.Sleep(1000);
            // Hämta HTML-innehållet från den renderade sidan
            string htmlContent = driver.PageSource;

            // Använd HtmlAgilityPack för att analysera HTML-innehållet
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // Extrahera information från varje listobjekt (<a>-element)
            var linkNodes = doc.DocumentNode.SelectNodes("//a[contains(@class, 'v-list-item') and contains(@class, 'v-list-item--link')]");

            if (linkNodes != null)
            {
                foreach (var link in linkNodes)
                {
                    var diaryNumberNode = link.SelectSingleNode(".//div[contains(@class, 'v-list-item__subtitle') and contains(@class, 'letter-space-2')]");
                    if (diaryNumberNode != null)
                    {
                        Console.WriteLine("Diarienummer: " + diaryNumberNode.InnerText.Trim());
                    }
                    else
                    {
                        Console.WriteLine("Inget diarienummer hittades.");
                    }
                    var reviewNode = link.SelectSingleNode(".//div[contains(@class, 'v-list-item__title') and contains(@class, 'myh-h3')]");
                    if (reviewNode != null)
                    {
                        Console.WriteLine("Granskning: " + reviewNode.InnerText.Trim());
                    }
                    else
                    {
                        Console.WriteLine("Ingen granskningsinformation hittades.");
                    }

                    // Extrahera aktörsnamn
                    var actorNode = link.SelectSingleNode(".//span[contains(@class, 'v-card') and contains(@class, 'text--primary') and contains(@class, 'myh-body-2')]");
                    if (actorNode != null)
                    {
                        Console.WriteLine("Aktör: " + actorNode.InnerText.Trim());
                    }
                    else
                    {
                        Console.WriteLine("Ingen aktör hittades.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Inga länkar hittades.");
            }
        }
    }
    catch (WebDriverException e)
    {
        Console.WriteLine($"Ett fel uppstod: {e.Message}");
    }

}
