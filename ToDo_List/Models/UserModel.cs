using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDo_List.Models
{
    public class UserModel : IdentityUser
    {
        public ICollection<TaskModel> Tasks { get; set; }
    }
}
