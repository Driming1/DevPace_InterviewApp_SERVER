using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewApp.Models.Shared
{
    public class EmailCheckRequest
    {
        public string Email { get; set; }
        public long? Id { get; set; }
    }
}
