using Hangfire;
using ReactProjectsAuthApi.Data;

namespace ReactProjectsAuthApi
{
    public class HangfireJobs
    {
        private readonly ChatDbContext _ctx;



        public HangfireJobs(ChatDbContext ctx)
        {
            _ctx = ctx;
        }

        public void RemoveChats()
        {
            //RecurringJob.AddOrUpdate("RemoveMsgs", () => RemoveMessages(), Cron.Weekly);
        }

        public void RemoveMessages()
        {
            var msgs = _ctx.Messages.ToList();
            _ctx.Messages.RemoveRange(msgs);
            _ctx.SaveChanges();
        }
    }
}
