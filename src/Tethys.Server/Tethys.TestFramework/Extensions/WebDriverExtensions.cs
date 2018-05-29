namespace OpenQA.Selenium
{
    public static class WebDriverExtensions
    {
        public static IWebElement FindElementByName(this IWebDriver webDriver, string name)
        {
            return webDriver.FindElement(By.Name(name));
        }
    }
}
