﻿namespace SimpleInjector.CodeSamples
{
    // https://simpleinjector.readthedocs.io/en/latest/extensibility.html#overriding-constructor-resolution-behavior
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using SimpleInjector.Advanced;

    // Mimics the constructor resolution behavior of Autofac, Ninject and Castle Windsor.
    // Register this as follows:
    // container.Options.ConstructorResolutionBehavior =
    //     new MostResolvableParametersConstructorResolutionBehavior(container);
    public class MostResolvableParametersConstructorResolutionBehavior : IConstructorResolutionBehavior
    {
        private readonly Container container;

        public MostResolvableParametersConstructorResolutionBehavior(Container container)
        {
            this.container = container;
        }

        private bool IsCalledDuringRegistrationPhase
        {
            [DebuggerStepThrough]
            get { return !this.container.IsLocked(); }
        }

        [DebuggerStepThrough]
        public ConstructorInfo GetConstructor(Type implementationType)
        {
            var constructor = this.GetConstructors(implementationType).FirstOrDefault();

            if (constructor != null)
            {
                return constructor;
            }

            throw new ActivationException(BuildExceptionMessage(implementationType));
        }

        // We prevent calling GetRegistration during the registration phase, because at this point not
        // all dependencies might be registered, and calling GetRegistration would lock the container,
        // making it impossible to do other registrations.
        private IEnumerable<ConstructorInfo> GetConstructors(Type implementation) =>
            from ctor in implementation.GetConstructors()
            let parameters = ctor.GetParameters()
            where this.IsCalledDuringRegistrationPhase
                || implementation.GetConstructors().Length == 1
                || ctor.GetParameters().All(this.CanBeResolved)
            orderby parameters.Length descending
            select ctor;

        private bool CanBeResolved(ParameterInfo parameter) =>
            this.GetInstanceProducerFor(new InjectionConsumerInfo(parameter)) != null;

        private InstanceProducer GetInstanceProducerFor(InjectionConsumerInfo info) =>
            this.container.Options.DependencyInjectionBehavior.GetInstanceProducer(info, false);

        private static string BuildExceptionMessage(Type type) =>
            !type.GetConstructors().Any()
                ? TypeShouldHaveAtLeastOnePublicConstructor(type)
                : TypeShouldHaveConstructorWithResolvableTypes(type);

        private static string TypeShouldHaveAtLeastOnePublicConstructor(Type type) =>
            string.Format(CultureInfo.InvariantCulture,
                "For the container to be able to create {0}, it should contain at least one public " +
                "constructor.", type.ToFriendlyName());

        private static string TypeShouldHaveConstructorWithResolvableTypes(Type type) =>
            string.Format(CultureInfo.InvariantCulture,
                "For the container to be able to create {0}, it should contain a public constructor " +
                "that only contains parameters that can be resolved.", type.ToFriendlyName());
    }
}