using ParallelExecution;
using TechTalk.SpecFlow;

namespace ParallelExecution01.Hooks
{
    [Binding]
    public sealed class Hooks1
    {
        private readonly Detail _testDetail;

        public Hooks1(Detail testDetail)
        {
            _testDetail = testDetail;
        }

        [BeforeScenario("AddValue20BeforeScenario")]
        public void BeforeScenario()
        {
            _testDetail.Values.Add(20);
        }
    }
}
