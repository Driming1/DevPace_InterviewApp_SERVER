using InterviewApp.Models.Shared.Base;
using InterviewApp.Models.Shared.Enums;

namespace InterviewApp.Models.Shared
{
    public class CustomerFilter : BaseFilter
    {
        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
#warning тут по задаче нужно уточнить нужен ли нам налл для первичного отображения всех данных
        public ActiveState? ActiveState { get; set; }
    }
}
