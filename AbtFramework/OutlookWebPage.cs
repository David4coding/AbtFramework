﻿using AbtFramework.Utils_Classes;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AbtFramework
{
    public class OutlookWebPage : PageModel
    {

        [FindsBy(How=How.TagName,Using ="span")]
        private IList<IWebElement> spans;
        [FindsBy(How = How.TagName, Using = "button")]
        private IList<IWebElement> buttons;
        [FindsBy(How=How.ClassName,Using = "_ho2_0")]
        private IWebElement userCard;

        [FindsBy(How = How.ClassName,Using ="_pe_01")]
        private IWebElement divInfo;
        [FindsBy(How=How.LinkText,Using ="Outlook Web Access")]
        private IWebElement outlookLink;
        [FindsBy(How = How.TagName, Using = "div")]
        private IList<IWebElement> divs;

        [FindsBy(How = How.ClassName, Using = "_fce_c")]
        private IWebElement newMailWrapper;

        [FindsBy(How = How.TagName, Using = "input")]
        private IList<IWebElement> inputs;
        [FindsBy(How=How.Id,Using = "MailFolderPane.FavoritesFolders")]
        private IWebElement EmailFolders;

        [FindsBy(How = How.ClassName, Using = "findControlWrapper")]
        private IWebElement recipientWrapper;

        [FindsBy(How = How.ClassName, Using = "conductorContent")]
        private IWebElement EmailList;



        public bool SendEmail(string receipient, string subjectText, string bodyText)
        {
           
          
            action.Click(getEmailBtn()).Perform(); 
            getRecipientElement().SendKeys(receipient);
            getRecipientElement().SendKeys(Keys.Enter);
            getSubjectInput().SendKeys(subjectText);
            getSubjectInput().SendKeys(Keys.Tab);
            IWebElement body = Driver.seleniumdriver.SwitchTo().ActiveElement();
            body.SendKeys(bodyText);
            body.SendKeys(Keys.Tab);
            IWebElement sendBtn = Driver.seleniumdriver.SwitchTo().ActiveElement();
            action.Click(sendBtn).Perform();
            // now check if it was sent
            action.Click(GetSentItemsFolder()).Perform();

            var LastEmail = GetEmailList().First();

            return LastEmail.GetAttribute("aria-label").Split(',')[2].Contains(subjectText);

        }

        public void OpenEmail()
        {
            wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("div")));

            var emails = GetEmailList();
            var randomMail = emails.First();


            wait.Timeout = TimeSpan.FromSeconds(20);
            wait.PollingInterval = TimeSpan.FromSeconds(1);
            wait.Until((e) =>
            {
                if (Driver.seleniumdriver.WindowHandles.Count < 2)
                {
                    action.DoubleClick(randomMail).Perform();

                }
                else
                {

                    return true;
                }

                return false;

            });

            Console.WriteLine("First Window handle: "+Driver.seleniumdriver.WindowHandles.First());

            Console.WriteLine("Last Window handle: " + Driver.seleniumdriver.WindowHandles.Last());

            Driver.seleniumdriver.SwitchTo().Window(Driver.seleniumdriver.WindowHandles.Last());
      
      
           

        }

        private IWebElement GetSentItemsFolder()
        {
            return EmailFolders.FindElements(By.TagName("span"))
                           .Single(e => e.Text.Equals("Sent Items"));
        }

        public IEnumerable<IWebElement>  GetEmailList()
        {
                     

              return EmailList.FindElements(By.TagName("div"))
                                  .Where(e => e.GetAttribute("autoid") != null && e.GetAttribute("autoid")
                                  .Equals("_lvv_3"));
        }

        private IWebElement getSendBtn()
        {
            var elements = buttons.Where(e => e.GetAttribute("aria-label") != null);
            return elements.Where(e => e.GetAttribute("aria-label").Equals("Send")).First();
        }

        private IWebElement getSubjectInput()
        {
            var elements = inputs.Where(e => e.GetAttribute("placeholder") != null);
            return elements.Single(e => e.GetAttribute("placeholder").Equals("Add a subject"));
            
        }

        private IWebElement getBodyInput()
        {
           // var elements = divs.Single(e=>e.Text.Equals("Add a message or drag a file here"));
           // Console.WriteLine("amount of divs overall"+divs.Count());
           // Console.WriteLine("amount of div elements with aria-label:"+elements.Count());
          //  return elements.Single(e => e.GetAttribute("aria-label").Equals("Message body"));
          return divs.Where(e => e.Text.Equals("Add a message or drag a file here")).First();
        }

        private IWebElement getEmailBtn()
        {
         
          return  newMailWrapper.FindElements(By.TagName("button")).Where(e => e.GetAttribute("title").Equals("Write a new message (N)")).First();
        }

        private IWebElement getRecipientElement()
        {
            //var elements = inputs.Where(e => e.GetAttribute("aria-label") != null);
            // return elements.Single(e => e.GetAttribute("aria-label")
            //   .Equals("To recipients. Enter an email address or a name from your contact list."));

            return recipientWrapper.FindElements(By.TagName("input"))
                .Single(e => e.GetAttribute("aria-label")
                .Equals("To recipients. Enter an email address or a name from your contact list."));

        }

     
    
        public bool isAt()
        {

            var btns = buttons.Where(b => b.GetAttribute("autoid")!=null); //eliminate btns withouth autoid attribute
            var btn = btns.SingleOrDefault(b => b.GetAttribute("autoid").Equals("_ho2_0")); //get the btn that contains user info
            Console.WriteLine("Outlook Web Page Loaded in: "+LoadTime);
            btn.Click();
         
          
            IWebElement username =divInfo.FindElements(By.TagName("span"))[0];  //this div element contains to spans nested and the first one is the one that contains the username
            if (username.Text.Equals(SSOCrendentials.CurrentUser))
            {
                return true;

            }


            return false;
           
        }
    }
}