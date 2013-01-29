﻿namespace SimpleInjector.Tests.Unit
{
    using NUnit.Framework;

    [TestFixture]
    public class ConsistencyTests
    {
        [Test]
        public void GetInstance_CalledFromMultipleThreads_Succeeds()
        {
            // Arrange
            bool shouldCall = true;

            var container = new Container();

            // We 'abuse' ResolveUnregisteredType to simulate multi-threading. ResolveUnregisteredType is 
            // called during GetInstance, but before the IInstanceProvider<Concrete> is added to the
            // registrations dictionary.
            container.ResolveUnregisteredType += (s, e) =>
            {
                if (shouldCall)
                {
                    // Prevent stack overflow.
                    shouldCall = false;

                    // Simulate multi-threading.
                    container.GetInstance<Concrete>();
                }
            };

            // Act
            // What we in fact test here is whether the container correctly makes a snapshot of the 
            // registrations dictionary. This call would fail in that case, because the snapshot is needed
            // for consistency.
            container.GetInstance<Concrete>();
            
            // Assert
            Assert.IsFalse(shouldCall, "ResolveUnregisteredType was not called.");
        }

        private class Concrete
        {
        }
    }
}