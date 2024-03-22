using MongoDB.Bson;

namespace CommyAPI.BBS
{
    public class BBSMessage
    {
        public ObjectId id;
        public string msgId;
        public long timeUTC;
        public string postedBy;
        public string message;
        
        public BBSMessage(string msgId, long timeUTC, string postedBy, string message)
        {
            this.msgId = msgId;
            this.timeUTC = timeUTC;
            this.postedBy = postedBy;
            this.message = message;
        }
    }
}
