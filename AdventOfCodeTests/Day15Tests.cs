using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdventOfCode;

namespace AdventOfCodeTests
{
    [TestClass]
    public class Day15Tests
    {
        [TestMethod]
        public void Day15Test1()
        {
            var result = Day15.ProcessGame("TestFiles/Day15/Test1.txt");
            Assert.AreEqual(47, result.fullyFinishedRounds);
            Assert.AreEqual(590, result.totalHitpoints);
        }

        [TestMethod]
        public void Day15Test2()
        {
            var result = Day15.ProcessGame("TestFiles/Day15/Test2.txt");
            Assert.AreEqual(37, result.fullyFinishedRounds);
            Assert.AreEqual(982, result.totalHitpoints);
        }

        [TestMethod]
        public void Day15Test3()
        {
            var result = Day15.ProcessGame("TestFiles/Day15/Test3.txt");
            Assert.AreEqual(46, result.fullyFinishedRounds);
            Assert.AreEqual(859, result.totalHitpoints);
        }

        [TestMethod]
        public void Day15Test4()
        {
            var result = Day15.ProcessGame("TestFiles/Day15/Test4.txt");
            Assert.AreEqual(35, result.fullyFinishedRounds);
            Assert.AreEqual(793, result.totalHitpoints);
        }

        [TestMethod]
        public void Day15Test5()
        {
            var result = Day15.ProcessGame("TestFiles/Day15/Test5.txt");
            Assert.AreEqual(54, result.fullyFinishedRounds);
            Assert.AreEqual(536, result.totalHitpoints);
        }

        [TestMethod]
        public void Day15Test6()
        {
            var result = Day15.ProcessGame("TestFiles/Day15/Test6.txt",34);
            Assert.AreEqual(30, result.fullyFinishedRounds);
            Assert.AreEqual(38, result.totalHitpoints);
            Assert.IsFalse(result.anyElfDied);
        }

        [TestMethod]
        public void Day15Test7()
        {
            var result = Day15.ProcessGame("Day15/Input.txt");
            Assert.AreEqual(82, result.fullyFinishedRounds);
            Assert.AreEqual(2606, result.totalHitpoints);
        }

        [TestMethod]
        public void Day15Test8()
        {
            var result = Day15.ProcessGame("Day15/Input.txt", 23);
            Assert.AreEqual(37, result.fullyFinishedRounds);
            Assert.AreEqual(1424, result.totalHitpoints);
            Assert.IsFalse(result.anyElfDied);
        }
    }
}
