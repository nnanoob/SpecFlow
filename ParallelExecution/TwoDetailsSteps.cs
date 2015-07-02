using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace ParallelExecution
{
    public class Detail
    {
        public Detail()
        {
            Values = new List<int>();
        }

        public IList<int> Values { get; private set; }
        public int Results { get; set; }
    }

    [Binding]
    public class TwoDetailsSteps
    {
        static Random _r = new Random();
        private readonly Detail _testDetail;

        public TwoDetailsSteps(Detail testDetail)
        {
            _testDetail = testDetail;
        }

        [When(@"I press add")]
        public void WhenIPressAdd()
        {
            Thread.Sleep(TimeSpan.FromSeconds(_r.Next(1, 2)));
            _testDetail.Results = _testDetail.Values.Sum();
        }

        [Given(@"I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredIntoTheCalculator(int p0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(_r.Next(1, 2)));
            _testDetail.Values.Add(p0);
        }

        [Then(@"the result should be (.*) on the screen")]
        public void ThenTheResultShouldBeOnTheScreen(int p0)
        {
            Thread.Sleep(TimeSpan.FromSeconds(_r.Next(1, 2)));
            Assert.AreEqual(p0, _testDetail.Results);
        }
    }
}
