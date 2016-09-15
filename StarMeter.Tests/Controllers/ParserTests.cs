﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StarMeter.Controllers;
using StarMeter.Models;
using Moq;
using StarMeter.Interfaces;


namespace StarMeter.Tests.Controllers
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void PassingCorrectStringReturnsDateTimeTest()
        {
            var parser = new Parser();
            const string stringDateTime = "08-09-2016 14:27:53.726";
            var result = parser.ParseDateTime(stringDateTime);

            Assert.IsInstanceOfType(result, typeof(DateTime));
            Assert.AreEqual(result, new DateTime(2016, 09, 08, 14, 27, 53, 726));
        }


        [TestMethod]
        public void NoErrorOnePacketReturnsListWithOneElement()
        {
            var readerMock = new Mock<IStreamReader>();

            var stockResponses = new Queue<string>();
            stockResponses.Enqueue("08-09-2016 18:45:04.045");
            stockResponses.Enqueue("1");
            stockResponses.Enqueue("");
            stockResponses.Enqueue("08-09-2016 15:12:50.081");
            stockResponses.Enqueue("P");
            stockResponses.Enqueue(@"00 fe fa 00 17 50 b8 f6 ca d3 9e 3c 52 74 51 9f ef 80 ba f6 75 92 de c3 aa 62 5f aa f0 de 46 28 24 7c ff 81 c5 ce a5 fa 59 57 81 49 0c 9d cd 4a 9b 7f bd f3 70 c9 c0 8a 0f 06 03 15 b0 95 36 13 2d ff 94 69 1f 88 1d 9f 44 04 26 4c 25 ec 14 cf f5 b1 65 40 bb 50 f0 a7 b4 27 6d 6b f2 07 37 0d 4a 8a 51 15 6d a7 a7 4d 55 83 97 2e e3 8a b0 98 c6 bf ba c6 9e 50 f6 80 61 6e a7 92 fe 5b d0 7e 41 c5 40 6e f7 52 cc 6c 52 7c dc d5 8f 9f 29 0b d5 50 c4 6b 61 f1 5b 7f e0 82 b8 74 1c ba 8a ce db 57 68 5a 04 b2 13 64 04 96 fb 2b 70 52 05 92 ec 0d 8c 18 4b 5a a6 0a f8 0d a8 f8 94 4c ec 65 e0 e9 d1 c2 de ef 04 9e 33 7a fe 17 d0 cc ce 94 d1 9e 19 b6 a5 b4 5f 8b 70 b4 7f 05 ad 38 7e ab 18 22 84 8f cb 30 27 80 a7 d0 ec 80 f5 35 0b 79 4d aa 73 2b b7 26 0e 69 11 21 46 85 b1 a7 c8");
            stockResponses.Enqueue("EOP");
            stockResponses.Enqueue("");
            stockResponses.Enqueue("08-09-2016 15:13:55.193");
            stockResponses.Enqueue(null);
            readerMock.Setup(t => t.ReadLine()).Returns(stockResponses.Dequeue);

            readerMock.SetupSequence(t => t.Peek()).Returns(5).Returns(-1);

            var parser = new Parser();
            var result = parser.ParsePackets(readerMock.Object);
            var expected = new Dictionary<Guid, Packet>();
            var packetId = Guid.NewGuid();
            var packet1 = new Packet
            {
                IsError = false,
                PacketId = packetId,
                DateRecieved = DateTime.ParseExact("08-09-2016 15:12:50.081", "dd-MM-yyyy HH:mm:ss.fff", null),
                Cargo =
                    @"00 fe fa 00 17 50 b8 f6 ca d3 9e 3c 52 74 51 9f ef 80 ba f6 75 92 de c3 aa 62 5f aa f0 de 46 28 24 7c ff 81 c5 ce a5 fa 59 57 81 49 0c 9d cd 4a 9b 7f bd f3 70 c9 c0 8a 0f 06 03 15 b0 95 36 13 2d ff 94 69 1f 88 1d 9f 44 04 26 4c 25 ec 14 cf f5 b1 65 40 bb 50 f0 a7 b4 27 6d 6b f2 07 37 0d 4a 8a 51 15 6d a7 a7 4d 55 83 97 2e e3 8a b0 98 c6 bf ba c6 9e 50 f6 80 61 6e a7 92 fe 5b d0 7e 41 c5 40 6e f7 52 cc 6c 52 7c dc d5 8f 9f 29 0b d5 50 c4 6b 61 f1 5b 7f e0 82 b8 74 1c ba 8a ce db 57 68 5a 04 b2 13 64 04 96 fb 2b 70 52 05 92 ec 0d 8c 18 4b 5a a6 0a f8 0d a8 f8 94 4c ec 65 e0 e9 d1 c2 de ef 04 9e 33 7a fe 17 d0 cc ce 94 d1 9e 19 b6 a5 b4 5f 8b 70 b4 7f 05 ad 38 7e ab 18 22 84 8f cb 30 27 80 a7 d0 ec 80 f5 35 0b 79 4d aa 73 2b b7 26 0e 69 11 21 46 85 b1 a7 c8"
                        .Split(' ')
            };
            expected.Add(packetId, packet1);
           
            var comparativePacket = (from a in result.Values
                where a.DateRecieved == packet1.DateRecieved
                select a).First();


            Assert.AreEqual(expected[packetId].Cargo.Length, comparativePacket.Cargo.Length);
            Assert.AreEqual(expected[packetId].IsError, comparativePacket.IsError);
        }

        [TestMethod]
        public void PacketCollectionWithErrorParsed()
        {
            var readerMock = new Mock<IStreamReader>();

            var stockResponses = new Queue<string>();
            stockResponses.Enqueue("08-09-2016 18:45:04.045");
            stockResponses.Enqueue("1");
            stockResponses.Enqueue("");
            stockResponses.Enqueue("08-09-2016 15:12:50.081");
            stockResponses.Enqueue("P");
            stockResponses.Enqueue(@"00 fe fa 00 17 50 b8 f6 ca d3 9e 3c 52 74 51 9f ef 80 ba f6 75 92 de c3 aa 62 5f aa f0 de 46 28 24 7c ff 81 c5 ce a5 fa 59 57 81 49 0c 9d cd 4a 9b 7f bd f3 70 c9 c0 8a 0f 06 03 15 b0 95 36 13 2d ff 94 69 1f 88 1d 9f 44 04 26 4c 25 ec 14 cf f5 b1 65 40 bb 50 f0 a7 b4 27 6d 6b f2 07 37 0d 4a 8a 51 15 6d a7 a7 4d 55 83 97 2e e3 8a b0 98 c6 bf ba c6 9e 50 f6 80 61 6e a7 92 fe 5b d0 7e 41 c5 40 6e f7 52 cc 6c 52 7c dc d5 8f 9f 29 0b d5 50 c4 6b 61 f1 5b 7f e0 82 b8 74 1c ba 8a ce db 57 68 5a 04 b2 13 64 04 96 fb 2b 70 52 05 92 ec 0d 8c 18 4b 5a a6 0a f8 0d a8 f8 94 4c ec 65 e0 e9 d1 c2 de ef 04 9e 33 7a fe 17 d0 cc ce 94 d1 9e 19 b6 a5 b4 5f 8b 70 b4 7f 05 ad 38 7e ab 18 22 84 8f cb 30 27 80 a7 d0 ec 80 f5 35 0b 79 4d aa 73 2b b7 26 0e 69 11 21 46 85 b1 a7 c8");
            stockResponses.Enqueue("EOP");
            stockResponses.Enqueue("");
            stockResponses.Enqueue("08-09-2016 15:53:23.690");
            stockResponses.Enqueue("E");
            stockResponses.Enqueue("Disconnect");
            stockResponses.Enqueue("");
            stockResponses.Enqueue("08-09-2016 15:13:55.193");
            stockResponses.Enqueue(null);
            readerMock.Setup(t => t.ReadLine()).Returns(stockResponses.Dequeue);

            readerMock.SetupSequence(t => t.Peek()).Returns(5).Returns(5).Returns(-1);

            var parser = new Parser();
            var result = parser.ParsePackets(readerMock.Object);
            var expected = new Dictionary<Guid, Packet>();
            var packet1 = new Packet
            {
                IsError = false,
                DateRecieved = DateTime.ParseExact("08-09-2016 15:12:50.081", "dd-MM-yyyy HH:mm:ss.fff", null),
                Cargo =
                    @"00 fe fa 00 17 50 b8 f6 ca d3 9e 3c 52 74 51 9f ef 80 ba f6 75 92 de c3 aa 62 5f aa f0 de 46 28 24 7c ff 81 c5 ce a5 fa 59 57 81 49 0c 9d cd 4a 9b 7f bd f3 70 c9 c0 8a 0f 06 03 15 b0 95 36 13 2d ff 94 69 1f 88 1d 9f 44 04 26 4c 25 ec 14 cf f5 b1 65 40 bb 50 f0 a7 b4 27 6d 6b f2 07 37 0d 4a 8a 51 15 6d a7 a7 4d 55 83 97 2e e3 8a b0 98 c6 bf ba c6 9e 50 f6 80 61 6e a7 92 fe 5b d0 7e 41 c5 40 6e f7 52 cc 6c 52 7c dc d5 8f 9f 29 0b d5 50 c4 6b 61 f1 5b 7f e0 82 b8 74 1c ba 8a ce db 57 68 5a 04 b2 13 64 04 96 fb 2b 70 52 05 92 ec 0d 8c 18 4b 5a a6 0a f8 0d a8 f8 94 4c ec 65 e0 e9 d1 c2 de ef 04 9e 33 7a fe 17 d0 cc ce 94 d1 9e 19 b6 a5 b4 5f 8b 70 b4 7f 05 ad 38 7e ab 18 22 84 8f cb 30 27 80 a7 d0 ec 80 f5 35 0b 79 4d aa 73 2b b7 26 0e 69 11 21 46 85 b1 a7 c8"
                        .Split(' ')
            };
            var packet2 = new Packet
            {
                DateRecieved = DateTime.ParseExact("08-09-2016 15:53:23.690", "dd-MM-yyyy HH:mm:ss.fff", null),
                IsError = true
            };
            var packetId = Guid.NewGuid();
            expected.Add(packetId, packet1);
            packetId = Guid.NewGuid();
            expected.Add(packetId, packet2);
            var comparativePacket = (from a in result.Values
                                    where a.DateRecieved == packet2.DateRecieved
                                    select a).First();

            Assert.AreEqual(expected.Count, result.Count);
            Assert.AreEqual(expected[packetId].DateRecieved, comparativePacket.DateRecieved);
            Assert.AreEqual(expected[packetId].IsError, comparativePacket.IsError);
        }
         
        [TestMethod]
        public void NoneErrorParsesSuccesfully()
        {
            var readerMock = new Mock<IStreamReader>();

            var stockResponses = new Queue<string>();
            stockResponses.Enqueue("08-09-2016 18:45:04.045");
            stockResponses.Enqueue("1");
            stockResponses.Enqueue("");
            stockResponses.Enqueue("08-09-2016 15:12:55.051");
            stockResponses.Enqueue("P");
            stockResponses.Enqueue(@"01 00 fe fa 53 2d e5 81 d1 27 41 d5 e5 fe c6 67 05 54 dd 12 75 f0 86 e4 dd 6c 3f 71 49 2d 29 6c 73 99 66 78 45 83 c5 3b 9a ea a1 b4 45 e4 06 cf 54 d5 16 37 96 e4 ab 6c 5a b0 3e");
            stockResponses.Enqueue("None");
            stockResponses.Enqueue("");
            stockResponses.Enqueue("08-09-2016 15:13:55.193");
            stockResponses.Enqueue(null);
            readerMock.Setup(t => t.ReadLine()).Returns(stockResponses.Dequeue);

            readerMock.SetupSequence(t => t.Peek()).Returns(5).Returns(-1);

            var parser = new Parser();
            var result = parser.ParsePackets(readerMock.Object);
           
            var expected = new Dictionary<Guid, Packet>();
            var packet1 = new Packet
            {
                IsError = true,
                DateRecieved = DateTime.ParseExact("08-09-2016 15:12:55.051", "dd-MM-yyyy HH:mm:ss.fff", null),
                Cargo =
                    @"01 00 fe fa 53 2d e5 81 d1 27 41 d5 e5 fe c6 67 05 54 dd 12 75 f0 86 e4 dd 6c 3f 71 49 2d 29 6c 73 99 66 78 45 83 c5 3b 9a ea a1 b4 45 e4 06 cf 54 d5 16 37 96 e4 ab 6c 5a b0 3e"
                        .Split(' ')
            };
           
            var packetId = Guid.NewGuid();
            expected.Add(packetId, packet1);

            var comparativePacket = (from a in result.Values
                                     where a.DateRecieved == packet1.DateRecieved
                                     select a).First();

            Assert.AreEqual(expected[packetId].Cargo.Length, comparativePacket.Cargo.Length);
            Assert.AreEqual(expected[packetId].Cargo[2], comparativePacket.Cargo[2]);
            Assert.AreEqual(expected[packetId].DateRecieved, comparativePacket.DateRecieved);
            Assert.AreEqual(expected[packetId].IsError, comparativePacket.IsError);
        }

        [TestMethod]
        public void PassingCargoWithAddressinFirstByteReturnsIndex()
        {
            var parser = new Parser();
            var cargoParam =
                @"57 01 4c 20 2d ff fa 00 00 02 00 00 00 00 08 12".Split(' ');
            var expected = 0;

            var actual = parser.GetLogicalAddressIndex(cargoParam);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PassingCargoWithAddressInThirdByteReturnsIndex()
        {
            var parser = new Parser();
            var cargoParam =
                @"01 00 fe fa 4b e7 5d b0 07 bd 57 83 82 ac ac 66 79 2e 35 d7 3d db 80 41 5e 98 2d d1 29 0b 98 85 6f 52 b9 86 88 f4 48 b7 98 17 24 8e 11 f4 f5 23 79 89 10 86 6e 8d 9c 56 e7 a9 f3 8f bc e5 4b a4 69 61 96 ce 0f 45 a7 a3 bb fa 95 ea eb 9b 64 ae 4e a6 e2 91 cc f8 d0 63 c6 f8 42 f3 9f 12 49 25 7b 67 ce 45 46 c2 c1 9b f9 ce 74 40 18 87 41 30 7f ef 73 9f 5f 02 64 91 84 eb e2 a5 d9 78 d7 31 2d cc 29 94 37 54 e0 c8 d8 fc 87 37 a1 a2 d4 d0 95 ca 8b 59 30 94 9f 07 a8 ec 9a 4c 72 01 40 f0 08 f8 6f 63 ea e1 4a 52 e2 ea 97 78 8d d2 64 b7 17 a1 f1 67 46 97 ca ee ba 62 34 90 73 94 ca 89 93 53 68 59 65 52 48 60 9f 01 6b aa e4 01 3b 0c 8d db 6e 70 a9 f1 2d 6d 42 b5 76 1b e3 18 bf 25 56 46 dc 1f b2 90 22 1a 96 a9 cd 76 af 16 9f f8 80 e0 ca 1c 62 8c 10 ad c9 4c 29 92 ca 77 65 eb".Split(' ');
            var expected = 2;

            var actual = parser.GetLogicalAddressIndex(cargoParam);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetPhysicalPathPortIndexesFromAddress()
        {
            var parser = new Parser();
            var cargoParam =
                @"00 04 fe fa 4b e7 5d b0 07 bd 57 83 82 ac ac 66 79 2e 35 d7 3d db 80 41 5e 98 2d d1 29 0b 98 85 6f 52 b9 86 88 f4 48 b7 98 17 24 8e 11 f4 f5 23 79 89 10 86 6e 8d 9c 56 e7 a9 f3 8f bc e5 4b a4 69 61 96 ce 0f 45 a7 a3 bb fa 95 ea eb 9b 64 ae 4e a6 e2 91 cc f8 d0 63 c6 f8 42 f3 9f 12 49 25 7b 67 ce 45 46 c2 c1 9b f9 ce 74 40 18 87 41 30 7f ef 73 9f 5f 02 64 91 84 eb e2 a5 d9 78 d7 31 2d cc 29 94 37 54 e0 c8 d8 fc 87 37 a1 a2 d4 d0 95 ca 8b 59 30 94 9f 07 a8 ec 9a 4c 72 01 40 f0 08 f8 6f 63 ea e1 4a 52 e2 ea 97 78 8d d2 64 b7 17 a1 f1 67 46 97 ca ee ba 62 34 90 73 94 ca 89 93 53 68 59 65 52 48 60 9f 01 6b aa e4 01 3b 0c 8d db 6e 70 a9 f1 2d 6d 42 b5 76 1b e3 18 bf 25 56 46 dc 1f b2 90 22 1a 96 a9 cd 76 af 16 9f f8 80 e0 ca 1c 62 8c 10 ad c9 4c 29 92 ca 77 65 eb".Split(' ');

            var logicalIndex = parser.GetLogicalAddressIndex(cargoParam);

            var addressArray = parser.GetAddressArray(logicalIndex, cargoParam);
            var expectedPathValues = new byte[]
            {
                (byte)Convert.ToInt32("00", 16),
                (byte)Convert.ToInt32("04", 16),
                (byte)Convert.ToInt32("fe", 16)
            };
            Assert.AreEqual(expectedPathValues[1], addressArray[1]);
        }

        [TestMethod]
        public void GetLogicalAddress()
        {
            var parser = new Parser();
            var cargoParam =
                @"57 01 4c 20 2d ff fb 00 00 02 00 00 00 00 08 3e".Split(' ');

            var logicalIndex = parser.GetLogicalAddressIndex(cargoParam);

            var physicalPathValues = parser.GetAddressArray(logicalIndex, cargoParam);
            var expectedPathValues = new byte[]
            {
                (byte)Convert.ToInt32("57", 16)
            };
            Assert.AreEqual(expectedPathValues[0], physicalPathValues[0]);
        }

        [TestMethod]
        public void GetCrcFromCargo()
        {
            var parser = new Parser();
            var cargoParam =
                @"57 01 4c 20 2d ff fb 00 00 02 00 00 00 00 08 3e".Split(' ');


            var expected = (byte)Convert.ToInt32("3e", 16);

            var actual = parser.GetCrc(cargoParam);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetProtocolIdFromCargo()
        {
            var parser = new Parser();
            var cargoParam =
                @"57 01 4c 20 2d ff fb 00 00 02 00 00 00 00 08 3e".Split(' ');
            var logicalIndex = parser.GetLogicalAddressIndex(cargoParam);

            var expected = 1;
            var actual = parser.GetProtocolId(cargoParam, logicalIndex);
            Assert.AreEqual(expected, actual);
        }
    }
}
