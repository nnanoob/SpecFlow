﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IContextManager
    {
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }
		ScenarioStepContext StepContext { get; }
        void InitializeFeatureContext(FeatureInfo featureInfo, CultureInfo bindingCulture);
        void CleanupFeatureContext();

        void InitializeScenarioContext(ScenarioInfo scenarioInfo);
        void CleanupScenarioContext();
        void InitializeStepContext(StepInfo stepInfo);
        void CleanupStepContext();
    }

    internal static class ContextManagerExtensions
    {
        public static StepContext GetStepContext(this IContextManager contextManager)
        {
            return new StepContext(
                contextManager.FeatureContext == null ? null : contextManager.FeatureContext.FeatureInfo,
                contextManager.ScenarioContext == null ? null : contextManager.ScenarioContext.ScenarioInfo);
        }
    }

    public class ContextManager : IContextManager, IDisposable
    {
        private class InternalContextManager<TContext>: IDisposable where TContext : SpecFlowContext
        {
            private readonly ITestTracer testTracer;

            public InternalContextManager(ITestTracer testTracer)
            {
                this.testTracer = testTracer;
            }

            public TContext Instance { get; private set; }


            public void Init(TContext newInstance)
            {
                if (Instance != null)
                {
                    testTracer.TraceWarning(string.Format("The previous {0} was not disposed.", typeof(TContext).Name));
                    Dispose();
                }
                Instance = newInstance;
            }

            public void Cleanup()
            {
                if (Instance == null)
                {
                    testTracer.TraceWarning(string.Format("The previous {0} was already disposed.", typeof(TContext).Name));
                    return;
                }
                ((IDisposable)Instance).Dispose();
                Instance = null;
            }

            public void Dispose()
            {


                if (Instance != null)
                {
                    ((IDisposable)Instance).Dispose();
                    Instance = null;
                }
            }
        }

        /// <summary>
        /// Implementation of internal context manager which keeps a stack of contexts, rather than a single one. 
        /// This allows the contexts to be used when a new context is created before the previous context has been completed 
        /// which is what happens when a step calls other steps. This means that the step contexts will be reported 
        /// correctly even when there is a nesting of steps calling steps calling steps.
        /// </summary>
        /// <typeparam name="TContext">A type derived from SpecFlowContext, which needs to be managed  in a way</typeparam>
        private class StackedInternalContextManager<TContext> : IDisposable where TContext : SpecFlowContext
        {
            private readonly ITestTracer testTracer;
            private readonly Stack<TContext> instances = new Stack<TContext>();

            public StackedInternalContextManager(ITestTracer testTracer)
            {
                this.testTracer = testTracer;
            }

            public TContext Instance
            {
                get { return instances.Any() ? instances.Peek() : null; }
            }

            public void Init(TContext newInstance)
            {
                instances.Push(newInstance);
            }

            public void Cleanup()
            {
                if (!instances.Any())
                {
                    testTracer.TraceWarning(string.Format("The previous {0} was already disposed.", typeof(TContext).Name));
                    return;
                }
                var instance = instances.Pop();
                ((IDisposable)instance).Dispose();

            }

            public void Dispose()
            {
                var instance = instances.Pop();
                if (instance != null)
                {
                    ((IDisposable)instance).Dispose();

                }
            }
        }

        private readonly IObjectContainer parentContainer;

#if BODI_LIMITEDRUNTIME  
        private InternalContextManager<ScenarioContext> _scenarioContext;
#else
        private ThreadStorage<InternalContextManager<ScenarioContext>> _scenarioContext;
        private ThreadStorage<StackedInternalContextManager<ScenarioStepContext>> _stepContext;

#endif
        private readonly InternalContextManager<FeatureContext> featureContext;

        public ContextManager(ITestTracer testTracer, IObjectContainer parentContainer)
        {
            featureContext = new InternalContextManager<FeatureContext>(testTracer);

#if BODI_LIMITEDRUNTIME  
            _scenarioContext = new InternalContextManager<ScenarioContext>(testTracer);
#else
            _scenarioContext =new ThreadStorage<InternalContextManager<ScenarioContext>>(
                () => new InternalContextManager<ScenarioContext>(testTracer));
            _stepContext = new ThreadStorage<StackedInternalContextManager<ScenarioStepContext>>(
                () => new StackedInternalContextManager<ScenarioStepContext>(testTracer));
#endif
            this.parentContainer = parentContainer;
        }

        private InternalContextManager<ScenarioContext> scenarioContext
        {
#if BODI_LIMITEDRUNTIME  
            get{ return _scenarioContext;}
#else
            get { return _scenarioContext.ThreadInstance; }
#endif
        }

        private StackedInternalContextManager<ScenarioStepContext> stepContext
        {
#if BODI_LIMITEDRUNTIME  
            get{ return _stepContext;}
#else
            get { return _stepContext.ThreadInstance; }
#endif
        }

        public FeatureContext FeatureContext
        {
            get { return featureContext.Instance; }
        }

        public ScenarioContext ScenarioContext
        {
            get { return scenarioContext.Instance; }
        }

		public ScenarioStepContext StepContext 
        {
            get{return stepContext.Instance;} 
        }
		
        public void InitializeFeatureContext(FeatureInfo featureInfo, CultureInfo bindingCulture)
        {
            var newContext = new FeatureContext(featureInfo, bindingCulture);
            featureContext.Init(newContext);
            FeatureContext.Current = newContext;
        }

        public void CleanupFeatureContext()
        {
            featureContext.Cleanup();
        }

        public void InitializeScenarioContext(ScenarioInfo scenarioInfo)
        {
            var testRunner = parentContainer.Resolve<ITestRunner>(); // we need to delay-resolve the test runner to avoid circular dependencies
            var newContext = new ScenarioContext(scenarioInfo, testRunner, parentContainer);
            scenarioContext.Init(newContext);
            ScenarioContext.Current = newContext;
        }

        public void CleanupScenarioContext()
        {
            scenarioContext.Cleanup();
        }

        public void InitializeStepContext(StepInfo stepInfo)
        {
            var newContext = new ScenarioStepContext(stepInfo);
            stepContext.Init(newContext);
            ScenarioStepContext.Current = newContext;
        }

        public void CleanupStepContext()
        {
            stepContext.Cleanup();
            ScenarioStepContext.Current = stepContext.Instance;
        }
        public void Dispose()
        {
            if (featureContext != null)
            {
                featureContext.Dispose();
            }
            if (scenarioContext != null)
            {
                scenarioContext.Dispose();
            }
            if (stepContext != null)
            {
                stepContext.Dispose();
            }
        }
    }




}
