using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Data.Models
{
    // Junction table (Member >==< Plan)
    public class Membership : BaseEntity
    {
        //Attributes
        public DateTime EndDate { get; set; }

        //Calc attributes (get only - not in database[not mapping])
        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";
        public bool ISActive => EndDate > DateTime.Now;


        //Relations
        #region Relations
        public Member Member { get; set; }
        public int MemberId { get; set; }



        public Plan Plan { get; set; }
        public int PlanId { get; set; }  
        #endregion


    }
}
