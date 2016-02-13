using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor;
using NUnit.Framework;
using Rhino.Mocks;

namespace AutoMockingFixture
{
    public abstract class AutoMockingFixture<TSubject> 
        where TSubject : class 
    {
        protected WindsorContainer autoMockingContainer;
        
        [SetUp]
        public void SetUp()
        {
            autoMockingContainer = new WindsorContainer();
            autoMockingContainer.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LazyComponentAutoMocker>());
            ResolveMockFields();
            ResolveDependencyFields();
            autoMockingContainer.Register(Component.For<TSubject>());
        }

        [TearDown]
        public void TearDown()
        {
            autoMockingContainer.Dispose();
        }

        public TSubject Subject
        {
            get { return autoMockingContainer.Resolve<TSubject>(); }
        }

        protected TDependency Dep<TDependency>()
        {
            var dependency = autoMockingContainer.Resolve<TDependency>();
            return dependency;
        }

        protected TMock Mock<TMock>() where TMock : class
        {
            return MockRepository.GenerateMock<TMock>();
        }

        protected void Put<TDependency>([NotNull] TDependency dependency) where TDependency : class
        {
            autoMockingContainer.Register(Component.For<TDependency>()
                .Instance(dependency).IsDefault().Named(dependency.GetType().Name + "-Explicit"));
        }

        protected void Put<TDependency, TDependencyImplementation>() 
            where TDependency : class
            where TDependencyImplementation : TDependency
        {
            autoMockingContainer.Register(Component.For<TDependency>().ImplementedBy<TDependencyImplementation>()
                .IsDefault().Named(typeof(TDependencyImplementation).Name + "-Explicit"));
        }

        private void ResolveMockFields()
        {
            var mockFields = this.GetType()
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(field => Attribute.IsDefined(field, typeof(MockAttribute)));
            foreach (var mockField in mockFields)
            {
                var type = mockField.FieldType;
                var resolvedMock = autoMockingContainer.Resolve(type);
                mockField.SetValue(this, resolvedMock);
            }
        }

        private void ResolveDependencyFields()
        {
            var dependencyFields = this.GetType()
                    .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(field => Attribute.IsDefined(field, typeof(DependencyAttribute)));
            foreach (var dependencyField in dependencyFields)
            {
                var dependency = dependencyField.GetValue(this);
                autoMockingContainer.Register(Component.For(dependencyField.FieldType).Instance(dependency));
            }
        }

        internal class LazyComponentAutoMocker : ILazyComponentLoader
        {
            public IRegistration Load(string key, Type service, IDictionary arguments)
            {
                var stub = MockRepository.GenerateStub(service);
                return Component.For(service).Instance(stub);
            }
        }
    }

    public class NotNullAttribute : Attribute
    {
    }
}
