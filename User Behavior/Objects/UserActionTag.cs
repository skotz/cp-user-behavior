namespace UserBehavior
{
    class UserActionTag
    {
        public int UserID { get; set; }

        public double[] ActionTagData { get; set; }

        public double Score { get; set; }

        public UserActionTag(int userId, int actionTagCount)
        {
            UserID = userId;
            ActionTagData = new double[actionTagCount];
        }
    }
}
