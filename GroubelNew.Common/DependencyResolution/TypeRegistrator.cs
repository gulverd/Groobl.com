using GroubelNew.BLL;
using StructureMap;
using StructureMap.Graph;

namespace Groubel.Common.DependencyResolution
{
    public class TypeRegistrator: Registry
    {
        #region Constructor

        public TypeRegistrator()
        {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                });

            #region Custom Types Registration

            For<SecurityService>().Use<SecurityService>();
            For<UserService>().Use<UserService>();
            For<InterestsService>().Use<InterestsService>();
            For<PostsService>().Use<PostsService>();

            For<FriendsService>().Use<FriendsService>();

            For<RoomsService>().Use<RoomsService>();
            For<NotificationService>().Use<NotificationService>();
            For<SuggestionsService>().Use<SuggestionsService>();


            #endregion
        }

        #endregion
    }
    
}
