﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StarMeter.Controllers;
using StarMeter.Models;

namespace StarMeter.Tests.Controllers
{
    [TestClass]
    public class AnalyserTest
    {
        private Analyser _analyser;

        [TestInitialize]
        public void Initialize()
        {
            _analyser = new Analyser();
        }

        private static readonly byte[] ExampleCargo =
        {
            0x00, 0xfe, 0xfa, 0x00, 0x17, 0x50, 0xb8, 0xf6, 0xca, 0xd3, 0x9e, 0x3c,
            0x52, 0x74, 0x51, 0x9f, 0xef, 0x80, 0xba, 0xf6, 0x75, 0x92, 0xde, 0xc3, 0xaa, 0x62, 0x5f, 0xaa, 0xf0, 0xde,
            0x46, 0x28, 0x24, 0x7c, 0xff, 0x81, 0xc5, 0xce, 0xa5, 0xfa, 0x59, 0x57, 0x81, 0x49, 0x0c, 0x9d, 0xcd, 0x4a,
            0x9b, 0x7f, 0xbd, 0xf3, 0x70, 0xc9, 0xc0, 0x8a, 0x0f, 0x06, 0x03, 0x15, 0xb0, 0x95, 0x36, 0x13, 0x2d, 0xff,
            0x94, 0x69, 0x1f, 0x88, 0x1d, 0x9f, 0x44, 0x04, 0x26, 0x4c, 0x25, 0xec, 0x14, 0xcf, 0xf5, 0xb1, 0x65, 0x40,
            0xbb, 0x50, 0xf0, 0xa7, 0xb4, 0x27, 0x6d, 0x6b, 0xf2, 0x07, 0x37, 0x0d, 0x4a, 0x8a, 0x51, 0x15, 0x6d, 0xa7,
            0xa7, 0x4d, 0x55, 0x83, 0x97, 0x2e, 0xe3, 0x8a, 0xb0, 0x98, 0xc6, 0xbf, 0xba, 0xc6, 0x9e, 0x50, 0xf6, 0x80,
            0x61, 0x6e, 0xa7, 0x92, 0xfe, 0x5b, 0xd0, 0x7e, 0x41, 0xc5, 0x40, 0x6e, 0xf7, 0x52, 0xcc, 0x6c, 0x52, 0x7c,
            0xdc, 0xd5, 0x8f, 0x9f, 0x29, 0x0b, 0xd5, 0x50, 0xc4, 0x6b, 0x61, 0xf1, 0x5b, 0x7f, 0xe0, 0x82, 0xb8, 0x74,
            0x1c, 0xba, 0x8a, 0xce, 0xdb, 0x57, 0x68, 0x5a, 0x04, 0xb2, 0x13, 0x64, 0x04, 0x96, 0xfb, 0x2b, 0x70, 0x52,
            0x05, 0x92, 0xec, 0x0d, 0x8c, 0x18, 0x4b, 0x5a, 0xa6, 0x0a, 0xf8, 0x0d, 0xa8, 0xf8, 0x94, 0x4c, 0xec, 0x65,
            0xe0, 0xe9, 0xd1, 0xc2, 0xde, 0xef, 0x04, 0x9e, 0x33, 0x7a, 0xfe, 0x17, 0xd0, 0xcc, 0xce, 0x94, 0xd1, 0x9e,
            0x19, 0xb6, 0xa5, 0xb4, 0x5f, 0x8b, 0x70, 0xb4, 0x7f, 0x05, 0xad, 0x38, 0x7e, 0xab, 0x18, 0x22, 0x84, 0x8f,
            0xcb, 0x30, 0x27, 0x80, 0xa7, 0xd0, 0xec, 0x80, 0xf5, 0x35, 0x0b, 0x79, 0x4d, 0xaa, 0x73, 0x2b, 0xb7, 0x26,
            0x0e, 0x69, 0x11, 0x21, 0x46, 0x85, 0xb1, 0xa7, 0xc8
        };

        private static readonly byte[] ExampleAddress = {0x00, 0xfe};
        
        private static readonly Packet Packet1 = new Packet
        {
            PacketId = Guid.NewGuid(),
            DateReceived = DateTime.ParseExact("08-09-2016 15:12:50.081", "dd-MM-yyyy HH:mm:ss.fff", null),
            Cargo = ExampleCargo,
            IsError = true,
            DestinationAddress = ExampleAddress
        };

        private static readonly Packet Packet2 = new Packet
        {
            PacketId = Guid.NewGuid(),
            DateReceived = DateTime.ParseExact("08-09-2016 15:12:52.081", "dd-MM-yyyy HH:mm:ss.fff", null),
            Cargo = ExampleCargo,
            DestinationAddress = ExampleAddress
        };

        private static readonly Packet Packet3 = new Packet
        {
            PacketId = Guid.NewGuid(),
            DateReceived = DateTime.ParseExact("08-09-2016 15:12:54.081", "dd-MM-yyyy HH:mm:ss.fff", null),
            Cargo = ExampleCargo,
            DestinationAddress = ExampleAddress
        };

        private readonly Dictionary<Guid,Packet> _packetDict = new Dictionary<Guid, Packet>
            {
                {Packet1.PacketId, Packet1},
                {Packet2.PacketId, Packet2},
                {Packet3.PacketId, Packet3}
            };

        [TestMethod]
        public void CalculateTotalNoOfDataCharsTest()
        {
            const int expectedResult = 771;
            var actualResult = _analyser.CalculateTotalNoOfDataChars(_packetDict);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void CalculateTotalNumberOfPacketsTest()
        {
            const int expectedResult = 3;
            var actualResult = _analyser.CalculateTotalNoOfPackets(_packetDict);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void CalculateTotalNumberOfErrorPacketsTest()
        {
            const int expectedResult = 1;
            var actualResult = _analyser.CalculateTotalNoOfErrorPackets(_packetDict);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void CalculateErrorRateTest()
        {
            const double expectedResult = 0.33;
            var actualResult = _analyser.CalculateErrorRate(_packetDict);
            Assert.AreEqual(expectedResult, Math.Round(actualResult, 2));
        }

        [TestMethod]
        public void CalculatePacketRateTest()
        {
            const double expectedResult = 0.75;
            var actualResult = _analyser.CalculatePacketRatePerSecond(_packetDict);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void CalculateDataRateTest()
        {
            const double expectedResult = 192.75;
            var actualResult = _analyser.CalculateDataRateBytePerSecond(_packetDict);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _analyser = null;
        }
    }
}
