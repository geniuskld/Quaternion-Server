﻿
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Client
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        public void CreateInstance()
        {
          var game = new ClientGamePlugin.GamePlugin();
        }
    }
}
