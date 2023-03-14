
using System;
using System.Threading.Tasks;

namespace TestHook
{
    public class Hook
    {
        public string Trigger { get; set; } = "AdminGroupCreatedEvent";

        public async Task<object> Run(object data, dynamic localApi)
        {
            await localApi.UserService.MailAdminUser("congrats You triggered the Example hook", "You caused this again !");
            return $"{Trigger} Ran Sucessfully";
        }
    }
}
