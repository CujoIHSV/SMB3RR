namespace smb3rr
{

    struct AddressGroup
    {

        // ROM addresses of map pointers to level data
        // The pointers at these locations will be changed in the randomized ROM
        public uint setPointer;
        public uint enemyPointer;
        public uint objectPointer;

    }

}
