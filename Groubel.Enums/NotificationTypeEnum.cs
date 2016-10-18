using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groubel.Enums
{
    public enum NotificationTypeEnum
    {
        PostAddedOnInterest=1,
        CmmentedOnYourPost=2,
        LikedYourPost=3,
        SharedYourPost=4,

        AddedYouInRoom=5,
        CommentedInRoom=6,
        ChangedYouStatusInRoom=7,
        RequestedAddToRoom=10,

        SendMessageInChat=8,
        AddEdYouAsFriend=9,

        RequestAddFriend=11
    }
}
