using GymSystem.BLL.Common;
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
        Task<Result<IEnumerable<MemberViewModel>>> GetAllMemberAysnc(CancellationToken ct=default);

        Task<Result> CreateMemberAsync(CreateMemberViewModel model , CancellationToken ct = default);

        Task<Result<MemberViewModel?>> GetMemberDetailsByIdAsync (int MemberID , CancellationToken ct=default); 

        Task<Result<HealthRecordViewModel?>> GetMemberHealthRecordAsync (int MemberID , CancellationToken ct=default);

        Task<Result<MemberToUpdateViewModel?>> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default);

        Task<Result> UpdateMemberDetailsAsync(int id , MemberToUpdateViewModel model , CancellationToken ct = default);

        Task<Result> RemoveMemberAsync (int memberId , CancellationToken ct=default);
    }
}
