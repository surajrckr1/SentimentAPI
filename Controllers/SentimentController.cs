using Microsoft.AspNetCore.Mvc;
using SentimentAPI.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SentimentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SentimentController : ControllerBase
    {
        static string[] positiveWords = null;
        static string[] negativeWords = null;
        public SentimentController()
        {
           // string[] scoreLines = System.IO.File.ReadAllLines("stockdata.txt");
            string[] scoreLines = System.IO.File.ReadAllLines(@"C:\VS2019\WebAPI\stockdata.txt");
            positiveWords = scoreLines.Where(c => c.Contains("1")).Select(x => x.Replace(" 1,", "")).ToArray();
            negativeWords = scoreLines.Where(c => c.Contains("0")).Select(x => x.Replace(" 0,", "")).ToArray();
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value12", "value22" };
        }

        [HttpPost]
        public ActionResult CheckArticleSentiment(StockDataModel model)
        {
            int _negativeWords = 0; int _positivewords = 0;
            SmartReader.Reader sr = new SmartReader.Reader(model.StockUrl);
            SmartReader.Article article = sr.GetArticle();
            if (article.IsReadable)
            {
                Regex rxFirst5Words = new Regex(@"^\W*\w+(?:\W+\w+){0," + model.WordCount + "}");
                string stringToCheck = rxFirst5Words.Match(article.TextContent).ToString();
                foreach (string x in positiveWords)
                {
                    if (stringToCheck.Contains(x))
                    {
                        _positivewords++;
                    }
                }
                foreach (string x in negativeWords)
                {
                    if (stringToCheck.Contains(x))
                    {
                        _negativeWords++;
                    }
                }
            }
            dynamic flexible = new ExpandoObject();
            flexible.Positive = _positivewords;
            flexible.Negative = _negativeWords;

            return Ok(flexible);
        }
    }
}
