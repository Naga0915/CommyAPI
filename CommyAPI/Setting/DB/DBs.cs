namespace CommyAPI.Setting.DB
{
    public static class DBs
    {
        public const string dbNameBBS = "commyBBS";
        public const string colNameBBS = "commyBBSThreads";
        public const string colNameBBSThreadPrefix = "commyBBSThread-";
        public const string conStrBBS = $"mognodb://commyBBS:7EDBE229-7E97-41E1-988D-A5927C3E15B0@localhost:27017?authSource={dbNameBBS}";
    }
}
