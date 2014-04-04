namespace smb3rr
{

    struct DataGroup
    {

        // Offsets of actual level data
        // These will be placed into random map pointers in the randomized ROM
        public byte setData;
        public ushort enemyData;
        public ushort objectData;

    }

}
