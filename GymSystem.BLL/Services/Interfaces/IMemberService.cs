using GymSystem.BLL.ViewModels.MemberViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMemberAysnc(CancellationToken ct=default);

        Task<bool> CreateMemberAsync(CreateMemberViewModel model , CancellationToken ct = default);
    }
}
