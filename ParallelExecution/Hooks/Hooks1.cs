using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace ParallelExecution.Hooks
{
    [Binding]
    public sealed class Hooks1
    {
        private readonly Detail _testDetail;

        public Hooks1(Detail testDetail)
        {
            _testDetail = testDetail;
        }

        [BeforeScenario("AddValue10BeforeScenario")]
        public void BeforeScenario()
        {
            _testDetail.Values.Add(10);
        }
    }
}
