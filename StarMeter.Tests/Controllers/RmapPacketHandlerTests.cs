﻿using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StarMeter.Controllers;
using StarMeter.Models;

namespace StarMeter.Tests.Controllers
{
    [TestClass]
    public class RmapPacketHandlerTests
    {
        private PacketHandler _packetHandler;
        [TestInitialize]
        public void Initialize()
        {
            _packetHandler = new PacketHandler();
        }

        [TestMethod]
        public void GetDestinationKeyFromRmap()
        {
            byte[] packetData =
            {
                0x2d, 0x01, 0x0c, 0x00, 0x57, 0xff, 0xfb, 0x00, 0x00, 0x00, 0x08, 0x2f, 0xf3, 0xe3, 0x58, 0x99, 0xaa, 0xef, 0xe5, 0x20, 0x24
            };

            var actual = RmapPacketHandler.GetDestinationKey(packetData, _packetHandler.GetLogicalAddressIndex(packetData)); // 3 spaces after logical address index

            byte expected = 0x00;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCheckRmapCrcTwoErrors()
        {
            byte[] packetData =
            {
                0x2d, 0x01, 0x0c, 0x00, 0x57, 0xff, 0xfb, 0x00, 0x00, 0x00, 0x08, 0x2f, 0xf3, 0xe3, 0x58, 0x99, 0xaa, 0xef, 0xe5, 0x20, 0x24
            };

            RmapPacket p = new RmapPacket()
            {
                PacketType = "Read Reply",
                FullPacket = packetData,
            };
            p.Cargo = _packetHandler.GetCargoArray(p, _packetHandler.GetLogicalAddressIndex(packetData));

            Assert.IsFalse(RmapPacketHandler.CheckRmapCrc(p));
        }
        [TestMethod]
        public void TestCheckRmapCrcCargoError()
        {
            byte[] packetData =
            {
                0x2d, 0x01, 0x0c, 0x00, 0x57, 0xff, 0xfb, 0x00, 0x00, 0x00, 0x08, 0x2e, 0xf3, 0xe3, 0x58, 0x99, 0xaa, 0xef, 0xe5, 0x20, 0x24
            };

            RmapPacket p = new RmapPacket()
            {
                PacketType = "Read Reply",
                ProtocolId = 1,
                FullPacket = packetData,
            };
            p.Cargo = _packetHandler.GetCargoArray(p, _packetHandler.GetLogicalAddressIndex(packetData));

            Assert.IsFalse(RmapPacketHandler.CheckRmapCrc(p));
        }

        [TestMethod]
        public void TestCheckRmapCrc()
        {
            byte[] packetData =
            {
                0x57, 0x01, 0x4c, 0x20, 0x2d, 0xff, 0xfb, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x08, 0x3e
            };

            RmapPacket p = new RmapPacket()
            {
                PacketType = "Read",
                FullPacket = packetData,
            };
            p.Cargo = _packetHandler.GetCargoArray(p, _packetHandler.GetLogicalAddressIndex(packetData));

            Assert.IsTrue(RmapPacketHandler.CheckRmapCrc(p));
        }

        [TestMethod]
        public void TestCheckRmapCrcValid()
        {
            byte[] packetData =
            {
                0x2d, 0x01, 0x0c, 0x00, 0x57, 0xff, 0xfb, 0x00, 0x00, 0x00, 0x08, 0x2e, 0xf3, 0xe3, 0x58, 0x99, 0xaa, 0xef, 0xe5, 0x20, 0x25
            };

            RmapPacket p = new RmapPacket()
            {
                ProtocolId = 1,
                PacketType = "Read Reply",
                FullPacket = packetData,
            };
            p.Cargo = _packetHandler.GetCargoArray(p, _packetHandler.GetLogicalAddressIndex(packetData));

            Assert.IsTrue(RmapPacketHandler.CheckRmapCrc(p));
        }

        [TestMethod]
        public void TestCheckRmapCrcHeaderError()
        {
            byte[] packetData =
            {
                0x2d, 0x01, 0x0c, 0x00, 0x57, 0xff, 0xfb, 0x00, 0x00, 0x00, 0x08, 0x2f, 0xf3, 0xe3, 0x58, 0x99, 0xaa, 0xef, 0xe5, 0x20, 0x25
            };

            RmapPacket p = new RmapPacket()
            {
                PacketType = "Read Reply",
                FullPacket = packetData,
            };
            p.Cargo = _packetHandler.GetCargoArray(p, _packetHandler.GetLogicalAddressIndex(packetData));

            Assert.IsFalse(RmapPacketHandler.CheckRmapCrc(p));
        }

        [TestMethod]
        public void GetRmapTypeWrite()
        {
            const string expectedValue = "Write";
            var actual = RmapPacketHandler.GetRmapType(new BitArray(new[] { 0x6c }));
            Assert.AreEqual(expectedValue, actual);
        }

        [TestMethod]
        public void GetRmapTypeWriteReply()
        {
            const string expectedValue = "Write Reply";
            var actual = RmapPacketHandler.GetRmapType(new BitArray(new[] { 0x2c }));
            Assert.AreEqual(expectedValue, actual);
        }

        [TestMethod]
        public void GetRmapTypeRead()
        {
            const string expectedValue = "Read";
            var actual = RmapPacketHandler.GetRmapType(new BitArray(new[] { 0x4c }));
            Assert.AreEqual(expectedValue, actual);
        }

        [TestMethod]
        public void GetRmapTypeReadReply()
        {
            const string expectedValue = "Read Reply";
            var actual = RmapPacketHandler.GetRmapType(new BitArray(new[] { 0x0c }));
            Assert.AreEqual(expectedValue, actual);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _packetHandler = null;
        }

    }
}