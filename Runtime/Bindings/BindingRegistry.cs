using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingRegistry
    {
        bool Ready { get; set; }

        IEnumerable<IStepDefinitionBinding> GetConsideredStepDefinitions(StepDefinitionType stepDefinitionType, string stepText = null);
        IEnumerable<IHookBinding> GetHooks(HookType bindingEvent);
        IEnumerable<IStepArgumentTransformationBinding> GetStepTransformations();

        void RegisterStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding);
        void RegisterHookBinding(IHookBinding hookBinding);
        void RegisterStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding);
    }

    internal class BindingRegistry : IBindingRegistry
    {
        private readonly List<IStepDefinitionBinding> stepDefinitions = new List<IStepDefinitionBinding>();
        private readonly List<IStepArgumentTransformationBinding> stepArgumentTransformations = new List<IStepArgumentTransformationBinding>();

        // HACK: WBA: Switched this to ConcurrentDictionary
        private readonly ConcurrentDictionary<HookType, List<IHookBinding>> hooks = new ConcurrentDictionary<HookType, List<IHookBinding>>();

        public bool Ready { get; set; }

        public IEnumerable<IStepDefinitionBinding> GetConsideredStepDefinitions(StepDefinitionType stepDefinitionType, string stepText)
        {
            //TODO: later optimize to return step definitions that has a chance to match to stepText
            return stepDefinitions.Where(sd => sd.StepDefinitionType == stepDefinitionType);
        }

        private List<IHookBinding> GetHookList(HookType bindingEvent)
        {
            List<IHookBinding> list;
            if (!hooks.TryGetValue(bindingEvent, out list))
            {
                list = new List<IHookBinding>();
                hooks.TryAdd(bindingEvent, list);
            }

            return list;
        }

        public IEnumerable<IHookBinding> GetHooks(HookType bindingEvent)
        {
            return GetHookList(bindingEvent);
        }

        public IEnumerable<IStepArgumentTransformationBinding> GetStepTransformations()
        {
            return stepArgumentTransformations;
        }

        public void RegisterStepDefinitionBinding(IStepDefinitionBinding stepDefinitionBinding)
        {
            stepDefinitions.Add(stepDefinitionBinding);
        }

        public void RegisterHookBinding(IHookBinding hookBinding)
        {
            GetHookList(hookBinding.HookType).Add(hookBinding);
        }

        public void RegisterStepArgumentTransformationBinding(IStepArgumentTransformationBinding stepArgumentTransformationBinding)
        {
            stepArgumentTransformations.Add(stepArgumentTransformationBinding);
        }
    }
}