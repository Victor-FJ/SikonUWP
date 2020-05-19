
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelLibrary.Model;
using SikonUWP.Common;
using SikonUWP.Handlers;
using SikonUWP.Model;
using SikonUWP.Persistency;

namespace SikonUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestConnection()
        {
            //Arrange
            //Der er intet Arrange
            //Unittest havde brug for en connection fra et andet sted.
            //Derfor måtte jeg lave en notesblock hvor jeg lagde connectionString ind i.
            
            //Act
            bool ok = await PersistencyManager.TryOpenConn();
            
            //Assert
            Assert.IsTrue(ok);


        }
    }
}
