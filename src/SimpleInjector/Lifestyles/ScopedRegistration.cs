﻿#region Copyright Simple Injector Contributors
/* The Simple Injector is an easy-to-use Inversion of Control library for .NET
 * 
 * Copyright (c) 2014-2016 Simple Injector Contributors
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
 * associated documentation files (the "Software"), to deal in the Software without restriction, including 
 * without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the 
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial 
 * portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
 * LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO 
 * EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER 
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE 
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

namespace SimpleInjector.Lifestyles
{
    using System;
    using System.Linq.Expressions;

    internal sealed class ScopedRegistration<TImplementation> : Registration
        where TImplementation : class
    {
        private readonly Func<TImplementation> userSuppliedInstanceCreator;

        private Func<Scope> scopeFactory;

        internal ScopedRegistration(
            ScopedLifestyle lifestyle, Container container, Func<TImplementation> instanceCreator)
            : this(lifestyle, container)
        {
            this.userSuppliedInstanceCreator = instanceCreator;
        }

        internal ScopedRegistration(ScopedLifestyle lifestyle, Container container)
            : base(lifestyle, container)
        {
        }

        public override Type ImplementationType => typeof(TImplementation);
        public new ScopedLifestyle Lifestyle => (ScopedLifestyle)base.Lifestyle;

        internal Func<TImplementation> InstanceCreator { get; private set; }

        public override Expression BuildExpression()
        {
            if (this.InstanceCreator == null)
            {
                this.scopeFactory = this.Lifestyle.CreateCurrentScopeProvider(this.Container);

                this.InstanceCreator = this.BuildInstanceCreator();
            }

            return Expression.Call(
                instance: Expression.Constant(this),
                method: this.GetType().GetMethod(nameof(this.GetInstance)));
        }

        // This method needs to be public, because the BuildExpression methods build a
        // MethodCallExpression using this method, and this would fail in partial trust when the 
        // method is not public.
        // Simple Injector does some aggressive optimizations for scoped lifestyles and this method will
        // is most cases not be called. It will however be called when the expression that is built by
        // this instance will get compiled by someone else than the core library. That's why this method
        // is still important.
        public TImplementation GetInstance() => Scope.GetInstance(this, this.scopeFactory());

        private Func<TImplementation> BuildInstanceCreator() =>
            this.userSuppliedInstanceCreator != null
                ? this.BuildTransientDelegate(this.userSuppliedInstanceCreator)
                : (Func<TImplementation>)this.BuildTransientDelegate();
    }
}