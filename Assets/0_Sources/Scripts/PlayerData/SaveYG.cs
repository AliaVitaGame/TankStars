namespace YG
{
    public partial class SavesYG
    {
        public int coins; 
        public int diamonds;

        public int killedEnemies;

        public int selectedSkinID;
        
        public int[] purchasedSkinIDs = new int[0];
        public bool[] receivedRewards = new bool[9999];
        public bool[] completedQuests = new bool[9999];

        public float musicVolume;
        public float sfxVolume;
    }
}