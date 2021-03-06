﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace CSV_AutoFormFill
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver;
            string[][] profiles = ReadCSV_GetValues();

            for (int j = 0; j < profiles.Count(); j++)
            {
                driver = new ChromeDriver();
                driver.Navigate().GoToUrl("https://www.activepdf.com/products/ocr/download?hsCtaTracking=e71ff45f-425f-43cd-a196-e20f6fe59774%7C2acbcf88-0cf5-44e5-a98c-e4e8dfd58457");

                string[] tfSelectors = { "[name*='firstname']", "[name*='lastname']", "[name*='email']", "[name*='phone']", "[name*='company_initial_form_submission']" };
                int i;
                for (i = 0; i < tfSelectors.Count(); i++)
                {
                    TextInput(driver, tfSelectors[i], profiles[j][i]);
                };

                Dropdown(driver, profiles[j][i]);
                ProjectTimeline(driver, profiles[j][++i]);
                SelectOption(driver, profiles[j][++i]);
                Consent(driver);

                //This is the download button
                //driver.FindElement(By.CssSelector("[type*='submit']"));
                driver.Quit();
            }
        }

        public static void Consent(IWebDriver driver)
        {
            Wait(driver, "[name*='LEGAL_CONSENT.subscription_type_4614226']").Click();
            Wait(driver, "[name*='LEGAL_CONSENT.subscription_type_4459042']").Click();
            Wait(driver, "[name*='LEGAL_CONSENT.processing']").Click();
        }

        public static void SelectOption(IWebDriver driver, string index)
        {
            string selector = @"$(""[name*='free_trial_option']"")[" + index + "]";

            jQueryClick(driver, selector);
        }

        public static void ProjectTimeline(IWebDriver driver, string index)
        {
            string selector = @"$(""[name*='project_timeline']"")["+index+"]";
            jQueryClick(driver, selector);
        }

        public static void Dropdown(IWebDriver driver, string index)
        {
            IWebElement describeYourself = Wait(driver, "[name*='hs_persona']");
            SelectElement dropdown = new SelectElement(describeYourself);
            dropdown.SelectByIndex(Convert.ToInt32(index));
        }

        public static void jQueryClick(IWebDriver driver, string selector)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            string script = "$(" + selector + ").trigger('click')";
            js.ExecuteScript(script);
        }

        //Will explicitly wait until element is found.
        //Then method will return the webelement.
        public static IWebElement Wait(IWebDriver driver, string selector)
        {
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
        }

        //Find the element, Scroll into view, then sends text
        public static void TextInput(IWebDriver driver, string selector, string text)
        {
            IWebElement input = Wait(driver, selector);
            Scroll(driver, input);
            input.SendKeys(text);
        }


        //Reads the CSV file and then returns an array of values
        public static string[][] ReadCSV_GetValues()
        {
            StreamReader sr = new StreamReader(File.OpenRead(Directory.GetCurrentDirectory() + @"\fields.csv"));
            List<string[]> profilesList = new List<string[]>();
            string line;
            while( (line = sr.ReadLine()) != null)
            {
                //Removes all whitespace then separate string by commas and convert into array
                line.Replace(" ", "");
                string[] profile = (line.Split(','));
                profilesList.Add(profile);
            }
            string[][] profilesArr = profilesList.ToArray();

            return profilesArr;

        }

        //Scrolls into whatever page you're looking for.
        //Without this method, looking for a sidebar page thumbnail
        //that is hidden from view will bug and crash.
        public static void Scroll(IWebDriver driver, IWebElement selector)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            js.ExecuteScript("arguments[0].scrollIntoView();", selector);
        }

    }
}