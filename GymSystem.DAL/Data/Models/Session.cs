using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Data.Models
{
    public class Session : BaseEntity
    {

        public string Description { get; set; } = default!;

        public int Capacity { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }


        #region Relation
        public Trainer Trainer { get; set; }
        public int TrainerId { get; set; }  //FK



        public Category Category { get; set; }

        public int CategoryId { get; set; } //FK


        public ICollection<Booking> SessionMembers { get; set; }
        #endregion

    }
}
