using Groubel.Data;
using GroubelNew.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroubelNew.BLL
{
    public class SuggestionsService
    {
        public Dictionary<string, Dictionary<string, List<SuggestionEntity>>> GetSuggestions(int userId)
        {
            using (var db = new groubel_dbEntities1())
            {
                var dat = new Dictionary<string, Dictionary<string, List<SuggestionEntity>>>();

                var ip = db.Users.FirstOrDefault(i => i.Id == userId).LastLoginIp;

                var users = GetSuggestedUsers(ip, userId, 0);
                dat.Add("1", users);
                var rooms = GetSuggestedRooms(userId, 0);
                dat.Add("2", rooms);
                var interest = GetSuggestedInterests(userId, 0, ip);
                dat.Add("3", interest);

                return dat;

            }
        }

        public Dictionary<string, List<SuggestionEntity>> GetSuggestedUsers(string ip, int userid, int ind)
        {
            using (var db = new groubel_dbEntities1())
            {
                var data = new Dictionary<string, List<SuggestionEntity>>();

                var suggestion = db.Users
                    .Where(i => i.Id != userid)
                    .OrderBy(i => Guid.NewGuid())
                    .Skip(ind)
                    .Take(4)
                    .Select(i => new SuggestionEntity
                    {
                        Id = i.Id,
                        Name = i.FirstName + " " + i.LastName,
                        Image = db.UserImages.FirstOrDefault(j => j.UserId == i.Id) != null ? db.UserImages.FirstOrDefault(j => j.UserId == i.Id).Image : ""
                    })
                        .ToList();

                data.Add("1", suggestion);

                var mutualInterests = db.Users.Where(i =>
                          db.UserInterests
                             .Where(t => t.UserId != userid &&
                                                   db.UserInterests
                                                      .Where(j => j.UserId == userid)
                                                      .Select(j => j.InterestId)
                                                      .Contains(t.InterestId)
                              )
                              .Select(k => k.UserId)
                              .Contains(i.Id)
                ).OrderByDescending(t => t.Id).Skip(ind)
                    .Take(4)
                    .Select(i => new SuggestionEntity
                    {
                        Id = i.Id,
                        Name = i.FirstName + " " + i.LastName,
                        Image = db.UserImages.FirstOrDefault(j => j.UserId == i.Id) != null ? db.UserImages.FirstOrDefault(j => j.UserId == i.Id).Image : ""
                    })
                        .ToList();

                data.Add("2", mutualInterests);

                var mutualRooms = db.Users.Where(i =>
                          db.ChatMembers
                             .Where(t => t.UserId != userid &&
                                                   db.ChatMembers
                                                      .Where(j => j.UserId == userid)
                                                      .Select(j => j.RoomId)
                                                      .Contains(t.RoomId)
                              )
                              .Select(k => k.UserId)
                              .Contains(i.Id)
                ).OrderByDescending(t => t.Id).Skip(ind)
                    .Take(4)
                    .Select(i => new SuggestionEntity
                    {
                        Id = i.Id,
                        Name = i.FirstName + " " + i.LastName,
                        Image = db.UserImages.FirstOrDefault(j => j.UserId == i.Id) != null ? db.UserImages.FirstOrDefault(j => j.UserId == i.Id).Image : ""
                    })
                        .ToList();

                data.Add("3", mutualRooms);

                var shrtIp = ip.Substring(0, ip.Length - 3);
                var nierby = db.Users.Where(i =>
                           i.LastLoginIp.StartsWith(shrtIp)
                ).OrderByDescending(t => t.Id).Skip(ind)
                    .Take(4)
                    .Select(i => new SuggestionEntity
                    {
                        Id = i.Id,
                        Name = i.FirstName + " " + i.LastName,
                        Image = db.UserImages.FirstOrDefault(j => j.UserId == i.Id) != null ? db.UserImages.FirstOrDefault(j => j.UserId == i.Id).Image : ""
                    })
                        .ToList();

                data.Add("4", nierby);

                return data;
            }
        }

        public Dictionary<string, List<SuggestionEntity>> GetSuggestedRooms(int userid, int ind)
        {

            using (var db = new groubel_dbEntities1())
            {
                var data = new Dictionary<string, List<SuggestionEntity>>();

                var sg = db.Chats.OrderBy(i => Guid.NewGuid()).Skip(ind).Take(2).Select(i => new SuggestionEntity
                {
                    Id = i.Id,
                    Name = i.Name,
                    Image = i.Image
                }).ToList();

                data.Add("1", sg);

                var mtInt = db.Chats.Where(i =>
                            db.ChatInterests
                                 .Where(j =>
                                      db.UserInterests
                                         .Where(t => t.UserId == userid)
                                         .Select(t => t.InterestId)
                                         .Contains(j.InterestId)
                                      )
                                      .Select(j => j.ChatId)
                                      .Contains(i.Id)
                ).OrderByDescending(t => t.Id).Skip(ind).Take(2).Select(i => new SuggestionEntity
                {
                    Id = i.Id,
                    Name = i.Name,
                    Image = i.Image
                }).ToList();

                data.Add("2", mtInt);

                var mtRms = db.Chats.Where(i =>
                            db.ChatMembers
                                 .Where(j =>
                                      db.ChatMembers
                                         .Where(t => t.UserId == userid)
                                         .Select(t => t.RoomId)
                                         .Contains(j.RoomId)
                                      )
                                      .Select(j => j.RoomId)
                                      .Contains(i.Id)
                ).OrderByDescending(t => t.Id).Skip(ind).Take(2).Select(i => new SuggestionEntity
                {
                    Id = i.Id,
                    Name = i.Name,
                    Image = i.Image
                }).ToList();

                data.Add("3", mtRms);

                var popRms = db.Chats.Where(i =>
                            db.ChatComments
                            .OrderByDescending(j => j.Id)
                            .Select(j => j.RoomId)
                                      .Contains(i.Id)
                ).OrderByDescending(t => t.Id).Skip(ind).Take(3).Select(i => new SuggestionEntity
                {
                    Id = i.Id,
                    Name = i.Name,
                    Image = i.Image
                }).ToList();

                data.Add("4", popRms);

                return data;
            }
        }

        public Dictionary<string, List<SuggestionEntity>> GetSuggestedInterests(int userid, int ind, string ip)
        {
            using (var db = new groubel_dbEntities1())
            {
                var data = new Dictionary<string, List<SuggestionEntity>>();

                var trending=db.Interests
                    .Where(j=>
                      db.PostInterests.OrderByDescending(i=>i.Id).Select(i=>i.InterestId).Contains(j.Id)
                    )
                    .OrderByDescending(t => t.Id)
                    .Skip(ind)
                    .Take(3)
                    .Select(i => new SuggestionEntity
                            {
                                Id = i.Id,
                                Name = i.Name,
                                Image = "",
                                IsMine=db.UserInterests.Where(t=>t.InterestId==i.Id).Select(t=>t.UserId).Contains(userid)
                            }).ToList();

                data.Add("1", trending);

                var lastWeek = DateTime.Now.AddDays(-7);

                var lastWeekTrending = db.Interests
                    .Where(j =>
                      db.PostInterests
                          .Where(u=>
                                   db.Posts
                                        .Where(e=>e.Date<lastWeek)
                                        .OrderByDescending(t => t.Id)
                                        .Skip(ind)
                                        .Take(100)
                                        .Select(r=>r.Id)
                                        .Contains(u.InterestId)
                                        )
                          .Select(i => i.InterestId)
                          .Contains(j.Id)
                    )
                    .OrderByDescending(t => t.Id)
                    .Skip(ind)
                    .Take(3)
                    .Select(i => new SuggestionEntity
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Image = ""
                    }).ToList();

                data.Add("2", lastWeekTrending);

                var popular = db.Interests
                    .Where(j =>
                      db.PostInterests.GroupBy(i=>i.InterestId).OrderByDescending(i => i.Key).Select(i => i.Key).Contains(j.Id)
                    )
                    .OrderByDescending(t => t.Id)
                    .Skip(ind)
                    .Take(3)
                    .Select(i => new SuggestionEntity
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Image = ""
                    }).ToList();

                data.Add("3", popular);

                var shrtIp = ip.Substring(0, ip.Length - 3);
                var nierby = db.Users.Where(i =>
                           i.LastLoginIp.StartsWith(shrtIp)).Select(i=>i.Id).ToList();

                var nearby = db.Interests
                   .Where(j =>
                      db.PostInterests
                          .Where(u =>
                                   db.Posts
                                        .Where(e => nierby.Contains(e.UserId))
                                        .Select(r => r.Id)
                                        .Contains(u.InterestId)
                                        )
                          .Select(i => i.InterestId)
                          .Contains(j.Id)
                    )
                   .OrderByDescending(t => t.Id)
                   .Skip(ind)
                   .Take(3)
                   .Select(i => new SuggestionEntity
                   {
                       Id = i.Id,
                       Name = i.Name,
                       Image = ""
                   }).ToList();

                data.Add("4", nearby);

                return data;
            }
        }
    }
}
