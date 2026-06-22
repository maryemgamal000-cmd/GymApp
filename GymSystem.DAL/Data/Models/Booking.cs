using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Data.Models
{
    // Junction table (Member >==< Session)
    public class Booking :BaseEntity
    {
      //Attributes
        public bool IsAttended { get; set; }





        //Relations
        #region Relations
        public Member Member { get; set; }
        public int MemberId { get; set; }



        public Session Session { get; set; }
        public int SessionId { get; set; }  
        #endregion

    }
}
