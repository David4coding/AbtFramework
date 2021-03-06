﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace XmlTransformer
{
    class Program
    {
        static void Main(string[] args)
        {
            string problematicPath = args[1].Substring(1);
            
            if(args.Length==3)
            convertXmlFile(args[0], problematicPath,args[2]);

            else
            {
                Console.WriteLine("needs to provide xml path, stylesheet for transform, and full path where you want the results to be saved.");
            }
           
        }

        private static void AddBootStrap(String htmlPath)
        {
            string[] HtmlResults ;
            List<String> BootstrapLines;

            using (StreamReader reader = new StreamReader(htmlPath))
            {
               string line = String.Empty;
                

              string htmlfile= reader.ReadToEnd();
                var lines = htmlfile.Split('\n');
                BootstrapLines = new List<string>();
                BootstrapLines.Add("<html><head><title>Test Results</title>");
                BootstrapLines.Add("<meta charset=\"utf - 8\">");
                BootstrapLines.Add("<meta name=\"viewport\" content=\"width = device - width, initial - scale = 1\">");
                BootstrapLines.Add("<link rel=\"stylesheet\" href=\"http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css\">");
                BootstrapLines.Add("<script src=\"https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js\"></script>");
                BootstrapLines.Add("<script src=\"http://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js\"></script></head><body><div class=\"container\"><h1> Test Results </h1><div>");

                HtmlResults = new string[lines.Length+BootstrapLines.Count]; //create array with size of source html + boostrap lines
                BootstrapLines.CopyTo(0, HtmlResults, 0, BootstrapLines.Count); //First Copy boostraplines
                lines[0] = String.Empty;                //delete the first line of the source html that contains the old header
                lines.CopyTo(HtmlResults, 6);           //now copy all lines from html source starting at the 6 position on target array(html results)    
            }
            File.Delete(htmlPath);


            using (StreamWriter sw = File.CreateText(htmlPath))
            {
                foreach(string  line in HtmlResults)
                {
                    sw.WriteLine(line);
               
                }

            }
        }

        private static void convertXmlFile(string xmlPath,string stylesheet,string resultsPath)
        {
            try
            {
                Console.WriteLine("Xml Path is : " + xmlPath);
                Console.WriteLine("stylesheet Path is : " + stylesheet);
                Console.WriteLine("results path is : " + resultsPath);
                XPathDocument xml = new XPathDocument(@""+xmlPath);  //load xml generated by xunit task runner
                XslCompiledTransform xslTransform = new XslCompiledTransform();  //instantiate xml transform object
                xslTransform.Load(@""+stylesheet); //Load XSL sheet that will do the xml transformation
                string HtmlResultsPath = @""+resultsPath +"\\"+ DateTime.Now.ToString("yyyyMMddHHmmss")+ "results.html";
                 XmlTextWriter writer = new XmlTextWriter(HtmlResultsPath, null);   //Loads xml writer 
                xslTransform.Transform(xml, null, writer); //apply transform
                writer.Close();
                AddBootStrap(HtmlResultsPath);
                Console.WriteLine(@"You can find the Html results at:"+HtmlResultsPath);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException());
            }
        }
    }
}
