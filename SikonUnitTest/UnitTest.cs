
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
        /// <summary>
        /// Denne metode tester om der er forbindelse om UWP-appen har dataforbindelse
        /// TryOpenConn er en metode fra der prøver at åbne en forbindelse til dataen
        /// </summary>
        /// <returns>Forbindelse til data</returns>
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
