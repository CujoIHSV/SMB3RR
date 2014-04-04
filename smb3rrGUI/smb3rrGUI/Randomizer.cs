// SMB3RR Randomizer Class
// Copyright 2014 Tyler Mulholland
// Distributed under GNU GPL v3

using PRNG;
using smb3rrGUI;

using System;
using System.Collections.Generic;
using System.IO;

namespace smb3rr
{

    class Randomizer
    {

        private MersenneTwister mt;
        private string oldRomPath;
        private string newRomPath;
        private bool?[] levelFlags;
        private uint prngSeed;

        // We absolutely need a ROM file, so no default constructor
        private Randomizer() { }

        public Randomizer(string oldRomPath_, List<TreeViewModel> levelTree)
        {

            mt = new MersenneTwister();
            oldRomPath = oldRomPath_;
            prngSeed = mt.Seed;

            levelFlags = ExtractLevelFlags(levelTree);
            bool longSeed = !((levelTree[0].IsChecked == true) && (levelTree[1].IsChecked == true) && (levelTree[2].IsChecked == true));
            newRomPath = "smb3rr_" + GenerateFullSeed(longSeed) + ".nes";

        }

        public Randomizer(string oldRomPath_, string fullSeed)
        {

            oldRomPath = oldRomPath_;
            newRomPath = "smb3rr_" + fullSeed + ".nes";

            SeparateFullSeed(fullSeed);             // prngSeed and levelFlags are initialized here
            mt = new MersenneTwister(prngSeed);

        }

        public string CreateRom()
        {

            int flag = 0;

            // Get all the necessary level pointer ROM addresses and data location offsets for regular levels
            BinaryReader oldRom = new BinaryReader(File.OpenRead(oldRomPath));
            BinaryReader regAddressReader = new BinaryReader(File.OpenRead("regular.dat"));
            List<AddressGroup> regAddressGroups = new List<AddressGroup>();
            List<DataGroup> regDataGroups = new List<DataGroup>();
            while (regAddressReader.BaseStream.Position != regAddressReader.BaseStream.Length)
            {

                AddressGroup currentAGroup = new AddressGroup();
                currentAGroup.setPointer = regAddressReader.ReadUInt32();
                currentAGroup.enemyPointer = regAddressReader.ReadUInt32();
                currentAGroup.objectPointer = regAddressReader.ReadUInt32();

                DataGroup currentDGroup = new DataGroup();
                oldRom.BaseStream.Seek(currentAGroup.setPointer, SeekOrigin.Begin);
                currentDGroup.setData = (byte)oldRom.ReadUInt32();
                oldRom.BaseStream.Seek(currentAGroup.enemyPointer, SeekOrigin.Begin);
                currentDGroup.enemyData = (ushort)oldRom.ReadUInt32();
                oldRom.BaseStream.Seek(currentAGroup.objectPointer, SeekOrigin.Begin);
                currentDGroup.objectData = (ushort)oldRom.ReadUInt32();

                if (levelFlags[flag++] == true)
                {
                    regAddressGroups.Add(currentAGroup);
                    regDataGroups.Add(currentDGroup);
                }

            }

            regAddressReader.Close();

            mt.Shuffle(regDataGroups);

            // Repeat above for fortresses
            BinaryReader fortAddressReader = new BinaryReader(File.OpenRead("fort.dat"));
            List<AddressGroup> fortAddressGroups = new List<AddressGroup>();
            List<DataGroup> fortDataGroups = new List<DataGroup>();
            while (fortAddressReader.BaseStream.Position != fortAddressReader.BaseStream.Length)
            {

                AddressGroup currentAGroup = new AddressGroup();
                currentAGroup.setPointer = fortAddressReader.ReadUInt32();
                currentAGroup.enemyPointer = fortAddressReader.ReadUInt32();
                currentAGroup.objectPointer = fortAddressReader.ReadUInt32();

                DataGroup currentDGroup = new DataGroup();
                oldRom.BaseStream.Seek(currentAGroup.setPointer, SeekOrigin.Begin);
                currentDGroup.setData = (byte)oldRom.ReadUInt32();
                oldRom.BaseStream.Seek(currentAGroup.enemyPointer, SeekOrigin.Begin);
                currentDGroup.enemyData = (ushort)oldRom.ReadUInt32();
                oldRom.BaseStream.Seek(currentAGroup.objectPointer, SeekOrigin.Begin);
                currentDGroup.objectData = (ushort)oldRom.ReadUInt32();

                if (levelFlags[flag++] == true)
                {
                    fortAddressGroups.Add(currentAGroup);
                    fortDataGroups.Add(currentDGroup);
                }

            }

            fortAddressReader.Close();

            mt.Shuffle(fortDataGroups);

            // Repeat above for castles
            BinaryReader castleAddressReader = new BinaryReader(File.OpenRead("castle.dat"));
            List<AddressGroup> castleAddressGroups = new List<AddressGroup>();
            List<DataGroup> castleDataGroups = new List<DataGroup>();
            while (castleAddressReader.BaseStream.Position != castleAddressReader.BaseStream.Length)
            {

                AddressGroup currentAGroup = new AddressGroup();
                currentAGroup.setPointer = castleAddressReader.ReadUInt32();
                currentAGroup.enemyPointer = castleAddressReader.ReadUInt32();
                currentAGroup.objectPointer = castleAddressReader.ReadUInt32();

                DataGroup currentDGroup = new DataGroup();
                oldRom.BaseStream.Seek(currentAGroup.setPointer, SeekOrigin.Begin);
                currentDGroup.setData = (byte)oldRom.ReadUInt32();
                oldRom.BaseStream.Seek(currentAGroup.enemyPointer, SeekOrigin.Begin);
                currentDGroup.enemyData = (ushort)oldRom.ReadUInt32();
                oldRom.BaseStream.Seek(currentAGroup.objectPointer, SeekOrigin.Begin);
                currentDGroup.objectData = (ushort)oldRom.ReadUInt32();

                if (levelFlags[flag++] == true)
                {
                    castleAddressGroups.Add(currentAGroup);
                    castleDataGroups.Add(currentDGroup);
                }

            }

            castleAddressReader.Close();

            mt.Shuffle(castleDataGroups);

            // Combine all address and data groups
            List<AddressGroup> allAddressGroups = regAddressGroups;
            allAddressGroups.AddRange(fortAddressGroups);
            allAddressGroups.AddRange(castleAddressGroups);

            List<DataGroup> allDataGroups = regDataGroups;
            allDataGroups.AddRange(fortDataGroups);
            allDataGroups.AddRange(castleDataGroups);

            // New ROM = copy of old ROM with offsets at level pointers shuffled
            oldRom.BaseStream.Seek(0, SeekOrigin.Begin);
            BinaryWriter newRom = new BinaryWriter(File.OpenWrite(newRomPath));
            oldRom.BaseStream.CopyTo(newRom.BaseStream);
            newRom.Flush();
            for (int level = 0; level < allAddressGroups.Count; ++level)
            {

                // Set data offset is 4 bits wide and located in the lower nibble only -- Don't overwrite the upper nibble!
                oldRom.BaseStream.Seek(allAddressGroups[level].setPointer, SeekOrigin.Begin);
                newRom.BaseStream.Seek(allAddressGroups[level].setPointer, SeekOrigin.Begin);
                byte newSetByte = oldRom.ReadByte();
                newSetByte |= (byte)(allDataGroups[level].setData & 0x0f);
                newSetByte &= (byte)(allDataGroups[level].setData | 0xf0);
                newRom.Write(newSetByte);

                // Enemy data offset is 16 bits wide
                newRom.BaseStream.Seek(allAddressGroups[level].enemyPointer, SeekOrigin.Begin);
                newRom.Write(allDataGroups[level].enemyData);

                // Object data offset is 16 bits wide
                newRom.BaseStream.Seek(allAddressGroups[level].objectPointer, SeekOrigin.Begin);
                newRom.Write(allDataGroups[level].objectData);

            }

            oldRom.Close();
            newRom.Close();

            return newRomPath;

        }

        private static bool?[] ExtractLevelFlags(IEnumerable<TreeViewModel> levelTree)
        {

            List<bool?> flagList = new List<bool?>();
            foreach (TreeViewModel branch in levelTree)
            {

                if (branch.Children.Count == 0)
                {
                    flagList.Add(branch.IsChecked);
                }
                else
                {
                    flagList.AddRange(ExtractLevelFlags(branch.Children));
                }

            }

            return flagList.ToArray();

        }

        // The full "seed" is a string representing the PRNG seed and the level flags
        private string GenerateFullSeed(bool longSeed)
        {

            List<byte> seedData = new List<byte>();

            // 1st 4 bytes = PRNG seed, so we can reproduce the same sequence
            for (int i = 24; i >= 0; i -= 8)
            {
                seedData.Add((byte)~(prngSeed >> i));
            }

            // The other bytes = flags indicating which levels to randomize
            if (longSeed)
            {

                byte buffer = 0;
                int bufferLength = 0;
                foreach (bool? flag in levelFlags)
                {

                    buffer <<= 1;
                    if (flag == true)
                        ++buffer;
                    if (++bufferLength == 8)
                    {
                        seedData.Add((byte)~buffer);
                        bufferLength = 0;
                    }

                }
                if (bufferLength > 0)
                {
                    buffer <<= 8 - bufferLength;
                    seedData.Add((byte)~buffer);
                }

            }

            // Convert the full seed to a form of base64 that is valid for filenames
            string b64seed = Convert.ToBase64String(seedData.ToArray());
            b64seed = b64seed.Replace("=", null);
            b64seed = b64seed.Replace('/', '-');
            return b64seed;

        }

        private void SeparateFullSeed(string fullSeed)
        {

            // The full "seed" is a filename-safe, unpadded base64 string
            string workingSeed = fullSeed.Replace('-', '/');
            if (workingSeed.Length % 4 != 0)
                workingSeed = workingSeed.PadRight(workingSeed.Length + 4 - workingSeed.Length % 4, '=');
            byte[] fullSeedData = Convert.FromBase64String(workingSeed);

            // 1st 4 bytes are the actual PRNG seed
            prngSeed = 0;
            for (int i = 0; i < 4; ++i)
            {
                prngSeed += (uint)fullSeedData[i] << (8 * (3 - i));
            }
            prngSeed = ~prngSeed;

            // The rest of the bytes are the flags indicating which levels will be randomized
            if (fullSeed.Length > 6)
            {

                levelFlags = new bool?[8 * (fullSeedData.Length - 4)];
                for (int i = 0; i < levelFlags.Length; ++i)
                {
                    int bit = (fullSeedData[4 + i / 8] >> (7 - i % 8)) % 2;
                    levelFlags[i] = (bit == 0);
                }

            }
            else
            {

                levelFlags = new bool?[74];
                for (int i = 0; i < levelFlags.Length; ++i)
                {
                    levelFlags[i] = true;
                }

            }

        }

    }

}
