using AdventOfCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AdventOfCodeTests
{
    [TestClass]
    public class Day16Tests
    {
        [TestMethod]
        public void Day16Test1()
        {
            var startRoom = Day20.CreateMap("^WNE$");
            var allRooms = startRoom.AllRooms.ToList();
            Assert.AreEqual(3, allRooms.Max(x => x.DistanceFromStart));
        }

        [TestMethod]
        public void Day16Test2()
        {
            var startRoom = Day20.CreateMap("^ENWWW(NEEE|SSE(EE|N))$");
            var allRooms = startRoom.AllRooms.ToList();
            Assert.AreEqual(10, allRooms.Max(x => x.DistanceFromStart));
        }

        [TestMethod]
        public void Day16Test3()
        {
            var startRoom = Day20.CreateMap("^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$");
            var allRooms = startRoom.AllRooms.ToList();
            Assert.AreEqual(18, allRooms.Max(x => x.DistanceFromStart));
        }

        [TestMethod]
        public void Day16Test4()
        {
            var startRoom = Day20.CreateMap("^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$");
            var allRooms = startRoom.AllRooms.ToList();
            Assert.AreEqual(23, allRooms.Max(x => x.DistanceFromStart));
        }

        [TestMethod]
        public void Day16Test5()
        {
            var startRoom = Day20.CreateMap("^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$");
            var allRooms = startRoom.AllRooms.ToList();
            Assert.AreEqual(31, allRooms.Max(x => x.DistanceFromStart));
        }
    }
}
