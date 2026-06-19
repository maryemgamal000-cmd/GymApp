using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Data.Models
{
    public  class Member : GymUser
    { 
        public string ? Photo {  get; set; }

        //join date ==> Created At


        #region Relation
        public HealthRecord HealthRecord { get; set; } = default!;

        public ICollection<Membership> Memberships { get; set; }

        public ICollection<Booking> MemberSessions { get; set; }
        #endregion

    }
}
