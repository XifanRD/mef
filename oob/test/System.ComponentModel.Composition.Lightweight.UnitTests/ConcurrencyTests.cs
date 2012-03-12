﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    [TestClass]
    public class ConcurrencyTests : ContainerTests
    {
        [Export, Shared]
        public class PausesDuringActivation : IPartImportsSatisfiedNotification
        {
            public bool IsActivationComplete { get; set; }

            public void OnImportsSatisfied()
            {
                Task.Delay(200).Wait();
                IsActivationComplete = true;
            }
        }
        
        // This does not test the desired behaviour deterministically,
        // but is close enough to be repeatable at least on my machine :)
        [TestMethod]
        public void SharedInstancesAreNotVisibleUntilActivationCompletes()
        {
            var c = CreateContainer(typeof(PausesDuringActivation));
            Task.Run(() => c.GetExport<PausesDuringActivation>());
            Task.Delay(50).Wait();
            var pda = c.GetExport<PausesDuringActivation>();
            Assert.IsTrue(pda.IsActivationComplete);
        }
    }
}
