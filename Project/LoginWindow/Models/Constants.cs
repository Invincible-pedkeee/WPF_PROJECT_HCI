using System.Collections.Generic;

namespace BasketballBallBrandsCMS.Models
{
    public class Constants
    {
        public static List<User> DefaultUsers = new List<User>()
        {
            new User("admin", "admin123", UserRole.Admin),
            new User("pelemaradona", "peroperic2004", UserRole.Visitor)
        };
    }
}