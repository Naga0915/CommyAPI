using Amazon.Runtime.Internal.Transform;
using CommyAPI.Authentication;
using CommyAPI.Setting.DB;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using SixLabors.ImageSharp.Processing;
using System.ComponentModel;

namespace CommyAPI.BBS
{
    public class BBSThread
    {
        public ObjectId id { get; set; }
        public string threadId { get; private set; }
        public string title { get; private set; }
        public DateTime created { get; private set; }
        public List<string> tags { get; private set; }
        public string? passwordHash { get; private set; }
        public bool IsClosed { get; private set; }

        private BBSThread(string threadId, string title, DateTime created, List<string> tags, string? passwordHash, bool isClosed)
        {
            this.created = created;
            this.threadId = threadId;
            this.title = title;
            this.tags = tags;
            this.passwordHash = passwordHash;
            this.IsClosed = isClosed;
        }

        public static async Task<BBSThread> GetThreadAsync(string id, string? password)
        {
            var client = new MongoClient(DBs.conStrBBS);
            var db = client.GetDatabase(DBs.dbNameBBS);
            var collection = db.GetCollection<BBSThread>(DBs.colNameBBS);
            var find = Builders<BBSThread>.Filter.Eq("id", id);
            using (var cursor = await collection.FindAsync(find))
            {
                var list = await cursor.ToListAsync();
                if (list.Count > 1) throw new BBSThreadException($"同じスレッドIDが2つ以上存在します。管理者に問い合わせてください。スレッドID:{id}");
                if (list.Count < 1) throw new BBSThreadException("スレッドが存在しないかアクセス権がありません。");
                var thread = list.First();
                if (thread.passwordHash != null)
                {
                    if (password == null)
                    {
                        throw new AuthGeneralException("スレッドが存在しないかアクセス権がありません。\"");
                    }
                    if (!AuthGeneral.Authenticate(thread.passwordHash, password))
                    {
                        throw new AuthGeneralException("パスワードが違います。");
                    }
                }
                return thread;
            }
        }

        public async Task<List<BBSMessage>> GetMessageAsync(int from, int count = 1)
        {
            var client = new MongoClient(DBs.conStrBBS);
            var db = client.GetDatabase(DBs.dbNameBBS);
            var collection = db.GetCollection<BBSMessage>(DBs.colNameBBSThreadPrefix + this.threadId);
            var sort = Builders<BBSMessage>.Sort.Ascending("timeUTC");
            var options = new FindOptions<BBSMessage>
            {
                Sort = sort,
                Skip = from - 1,
                Limit = count + 1
            };
            var filter = Builders<BBSMessage>.Filter.Empty;
            using(var cursor = await collection.FindAsync(filter, options))
            {
                return await cursor.ToListAsync();
            }
        }

        public async void InsertMessageAsync(string postedBy, string message)
        {
            string msgId = Guid.NewGuid().ToString();
            long timeUTC = DateTime.UtcNow.Ticks;
            //var msg = new BBSMessage();
        }

        public async void CloseThread()
        {

        }
    }

    public class BBSThreadException : Exception
    {
        public BBSThreadException(string? message) : base(message)
        {

        }
    }
}
