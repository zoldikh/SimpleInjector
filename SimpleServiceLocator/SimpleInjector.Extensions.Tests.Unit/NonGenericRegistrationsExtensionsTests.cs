﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleInjector.Extensions.Tests.Unit
{
    /// <content>
    /// Tests for the NonGenericRegistrationsExtensions class.
    /// </content>
    [TestClass]
    public partial class NonGenericRegistrationsExtensionsTests
    {
        public interface IPublicService
        {
        }

        private interface IPublicServiceEx : IPublicService
        {
        }

        private interface IInternalService
        {
        }

        private interface IGenericDictionary<T> : IDictionary
        {
        }

        [TestMethod]
        public void RegisterSingleByInstance_ValidRegistration_GetInstanceReturnsExpectedInstance()
        {
            // Arrange
            var container = new Container();

            object impl = new InternalImplOfPublicService(null);

            // Act
            container.RegisterSingle(typeof(IPublicService), impl);

            // Assert
            Assert.AreEqual(impl, container.GetInstance<IPublicService>(),
                "GetInstance should return the instance registered using RegisterSingle.");
        }

        [TestMethod]
        public void RegisterSingleByInstance_ValidRegistration_GetInstanceAlwaysReturnsSameInstance()
        {
            // Arrange
            var container = new Container();

            object impl = new InternalImplOfPublicService(null);

            // Act
            container.RegisterSingle(typeof(IPublicService), impl);

            var instance1 = container.GetInstance<IPublicService>();
            var instance2 = container.GetInstance<IPublicService>();

            // Assert
            Assert.AreEqual(instance1, instance2, "RegisterSingle should register singleton.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterSingleByInstance_ImplementationNoDescendantOfServiceType_ThrowsException()
        {
            // Arrange
            var container = new Container();

            object impl = new List<int>();

            // Act
            container.RegisterSingle(typeof(IPublicService), impl);
        }

        [TestMethod]
        public void RegisterSingleByInstance_InstanceOfSameTypeAsService_Succeeds()
        {
            // Arrange
            var container = new Container();

            object impl = new List<int>();

            // Act
            container.RegisterSingle(impl.GetType(), impl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingleByInstance_NullContainer_ThrowsException()
        {
            // Arrange
            Container invalidContainer = null;

            Type validServiceType = typeof(IPublicService);
            object validInstance = new InternalImplOfPublicService(null);

            // Act
            invalidContainer.RegisterSingle(validServiceType, validInstance);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingleByInstance_NullServiceType_ThrowsException()
        {
            // Arrange
            var container = new Container();

            Type invalidServiceType = null;
            object validInstance = new InternalImplOfPublicService(null);

            // Act
            container.RegisterSingle(invalidServiceType, validInstance);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingleByInstance_NullInstance_ThrowsException()
        {
            // Arrange
            var container = new Container();

            Type validServiceType = typeof(IPublicService);
            object invalidInstance = null;

            // Act
            container.RegisterSingle(validServiceType, invalidInstance);
        }

        [TestMethod]
        public void RegisterSingleByType_ValidRegistration_GetInstanceReturnsExpectedType()
        {
            // Arrange
            var container = new Container();

            // Act
            container.RegisterSingle(typeof(IPublicService), typeof(PublicServiceImpl));

            // Assert
            Assert.IsInstanceOfType(container.GetInstance<IPublicService>(), typeof(PublicServiceImpl));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingleByType_NullServiceType_ThrowsException()
        {
            // Arrange
            var container = new Container();

            Type invalidServiceType = null;

            // Act
            container.RegisterSingle(invalidServiceType, typeof(InternalImplOfPublicService));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingleByType_NullImplementationType_ThrowsException()
        {
            // Arrange
            var container = new Container();

            Type invalidImplementationType = null;

            // Act
            container.RegisterSingle(typeof(IPublicService), invalidImplementationType);
        }

        [TestMethod]
        public void RegisterSingleByType_ValidRegistration_GetInstanceAlwaysReturnsSameInstance()
        {
            // Arrange
            var container = new Container();

            object impl = new InternalImplOfPublicService(null);

            // Act
            container.RegisterSingle(typeof(IPublicService), typeof(PublicServiceImpl));
            
            var instance1 = container.GetInstance<IPublicService>();
            var instance2 = container.GetInstance<IPublicService>();

            // Assert
            Assert.AreEqual(instance1, instance2, "RegisterSingle should register singleton.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterSingleByType_InstanceThatDoesNotImplementServiceType_Fails()
        {
            // Arrange
            var container = new Container();

            // Act
            container.RegisterSingle(typeof(IPublicService), typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterSingleByType_ImplementationIsServiceType_Fails()
        {
            // Arrange
            var container = new Container();

            // Act
            container.RegisterSingle(typeof(InternalImplOfPublicService), typeof(InternalImplOfPublicService));
        }

        [TestMethod]
        public void RegisterSingleByFunc_ValidArguments_GetInstanceAlwaysReturnsSameInstance()
        {
            // Arrange
            var container = new Container();

            Func<object> instanceCreator = () => new InternalImplOfPublicService(null);

            // Act
            container.RegisterSingle(typeof(IPublicService), instanceCreator);

            var instance1 = container.GetInstance<IPublicService>();
            var instance2 = container.GetInstance<IPublicService>();

            // Assert
            Assert.AreEqual(instance1, instance2, "RegisterSingle should register singleton.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingleByFunc_NullContainer_ThrowsException()
        {
            // Arrange
            Container invalidContainer = null;

            Type validServiceType = typeof(IPublicService);
            Func<object> validInstanceCreator = () => new InternalImplOfPublicService(null);

            // Act
            invalidContainer.RegisterSingle(validServiceType, validInstanceCreator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingleByFunc_NullServiceType_ThrowsException()
        {
            // Arrange
            var container = new Container();

            Type invalidServiceType = null;
            Func<object> validInstanceCreator = () => new InternalImplOfPublicService(null);

            // Act
            container.RegisterSingle(invalidServiceType, validInstanceCreator);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterSingleByFunc_NullInstanceCreator_ThrowsException()
        {
            // Arrange
            var container = new Container();

            Type validServiceType = typeof(IPublicService);
            Func<object> invalidInstanceCreator = null;

            // Act
            container.RegisterSingle(validServiceType, invalidInstanceCreator);
        }

        [TestMethod]
        public void RegisterSingle_OpenGenericServiceType_ThrowsExcpectedException()
        {
            // Arrange
            var container = new Container();

            try
            {
                // Act
                container.RegisterSingle(typeof(IDictionary<,>), typeof(Dictionary<,>));

                // Assert
                Assert.Fail("Exception expected.");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The supplied type ") &&
                    ex.Message.Contains(" is an open generic type."), "Actual: " + ex.Message);
            }
        }

        [TestMethod]
        public void RegisterAll_WithOpenGenericType_FailsWithExpectedExceptionMessage()
        {
            // Arrange
            var container = new Container();

            try
            {
                // Act
                container.RegisterAll<IDictionary>(new[] { typeof(IGenericDictionary<>) });

                // Assert
                Assert.Fail("Exception expected.");
            }
            catch (ArgumentException ex)
            {
                Assert.IsTrue(ex.Message.Contains("open generic type"), "Actual: " + ex.Message);
            }
        }

        [TestMethod]
        public void RegisterAll_WithValidCollectionOfServices_Succeeds()
        {
            // Arrange
            var instance = new InternalImplOfPublicService(null);

            var container = new Container();

            // Act
            container.RegisterAll(typeof(IPublicService), new IPublicService[] { instance });

            // Assert
            var instances = container.GetAllInstances<IPublicService>();

            Assert.AreEqual(1, instances.Count());
            Assert.AreEqual(instance, instances.First());
        }

        [TestMethod]
        public void RegisterAll_WithValidObjectCollectionOfServices_Succeeds()
        {
            // Arrange
            var instance = new InternalImplOfPublicService(null);

            var container = new Container();

            // Act
            container.RegisterAll(typeof(IPublicService), new object[] { instance });

            // Assert
            var instances = container.GetAllInstances<IPublicService>();

            Assert.AreEqual(1, instances.Count());
            Assert.AreEqual(instance, instances.First());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterAll_NullContainer_ThrowsException()
        {
            // Arrange
            var instance = new InternalImplOfPublicService(null);

            Container invalidContainer = null;

            // Act
            invalidContainer.RegisterAll(typeof(IPublicService), new IPublicService[] { instance });
        }

        [TestMethod]
        public void RegisterAll_WithValidCollectionOfImplementations_Succeeds()
        {
            // Arrange
            var instance = new InternalImplOfPublicService(null);

            var container = new Container();

            // Act
            container.RegisterAll(typeof(IPublicService), new InternalImplOfPublicService[] { instance });

            // Assert
            var instances = container.GetAllInstances<IPublicService>();

            Assert.AreEqual(1, instances.Count());
            Assert.AreEqual(instance, instances.First());
        }

        [TestMethod]
        public void RegisterByType_ValidArguments_Succeeds()
        {
            // Arrange
            var container = new Container();

            Type validServiceType = typeof(IPublicService);
            Type validImplementation = typeof(PublicServiceImpl);

            // Act
            container.Register(validServiceType, validImplementation);

            // Assert
            var instance = container.GetInstance(validServiceType);

            Assert.IsInstanceOfType(instance, validImplementation);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterByType_NullContainer_ThrowsException()
        {
            // Arrange
            Container invalidContainer = null;

            Type validServiceType = typeof(IPublicService);
            Type validImplementation = typeof(InternalImplOfPublicService);

            // Act
            invalidContainer.Register(validServiceType, validImplementation);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterByType_NullServiceType_ThrowsException()
        {
            // Arrange
            var container = new Container();

            Type invalidServiceType = null;
            Type validImplementation = typeof(InternalImplOfPublicService);

            // Act
            container.Register(invalidServiceType, validImplementation);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterByType_NullImplementation_ThrowsException()
        {
            // Arrange
            var container = new Container();

            Type validServiceType = typeof(IPublicService);
            Type invalidImplementation = null;

            // Act
            container.Register(validServiceType, invalidImplementation);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterByType_ServiceTypeAndImplementationSameType_ThrowsException()
        {
            // Arrange
            var container = new Container();

            Type implementation = typeof(InternalImplOfPublicService);

            // Act
            container.Register(implementation, implementation);
        }

        [TestMethod]
        public void RegisterByFunc_ValidArguments_Succeeds()
        {
            // Arrange
            var container = new Container();

            Type validServiceType = typeof(IPublicService);
            Func<object> instanceCreator = () => new InternalImplOfPublicService(null);

            // Act
            container.Register(validServiceType, instanceCreator);

            // Assert
            var instance = container.GetInstance(validServiceType);

            Assert.IsInstanceOfType(instance, typeof(InternalImplOfPublicService));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterByFunc_NullInstanceCreator_ThrowsException()
        {
            // Arrange
            var container = new Container();

            Type validServiceType = typeof(IPublicService);
            Func<object> invalidInstanceCreator = null;

            // Act
            container.Register(validServiceType, invalidInstanceCreator);
        }

        [TestMethod]
        public void RegisterAll_WithValidListOfTypes_Succeeds()
        {
            // Arrange
            var container = new Container();

            // Act
            // IServiceEx is a valid registration, because it could be registered.
            container.RegisterAll<IPublicService>(new[] { typeof(InternalImplOfPublicService), typeof(IPublicServiceEx) });
        }

        [TestMethod]
        public void RegisterAll_WithValidEnumeableOfTypes_Succeeds()
        {
            // Arrange
            IEnumerable<Type> types = new[] { typeof(InternalImplOfPublicService) };

            var container = new Container();

            // Act
            container.RegisterAll<IPublicService>(types);
        }

        [TestMethod]
        public void RegisterAll_WithValidParamListOfTypes_Succeeds()
        {
            // Arrange
            Type[] types = new[] { typeof(InternalImplOfPublicService) };

            var container = new Container();

            // Act
            container.RegisterAll<IPublicService>(types);
        }

        [TestMethod]
        public void GetAllInstances_RegisteringValidListOfTypesWithRegisterAll_ReturnsExpectedList()
        {
            // Arrange
            var container = new Container();

            container.RegisterAll<IPublicService>(new[] { typeof(PublicServiceImpl) });

            // Act
            container.Verify();

            var list = container.GetAllInstances<IPublicService>().ToArray();

            // Assert
            Assert.AreEqual(1, list.Length);
            Assert.IsInstanceOfType(list[0], typeof(PublicServiceImpl));
        }

        [TestMethod]
        public void Verify_RegisterAllCalledWithUnregisteredType_ThrowsExpectedException()
        {
            // Arrange
            string expectedException = 
                string.Format("No registration for type {0} could be found.", typeof(IPublicServiceEx).Name);

            var container = new Container();

            var types = new[] { typeof(PublicServiceImpl), typeof(IPublicServiceEx) };

            container.RegisterAll<IPublicService>(types);

            try
            {
                // Act
                container.Verify();

                Assert.Fail("Exception expected.");
            }
            catch (InvalidOperationException ex)
            {
                string actualMessage = ex.Message.Replace(typeof(IPublicServiceEx).FullName, typeof(IPublicServiceEx).Name);

                AssertThat.StringContains(expectedException, actualMessage);
            }
        }

        [TestMethod]
        public void RegisterAll_WithInvalidListOfTypes_ThrowsExceptionWithExpectedMessage()
        {
            // Arrange
            string expectedMessage = string.Format(
                "The supplied type '{0}' does not implement '{0}'.", 
                typeof(IPublicService).Name);

            var container = new Container();

            try
            {
                // Act
                // Cannot register a IService, because this would cause a recursive dependency.
                container.RegisterAll<IPublicService>(new[] { typeof(InternalImplOfPublicService), typeof(IPublicService) });

                Assert.Fail("Exception expected.");
            }
            catch (ArgumentException ex)
            {
                string actualMessage = ex.Message.Replace(typeof(IPublicService).FullName, typeof(IPublicService).Name);

                AssertThat.StringContains(expectedMessage, actualMessage);
            }
        }

        public sealed class Dependency
        {
        }

        public sealed class PublicServiceImpl : IPublicService
        {
            public PublicServiceImpl(Dependency dependency)
            {
            }
        }

        private sealed class InternalImplOfPublicService : IPublicService
        {
            public InternalImplOfPublicService(Dependency dependency)
            {
            }
        }

        private sealed class InternalServiceImpl : IInternalService
        {
            public InternalServiceImpl(Dependency dependency)
            {
            }
        }
    }
}