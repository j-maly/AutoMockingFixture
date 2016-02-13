using AutoMockingFixture;
using NUnit.Framework;
using Rhino.Mocks;
using SampleProject;

namespace SampleProjectTests
{
    public class MyServiceTests: AutoMockingFixture<MyService>
    {
        // in both tests we are mocking 2 dependencies and providing 
        // a real implementation for the third one (transformer)
        // Test2 uses the fields to access mocks and dependencies, 
        // Test1 uses Put and Dep methods provided by base class

        [Test]
        public void Test1()
        {
            Put<IMyTransformer, MyTransformer>();
            // ARRANGE
            Dep<IDataProvider>().Stub(p => p.GetData()).Return("test");
            // ACT 
            Subject.DoStuff();
            // ASSRT
            Dep<IOutputWriter>().AssertWasCalled(w => w.WriteData("test (transformed)"));
        }

        [Mock]
        private IDataProvider dataProviderMock;
        [Mock]
        private IOutputWriter outputWriterMock;
        [Dependency]
        private IMyTransformer transformer = new MyTransformer();

        [Test]
        public void Test2()
        {
            // ARRANGE
            dataProviderMock.Stub(p => p.GetData()).Return("test");
            // ACT 
            Subject.DoStuff();
            // ASSRT
            outputWriterMock.AssertWasCalled(w => w.WriteData("test (transformed)"));
        }
    }
}