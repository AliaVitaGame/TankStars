namespace YG
{
    public partial class SavesYG
    {
        public int coins; 
        public int diamonds;

        public int killedEnemies;
        public int matchesPlayed;
        public int matchesWon;
        public int playTimeSeconds;
        public int coinsSpent;

        public int selectedSkinID;
        
        public int[] purchasedSkinIDs = new int[0];
        public bool[] receivedRewards = new bool[9999];
        public bool[] completedQuests = new bool[9999];
        public bool[] claimedQuests = new bool[64];

        public float musicVolume;
        public float sfxVolume;
    }
}
