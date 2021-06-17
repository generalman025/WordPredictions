using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WordPredictions;

namespace UnitTestWordPrediction
{
    [TestClass]
    public class UnitTestForWordPrediction
    {
        [TestMethod]
        public void CheckResultFromWebService()
        {
            var testString = "abc";
            
            var ws = new WebServiceCaller();
            var result = ws.getWords(testString);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count, 0);
            Assert.IsTrue(result[0].ToLower().StartsWith(testString));
        }

        [TestMethod]
        public void CheckResultFromSQLite()
        {
            var testString = "ab";

            var sqlite = new SQLiteAdapter();
            var result = sqlite.getWords(testString);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(result.Count, 0);
            Assert.IsTrue(result[0].ToLower().StartsWith(testString));
        }
    }
}
